namespace QualityBot.ComparePocos
{
    using System.Collections.Generic;
    using System.Linq;
    using QualityBot.Util;

    public class CssChange
    {
        public List<CssChangeDetail> Changed { get; set; }

        public Dictionary<string, string> Added { get; set; }

        public Dictionary<string, string> Deleted { get; set; }

        protected bool Equals(CssChange other)
        {
            return Changed.OrderBy(s => s).SequenceEqual(other.Changed.OrderBy(t => t))
                && Added.DictionaryEqual(other.Added)
                && Deleted.DictionaryEqual(other.Deleted);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((CssChange)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Changed != null ? Changed.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Added != null ? Added.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Deleted != null ? Deleted.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(CssChange left, CssChange right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CssChange left, CssChange right)
        {
            return !Equals(left, right);
        }
    }
}