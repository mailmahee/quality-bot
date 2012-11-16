namespace QualityBot.ServiceLibrary
{
    using System.ServiceModel;

    [ServiceContract]
    public interface IQualityBotService
    {
        [OperationContract]
        dynamic Scrape(string url, bool persist);
        [OperationContract]
        dynamic Compare(string urlA, string urlB, bool persist = true);
        [OperationContract]
        dynamic CompareDynamic(dynamic requestA, dynamic requestB, bool persist = true);
        [OperationContract]
        dynamic ScrapeDynamic(dynamic request);
        [OperationContract]
        dynamic CompareScrapeIds(string scrapeIdA, string scrapeIdB, bool persist);
    }
}
