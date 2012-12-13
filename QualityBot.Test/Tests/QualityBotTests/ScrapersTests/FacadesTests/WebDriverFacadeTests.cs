namespace QualityBot.Test.Tests.QualityBotTests.ScrapersTests.FacadesTests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Net;
    using NSubstitute;
    using NUnit.Framework;
    using OpenQA.Selenium;
    using QualityBot.RequestPocos;
    using QualityBot.ScrapePocos;
    using QualityBot.Scrapers.Facades;
    using QualityBot.Util;
    using Cookie = OpenQA.Selenium.Cookie;

    [TestFixture]
    class WebDriverFacadeTests
    {
        private IWebDriver _web;
        private IJavaScriptExecutor _js;
        private ITakesScreenshot _ss;
        private Request _request;
        private WebDriverFacade _facade;

        [TestFixtureSetUp]
        public void Setup()
        {
            #region IWebDriver Setup
            IList<Cookie> cookies = new List<Cookie>();
            cookies.Add(new Cookie("s_cc","=true"));
            _web = Substitute.For<IWebDriver>();
            _web.Manage().Cookies.AllCookies.Returns(new ReadOnlyCollection<Cookie>(cookies));
            #endregion
            #region IJavascriptExecutor Setup
            IList<object> _dimensions = new List<object>(new object[] {800,600});
            var _dimensionsCollection = new ReadOnlyCollection<object>(_dimensions);
            _js = Substitute.For<IJavaScriptExecutor>();
            _js.ExecuteScript(Arg.Is(Javascript.Viewport)).Returns(_dimensionsCollection);
            IList<object> _browserInfo = new List<object>(new object[] { "firefox", "10.1", "WINDOWS" });
            var _browserInfoCollection = new ReadOnlyCollection<object>(_browserInfo);
            _js.ExecuteScript(Arg.Is(Javascript.Info)).Returns(_browserInfoCollection);
            IList<object> _resources = new List<object>(new[]
            {
                new Resource
                {
                    Uri = "http://c.mfcreativedev.com/webparts/banner/Banner.js?v=c5589edb",
                    StatusCode = HttpStatusCode.OK,
                    StatusDescription = "OK",
                    Headers = new List<string>
                    {
                        "Content-Length:194",
                        "Cache-Control:public, must-revalidate",
                        "Content-Type:application/x-javascript",
                        "Date:Thu, 20 Sep 2012 17:15:03 GMT",
                        "ETag:JsJt380DknGc4kAEEn76og=="
                    }
                }
            });

            var _resourcesCollection = new ReadOnlyCollection<object>(_resources);
            _js.ExecuteScript(Arg.Is(Javascript.Resources)).Returns(_resourcesCollection);
            #endregion
            #region ITakesScreenshot Setup
            _ss = Substitute.For<ITakesScreenshot>();
            var bp = ImageUtil.ImageToBase64(new Bitmap(10, 10), ImageFormat.Png);
            _ss.GetScreenshot().Returns(new Screenshot(bp));
            #endregion
            _request = new Request
            {
                Url = "http://www.google.com/",
                Browser = "firefox"
            };
            _facade = new WebDriverFacade(_web,_request,_ss,_js);
            
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _facade.Dispose();
        }

        [Test]
        public void VerifyWebDriverFacade()
        {
            var result = _facade.ScrapeData();
            Assert.IsTrue(result != null);
        }
    }
}
