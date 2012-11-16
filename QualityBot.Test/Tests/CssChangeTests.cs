namespace QualityBot.Test.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NUnit.Framework;
    using QualityBot.ComparePocos;

    [TestFixture]
    class CssChangeTests
    {
        public CssChange GetTestCssChange(string change = null)
        {
            CssChange cssChanges;
            if(change == null)
            {
                cssChanges = new CssChange()
                {
                    Added = new Dictionary<string, string>() {{"", ""}},
                    Changed = new List<CssChangeDetail>() {new CssChangeDetail()},
                    Deleted = new Dictionary<string, string>() {{"", ""}}
                };
            }
            else
            {
                cssChanges = new CssChange()
                {
                    Added = new Dictionary<string, string>() { { change, change } },
                    Changed = new List<CssChangeDetail>() { new CssChangeDetail() },
                    Deleted = new Dictionary<string, string>() { { change, change } }
                };
            }
            return cssChanges;
        }

        [Test]
        public void VerifyEqualsChange()
        {
            //Arrange
            var cssChanges = GetTestCssChange();
            var cssChanges2 = GetTestCssChange();
            
            //Act
            var result = cssChanges.Equals(cssChanges2);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void VerifyEqualsObject()
        {
            //Arrange
            var cssChanges = GetTestCssChange();
            var cssChangesNot = GetTestCssChange("Changed");
            
            //Act
            var result = cssChanges.Equals(cssChangesNot);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void VerifyGetHashCode()
        {
            //Arrange
            var cssChanges = GetTestCssChange();

            //Act
            var result = cssChanges.GetHashCode();

            //Assert
            Assert.IsTrue(result != 0);
        }

        [Test]
        public void VerifyCssChangeEqualsOperator()
        {
            //Arrange
            var cssChanges = GetTestCssChange();
            var cssChanges2 = GetTestCssChange();

            //Act
            var result = cssChanges == cssChanges2;

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void VerifyCssChangeNotEqualsOperator()
        {
            //Arrange
            var cssChanges = GetTestCssChange();
            var cssChangesNot = GetTestCssChange("Changed");

            //Act
            var result = cssChanges != cssChangesNot;

            //Assert
            Assert.IsTrue(result);
        }
    }
}
