namespace QualityBot.Test.Tests.QualityBotTests.RequestPocosTests
{
    using System.Drawing;
    using NUnit.Framework;
    using QualityBot.RequestPocos;

    [TestFixture]
    class RequestTests
    {
        private Request _request;
        private string _url = "http://www.google.com";
        private string _browser = "firefox";
        private string _browserVersion = "10.1";
        private Size _viewport = new Size(800,600);
        [Test]
        public void VerifyRequestArgs()
        {
            //Act
            _request = new Request(_url);
            //Assert
            Assert.IsTrue(_request.Url == _url);
            //Act
            _request = new Request(_url, _browser);
            //Assert
            Assert.IsTrue(_request.Url == _url);
            Assert.IsTrue(_request.Browser == _browser);
            //Act
            _request = new Request(_url, _browser, _viewport);
            //Assert
            Assert.IsTrue(_request.Url == _url);
            Assert.IsTrue(_request.Browser == _browser);
            Assert.IsTrue(_request.ViewportResolution == _viewport);
            //Act
            _request = new Request(_url, _browser, _browserVersion);
            //Assert
            Assert.IsTrue(_request.Url == _url);
            Assert.IsTrue(_request.Browser == _browser);
            Assert.IsTrue(_request.BrowserVersion == _browserVersion);
            //Act
            _request = new Request(_url, _browser, _browserVersion, _viewport);
            //Assert
            Assert.IsTrue(_request.Url == _url);
            Assert.IsTrue(_request.Browser == _browser);
            Assert.IsTrue(_request.BrowserVersion == _browserVersion);
            Assert.IsTrue(_request.ViewportResolution == _viewport);
        }
    }
}
