using EnumsLib;
using System;
using System.Globalization;
namespace SpaceDataLib
{
    internal class Parser
    {
        private static Letters ParseFile(char file)
        {
            file = char.ToUpper(file, CultureInfo.InvariantCulture);
            if (file < 'A' || file > 'H')
                throw new ArgumentOutOfRangeException(nameof(file));
            return (Letters)(file - 'A');
        }

        private static Rank ParseRank(char rank)
        {
            if (rank < '1' || rank > '8')
                throw new ArgumentOutOfRangeException(nameof(rank));
            return (Rank)(rank - '1');
        }

        public static Square Parse(string square)
        {
            _ = square ?? throw new ArgumentNullException(nameof(square));

            if (square.Length != 2)
                throw new ArgumentException("Argument length must be 2", nameof(square));

            Letters letter = ParseFile(square[0]);
            Rank rank = ParseRank(square[1]);

            return new Square(letter, rank);
        }

        public static Square Parse(ReadOnlySpan<char> square)
        {
            if (square.Length != 2)
                throw new ArgumentException("Argument length must be 2", nameof(square));

            Letters letter = ParseFile(square[0]);
            Rank rank = ParseRank(square[1]);

            return new Square(letter, rank);
        }
    }
}
