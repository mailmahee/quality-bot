namespace QualityBot.Persistence
{
    using System.Collections.Generic;

    public interface IPersister<T>
    {
        T RetrieveFromDisc(string file);

        IEnumerable<T> RetrieveFromMongoDb(T data);

        void SaveToDisc(string outputDir, T data);
        
        void SaveToMongoDb(T data);
    }
}