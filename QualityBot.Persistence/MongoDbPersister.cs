namespace QualityBot.Persistence
{
    using System.Reflection;
    using System.Collections.Generic;
    using System.IO;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;
    using QualityBot.Util;

    public class MongoDbPersister
    {
        private static string _mongoDbName;

        private static dynamic _settings;

        private MongoDatabase _database;

        private MongoServer _server;

        public MongoDbPersister()
        {
            Config();

            _database = ConnectToMongoDb();
        }

        public static string MongoHost { get; set; }

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

        public string MongoDbName
        {
            get
            {
                return _mongoDbName;
            }
            set
            {
                _mongoDbName = value;
            }
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
            var collection = _database.GetCollection<T>(CollectionName<T>());
            collection.Insert(item);
        }

        /// <summary>
        /// Load objects from MongoDB.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <returns></returns>
        public IEnumerable<T> LoadFromMongoDb<T>(string id)
        {
            // Right now it only finds by id
            var collection = _database.GetCollection<T>(CollectionName<T>());
            var oId = new ObjectId(id);
            var data = collection.Find(Query.EQ("_id", oId));
            return data;
        }

        private static void Config()
        {
            if (_settings == null)
            {
                string result;
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("QualityBot.Persistence.mongoDbSettings.json"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }

                _settings = SettingsUtil.ReadSettingsFromString(result);
            }

            if (string.IsNullOrWhiteSpace(MongoHost))
            {
                MongoHost = SettingsUtil.GetConfig("mongoHost", _settings);
            }

            if (string.IsNullOrWhiteSpace(_mongoDbName))
            {
                _mongoDbName = SettingsUtil.GetConfig("mongoDbName", _settings);
            }
        }

        private string CollectionName<T>()
        {
            return string.Format("{0}s", typeof(T).Name).ToLowerInvariant();
        }
    }
}