namespace QualityBot.Test.MediaServiceTests
{
    using NUnit.Framework;
    using System.Net;

    [TestFixture]
    class MediaServiceAliveTests
    {
        [Test]
        public void VerifyMediaServiceIsOnline()
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(@"http://m.mfcreativedev.com/");

            Assert.DoesNotThrow(() => webRequest.GetResponse());
        }
    }
}