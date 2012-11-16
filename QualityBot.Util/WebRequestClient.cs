namespace QualityBot.Util
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Xml;
    using HtmlAgilityPack;

    /// <summary>
    /// A Wrapper for WebClientBase.
    /// </summary>
    public class WebRequestClient : WebRequestClientBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebRequestClient"/> class.
        /// </summary>
        public WebRequestClient()
            : base(new CookieContainer())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebRequestClient"/> class.
        /// </summary>
        /// <param name="cookies">The cookies.</param>
        public WebRequestClient(CookieContainer cookies)
            : base(cookies)
        {
        }

        /// <summary>
        /// Posts the data to the url provided.
        /// </summary>
        /// <param name="url">The url.</param>
        /// <param name="data">The data.</param>
        /// <returns>The response.</returns>
        public string DoPost(string url, string data)
        {
            var response = UploadString(url, data);
            return response;
        }

        /// <summary>
        /// Posts a form to the Url.
        /// </summary>
        /// <param name="url">The url.</param>
        /// <param name="valuesDictionary">The values dictionary { form key, value }.</param>
        /// <returns>The response.</returns>
        public string DoPostForm(string url, Dictionary<string, string> valuesDictionary)
        {
            var data = WriteFormData(valuesDictionary);
            var oldContentType = Headers.Get("ContentType");
            Headers.Set(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
            var response = UploadString(url, data);
            if (!string.IsNullOrWhiteSpace(oldContentType))
            {
                Headers.Set(HttpRequestHeader.ContentType, oldContentType);
            }

            return response;
        }
        
        /// <summary>
        /// Performs a HEAD request.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <param name="statusCode">
        /// The status code returned.
        /// </param>
        /// <param name="statusDescription">
        /// The status description.
        /// </param>
        /// <returns>
        /// The Uri of the Internet resource that actually responded to the request.
        /// </returns>
        public Uri DoGetUrlHeadOnly(string url, out HttpStatusCode statusCode, out string statusDescription)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException("url");

            HeadOnly = true;
            var request = GetWebRequest(new Uri(url));
            if (request != null) GetWebResponse(request);
            HeadOnly = false;

            statusCode = StatusCode;
            statusDescription = StatusDescription;

            return DestinationUri;
        }

        /// <summary>
        /// Performs a HEAD request.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <param name="statusCode">
        /// The status code returned.
        /// </param>
        /// <returns>
        /// The Uri of the Internet resource that actually responded to the request.
        /// </returns>
        public Uri DoGetUrlHeadOnly(string url, out HttpStatusCode statusCode)
        {
            HeadOnly = true;
            var request = GetWebRequest(new Uri(url));
            if (request != null) GetWebResponse(request);
            HeadOnly = false;

            statusCode = StatusCode;

            return DestinationUri;
        }

        /// <summary>
        /// Performs a HEAD request.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <returns>
        /// The Uri of the Internet resource that actually responded to the request.
        /// </returns>
        public Uri DoGetUrlHeadOnly(string url)
        {
            HeadOnly = true;
            var request = GetWebRequest(new Uri(url));
            if (request != null) GetWebResponse(request);
            HeadOnly = false;

            return DestinationUri;
        }

        /// <summary>
        /// Gets the content from the Url.
        /// </summary>
        /// <param name="url">The url.</param>
        /// <returns>The get response.</returns>
        public string DoGetUrl(string url)
        {
            return DownloadString(url);
        }

        /// <summary>
        /// Gets the content from the Url.
        /// </summary>
        /// <param name="url">The url.</param>
        /// <param name="statusCode">The status code returned.</param>
        /// <returns>The get response.</returns>
        public string DoGetUrl(string url, out HttpStatusCode statusCode)
        {
            string response = null;
            try
            {
                response = DownloadString(url);
            }
            catch
            {
            }

            statusCode = StatusCode;

            return response;
        }

        /// <summary>
        /// Gets the content from the Url.
        /// </summary>
        /// <param name="url">The url.</param>
        /// <param name="statusCode">The status code returned.</param>
        /// <param name="statusDescription">The status description.</param>
        /// <returns>The get response.</returns>
        public string DoGetUrl(string url, out HttpStatusCode statusCode, out string statusDescription)
        {
            string response = null;
            try
            {
                response = DownloadString(url);
            }
            catch
            {
            }

            statusCode = StatusCode;
            statusDescription = StatusDescription;

            return response;
        }

        /// <summary>
        /// Gets the content from the Url.
        /// Creates the query string from the dictionary provided.
        /// </summary>
        /// <param name="url">The url.</param>
        /// <param name="valuesDictionary">The values dictionary { form key, value }.</param>
        /// <returns>The get response.</returns>
        public string DoGetUrl(string url, Dictionary<string, string> valuesDictionary)
        {
            var data = WriteFormData(valuesDictionary);
            return DownloadString(string.Format("{0}?{1}", url, data));
        }

        /// <summary>
        /// Gets the content from the Url as xml.
        /// </summary>
        /// <param name="url">The url.</param>
        /// <returns>The get response.</returns>
        public XmlDocument DoGetUrlXml(string url)
        {
            // Remove new lines
            string response = DoGetUrl(url).Replace("\n", string.Empty).Replace("\r", string.Empty);

            // remove comments
            response = System.Text.RegularExpressions.Regex.Replace(response, "<!--(.|\n)+?-->", string.Empty);

            // removes DOCTYPE
            response = System.Text.RegularExpressions.Regex.Replace(response, "(<!.+?>)", string.Empty);

            // removes xml version
            response = System.Text.RegularExpressions.Regex.Replace(response, "(<.xml.+?>)", string.Empty);

            // removes xmlns
            response = System.Text.RegularExpressions.Regex.Replace(response, "(xmlns.+?>)", ">");

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(response);
            return doc;
        }

        /// <summary>
        /// Gets the content from the Url as an Html Agility Pack document.
        /// </summary>
        /// <param name="url">The url.</param>
        /// <returns>The get response.</returns>
        public HtmlDocument DoGetUrlHtml(string url)
        {
            HtmlDocument doc = new HtmlDocument();
            Headers.Add(HttpRequestHeader.Accept, "application/x-www-form-urlencoded; charset=UTF-8");
            string html = DownloadString(url);
            doc.LoadHtml(html);

            return doc;
        }

        /// <summary>
        /// Gets the content from the Url and saves it as the specified file.
        /// </summary>
        /// <param name="url">The url.</param>
        /// <param name="filename">The filename, absolute or relative.</param>
        public void DoGetFile(string url, string filename)
        {
            byte[] bytes = DownloadData(url);
            using (var writer = new FileStream(filename, FileMode.CreateNew))
            {
                writer.Write(bytes, 0, bytes.Length);
                writer.Flush();
            }
        }

        /// <summary>
        /// Gets the content from the Url as a byte stream.
        /// </summary>
        /// <param name="url">The url.</param>
        /// <returns>
        /// The byte stream.
        /// </returns>
        public byte[] DoGetFile(string url)
        {
            byte[] bytes = DownloadData(url);

            return bytes;
        }

        /// <summary>
        /// Url encodes json.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>Url encoded json.</returns>
        public string EncodeJson(string json)
        {
            return HttpUtility.UrlEncode(json);
        }

        #region Helper Functions

        /// <summary>
        /// Given a dictionary, creates Form data to be used in a Post.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns>
        /// A string containing the Form data for use in a Post.
        /// </returns>
        private string WriteFormData(Dictionary<string, string> dictionary)
        {
            // Construct data string and url encode keys and values
            var sb = new StringBuilder();
            foreach (var key in dictionary.Keys)
            {
                if (sb.Length != 0)
                {
                    sb.Append("&");
                }

                sb.Append(key);
                sb.Append("=");
                sb.Append(HttpUtility.UrlEncode(dictionary[key]));
            }

            var data = sb.ToString();
            return data;
        }

        #endregion
    }
}
