namespace QualityBot.Test.Tests
{
    using System;
    using System.Drawing;
    using System.Linq;
    using Newtonsoft.Json;
	using NUnit.Framework;
    using QualityBot.ComparePocos;
    using QualityBot.RequestPocos;
    using QualityBot.ScrapePocos;
    using QualityBot.Test.Tests.Base;
    using QualityBot.Util;

    [TestFixture]
    public class IntegrationTests : BaseTest
    {
        //[Test, Category("Quarantine")]
        public void CompareDynamic()
        {
            var _service = new Service();

            var rA = JsonConvert.DeserializeObject<dynamic>(@"{""boundingRectangle"":"",,,"",""browser"":""phantomjs"",""browserVersion"":"""",""includeJquerySelector"":""'body *'"",""excludeJquerySelector"":"""",""script"":"""",""viewportResolution"":""800,600"",""url"":""http://www.google.com""}");
            var rB = JsonConvert.DeserializeObject<dynamic>(@"{""boundingRectangle"":"",,,"",""browser"":""phantomjs"",""browserVersion"":"""",""includeJquerySelector"":""'body *'"",""excludeJquerySelector"":"""",""script"":""$QBjQuery('input').hide();"",""viewportResolution"":""800,600"",""url"":""http://www.google.com""}");
            Comparison[] comparison = _service.CompareDynamic(rA, rB, true);

            Console.WriteLine(comparison[0].Id.ToString());
        }

        ////[Test, Category("Quarantine")]
        //public void ScrapeDynamic()
        //{
        //    var _service = new Service();

        //    var r = JsonConvert.DeserializeObject<dynamic>(@"{""boundingRectangle"":"",,,"",""browser"":""Firefox"",""browserVersion"":""10.1"",""includeJquerySelector"":""'body *'"",""excludeJquerySelector"":"""",""script"":"""",""viewportResolution"":""800,600"",""url"":""http://www.google.com""}");
        //    dynamic scrape = _service.ScrapeDynamic(r);

        //    Console.WriteLine(scrape);
        //}

        ////[Test, Category("Quarantine")]
        //public void CompareScrapeIds()
        //{
        //    var _service = new Service();

        //    var comparison = _service.CompareScrapeIds(@"5053810fcf84f61820174a2a", @"50538257cf84f618d46dd6e7", true);

        //    Console.WriteLine(comparison[0].IdString);
        //}

        ////[Test, Category("Quarantine")]
        //public void PhantomJs()
        //{
        //    var comparer = new Comparer();
        //    var scraper = new Scraper();

        //    var requestA = new Request("http://www.google.com", "phantomjs");
        //    var requestB = new Request{ Url = "http://www.google.com", Browser = "phantomjs", Script = @"$QBjQuery(""input"").hide()" };

        //    var scrapes = scraper.Scrape(requestA, requestB);
        //    var comparison = comparer.Compare(scrapes[0], scrapes[1]);

        //    OutputToFile(comparison, scrapes);
        //}

        ////[Test, Category("Quarantine")]
        //public void FFvsIE()
        //{
        //    var comparer = new Comparer();
        //    var scraper = new Scraper();

        //    var requestA = new Request("google.com", "firefox", "10.1");
        //    var requestB = new Request("google.com", "internetexplorer");
        //    var scrapes = scraper.Scrape(requestA, requestB);
        //    var comparison = comparer.Compare(scrapes[0], scrapes[1]);

        //    OutputToFile(comparison, scrapes);
        //}

        ////[Test, Category("Quarantine")]
        //public void ElementAdded()
        //{
        //    var comparer = new Comparer();
        //    var scraper = new Scraper();

        //    var requestA = new Request("google.com", "firefox", "10.1");
        //    var requestB = new Request("google.com", "firefox", "10.1")
        //        {
        //            Script  = @"var e = $('#gbqfba').clone();"
        //                    + @"$(e).children().first().text('New Button');"
        //                    + @"$(e).children().first().attr('id', 'newText');"
        //                    + @"$(e).attr('id', 'newButton');"
        //                    + @"$('#gbqfba').parent().append(e);"
        //        };
        //    var scrapes = scraper.Scrape(requestA, requestB);
        //    var comparison = comparer.Compare(scrapes[0], scrapes[1]);

        //    Assert.AreEqual(0, comparison.Result.RemovedItems.Count);
        //    Assert.AreEqual(2, comparison.Result.AddedItems.Count);

        //    OutputToFile(comparison, scrapes);
        //}

        ////[Test, Category("Quarantine")]
        //public void ElementChangedAndAdded()
        //{
        //    var comparer = new Comparer();
        //    var scraper = new Scraper();

        //    var requestA = new Request("google.com", "firefox", "10.1");
        //    var requestB = new Request("google.com", "firefox", "10.1")
        //        {
        //            Script  = @"var e = $('#gbqfba').clone();"
        //                    + @"$(e).children().first().text('New Button');"
        //                    + @"$(e).children().first().attr('id', 'newText');"
        //                    + @"$(e).attr('id', 'newButton');"
        //                    + @"$('#gbqfba').parent().append(e);"
        //                    + @"$('#gbqfba').remove();"
        //        };
        //    var scrapes = scraper.Scrape(requestA, requestB);
        //    var comparison = comparer.Compare(scrapes[0], scrapes[1]);

        //    OutputToFile(comparison, scrapes);
        //}

        ////[Test, Category("Quarantine")]
        //public void ElementChanged()
        //{
        //    var comparer = new Comparer();
        //    var scraper = new Scraper();

        //    var requestA = new Request("google.com", "firefox", "10.1");
        //    var requestB = new Request("google.com", "firefox", "10.1") { Script = @"$('#gbqfba').children().first().text('Text Changed');" };
        //    var scrapes = scraper.Scrape(requestA, requestB);
        //    var comparison = comparer.Compare(scrapes[0], scrapes[1]);

        //    Assert.AreEqual(0, comparison.Result.RemovedItems.Count);
        //    Assert.AreEqual(0, comparison.Result.AddedItems.Count);

        //    OutputToFile(comparison, scrapes);
        //}

        ////[Test, Category("Quarantine")]
        //public void ElementRemoved()
        //{
        //    var comparer = new Comparer();
        //    var scraper = new Scraper();

        //    var requestA = new Request("google.com", "firefox", "10.1");
        //    var requestB = new Request("google.com", "firefox", "10.1") { Script = @"$('#gbqfba').remove();" };
        //    var scrapes = scraper.Scrape(requestA, requestB);
        //    var comparison = comparer.Compare(scrapes[0], scrapes[1]);

        //    Assert.AreEqual(2, comparison.Result.RemovedItems.Count);
        //    Assert.AreEqual(0, comparison.Result.AddedItems.Count);

        //    OutputToFile(comparison, scrapes);
        //}

        ////[Test, Category("Quarantine")]
        //public void BigSites()
        //{
        //    var comparer = new Comparer();
        //    var scraper = new Scraper();

        //    var requestA = new Request("hotmail.com", "firefox", "10.1");
        //    var requestB = new Request("google.com", "firefox", "10.1");
        //    var scrapes = scraper.Scrape(requestA, requestB);
        //    var comparison = comparer.Compare(scrapes[0], scrapes[1]);

        //    OutputToFile(comparison, scrapes);
        //}

        ////[Test, Category("Quarantine")]
        //public void ExcludeJquerySelector()
        //{
        //    var scraper = new Scraper();

        //    var requestA = new Request("google.com", "firefox", "10.1") { ExcludeJquerySelector = "'button,button span'" };
        //    var requestB = new Request("google.com", "firefox", "10.1");
        //    var scrapes = scraper.Scrape(requestA, requestB);

        //    Assert.AreEqual(scrapes[0].Elements.Count, scrapes[1].Elements.Count - 4);

        //    OutputToFile(null, scrapes);
        //}

        ////[Test, Category("Quarantine")]
        //public void FFvsChrome()
        //{
        //    var comparer = new Comparer();
        //    var scraper = new Scraper();

        //    var requestA = new Request("google.com", "firefox", "10.1");
        //    var requestB = new Request("google.com", "chrome");
        //    var scrapes = scraper.Scrape(requestA, requestB);
        //    var comparison = comparer.Compare(scrapes[0], scrapes[1]);

        //    Assert.Greater(70, comparison.Result.CssChangePercentage);

        //    OutputToFile(comparison, scrapes);
        //}

        ////[Test, Category("Quarantine")]
        //public void IncludeJquerySelectorShouldDetectLocationChanges()
        //{
        //    var comparer = new Comparer();
        //    var scraper = new Scraper();

        //    var requestA = new Request("google.com", "firefox", "10.1") { IncludeJquerySelector = "'button'" };
        //    var requestB = new Request("google.com", "firefox", "10.1") { IncludeJquerySelector = "'button'", Script = @"$QBjQuery('#gbqfba').remove()" };
        //    var scrapes = scraper.Scrape(requestA, requestB);
        //    var comparison = comparer.Compare(scrapes[0], scrapes[1]);

        //    Assert.IsTrue(comparison.Result.ChangedItems.First().LocationPercentageChange > 0);

        //    OutputToFile(comparison, scrapes);
        //}

        ////[Test, Category("Quarantine")]
        //public void IncludeJquerySelector()
        //{
        //    var scraper = new Scraper();

        //    var request = new Request("google.com", "firefox", "10.1") { IncludeJquerySelector = "'button,button span'" };
        //    var scrape = scraper.Scrape(request);

        //    Assert.AreEqual(4, scrape.Elements.Count);

        //    OutputToFile(null, scrape);
        //}

        ////[Test, Category("Quarantine")]
        //public void BoundingRectangle()
        //{
        //    var scraper = new Scraper();

        //    var request = new Request("google.com", "firefox", "10.1") { BoundingRectangle = new Rectangle(10, 10, 200, 300) };
        //    var scrape = scraper.Scrape(request);

        //    var image = ImageUtil.Base64ToImage(scrape.Screenshot);
        //    Assert.AreEqual(200, image.Width);
        //    Assert.AreEqual(300, image.Height);

        //    OutputToFile(null, scrape);
        //}

        ////[Test, Category("Quarantine")]
        //public void StageVsLive()
        //{
        //    var comparer = new Comparer();
        //    var scraper = new Scraper();

        //    var requestA = new Request("http://www.ancestrystage.com", "firefox", "10.1");
        //    var requestB = new Request("http://www.ancestry.com", "firefox", "10.1");
        //    var scrapes = scraper.Scrape(requestA, requestB);
        //    var comparison = comparer.Compare(scrapes[0], scrapes[1]);
            
        //    // TODO: assert

        //    OutputToFile(comparison, scrapes);
        //}

        ////[Test, Category("Quarantine")]
        //public void Foo()
        //{
        //    var comparer = new Comparer();
        //    var scraper = new Scraper();

        //    var requestA = new Request("http://www.google.com", "firefox", "10.1");
        //    var requestB = new Request("http://www.google.com", "firefox", "10.1");
        //    Scrape scrapeA = scraper.Scrape(requestA);
        //    Scrape scrapeB = scraper.Scrape(requestB);

        //    var comparison = comparer.Compare(scrapeA, scrapeB);
            
        //    // TODO: assert

        //    //OutputToFile(comparison, scrapes);
        //}
    }
}