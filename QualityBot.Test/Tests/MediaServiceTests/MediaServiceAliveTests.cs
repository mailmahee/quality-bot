namespace QualityBot.Test.Tests.MediaServiceTests
{
    using NUnit.Framework;

    using System.Net;

    [TestFixture]
    class MediaServiceAliveTests
    {
        [Test, Category("Integration")]
        public void VerifyMediaServiceIsOnline()
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(@"http://m.mfcreativedev.com/");

            Assert.DoesNotThrow(() => webRequest.GetResponse());
        }
    }
}