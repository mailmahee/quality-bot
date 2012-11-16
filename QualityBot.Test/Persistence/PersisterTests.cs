namespace QualityBot.Test.Persistence
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using MongoDB.Driver.Builders;
    using NLog;
    using NUnit.Framework;
    using QualityBot.ComparePocos;
    using QualityBot.Persistence;
    using QualityBot.RequestPocos;
    using QualityBot.ScrapePocos;
    using QualityBot.Test.Tests.Base;

    [TestFixture]
	internal class PersisterTests : BaseTest
    {
        readonly string _browser = "firefox";

        readonly string _browserVersion = "10.1";

        readonly Size _resolution = new Size(800, 600);

		private static Logger log = LogManager.GetCurrentClassLogger();

		[Test]
		public void TestConnectToMongoDb()
		{
		    var mongoDb = new MongoDbPersister();
            log.Debug("connected to MongoDB: " + mongoDb.Database);

            Assert.AreEqual(mongoDb.MongoDbName, mongoDb.Database.Name);
		}

		//[Test]
		public void TestSaveRequestToFileSystem()
		{
			var request = new Request();
			request.Url = "http://www.example.com?timestamp=" + GetTimestamp();

		    var persister = PersisterFactory.CreateRequestPersisterInstance(@"C:\OutputDirectory\TestSaveRequestToFileSystem");
			
			// cleanup
            if (Directory.Exists(persister.OutputDir)) Directory.Delete(persister.OutputDir, true);

			// save
			persister.Save(request);

			// verify
			Assert.IsTrue(Directory.Exists(persister.OutputDir));

			var files = Directory.GetFiles(persister.OutputDir);
			Assert.IsTrue(files.Any(f => f.Contains("request.json")));
		}

		//[Test]
		public void TestSaveScrapeToFileSystem()
		{
			var url = "http://www.google.com?timestamp=" + GetTimestamp();

            var scraper = new Service();
		    var request = new Request(url, _browser, _browserVersion, _resolution);
			var scrape = scraper.Scrape(request);
            
            var persister = PersisterFactory.CreateScrapePersisterInstance(@"C:\OutputDirectory\TestSaveScrapeToFileSystem");

            // cleanup
            if (Directory.Exists(persister.OutputDir)) Directory.Delete(persister.OutputDir, true);

			// save
			persister.Save(scrape);

			// verify
			Assert.IsTrue(Directory.Exists(persister.OutputDir));
			
			var files = Directory.GetFiles(persister.OutputDir);
			Assert.IsTrue(files.Any(f => f.Contains("Html.html")));
			Assert.IsTrue(files.Any(f => f.Contains("scrape.json")));
			Assert.IsTrue(files.Any(f => f.Contains("Screenshot.png")));
		}

		//[Test]
		public void TestSaveComparisonToFileSystem()
		{
			var timestamp = GetTimestamp();
			var url1 = "http://www.google.com?timestamp=" + timestamp + "&url=1";
			var url2 = "http://www.google.com?timestamp=" + timestamp + "&url=2";

			var request1 = new Request(url1);
			var request2 = new Request(url2);

			var comparison = Compare(request1, request2);

            var persister = PersisterFactory.CreateComparePersisterInstance(@"C:\OutputDirectory\TestSaveComparisonToFileSystem");

			// cleanup
		    if (Directory.Exists(persister.OutputDir)) Directory.Delete(persister.OutputDir, true);

			// save
			persister.Save(comparison);

			// verify
			Assert.IsTrue(Directory.Exists(persister.OutputDir));
			
			var files = Directory.GetFiles(persister.OutputDir);
			Assert.IsTrue(files.Any(f => f.Contains("comparison.json")));
			Assert.IsTrue(files.Any(f => f.Contains("HtmlDiff.html")));
			Assert.IsTrue(files.Any(f => f.Contains("HtmlDiff.png")));
			Assert.IsTrue(files.Any(f => f.Contains("PixelDiff.png")));
			
		}
        
		[Test]
		public void TestHasRequestsCollection()
		{
            var mongoDb = new MongoDbPersister();

            var collections = mongoDb.Database.GetCollectionNames().ToArray();

			Assert.Contains("requests", collections);

			foreach (string collectionName in collections)
			{
                log.Debug(mongoDb.Database.Name + " collection: " + collectionName);
			}

			//NOTE: this will fail if the MongoDB doesn't have the collections specified
            Assert.IsTrue(mongoDb.Database.CollectionExists("requests"));
            Assert.IsTrue(mongoDb.Database.CollectionExists("scrapes"));
            Assert.IsTrue(mongoDb.Database.CollectionExists("comparisons"));
		}

		//[Test]
		public void SaveRequestToMongoDb()
		{
			var request = new Request();
			request.Url = "http://www.example.com?timestamp=" + GetTimestamp();

			var persister = PersisterFactory.CreateRequestPersisterInstance();
			persister.Save(request);

			// verify by retrieving
            var mongoDb = new MongoDbPersister();
            var collection = mongoDb.Database.GetCollection<Request>("requests");

            // check id
            var query = Query.EQ("_id", request.Id);
            var results = collection.Find(query);
		    Assert.AreEqual(1, results.Size(), "should only have 1 result returned from MongoDB because Id is unique");
            var returned = results.First();
            Assert.AreEqual(request.Id, returned.Id);

            // check url
			query = Query.EQ("Url", request.Url);
			results = collection.Find(query);
			Assert.AreEqual(1, results.Size(), "should only have 1 result returned from MongoDB (if URL is unique)");
			returned = results.First();
            Assert.AreEqual(request.Url, returned.Url);

            // check type
			log.Debug("returned: " + returned);
			Assert.IsInstanceOf(typeof(Request), returned);
		}

		//[Test]
		public void SaveScrapeToMongoDb()
		{
			var request = new Request();
			request.Url = "http://www.google.com?timestamp=" + GetTimestamp();

			var scrape = Scrape(request);

			var persister = PersisterFactory.CreateScrapePersisterInstance();
			persister.Save(scrape);

			// verify by retrieving
            var mongoDb = new MongoDbPersister();
            var collection = mongoDb.Database.GetCollection<Scrape>("scrapes");
            
            // check id
            Console.WriteLine(scrape.Id);
            var query = Query.EQ("_id", scrape.Id);
            var results = collection.Find(query);
		    Assert.AreEqual(1, results.Size(), "should only have 1 result returned from MongoDB because Id is unique");
            var returned = results.First();
            Assert.AreEqual(scrape.Id, returned.Id);

            // check url
			query = Query.EQ("Url", scrape.Url);
			results = collection.Find(query);
			Assert.AreEqual(1, results.Size(), "should only have 1 result returned from MongoDB (if URL is unique)");
			returned = results.First();
            Assert.AreEqual(scrape.Url, returned.Url);

            // check type
			log.Debug("returned: " + returned);
			Assert.IsInstanceOf(typeof(Scrape), returned);
		}
        
		//[Test]
		public void SaveComparisonToMongoDb()
		{
			var request1 = new Request();
			request1.Url = "http://www.google.com?timestamp=" + GetTimestamp() + "&scrape=1";
	
			var request2 = new Request();
			request2.Url = "http://www.google.com?timestamp=" + GetTimestamp() + "&scrape=2";

			var comparison = Compare(request1, request2);

            var persister = PersisterFactory.CreateComparePersisterInstance();
			persister.Save(comparison);

			// verify by retrieving
            var mongoDb = new MongoDbPersister();
            var collection = mongoDb.Database.GetCollection<Comparison>("comparisons");

            // check id
            var query = Query.EQ("_id", comparison.Id);
            var results = collection.Find(query);
		    Assert.AreEqual(1, results.Size(), "should only have 1 result returned from MongoDB because Id is unique");
            var returned = results.First();
            Assert.AreEqual(comparison.Id, returned.Id);

            // check type
			log.Debug("returned: " + returned);
			Assert.IsInstanceOf(typeof(Comparison), returned);

		}

		/// HELPER METHODS ///

		private Scrape Scrape(Request request)
		{
            var scraper = new Service();
            var scrape = scraper.Scrape(request);

			return scrape;
		}
        
        private Comparison Compare(Request requestA, Request requestB)
		{
			var scrapeA = Scrape(requestA);
			var scrapeB = Scrape(requestB);

            var comparer = new Comparer();
            var compareResult = comparer.Compare(scrapeA, scrapeB);
            
			return compareResult;
		}
        
		private string GetTimestamp()
		{
			return DateTime.Now.ToString("yyyyMMddHHmmssfffffff");
		}
    }
}