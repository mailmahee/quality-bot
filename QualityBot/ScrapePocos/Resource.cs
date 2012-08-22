namespace QualityBot.ScrapePocos
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    public class Resource
    {
        public Resource()
        {
            Headers = new List<string>();
        }

        public string Uri { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public string StatusDescription { get; set; }

        public List<string> Headers { get; set; }

        protected bool Equals(Resource other)
        {
            return string.Equals(Uri, other.Uri) 
                   && StatusCode.Equals(other.StatusCode)
                   && string.Equals(StatusDescription, other.StatusDescription)
                   && Headers.OrderBy(s => s).SequenceEqual(other.Headers.OrderBy(t => t));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Resource)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Uri != null ? Uri.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ StatusCode.GetHashCode();
                hashCode = (hashCode * 397) ^ (StatusDescription != null ? StatusDescription.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Headers != null ? Headers.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(Resource left, Resource right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Resource left, Resource right)
        {
            return !Equals(left, right);
        }
    }
}