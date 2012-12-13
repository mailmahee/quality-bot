namespace QualityBot.Test.Tests.QualityBotIntegrationTests
{
    using System;
    using System.Linq;
    using NUnit.Framework;
    using QualityBot.Compare;
    using QualityBot.RequestPocos;
    using QualityBot.Test.Tests.Base;

    //[TestFixture]
    class ServiceTests : BaseTest
    {
        //[Test]
        public void VerifyScrapeWithWebDriver()
        {
            //Arrange
            var qbService = new Service();
            var request = new Request
            {
                Url = "http://www.google.com/",
                Browser = "firefox"
            };

            //Act & Assert
            Assert.DoesNotThrow(() =>
            {
                var result = qbService.Scrape(request);
                Assert.IsTrue(result.Url == request.Url);
                Assert.IsTrue(result.Browser == request.Browser);
            });
        }
        //[Test]
        public void VerifyScrapeWithPhantomJS()
        {
            //Arrange
            var qbService = new Service();
            var request = new Request
            {
                Url = "http://www.google.com/",
                Browser = "phantomjs"
            };

            //Act & Assert
            Assert.DoesNotThrow(() =>
            {
                var result = qbService.Scrape(request);
                Assert.IsTrue(result.Url == request.Url);
                Assert.IsTrue(result.Browser == request.Browser);
            });
            
        }
        //[Test]
        public void CanCompareScrapes()
        {
            //Arrange
            var tmpScraper = new Service();
            var tmpScrapeA = tmpScraper.Scrape(new Request("www.ancestry.com"));
            var tmpMongoIdA = new MongoDB.Bson.ObjectId(DateTime.Now, 0, 0, 0);
            tmpScrapeA.Id = tmpMongoIdA;

            var tmpScrapeB = tmpScraper.Scrape(new Request("www.ancestrystage.com"));
            var tmpMongoIdB = new MongoDB.Bson.ObjectId(DateTime.Now, 0, 0, 0);
            tmpScrapeB.Id = tmpMongoIdB;

            //Act
            var qbService = new Comparer();
            var retvalComparison = qbService.Compare(tmpScrapeA, tmpScrapeB);

            //Assert
            Assert.AreEqual(retvalComparison.Scrapes[0].IdString, tmpScrapeA.IdString, "Compare failed: Invalid ScrapeIDs Expected: {0}", tmpMongoIdA);
            Assert.AreEqual(retvalComparison.Scrapes[1].IdString, tmpScrapeB.IdString, "Compare failed: Invalid ScrapeIDs Expected: {0}", tmpMongoIdB);
        }

        //[Test]
        public void CanPersistScrape()
        {
            //Arrange
            var qbService = new Service();

            //Act
            var myScrape = qbService.Scrape("www.google.com");

            //Assert
            Assert.IsTrue(!String.IsNullOrEmpty(myScrape.Id.ToString()));
        }

        //[Test]
        public void CanPersistComparison()
        {
            //Arrange
            var qbService = new Service();
            var myScrapeA = qbService.Scrape("www.google.com", false);
            var myScrapeB = qbService.Scrape("www.google.com", false);

            //Act
            var comparer = new Comparer();
            var myComparison = comparer.Compare(myScrapeA, myScrapeB);

            //Assert
            Assert.IsTrue(!String.IsNullOrEmpty(myComparison.Id.ToString()), "Comparison Id is Null or Empty: {0}", myComparison.Id.ToString());
        }

        //[Test]
        public void CanCompareTwoUrls()
        {
            //Arrange
            var urlbaseline = "www.ancestry.com";
            var urldelta = "www.ancestry.com";
            
            //Act
            var qbService = new Service();
            var tmpComparison = qbService.Compare(urlbaseline, urldelta).First();

            //Assert

            Assert.IsFalse(String.IsNullOrEmpty(tmpComparison.IdString), "Comparison Id is Null or Empty: {0}", tmpComparison.Id.ToString());
        }
    }
}