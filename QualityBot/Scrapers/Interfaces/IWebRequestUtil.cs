namespace QualityBot.Scrapers.Interfaces
{
    using System.Collections.Generic;

    using QualityBot.ScrapePocos;

    public interface IWebRequestUtil
    {
        Resource[] HeadCheck(IEnumerable<object> resources);
    }
}