namespace QualityBot.ComparePocos
{
    using System.Collections.Generic;
    using System.Drawing;
    using QualityBot.Util;

    public class ElementAddRemoveResult
    {
        public string Html { get; set; }

        public string Text { get; set; }

        public string Tag { get; set; }

        public Rectangle Location { get; set; }

        public Dictionary<string, string> Attributes { get; set; }

        public string Image { get; set; }

        public string ImageClipped { get; set; }

        public string ImageMask { get; set; }

        protected bool Equals(ElementAddRemoveResult other)
        {
            return string.Equals(Html, other.Html)
                && string.Equals(Text, other.Text)
                && string.Equals(Tag, other.Tag)
                && Location.Equals(other.Location)
                && Attributes != null 
                && Attributes.DictionaryEqual(other.Attributes);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((ElementAddRemoveResult)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Html != null ? Html.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Text != null ? Text.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Tag != null ? Tag.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Location.GetHashCode();
                hashCode = (hashCode * 397) ^ (Attributes != null ? Attributes.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(ElementAddRemoveResult left, ElementAddRemoveResult right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ElementAddRemoveResult left, ElementAddRemoveResult right)
        {
            return !Equals(left, right);
        }
    }
}
