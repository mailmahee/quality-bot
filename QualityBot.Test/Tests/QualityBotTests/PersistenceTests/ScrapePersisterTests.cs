namespace QualityBot.Test.Tests.QualityBotTests.PersistenceTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using NUnit.Framework;
    using QualityBot.Persistence;
    using QualityBot.ScrapePocos;
    using QualityBot.Test.Tests.QualityBotFake;

    [TestFixture]
    class ScrapePersisterTests
    {
        readonly ScrapePersister _sp = new ScrapePersister();
        private const string _path = @"C:\Test\";
        private Scrape _scrape;

        [TestFixtureSetUp]
        public void Setup()
        {
            _scrape = new QBFake().FakeScrape(new FakeScrapeParams());
        }

        [TestFixtureTearDown]
        public void Cleanup()
        {
            foreach (var file in Directory.GetFiles(_path))
            {
                do
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (IOException)
                    {}
                    
                } while (File.Exists(file));
            }

            Directory.Delete(_path);
        }
        

        [Test]
        public void VerifyRetrieveFromDisc()
        {
            _sp.SaveToDisc(_path, _scrape);
            var json = Directory.GetFiles(_path);
            Scrape result = null;
            foreach (var s in json)
            {
                if (s.Contains(".json"))
                {
                    result = _sp.RetrieveFromDisc(s);
                }
            }
            Assert.IsTrue(result != null);
        }

        [Test]
        public void VerifySaveToDisc()
        {

            _sp.SaveToDisc(_path, _scrape);

            Assert.IsTrue(Directory.GetFiles(_path).Length > 0);
        }
    }
}
