namespace QualityBot.Scrapers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Linq;
    using System.Text.RegularExpressions;

    using QualityBot.RequestPocos;
    using QualityBot.ScrapePocos;
    using QualityBot.Scrapers.Interfaces;
    using QualityBot.Util;
    using QualityBot.Util.Interfaces;

    public class ScrapeBuilder
    {
        private IElementProvider _elementProvider;

        private IWebHeadClient _webHeadClient;

        public ScrapeBuilder(IElementProvider elementProvider, IWebHeadClient webHeadClient)
        {
            _elementProvider = elementProvider;
            _webHeadClient = webHeadClient;
        }

        /// <summary>
        /// Scrapes information from a web page.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="pageData">Information pulled from the page.</param>
        /// <returns>An object of type Scrape.</returns>
        public Scrape BuildScrape(Request request, PageData pageData)
        {
            var screenshotSize = new Rectangle(0, 0, pageData.Screenshot.Width, pageData.Screenshot.Height);
            var boundingRectangleIsDefined = BoundingRectangleIsDefined(request.BoundingRectangle);
            ValidateBoundingRectangle(request.BoundingRectangle, screenshotSize, boundingRectangleIsDefined);

            // If bounding rectangle is defined, use it, otherwise use the size of the screenshot
            var boundingRectangle = boundingRectangleIsDefined ? request.BoundingRectangle.Value : screenshotSize;

            // Identify page resources
            var pageResources = _webHeadClient.HeadCheck(pageData.Resources);

            var page = AssembleScrape(request, pageData, pageResources);

            // Build elements
            _elementProvider.Load(pageData.ElementsJson, pageData.Html, boundingRectangle);
            var elements = _elementProvider.Elements().ToArray();
            page.Elements.AddRange(elements);

            /* If user-defined area is set
             *  Constrain to user-defined area
             * Else
             *  Constrain to existing elements locations
            */
            var clipRect = boundingRectangleIsDefined ? boundingRectangle : ConstrainRectangleToElements(page.Elements);

            // Crop image to bounding rectangle
            var image = ImageUtil.CropImage(pageData.Screenshot, clipRect);
            page.Screenshot = ImageUtil.ImageToBase64(image, ImageFormat.Png);

            // Offset elements LocationOnScreenshot.
            ConstrainElementsToBoundingRectangle(page, clipRect);

            // Cleanup
            image.Dispose();
            pageData.Screenshot.Dispose();

            return page;
        }

        private Regex reg;

        public string CleanHtml(string html)
        {
            reg = reg ?? new Regex(@"diffengineindexer=""[0-9]+""");
            return reg.Replace(html, string.Empty);
        }

        private Scrape AssembleScrape(Request request, PageData pageData, Resource[] pageResources)
        {
            var screenshotBase64 = ImageUtil.ImageToBase64(pageData.Screenshot, ImageFormat.Png);

            var scrape = new Scrape
            {
                ExcludeJquerySelector = request.ExcludeJquerySelector,
                IncludeJquerySelector = request.IncludeJquerySelector,
                BoundingRectangle     = request.BoundingRectangle,
                Script                = request.Script,
                Url                   = pageData.Url,
                TimeStamp             = DateTime.Now,
                Browser               = pageData.BrowserName.ToLower(),
                BrowserVersion        = pageData.BrowserVersion.ToLower(),
                ViewportSize          = pageData.Size,
                Platform              = pageData.Platform.ToLower(),
                Resources             = pageResources,
                Html                  = CleanHtml(pageData.Html),
                Screenshot            = screenshotBase64,
                Cookies               = pageData.Cookies.ToList()
            };

            return scrape;
        }

        private bool BoundingRectangleIsDefined(Rectangle? rect)
        {
            return rect != null && !RectangleUtil.AllZeroes(rect.Value);
        }

        private void ConstrainElementsToBoundingRectangle(Scrape page, Rectangle clipRect)
        {
            foreach (var e in page.Elements)
            {
                var r = e.LocationOnScreenshot;
                r.X -= clipRect.X;
                r.Y -= clipRect.Y;
                e.LocationOnScreenshot = r;
            }
        }

        /// <summary>
        /// Calculates a bounding rectangle given child elements.
        /// </summary>
        /// <param name="elements">The child elements.</param>
        /// <returns>A bounding rectangle.</returns>
        private Rectangle ConstrainRectangleToElements(List<ScrapedElement> elements)
        {
            var minX = elements.Min(e => e.LocationOnScreenshot.Left);
            var maxX = elements.Max(e => e.LocationOnScreenshot.Right);
            var minY = elements.Min(e => e.LocationOnScreenshot.Top);
            var maxY = elements.Max(e => e.LocationOnScreenshot.Bottom);

            return new Rectangle(minX, minY, maxX - minX, maxY - minY);
        }

        private void ValidateBoundingRectangle(Rectangle? rect, Rectangle parentRectangle, bool isDefined)
        {
            var isValid = rect != null
                   && RectangleUtil.PositiveDimensions(rect.Value) 
                   && RectangleUtil.PositiveOrZeroCoordinates(rect.Value)
                   && parentRectangle.Encompasses(rect.Value);
            
            if (isDefined && !isValid)
            {
                throw new ArgumentException("Request bounding rectangle is invalid.");
            }
        }
    }
}