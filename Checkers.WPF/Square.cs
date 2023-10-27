namespace Checkers.WPF
{
    public class Square
    {
        public int Row { get; }
        public int Column { get; }

        public Square(int row, int col, SquareColor color) => (Row, Column, Color) = (row, col, color);

        public SquareColor Color { get; }
    }
}