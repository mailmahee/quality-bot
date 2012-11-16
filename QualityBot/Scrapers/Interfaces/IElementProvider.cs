namespace QualityBot.Scrapers.Interfaces
{
    using System.Collections.Generic;
    using System.Drawing;
    using QualityBot.ScrapePocos;

    public interface IElementProvider
    {
        void Load(string elementsJson, string html, Rectangle boundingRectangle);

        IEnumerable<ScrapedElement> Elements();
    }
}