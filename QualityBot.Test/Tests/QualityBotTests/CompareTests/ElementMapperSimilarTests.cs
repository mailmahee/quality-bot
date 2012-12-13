namespace QualityBot.Test.Tests.QualityBotTests.CompareTests
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using Diff;
    using NUnit.Framework;
    using QualityBot.Compare;
    using QualityBot.ScrapePocos;

    [TestFixture]
    public class ElementMapperSimilarTests
    {
        private ElementMapper _elementMapper;

        private ScrapedElement[] _scrapedElements = new[]
            {
                new ScrapedElement
                    {
                        Attributes = new Dictionary<string, string> { { "foo", "bar" } },
                        Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque justo lorem, lacinia ac accumsan ut, auctor vel ligula. Fusce ut arcu purus. Proin id metus sit amet est venenatis auctor. Curabitur nunc elit, pretium at mattis luctus, molestie quis enim. Mauris eu ipsum a ligula auctor facilisis ac sed arcu. Vestibulum tristique lobortis nibh id blandit. Suspendisse nulla elit, dictum eget lobortis et, luctus at felis. Duis aliquet, quam lobortis congue rutrum, mauris libero posuere est, eu faucibus leo risus eu arcu. Nam viverra lobortis sem egestas fringilla.",
                        Tag = "div",
                        Location = new Rectangle(10, 10, 15, 15),
                    },
                new ScrapedElement
                    {
                        Attributes = new Dictionary<string, string> { { "foo", "bar" } },
                        Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque justo lorem, lacinia ac accumsan ut, auctor vel ligula. Fusce ut arcu purus. Proin id metus sit amet est venenatis auctor. Curabitur nunc elit, pretium at mattis luctus, molestie quis enim. Mauris eu ipsum a ligula auctor facilisis ac sed arcu. Vestibulum tristique lobortis nibh id blandit. Suspendisse nulla elit, dictum eget lobortis et, luctus at felis. Duis aliquet, quam lobortis congue rutrum, mauris libero posuere est, eu faucibus leo risus eu arcu. Nam viverra lobortis sem egestas.",
                        Tag = "div",
                        Location = new Rectangle(10, 10, 15, 15),
                    },
                new ScrapedElement
                    {
                        Attributes = new Dictionary<string, string> { { "foo", "bar" } },
                        Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque justo lorem, lacinia ac accumsan ut, auctor vel ligula. Fusce ut arcu purus. Proin id metus sit amet est venenatis auctor. Curabitur nunc elit, pretium at mattis luctus, molestie quis enim. Mauris eu ipsum a ligula auctor facilisis ac sed arcu. Vestibulum tristique lobortis nibh id blandit. Suspendisse nulla elit, dictum eget lobortis et, luctus at felis. Duis aliquet, quam lobortis congue rutrum, mauris libero posuere est, eu faucibus leo risus eu arcu. Nam viverra lobortis sem.",
                        Tag = "div",
                        Location = new Rectangle(10, 10, 15, 15),
                    }
            };

        public ElementMapperSimilarTests()
        {
            var diffEngine = new DiffMatchPatch();
            _elementMapper = new ElementMapper(diffEngine);
        }

        [Test]
        public void HasSimilarTextMatch()
        {
            // Arrange
            var scrapedElement = new ScrapedElement
            {
                Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque justo lorem, lacinia ac accumsan ut, auctor vel ligula. Fusce ut arcu purus. Proin id metus sit amet est venenatis auctor. Curabitur nunc elit, pretium at mattis luctus, molestie quis enim. Mauris eu ipsum a ligula auctor facilisis ac sed arcu. Vestibulum tristique lobortis nibh id blandit. Suspendisse nulla elit, dictum eget lobortis et, luctus at felis. Duis aliquet, quam lobortis congue rutrum, mauris libero posuere est, eu faucibus leo risus eu arcu. Nam viverra lobortis sem egestas fringilla.",
                Tag = "div",
                Location = new Rectangle(10, 10, 15, 15),
                Attributes = new Dictionary<string, string> { { "foo", "bar" } },
            };
            Tuple<ElementMatch<ScrapedElement>, decimal>[] matches;

            // Act
            var found = _elementMapper.HasSimilarElements(scrapedElement, _scrapedElements, 50M, out matches);

            // Assert
            Assert.IsTrue(found);
            Assert.IsTrue(ReferenceEquals(matches[0].Item1.This, _scrapedElements[0]));
            Assert.IsTrue(ReferenceEquals(matches[1].Item1.This, _scrapedElements[1]));
            Assert.IsTrue(ReferenceEquals(matches[2].Item1.This, _scrapedElements[2]));
        }
    }
}