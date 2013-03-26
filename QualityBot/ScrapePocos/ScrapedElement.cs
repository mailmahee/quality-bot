namespace QualityBot.ScrapePocos
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    [Serializable]
    public class ScrapedElement
    {
        public string Html { get; set; }

        public string Tag { get; set; }

        public Rectangle LocationOnScreenshot { get; set; }

        public Rectangle Location { get; set; }

        public Dictionary<string, string> Css { get; set; }

        public Dictionary<string, string> Attributes { get; set; }

        public ScrapedElement CorrespondingScrapedElement { get; set; }

        public bool HasChanges { get; set; }

        public string Text { get; set; }
    }
}