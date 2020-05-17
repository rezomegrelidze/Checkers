using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
using Checkers.WPF.Annotations;
using Path = System.IO.Path;

namespace Checkers.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }

    public sealed class Board
    {
        public Square[,] BoardMatrix { get; }
        public ObservableCollection<Piece> Pieces { get; }

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
                    if (square.Color == SquareColor.White)
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

    public static class Utility
    {
        public static Brush CellColorToBrush(SquareColor color) => color switch
        {
            SquareColor.Black => Brushes.Black,
            SquareColor.White => Brushes.White,
            _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
        };

        public static string ResourcesUri => System.IO.Path.Combine(Assembly.GetExecutingAssembly().Location, "../Resources/");
    }

    public class Square
    {
        public int Row { get; }
        public int Column { get; }

        public Square(int row, int col, SquareColor color) => (Row, Column, Color) = (row, col, color);

        public SquareColor Color { get; }
    }

    public enum SquareColor
    {
        Black, White
    }

    public class Piece : INotifyPropertyChanged
    {
        private int _row;
        private int _column;
        public PieceColor Color { get; private set; }

        public Piece(PieceColor color) => Color = color;

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

        public string ImagePath { get; set; }

        public bool Kinged { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum PieceColor
    {
        Red, Black
    }

}
