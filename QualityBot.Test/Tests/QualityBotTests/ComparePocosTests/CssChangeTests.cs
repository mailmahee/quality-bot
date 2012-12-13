namespace QualityBot.Test.Tests.QualityBotTests.ComparePocosTests
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using QualityBot.ComparePocos;

    [TestFixture]
    class CssChangeTests
    {
        private CssChange _cssA = new CssChange
        {
            Added = new Dictionary<string, string> {{"added", "value"}},
            Changed = new List<CssChangeDetail> { new CssChangeDetail() },
            Deleted = new Dictionary<string, string> { { "deleted", "value" } }
        };

        private CssChange _cssB = new CssChange
        {
            Added = new Dictionary<string, string> { { "added", "value" } },
            Changed = new List<CssChangeDetail> { new CssChangeDetail() },
            Deleted = new Dictionary<string, string> { { "deleted", "value" } }
        };

        [Test]
        public void VerifyCssChangeEquals()
        {
            Assert.IsTrue(_cssA.Equals(_cssB));
        }
        
        [Test]
        public void VerifyCssChangeEqualsOperator()
        {
            Assert.IsTrue(_cssA == _cssB);
        }

        [Test]
        public void VerifyCssChangeNotEqualsOperator()
        {
            Assert.IsFalse(_cssA != _cssB);
        }

        [Test]
        public void VerifyCssChangeGetHashCode()
        {
            Assert.IsTrue(_cssA.GetHashCode() != 0);
        }
    }
}
