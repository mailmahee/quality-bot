namespace QualityBot.ScrapePocos
{
    using System.Collections.Generic;
    using System.Drawing;
    using QualityBot.Util;

    public class ElementInfo
    {
        public string Html { get; set; }

        public string Tag { get; set; }

        public Rectangle LocationOnScreenshot { get; set; }

        public Rectangle Location { get; set; }

        public Dictionary<string, string> Css { get; set; }

        public Dictionary<string, string> Attributes { get; set; }

        public ElementInfo CorrespondingElement { get; set; }

        public string Text { get; set; }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Html != null ? Html.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Tag != null ? Tag.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ LocationOnScreenshot.GetHashCode();
                hashCode = (hashCode * 397) ^ Location.GetHashCode();
                hashCode = (hashCode * 397) ^ (Css != null ? Css.GetHashCode() : 0);
                return hashCode;
            }
        }

        protected bool Equals(ElementInfo other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((ElementInfo)obj);
        }

        public static bool operator == (ElementInfo e1, ElementInfo e2)
        {
            var e1Null = ReferenceEquals(null, e1);
            var e2Null = ReferenceEquals(null, e2);

            if (e1Null && e2Null) return true;
            if (e1Null || e2Null) return false;

            return (e1.Html == e2.Html 
                    && e1.LocationOnScreenshot.Equals(e2.LocationOnScreenshot)
                    && e1.Location.Equals(e2.Location) 
                    && e1.Css.DictionaryEqual(e2.Css)
                    && e1.Text == e2.Text);
        }

        public static bool operator !=(ElementInfo e1, ElementInfo e2)
        {
            return !(e1 == e2);
        }
    }
}