namespace QualityBot.Test.Tests
{
    using System.Drawing;
    using System.Linq;

    using QualityBot.RequestPocos;
    using QualityBot.Test.Tests.Base;
	using NUnit.Framework;
    using QualityBot;
    using QualityBot.Util;

    [TestFixture]
    public class IntegrationTests : BaseTest
    {
        [Test]
        public void ElementAdded()
        {
            var comparer = new Comparer();
            var scraper = new Scraper();

            var requestA = new Request("google.com", "firefox", "10.1");
            var requestB = new Request("google.com", "firefox", "10.1")
                {
                    Script  = @"var e = $('#gbqfba').clone();"
                            + @"$(e).children().first().text('New Button');"
                            + @"$(e).children().first().attr('id', 'newText');"
                            + @"$(e).attr('id', 'newButton');"
                            + @"$('#gbqfba').parent().append(e);"
                };
            var scrapes = scraper.Scrape(requestA, requestB);
            var comparison = comparer.Compare(scrapes[0], scrapes[1]);

            Assert.AreEqual(0, comparison.Result.RemovedItems.Count);
            Assert.AreEqual(2, comparison.Result.AddedItems.Count);

            OutputToFile(comparison, scrapes);
        }

        [Test]
        public void ElementChangedAndAdded()
        {
            var comparer = new Comparer();
            var scraper = new Scraper();

            var requestA = new Request("google.com", "firefox", "10.1");
            var requestB = new Request("google.com", "firefox", "10.1")
                {
                    Script  = @"var e = $('#gbqfba').clone();"
                            + @"$(e).children().first().text('New Button');"
                            + @"$(e).children().first().attr('id', 'newText');"
                            + @"$(e).attr('id', 'newButton');"
                            + @"$('#gbqfba').parent().append(e);"
                            + @"$('#gbqfba').remove();"
                };
            var scrapes = scraper.Scrape(requestA, requestB);
            var comparison = comparer.Compare(scrapes[0], scrapes[1]);

            OutputToFile(comparison, scrapes);
        }

        [Test]
        public void ElementChanged()
        {
            var comparer = new Comparer();
            var scraper = new Scraper();

            var requestA = new Request("google.com", "firefox", "10.1");
            var requestB = new Request("google.com", "firefox", "10.1") { Script = @"$('#gbqfba').children().first().text('Text Changed');" };
            var scrapes = scraper.Scrape(requestA, requestB);
            var comparison = comparer.Compare(scrapes[0], scrapes[1]);

            Assert.AreEqual(0, comparison.Result.RemovedItems.Count);
            Assert.AreEqual(0, comparison.Result.AddedItems.Count);

            OutputToFile(comparison, scrapes);
        }

        [Test]
        public void ElementRemoved()
        {
            var comparer = new Comparer();
            var scraper = new Scraper();

            var requestA = new Request("google.com", "firefox", "10.1");
            var requestB = new Request("google.com", "firefox", "10.1") { Script = @"$('#gbqfba').remove();" };
            var scrapes = scraper.Scrape(requestA, requestB);
            var comparison = comparer.Compare(scrapes[0], scrapes[1]);

            Assert.AreEqual(2, comparison.Result.RemovedItems.Count);
            Assert.AreEqual(0, comparison.Result.AddedItems.Count);

            OutputToFile(comparison, scrapes);
        }

        [Test]
        public void BigSites()
        {
            var comparer = new Comparer();
            var scraper = new Scraper();

            var requestA = new Request("hotmail.com", "firefox", "10.1");
            var requestB = new Request("google.com", "firefox", "10.1");
            var scrapes = scraper.Scrape(requestA, requestB);
            var comparison = comparer.Compare(scrapes[0], scrapes[1]);

            OutputToFile(comparison, scrapes);
        }

        [Test]
        public void ExcludeJquerySelector()
        {
            var scraper = new Scraper();

            var requestA = new Request("google.com", "firefox", "10.1") { ExcludeJquerySelector = "'button,button span'" };
            var requestB = new Request("google.com", "firefox", "10.1");
            var scrapes = scraper.Scrape(requestA, requestB);

            Assert.AreEqual(scrapes[0].Elements.Count, scrapes[1].Elements.Count - 4);

            OutputToFile(null, scrapes);
        }

        [Test]
        public void FFvsChrome()
        {
            var comparer = new Comparer();
            var scraper = new Scraper();

            var requestA = new Request("google.com", "firefox", "10.1");
            var requestB = new Request("google.com", "chrome");
            var scrapes = scraper.Scrape(requestA, requestB);
            var comparison = comparer.Compare(scrapes[0], scrapes[1]);

            Assert.Greater(70, comparison.Result.CssChangePercentage);

            OutputToFile(comparison, scrapes);
        }

        [Test]
        public void IncludeJquerySelectorShouldDetectLocationChanges()
        {
            var comparer = new Comparer();
            var scraper = new Scraper();

            var requestA = new Request("google.com", "firefox", "10.1") { IncludeJquerySelector = "'button'" };
            var requestB = new Request("google.com", "firefox", "10.1") { IncludeJquerySelector = "'button'", Script = @"$('#gbqfba').remove()" };
            var scrapes = scraper.Scrape(requestA, requestB);
            var comparison = comparer.Compare(scrapes[0], scrapes[1]);

            Assert.IsTrue(comparison.Result.ChangedItems.First().LocationPercentageChange > 0);

            OutputToFile(comparison, scrapes);
        }

        [Test]
        public void IncludeJquerySelector()
        {
            var scraper = new Scraper();

            var request = new Request("google.com", "firefox", "10.1") { IncludeJquerySelector = "'button,button span'" };
            var scrape = scraper.Scrape(request);

            Assert.AreEqual(4, scrape.Elements.Count);

            OutputToFile(null, scrape);
        }

        [Test]
        public void BoundingRectangle()
        {
            var scraper = new Scraper();

            var request = new Request("google.com", "firefox", "10.1") { BoundingRectangle = new Rectangle(10, 10, 200, 300) };
            var scrape = scraper.Scrape(request);

            var image = ImageUtil.Base64ToImage(scrape.Screenshot);
            Assert.AreEqual(200, image.Width);
            Assert.AreEqual(300, image.Height);

            OutputToFile(null, scrape);
        }

        [Test]
        public void StageVsLive()
        {
            var comparer = new Comparer();
            var scraper = new Scraper();

            var requestA = new Request("http://www.ancestrystage.com", "firefox", "10.1");
            var requestB = new Request("http://www.ancestry.com", "firefox", "10.1");
            var scrapes = scraper.Scrape(requestA, requestB);
            var comparison = comparer.Compare(scrapes[0], scrapes[1]);
            
            // TODO: assert

            OutputToFile(comparison, scrapes);
        }
    }
}