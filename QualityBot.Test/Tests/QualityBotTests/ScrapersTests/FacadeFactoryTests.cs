namespace QualityBot.Test.Tests.QualityBotTests.ScrapersTests
{
    using NUnit.Framework;
    using QualityBot.RequestPocos;
    using QualityBot.Scrapers;
    using QualityBot.Scrapers.Facades;
    using OpenQA.Selenium;
    using QualityBot.Scrapers.Interfaces;

    [TestFixture]
    class FacadeFactoryTests
    {
        [Test]
        public void VerifyCreateFacadePhantomJs()
        {
            using (var facade = FacadeFactory.CreateFacade(new Request { Browser = "phantomjs" }))
            {
                Assert.IsTrue(facade is PhantomJsFacade);
            }
        }

        [Test]
        public void VerifyCreateFacadeWebdriver()
        {
            using (var facade = FacadeFactory.CreateFacade(new Request { Url = "http://www.google.com", Browser = "firefox" }))
            {
                Assert.IsTrue(facade is WebDriverFacade);
            }
            using (var facade = FacadeFactory.CreateFacade(new Request { Url = "http://www.google.com",Browser = "asdfasdf" }))
            {
                Assert.IsTrue(facade is WebDriverFacade);
            }
        }

        [Test]
        public void VerifyCreateFacadeIBrowser()
        {
            var request = new Request
            {
                Url = "http://www.google.com",
                Browser = "firefox"
            };
            var webdriver = NSubstitute.Substitute.For<IWebDriver, ITakesScreenshot, IJavaScriptExecutor>();
            using (var facade = FacadeFactory.CreateFacade(webdriver, request))
            {
                Assert.IsTrue(facade is WebDriverFacade);
            }
        }

        [Test]
        public void VerifyCreateFacadeEdgeCases()
        {
            var request = new Request
            {
                Url = "http://www.google.com",
                Browser = "PHANTOMJS"
            };
            var webdriver = NSubstitute.Substitute.For<IWebDriver, ITakesScreenshot, IJavaScriptExecutor>();
            using (var facade = FacadeFactory.CreateFacade(webdriver, request))
            {
                Assert.IsTrue(facade is PhantomJsFacade);
            }
        }
    }
}