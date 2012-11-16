namespace QualityBot.RequestPocos
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

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

        protected bool Equals(Request other)
        {
            return string.Equals(Browser, other.Browser)
                   && string.Equals(BrowserVersion, other.BrowserVersion)
                   && string.Equals(Script, other.Script) 
                   && string.Equals(Url, other.Url);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Request)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Browser != null ? Browser.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (BrowserVersion != null ? BrowserVersion.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Script != null ? Script.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Url != null ? Url.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(Request left, Request right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Request left, Request right)
        {
            return !Equals(left, right);
        }
	}
}