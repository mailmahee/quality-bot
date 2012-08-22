using System;
using QualityBot.ComparePocos;
using QualityBot.RequestPocos;
using QualityBot.ScrapePocos;

namespace QualityBot
{
    public class Service
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="persist"> </param>
        /// <returns></returns>
        public Scrape Scrape(string url, bool persist = true)
        {
            Scrape retval = null;
            try
            {
                var tmpScraper = new Scraper();
                var tmpRequest = new Request(url);
                var tmpScrape = tmpScraper.Scrape(tmpRequest);

                if (tmpScrape != null)
                {
                    retval = tmpScrape;
                    if(persist)
                    {
                        var tmpPersister = new Persistence.ScrapePersister();
                        tmpPersister.SaveToMongoDb(tmpScrape);
                    }
                }
            }
            catch (Exception ex)
            {
                
                throw;
            }
           
            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageA"></param>
        /// <param name="pageB"></param>
        /// <returns></returns>
        public Comparison Compare(Scrape pageA, Scrape pageB, bool persist = true)
        {
            Comparison retComparison = null;
            var tmpComparer = new Comparer();

            retComparison = tmpComparer.Compare(pageA, pageB);
            if (retComparison != null)
            {
                if(persist)
                {
                    var tmpPersister = new Persistence.ComparePersister();

                    tmpPersister.SaveToMongoDb(retComparison);
                }
            }
            return retComparison;
        }

        public Comparison Compare(string urlA, string urlB)
        {
            Comparison retComparison = null;
            
            // Create Scrape(s)
            var tmpService = new Service();
            var retScrapeA = tmpService.Scrape("www.ancestry.com", false);
            var retScrapeB = tmpService.Scrape("www.ancestrystage.com", false);

            //Pass Scrape to Comparison
            var tmpComparer = new Comparer();
            retComparison = tmpComparer.Compare(retScrapeA, retScrapeB);

            // Persit Comparison Obj
            var tmpPersiter = new Persistence.ComparePersister();

            tmpPersiter.SaveToMongoDb(retComparison);

            return retComparison;
        }
    }
}
