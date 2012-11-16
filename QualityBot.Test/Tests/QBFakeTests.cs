using System.Drawing.Imaging;
using QualityBot.Util;

namespace QualityBot.Test.Tests
{
    using System.Drawing;
    using System.Linq;
    using NUnit.Framework;
    using QualityBot.ScrapePocos;
    using QualityBot.RequestPocos;

    [TestFixture]
    class QBFakeTests
    {
        private QBFake fakeTest = new QBFake();

        [Test]
        public void testFakeRequestDefault()
        {
            //Arrange
            Request expectedRequest = new Request();

            //Act
            Request actualRequest = fakeTest.FakeRequest();
            
            //Assert
            Assert.IsTrue(expectedRequest.Equals(actualRequest));
        }

        [Test]
        public void testFakeRequestUrl()
        {
            //Arrange
            Request expectedRequest = new Request("my.url.com");
            //Act
            Request actualRequest = fakeTest.FakeRequest("my.url.com");

            //Assert
            Assert.IsTrue(expectedRequest.Equals(actualRequest));
        }

        [Test]
        public void testFakeRequestUrlNBrowser()
        {
            //Arrange
            string url = "my.url.com";
            string browser = "mybrowser";
            Request expectedRequest = new Request(url, browser);

            //Act
            Request actualRequest = fakeTest.FakeRequest(url, browser);

            //Assert
            Assert.IsTrue(expectedRequest.Equals(actualRequest));
        }

        [Test]
        public void testFakeRequestUrlNBrowserNVersion()
        {
            //Arrange
            string url = "my.url.com";
            string browser = "mybrowser";
            string browserversion = "myversion";
            Request expectedRequest = new Request(url, browser, browserversion);

            //Act
            Request actualRequest = fakeTest.FakeRequest(url, browser, browserversion);

            //Assert
            Assert.IsTrue(expectedRequest.Equals(actualRequest));
        }

        [Test]
        public void testFakeRequestUrlNBrowserNVersionNViewport()
        {
            //Arrange
            string url = "my.url.com";
            string browser = "mybrowser";
            string browserversion = "myversion";
            Size viewportsize = new Size(800, 600);
            Request expectedRequest = new Request(url, browser, browserversion, viewportsize);

            //Act
            Request actualRequest = fakeTest.FakeRequest(url, browser, browserversion, viewportsize);

            //Assert
            Assert.IsTrue(expectedRequest.Equals(actualRequest));
        }

        [Test]
        public void testFakeRequestUrlNBrowserNViewport()
        {
            //Arrange
            string url = "my.url.com";
            string browser = "mybrowser";
            Size viewportsize = new Size(800, 600);
            Request expectedRequest = new Request(url, browser, viewportsize);

            //Act
            Request actualRequest = fakeTest.FakeRequest(url, browser, viewportSize: viewportsize);

            //Assert
            Assert.IsTrue(expectedRequest.Equals(actualRequest));
        }

        [Test]
        public void testFakes()
        {
            const string theId = "555b77b99b111d2aa4b33333";
            var rect = new Rectangle(10, 10, 10, 10);
            Scrape faked = fakeTest.FakeScrape(new FakeScrapeParams(theId, bounding: rect));
            Assert.IsTrue(theId == faked.IdString.Value && rect == faked.BoundingRectangle);
        }

        [Test]
        public void testFakeParams()
        {
            const string theId = "555b77b99b111d2aa4b33333";
            var rect = new Rectangle(10, 10, 10, 10);
            var faked = new FakeScrapeParams(theId, bounding: rect);
            Assert.IsTrue(theId == faked.Id && rect == faked.Bounding);
        }

        [Test]
        public void testMultipleScrapes()
        {
            FakeScrapeParams[] para =
                {
                    new FakeScrapeParams(),
                    new FakeScrapeParams("555b77b99b111d2aa4b33333", bounding: new Rectangle(10,10,10,10))
                };
            var scrapes = fakeTest.FakeScrape(para).ToArray();
            Assert.IsTrue(scrapes.Length > 1 && scrapes[0].Id.ToString() == "505b74b57b350d1ac4b55993" && scrapes[1].Id.ToString() == "555b77b99b111d2aa4b33333");
        }

        [Test]
        public void testCompareFakeScrapes()
        {
            //Arrange
            Scrape fake1 = fakeTest.FakeScrape(new FakeScrapeParams());
            Scrape fake2 = fakeTest.FakeScrape(new FakeScrapeParams("555b77b99b111d2aa4b33333", bounding: new Rectangle(10, 10, 10, 10)));

            //Act
            var compared = fakeTest.CompareFakeScrapes(fake1, fake2);

            //Assert
            Assert.IsTrue(compared.Result.Html.PercentChanged == 0);
        }

        [Test]
        public void testFakeComparison()
        {
            var compared = fakeTest.FakeComparison(new FakeComparisonParams());
            Assert.IsTrue(compared.IdString == "555b77b99b111d2aa4b33333");
        }
    }
}
