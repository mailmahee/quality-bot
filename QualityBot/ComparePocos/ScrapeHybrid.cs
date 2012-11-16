namespace QualityBot.ComparePocos
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using QualityBot.ScrapePocos;
    using QualityBot.Util;

    public class ScrapeHybrid
    {
        public ScrapeHybrid()
        {
            Cookies = new List<string>();
            Path = new StringAsReference();
            Html = new StringAsReference();
            Screenshot = new StringAsReference();
        }

        public string ExcludeJquerySelector { get; set; }

        public string IncludeJquerySelector { get; set; }

        public string Script { get; set; }

        public Rectangle? BoundingRectangle { get; set; }

        public string Description { get; set; }

        public StringAsReference IdString { get; set; }

        public StringAsReference Path { get; set; }

        public Resource[] Resources { get; set; }

        public StringAsReference Html { get; set; }

        public string Url { get; set; }

        public StringAsReference Screenshot { get; set; }

        public Size ViewportSize { get; set; }

        public string Browser { get; set; }

        public string BrowserVersion { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Platform { get; set; }

        public List<string> Cookies { get; set; }

        protected bool Equals(ScrapeHybrid other)
        {
            return Resources.OrderBy(s => s).SequenceEqual(other.Resources.OrderBy(t => t))
                   && string.Equals(Url, other.Url) 
                   && ViewportSize.Equals(other.ViewportSize)
                   && string.Equals(Browser, other.Browser)
                   && string.Equals(TimeStamp, other.TimeStamp)
                   && string.Equals(BrowserVersion, other.BrowserVersion)
                   && string.Equals(Platform, other.Platform)
                   && Cookies.OrderBy(s => s).SequenceEqual(other.Cookies.OrderBy(t => t));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true; 
            return obj.GetType() == GetType() && Equals((ScrapeHybrid)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Description != null ? Description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Resources != null ? Resources.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Url != null ? Url.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ ViewportSize.GetHashCode();
                hashCode = (hashCode * 397) ^ (Browser != null ? Browser.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TimeStamp != null ? TimeStamp.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (BrowserVersion != null ? BrowserVersion.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Platform != null ? Platform.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Cookies != null ? Cookies.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(ScrapeHybrid left, ScrapeHybrid right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ScrapeHybrid left, ScrapeHybrid right)
        {
            return !Equals(left, right);
        }
    }
}