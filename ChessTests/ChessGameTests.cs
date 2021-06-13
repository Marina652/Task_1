using ChessGameLib;
using EnumsLib;
using NUnit.Framework;
using PiecesLib;
using SpaceDataLib;

namespace TestProject
{
    [TestFixture]
    public class ChessGameTests
    {
        [Test]
        public void MakeMove_Tests()
        {
            var game = new ChessGame();
            var sourse = new Square(Letters.A, Rank.Second);
            var destination = new Square(Letters.A, Rank.Third);
            var move = new Move(sourse, destination, ColorFigures.White);
            
            Assert.IsTrue(game.MakeMove(move, true));
        }

        [Test]
        public void IsValidMove_Tests()
        {
            var game = new ChessGame();
            var sourse = new Square(Letters.A, Rank.Second);
            var destination = new Square(Letters.A, Rank.Third);
            var move = new Move(sourse, destination, ColorFigures.White);

            var destinationTwo = new Square(Letters.A, Rank.Sixth);
            var moveTwo = new Move(sourse, destinationTwo, ColorFigures.White);
            Assert.IsTrue(game.IsValidMove(move));
            Assert.IsFalse(game.IsValidMove(moveTwo));
        }
    }
}
