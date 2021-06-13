using ChessGameLib;
using EnumsLib;
using System;

namespace PiecesLib
{
    public class Rook : Piece
    {
        public Rook(ColorFigures color) : base(color) { }

        public override bool IsValidGameMove(Move move, ChessGame board)
        {
            if (move == null)
                throw new ArgumentNullException(nameof(move));

            if (board == null)
                throw new ArgumentNullException(nameof(board));

            return (move.GetAbsDeltaX() == 0 || move.GetAbsDeltaY() == 0) && !board.IsTherePieceInBetween(move.Source, move.Destination);
        }
    }
}
