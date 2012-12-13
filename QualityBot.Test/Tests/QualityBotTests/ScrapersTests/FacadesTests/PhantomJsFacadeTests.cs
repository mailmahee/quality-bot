namespace QualityBot.Test.Tests.QualityBotTests.ScrapersTests.FacadesTests
{
    using NUnit.Framework;
    using QualityBot.RequestPocos;
    using QualityBot.Scrapers.Facades;

    [TestFixture]
    class PhantomJsFacadeTests
    {
        private Request _request;
        private PhantomJsFacade _facade;

        [TestFixtureSetUp]
        public void Setup()
        {
            _request = new Request()
            {
                Url = "http://www.google.com/",
                Browser = "phantomjs"
            };
            _facade = new PhantomJsFacade(_request);
            
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _facade.Dispose();
        }

        [Test]//Integration Test
        public void VerifyPhantomJsFacade()
        {
            var result = _facade.ScrapeData();
            Assert.IsTrue(result != null);
        }
    }
}
