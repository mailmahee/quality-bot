namespace QualityBot.Test.Tests.QualityBotTests.ScrapersTests
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Runtime.Serialization;
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

        internal static Stream GetResourceStream(string resourceName)
        {
            var thisExe = Assembly.GetExecutingAssembly();
            var file = thisExe.GetManifestResourceStream(resourceName);
            return file;
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
            IFormatter formatter = new BinaryFormatter();
            _pageData = (PageData)formatter.Deserialize(GetResourceStream("QualityBot.Test.Tests.TestData.FakePageData.bin"));
            _scrapeBuilder = new ScrapeBuilder(new ElementProvider(), webRequestUtilMock);
        }

        [Test, Category("Unit")]
        public void VerifyBuildScrape()
        {
            var result = _scrapeBuilder.BuildScrape(new Request(), _pageData);

            Assert.IsTrue(result != null);
        }
    }
}
