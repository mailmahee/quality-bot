namespace QualityBot.Scrapers.Interfaces
{
    using System;

    using QualityBot.ScrapePocos;

    public interface IBrowserFacade : IDisposable
    {
        PageData ScrapeData(bool useCurrent = true);
    }
}