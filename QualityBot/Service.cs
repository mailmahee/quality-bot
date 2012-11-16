namespace QualityBot
{
    using System;
    using System.Drawing;
    using System.Linq;
    using OpenQA.Selenium;
    using QualityBot.ComparePocos;
    using QualityBot.Persistence;
    using QualityBot.RequestPocos;
    using QualityBot.ScrapePocos;
    using QualityBot.Scrapers;

    public class Service : IService
    {
        private Persister<Comparison> _comparePersister;

        private Persister<Scrape> _scrapePersister;

        private ScrapeBuilder _scrapeBuilder;

        public Service()
        {
            _comparePersister = PersisterFactory.CreateComparePersisterInstance();
            _scrapePersister = PersisterFactory.CreateScrapePersisterInstance();

            var elementProvider = new ElementProvider();
            var webRequestUtil = new WebRequestUtil();
            _scrapeBuilder = new ScrapeBuilder(elementProvider, webRequestUtil);
        }

        public Comparison[] Compare(string urlA, string urlB, bool persist = true)
        {
            // Create Scrape(s)
            var requestA = new Request(urlA);
            var requestB = new Request(urlB);
            var scrapeA = DoScrape(requestA);
            var scrapeB = DoScrape(requestB);

            // Pass Scrape to Comparison
            var comparer = new Comparer();
            var comparison = comparer.Compare(scrapeA, scrapeB);

            // Persist Scrapes
            SaveScrapes(persist, scrapeA, scrapeB);

            // Persist Comparison Obj
            SaveComparison(persist, comparison);

            return new[] { comparison };
        }

        public Comparison[] CompareDynamic(dynamic requestA, dynamic requestB, bool persist = true)
        {
            var rA = new Request
            {
                BoundingRectangle = GetRectangle((string)requestA.boundingRectangle),
                Browser = requestA.browser,
                BrowserVersion = requestA.browserVersion,
                IncludeJquerySelector = requestA.includeJquerySelector,
                ExcludeJquerySelector = requestA.excludeJquerySelector,
                Script = requestA.script,
                ViewportResolution = GetSize((string)requestA.viewportResolution),
                Url = requestA.url
            };

            var rB = new Request
            {
                BoundingRectangle = GetRectangle((string)requestB.boundingRectangle),
                Browser = requestB.browser,
                BrowserVersion = requestB.browserVersion,
                IncludeJquerySelector = requestB.includeJquerySelector,
                ExcludeJquerySelector = requestB.excludeJquerySelector,
                Script = requestB.script,
                ViewportResolution = GetSize((string)requestB.viewportResolution),
                Url = requestB.url
            };

            var scrapeA = DoScrape(rA);
            var scrapeB = DoScrape(rB);

            // Pass Scrapes to Comparison
            var comparer = new Comparer();
            var comparison = comparer.Compare(scrapeA, scrapeB);

            // Persist Scrapes
            SaveScrapes(persist, scrapeA, scrapeB);

            // Persist Comparison Obj
            SaveComparison(persist, comparison);

            return new[] { comparison };
        }

        public Comparison[] CompareScrapeIds(string scrapeIdA, string scrapeIdB, bool persist)
        {
            var persister = PersisterFactory.CreateScrapePersisterInstance();

            var scrapeA = persister.Load(scrapeIdA).First();
            var scrapeB = persister.Load(scrapeIdB).First();

            // Pass Scrapes to Comparison
            var comparer = new Comparer();
            var comparison = comparer.Compare(scrapeA, scrapeB);

            // Persist Comparison Obj
            SaveComparison(persist, comparison);

            return new[] { comparison };
        }

        public Scrape Scrape(string url, bool persist = true)
        {
            // Get scrape
            var request = new Request(url);
            var scrape = DoScrape(request);

            // Persist Scrape
            SaveScrapes(persist, scrape);

            return scrape;
        }

        public string ScrapeDynamic(dynamic request)
        {
            var sR = new Request
            {
                BoundingRectangle = GetRectangle((string)request.boundingRectangle),
                Browser = request.browser,
                BrowserVersion = request.browserVersion,
                IncludeJquerySelector = request.includeJquerySelector,
                ExcludeJquerySelector = request.excludeJquerySelector,
                Script = request.script,
                ViewportResolution = GetSize((string)request.viewportResolution),
                Url = request.url
            };

            var scrape = DoScrape(sR);

            SaveScrapes(true, scrape);

            return scrape.IdString.Value;
        }

        /// <summary>
        /// Scrapes the page as defined by the request object.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>An object containing information about the web page as a Scrape object.</returns>
        public Scrape Scrape(Request request)
        {
            return DoScrape(request);
        }

        /// <summary>
        /// Scrapes the current page.
        /// </summary>
        /// <param name="webDriver">The WebDriver instance to use.</param>
        /// <param name="request">The request.</param>
        /// <returns>An object containing information about the web page as a Scrape object.</returns>
        public Scrape ScrapeCurrent(IWebDriver webDriver, Request request)
        {
            Scrape scrape;
            using (var facade = FacadeFactory.CreateFacade(webDriver, request))
            {
                var data = facade.ScrapeData();
                scrape = _scrapeBuilder.BuildScrape(request, data);
            }

            return scrape;
        }

        private Scrape DoScrape(Request request)
        {
            Scrape scrape;
            using (var facade = FacadeFactory.CreateFacade(request))
            {
                var data = facade.ScrapeData();
                scrape = _scrapeBuilder.BuildScrape(request, data);
            }

            return scrape;
        }

        private Rectangle? GetRectangle(string rectangle)
        {
            Rectangle? rect;
            try
            {
                var rectData = rectangle.Split(',');
                rect = new Rectangle(int.Parse(rectData[0]), int.Parse(rectData[1]), int.Parse(rectData[2]), int.Parse(rectData[3]));
            }
            catch (Exception)
            {
                rect = null;
            }

            if (rect != null && (rect.Value.X < 1 || rect.Value.Y < 1 || rect.Value.Width < 1 || rect.Value.Height < 1)) rect = null;

            return rect;
        }

        private Size? GetSize(string size)
        {
            Size? sz;
            try
            {
                var sizeData = (size).Split(',');
                sz = new Size(int.Parse(sizeData[0]), int.Parse(sizeData[1]));
            }
            catch (Exception)
            {
                sz = null;
            }

            return sz;
        }

        private void SaveComparison(bool persist, params Comparison[] comparisons)
        {
            if (!persist) return;
            foreach (var comparison in comparisons.Where(comparison => comparison != null))
            {
                _comparePersister.Save(comparison);
            }
        }

        private void SaveScrapes(bool persist, params Scrape[] scrapes)
        {
            if (!persist) return;
            foreach (var scrape in scrapes.Where(scrape => scrape != null))
            {
                _scrapePersister.Save(scrape);
            }
        }
    }
}