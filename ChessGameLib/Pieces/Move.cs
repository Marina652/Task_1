using EnumsLib;
using InterfacesLib;
using SpaceDataLib;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PiecesLib
{
    public class Move : IDeepCloneable<Move>
    {
        public Square Source { get; }
        public Square Destination { get; }
        public ColorFigures ColorFigures { get; }
        public PawnPromotion? PromoteTo { get; }
        public Move DeepClone()
        {
            return new Move(Source, Destination, ColorFigures, PromoteTo);
        }

        public override bool Equals([NotNullWhen(true)] object? obj) =>
            obj is Move move &&
                move.Source == Source &&
                move.Destination == Destination; 

        public override int GetHashCode() => HashCode.Combine(Source, Destination, ColorFigures);
        public Move(Square source, Square destination, ColorFigures color, PawnPromotion? promoteTo = null) =>
                (Source, Destination, ColorFigures, PromoteTo) = (source, destination, color, promoteTo);

        public int GetAbsDeltaX() => Math.Abs(GetDeltaX());

        public int GetAbsDeltaY() => Math.Abs(GetDeltaY());

        public int GetDeltaX() => Destination.Letters - Source.Letters;

        public int GetDeltaY() => Destination.Rank - Source.Rank;
    }
}
