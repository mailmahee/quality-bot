namespace QualityBot.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;
    using QualityBot.RequestPocos;

    public class RequestPersister : IPersister<Request>
    {
        private MongoDbPersister _mongoDbPersister;

        private MongoDbPersister MongoDbPersister
        {
            get
            {
                return _mongoDbPersister ?? (_mongoDbPersister = new MongoDbPersister());
            }
        }

        public Request RetrieveFromDisc(string file)
        {
            var requestData = File.ReadAllText(file);
            var request = JsonConvert.DeserializeObject<Request>(requestData);

            return request;
        }

        public IEnumerable<Request> RetrieveFromMongoDb(string id)
        {
            return MongoDbPersister.LoadFromMongoDb<Request>(id);
        }

        public void SaveToDisc(string outputDir, Request data)
        {
            Directory.CreateDirectory(outputDir);
            var now = DateTime.Now.ToString("yyyyMMddHHmmssfffffff");

            var filename = string.Format(@"{0}_request.json", now);
            var path = Path.Combine(outputDir, filename);

            data.Path.Value = path;
            var json = JsonConvert.SerializeObject(data);
            File.WriteAllText(path, json);
        }

        public void SaveToMongoDb(Request data)
        {
            MongoDbPersister.InsertItemInCollection(data);
        }
    }
}