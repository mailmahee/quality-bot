namespace QualityBot.ComparePocos
{
    using System.Collections.Generic;
    using System.Linq;

    public class PageResult
    {
        public ElementChangeResult[] ChangedItems { get; set; }

        public ElementAddRemoveResult[] AddedItems { get; set; }

        public ElementAddRemoveResult[] RemovedItems { get; set; }

        public int UnchangedItems { get; set; }

        public decimal CssChangePercentage { get; set; }

        public decimal TextChangePercentage { get; set; }

        public decimal OverallElementPositionChangePercentage { get; set; }

        public PixelResult Pixels { get; set; }

        public HtmlResult Html { get; set; }
        
        public string HtmlDiff { get; set; }

        protected bool Equals(PageResult other)
        {
            return AddedItems.OrderBy(s => s).SequenceEqual(other.AddedItems.OrderBy(t => t))
                   && RemovedItems.OrderBy(s => s).SequenceEqual(other.RemovedItems.OrderBy(t => t))
                   && ChangedItems.OrderBy(s => s).SequenceEqual(other.ChangedItems.OrderBy(t => t))
                   && UnchangedItems == other.UnchangedItems
                   && CssChangePercentage == other.CssChangePercentage
                   && TextChangePercentage == other.TextChangePercentage
                   && OverallElementPositionChangePercentage == other.OverallElementPositionChangePercentage
                   && Pixels.Equals(other.Pixels)
                   && Html.Equals(other.Html)
                   && string.Equals(HtmlDiff, other.HtmlDiff);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((PageResult)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (AddedItems != null ? AddedItems.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ UnchangedItems;
                hashCode = (hashCode * 397) ^ (RemovedItems != null ? RemovedItems.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ CssChangePercentage.GetHashCode();
                hashCode = (hashCode * 397) ^ TextChangePercentage.GetHashCode();
                hashCode = (hashCode * 397) ^ OverallElementPositionChangePercentage.GetHashCode();
                hashCode = (hashCode * 397) ^ (Pixels != null ? Pixels.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Html != null ? Html.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (HtmlDiff != null ? HtmlDiff.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(PageResult left, PageResult right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PageResult left, PageResult right)
        {
            return !Equals(left, right);
        }
    }
}