namespace QualityBot.ComparePocos
{
    using System;
    using MongoDB.Bson;
    using QualityBot.Persistence;
    using QualityBot.Util;
    [Serializable]
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

    }
}