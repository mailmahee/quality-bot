using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using NUnit.Framework;
using QualityBot.ComparePocos;

namespace QualityBot.Test.Tests
{
    [TestFixture]
    class ElementChangeResultTests
    {
        public ElementChangeResult GetTestElementResult(int change = 0)
        {
            var eleChaRes = new ElementChangeResult();
            if (change == 0)
            {
                eleChaRes = new ElementChangeResult()
                {
                    CssChanges = new CssChange()
                    {
                        Added = new Dictionary<string, string>() {{"", ""}},
                        Changed = new List<CssChangeDetail>() {new CssChangeDetail()},
                        Deleted = new Dictionary<string, string>() {{"", ""}}
                    },
                    CssPercentageChange = 0,
                    HtmlChanges = "",
                    HtmlPercentageChange = 0,
                    LocationChanges = "",
                    LocationPercentageChange = 0,
                    PixelChanges = new PixelChange()
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
                    PixelPercentageChange = 0,
                    TextChanges = "",
                    TextPercentageChange = 0
                };
            }
            else
            {
                eleChaRes = new ElementChangeResult()
                {
                    CssChanges = new CssChange()
                    {
                        Added = new Dictionary<string, string>() { { "", "" } },
                        Changed = new List<CssChangeDetail>() { { new CssChangeDetail() } },
                        Deleted = new Dictionary<string, string>() { { "", "" } }
                    },
                    CssPercentageChange = change,
                    HtmlChanges = "",
                    HtmlPercentageChange = change,
                    LocationChanges = "",
                    LocationPercentageChange = change,
                    PixelChanges = new PixelChange()
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
            return eleChaRes;
        }

        [Test]
        public void VerifyElementChangeEquals()
        {
            var elementChange  = GetTestElementResult();
            var elementChange2 = GetTestElementResult();

            var result = elementChange.Equals(elementChange2);

            Assert.IsTrue(result);
        }

        [Test]
        public void VerifyElementChangeNotEqual()
        {
            var elementChange = GetTestElementResult();
            var elementChangeNot = GetTestElementResult(1);

            var result = elementChange.Equals((ElementChangeResult)elementChangeNot);

            Assert.IsFalse(result);
        }

        [Test]
        public void VerifyGetHashCode()
        {
            var elementChange = GetTestElementResult();

            var result = elementChange.GetHashCode();

            Assert.IsTrue(result != 0);
        }

        [Test]
        public void VerifyElementChangeEqualsOperator()
        {
            var elementChange = GetTestElementResult();
            var elementChange2 = GetTestElementResult();

            var result = elementChange == elementChange2;

            Assert.IsTrue(result);
        }

        [Test]
        public void VerifyElementChangeNotEqualsOperator()
        {
            var elementChange = GetTestElementResult();
            var elementChangeNot = GetTestElementResult(1);

            var result = elementChange != elementChangeNot;

            Assert.IsTrue(result);
        }
    }
}
