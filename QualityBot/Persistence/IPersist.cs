namespace QualityBot.Persistence
{
    using MongoDB.Bson;
    using QualityBot.Util;

    public interface IPersist
    {
        ObjectId Id { get; set; }

        StringAsReference Path { get; set; }
    }
}