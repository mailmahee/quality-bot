namespace QualityBot.ScrapePocos
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    [Serializable]
    public class PageData
    {
        public Size Size { get; set; }

        public string Html { get; set; }

        public string[] Cookies { get; set; }

        public string ElementsJson { get; set; }

        public Image Screenshot { get; set; }

        public string BrowserName { get; set; }

        public string BrowserVersion { get; set; }

        public string Platform { get; set; }

        public IEnumerable<string> Resources { get; set; }

        public string Url { get; set; }
    }
}