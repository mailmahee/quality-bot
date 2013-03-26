using System;
using System.Linq;
using System.Threading;
using QualityBot;
using QualityBot.RequestPocos;
using QualityBot.ComparePocos;

namespace QualityBot.Example
{
	public class Program
	{
		private static void Main(string[] args)
		{
			CompareStageAndProduction();
			CompareFirefoxAndChrome();
			ComparePageOverTime();
		}

		public static void CompareStageAndProduction()
		{
			var qb = new QualityBot.Service();
			var comparisons = qb.Compare("http://www.ancestry.com", "http://www.ancestrystage.com");
			var comparison = comparisons.Single();

			DisplayComparisonInfo(comparison);
		}
		
		public static void CompareFirefoxAndChrome()
		{
			var firefox = new Request("http://www.ancestrystage.com", Browsers.Firefox, "10.0");
			var chrome = new Request("http://www.ancestrystage.com", Browsers.Chrome);

			var qb = new QualityBot.Service();
			var comparisons = qb.CompareDynamic(firefox, chrome);
			var comparison = comparisons.Single();

			DisplayComparisonInfo(comparison);
		}

		public static void ComparePageOverTime()
		{
			var qb = new QualityBot.Service();

			var request = new Request("http://www.ancestrystage.com");
			request.Browser = Browsers.IE;
			request.BrowserVersion = "9";

			var scrapeId1 = qb.ScrapeDynamic(request);
			Console.WriteLine("scrape ID: {0}", scrapeId1);

			Thread.Sleep(10000);

			var scrapeId2 = qb.ScrapeDynamic(request);
			Console.WriteLine("scrape ID: {0}", scrapeId2);

			var comparisons = qb.CompareScrapeIds(scrapeId1, scrapeId2, true);
			var comparison = comparisons.First();
			DisplayComparisonInfo(comparison);
		}
		
		public static void DisplayComparisonInfo(QualityBot.ComparePocos.Comparison comparison)
		{
			Console.WriteLine("comparison ID: " + comparison.IdString);
			Console.WriteLine("HTML diff: " + comparison.Result.Html.PercentChanged);
			Console.WriteLine("Pixel diff: " + comparison.Result.Pixels.PercentChanged);

			var scrape = comparison.Scrapes.First();
			Console.WriteLine("First scrape");
			Console.WriteLine("Id: " + scrape.IdString);
			Console.WriteLine("Url: " + scrape.Url);
			Console.WriteLine("Browser: {0}, {1}", scrape.Browser, scrape.BrowserVersion);
			Console.WriteLine("OS: " + scrape.Platform);
			Console.WriteLine("timestamp: " + scrape.TimeStamp);

			var html = scrape.Html.Value;
			var screenshot = scrape.Screenshot;
		}
	}
}

namespace QualityBot.Example {
	public static class Browsers
	{
		public const string Firefox = "firefox";
		public const string Chrome = "chrome";
		public const string IE = "iexplorer"; // What is the right string for IE?
	}
}
