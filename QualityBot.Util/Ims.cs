namespace QualityBot.Util
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Xml;
    using QualityBot.Util.Enums;
    using QualityBot.Util.Impls;
    using QualityBot.Util.Interfaces;

    public class Ims
    {
        private readonly IWebUploadClient _uploadClient;

        public readonly Dictionary<string, string> EnvironmentLevelDictionary = new Dictionary<string, string>
		{
			{ "Dev"  , "ancestrydev" },
			{ "Stage", "ancestrystage" },
			{ "Live" , "ancestry" },
		};

        private readonly Dictionary<string, string> _imsLevelDictionary = new Dictionary<string, string>
		{
			{ "Dev"  , "mfsbedev" },
			{ "Stage", "mfsbestage" },
			{ "Live" , "mfsbe" },
		};

        private string _imsUrl;

        public Ims(ImsLevel level, IWebUploadClient uploadClient)
        {
            _uploadClient = uploadClient;
            Init(level);
        }

        public Ims(ImsLevel level)
        {
            _uploadClient = new WebUploadClientImpl(EnvironmentLevelDictionary[level.ToString()]);
            Init(level);
        }

        /// <summary>
        /// Retrieves the ATT token and user ID for a given username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="userId">The user ID.</param>
        /// <returns>The ATT token.</returns>
        public string GetAtt(string username, out string userId)
        {
            string xml = @"<xapi>" +
                            @"	<rq c=""autht"" f=""api_login"">" +
                            @"		<username>UCDM</username>" +
                            @"		<password>UCDMUCDM</password>" +
                            @"	</rq>" +
                            @"	<rq c=""autht"" f=""api_proxy"">" +
                            string.Format(@"		<UserName>{0}</UserName>", username) +
                            @"	</rq>" +
                            @"	<rq c=""user"" f=""api_select_user"" />" +
                            @"	<rq c=""user"" f=""api_get_username"" />" +
                            @"</xapi>";

            var response = _uploadClient.UploadString(_imsUrl, xml);
            var userXml = TransformToXml(response);
            var attNodes = userXml.SelectNodes("//ATT");
            var userIdNodes = userXml.SelectNodes("//UserID");
            userId = userIdNodes[userIdNodes.Count - 1].InnerText.ToLower();
            string att = attNodes[attNodes.Count - 1].InnerText;
            return att;
        }

        /// <summary>
        /// Initializes this class.
        /// </summary>
        /// <param name="level">The IMS level.</param>
        private void Init(ImsLevel level)
        {
            _imsUrl = string.Format("http://ims.{0}.com/exec", _imsLevelDictionary[level.ToString()]);
        }

        /// <summary>
        /// Takes a string and turns it into xml.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>
        /// An Xml Document.
        /// </returns>
        private XmlDocument TransformToXml(string response)
        {
            const string pattern = "<!--(.|\n)+?-->";   // remove comments
            const string pattern2 = "(<!.+?>)";         // removes DOCTYPE node
            const string pattern3 = "(<.xml.+?>)";      // removes xml version node
            const string pattern4 = "(xmlns.+?>)";      // removes xmlns stuff from html tag.

            response = Regex.Replace(response, pattern, string.Empty);
            response = Regex.Replace(response, pattern2, string.Empty);
            response = Regex.Replace(response, pattern3, string.Empty);
            response = Regex.Replace(response, pattern4, ">"); // put the tail back on...

            var xmlReader = XmlReader.Create(new StringReader(response));

            XmlDocument doc = new XmlDocument();
            doc.Load(xmlReader);

            return doc;
        }
    }
}