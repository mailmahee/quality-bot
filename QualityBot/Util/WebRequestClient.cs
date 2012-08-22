namespace QualityBot.Util
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Script.Serialization;
    using System.Xml;
    using HtmlAgilityPack;
    using Webinator.Toolboxes;
    using Webinator.Util;
    using Webinator.WebClient;

    /// <summary>
    /// A Wrapper for WebClientBase.
    /// </summary>
    public class WebRequestClient : WebRequestClientBase
    {
        /// <summary>
        /// Json deserializer.
        /// </summary>
        private readonly JavaScriptSerializer _serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebRequestClient"/> class.
        /// </summary>
        public WebRequestClient()
            : base(new CookieContainer())
        {
            _serializer = new JavaScriptSerializer();
            _serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebRequestClient"/> class.
        /// </summary>
        /// <param name="cookies">The cookies.</param>
        public WebRequestClient(CookieContainer cookies)
            : base(cookies)
        {
            _serializer = new JavaScriptSerializer();
            _serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
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
        /// Does a multipart post that contains the specified values and files.
        /// </summary>
        /// <param name="valuesDictionary">The values dictionary { form key, value }.</param>
        /// <param name="filesToUpload">The files dictionary { path, mime type, form key }.</param>
        /// <param name="url">The url.</param>
        /// <param name="contentDisposition">The content disposition.</param>
        /// <returns>The response.</returns>
        public string DoPostMultipart(Dictionary<string, string> valuesDictionary, IEnumerable<FileToUpload> filesToUpload, string url, string contentDisposition = "form-data")
        {
            // Setup the request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            //// Don't copy the headers, it causes the response to be garbled

            request.CookieContainer = CookieContainer;
            request.Method = "POST";
            request.KeepAlive = true;
            string boundary = CreateFormDataBoundary();
            request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
            Stream requestStream = request.GetRequestStream();

            // Add form data to stream
            requestStream = WriteMultipartFormData(valuesDictionary, requestStream, boundary);

            // Add file to stream
            foreach (var file in filesToUpload)
            {
                if (string.IsNullOrWhiteSpace(file.FileLocation))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("--" + boundary);
                    sb.AppendLine(string.Format(@"Content-Disposition: {0}; name=""{1}""; filename=""{2}""", contentDisposition, file.FormKey, file.FileLocation));
                    sb.AppendLine(string.Format("Content-Type: {0}", file.MimeType));
                    sb.AppendLine();

                    byte[] headerbytes = Encoding.UTF8.GetBytes(sb.ToString());
                    requestStream.Write(headerbytes, 0, headerbytes.Length);
                }
                else
                {
                    FileInfo fileToUpload = new FileInfo(file.FileLocation);
                    requestStream = WriteMultipartFormData(fileToUpload, requestStream, boundary, file.MimeType, file.FormKey);
                }
            }

            // Complete the stream
            byte[] endBytes = Encoding.UTF8.GetBytes(string.Format("--{0}--\r\n", boundary));
            requestStream.Write(endBytes, 0, endBytes.Length);
            requestStream.Close();

            string response = null;
            using (WebResponse webResponse = request.GetResponse())
            {
                var stream = webResponse.GetResponseStream();
                if (stream != null)
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        response = reader.ReadToEnd();
                    }
                }
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
        /// Gets the content from the Url as a deserialized json object.
        /// </summary>
        /// <param name="url">The url.</param>
        /// <returns>The get response.</returns>
        public dynamic DoGetUrlJson(string url)
        {
            Headers.Add(HttpRequestHeader.Accept, "application/json; charset=UTF-8");
            var response = DownloadString(url);

            // Deserialize json
            dynamic data = _serializer.Deserialize(response, typeof(object));

            return data;
        }

        /// <summary>
        /// Gets the content from the Url as a deserialized json object.
        /// </summary>
        /// <param name="url">The url.</param>
        /// <returns>The get response.</returns>
        public dynamic DoGetUrlJsonp(string url)
        {
            Headers.Add(HttpRequestHeader.Accept, "application/json; charset=UTF-8");
            var response = DownloadString(url);

            // Deserialize json
            var json = JsonToolbox.JsonpToJson(response);
            dynamic data = _serializer.Deserialize(json, typeof(object));

            return data;
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
        /// Creates a multipart/form-data boundary.
        /// </summary>
        /// <returns>
        /// A dynamically generated form boundary for use in posting multipart/form-data requests.
        /// </returns>
        private string CreateFormDataBoundary()
        {
            return "---------------------------" + DateTime.Now.Ticks.ToString("x");
        }

        /// <summary>
        /// Writes a dictionary to a stream as a multipart/form-data set.
        /// </summary>
        /// <param name="dictionary">The dictionary of form values to write to the stream.</param>
        /// <param name="stream">The stream to which the form data should be written.</param>
        /// <param name="mimeBoundary">The MIME multipart form boundary string.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if <paramref name="stream" /> or <paramref name="mimeBoundary" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown if <paramref name="mimeBoundary" /> is empty.
        /// </exception>
        /// <remarks>
        /// If <paramref name="dictionary" /> is <see langword="null" /> or empty,
        /// nothing will be written to the stream.
        /// </remarks>
        /// <returns>A stream containing appended dictionary values.</returns>
        private Stream WriteMultipartFormData(Dictionary<string, string> dictionary, Stream stream, string mimeBoundary)
        {
            if (dictionary == null || dictionary.Count == 0)
            {
                return stream;
            }

            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (mimeBoundary == null)
            {
                throw new ArgumentNullException("mimeBoundary");
            }

            if (mimeBoundary.Length == 0)
            {
                throw new ArgumentException("MIME boundary may not be empty.", "mimeBoundary");
            }

            foreach (var key in dictionary.Keys)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("--" + mimeBoundary);
                sb.AppendLine(string.Format(@"Content-Disposition: form-data; name=""{0}""", key));
                sb.AppendLine();
                sb.AppendLine(dictionary[key]);

                byte[] itemBytes = Encoding.UTF8.GetBytes(sb.ToString());
                stream.Write(itemBytes, 0, itemBytes.Length);
            }

            return stream;
        }

        /// <summary>
        /// Writes a file to a stream in multipart/form-data format.
        /// </summary>
        /// <param name="file">The file that should be written.</param>
        /// <param name="stream">The stream to which the file should be written.</param>
        /// <param name="mimeBoundary">The MIME multipart form boundary string.</param>
        /// <param name="mimeType">The MIME type of the file.</param>
        /// <param name="formKey">The name of the form parameter corresponding to the file upload.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if any parameter is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown if <paramref name="mimeBoundary"/>, <paramref name="mimeType"/>,
        /// or <paramref name="formKey"/> is empty.
        /// </exception>
        /// <exception cref="System.IO.FileNotFoundException">
        /// Thrown if <paramref name="file"/> does not exist.
        /// </exception>
        /// <returns>A stream containing appended file.</returns>
        private Stream WriteMultipartFormData(FileInfo file, Stream stream, string mimeBoundary, string mimeType, string formKey)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            if (!file.Exists)
            {
                throw new FileNotFoundException("Unable to find file to write to stream.", file.FullName);
            }

            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (mimeBoundary == null)
            {
                throw new ArgumentNullException("mimeBoundary");
            }

            if (mimeBoundary.Length == 0)
            {
                throw new ArgumentException("MIME boundary may not be empty.", "mimeBoundary");
            }

            if (mimeType == null)
            {
                throw new ArgumentNullException("mimeType");
            }

            if (mimeType.Length == 0)
            {
                throw new ArgumentException("MIME type may not be empty.", "mimeType");
            }

            if (formKey == null)
            {
                throw new ArgumentNullException("formKey");
            }

            if (formKey.Length == 0)
            {
                throw new ArgumentException("Form key may not be empty.", "formKey");
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("--" + mimeBoundary);
            sb.AppendLine(string.Format(@"Content-Disposition: file; name=""{0}""; filename=""{1}""", formKey, file.Name));
            sb.AppendLine(string.Format("Content-Type: {0}", mimeType));
            sb.AppendLine();

            byte[] headerbytes = Encoding.UTF8.GetBytes(sb.ToString());
            stream.Write(headerbytes, 0, headerbytes.Length);
            using (FileStream fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[1024];
                int bytesRead;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    stream.Write(buffer, 0, bytesRead);
                }

                fileStream.Close();
            }

            byte[] newlineBytes = Encoding.UTF8.GetBytes(Environment.NewLine);
            stream.Write(newlineBytes, 0, newlineBytes.Length);

            return stream;
        }

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
