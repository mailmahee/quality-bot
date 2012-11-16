namespace QualityBot.ServiceLibrary
{
    public class QualityBotService : IQualityBotService
    {
        public dynamic Scrape(string url, bool persist)
        {
            var qBSvc = new Service();

            return qBSvc.Scrape(url, persist);
        }

        public dynamic Compare(string urlA, string urlB, bool persist)
        {
            var qBSvc = new Service();

            return qBSvc.Compare(urlA, urlB, persist);
        }

        public dynamic CompareDynamic(dynamic requestA, dynamic requestB, bool persist = true)
        {
            var qBSvc = new Service();

            return qBSvc.CompareDynamic(requestA, requestB, persist);
        }

        public dynamic ScrapeDynamic(dynamic request)
        {
            var qBSvc = new Service();

            return qBSvc.ScrapeDynamic(request);
        }

        public dynamic CompareScrapeIds(string scrapeIdA, string scrapeIdB, bool persist)
        {
            var qBSvc = new Service();

            return qBSvc.CompareScrapeIds(scrapeIdA, scrapeIdB, persist);
        }
    }
}
