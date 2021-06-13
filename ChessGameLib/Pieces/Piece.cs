using ChessGameLib;
using EnumsLib;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PiecesLib
{
    public abstract class Piece
    {
        public ColorFigures Color { get; }
        public abstract bool IsValidGameMove(Move move, ChessGame board);
        public override bool Equals([NotNullWhen(true)] object? obj) =>
               obj is Piece p && p.GetType() == GetType() && Color == p.Color;
        public override int GetHashCode() => HashCode.Combine(GetType(), Color);
        protected Piece(ColorFigures colorFigure) => Color = colorFigure;

    }
}
