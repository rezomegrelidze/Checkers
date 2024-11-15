using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Checkers.WPF
{
    public class Board
    {
        public Square[,] BoardMatrix { get; }
        public ObservableCollection<Piece> Pieces { get; }

        public PieceColor CurrentPlayer { get; set; }

        public void SwitchCurrentPlayer() => CurrentPlayer = Opponent;

        public ObservableCollection<Square> Squares { get; set; }

        private void InitializeBoardMatrix(IEnumerable<Square> value)
        {

        }


        public Piece SelectedPiece = null;

        public Board()
        {
            BoardMatrix = new Square[8, 8];
            Pieces = new ObservableCollection<Piece>();
            PopulateBoard();
            PopulatePieces();
            CurrentPlayer = PieceColor.Black;
            Squares = new(BoardMatrix.Cast<Square>());
        }

        private void PopulatePieces()
        {
            for (int row = 0; row < BoardMatrix.GetLength(0); row++)
            {
                for (int col = 0; col < BoardMatrix.GetLength(1); col++)
                {
                    if (row > 2 && row < 5) continue; // ignore rows that shouldn't have initial pieces

                    var square = BoardMatrix[row, col];
                    if (square.Color == SquareColor.Black)
                    {
                        var pieceColor = square.Row < 3 ? PieceColor.Red : PieceColor.Black;
                        var piece = new Piece(pieceColor)
                        {
                            Row = row,
                            Column = col,
                            ImagePath = pieceColor == PieceColor.Black
                                ? Utility.ResourcesUri + "blackpiece.png"
                                : Utility.ResourcesUri + "redpiece.png"
                        };
                        Pieces.Add(piece);
                    }
                }
            }
        }

        private void PopulateBoard()
        {
            for (int row = 0; row < BoardMatrix.GetLength(0); row++)
            {
                for (int col = 0; col < BoardMatrix.GetLength(1); col++)
                {
                    BoardMatrix[row, col] = new Square(row, col, (row & 1) == (col & 1) ? SquareColor.White : SquareColor.Black);
                }
            }
        }

        public Square this[int row, int col] => 
            BoardMatrix[row, col];

        public int RowsCount => BoardMatrix.GetLength(0);
        public int ColumnsCount => BoardMatrix.GetLength(1);

        public bool IsOccupiedWithOpponent(Vector dest)
        {
            return Pieces.Any(p => p.Color == Opponent && p.PositionVector == dest);
        }

        public bool IsInBoard(Vector dest) => dest is { X : >= 0 and < 8 ,Y : >= 0 and < 8};

        public PieceColor Opponent => CurrentPlayer == PieceColor.Black ? PieceColor.Red : PieceColor.Black;
        
        public bool IsOccupiedWithFriendly(Vector dest)
        {
            return Pieces.Any(p => p.Color == CurrentPlayer && p.PositionVector == dest);
        }

        public bool IsEmpty(Vector dest)
        {
            return IsInBoard(dest) && Pieces.All(p => p.PositionVector != dest);
        }

        public Piece GetPiece(Vector dest)
        {
            return Pieces.SingleOrDefault(p => p.PositionVector == dest);
        }

        public void RemovePiece(Piece piece)
        {
            Pieces.Remove(piece);
        }
    }
}