using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Checkers.WPF.ValueConverters
{
    public class SquareColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var squareColor = (SquareColor) value;
            return squareColor == SquareColor.Black ? Brushes.Black : Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brush = value as Brush;
            return brush == Brushes.Black ? SquareColor.Black : SquareColor.White;
        }
    }
}