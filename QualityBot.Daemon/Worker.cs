namespace QualityBotDaemon
{
    using System;
    using System.Threading;
    using Newtonsoft.Json;
    using QualityBot;

    public class Worker
    {
        private IJobsDb _db;

        private IService _service;

        public bool DoWork { get; set; }

        public int PollRate { get; set; }

        public Worker(IService service, IJobsDb jobsDb)
        {
            _db = jobsDb;
            DoWork = true;
            _service = service;
            PollRate = 10000;
        }

        public void DoCompare(Job job)
        {
            var request = JsonConvert.DeserializeObject<dynamic>(job.Metadata);
            var comparisons = _service.CompareDynamic(request.requestA, request.requestB);
            job.CompletedId = comparisons[0].Id.ToString();
            job.Status = "C";
        }

        public void DoJob(Job job)
        {
            Console.WriteLine("Starting Job: {0}", job.Id);
            try
            {
                switch (job.Type)
                {
                    case "compare":
                        DoCompare(job);
                        break;
                }

                Console.WriteLine("Finished Job: {0}", job.Id);
            }
            catch (Exception e)
            {
                job.Status = "E";
                job.ErrorText = JsonConvert.SerializeObject(e);
                Console.WriteLine("Error Job: {0}", job.Id);
            }

            _db.UpdateJob(job);
        }

        /// <summary>
        /// Does work until no work is found, and then it polls for more work every 10 seconds.
        /// Could multi-thread this, but it's just as trivial to load multiple instances.
        /// </summary>
        public void Poll()
        {
            var job = _db.GetNextJob();
            if (job == null)
            {
                Console.WriteLine("No jobs.");
                Thread.Sleep(PollRate);
            }

            while (DoWork)
            {
                if (job != null)
                {
                    DoJob(job);
                }

                job = _db.GetNextJob();

                if (job != null)
                {
                    continue;
                }

                Console.WriteLine("No jobs.");
                Thread.Sleep(PollRate);
            }
        }
    }
}