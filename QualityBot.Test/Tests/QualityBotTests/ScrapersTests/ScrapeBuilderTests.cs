namespace QualityBot.Test.Tests.QualityBotTests.ScrapersTests
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Runtime.Serialization.Formatters.Binary;
    using NSubstitute;
    using NUnit.Framework;
    using QualityBot.RequestPocos;
    using QualityBot.ScrapePocos;
    using QualityBot.Scrapers;
    using QualityBot.Scrapers.Interfaces;
    using QualityBot.Test.Tests.QualityBotFake;

    [TestFixture]
    class ScrapeBuilderTests
    {
        private PageData _pageData;
        private ScrapeBuilder _scrapeBuilder;

        internal string GetFromResources(string resourceName)
        {
            var assem = GetType().Assembly;
            using (var stream = assem.GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        [TestFixtureSetUp]
        public void Setup()
        {
            var webRequestUtilMock = Substitute.For<IWebRequestUtil>();
            Resource[] resArray =
            {
                new Resource
                {
                    Uri = "http://www.zombo.com/inrozxa.swf"
                },
                new Resource
                {
                    Uri = "http://www.google.com/197834gfn8qw673tgfn8763tbf.aspx"
                }
            };
            
            webRequestUtilMock.HeadCheck(resArray).ReturnsForAnyArgs(new []
            {
                new Resource
                {
                    Headers = new List<string>()
                    {
                        "Content-Encoding:",
                        "Vary:Accept-Encoding",
                        "X-AspNetMvc-Version:4.0",
                        "X-UA-Compatible:IE=edge",
                        "Content-Length:154",
                        "Cache-Control:private",
                        "Content-Type:text/html; charset=utf-8",
                        "Date:Mon, 03 Dec 2012 16:42:16 GMT",
                        "Server:Microsoft-IIS/7.5",
                        "X-AspNet-Version:4.0.30319"
                    },
                    StatusCode = HttpStatusCode.OK,
                    StatusDescription = "OK",
                    Uri = "http://www.zombo.com/inrozxa.swf"
                } 
            });
            _pageData = new PageData
            {
                Size           = new Size(800, 600),
                Html           = GetFromResources("QualityBot.Test.Tests.TestData.FakeHTML.txt"),
                Cookies        = new QBFake().FakeCookies(),
                ElementsJson   = GetFromResources("QualityBot.Test.Tests.TestData.FakeElementJSON.txt"),
                Screenshot     = new Bitmap(10, 10),
                BrowserName    = "firefox",
                BrowserVersion = "10.1",
                Platform       = "WINDOWS",
                Resources      = new[] { "http://localhost" },
                Url            = "http://www.google.com"
            };
            _scrapeBuilder = new ScrapeBuilder(new ElementProvider(), webRequestUtilMock);
        }

        [Test]
        public void VerifyBuildScrape()
        {
            var result = _scrapeBuilder.BuildScrape(new Request(), _pageData);

            Assert.IsTrue(result != null);
        }
    }
}
