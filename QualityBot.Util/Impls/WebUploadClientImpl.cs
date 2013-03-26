namespace QualityBot.Util.Impls
{
    using System.Net;
    using QualityBot.Util.Interfaces;

    internal sealed class WebUploadClientImpl : WebClient, IWebUploadClient
    {
        private readonly WebClient _client;

        public WebUploadClientImpl(string host)
        {
            _client = new WebClient();

            // Setup request headers
            _client.Headers.Clear();
            _client.Headers.Add(HttpRequestHeader.Host, string.Format("www.{0}.com", host));
            _client.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            _client.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.5");
            _client.Headers.Add(HttpRequestHeader.Accept, "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            _client.Headers.Add(HttpRequestHeader.AcceptCharset, "ISO-8859-1,utf-8;q=0.7,*;q=0.7");
        }
    }
}
