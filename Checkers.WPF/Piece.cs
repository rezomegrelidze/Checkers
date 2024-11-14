using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
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

        public IEnumerable<IMove> PossibleMoves(Board board)
        {
            var options =
                new[] {PositionVector + Forward + Left, PositionVector + Forward + Right};

            if (IsQueened)
            {
                options = options.Concat(
                [
                    PositionVector + Backward + Left, PositionVector + Backward + Right
                ]).ToArray();
            }
            var possibleMovesLeft = GeneratePossibleMoves(board, options[0],isLeft: true).ToList();
            var possibleMovesRight = GeneratePossibleMoves(board, options[1],isLeft: false).ToList();
            if (IsQueened)
            {
                var possibleMovesLeftBack = GeneratePossibleMoves(board, options[2],isLeft: true).ToList();
                var possibleMovesRightBack = GeneratePossibleMoves(board, options[3],isLeft: false).ToList();
                return [..possibleMovesLeft,..possibleMovesRight,..possibleMovesLeftBack,..possibleMovesRightBack];
            }
            return possibleMovesLeft.Concat(possibleMovesRight);
        }

        private IEnumerable<IMove> GeneratePossibleMoves(Board board, Vector dest,bool isLeft)
        {
            if(!board.IsInBoard(dest)) yield break;

            if (board.IsOccupiedWithFriendly(dest)) yield break;

            if (!board.IsOccupiedWithOpponent(dest))
                yield return new NormalMove(this, PositionVector, dest);
            else
            {


                List<CaptureMove> captureMoves = new();
                var visited = new HashSet<Vector>();
                GetPossibleCaptureMoves(PositionVector,dest, board, captureMoves,isLeft,false, visited);

                foreach (var move in captureMoves)
                    yield return move;
            }
        }


        private void GetPossibleCaptureMoves(Vector src,Vector dest, Board board,List<CaptureMove> captureMoves,bool isLeft,bool isBack, HashSet<Vector> visited)
        {
            if (visited.Contains(src)) return;

            
            var hopOverPositionForward = dest + Forward + (isLeft ? Left : Right);
            var hopOverPositionBackward = dest + Backward + (isLeft ? Left : Right);



            visited.Add(src);

            


            if ((isBack || visited.Count == 1) && IsQueened && (board.IsInBoard(hopOverPositionBackward)) && board.IsEmpty(hopOverPositionBackward))
            {
                captureMoves.Add(new CaptureMove(this, board.GetPiece(dest), PositionVector, hopOverPositionBackward));

                
                if (isLeft && board.IsOccupiedWithOpponent(hopOverPositionBackward + Backward + Left))
                    GetPossibleCaptureMoves(dest, hopOverPositionBackward + Backward + Left, board, captureMoves, isLeft: true,isBack:true, visited);
                if (!isLeft && board.IsOccupiedWithOpponent(hopOverPositionBackward + Backward + Right))
                    GetPossibleCaptureMoves(dest, hopOverPositionBackward + Backward + Right, board, captureMoves, isLeft: false, isBack: true, visited);
            }


            if ((!isBack || visited.Count == 1) && board.IsInBoard(hopOverPositionForward) && board.IsEmpty(hopOverPositionForward))
            {
                captureMoves.Add(new CaptureMove(this,board.GetPiece(dest),PositionVector,hopOverPositionForward));


                    if (isLeft && board.IsOccupiedWithOpponent(hopOverPositionForward + Forward + Left))
                        GetPossibleCaptureMoves(dest, hopOverPositionForward + Forward + Left, board, captureMoves, isLeft: true, isBack: false, visited);
                    if (!isLeft && board.IsOccupiedWithOpponent(hopOverPositionForward + Forward + Right))
                        GetPossibleCaptureMoves(dest, hopOverPositionForward + Forward + Right, board, captureMoves, isLeft: false, isBack: false, visited);
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
    }

    public static class VectorExtensions
    {  
        public static Vector Left => new Vector(-1, 0);
        public static Vector Right => new Vector(1, 0);
    }
}