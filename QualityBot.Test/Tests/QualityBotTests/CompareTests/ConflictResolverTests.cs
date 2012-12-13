namespace QualityBot.Test.Tests.QualityBotTests.CompareTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using QualityBot.Compare;

    [TestFixture]
    public class ConflictResolverTests
    {
        private decimal _threshold = 5M;

        private ConflictResolver<Element> _conflictResolver = new ConflictResolver<Element>();

        [Test]
        public void ConflictResolvesToNull()
        {
            // Arrange
            var seriesA = new[] { new Element(0), new Element(2), new Element(5) };
            var seriesB = new[] { new Element(1), new Element(5) };

            var elementMatches = FindCorrespondingElements(seriesA, seriesB);

            // Act
            _conflictResolver.ResolveAllConflicts(elementMatches.ToArray());
            SetResolvedMatches(elementMatches);

            // Assert
            Assert.IsTrue(seriesA.Where(e => e.Value == 2).All(e => e.Match == null));
        }

        [Test]
        public void ConflictResolvesToNextMatch()
        {
            // Arrange
            var seriesA = new[] { new Element(0), new Element(2), new Element(5) };
            var seriesB = new[] { new Element(1), new Element(4), new Element(5) };

            var elementMatches = FindCorrespondingElements(seriesA, seriesB);

            // Act
            _conflictResolver.ResolveAllConflicts(elementMatches.ToArray());
            SetResolvedMatches(elementMatches);

            // Assert
            Assert.IsTrue(seriesA.Where(e => e.Value == 2).All(e => e.Match.Value == 4));
        }

        private void SetResolvedMatches(IEnumerable<ElementMatch<Element>> elementMatches)
        {
            foreach (var element in elementMatches.Where(m => m.Match != null))
            {
                element.This.Match = element.Match.This;
                element.Match.This.Match = element.This;
            }
        }

        private List<ElementMatch<Element>> FindCorrespondingElements(IEnumerable<Element> elementsA, Element[] elementsB)
        {
            var elementMatches = new List<ElementMatch<Element>>();

            foreach (var element in elementsA)
            {
                Tuple<ElementMatch<Element>, decimal>[] matches;
                if (HasSimilarElements(element, elementsB, out matches))
                {
                    var elementMatch = new ElementMatch<Element>
                    {
                        This = element,
                        Matches = new Queue<Tuple<ElementMatch<Element>, decimal>>(matches)
                    };

                    elementMatch.SetToNext();
                    elementMatches.Add(elementMatch);
                }
            }

            return elementMatches;
        }

        private bool HasSimilarElements(Element element, IEnumerable<Element> elements, out Tuple<ElementMatch<Element>, decimal>[] similarElements)
        {
            var matchesOrderedBySimilarity = elements.Select(e => new Tuple<Element, decimal>(e, Math.Abs(e.Value - element.Value)))
                                                     .Where(e => e.Item2 < _threshold)
                                                     .OrderBy(e => e.Item2)
                                                     .ToArray();

            similarElements = matchesOrderedBySimilarity.Select(m => new Tuple<ElementMatch<Element>, decimal>(new ElementMatch<Element> { This = m.Item1 }, m.Item2)).ToArray();

            return matchesOrderedBySimilarity.Length > 0;
        }

        private class Element
        {
            public int Value { get; private set; }

            public Element Match { get; set; }

            public Element(int value)
            {
                Value = value;
            }
        }
    }
}