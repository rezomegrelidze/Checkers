using System;
using System.Reflection;
using System.Windows.Media;

namespace Checkers.WPF
{
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
}