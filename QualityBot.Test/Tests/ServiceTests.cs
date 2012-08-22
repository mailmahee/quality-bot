using System;
using QualityBot.RequestPocos;

namespace QualityBot.Test.Tests
{
    using QualityBot.Test.Tests.Base;
    using NUnit.Framework;
    using QualityBot;

    [TestFixture]
    class ServiceTests : BaseTest
    {
        [TestCase]
        public void ValidateServiceIsNotNull()
        {
            //Arrange
            QualityBot.Service qbService;

            //Act
            qbService = new QualityBot.Service();

            //Assert
            Assert.IsInstanceOf(typeof(Service), qbService, "Object is not of type QualityBot.Service");
            Assert.IsNotNull(qbService, "Service object is Null");
        }

        [TestCase]
        public void CanScrapeWithUrL()
        {
            //Arrange
            QualityBot.Service qbService;
            string expectedUrl = "www.ancestry.com";
            string actualUrl;

            //Act
            qbService = new Service();
            var retScrape = qbService.Scrape(expectedUrl);
            actualUrl = retScrape.Url;

            //Assert
            Assert.IsTrue(actualUrl.Contains(expectedUrl), "Expected Url:{0} Actual Url:{1}", expectedUrl, actualUrl);
        }

        [TestCase]
        public void CanCompareScrapes()
        {
            //Arrange
            QualityBot.Service qbService;
            var tmpScraper = new Scraper();
            var tmpScrapeA = tmpScraper.Scrape(new Request("www.ancestry.com"));
            var tmpMongoIdA = new MongoDB.Bson.ObjectId(DateTime.Now, 0, 0, 0);
            tmpScrapeA.Id = tmpMongoIdA;

            var tmpScrapeB = tmpScraper.Scrape(new Request("www.ancestrystage.com"));
            var tmpMongoIdB = new MongoDB.Bson.ObjectId(DateTime.Now, 0, 0, 0);
            tmpScrapeB.Id = tmpMongoIdB;

            //Act
            qbService = new Service();
            var retvalComparison = qbService.Compare(tmpScrapeA, tmpScrapeB);

            //Assert
            Assert.AreEqual(retvalComparison.Scrapes[0].Id, tmpScrapeA.Id, "Compare failed: Invalid ScrapeIDs Expected: {0}", tmpMongoIdA);
            Assert.AreEqual(retvalComparison.Scrapes[1].Id, tmpScrapeB.Id, "Compare failed: Invalid ScrapeIDs Expected: {0}", tmpMongoIdB);
        }

        [TestCase]
        public void CanPersistScrape()
        {
            //Arrange
            Service qbService;

            //Act
            qbService = new Service();
            var myScrape = qbService.Scrape("www.google.com");

            //Assert
            Assert.IsTrue(!String.IsNullOrEmpty(myScrape.Id.ToString()));
        }

        [TestCase]
        public void CanPersistComparison()
        {
            //Arrange
            Service qbService;
            qbService = new Service();
            var myScrapeA = qbService.Scrape("www.google.com", false);
            var myScrapeB = qbService.Scrape("www.google.com", false);

            //Act
            var myComparison = qbService.Compare(myScrapeA, myScrapeB);

            //Assert
            Assert.IsTrue(!String.IsNullOrEmpty(myComparison.Id.ToString()), "Comparison Id is Null or Empty: {0}", myComparison.Id.ToString());
        }

        [TestCase]
        public void CanCompareTwoUrls()
        {
            //Arrange
            Service qbService;
            var urlbaseline = "www.ancestry.com";
            var urldelta = "www.ancestry.com";
            
            
            //Act
            qbService = new Service();
            var tmpComparison = qbService.Compare(urlbaseline, urldelta);

            //Assert

            Assert.IsTrue(String.IsNullOrEmpty(tmpComparison.Id.ToString()), "Comparison Id is Null or Empty: {0}", tmpComparison.Id.ToString());
        }
    }
}
