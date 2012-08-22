namespace QualityBot.ScrapePocos
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using MongoDB.Bson;
    using QualityBot.Persistence;
    using QualityBot.Util;

    public class Scrape : IPersist
    {
        public Scrape()
        {
            Cookies = new List<string>();
            Path = new StringAsReference();
            HtmlRef = new StringAsReference();
            ScreenshotRef = new StringAsReference();
        }

        public ObjectId Id { get; set; }

        public StringAsReference Path { get; set; }

        public readonly List<ElementInfo> Elements = new List<ElementInfo>();

        public Resource[] Resources { get; set; }

        public string Html { get; set; }

        public StringAsReference HtmlRef { get; set; }

        public string Url { get; set; }

        public string Screenshot { get; set; }

        public StringAsReference ScreenshotRef { get; set; }

        public Size ViewportSize { get; set; }

        public string Browser { get; set; }

        public string BrowserVersion { get; set; }

        public string TimeStamp { get; set; }

        public string Platform { get; set; }

        public List<string> Cookies { get; set; }

        protected bool Equals(Scrape other)
        {
            return Elements.OrderBy(s => s).SequenceEqual(other.Elements.OrderBy(t => t))
                   && Resources.OrderBy(s => s).SequenceEqual(other.Resources.OrderBy(t => t))
                   && string.Equals(Url, other.Url) 
                   && ViewportSize.Equals(other.ViewportSize)
                   && string.Equals(Browser, other.Browser) 
                   && string.Equals(BrowserVersion, other.BrowserVersion)
                   && string.Equals(TimeStamp, other.TimeStamp) 
                   && string.Equals(Platform, other.Platform)
                   && Cookies.OrderBy(s => s).SequenceEqual(other.Cookies.OrderBy(t => t));
        }

        /// <summary>
        /// Compares this Scrape to another and determines if they are equivalent.
        /// Does not compare the Screenshot or Html.
        /// </summary>
        /// <param name="obj">The other Scrape object.</param>
        /// <returns>A bool value indicating equality.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Scrape)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Elements != null ? Elements.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Resources != null ? Resources.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Url != null ? Url.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ ViewportSize.GetHashCode();
                hashCode = (hashCode * 397) ^ (Browser != null ? Browser.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (BrowserVersion != null ? BrowserVersion.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TimeStamp != null ? TimeStamp.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Platform != null ? Platform.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Cookies != null ? Cookies.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(Scrape left, Scrape right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Scrape left, Scrape right)
        {
            return !Equals(left, right);
        }
    }
}