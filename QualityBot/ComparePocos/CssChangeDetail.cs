namespace QualityBot.ComparePocos
{
    public class CssChangeDetail
    {
        public string Key { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        protected bool Equals(CssChangeDetail other)
        {
            return string.Equals(Key, other.Key) 
                && string.Equals(From, other.From) 
                && string.Equals(To, other.To);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((CssChangeDetail)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Key != null ? Key.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (From != null ? From.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (To != null ? To.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(CssChangeDetail left, CssChangeDetail right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CssChangeDetail left, CssChangeDetail right)
        {
            return !Equals(left, right);
        }
    }
}