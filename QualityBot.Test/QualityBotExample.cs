using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using QualityBot;
using QualityBot.ComparePocos;
using QualityBot.ScrapePocos;

namespace QualityBot.Test
{
	class QualityBotExample
	{
		public void ExampleComparison()
		{
			Console.WriteLine("Example Comparison");

			var qb = new QualityBot.Service();
			var comparisons = qb.Compare("http://www.ancestry.com", "http://www.ancestrystage.com");

			Console.WriteLine("Done");

			var comparison = comparisons.SingleOrDefault();
			Console.WriteLine("comparison ID: " + comparison.Id);

			Console.WriteLine("HTML diff: " + comparison.Result.Html.PercentChanged);
			Console.WriteLine("Pixel diff: " + comparison.Result.Pixels.PercentChanged);
		
			var scrape = comparison.Scrapes.First();
			Console.WriteLine("First scrape");
			Console.WriteLine("Id: " + scrape.IdString);
			Console.WriteLine("Url: " + scrape.Url);
			Console.WriteLine("Browser: " + scrape.Browser + " " + scrape.BrowserVersion);
			Console.WriteLine("OS: " + scrape.Platform);
			Console.WriteLine("timestamp: " + scrape.TimeStamp);

			var html = scrape.Html.Value;
			var screenshot = scrape.Screenshot;
		}

		[Test]
		public void ComparisonAPI()
		{
			ExampleComparison();
		}
	}

}
