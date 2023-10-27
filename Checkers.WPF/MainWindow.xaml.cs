using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
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

        public MainWindow()
        {
            InitializeComponent();

            board = (Board) (Content as Grid).DataContext;
        }


        private void PieceClicked_Event(object sender, MouseButtonEventArgs e)
        {
            if (board.SelectedPiece == null)
            {

                var piece = (sender as Image).Tag as Piece;
                if (piece.Color == board.Opponent) return;
                var moves = piece.PossibleMoves(board).ToList();
                HighLightMoves(moves);
                board.SelectedPiece = piece;
            }
        }

        private void HighLightMoves(IEnumerable<IMove> moves)
        {
            foreach ((int row,int col) in moves.Where(m => m is NormalMove).Select(m => m.To))
            {
                board[row, col].IsHighLighted = true;
            }
        }

        private void SquareSelectedEvent(object sender, MouseButtonEventArgs e)
        {
            var selectedSquare = (sender as Rectangle).Tag as Square;
            if (board.IsCapturingMultiple || board.SelectedPiece != null)
            {
                var possibleMoves = board.SelectedPiece.PossibleMoves(board).ToList();
                var selectedMove =
                    possibleMoves.Where(m => m is NormalMove).SingleOrDefault(m => m.To == (selectedSquare.Row, selectedSquare.Column));
                var captureMoves = possibleMoves.Where(m => m is CaptureMove).Cast<CaptureMove>().ToList();
                var captureMove =captureMoves
                    .SingleOrDefault(m => m.To == (selectedSquare.Row, selectedSquare.Column));
                if (selectedMove != null)
                {
                    HideHighlightMoves(possibleMoves);
                    board.SelectedPiece.MoveTo(selectedMove.To);
                    board.SelectedPiece = null;
                    board.SwitchCurrentPlayer();
                }
                else if (captureMove != null)
                {
                        board.RemovePiece(captureMove.CapturedPiece);
                        board.SelectedPiece.MoveTo(captureMove.To);
                        if (captureMoves.Count == 1)
                        {
                            board.SelectedPiece = null;
                            board.SwitchCurrentPlayer();
                            board.IsCapturingMultiple = false;
                        }
                        else
                        {
                            board.IsCapturingMultiple = true;
                        }
                }
            }
        }

        private void HideHighlightMoves(IEnumerable<IMove> moves)
        {
            foreach ((int row,int col) in moves.Select(m => m.To))
            {
                board[row, col].IsHighLighted = false;
            }
        }
    }
}
