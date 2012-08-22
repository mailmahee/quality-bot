using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QualityBot.ServiceLibrary
{
    public class QualityBotService : IQualityBotService
    {
        public ScrapePocos.Scrape Scrape(string url, bool persist)
        {
            var qBSvc = new QualityBot.Service();

            return qBSvc.Scrape(url, persist);
        }

        public ComparePocos.Comparison Compare(string urlA, string urlB)
        {
            var qBSvc = new QualityBot.Service();

            return qBSvc.Compare(urlA, urlB);
        }
    }
}
