using EnumsLib;
using PiecesLib;
using SpaceDataLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessGameLib
{
    public static class HelperMethods
    {
        private static readonly IEnumerable<Square> s_allSquares =
            from letter in Enum.GetValues(typeof(Letters)).Cast<Letters>()
            from rank in Enum.GetValues(typeof(Rank)).Cast<Rank>()
            select new Square(letter, rank);

        internal static ColorFigures RevertPlayer(ColorFigures color) => color == ColorFigures.White ? ColorFigures.Black : ColorFigures.White;

        public static List<Move> GetValidMoves(ChessGame board)
        {
            _ = board ?? throw new ArgumentNullException(nameof(board));

            ColorFigures color = board.WhoseTurn;
            var validMoves = new List<Move>();

            IEnumerable<Square> playerOwnedSquares = s_allSquares.Where(sq => board[sq.Letters, sq.Rank]?.Color == color);
            Square[] nonPlayerOwnedSquares = s_allSquares.Where(sq => board[sq.Letters, sq.Rank]?.Color != color).ToArray(); 

            foreach (Square playerOwnedSquare in playerOwnedSquares)
            {
                validMoves.AddRange(nonPlayerOwnedSquares
                    .Select(nonPlayerOwnedSquare => new Move(playerOwnedSquare, nonPlayerOwnedSquare, color))
                    .Where(move => ChessGame.IsValidMove(move, board)));
            }
            return validMoves;
        }

        internal static bool IsPlayerInCheck(ColorFigures color, ChessGame board)
        {
            ColorFigures opponent = RevertPlayer(color);
            IEnumerable<Square> opponentOwnedSquares = s_allSquares.Where(sq => board[sq.Letters, sq.Rank]?.Color == opponent);
            Square playerKingSquare = s_allSquares.First(sq => new King(color).Equals(board[sq.Letters, sq.Rank]));

            return (from opponentOwnedSquare in opponentOwnedSquares
                    let piece = board[opponentOwnedSquare.Letters, opponentOwnedSquare.Rank]
                    let move = new Move(opponentOwnedSquare, playerKingSquare, opponent, PawnPromotion.Queen) 
                    where piece.IsValidGameMove(move, board)
                    select piece).Any();
        }
    }
}
