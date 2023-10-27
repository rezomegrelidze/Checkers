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
            var possibleMovesLeft = GeneratePossibleMoves(board, options[0],isLeft: true).ToList();
            var possibleMovesRight = GeneratePossibleMoves(board, options[1],isLeft: false).ToList();
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
                GetPossibleCaptureMoves(dest, board, captureMoves,isLeft);

                foreach (var move in captureMoves)
                    yield return move;
            }
        }


        private void GetPossibleCaptureMoves(Vector dest, Board board,List<CaptureMove> captureMoves,bool isLeft)
        {
            var hopOverPosition = dest + Forward + (isLeft ? Left : Right);
            if (board.IsInBoard(hopOverPosition) && board.IsEmpty(hopOverPosition))
            {
                captureMoves.Add(new CaptureMove(this,board.GetPiece(dest),PositionVector,hopOverPosition));

                GetPossibleCaptureMoves(hopOverPosition,board,captureMoves,true);
                
                GetPossibleCaptureMoves(hopOverPosition,board,captureMoves,false);
            }
            else
            {
                return;
            }
        }

        public void MoveTo((int row, int col) to)
        {
            Row = to.row;
            Column = to.col;
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