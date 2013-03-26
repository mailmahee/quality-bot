namespace QualityBot.Util.Interfaces
{
    using System.Collections.Generic;

    public interface IWebHeadClient
    {
        Resource[] HeadCheck(IEnumerable<object> resources);
    }
}