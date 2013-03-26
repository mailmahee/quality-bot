namespace QualityBot.Test.Tests.QualityBotTests.PersistenceTests
{
    using System;
    using System.IO;
    using NUnit.Framework;
    using QualityBot.Persistence;
    using QualityBot.ScrapePocos;
    using QualityBot.Test.Tests.QualityBotFake;

    [TestFixture]
    class ScrapePersisterTests
    {
        readonly ScrapePersister _sp = new ScrapePersister();
        private string _path = @"Test";
        private Scrape _scrape;
        
        [TestFixtureSetUp]
        public void Setup()
        {
            _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _path);
            Directory.CreateDirectory(_path);
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

            Directory.Delete(_path, true);
        }


        [Test, Category("Unit")]
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

        [Test, Category("Unit")]
        public void VerifySaveToDisc()
        {

            _sp.SaveToDisc(_path, _scrape);

            Assert.IsTrue(Directory.GetFiles(_path).Length > 0);
        }
    }
}
