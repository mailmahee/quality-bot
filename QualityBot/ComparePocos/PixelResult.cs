namespace QualityBot.ComparePocos
{
    using System.Collections.Generic;

    public class PixelResult
    {
        public string[] Images { get; set; }

        public decimal PercentChanged { get; set; }

        protected bool Equals(PixelResult other)
        {
            return PercentChanged == other.PercentChanged;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((PixelResult)obj);
        }

        public override int GetHashCode()
        {
            return PercentChanged.GetHashCode();
        }

        public static bool operator ==(PixelResult left, PixelResult right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PixelResult left, PixelResult right)
        {
            return !Equals(left, right);
        }
    }
}