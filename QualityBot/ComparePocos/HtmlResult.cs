namespace QualityBot.ComparePocos
{
    using System.Collections.Generic;

    public class HtmlResult
    {
        public HtmlResult()
        {
            Images = new List<string>();
        }

        public List<string> Images { get; set; }

        public decimal PercentChanged { get; set; }

        protected bool Equals(HtmlResult other)
        {
            return PercentChanged == other.PercentChanged;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((HtmlResult)obj);
        }

        public override int GetHashCode()
        {
            return PercentChanged.GetHashCode();
        }

        public static bool operator ==(HtmlResult left, HtmlResult right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(HtmlResult left, HtmlResult right)
        {
            return !Equals(left, right);
        }
    }
}