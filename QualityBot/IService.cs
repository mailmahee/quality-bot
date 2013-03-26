namespace QualityBot
{
    using OpenQA.Selenium;
    using QualityBot.ComparePocos;
    using QualityBot.RequestPocos;
    using QualityBot.ScrapePocos;

    public interface IService
    {
        Comparison[] Compare(string urlA, string urlB, bool persist = true);

        Comparison[] Compare(Request requestA, Request requestB, bool persist = true);

        Comparison[] CompareDynamic(dynamic requestA, dynamic requestB, bool persist = true);

        Comparison[] CompareScrapeIds(string scrapeIdA, string scrapeIdB, bool persist);

        Scrape Scrape(string url, bool persist = true);

        string ScrapeDynamic(dynamic request);

        Scrape ScrapeCurrent(IWebDriver webDriver, Request request);

        Scrape Scrape(Request request);
    }
}