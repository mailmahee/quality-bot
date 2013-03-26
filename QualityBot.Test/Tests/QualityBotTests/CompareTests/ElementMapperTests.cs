namespace QualityBot.Test.Tests.QualityBotTests.CompareTests
{
    using System.Collections.Generic;
    using System.Drawing;
    using Diff;
    using NUnit.Framework;
    using QualityBot.Compare;
    using QualityBot.ScrapePocos;

    [TestFixture]
    public class ElementMapperTests
    {
        private ElementMapper _elementMapper;

        private ScrapedElement[] _scrapedElements = new[]
            {
                new ScrapedElement
                    {
                        Attributes = new Dictionary<string, string> { { "id", "bar" } },
                        Css = new Dictionary<string, string> { { "foo", "bar" } },
                        Text = "bar",
                        Tag = "div",
                        Location = new Rectangle(10, 10, 15, 15),
                    },
                new ScrapedElement
                    {
                        Attributes = new Dictionary<string, string> { { "id", "foo" } },
                        Css = new Dictionary<string, string> { { "foo", "bar" } },
                        Text = "foo",
                        Tag = "div",
                        Location = new Rectangle(10, 10, 15, 15),
                    },
                new ScrapedElement
                    {
                        Attributes = new Dictionary<string, string> { { "id", "baz" } },
                        Css = new Dictionary<string, string> { { "foo", "bar" } },
                        Text = "baz",
                        Tag = "div",
                        Location = new Rectangle(10, 10, 15, 15),
                    }
            };

        public ElementMapperTests()
        {
            var diffEngine = new DiffMatchPatch();
            _elementMapper = new ElementMapper(diffEngine);
        }

        [Test, Category("Unit")]
        public void IdMatch()
        {
            // Arrange
            var scrapedElement = new ScrapedElement { Attributes = new Dictionary<string, string> { { "id", "foo" } }, };
            ScrapedElement match;

            // Act
            var found = _elementMapper.HasIdMatch(scrapedElement, _scrapedElements, out match);

            // Assert
            Assert.IsTrue(found);
            Assert.IsTrue(match.Attributes["id"] == "foo");
        }

        [Test, Category("Unit")]
        public void NoIdMatch()
        {
            // Arrange
            var scrapedElement = new ScrapedElement { Attributes = new Dictionary<string, string> { { "id", "foobar" } }, };
            ScrapedElement match;

            // Act
            var found = _elementMapper.HasIdMatch(scrapedElement, _scrapedElements, out match);

            // Assert
            Assert.IsFalse(found);
            Assert.IsNull(match);
        }

        [Test, Category("Unit")]
        public void ExactMatch()
        {
            // Arrange
            var scrapedElement = new ScrapedElement
                {
                    Attributes = new Dictionary<string, string> { { "id", "foo" } },
                    Css = new Dictionary<string, string> { { "foo", "bar" } },
                    Text = "foo",
                    Tag = "div",
                    Location = new Rectangle(10, 10, 15, 15),
                };
            ScrapedElement match;

            // Act
            var found = _elementMapper.HasExactMatch(scrapedElement, _scrapedElements, out match);

            // Assert
            Assert.IsTrue(found);
            Assert.AreEqual("foo", match.Text);
        }

        [Test, Category("Unit")]
        public void NoExactMatch()
        {
            // Arrange
            var scrapedElement = new ScrapedElement
                {
                    Text = "foo",
                    Tag = "span",
                    Location = new Rectangle(10, 10, 15, 15),
                    Attributes = new Dictionary<string, string> { { "id", "foo" } },
                    Css = new Dictionary<string, string> { { "foo", "bar" } },
                };
            ScrapedElement match;

            // Act
            var found = _elementMapper.HasExactMatch(scrapedElement, _scrapedElements, out match);

            // Assert
            Assert.IsFalse(found);
            Assert.IsNull(match);
        }

        [Test, Category("Unit")]
        public void MatchUnorderedHtmlAttributes()
        {
            // Arrange
            var scrapedElement = new ScrapedElement
                {
                    Html = @"<div id=""foo"" style=""bar"" class=""baz"">foo</div>",
                    Text = "foo",
                    Tag = "div",
                    Location = new Rectangle(10, 10, 15, 15),
                    Attributes = new Dictionary<string, string> { { "style", "bar" }, { "class", "baz" }, { "id", "foo" } },
                    Css = new Dictionary<string, string> { { "foo", "bar" } },
                };
            var scrapedElements = new[]
                {
                    new ScrapedElement
                        {
                            Html = @"<div id=""foo"" style=""bar"" class=""baz"">foo</div>",
                            Text = "foo",
                            Tag = "div",
                            Location = new Rectangle(10, 10, 15, 15),
                            Attributes = new Dictionary<string, string> { { "id", "foo" }, { "style", "bar" }, { "class", "baz" } },
                            Css = new Dictionary<string, string> { { "foo", "bar" } },
                        }
                };
            ScrapedElement match;

            // Act
            var found = _elementMapper.HasExactMatch(scrapedElement, scrapedElements, out match);

            // Assert
            Assert.IsTrue(found);
        }
    }
}