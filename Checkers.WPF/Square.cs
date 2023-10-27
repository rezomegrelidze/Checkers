using System.ComponentModel;
using System.Runtime.CompilerServices;
using Checkers.WPF.Annotations;

namespace Checkers.WPF
{
    public class Square : INotifyPropertyChanged
    {
        private bool _isHighLighted;
        public int Row { get; }
        public int Column { get; }

        public Square(int row, int col, SquareColor color) => (Row, Column, Color) = (row, col, color);

        public bool IsHighLighted
        {
            get => _isHighLighted;
            set
            {
                if (value == _isHighLighted) return;
                _isHighLighted = value;
                OnPropertyChanged();
            }
        }

        public SquareColor Color { get; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}