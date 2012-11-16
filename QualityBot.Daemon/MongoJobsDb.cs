namespace QualityBotDaemon
{
    using System.Linq;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;
    using QualityBot.Persistence;

    public class MongoJobsDb : IJobsDb
    {
        private MongoDbPersister _db;

        private MongoCollection<Job> _jobCollection;

        public MongoJobsDb()
        {
            _db = new MongoDbPersister();
            _jobCollection = _db.Database.GetCollection<Job>("jobs");
        }

        public Job GetNextJob()
        {
            // Get "P"ending jobs
            var query = Query.EQ("Status", "P");

            // Update status to "A"ctive
            var update = Update.Set("Status", "A");

            // Return the oldest
            var sort = SortBy.Ascending("_id");

            // Atomically get oldest pending job and update status to "A"ctive
            var job = _jobCollection.FindAndModify(query, sort, update, true);

            return job.GetModifiedDocumentAs<Job>();
        }

        public void UpdateJob(Job job)
        {
            _jobCollection.Save(job);
        }
    }
}