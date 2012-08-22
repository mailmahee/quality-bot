using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using QualityBot.ComparePocos;
using QualityBot.ScrapePocos;

namespace QualityBot.ServiceLibrary
{
    [ServiceContract]
    public interface IQualityBotService
    {
        [OperationContract]
        Scrape Scrape(string url, bool persist);
        [OperationContract]
        Comparison Compare(string urlA, string urlB);

    }
}
