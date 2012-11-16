namespace QualityBot.Scrapers.Facades
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.Linq;

    using OpenQA.Selenium;

    using QualityBot.RequestPocos;
    using QualityBot.ScrapePocos;
    using QualityBot.Scrapers.Interfaces;
    using QualityBot.Util;

    public class WebDriverFacade : IBrowserFacade
    {
        private ITakesScreenshot _ss;

        private IJavaScriptExecutor _js;

        private IWebDriver _web;

        private Request _request;

        private ReadOnlyCollection<object> _browserInfo;

        public WebDriverFacade(IWebDriver web, Request request)
        {
            _request = request;
            _web     = web;
            _ss      = (ITakesScreenshot)web;
            _js      = (IJavaScriptExecutor)web;

            web.Manage().Timeouts().SetScriptTimeout(new TimeSpan(0, 5, 0));
        }

        private void InjectJavascriptHelpers()
        {
            _js.ExecuteAsyncScript(string.Format(@"{{var callback = arguments[arguments.length-1];{0}}}", Javascript.JQuery));
            _js.ExecuteScript(string.Format(@"{{{0}}}", Javascript.GetText));
            _js.ExecuteScript(string.Format(@"{{{0}}}", Javascript.GetCss));
        }

        private void RunUserScript()
        {
            if (!string.IsNullOrWhiteSpace(_request.Script))
            {
                _js.ExecuteScript(string.Format(@"{{{0}}}", _request.Script));
            }
        }

        private string[] GetCookies()
        {
            return _web.Manage().Cookies.AllCookies
                        .Where(c => c != null)
                        .Select(c => c.ToString())
                        .ToArray();
        }

        private Image GetScreenshotImage()
        {
            var screenshotBytes = _ss.GetScreenshot().AsByteArray;
            var pageImage = ImageUtil.BytesToImage(screenshotBytes);

            return pageImage;
        }

        private IEnumerable<string> GetPageResources()
        {
            var resources = (ReadOnlyCollection<object>)_js.ExecuteScript(Javascript.Resources);

            return resources.Select(r => r.ToString()).ToArray();
        }

        private string BrowserName()
        {
            return _browserInfo[0].ToString().ToLower();
        }

        private string BrowserVersion()
        {
            return _browserInfo[1].ToString().ToLower();
        }

        private string Platform()
        {
            return _browserInfo[2].ToString().ToLower();
        }

        private void GetBrowserInfo()
        {
            _browserInfo = (ReadOnlyCollection<object>)_js.ExecuteScript(Javascript.Info);    
        }

        private string GetPageSource()
        {
            return _web.PageSource;
        }

        private string GetUrl()
        {
            return _web.Url;
        }

        private void SetUrl()
        {
            _web.Url = _request.Url.QualifyUrl();
        }

        private string GetElements()
        {
            var getRects = Javascript.GetRects
                .Replace("%INCLUDE%", _request.IncludeJquerySelector)
                .Replace("%EXCLUDE%", _request.ExcludeJquerySelector);
            var infos = (string)_js.ExecuteScript(getRects);

            return infos;
        }

        /// <summary>
        /// Sets the browser viewport to the specified size.
        /// </summary>
        private void SetViewportSize()
        {
            if (_request.ViewportResolution != null)
            {
                var resolution = new Size(_request.ViewportResolution.Value.Width, _request.ViewportResolution.Value.Height);
                var size = GetViewportSize();

                while (!size.Equals(resolution))
                {
                    // Set resolution
                    var currentSize = _web.Manage().Window.Size;
                    _web.Manage().Window.Size = new Size(
                        resolution.Width + (Math.Abs(currentSize.Width - size.Width)),
                        resolution.Height + (Math.Abs(currentSize.Height - size.Height)));

                    size = GetViewportSize();
                }
            }

        }

        private Size GetViewportSize()
        {
            var dimensions = (ReadOnlyCollection<object>)_js.ExecuteScript(Javascript.Viewport);
            var size = new Size(IntFromObject(dimensions[0]), IntFromObject(dimensions[1]));

            return size;
        }

        public PageData ScrapeData()
        {
            // Order that these are done in is crucial, don't change the order
            SetUrl();
            SetViewportSize();
            InjectJavascriptHelpers();
            RunUserScript();

            var size = GetViewportSize();
            var cookies = GetCookies();
            var elements = GetElements();
            var screenshot = GetScreenshotImage();

            GetBrowserInfo();

            var browserName = BrowserName();
            var browserVersion = BrowserVersion();
            var platform = Platform();
            var resources = GetPageResources();
            var html = GetPageSource();
            var url = GetUrl();

            var pageData = new PageData
                {
                    BrowserName = browserName,
                    BrowserVersion = browserVersion,
                    Platform = platform,
                    Cookies = cookies,
                    ElementsJson = elements,
                    Html = html,
                    Resources = resources,
                    Screenshot = screenshot,
                    Size = size,
                    Url = url
                };

            return pageData;
        }

        private int IntFromObject(object number)
        {
            return (int)Math.Round(Decimal.Parse(number.ToString()), 0, MidpointRounding.AwayFromZero);
        }

        public void Dispose()
        {
            _web.Quit();
        }
    }
}