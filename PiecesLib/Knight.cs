
using EnumsLib;

namespace PiecesLib
{
    public class Knight : Piece
    {
        public Knight(ColorFigures color) : base(color) { }

        public override bool IsValidGameMove(Move move, ChessGame board)
        {
            int deltaX = move.GetAbsDeltaX();
            int deltaY = move.GetAbsDeltaY();
            return (deltaX == 1 && deltaY == 2) || (deltaX == 2 && deltaY == 1);
        }
    }
}
