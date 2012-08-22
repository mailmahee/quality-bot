namespace QualityBot
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Net;
    using OpenQA.Selenium;

    using QualityBot.RequestPocos;
    using QualityBot.ScrapePocos;
    using QualityBot.Util;
    using Webinator;
    using Webinator.Toolboxes;

    public class Scraper
    {
        private readonly Config.AvailableFrameworks _frameworkEnum = AppConfigUtil.AppConfig<Config.AvailableFrameworks>("framework");

        private readonly string _seleniumGridHub = AppConfigUtil.AppConfig("seleniumGrid2Hub");

        /// <summary>
        /// Scrapes the page as defined by the request object.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>An object containing information about the web page as a Scrape object.</returns>
        public Scrape Scrape(Request request)
        {
            var web = GetWebManager(GetConfig(request));
            var page = Scrape(web, request);
            web.CloseBrowser();

            return page;
        }
        
        /// <summary>
        /// Scrapes pages as defined by the request objects.
        /// Will attempt to optimize by parallelizing the work across multiple threads.
        /// </summary>
        /// <param name="requests">The requests.</param>
        /// <returns>A collection of objects containing information about each request as a Scrape object.</returns>
        public Scrape[] Scrape(params Request[] requests)
        {
            /* AsParallel() isn't as optimal here because half of the work being done isn't CPU bound,
             *  much of the time is spend waiting for a browser instance from the grid, or waiting for
             *  information to travel back and forth.
             *  This function also has the benefit of returning the results in the same order they were entered.
             */
            var scrapeActions = requests.Select(t => (Func<Scrape>)(() => Scrape(t)));
            var results =  scrapeActions.Parallelize(Environment.ProcessorCount * 2);
            return results;
        }

        /// <summary>
        /// Scrapes the current page.
        /// </summary>
        /// <param name="webDriver">The WebDriver instance to use.</param>
        /// <returns>An object containing information about the web page as a Scrape object.</returns>
        public Scrape ScrapeCurrent(IWebDriver webDriver)
        {
            var request = new Request();
            return GetScrape(webDriver, request);
        }

        /// <summary>
        /// Scrapes the current page.
        /// </summary>
        /// <param name="webManager">The WebManager instance to use.</param>
        /// <returns>An object containing information about the web page as a Scrape object.</returns>
        public Scrape ScrapeCurrent(IWebManager webManager)
        {
            return ScrapeCurrent((IWebDriver)webManager.GetEngine());
        }

        /// <summary>
        /// Scrapes the current page.
        /// </summary>
        /// <param name="webDriver">The WebDriver instance to use.</param>
        /// <param name="request">The request.</param>
        /// <returns>An object containing information about the web page as a Scrape object.</returns>
        public Scrape ScrapeCurrent(IWebDriver webDriver, Request request)
        {
            return GetScrape(webDriver, request);
        }

        /// <summary>
        /// Scrapes the current page.
        /// </summary>
        /// <param name="webManager">The WebManager instance to use.</param>
        /// <param name="request">The request.</param>
        /// <returns>An object containing information about the web page as a Scrape object.</returns>
        public Scrape ScrapeCurrent(IWebManager webManager, Request request)
        {
            return ScrapeCurrent((IWebDriver)webManager.GetEngine(), request);
        }

        /// <summary>
        /// Sets the browser viewport to the specified size.
        /// </summary>
        /// <param name="web">The web manager.</param>
        /// <param name="resolution">The desired size.</param>
        public void SetViewportSize(IWebManager web, Size resolution)
        {
            var engine = (IWebDriver)web.GetEngine();

            var dimensions = (ReadOnlyCollection<object>)web.EvalJavaScript(Javascript.Viewport);
            var size = new Size(
                (int)Math.Round(decimal.Parse(dimensions[0].ToString()), 0, MidpointRounding.AwayFromZero),
                (int)Math.Round(decimal.Parse(dimensions[1].ToString()), 0, MidpointRounding.AwayFromZero));

            while (!size.Equals(resolution))
            {
                // Set resolution
                var currentSize = engine.Manage().Window.Size;
                engine.Manage().Window.Size = new Size(
                        resolution.Width + (Math.Abs(currentSize.Width - size.Width)),
                        resolution.Height + (Math.Abs(currentSize.Height - size.Height)));

                dimensions = (ReadOnlyCollection<object>)web.EvalJavaScript(Javascript.Viewport);
                size = new Size(
                    (int)Math.Round(decimal.Parse(dimensions[0].ToString()), 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round(decimal.Parse(dimensions[1].ToString()), 0, MidpointRounding.AwayFromZero));
            }
        }

        /// <summary>
        /// Instantiates a configuration for Webinator.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// A <c>Config</c> instance.
        /// </returns>
        private Config GetConfig(Request request)
        {
            Config.AvailableBrowsers browserEnum;
            if (!Enum.TryParse(request.Browser, true, out browserEnum))
            {
                browserEnum = Config.AvailableBrowsers.Firefox;
            }

            var config = new Config
            {
                LogScreenshots = false,
                LogLevel = Config.AvailableLogLevels.None,
                BaseUrl = request.Url,
                Browser = browserEnum,
                Framework = _frameworkEnum,
                LogDirectory = Path.Combine("Logs", DateTime.Now.ToString("yyyyMMddHHmmssfffffff")),
                HighlightElements = false,
                SeleniumGridHubUri = _seleniumGridHub
            };

            if (!string.IsNullOrWhiteSpace(request.BrowserVersion))
            {
                config.DesiredCapabilities = new Dictionary<string, object> { { "version", request.BrowserVersion } };
            }
                
            return config;
        }

        /// <summary>
        /// Scrapes information from a web page.
        /// </summary>
        /// <param name="web">The web manager to use to navigate to the url and retrieve data.</param>
        /// <param name="request">The request.</param>
        /// <returns>An object of type Scrape.</returns>
        private Scrape GetScrape(IWebDriver web, Request request)
        {
            var boundingRectangle = new Rectangle();
            var constrainScrape = false;
            if (request.BoundingRectangle != null)
            {
                if (request.BoundingRectangle.Value.Width != 0 || request.BoundingRectangle.Value.Height != 0
                    || request.BoundingRectangle.Value.X != 0 || request.BoundingRectangle.Value.Y != 0)
                {
                    if (request.BoundingRectangle.Value.Width <= 0 || request.BoundingRectangle.Value.Height <= 0
                        || request.BoundingRectangle.Value.X < 0 || request.BoundingRectangle.Value.Y < 0)
                    {
                        throw new Exception("Request bounding rectangle is invalid.");
                    }
                    boundingRectangle = request.BoundingRectangle.Value;
                    constrainScrape = true;
                }
            }

            var ss = (ITakesScreenshot)web;
            var js = (IJavaScriptExecutor)web;

            js.ExecuteScript(Javascript.JQuery);

            // Execute user script on page
            if (!string.IsNullOrWhiteSpace(request.Script)) js.ExecuteScript(string.Format(@"{{{0}}}", request.Script));

            var dimensions = (ReadOnlyCollection<object>)js.ExecuteScript(Javascript.Viewport);
            var size = new Size(
                (int)Math.Round(decimal.Parse(dimensions[0].ToString()), 0, MidpointRounding.AwayFromZero),
                (int)Math.Round(decimal.Parse(dimensions[1].ToString()), 0, MidpointRounding.AwayFromZero));

            var htmlString = web.PageSource;
            var cookies = web.Manage().Cookies.AllCookies;

            var getRects = Javascript.GetRects.Replace("%INCLUDE%", request.IncludeJquerySelector).Replace("%EXCLUDE%", request.ExcludeJquerySelector);
            var infos = (ReadOnlyCollection<object>)js.ExecuteScript(string.Format(@"{{{0}{1}{2}}}", Javascript.GetText, Javascript.GetCss, getRects));
            var pageImage = ImageUtil.BytesToImage(ss.GetScreenshot().AsByteArray);
            var browserInfo = (ReadOnlyCollection<object>)js.ExecuteScript(Javascript.Info);
            var resources = (ReadOnlyCollection<object>)js.ExecuteScript(Javascript.Resources);

            // Identify page resources
            var client = GetWebRequestClient();
            var pageResources = new List<Resource>();
            foreach (var resource in resources.Select(r => r.ToString()))
            {
                string statusDescription;
                HttpStatusCode statusCode;
                var headers = new List<string>();

                try
                {
                    client.DoGetUrlHeadOnly(resource);
                }
                catch
                {
                }
                finally
                {
                    statusCode = client.StatusCode;
                    statusDescription = client.StatusDescription;
                    headers.AddRange(client.ResponseHeaders.Cast<object>().Select((t, i) =>
                            string.Format("{0}:{1}", client.ResponseHeaders.GetKey(i), client.ResponseHeaders.Get(i))));
                }

                pageResources.Add(new Resource { Uri = resource, StatusCode = statusCode, StatusDescription = statusDescription, Headers = headers });
            }

            var page = new Scrape
                {
                    Url = web.Url,
                    TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Browser = browserInfo[0].ToString().ToLower(),
                    BrowserVersion = browserInfo[1].ToString().ToLower(),
                    ViewportSize = size,
                    Platform = browserInfo[2].ToString().ToLower(),
                    Resources = pageResources.ToArray(),
                    Html = htmlString,
                    Screenshot = ImageUtil.ImageToBase64(pageImage, ImageFormat.Png)
                };

            // Identify cookies
            if (cookies != null)
            {
                foreach (var cookie in cookies.Where(cookie => cookie != null))
                {
                    page.Cookies.Add(cookie.ToString());
                }
            }

            var pageRect = new Rectangle(0, 0, pageImage.Width, pageImage.Height);

            // Constrain screenshot
            if (constrainScrape) 
            {
                if (!pageRect.Encompasses(boundingRectangle)) throw new Exception("Request bounding rectangle is invalid.");

                pageRect = boundingRectangle;
            }
            
            var html = XPathToolbox.GetHtmlDocument(web.PageSource);

            for (var i = 0; i < infos.Count; i++)
            {
                var n = XPathToolbox.GetNode(html, string.Format("//*[@diffengineindexer='{0}']", i));
                var node = n.CloneNode(true);
                node.Attributes.Remove("diffengineindexer");
                node.RemoveAllChildren();

                var info = (Dictionary<string, object>)infos[i];
                var rect = new Rectangle(
                    (int)Math.Round(decimal.Parse(info["x"].ToString()), 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round(decimal.Parse(info["y"].ToString()), 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round(decimal.Parse(info["width"].ToString()), 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round(decimal.Parse(info["height"].ToString()), 0, MidpointRounding.AwayFromZero));
                var css = (Dictionary<string, object>)info["css"];
                var text = (string)info["text"];

                // Constrain elements
                var originalRect = rect;
                rect = Rectangle.Intersect(pageRect, rect);
                
                if (!pageRect.Encompasses(rect) || rect.Width <= 0 || rect.Height <= 0) continue;

                page.Elements.Add(new ElementInfo
                {
                    Html = node.OuterHtml.Replace("&amp;", "&"),
                    Text = text,
                    Tag = node.Name.ToLowerInvariant(),
                    Css = css.ToDictionary(k => k.Key, k => Convert.ToString(k.Value)),
                    Attributes = node.Attributes.ToDictionary(k => k.Name.ToLower(), k => Convert.ToString(k.Value)),
                    LocationOnScreenshot = rect,
                    Location = originalRect
                });
            }

            /* Clips the screenshot to bounds of scraped elements or the bounding rectangle (if specified)
             *  Offsets LocationOnScreenshot for affected elements.
            */
            Rectangle clipRect;
            if (constrainScrape)
            {
                clipRect = boundingRectangle;
            }
            else
            {
                var minX = page.Elements.Min(e => e.LocationOnScreenshot.Left);
                var maxX = page.Elements.Max(e => e.LocationOnScreenshot.Right);
                var minY = page.Elements.Min(e => e.LocationOnScreenshot.Top);
                var maxY = page.Elements.Max(e => e.LocationOnScreenshot.Bottom);
                clipRect = new Rectangle(minX, minY, maxX - minX, maxY - minY);
            }
            var image = ImageUtil.CropImage(pageImage, clipRect);
            page.Screenshot = ImageUtil.ImageToBase64(image, ImageFormat.Png);

            foreach (var e in page.Elements)
            {
                var r = e.LocationOnScreenshot;
                r.X -= clipRect.X;
                r.Y -= clipRect.Y;
                e.LocationOnScreenshot = r;
            }

            image.Dispose();
            pageImage.Dispose();

            return page;
        }

        /// <summary>
        /// Instantiates Webinator.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns>
        /// An <c>IWebManager</c> instance.
        /// </returns>
        private IWebManager GetWebManager(Config config)
        {
            return WebManagerFactory.CreateInstance(config);
        }

        /// <summary>
        /// Gets an instance of the robust web client.
        /// </summary>
        /// <returns>
        /// A <c>RobustWebClient</c> instance.
        /// </returns>
        private WebRequestClient GetWebRequestClient()
        {
            // Setup cookie container and WebRequest wrapper
            var client = new WebRequestClient();

            // Setup request headers
            client.Headers.Clear();
            client.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            client.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.5");
            client.Headers.Add(HttpRequestHeader.Accept, "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            client.Headers.Add(HttpRequestHeader.AcceptCharset, "ISO-8859-1,utf-8;q=0.7,*;q=0.7");

            return client;
        }

        /// <summary>
        /// Loads the specified url.
        /// </summary>
        /// <param name="web">The web manager.</param>
        /// <param name="url">The url.</param>
        private void LoadUrl(IWebManager web, string url)
        {
            web.GoToUrl(url);
        }
        
        /// <summary>
        /// Scrapes information about web pages.
        /// </summary>
        /// <param name="web">The web manager.</param>
        /// <param name="request">The request.</param>
        /// <returns>An object containing information about the web page as a Scrape object.</returns>
        private Scrape Scrape(IWebManager web, Request request)
        {
            LoadUrl(web, request.Url);
            if (request.ViewportResolution != null)
            {
                SetViewportSize(web, new Size(request.ViewportResolution.Value.Width, request.ViewportResolution.Value.Height));
            }

            var scrape = GetScrape((IWebDriver)web.GetEngine(), request);

            return scrape;
        }
    }
}