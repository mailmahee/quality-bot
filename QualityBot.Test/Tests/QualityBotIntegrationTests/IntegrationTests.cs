namespace QualityBot.Test.Tests.QualityBotIntegrationTests
{
    using System.Drawing;
    using System.Net;
    using MongoDB.Bson;
    using NUnit.Framework;
    using Newtonsoft.Json;
    using QualityBot.RequestPocos;
    using QualityBot.Test.Tests.Base;
    using QualityBot.Util;

    [TestFixture]
    public class IntegrationTests : BaseTest
    {
        [Test, Category("Integration")]
        public void RunComparisonUsingGrid()
        {
            var _service = new Service();
            var rA = new Request
            {
                BoundingRectangle = new Rectangle(0, 0, 0, 0),
                Browser = "firefox",
                BrowserVersion = "10.1",
                ExcludeJquerySelector = "",
                IncludeJquerySelector = "body*",
                Id = new ObjectId(),
                Path = new StringAsReference(),
                Script = "",
                Url = "http://www.google.com/"
            };
            var rB = new Request
            {
                BoundingRectangle = new Rectangle(0, 0, 0, 0),
                Browser = "firefox",
                BrowserVersion = "10.1",
                ExcludeJquerySelector = "",
                IncludeJquerySelector = "body*",
                Id = new ObjectId(),
                Path = new StringAsReference(),
                Script = "",
                Url = "http://www.ancestry.com/"
            };
            var comparison = _service.Compare(rA, rB,false);
            Assert.NotNull(comparison[0]);
            Assert.NotNull(comparison[0].Scrapes);
            Assert.IsTrue(comparison[0].Scrapes.Length > 1);
            Assert.NotNull(comparison[0].Result);
            Assert.NotNull(comparison[0].Id);
            Assert.NotNull(comparison[0].IdString);
            Assert.NotNull(comparison[0].TimeStamp);
        }

        [Test, Category("Integration")]
        public void RunComparisonUsingPhantom()
        {
            var _service = new Service();
            var rA = JsonConvert.DeserializeObject(@"{""viewportResolution"":""800,600"",""boundingRectangle"":""0,0,0,0"",""browser"":""phantomjs"",""browserVersion"":"""",""excludeJquerySelector"":"""",""includeJquerySelector"":""'body *'"",""script"":"""",""url"":""http://dna.ancestrydev.com/""}");
            var rB = JsonConvert.DeserializeObject(@"{""viewportResolution"":""800,600"",""boundingRectangle"":""0,0,0,0"",""browser"":""phantomjs"",""browserVersion"":"""",""excludeJquerySelector"":"""",""includeJquerySelector"":""'body *'"",""script"":"""",""url"":""http://dna.ancestrydev.com/""}");

            var comparison = _service.CompareDynamic(rA, rB, false);
            Assert.NotNull(comparison[0]);
            Assert.NotNull(comparison[0].Scrapes);
            Assert.IsTrue(comparison[0].Scrapes.Length > 1);
            Assert.NotNull(comparison[0].Result);
            Assert.NotNull(comparison[0].Id);
            Assert.NotNull(comparison[0].IdString);
            Assert.NotNull(comparison[0].TimeStamp);
        }
    }
}