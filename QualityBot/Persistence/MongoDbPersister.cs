namespace QualityBot.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Design.PluralizationServices;
    using System.Globalization;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;
    using NLog;

    using QualityBot.RequestPocos;
    using QualityBot.ScrapePocos;
    using QualityBot.Util;

    public class MongoDbPersister
    {
        private MongoDatabase _database;

        PluralizationService _pluralizationService = PluralizationService.CreateService(new CultureInfo("en-US"));

        private MongoServer _server;

        public MongoDbPersister()
        {
            MongoHost = AppConfigUtil.AppConfig("mongoHost");
            MongoDbName = AppConfigUtil.AppConfig("mongoDbName");
            _database = ConnectToMongoDb();
        }

        public MongoDatabase Database
        {
            get
            {
                return _database;
            }
            set
            {
                _database = value;
            }
        }

        public string MongoDbName { get; set; }

        public string MongoHost { get; set; }

        /// <summary>
        /// Build MongoDB queries.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public IMongoQuery ById<T>(T item) where T : IPersist
        {
            var query = Query.EQ("_id", item.Id);
            return query;
        }

        /// <summary>
        /// Set up the MongoDB connection.
        /// </summary>
        /// <returns>A MongoDB database.</returns>
        public MongoDatabase ConnectToMongoDb()
        {
            string connectionString = "mongodb://" + MongoHost + "/?safe=true";
            _server = MongoServer.Create(connectionString);
            _database = _server.GetDatabase(MongoDbName);

            return _database;
        }

        public void InsertItemInCollection<T>(T item)
        {
            var collection = _database.GetCollection<T>(CollectionName(item));
            collection.Insert(item);
        }

        public IMongoQuery Like<T>(T item)
        {
            //TODO: implement complex queries
            // this needs to be specific for each item type (e.g. scrape, comparison)
            throw new NotImplementedException();
        }

        /// <summary>
        /// Build a query to search for all elements that match the given Request.
        /// NOTE: incomplete and untested.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IMongoQuery LikeRequest(Request request)
        {
            var query = Query.Null;

            // if id exists, overwrite any other data
            if (request.Id != ObjectId.Empty)
            {
                query = Query.EQ("_id", request.Id);
            }
            else
            {
                // match all other defined valued
                var queries = new List<IMongoQuery>();

                if (request.Url != null)
                {
                    queries.Add(Query.EQ("Url", request.Url));
                }

                //TODO: add query logic for other parts of the request

                Query.And(queries);
            }

            return query;
        }

        /// <summary>
        /// Build a query to search for all elements that match the given Scrape.
        /// NOTE: incomplete and untested.
        /// </summary>
        /// <param name="scrape"></param>
        /// <returns></returns>
        public IMongoQuery LikeScrape(Scrape scrape)
        {
            IMongoQuery query;

            // if id exists, overwrite any other data
            if (scrape.Id != ObjectId.Empty)
            {
                query = Query.EQ("_id", scrape.Id);
            }
            else
            {
                // match all other defined valued
                var queries = new List<IMongoQuery>();


                if (scrape.Url != null)
                {
                    queries.Add(Query.EQ("Url", scrape.Url));
                }
                if (scrape.TimeStamp != null)
                {
                    // all scrapes with timestamp given or after 
                    queries.Add(Query.GTE("TimeStamp", scrape.TimeStamp));
                }
                //TODO: implement more search params

                query = Query.And(queries);
            }

            return query;
        }

        /// <summary>
        /// Load objects from MongoDB.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public IEnumerable<T> LoadFromMongoDb<T>(T item) where T : IPersist
        {
            // Right now it only finds by id
            return MongoQuery(item, ById(item));
        }

        /// <summary>
        /// Query the database.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public IEnumerable<T> MongoQuery<T>(T item, IMongoQuery query)
        {
            var collection = _database.GetCollection<T>(CollectionName(item));
            return collection.Find(query);
        }

        private string CollectionName<T>(T item)
        {
            return _pluralizationService.Pluralize(item.GetType().Name).ToLowerInvariant();
        }
    }
}