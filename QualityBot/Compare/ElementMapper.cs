namespace QualityBot.Compare
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Diff;
    using QualityBot.ComparePocos;
    using QualityBot.ScrapePocos;
    using QualityBot.Util;

    public class ElementMapper
    {
        private decimal _areaWeight      = 10;
        private decimal _distanceWeight  = 15;
        private decimal _tagWeight       = 25;
        private decimal _attributeWeight = 50;
        private decimal _textWeight      = 100;

        private decimal _maxAttributeDiffThreshold = 40;
        private decimal _maxTextDiffThreshold      = 15;

        private static string[] _listItemTags = new[] { "li", "dt", "dd" };
        private static string[] _listTags     = new[] { "ul", "ol", "dl" };
        private static string[] _parentTags   = new[] { "body", "div", "span", "table", "tr", "td", "th", "tbody", "thead", "tfoot", "form", "fieldset", "optgroup" };
        private static string[] _textTags     = new[] { "h1", "h2", "h3", "h4", "h5", "h6", "p", "strong", "em", "b", "i", "tt", "sub", "sup", "big", "small" };

        private DiffMatchPatch _diffEngine;

        public ElementMapper(DiffMatchPatch diffEngine)
        {
            _diffEngine = diffEngine;
        }

        public bool HasExactMatch(ScrapedElement element, IEnumerable<ScrapedElement> elements, out ScrapedElement exactMatch)
        {
            exactMatch = elements.FirstOrDefault(e => ElementsEqual(e, element));

            return exactMatch != null;
        }

        public bool HasIdMatch(ScrapedElement element, IEnumerable<ScrapedElement> elements, out ScrapedElement idMatch)
        {
            idMatch = elements.FirstOrDefault(e => IdsMatch(e, element));

            return idMatch != null;
        }

        public bool HasSimilarElements(ScrapedElement targetElement, IEnumerable<ScrapedElement> candidateElements, decimal maxDistance, out Tuple<ElementMatch<ScrapedElement>, decimal>[] similarElements)
        {
            var targetElementTextLength = targetElement.Text.Length;

            var candidates = GetElementsWithSimilarTag(targetElement.Tag, candidateElements);

            var tuples = (from c in candidates
                          let maxLength = Math.Max(c.Text.Length, targetElementTextLength)
                          let textDiff = maxLength.GetPercentageOfTotal(TextDistance(c.Text, targetElement.Text))
                          let attrDiff = AttributesSimilarity(targetElement, c)
                          where textDiff <= _maxTextDiffThreshold && attrDiff <= _maxAttributeDiffThreshold
                          select new Tuple<ScrapedElement, decimal>(c, (textDiff * _textWeight) + (attrDiff * _attributeWeight))).ToArray();

            tuples = (from c in tuples
                      let distRect = maxDistance.GetPercentageOfTotal(RectangleUtil.DistanceBetweenRectangles(c.Item1.Location, targetElement.Location))
                      let areaRect = RectangleUtil.AreaChangeAsPercent(c.Item1.Location, targetElement.Location)
                      let tagDist = c.Item1.Tag.Equals(targetElement.Tag) ? (100 * _tagWeight) : 0
                      let total = c.Item2 + tagDist + (distRect * _distanceWeight) + (areaRect * _areaWeight)
                      orderby total
                      select new Tuple<ScrapedElement, decimal>(c.Item1, total)).ToArray();

            similarElements = tuples.Select(t => new Tuple<ElementMatch<ScrapedElement>, decimal>(new ElementMatch<ScrapedElement> { This = t.Item1 }, t.Item2)).ToArray();

            return tuples.Length > 0;
        }

        private bool IdsMatch(ScrapedElement eA, ScrapedElement eB)
        {
            string idB;
            string idA;
            if (!eB.Attributes.TryGetValue("id", out idB)) return false;
            if (!eA.Attributes.TryGetValue("id", out idA)) return false;

            return idA == idB;
        }
        
        private static bool ElementsEqual(ScrapedElement e1, ScrapedElement e2)
        {
            /* Some comparisons are redundant - no need to check the html if we've checked
             *  the attributes and tag (and vice versa).  However, checking the tag and attributes
             *  is more accurate because order doesn't matter, whereas the html could have the
             *  attributes in any order.
             */
            var e1Null = ReferenceEquals(null, e1);
            var e2Null = ReferenceEquals(null, e2);

            if (e1Null && e2Null) return true;
            if (e1Null || e2Null) return false;

            return (e1.Tag == e2.Tag
                    && e1.Location.Equals(e2.Location)
                    && e1.Css.DictionaryEqual(e2.Css)
                    && e1.Attributes.DictionaryEqual(e2.Attributes)
                    && e1.Text == e2.Text);
        }

        /// <summary>
        /// Finds and returns elements with similar html tags.
        /// </summary>
        /// <param name="tag">The tag to match.</param>
        /// <param name="candidateElements">The elements that might contain a match.</param>
        /// <returns>A collection of matching elements.</returns>
        private static IEnumerable<ScrapedElement> GetElementsWithSimilarTag(string tag, IEnumerable<ScrapedElement> candidateElements)
        {
            var similarTags = GetSimilarTags(tag);

            return candidateElements.Where(cE => cE.Tag.In(similarTags));
        }

        /// <summary>
        /// Returns similar html tags.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        private static string[] GetSimilarTags(string tag)
        {
            if (tag.In(_parentTags)) return _parentTags;
            if (tag.In(_listTags)) return _listTags;
            if (tag.In(_listItemTags)) return _listItemTags;
            if (tag.In(_textTags)) return _textTags;

            return new[] { tag };
        }

        /// <summary>
        /// Determines the attribute similarity of two given attribute dictionaries.
        /// </summary>
        /// <param name="eA">The first element.</param>
        /// <param name="eB">The second element.</param>
        /// <returns>A decimal value.</returns>
        private decimal AttributesSimilarity(ScrapedElement eA, ScrapedElement eB)
        {
            var keysA = eA.Attributes.Select(k => k.Key).ToArray();
            var keysB = eB.Attributes.Select(k => k.Key).ToArray();
            var added = keysB.Except(keysA).ToArray();
            var deleted = keysA.Except(keysB).ToArray();
            var common = keysA.Except(deleted).ToArray();
            var changed = (from key in common
                           where eA.Attributes[key] != eB.Attributes[key]
                           select new AttributeChangeDetail { From = eA.Attributes[key], To = eB.Attributes[key], Key = key }).ToArray();

            var unchanged = common.Except(changed.Select(i => i.Key)).ToArray();

            var addedDeleted = (added.Length + added.Length) * 100;
            var changedPercentage = (from detail in changed
                                     let maxLength = Math.Max(detail.From.Length, detail.To.Length)
                                     let distance = TextDistance(detail.From, detail.To)
                                     select maxLength.GetPercentageOfTotal(distance)).Sum();

            var total = (unchanged.Length + changed.Length + deleted.Length + added.Length) * 100;
            var change = total.GetPercentageOfTotal(addedDeleted + changedPercentage);

            return change;
        }

        /// <summary>
        /// Computes the levenshtein distance between two strings.
        /// </summary>
        /// <param name="text1">The first string.</param>
        /// <param name="text2">The second string.</param>
        /// <returns>An integer value.</returns>
        private int TextDistance(string text1, string text2)
        {
            if (string.IsNullOrWhiteSpace(text1) && string.IsNullOrWhiteSpace(text2)) return 0;

            return _diffEngine.DiffLevenshtein(_diffEngine.DiffMain(text1, text2));
        }
    }
}