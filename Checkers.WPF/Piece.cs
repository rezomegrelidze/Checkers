using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Documents;
using Checkers.WPF.Annotations;

using static Checkers.WPF.VectorExtensions;

namespace Checkers.WPF
{
    public class Piece : INotifyPropertyChanged
    {
        private int _row;
        private int _column;
        public PieceColor Color { get; private set; }

        public Piece(PieceColor color) => Color = color;

        public Vector Forward => Color == PieceColor.Black ? new Vector(0,-1) : new Vector(0,1);

        public Vector Backward => Color == PieceColor.Black ? new Vector(0, 1) : new Vector(0, -1);



        public bool IsQueened { get; set; }


        public int Row
        {
            get => _row;
            set
            {
                _row = value; OnPropertyChanged();
            }
        }

        public int Column
        {
            get => _column;
            set { _column = value; OnPropertyChanged(); }
        }

        public Vector PositionVector => new (Column,Row);

        public void PieceClicked()
        {

        }

        public string ImagePath { get; set; }

        public bool Kinged { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        IEnumerable<Vector> GetLegalDirections()
        {
            yield return Forward + Left;
            yield return Forward + Right;
            if (IsQueened)
            {
                yield return Backward + Left;
                yield return Backward + Right;
            }
        }

        public IEnumerable<IMove> PossibleMovesNew(Board board)
        {
            var options = GetLegalDirections().Select(dir => PositionVector + dir)
                .Where(pos => board.IsInBoard(pos)).ToArray();

            var result = new List<IEnumerable<IMove>>();
            foreach(var option in options)
            {
                var possibleMoves = GeneratePossibleMovesNew(option,board).ToList();
                result.Add(possibleMoves);
            }

            return result.SelectMany(l => l);
        }

        private IEnumerable<IMove> GeneratePossibleMovesNew(Vector dest, Board board)
        {
            if (!board.IsInBoard(dest)) yield break;
            if (board.IsOccupiedWithFriendly(dest)) yield break;

            if (!board.IsOccupiedWithOpponent(dest))
                yield return new NormalMove(this, PositionVector, dest);
            else
            {
                List<CaptureMove> captureMoves = new();
                var visited = new HashSet<Vector>();
                GetPossibleCaptureMovesNew(PositionVector, board, captureMoves, visited);

                foreach (var move in captureMoves)
                    yield return move;
            }
        }

        private void GetPossibleCaptureMovesNew(Vector dest, Board board, List<CaptureMove> captureMoves, HashSet<Vector> visited,CaptureMove prevCaptureMove = null)
        {
            if (!board.IsInBoard(dest)) return;
            if (visited.Contains(dest)) return;
            visited.Add(dest);
            var legalDirs = GetLegalDirections();

            var allAdjacents = legalDirs.Select(dir => (dest + dir,dir))
                .Where((tup,_) => board.IsInBoard(tup.Item1));

            foreach(var (adj,dir) in allAdjacents)
            {
                if(board.IsOccupiedWithOpponent(adj) && board.IsEmpty(adj + dir))
                {

                    var lastCapturedMove = captureMoves.LastOrDefault();
                    if(lastCapturedMove != null)
                    {
                        var allCapturedPieces = lastCapturedMove.GetAllCapturedPieces();
                        if (allCapturedPieces.Contains(board.GetPiece(adj)))
                            continue;
                    }
                    CaptureMove captureMove = new(this, board.GetPiece(adj), dest, adj + dir);

                    if (prevCaptureMove == null)
                    {
                        captureMoves.Add(captureMove);
                    }
                    else
                    {


                        prevCaptureMove.Next = captureMove;
                    }
                    GetPossibleCaptureMovesNew(adj+dir, board, captureMoves, visited,captureMove);
                }
            }
        }

        public void MoveTo((int row, int col) to)
        {
            Row = to.row;
            Column = to.col;


            if(IsQueenPosition(PositionVector))
            {
                IsQueened = true;
            }
        }

        private bool IsQueenPosition(Vector positionVector)
        {
            if (Color == PieceColor.Black) return positionVector.Y == 0;
            else return positionVector.Y == 7;
        }
    }

    public interface IMove
    {
        public (int row,int col) From { get; set; }
        public (int row,int col) To { get; set; }
    }

    public class NormalMove : IMove
    {
        private readonly Piece _movingPiece;

        public NormalMove(Piece movingPiece,Vector from,Vector to)
        {
            _movingPiece = movingPiece;
            From = ((int) from.Y,(int) from.X);
            To = ((int)to.Y,(int) to.X);
        }

        public (int row, int col) From { get; set; }
        public (int row, int col) To { get; set; }
    }

    public class CaptureMove : IMove
    {
        private readonly Piece _movingPiece;
        public Piece CapturedPiece { get; }

        public CaptureMove(Piece movingPiece,Piece capturedPiece,Vector from, Vector to)
        {
            _movingPiece = movingPiece;
            CapturedPiece = capturedPiece;
            From = ((int) from.Y,(int) from.X);
            To = ((int) to.Y,(int) to.X);
;        }
        

        public (int row, int col) From { get; set; }
        public (int row, int col) To { get; set; }

        public CaptureMove Next { get; set; }

        public (int row,int col) GetFinalDestinationPos()
        {
            if (Next == null) return To;
            else return Next.GetFinalDestinationPos();
        }

        public IEnumerable<Piece> GetAllCapturedPieces()
        {
            if(Next == null)
            {
                yield return CapturedPiece;
                yield break;
            }
            else
            {
                foreach(var capturedPiece in Next.GetAllCapturedPieces()) 
                    yield return capturedPiece;
            }
        }
    }

    public static class VectorExtensions
    {  
        public static Vector Left => new Vector(-1, 0);
        public static Vector Right => new Vector(1, 0);
    }
}