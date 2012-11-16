namespace QualityBot.ComparePocos
{
    using System;
    using MongoDB.Bson;
    using QualityBot.Persistence;
    using QualityBot.Util;

    public class Comparison : IPersist
    {
        public Comparison()
        {
            Path = new StringAsReference();
        }

        public ScrapeHybrid[] Scrapes { get; set; }

        public ObjectId Id { get; set; }

        public string IdString
        {
            get
            {
                return Id.ToString();
            }
        }

        public StringAsReference Path { get; set; }

        public DateTime TimeStamp { get; set; }

        public PageResult Result { get; set; }

        protected bool Equals(Comparison other)
        {
            return Result.Equals(other.Result)
                && Scrapes.Equals(other.Scrapes);
        }

        /// <summary>
        /// Compares this Comparison to another and determines if they are equivalent.
        /// Does not compare screenshots or the HTML diff.
        /// </summary>
        /// <param name="obj">The other Comparison object.</param>
        /// <returns>A bool value indicating equality.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            return obj.GetType() == GetType() && Equals((Comparison)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Result != null ? Result.GetHashCode() : 0) * 397) ^ (Scrapes != null ? Scrapes.GetHashCode() : 0);
            }
        }

        public static bool operator ==(Comparison left, Comparison right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Comparison left, Comparison right)
        {
            return !Equals(left, right);
        }
    }
}