using ChessGameLib;
using EnumsLib;
using System;
using System.Linq;
using InterfacesLib;

namespace PiecesLib
{
    public class Pawn : Piece
    {
        public Pawn(ColorFigures color) : base(color) { }

         public static PawnMoveType GetPawnMoveType(Move move)
        {
            int deltaY = move.GetDeltaY();
            int absDeltaX = move.GetAbsDeltaX();

            // Check one step pawn move
            if ((move.ColorFigures == ColorFigures.White && deltaY == 1 && absDeltaX == 0) ||
                move.ColorFigures == ColorFigures.Black && deltaY == -1 && absDeltaX == 0)
            {
                if ((move.ColorFigures == ColorFigures.White && move.Destination.Rank == Rank.Eighth) ||
                    (move.ColorFigures == ColorFigures.Black && move.Destination.Rank == Rank.First))
                    return PawnMoveType.OneStep | PawnMoveType.Promotion;
                return PawnMoveType.OneStep;
            }

            // Check two step move from starting position
            if ((move.ColorFigures == ColorFigures.White && deltaY == 2 && absDeltaX == 0 && move.Source.Rank == Rank.Second) ||
                (move.ColorFigures == ColorFigures.Black && deltaY == -2 && absDeltaX == 0 && move.Source.Rank == Rank.Seventh))
                return PawnMoveType.TwoSteps;

            // Check capture (Enpassant is special case from capture)
            if ((move.ColorFigures == ColorFigures.White && deltaY == 1 && absDeltaX == 1) ||
                (move.ColorFigures == ColorFigures.Black && deltaY == -1 && absDeltaX == 1))
            {
                if ((move.ColorFigures == ColorFigures.White && move.Destination.Rank == Rank.Eighth) ||
                    (move.ColorFigures == ColorFigures.Black && move.Destination.Rank == Rank.First))
                    return PawnMoveType.Capture | PawnMoveType.Promotion;
                return PawnMoveType.Capture;
            }
            return PawnMoveType.Invalid;
        }

        public override bool IsValidGameMove(Move move, ChessGame board)
        {
            var moveType = GetPawnMoveType(move);

            if (moveType == PawnMoveType.Invalid)
                return false;

            if (moveType.Contains(PawnMoveType.Promotion) && move.PromoteTo == null)
                return false;

            if (moveType.Contains(PawnMoveType.TwoSteps))
                return !board.IsTherePieceInBetween(move.Source, move.Destination) && board[move.Destination.Letters, move.Destination.Rank] == null;

            if (moveType.Contains(PawnMoveType.OneStep))
                return board[move.Destination.Letters, move.Destination.Rank] == null;
         
            if (moveType.Contains(PawnMoveType.Capture))
            {
                if (board.Moves.Count == 0)
                    return false;

                // Check regular capture.
                if (board[move.Destination.Letters, move.Destination.Rank] != null)
                    return true;

                // Check enpassant.
                Move lastMove = board.Moves.Last();
                Piece? lastMovedPiece = board[lastMove.Destination.Letters, lastMove.Destination.Rank];

                if (lastMovedPiece is Pawn || !GetPawnMoveType(lastMove).Contains(PawnMoveType.TwoSteps) || 
                    lastMove.Destination.Letters != move.Destination.Letters || lastMove.Destination.Rank != move.Source.Rank)
                    return false;

                ChessGame clone = board.DeepClone();
                clone.Board[(int)move.Destination.Rank][(int)move.Destination.Letters] = null;
                clone.Board[((int)move.Destination.Rank + (int)move.Source.Rank) / 2][(int)move.Destination.Letters] = lastMovedPiece;
                return !clone.PlayerWillBeInCheck(move);
            }
            throw new Exception("Unexpected PawnMoveType"); 
        }
    }
}
