namespace QualityBot.Test.Tests.QualityBotTests.CompareTests
{
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using NUnit.Framework;
    using QualityBot.Compare;
    using QualityBot.ScrapePocos;

    [TestFixture]
    class ComparerTests
    {
        internal static Stream GetResourceStream(string resourceName)
        {
            var thisExe = Assembly.GetExecutingAssembly();
            var file = thisExe.GetManifestResourceStream(resourceName);
            return file;
        }
        private Scrape _scrapeOne;
        private Scrape _scrapeTwo;
        private Comparer _comparer;

        [TestFixtureSetUp]
        public void Setup()
        {
            IFormatter formatter = new BinaryFormatter();
            _scrapeOne = (Scrape)formatter.Deserialize(GetResourceStream("QualityBot.Test.Tests.TestData.FakeAncestryDevScrape.bin"));
            _scrapeTwo = (Scrape)formatter.Deserialize(GetResourceStream("QualityBot.Test.Tests.TestData.FakeAncestryStageScrape.bin"));
            _comparer = new Comparer();
        }
        [Test, Category("Unit")]
        public void VerifyCompareSame()
        {
            var result = _comparer.Compare(_scrapeOne, _scrapeOne);
            Assert.NotNull(result);
            Assert.NotNull(result.Id);
            Assert.NotNull(result.Path);
            Assert.NotNull(result.Result);
            Assert.NotNull(result.Scrapes);
            Assert.IsTrue(result.Scrapes.Length > 1);
            Assert.NotNull(result.TimeStamp);
        }
        [Test, Category("Unit")]
        public void VerifyCompare()
        {
            var result = _comparer.Compare(_scrapeOne, _scrapeTwo);
            Assert.NotNull(result);
            Assert.NotNull(result.Id);
            Assert.NotNull(result.Path);
            Assert.NotNull(result.Result);
            Assert.NotNull(result.Scrapes);
            Assert.IsTrue(result.Scrapes.Length > 1);
            Assert.NotNull(result.TimeStamp);
        }
    }
}
