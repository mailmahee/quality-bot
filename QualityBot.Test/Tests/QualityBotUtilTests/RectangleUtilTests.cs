namespace QualityBot.Test.Tests.QualityBotUtilTests
{
    using System.Drawing;
    using NUnit.Framework;
    using QualityBot.Util;

    [TestFixture]
    class RectangleUtilTests
    {
        [Test]
        public void VerifyPositiveOrZeroCoordinates()
        {
            var rect = new Rectangle(-10,-10,10,10);
            Assert.IsTrue(RectangleUtil.PositiveOrZeroCoordinates(new Rectangle(0,0,0,0)));
            Assert.IsFalse(RectangleUtil.PositiveOrZeroCoordinates(rect));
        }
        [Test]
        public void VerifyAreaChangeAsPercent()
        {
            var rect1 = new Rectangle(0, 0, 0, 0);
            var rect2 = new Rectangle(0, 0, 0, 0);
            Assert.IsTrue(RectangleUtil.AreaChangeAsPercent(rect1, rect2) == 0);
            Assert.IsFalse(RectangleUtil.AreaChangeAsPercent(new Rectangle(1,1,10,10),rect2) == 0);
            Assert.Throws<System.DivideByZeroException>(() => RectangleUtil.AreaChangeAsPercent(rect1,new Rectangle(1, 1, 10, 10)));
        }
        [Test]
        public void VerifyAreaDifferenceBetweenRectangles()
        {
            var rect1 = new Rectangle(0, 0, 0, 0);
            var rect2 = new Rectangle(0, 0, 0, 0);
            Assert.IsTrue(RectangleUtil.AreaDifferenceBetweenRectangles(rect1,rect2) == 0);
            Assert.IsFalse(RectangleUtil.AreaDifferenceBetweenRectangles(rect1, new Rectangle(1, 1, 10, 10)) == 0);
            Assert.IsFalse(RectangleUtil.AreaDifferenceBetweenRectangles(new Rectangle(1, 1, 10, 10),rect2) == 0);
        }
        [Test]
        public void VerifyDistanceBetweenRectangles()
        {
            var rect1 = new Rectangle(0, 0, 0, 0);
            var rect2 = new Rectangle(0, 0, 0, 0);
            Assert.IsTrue(RectangleUtil.DistanceBetweenRectangles(rect1,rect2) == 0);
            Assert.IsFalse(RectangleUtil.DistanceBetweenRectangles(rect1, new Rectangle(1, 1, 10, 10)) == 0);
            Assert.IsFalse(RectangleUtil.DistanceBetweenRectangles(new Rectangle(1, 1, 10, 10),rect2) == 0);
        }
    }
}
