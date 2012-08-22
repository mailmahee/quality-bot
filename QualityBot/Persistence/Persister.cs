namespace QualityBot.Persistence
{
    using System;
    using System.Collections.Generic;
    using QualityBot.Enums;

    public class Persister<T> where T : IPersist
	{
		public PersistenceMethod PersistenceMethod { get; set; }

        public string OutputDir { get; set; }

        private IPersister<T> _persister;

		public Persister(IPersister<T> persister)
        {
            _persister = persister;
		}

        public void Save(T item)
		{
			switch (PersistenceMethod)
			{
				case PersistenceMethod.MongoDb:
                    _persister.SaveToMongoDb(item);
					break;
				case PersistenceMethod.File:
        			_persister.SaveToDisc(OutputDir, item);
					break;
				default:
					throw new Exception("Persistence Method not defined");
			}
		}

		/// <summary>
		/// Load objects.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public IEnumerable<T> Load(T item)
		{
			switch (PersistenceMethod)
			{
				case PersistenceMethod.MongoDb:
					return _persister.RetrieveFromMongoDb(item);
				case PersistenceMethod.File:
					return new[] { LoadFromFileSystem(item) };
				default:
					throw new Exception("Persistence Method not defined");
			}
		}

		/// <summary>
        /// Load from the file system.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public T LoadFromFileSystem(T item)
		{
            var filename = item.Path.ToString();
            return LoadFromFileSystem(filename);
		}

        /// <summary>
        /// Load from the file system.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public T LoadFromFileSystem(string fileName)
        {
            var data = _persister.RetrieveFromDisc(fileName);
            return data;
        }
	}
}