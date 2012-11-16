namespace QualityBot.Util
{
    using System;
    using System.Net;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Base class to interact with POSTs and GETs.
    /// </summary>
    public class WebRequestClientBase : WebClient
    {
        public HttpStatusCode StatusCode;

        public string StatusDescription;

        public Uri DestinationUri;

        public long ContentLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebRequestClientBase"/> class.
        /// </summary>
        protected WebRequestClientBase()
            : this(new CookieContainer())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebRequestClientBase"/> class.
        /// </summary>
        /// <param name="cookies">
        /// The cookies.
        /// </param>
        protected WebRequestClientBase(CookieContainer cookies)
        {
            CookieContainer = cookies;
        }

        protected bool HeadOnly { get; set; }

        /// <summary>
        /// Gets or sets the cookie container.
        /// </summary>
        /// <value>
        /// The cookie container.
        /// </value>
        protected CookieContainer CookieContainer { get; set; }

        /// <summary>
        /// Gets or sets the query string.
        /// </summary>
        /// <value>
        /// The query string.
        /// </value>
        public string Data { get; set; }

        /// <summary>
        /// Sets the user agent to the specified browser.
        /// </summary>
        /// <param name="userAgent">The user agent.</param>
        public void SetupUserAgent(string userAgent)
        {
            Headers.Add(HttpRequestHeader.UserAgent, userAgent);
        }

        /// <summary>
        /// Gets a WebRequest from the URI provided.
        /// </summary>
        /// <param name="address">The URI.</param>
        /// <returns>A WebRequest object.</returns>
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request is HttpWebRequest)
            {
                (request as HttpWebRequest).CookieContainer = CookieContainer;
            }

            HttpWebRequest httpRequest = (HttpWebRequest)request;
            if (httpRequest != null)
            {
                httpRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                return httpRequest;
            }

            return null;
        }

        /// <summary>
        /// Gets a WebResponse from a given WebRequest.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>A WebResponse object.</returns>
        protected override WebResponse GetWebResponse(WebRequest request)
        {
            WebResponse response;
            HttpWebResponse httpResponse;

            if (HeadOnly && request.Method == "GET")
            {
                request.Method = "HEAD";
            }

            try
            {
                response = base.GetWebResponse(request);
            }
            catch (WebException e)
            {
                httpResponse = (HttpWebResponse)e.Response;
                if (httpResponse != null)
                {
                    StatusCode = httpResponse.StatusCode;
                    StatusDescription = httpResponse.StatusDescription;
                    ContentLength = httpResponse.ContentLength;
                }
                throw;
            }

            if (response != null)
            {
                DestinationUri = response.ResponseUri;
            }

            httpResponse = (HttpWebResponse)response;
            if (httpResponse != null)
            {
                StatusCode = httpResponse.StatusCode;
                StatusDescription = httpResponse.StatusDescription;
                ContentLength = httpResponse.ContentLength;
                httpResponse.Close();
            }

            if (response != null)
            {
                string setCookieHeader = response.Headers[HttpResponseHeader.SetCookie];

                if (setCookieHeader != null)
                {
                    setCookieHeader = Regex.Replace(setCookieHeader, "expires=.*?(;|$)", string.Empty);

                    string[] headers = setCookieHeader.Split(',');
                    foreach (string header in headers)
                    {
                        if (!string.IsNullOrWhiteSpace(header) && !header.Contains(".localhost"))
                        {
                            CookieContainer.Add(GetCookieFromSetCookieHeader(header.EndsWith(";") ? header : header + ";", request));
                        }
                    }
                }

                response.Close();
            }

            return response;
        }

        /// <summary>
        /// Retrieves a cookie from the WebRequest with a given header.
        /// </summary>
        /// <param name="headerValue">The header value.</param>
        /// <param name="request">The WebRequest.</param>
        /// <returns>A cookie.</returns>
        private static Cookie GetCookieFromSetCookieHeader(string headerValue, WebRequest request)
        {
            MatchCollection matches = Regex.Matches(headerValue, "([a-zA-Z0-9_.&-]*?)=(.*?);");
            string name = string.Empty;
            string value = string.Empty;
            string domain = request.Headers[HttpRequestHeader.Host];
            if (domain != null)
            {
                domain = Regex.Replace(domain, ":.*", string.Empty);
            }

            string path = string.Empty;
            foreach (Match m in matches)
            {
                if (m.Index == 0)
                {
                    name = m.Groups[1].Value;
                    value = m.Groups[2].Value;
                }
                else if ("domain".Equals(m.Groups[1].Value.ToLower()))
                {
                    domain = m.Groups[2].Value;
                }
                else if ("path".Equals(m.Groups[1].Value.ToLower()))
                {
                    path = m.Groups[2].Value;
                }
            }

            return new Cookie(name, value, path, domain);
        }
    }
}