namespace QualityBot.Util.Impls
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    using QualityBot.Util.Interfaces;

    public class WebHeadClientImpl : WebClient, IWebHeadClient
    {
        private WebRequestClient _client;

        public WebHeadClientImpl()
        {
            _client = GetWebRequestClient();
        }

        public Resource[] HeadCheck(IEnumerable<object> resources)
        {
            var pageResources = new List<Resource>();
            foreach (var resource in resources.Where(r => r != null).Select(r => r.ToString()).Where(r => !string.IsNullOrWhiteSpace(r)))
            {
                string statusDescription;
                HttpStatusCode statusCode;
                var headers = new List<string>();

                try
                {
                    _client.DoGetUrlHeadOnly(resource);
                }
                catch
                {
                    /* This will fail if for most status codes (404 would be the classic example).
                     * This can safely be ignored.
                     */
                }
                finally
                {
                    statusCode = _client.StatusCode;
                    statusDescription = _client.StatusDescription;
                    if (_client.ResponseHeaders != null)
                    {
                        headers.AddRange(from key in _client.ResponseHeaders.AllKeys where key != null select string.Format("{0}:{1}", key, _client.ResponseHeaders[key] ?? string.Empty));
                    }
                }

                pageResources.Add(
                    new Resource
                        {
                            Uri = resource,
                            StatusCode = statusCode,
                            StatusDescription = statusDescription,
                            Headers = headers
                        });
            }

            return pageResources.ToArray();
        }

        /// <summary>
        /// Gets an instance of the robust web client.
        /// </summary>
        /// <returns>
        /// A <c>RobustWebClient</c> instance.
        /// </returns>
        private WebRequestClient GetWebRequestClient()
        {
            // Setup cookie container and WebRequest wrapper
            var client = new WebRequestClient();

            // Setup request headers
            client.Headers.Clear();
            client.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            client.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.5");
            client.Headers.Add(HttpRequestHeader.Accept, "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            client.Headers.Add(HttpRequestHeader.AcceptCharset, "ISO-8859-1,utf-8;q=0.7,*;q=0.7");

            return client;
        }
    }
}