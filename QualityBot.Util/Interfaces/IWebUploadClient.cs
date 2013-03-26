namespace QualityBot.Util.Interfaces
{
    public interface IWebUploadClient
    {
        string UploadString(string address, string data);
    }
}
