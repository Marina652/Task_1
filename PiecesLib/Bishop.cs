using EnumsLib;

namespace PiecesLib
{
    public class Bishop : Piece
    {
        public Bishop(ColorFigures color) : base(color) { }
        public override bool IsValidGameMove(Move move, ChessGame board)
        {
            return move.GetAbsDeltaX() == move.GetAbsDeltaY() && !board.IsTherePieceInBetween(move.Source, move.Destination);
        }
    }
}
