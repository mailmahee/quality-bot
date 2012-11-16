namespace QualityBotDaemon
{
    using MongoDB.Bson;

    public class Job
    {
        public ObjectId Id { get; set; }

        public string CompletedId { get; set; }

        public string Status { get; set; }

        public string Type { get; set; }

        public string Metadata { get; set; }

        public string ErrorText { get; set; }
    }
}