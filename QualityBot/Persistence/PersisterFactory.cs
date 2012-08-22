namespace QualityBot.Persistence
{
    using QualityBot.ComparePocos;
    using QualityBot.Enums;
    using QualityBot.RequestPocos;
    using QualityBot.ScrapePocos;

    public static class PersisterFactory
    {
        public static Persister<Scrape> CreateScrapePersisterInstance()
        {
            var scrapePersister = new ScrapePersister();
            var persister = new Persister<Scrape>(scrapePersister) { PersistenceMethod = PersistenceMethod.MongoDb };

            return persister;
        }

        public static Persister<Comparison> CreateComparePersisterInstance()
        {
            var comparePersister = new ComparePersister();
            var persister = new Persister<Comparison>(comparePersister) { PersistenceMethod = PersistenceMethod.MongoDb };

            return persister;
        }

        public static Persister<Request> CreateRequestPersisterInstance()
        {
            var requestPersister = new RequestPersister();
            var persister = new Persister<Request>(requestPersister) { PersistenceMethod = PersistenceMethod.MongoDb };

            return persister;
        }

        public static Persister<Scrape> CreateScrapePersisterInstance(string outputDirectory)
        {
            var scrapePersister = new ScrapePersister();
            var persister = new Persister<Scrape>(scrapePersister)
                { PersistenceMethod = PersistenceMethod.File, OutputDir = outputDirectory };

            return persister;
        }

        public static Persister<Comparison> CreateComparePersisterInstance(string outputDirectory)
        {
            var comparePersister = new ComparePersister();
            var persister = new Persister<Comparison>(comparePersister)
                { PersistenceMethod = PersistenceMethod.File, OutputDir = outputDirectory };

            return persister;
        }

        public static Persister<Request> CreateRequestPersisterInstance(string outputDirectory)
        {
            var requestPersister = new RequestPersister();
            var persister = new Persister<Request>(requestPersister)
                { PersistenceMethod = PersistenceMethod.File, OutputDir = outputDirectory };

            return persister;
        }
    }
}