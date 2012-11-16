namespace QualityBot.Util
{
    using HtmlAgilityPack;

    /// <summary>
    /// Contains useful XPath functionality.
    /// </summary>
    public static class XpathUtil
    {
        /// <summary>
        /// Parses the html for the XPath.
        /// </summary>
        /// <param name="html">The html to parse.</param>
        /// <param name="xpath">The XPath.</param>
        /// <returns>
        /// The <c>HtmlNodes</c> found.
        /// </returns>
        public static HtmlNode GetNode(HtmlDocument html, string xpath)
        {
            return html.DocumentNode.SelectSingleNode(xpath);
        }

        /// <summary>
        /// Creates an <c>HtmlDocument</c> with the html provided.
        /// </summary>
        /// <param name="html">The source.</param>
        /// <returns>
        /// An <c>HtmlDocument</c>.
        /// </returns>
        public static HtmlDocument GetHtmlDocument(string html)
        {
            if (html == null) return new HtmlDocument();
            var htmlDoc = new HtmlDocument { OptionFixNestedTags = true };
            HtmlNode.ElementsFlags.Remove("form");
            HtmlNode.ElementsFlags.Remove("option");
            htmlDoc.LoadHtml(html);
            return htmlDoc;
        }
    }
}