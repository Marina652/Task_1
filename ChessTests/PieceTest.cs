using ChessGameLib;
using EnumsLib;
using NUnit.Framework;
using PiecesLib;

namespace TestProject
{
    public class Tests
    {
        [TestFixture]
        public class PieceTests
        {
            [Test]
            public void PieceEquals_SameOwnerButDifferentPieceType_ReturnsFalse()
            {
                var game = new ChessGame();
                var p1 = game[Letters.A, Rank.First]; // White rook.
                Assert.True(p1 is Rook && p1.Color == ColorFigures.White);

                var p2 = game[Letters.B, Rank.First]; // White Knight
                Assert.True(p2 is Knight && p2.Color == ColorFigures.White);

                Assert.False (p1.Equals(p2));
                Assert.False(p2.Equals(p1));
            }

            [Test]
            public void PieceEquals_SameReference_ReturnsTrue()
            {
                var game = new ChessGame();
                var p1 = game[Letters.A, Rank.First]; // White rook.
                Assert.True(p1 is Rook && p1.Color == ColorFigures.White);

                Assert.True(ReferenceEquals(p1, p1));
                Assert.True(p1.Equals(p1));
            }

            [Test]
            public void PieceEquals_DifferentReferenceButSameOwnerAndType_ReturnsTrue()
            {
                var game = new ChessGame();
                var p1 = game[Letters.A, Rank.First]; // White rook.
                Assert.True(p1 is Rook && p1.Color == ColorFigures.White);

                game = new ChessGame();
                var p2 = game[Letters.A, Rank.First]; // White rook.
                Assert.True(p2 is Rook && p2.Color == ColorFigures.White);

                Assert.False(ReferenceEquals(p1, p2));
                Assert.True(p1.Equals(p2));
            }
        }
    }
}