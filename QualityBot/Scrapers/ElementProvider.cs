﻿namespace QualityBot.Scrapers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using HtmlAgilityPack;
    using Newtonsoft.Json;
    using QualityBot.ScrapePocos;
    using QualityBot.Scrapers.Interfaces;
    using QualityBot.Util;

    public class ElementProvider : IElementProvider
    {
        private Rectangle _boundingRectangle;

        private Dictionary<string, object>[] _elements;

        private HtmlDocument _html;

        /// <summary>
        /// Loads the ScrapedElements.
        /// </summary>
        /// <returns>A yielded return of Type ScrapedElement.</returns>
        public IEnumerable<ScrapedElement> Elements()
        {
            for (var i = 0; i < _elements.Length; i++)
            {
                // Retrieves the html for the current element in the iteration
                HtmlNode node;

                if (!TryGetNode(i, out node)) continue;

                // If the element intersects with the bounding rectangle, then just take the intersection
                var rectangle = GetElementRectangle(_elements[i]);
                var elementLocation = Rectangle.Intersect(_boundingRectangle, rectangle);

                // If the element is outside the bounding rectangle, or has a width or height <= 0, then skip it
                if (!_boundingRectangle.Encompasses(elementLocation) || !RectangleUtil.PositiveDimensions(elementLocation)) continue;

                yield return new ScrapedElement
                {
                    Html = GetNodeHtml(node),
                    Text = GetElementText(_elements[i]),
                    Tag = GetNodeTag(node),
                    Css = GetElementCss(_elements[i]),
                    Attributes = GetNodeAttributes(node),
                    LocationOnScreenshot = elementLocation,
                    Location = rectangle
                };
            }
        }

        /// <summary>
        /// Loads the specified elements json.
        /// </summary>
        /// <param name="elementsJson">The elements json.</param>
        /// <param name="html">The HTML.</param>
        /// <param name="boundingRectangle">The bounding rectangle.</param>
        public void Load(string elementsJson, string html, Rectangle boundingRectangle)
        {
            _elements          = JsonConvert.DeserializeObject<Dictionary<string, object>[]>(elementsJson);
            _html              = XpathUtil.GetHtmlDocument(html);
            _boundingRectangle = boundingRectangle;
        }

        /// <summary>
        /// Dictionaries from json.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>An object of Type <c>Dictionary<String,String></c></returns>
        private static Dictionary<string, string> DictionaryFromJson(string json)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }

        /// <summary>
        /// Ints from object.
        /// </summary>
        /// <param name="number">The number object.</param>
        /// <returns>An Integer value.</returns>
        private static int IntFromObject(object number)
        {
            return (int)Math.Round(Decimal.Parse(number.ToString()), 0, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Returns a Rectangle object derived from a dynamic key value pair object.
        /// </summary>
        /// <param name="rectangle">The dynamic rectangle.</param>
        /// <returns>An object of type Rectangle.</returns>
        private static Rectangle RectangleFromDynamic(dynamic rectangle)
        {
            return new Rectangle(
                IntFromObject(rectangle["x"]),
                IntFromObject(rectangle["y"]),
                IntFromObject(rectangle["width"]),
                IntFromObject(rectangle["height"]));
        }

        /// <summary>
        /// Strings from dynamic.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>A String value.</returns>
        private static string StringFromDynamic(dynamic obj)
        {
            return (string)obj.ToString();
        }

        /// <summary>
        /// Gets the CSS element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>An object of Type <c>Dictionary<String,String></c></returns>
        private Dictionary<string, string> GetElementCss(Dictionary<string, object> element)
        {
            var cssJson = StringFromDynamic(element["css"]);
            var css = DictionaryFromJson(cssJson);

            return css;
        }

        /// <summary>
        /// Gets the element Rectangle.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>An object of Type Rectangle.</returns>
        private Rectangle GetElementRectangle(Dictionary<string, object> element)
        {
            var rectangle = RectangleFromDynamic(element);
            
            return rectangle;
        }

        /// <summary>
        /// Gets the element text.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>A String value.</returns>
        private string GetElementText(Dictionary<string, object> element)
        {
            var text = StringFromDynamic(element["text"]);

            return text;
        }

        /// <summary>
        /// Gets the node attributes.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>An object of Type <c>Dictionary<String,String></c></returns>
        private Dictionary<string, string> GetNodeAttributes(HtmlNode node)
        {
            var attributes = node.Attributes.ToDictionary(a => a.Name.ToLower(), a => Convert.ToString(a.Value));
            
            return attributes;
        }

        /// <summary>
        /// Gets the node HTML.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>A String value.</returns>
        private string GetNodeHtml(HtmlNode node)
        {
            return node.OuterHtml.Replace("&amp;", "&");
        }

        /// <summary>
        /// Gets the node tag.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>A String value.</returns>
        private string GetNodeTag(HtmlNode node)
        {
            return node.Name.ToLowerInvariant();
        }

        /// <summary>
        /// Tries to get the node.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="node">The node.</param>
        /// <returns>A boolean value.</returns>
        private bool TryGetNode(int index, out HtmlNode node)
        {
            var n = XpathUtil.GetNode(_html, string.Format("//*[@diffengineindexer='{0}']", index));
            node = null;

            if (n != null)
            {
                // Create a deep copy of the html
                node = n.CloneNode(true);

                // Remove the attribute set by the scraper (used for indexing)
                node.Attributes.Remove("diffengineindexer");

                // Discard child elements
                node.RemoveAllChildren();
            }

            return node != null;
        }
    }
}