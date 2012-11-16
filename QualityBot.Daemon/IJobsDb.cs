namespace QualityBotDaemon
{
    public interface IJobsDb
    {
        Job GetNextJob();

        void UpdateJob(Job job);
    }
}