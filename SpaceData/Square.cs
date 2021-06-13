using EnumsLib;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SpaceDataLib
{
    public struct Square : IEquatable<Square>
    {
        public Square(Letters letter, Rank rank)
        {
            Letters = letter;
            Rank = rank;
        }

        public static implicit operator Square(string s) => Parser.Parse(s);
        public static implicit operator Square(ReadOnlySpan<char> s) => Parser.Parse(s);

        public Letters Letters { get; }
        public Rank Rank { get; }

        public override bool Equals([NotNullWhen(true)] object? obj) =>
            obj is Square sq && sq.Letters == Letters && sq.Rank == Rank;

        public override int GetHashCode() => HashCode.Combine(Letters, Rank);
        public override string ToString() => $"{Letters}{(int)Rank + 1}";

        public bool Equals(Square other) => other.Letters == Letters && other.Rank == Rank; 
        public static bool operator ==(Square left, Square right) => left.Equals(right);
        public static bool operator !=(Square left, Square right) => !(left == right);
    }
}
