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
            var piece = (sender as Image).Tag as Piece;
            if(piece != board.SelectedPiece)
            {
                UnhighLightMoves();
            }
            if (piece.Color == board.Opponent) return;
            var moves = piece.PossibleMovesNew(board).ToList();
            HighLightMoves(moves);
            board.SelectedPiece = piece;
        }

        private void HighLightMoves(IEnumerable<IMove> moves)
        {
            foreach(var move in moves)
            {
                if(move is NormalMove) 
                {
                    board[move.To.row, move.To.col].IsHighLighted = true;
                }
                else if(move is CaptureMove capt)
                {
                    var temp = capt;
                    while(temp.Next != null)
                    {
                        
                        temp = temp.Next;
                    }
                    board[temp.To.row, temp.To.col].IsHighLighted = true;
                }
            }
            
        }

        private void SquareSelectedEvent(object sender, MouseButtonEventArgs e)
        {
            var selectedSquare = (sender as Grid).Tag as Square;
            if (board.SelectedPiece != null)
            {
                var possibleMoves = board.SelectedPiece.PossibleMovesNew(board).ToList();
                var selectedMove =
                    possibleMoves.Where(m => m is NormalMove).FirstOrDefault(m => m.To == (selectedSquare.Row, selectedSquare.Column));
                var captureMoves = possibleMoves.Where(m => m is CaptureMove).Cast<CaptureMove>().ToList();
                var captureMove =captureMoves
                    .FirstOrDefault(m => m.Next == null ?  m.To == (selectedSquare.Row, selectedSquare.Column)
                    : m.GetFinalDestinationPos() == (selectedSquare.Row,selectedSquare.Column));
                if (selectedMove != null)
                {
                    board.SelectedPiece.MoveTo(selectedMove.To);
                    board.SelectedPiece = null;
                    board.SwitchCurrentPlayer();
                }
                else if (captureMove != null)
                {
                    foreach (var captMove in captureMoves)
                    {
                        var temp = captMove;
                        while(temp != null)
                        {
                            board.RemovePiece(temp.CapturedPiece);
                            temp = temp.Next;
                        } 
                    }

                    board.SelectedPiece.MoveTo(captureMove.GetFinalDestinationPos());
                    board.SelectedPiece = null;
                    board.SwitchCurrentPlayer();

                }
                UnhighLightMoves();
            }
        }

        private void HideHighlightMoves(IEnumerable<IMove> moves)
        {
            foreach ((int row,int col) in moves.Select(m => m.To))
            {
                board[row, col].IsHighLighted = false;
            }
        }

        private void UnhighLightMoves()
        {
            foreach(var square in board.Squares.Cast<Square>())
            {
                square.IsHighLighted = false;
            }
        }
    }
}
