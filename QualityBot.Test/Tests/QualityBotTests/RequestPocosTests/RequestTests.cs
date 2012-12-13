namespace QualityBot.Test.Tests.QualityBotTests.RequestPocosTests
{
    using NUnit.Framework;
    using QualityBot.RequestPocos;

    [TestFixture]
    class RequestTests
    {
        private Request fullReq;
        private Request nullBrowser;
        private Request nullBrowserAndVersion;
        private Request allNullButUrl;
        private Request nullReq;

        [TestFixtureSetUp]
        public void Setup()
        {
            fullReq = new Request
            {
                Browser = "firefox",
                BrowserVersion = "10.1",
                Script = "somescript",
                Url = "http://www.google.com"
            };
            nullBrowser = new Request
            {
                Browser = null,
                BrowserVersion = "10.1",
                Script = "somescript",
                Url = "http://www.google.com"
            };
            nullBrowserAndVersion = new Request
            {
                Browser = null,
                BrowserVersion = null,
                Script = "somescript",
                Url = "http://www.google.com"
            };
            allNullButUrl = new Request
            {
                Browser = null,
                BrowserVersion = null,
                Script = null,
                Url = "http://www.google.com"
            };
            nullReq = new Request
            {
                Browser = null,
                BrowserVersion = null,
                Script = null,
                Url = null
            };
        }
        [Test]
        public void VerifyGetHashCode()
        {
            Assert.AreNotEqual(0,fullReq.GetHashCode());
            Assert.AreNotEqual(0, nullBrowser.GetHashCode());
            Assert.AreNotEqual(0, nullBrowserAndVersion.GetHashCode());
            Assert.AreNotEqual(0, allNullButUrl.GetHashCode());
            Assert.AreEqual(0, nullReq.GetHashCode());  
        }
        [Test]
        public void VerifyRequestEquals()
        {
            Assert.IsFalse(fullReq == nullBrowser);
            Assert.IsFalse(fullReq == nullBrowserAndVersion);
            Assert.IsFalse(fullReq == allNullButUrl);
            Assert.IsFalse(fullReq == nullReq);
            Assert.IsTrue(fullReq == new Request
            {
                Browser = "firefox",
                BrowserVersion = "10.1",
                Script = "somescript",
                Url = "http://www.google.com"
            });
        }
        [Test]
        public void VerifyRequestNotEquals()
        {
            Assert.IsTrue(fullReq != nullBrowser);
            Assert.IsTrue(fullReq != nullBrowserAndVersion);
            Assert.IsTrue(fullReq != allNullButUrl);
            Assert.IsTrue(fullReq != nullReq);
            Assert.IsFalse(fullReq != new Request
            {
                Browser = "firefox",
                BrowserVersion = "10.1",
                Script = "somescript",
                Url = "http://www.google.com"
            });
        }

    }
}
