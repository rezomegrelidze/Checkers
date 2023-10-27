
using System;
using System.Reflection;
using System.Windows.Media;

public sealed class Board
{
    private Cell[,] boardMatrix;
    public Board()
    {
        boardMatrix = new Cell[8,8];
            PopulateBoard();
        }

        private void PopulateBoard()
        {
            for (int row = 0; row < boardMatrix.GetLength(0); row++)
            {
                for (int col = 0; col < boardMatrix.GetLength(1); col++)
                {
                    boardMatrix[row, col] = new Cell((row, col), (row & 1) == (col & 1) ?
                        CellColor.White : CellColor.Black);
                }
            }
        }

        public Cell this[int row, int col] => boardMatrix[row, col];

        public int RowsCount => boardMatrix.GetLength(0);
        public int ColumnsCount => boardMatrix.GetLength(1);
    }

    public static class Utility
    {
        public static Brush CellColorToBrush(CellColor color) => color switch
        {
            CellColor.Black => Brushes.Black,
            CellColor.White => Brushes.White,
            _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
        };

        public static string ResourcesUri
        {
            get
            {
                return System.IO.Path.Combine(Assembly.GetExecutingAssembly().Location, "../Resources/");
            }
        }
    }

    public class Cell
    {
        private Piece? _piece;
        public (int row,int col) Position { get; }

        public Cell((int row, int col) position, CellColor color)
        {
            Position = position;
            Color = color;
        }

        public CellColor Color { get; }

        public Piece? Piece
        {
            get => _piece;
            set
            {
                _piece = value;
                if (_piece != null) _piece.Position = Position;
            }
        }
    }

    public enum CellColor
    {
        Black,White
    }

    public class Piece
    {
        public PieceColor Color { get; private set; }

        public Piece(PieceColor color) => Color = color;
        public (int row,int col) Position { get; set; }
        public string ImagePath { get; set; }

        public bool Queened { get; set; }
    }

    public enum PieceColor
    {
        Red,Black
    }