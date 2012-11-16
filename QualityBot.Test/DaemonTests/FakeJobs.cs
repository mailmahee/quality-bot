namespace QualityBot.Test.DaemonTests
{
    using MongoDB.Bson;
    using Newtonsoft.Json;
    using QualityBot.Util;
    using QualityBotDaemon;

    internal static class FakeJobs
    {
        public static Job PendingCompareJob
        {
            get
            {
                var rA = @"{""boundingRectangle"":"",,,"",""browser"":""chrome"",""browserVersion"":"" "",""includeJquerySelector"":""'body *'"",""excludeJquerySelector"":"""",""script"":"""",""viewportResolution"":""800,600"",""url"":""http://www.google.com""}";
                var rB = @"{""boundingRectangle"":"",,,"",""browser"":""chrome"",""browserVersion"":"" "",""includeJquerySelector"":""'body *'"",""excludeJquerySelector"":"""",""script"":""$QBjQuery('input').hide();"",""viewportResolution"":""800,600"",""url"":""http://www.google.com""}";
                var data = JsonConvert.SerializeObject(new { RequestA = rA, RequestB = rB, Persist = true });
                var job = new Job
                    {
                        Metadata = data,
                        Status = "P",
                        Type = "compare",
                        Id = new ObjectId(Randomator.RandomFromSample(24, "0123456789ABCDEF"))
                    };

                return job;
            }
        }

        public static Job[] OneOfEach
        {
            get
            {

                var pendingJob = new Job
                {
                    Metadata = string.Empty,
                    Status = "P",
                    Type = "compare",
                    Id = new ObjectId(Randomator.RandomFromSample(24, "0123456789ABCDEF"))
                };

                var activeJob = new Job
                {
                    Metadata = string.Empty,
                    Status = "A",
                    Type = "compare",
                    Id = new ObjectId(Randomator.RandomFromSample(24, "0123456789ABCDEF"))
                };

                var completedJob = new Job
                {
                    Metadata = string.Empty,
                    Status = "C",
                    Type = "compare",
                    Id = new ObjectId(Randomator.RandomFromSample(24, "0123456789ABCDEF")),
                    CompletedId = Randomator.RandomFromSample(24, "0123456789ABCDEF")
                };

                var erroredJob = new Job
                {
                    Metadata = string.Empty,
                    Status = "E",
                    Type = "compare",
                    Id = new ObjectId(Randomator.RandomFromSample(24, "0123456789ABCDEF"))
                };

                return new[] { pendingJob, activeJob, completedJob, erroredJob };
            }
        }
    }
}