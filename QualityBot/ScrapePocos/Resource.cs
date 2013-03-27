namespace QualityBot.ScrapePocos
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    [Serializable]
    public class Resource
    {
        public Resource()
        {
            Headers = new List<string>();
        }

        public string Uri { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public string StatusDescription { get; set; }

        public List<string> Headers { get; set; }

    }
}