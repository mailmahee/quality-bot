namespace QualityBot.RequestPocos
{
    using System.Drawing;
    using MongoDB.Bson;
    using QualityBot.Persistence;
    using QualityBot.Util;

    public class Request : IPersist
	{
        private Rectangle? _boundingRectangle;

        private string _excludeJquerySelector;

        private string _includeJquerySelector;

        private Size? _viewportResolution;

        public Request()
        {
            Initialize();
        }

        public Request(string url)
        {
            Initialize();
            Url = url;
        }

        public Request(string url, string browser, string browserVersion, Size viewportResolution)
        {
            Initialize();
            _viewportResolution = viewportResolution;
            Url = url;
            Browser = browser;
            BrowserVersion = browserVersion;
        }

        public Request(string url, string browser, Size viewportResolution)
        {
            Initialize();
            _viewportResolution = viewportResolution;
            Url = url;
            Browser = browser;
        }

        public Request(string url, string browser, string browserVersion)
        {
            Initialize();
            Url = url;
            Browser = browser;
            BrowserVersion = browserVersion;
        }

        public Request(string url, string browser)
        {
            Url = url;
            Browser = browser;
        }

        public Rectangle? BoundingRectangle
        {
            get
            {
                return _boundingRectangle ?? (_boundingRectangle = new Rectangle(0, 0, 0, 0));
            }
            set
            {
                _boundingRectangle = value;
            }
        }

        public string Browser { get; set; }

        public string BrowserVersion { get; set; }

        public string ExcludeJquerySelector
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_excludeJquerySelector)) _excludeJquerySelector = "''";
                return _excludeJquerySelector;
            }
            set
            {
                _excludeJquerySelector = value;
            }
        }

        public ObjectId Id { get; set; }

        public string IdString
        {
            get
            {
                return Id.ToString();
            }
        }

        public string IncludeJquerySelector
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_includeJquerySelector)) _includeJquerySelector = "'body *'";
                return _includeJquerySelector;
            }
            set
            {
                _includeJquerySelector = value;
            }
        }

        public StringAsReference Path { get; set; }

        public string Script { get; set; }

        public string Url { get; set; }

        public Size? ViewportResolution
        {
            get
            {
                return _viewportResolution ?? (_viewportResolution = new Size(800, 600));
            }
            set
            {
                _viewportResolution = value;
            }
        }

		private void Initialize()
		{
            Path = new StringAsReference();
        }

	}
}