namespace QualityBot.ComparePocos
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using QualityBot.ScrapePocos;
    using QualityBot.Util;
    [Serializable]
    public class ScrapeHybrid
    {
        public ScrapeHybrid()
        {
            Cookies = new List<string>();
            Path = new StringAsReference();
            Html = new StringAsReference();
            Screenshot = new StringAsReference();
        }

        public string ExcludeJquerySelector { get; set; }

        public string IncludeJquerySelector { get; set; }

        public string Script { get; set; }

        public Rectangle? BoundingRectangle { get; set; }

        public string Description { get; set; }

        public StringAsReference IdString { get; set; }

        public StringAsReference Path { get; set; }

        public Resource[] Resources { get; set; }

        public StringAsReference Html { get; set; }

        public string Url { get; set; }

        public StringAsReference Screenshot { get; set; }

        public Size ViewportSize { get; set; }

        public string Browser { get; set; }

        public string BrowserVersion { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Platform { get; set; }

        public List<string> Cookies { get; set; }

    }
}