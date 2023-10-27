using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Checkers.WPF
{
    public sealed class Board
    {
        public Square[,] BoardMatrix { get; }
        public ObservableCollection<Piece> Pieces { get; }

        public IEnumerable<Square> Squares => BoardMatrix.Cast<Square>();

        public Board()
        {
            BoardMatrix = new Square[8, 8];
            Pieces = new ObservableCollection<Piece>();
            PopulateBoard();
            PopulatePieces();
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

        public Square this[int row, int col] => BoardMatrix[row, col];

        public int RowsCount => BoardMatrix.GetLength(0);
        public int ColumnsCount => BoardMatrix.GetLength(1);
    }
}