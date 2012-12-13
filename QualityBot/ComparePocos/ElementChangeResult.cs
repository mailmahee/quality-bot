namespace QualityBot.ComparePocos
{
    using System.Drawing;

    public class ElementChangeResult
    {
        public CssChange CssChanges { get; set; }

        public string HtmlChanges { get; set; }

        public string TextChanges { get; set; }

        public string LocationChanges { get; set; }

        public PixelChange PixelChanges { get; set; }

        public decimal CssPercentageChange { get; set; }

        public decimal HtmlPercentageChange { get; set; }

        public decimal TextPercentageChange { get; set; }

        public decimal LocationPercentageChange { get; set; }

        public decimal PixelPercentageChange { get; set; }

        public Rectangle LocationOnScreenshot { get; set; }

        protected bool Equals(ElementChangeResult other)
        {
            return CssChanges != null 
                   && other.CssChanges != null 
                   && CssChanges.Equals(other.CssChanges)
                   && string.Equals(HtmlChanges, other.HtmlChanges) 
                   && string.Equals(TextChanges, other.TextChanges)
                   && string.Equals(LocationChanges, other.LocationChanges)
                   && CssPercentageChange == other.CssPercentageChange
                   && HtmlPercentageChange == other.HtmlPercentageChange
                   && TextPercentageChange == other.TextPercentageChange
                   && LocationPercentageChange == other.LocationPercentageChange
                   && PixelPercentageChange == other.PixelPercentageChange;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((ElementChangeResult)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (CssChanges != null ? CssChanges.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (HtmlChanges != null ? HtmlChanges.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TextChanges != null ? TextChanges.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (LocationChanges != null ? LocationChanges.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (PixelChanges != null ? PixelChanges.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ CssPercentageChange.GetHashCode();
                hashCode = (hashCode * 397) ^ HtmlPercentageChange.GetHashCode();
                hashCode = (hashCode * 397) ^ TextPercentageChange.GetHashCode();
                hashCode = (hashCode * 397) ^ LocationPercentageChange.GetHashCode();
                hashCode = (hashCode * 397) ^ PixelPercentageChange.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(ElementChangeResult left, ElementChangeResult right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ElementChangeResult left, ElementChangeResult right)
        {
            return !Equals(left, right);
        }
    }
}