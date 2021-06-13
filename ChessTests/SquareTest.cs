using EnumsLib;
using NUnit.Framework;
using SpaceDataLib;

namespace TestProject
{
    [TestFixture]
    public class SquareTest
    {
        [Test]
        public void SquareEquals_Tests()
        {
            var p1 = new Square(Letters.A, Rank.First);
            var p2 = new Square(Letters.A, Rank.Forth);
            var p3 = new Square(Letters.A, Rank.Forth);

            Assert.False(p1.Equals(p2));
            Assert.False(p2.Equals(p1));
            Assert.True(p2.Equals(p3));
        }
    }
}
