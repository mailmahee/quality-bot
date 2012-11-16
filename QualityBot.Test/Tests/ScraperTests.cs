namespace QualityBot.Test.Tests
{
    using NUnit.Framework;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Firefox;
    using QualityBot;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using NSubstitute;
    using QualityBot.Util;

    [TestFixture]
    class ScraperTests
    {
        private QBFake fakeQB = new QBFake();
        private IService myScraper = new Service();

        //[Test]
        public void CanScrapeByRequest()
        {
            //Arrange
            var fakescrapeparams = new FakeScrapeParams();
            var expectedScrape = fakeQB.FakeScrape(fakescrapeparams);
            var fakeRequest = fakeQB.FakeRequest(url: fakescrapeparams.Url);

            //Act
            var actualScrape = myScraper.Scrape(fakeRequest);

            //Assert
            Assert.IsTrue(expectedScrape.Equals(actualScrape));
        }

        //[Test]
        public void CanScrapeCurrentByWebDriverNRequest()
        {
            //Arrange
            var fakescrapeparams = new FakeScrapeParams();
            var fakeRequest = fakeQB.FakeRequest(url: fakescrapeparams.Url);
            var expectedScrape = fakeQB.FakeScrape(fakescrapeparams);
            IWebDriver webdriver = new FirefoxDriver();
            webdriver.Navigate().GoToUrl(fakescrapeparams.Url);

            //Act
            var actualScrape = myScraper.ScrapeCurrent(webdriver, fakeRequest);

            //Assert
            Assert.IsTrue(expectedScrape.Equals(actualScrape));
        }

        [Test]
        public void VerifyScrapeCurrentIWebDriver()
        {
            var options = Substitute.For<IOptions>();
            var cookieJar = Substitute.For<ICookieJar>();
            cookieJar.AllCookies.Returns(new ReadOnlyCollection<Cookie>(new[] {new Cookie("foo", "bar") }));
            var webDriver = Substitute.For<IWebDriver, ITakesScreenshot, IJavaScriptExecutor>();
            options.Cookies.Returns(cookieJar);
            webDriver.Manage().Returns(options);
            ((IJavaScriptExecutor)webDriver).ExecuteScript(Arg.Is(Javascript.Viewport)).Returns(new ReadOnlyCollection<object>(new object[] { 800, 600 }));

            var bitmap = new Bitmap(10, 10);
            var base64 = ImageUtil.ImageToBase64(bitmap, ImageFormat.Bmp);
            var screenshot = new Screenshot(base64);
            ((ITakesScreenshot)webDriver).GetScreenshot().Returns(screenshot);
            
            //var subScraper = Substitute.For<Scraper>();
            
            //Assert.Throws<ArgumentNullException>(() => subScraper.ScrapeCurrent(webDriver));
        }
    }
}
