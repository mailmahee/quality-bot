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
					throw new ArgumentException("Persistence Method not defined");
			}
		}

		/// <summary>
		/// Load objects.
		/// </summary>
		/// <param name="value">The value, could be a path or MongoDB ID.</param>
		/// <returns></returns>
		public IEnumerable<T> Load(string value)
		{
			switch (PersistenceMethod)
			{
				case PersistenceMethod.MongoDb:
                    return _persister.RetrieveFromMongoDb(value);
				case PersistenceMethod.File:
                    return new[] { _persister.RetrieveFromDisc(value) };
				default:
					throw new ArgumentException("Persistence Method not defined");
			}
		}
	}
}