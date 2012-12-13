namespace QualityBot.Test.Tests.QualityBotTests.ScrapersTests
{
    using System.Collections.Generic;
    using System.IO;
    using NUnit.Framework;
    using QualityBot.Scrapers;

    [TestFixture]
    class WebRequestUtilTests
    {
        private WebRequestUtil _client;
        private IEnumerable<object> _resources;

        internal string GetFromResources(string resourceName)
        {
            var assem = this.GetType().Assembly;
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
            _client = new WebRequestUtil();
            var res = GetFromResources("QualityBot.Test.Tests.TestData.FakeResources.txt");
            _resources = res.Split('\n');
        }
        
        [Test]//Integration Test
        public void VerifyHeadCheck()
        {
            //Act
            var result = _client.HeadCheck(_resources);
            //Assert
            Assert.IsTrue((int)result[0].StatusCode != 404);
            Assert.IsTrue((int)result[1].StatusCode == 404);
        }
    }
}