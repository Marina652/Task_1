using ChessGameLib;
using EnumsLib;
using SpaceDataLib;

namespace PiecesLib
{
    public class King : Piece
    {
        public King(ColorFigures color) : base(color) { }
        public override bool IsValidGameMove(Move move, ChessGame board)
        {
            int absDeltaX = move.GetAbsDeltaX();
            int absDeltaY = move.GetAbsDeltaY();

            // Regular king move
            if (move.GetAbsDeltaX() <= 1 && move.GetAbsDeltaY() <= 1) return true;

            // Not castle move
            if (absDeltaX != 2 || absDeltaY != 0 || move.Source.Letters != Letters.E || 
                (move.ColorFigures == ColorFigures.White && move.Source.Rank != Rank.First) ||
                (move.ColorFigures == ColorFigures.Black && move.Source.Rank != Rank.Eighth) ||
                (board.GameState == GameState.BlackInCheck || board.GameState == GameState.WhiteInCheck))
                return false;

            // White king-side castle move
            if (move.ColorFigures == ColorFigures.White && move.Destination.Letters == Letters.G && board.CanWhiteCastleKingSide &&
                !board.IsTherePieceInBetween(move.Source, new Square(Letters.H, Rank.First)) &&
                new Rook(ColorFigures.White).Equals(board[Letters.H, Rank.First]))
                return !board.PlayerWillBeInCheck(
                       new Move(move.Source, new Square(Letters.F, Rank.First), move.ColorFigures));

            // Black king-side castle move
            if (move.ColorFigures == ColorFigures.Black && move.Destination.Letters == Letters.G && board.CanBlackCastleKingSide &&
                !board.IsTherePieceInBetween(move.Source, new Square(Letters.H, Rank.Eighth)) &&
                new Rook(ColorFigures.Black).Equals(board[Letters.H, Rank.Eighth]))
                return !board.PlayerWillBeInCheck(
                        new Move(move.Source, new Square(Letters.F, Rank.Eighth), move.ColorFigures));

            // White queen-side castle move
            if (move.ColorFigures == ColorFigures.White && move.Destination.Letters == Letters.C && board.CanWhiteCastleQueenSide &&
                !board.IsTherePieceInBetween(move.Source, new Square(Letters.A, Rank.First)) &&
                new Rook(ColorFigures.White).Equals(board[Letters.A, Rank.First]))
                return !board.PlayerWillBeInCheck(
                       new Move(move.Source, new Square(Letters.D, Rank.First), move.ColorFigures));

            // Black queen-side castle move
            if (move.ColorFigures == ColorFigures.Black && move.Destination.Letters == Letters.C && board.CanBlackCastleQueenSide &&
                !board.IsTherePieceInBetween(move.Source, new Square(Letters.A, Rank.Eighth)) &&
                new Rook(ColorFigures.Black).Equals(board[Letters.A, Rank.Eighth]))
                return !board.PlayerWillBeInCheck(
                        new Move(move.Source, new Square(Letters.D, Rank.Eighth), move.ColorFigures));
            return false;
        }
    }
}
