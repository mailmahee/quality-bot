namespace QualityBot.Test.Tests.QualityBotTests.ComparePocosTests
{
    using System.Collections.Generic;
    using System.Drawing;
    using NUnit.Framework;
    using QualityBot.ComparePocos;

    [TestFixture]
    class ElementChangeResultTests
    {
        public ElementChangeResult GetTestElementResult(int change = 0)
        {
            return new ElementChangeResult
                {
                    CssChanges =
                        new CssChange
                            {
                                Added = new Dictionary<string, string> { { "", "" } },
                                Changed = new List<CssChangeDetail> { new CssChangeDetail() },
                                Deleted = new Dictionary<string, string> { { "", "" } }
                            },
                    CssPercentageChange = change,
                    HtmlChanges = "",
                    HtmlPercentageChange = change,
                    LocationChanges = "",
                    LocationPercentageChange = change,
                    PixelChanges =
                        new PixelChange
                            {
                                Diff = new Bitmap(100, 100),
                                DiffStyle = "",
                                From = new Bitmap(100, 100),
                                FromClipped = new Bitmap(100, 100),
                                FromClippedStyle = "",
                                FromMask = new Bitmap(100, 100),
                                FromMaskStyle = "",
                                FromStyle = "",
                                To = new Bitmap(100, 100),
                                ToClipped = new Bitmap(100, 100),
                                ToClippedStyle = "",
                                ToMask = new Bitmap(100, 100),
                                ToMaskStyle = "",
                                ToStyle = ""
                            },
                    PixelPercentageChange = change,
                    TextChanges = "",
                    TextPercentageChange = change
                };
        }

        [Test]
        public void VerifyElementChangeEquals()
        {
            var e1  = GetTestElementResult();
            var e2 = GetTestElementResult();

            Assert.IsTrue(e1.Equals(e2));
        }

        [Test]
        public void VerifyElementChangeNotEqual()
        {
            var e1 = GetTestElementResult();
            var e2 = GetTestElementResult(1);

            Assert.IsFalse(e1.Equals(e2));
        }

        [Test]
        public void VerifyGetHashCode()
        {
            var e = GetTestElementResult();

            Assert.IsTrue(e.GetHashCode() != 0);
        }

        [Test]
        public void VerifyElementChangeEqualsOperator()
        {
            var e1 = GetTestElementResult();
            var e2 = GetTestElementResult();

            Assert.IsTrue(e1 == e2);
        }

        [Test]
        public void VerifyElementChangeNotEqualsOperator()
        {
            var e1 = GetTestElementResult();
            var e2 = GetTestElementResult(1);

            Assert.IsTrue(e1 != e2);
        }
    }
}
