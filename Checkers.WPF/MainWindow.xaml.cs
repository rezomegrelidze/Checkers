using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace Checkers.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Board board;
        private Border[,] borders;
        private bool gameStarted = false;
        private List<Border> possibleMoves;
        private Piece selectedPiece;

        public MainWindow()
        {
            InitializeComponent();

            board = new Board();
            borders = new Border[8,8];
            possibleMoves = new List<Border>();
            PopulateBoardGrid();
        }


        private void PopulateBoardGrid()
        {
            var grid = BoardGrid;
            for (int row = 0; row < board.RowsCount; row++)
            {
                for (int col = 0; col < board.ColumnsCount; col++)
                {
                    var cellBorder = new Border
                    {
                        Background = Utility.CellColorToBrush(board[row, col].Color),
                        Tag = board[row, col]
                    };

                    borders[row, col] = cellBorder;
                    Grid.SetRow(cellBorder,row);
                    Grid.SetColumn(cellBorder, col);
                    grid.Children.Add(cellBorder);

                    if (row < 3 && board[row, col].Color == CellColor.Black)
                    {
                        board[row,col].Piece = new Piece(PieceColor.Red)
                        {
                            ImagePath = Utility.ResourcesUri + "redpiece.png"
                        };
                        cellBorder.Child = new Image()
                        {
                            Source = new BitmapImage(new Uri((board[row, col].Piece.ImagePath))),
                            Tag = board[row,col].Piece
                        };
                    }

                    if (row > 4 && board[row, col].Color == CellColor.Black)
                    {
                        board[row, col].Piece = new Piece(PieceColor.Black)
                        {
                            ImagePath = Utility.ResourcesUri + "blackpiece.png"
                        };
                        cellBorder.Child = new Image()
                        {
                            Source = new BitmapImage(new Uri((board[row, col].Piece.ImagePath))),
                            Tag = board[row, col].Piece
                        };
                    }

                    cellBorder.MouseDown += CellBorderOnMouseDown;
                }
            }
        }

        private void CellBorderOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var cell = (sender as Border).Tag as Cell;
            if (selectedPiece != null)
            {
                if (possibleMoves.Any(b => b.Equals(sender)))
                {
                    MakeMove(cell);
                    EndMove();
                }
            }
            else if(cell.Piece != null)
            {
                selectedPiece = cell.Piece;
                HighLightMoves(cell);
            }
        }

        private void EndMove()
        {
            foreach (var border in possibleMoves)
                border.Background = Brushes.Black;
            selectedPiece = null;
        }

        private void MakeMove(Cell destinationCell)
        {
            var currentCell = board[selectedPiece.Position.row, selectedPiece.Position.col];
            destinationCell.Piece = currentCell.Piece;
            currentCell.Piece = null;
            UpdateCell(currentCell);
            UpdateCell(destinationCell);
        }

        private void UpdateCell(Cell cell)
        {
            var border = borders[cell.Position.row, cell.Position.col];
            if (cell.Piece != null)
            {
                border.Child = new Image()
                {
                    Source = new BitmapImage(new Uri((cell.Piece.ImagePath))),
                    Tag = cell.Piece
                }; 
            }
            else
            {
                border.Child = null;
            }
        }

        private void HighLightMoves(Cell cell)
        {
            var piece = cell.Piece;
            var cellsToHighLight = new Stack<Border>();
            var piecePos = cell.Position;
            if (piece.Color == PieceColor.Red)
            {
                if (cell.Position.col == 0)
                {
                    cellsToHighLight.Push(borders[cell.Position.row+1, cell.Position.col + 1]);
                }
                else if (cell.Position.col == 7)
                {
                    cellsToHighLight.Push(borders[ cell.Position.row + 1, cell.Position.col - 1]);
                }
                else
                {
                    cellsToHighLight.Push(borders[ cell.Position.row + 1, cell.Position.col - 1]);
                    cellsToHighLight.Push(borders[cell.Position.row + 1, cell.Position.col + 1]);
                }
            }
            else
            {
                if (cell.Position.col == 0)
                {
                    cellsToHighLight.Push(borders[cell.Position.row - 1, cell.Position.col + 1]);
                }
                else if (cell.Position.col == 7)
                {
                    cellsToHighLight.Push(borders[cell.Position.row - 1, cell.Position.col - 1]);
                }
                else
                {
                    cellsToHighLight.Push(borders[cell.Position.row - 1, cell.Position.col - 1]);
                    cellsToHighLight.Push(borders[cell.Position.row - 1, cell.Position.col + 1]);
                }
            }

            possibleMoves.Clear();
            possibleMoves.AddRange(
                cellsToHighLight.Where(border => (border.Tag as Cell).Piece == null || (border.Tag as Cell).Piece.Color != piece.Color));

            HighLightBorders();
        }

        private void HighLightBorders()
        {
            foreach (var border in possibleMoves)
            {
                border.Background = Brushes.Yellow;
            }
        }
    }

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
                    boardMatrix[row, col] = new Cell((row, col), (row & 1) == (col & 1) ? CellColor.White : CellColor.Black);
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

        public bool Kinged { get; set; }
    }

    public enum PieceColor
    {
        Red,Black
    }
}
