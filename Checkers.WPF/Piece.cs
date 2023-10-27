using System.ComponentModel;
using System.Runtime.CompilerServices;
using Checkers.WPF.Annotations;

namespace Checkers.WPF
{
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
}