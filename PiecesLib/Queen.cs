using ChessGameLib;
using EnumsLib;

namespace PiecesLib
{
    public class Queen : Piece
    {
        public Queen(ColorFigures color) : base(color) { }
        public override bool IsValidGameMove(Move move, ChessGame board)
        {
            return new Rook(move.ColorFigures).IsValidGameMove(move, board) ||
                   new Bishop(move.ColorFigures).IsValidGameMove(move, board);
        }
    }
}
