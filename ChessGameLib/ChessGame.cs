using EnumsLib;
using NLog;
using PiecesLib;
using SpaceDataLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessGameLib
{
    public class ChessGame : InterfacesLib.IDeepCloneable<ChessGame>
    {
        public Piece this[Letters letter, Rank rank] => Board[(int)rank][(int)letter];
        public List<Move> Moves { get; private set; }
        public Piece?[][] Board { get; private set; } 
        public ColorFigures WhoseTurn { get; private set; } = ColorFigures.White;
        public GameState GameState { get; private set; }
        internal bool CanWhiteCastleKingSide { get; set; } = true;
        internal bool CanWhiteCastleQueenSide { get; set; } = true;
        internal bool CanBlackCastleKingSide { get; set; } = true;
        internal bool CanBlackCastleQueenSide { get; set; } = true;
        private static readonly Logger moveLogger = LogManager.GetCurrentClassLogger();
        public ChessGame()
        {
            Moves = new List<Move>();
            var whitePawn = new Pawn(ColorFigures.White);
            var whiteRook = new Rook(ColorFigures.White);
            var whiteKnight = new Knight(ColorFigures.White);
            var whiteBishop = new Bishop(ColorFigures.White);
            var whiteQueen = new Queen(ColorFigures.White);
            var whiteKing = new King(ColorFigures.White);

            var blackPawn = new Pawn(ColorFigures.Black);
            var blackRook = new Rook(ColorFigures.Black);
            var blackKnight = new Knight(ColorFigures.Black);
            var blackBishop = new Bishop(ColorFigures.Black);
            var blackQueen = new Queen(ColorFigures.Black);
            var blackKing = new King(ColorFigures.Black);
            Board = new Piece?[][]
            {
                new Piece?[] { whiteRook, whiteKnight, whiteBishop, whiteQueen, whiteKing, whiteBishop, whiteKnight, whiteRook },
                new Piece?[] { whitePawn, whitePawn, whitePawn, whitePawn, whitePawn, whitePawn, whitePawn, whitePawn },
                new Piece?[] { null, null, null, null, null, null, null, null },
                new Piece?[] { null, null, null, null, null, null, null, null },
                new Piece?[] { null, null, null, null, null, null, null, null },
                new Piece?[] { null, null, null, null, null, null, null, null },
                new Piece?[] { blackPawn, blackPawn, blackPawn, blackPawn, blackPawn, blackPawn, blackPawn, blackPawn},
                new Piece?[] { blackRook, blackKnight, blackBishop, blackQueen, blackKing, blackBishop, blackKnight, blackRook}
            };
        }

        public bool MakeMove(Move move, bool isMoveValidated)
        {
            if (move == null)
                throw new ArgumentNullException(nameof(move));

            Piece? piece = this[move.Source.Letters, move.Source.Rank];
            if (piece == null)
                throw new InvalidOperationException("Source square has no piece.");

            if (!isMoveValidated && !IsValidMove(move))
                return false;

            SetCastleStatus(move, piece);

            if (piece is King && move.GetAbsDeltaX() == 2)
            {
                // Queen-side castle
                if (move.Destination.Letters == Letters.C)
                {
                    var rook = this[Letters.A, move.Source.Rank];
                    Board[(int)move.Source.Rank][(int)Letters.A] = null;
                    Board[(int)move.Source.Rank][(int)Letters.D] = rook;
                }

                // King-side castle
                if (move.Destination.Letters == Letters.G)
                {
                    var rook = this[Letters.H, move.Source.Rank];
                    Board[(int)move.Source.Rank][(int)Letters.H] = null;
                    Board[(int)move.Source.Rank][(int)Letters.F] = rook;
                }
            }

            if (piece is Pawn)
            {
                if ((move.ColorFigures == ColorFigures.White && move.Destination.Rank == Rank.Eighth) ||
                    (move.ColorFigures == ColorFigures.Black && move.Destination.Rank == Rank.First))
                {
                    piece = move.PromoteTo switch
                    {
                        PawnPromotion.Knight => new Knight(piece.Color),
                        PawnPromotion.Bishop => new Bishop(piece.Color),
                        PawnPromotion.Rook => new Rook(piece.Color),
                        PawnPromotion.Queen => new Queen(piece.Color),
                        _ => throw new ArgumentException($"A promotion move should have a valid {move.PromoteTo} property.", nameof(move)),
                    };
                }
                
                if (Pawn.GetPawnMoveType(move) == PawnMoveType.Capture && this[move.Destination.Letters, move.Destination.Rank] == null)
                    Board[(int)Moves.Last().Destination.Rank][(int)Moves.Last().Destination.Letters] = null;
            }
            Board[(int)move.Source.Rank][(int)move.Source.Letters] = null;
            Board[(int)move.Destination.Rank][(int)move.Destination.Letters] = piece;

            moveLogger.Info("Player: " + move.ColorFigures + "Sourse: " + move.Source + "Destination: " + move.Destination);
            
            Moves.Add(move);
            WhoseTurn = HelperMethods.RevertPlayer(move.ColorFigures);
            SetGameState();
            return true;
        }

        private void SetCastleStatus(Move move, Piece piece)
        {
            if (piece.Color == ColorFigures.White && piece is King)
            {
                CanWhiteCastleKingSide = false;
                CanWhiteCastleQueenSide = false;
            }

            if (piece.Color == ColorFigures.White && piece is Rook && move.Source.Letters == Letters.A && move.Source.Rank == Rank.First)
                CanWhiteCastleQueenSide = false;

            if (piece.Color == ColorFigures.White && piece is Rook && move.Source.Letters == Letters.H && move.Source.Rank == Rank.First)
                CanWhiteCastleKingSide = false;

            if (piece.Color == ColorFigures.Black && piece is King)
            {
                CanBlackCastleKingSide = false;
                CanBlackCastleQueenSide = false;
            }

            if (piece.Color == ColorFigures.Black && piece is Rook && move.Source.Letters == Letters.A && move.Source.Rank == Rank.Eighth)
                CanBlackCastleQueenSide = false;

            if (piece.Color == ColorFigures.Black && piece is Rook && move.Source.Letters == Letters.H && move.Source.Rank == Rank.Eighth)
                CanBlackCastleKingSide = false;
        }

        public bool IsValidMove(Move move)
        {
            if (move == null)
                throw new ArgumentNullException(nameof(move));

            Piece? pieceSource = this[move.Source.Letters, move.Source.Rank];
            Piece? pieceDestination = this[move.Destination.Letters, move.Destination.Rank];
            return (WhoseTurn == move.ColorFigures && pieceSource != null && pieceSource.Color == move.ColorFigures && 
                    !Equals(move.Source, move.Destination) && (pieceDestination == null || pieceDestination.Color != move.ColorFigures) &&
                    !PlayerWillBeInCheck(move) && pieceSource.IsValidGameMove(move, this));
        }

        internal static bool IsValidMove(Move move, ChessGame board)
        {
            if (move == null)
                throw new ArgumentNullException(nameof(move));

            Piece? pieceSource = board[move.Source.Letters, move.Source.Rank];
            Piece? pieceDestination = board[move.Destination.Letters, move.Destination.Rank];

            return (pieceSource != null && pieceSource.Color == move.ColorFigures && !Equals(move.Source, move.Destination) && (pieceDestination == null ||
                   pieceDestination.Color != move.ColorFigures) && !board.PlayerWillBeInCheck(move) && pieceSource.IsValidGameMove(move, board));
        }

        internal bool PlayerWillBeInCheck(Move move)
        {
            if (move == null)
                throw new ArgumentNullException(nameof(move));

            ChessGame clone = DeepClone(); 
            Piece? piece = clone[move.Source.Letters, move.Source.Rank]; 
            clone.Board[(int)move.Source.Rank][(int)move.Source.Letters] = null;
            clone.Board[(int)move.Destination.Rank][(int)move.Destination.Letters] = piece;

            return HelperMethods.IsPlayerInCheck(move.ColorFigures, clone);
        }

        internal void SetGameState()
        {
            ColorFigures opponent = WhoseTurn;
            ColorFigures lastPlayer = HelperMethods.RevertPlayer(opponent);
            bool isInCheck = HelperMethods.IsPlayerInCheck(opponent, this);
            var hasValidMoves = HelperMethods.GetValidMoves(this).Count > 0;

            if (isInCheck && !hasValidMoves)
            {
                GameState = lastPlayer == ColorFigures.White ? GameState.WhiteWinner : GameState.BlackWinner;
                return;
            }

            if (!hasValidMoves)
            {
                GameState = GameState.Stalemate;
                return;
            }

            if (isInCheck)
            {
                GameState = opponent == ColorFigures.White ? GameState.WhiteInCheck : GameState.BlackInCheck;
                return;
            }
            GameState = IsInsufficientMaterial() ? GameState.Draw : GameState.NotCompleted;
        }

        internal bool IsInsufficientMaterial() 
        {
            IEnumerable<Piece?> pieces = Board.SelectMany(x => x); 
            var whitePieces = pieces.Select((p, i) => new { Piece = p, SquareColor = (i % 8 + i / 8) % 2 })
                .Where(p => p.Piece?.Color == ColorFigures.White).ToArray();

            var blackPieces = pieces.Select((p, i) => new { Piece = p, SquareColor = (i % 8 + i / 8) % 2 })
                .Where(p => p.Piece?.Color == ColorFigures.Black).ToArray();

            switch (whitePieces.Length)
            {
                // King vs King
                case 1 when blackPieces.Length == 1:
                // White King vs black king and (Bishop|Knight)
                case 1 when blackPieces.Length == 2 && blackPieces.Any(p => p.Piece is Bishop || p.Piece is Knight):
                // Black King vs white king and (Bishop|Knight)
                case 2 when blackPieces.Length == 1 && whitePieces.Any(p => p.Piece is Bishop || p.Piece is Knight):
                    return true;
                // King and bishop vs king and bishop
                case 2 when blackPieces.Length == 2:
                    {
                        var whiteBishop = whitePieces.First(p => p.Piece is Bishop);
                        var blackBishop = blackPieces.First(p => p.Piece is Bishop);
                        return whiteBishop != null && blackBishop != null &&
                               whiteBishop.SquareColor == blackBishop.SquareColor;
                    }
                default:
                    return false;
            }
        }

        internal bool IsTherePieceInBetween(Square square1, Square square2)
        {
            int xStep = Math.Sign(square2.Letters - square1.Letters);
            int yStep = Math.Sign(square2.Rank - square1.Rank);

            Rank rank = square1.Rank;
            Letters letter = square1.Letters;
            while (true) 
            {
                rank += yStep;
                letter += xStep;
                if (rank == square2.Rank && letter == square2.Letters)
                    return false;

                if (Board[(int)rank][(int)letter] != null)
                    return true;
            }
        }

        public ChessGame DeepClone()
        {
            return new ChessGame
            {
                Board = new Piece?[][]
                {
                    new Piece?[] { Board[0][0], Board[0][1], Board[0][2], Board[0][3], Board[0][4], Board[0][5], Board[0][6], Board[0][7] },
                    new Piece?[] { Board[1][0], Board[1][1], Board[1][2], Board[1][3], Board[1][4], Board[1][5], Board[1][6], Board[1][7] },
                    new Piece?[] { Board[2][0], Board[2][1], Board[2][2], Board[2][3], Board[2][4], Board[2][5], Board[2][6], Board[2][7] },
                    new Piece?[] { Board[3][0], Board[3][1], Board[3][2], Board[3][3], Board[3][4], Board[3][5], Board[3][6], Board[3][7] },
                    new Piece?[] { Board[4][0], Board[4][1], Board[4][2], Board[4][3], Board[4][4], Board[4][5], Board[4][6], Board[4][7] },
                    new Piece?[] { Board[5][0], Board[5][1], Board[5][2], Board[5][3], Board[5][4], Board[5][5], Board[5][6], Board[5][7] },
                    new Piece?[] { Board[6][0], Board[6][1], Board[6][2], Board[6][3], Board[6][4], Board[6][5], Board[6][6], Board[6][7] },
                    new Piece?[] { Board[7][0], Board[7][1], Board[7][2], Board[7][3], Board[7][4], Board[7][5], Board[4][6], Board[7][7] },
                },
                Moves = Moves.Select(m => m.DeepClone()).ToList(),
                GameState = GameState,
                WhoseTurn = WhoseTurn,
                CanBlackCastleKingSide = CanBlackCastleKingSide,
                CanBlackCastleQueenSide = CanBlackCastleQueenSide,
                CanWhiteCastleKingSide = CanWhiteCastleKingSide,
                CanWhiteCastleQueenSide = CanWhiteCastleQueenSide
            };
        }
    }
}
