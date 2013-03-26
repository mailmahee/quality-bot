namespace QualityBot.ScrapePocos
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using MongoDB.Bson;
    using QualityBot.Persistence;
    using QualityBot.Util;
    [Serializable]
    public class Scrape : IPersist
    {
        public Scrape()
        {
            Cookies       = new List<string>();
            Path          = new StringAsReference();
            _idString     = new StringAsReference();
            HtmlRef       = new StringAsReference();
            ScreenshotRef = new StringAsReference();
        }

        public ObjectId Id { get; set; }

        private List<ScrapedElement> _elements; 

        private StringAsReference _idString;

        public StringAsReference IdString
        {
            get
            {
                _idString.Value = Id.ToString();
                return _idString;
            }
            set
            {
                _idString = value;
            }
        }

        public string ExcludeJquerySelector { get; set; }

        public string IncludeJquerySelector { get; set; }

        public string Script { get; set; }

        public Rectangle? BoundingRectangle { get; set; }

        public StringAsReference Path { get; set; }

        public List<ScrapedElement> Elements 
        {
            get
            {
                return _elements ?? (_elements = new List<ScrapedElement>());
            }
            set
            {
                _elements = value;
            }
        }

        public Resource[] Resources { get; set; }

        public string Html { get; set; }

        public StringAsReference HtmlRef { get; set; }

        public string Url { get; set; }

        public string Screenshot { get; set; }

        public StringAsReference ScreenshotRef { get; set; }

        public Size ViewportSize { get; set; }

        public string Browser { get; set; }

        public string BrowserVersion { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Platform { get; set; }

        public List<string> Cookies { get; set; }

    }
}