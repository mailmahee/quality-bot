namespace QualityBot.Test.Tests.QualityBotDaemonTests
{
    using System;
    using System.Diagnostics;
    using MongoDB.Bson;
    using NSubstitute;
    using NUnit.Framework;
    using QualityBot;
    using QualityBot.ComparePocos;
    using QualityBot.Util;
    using QualityBotDaemon;

    [TestFixture]
    public class WorkerTests
    {
        [Test, Category("Unit")]
        public void WorkerSleepsIfNoJobsFound()
        {
            // Arrange
            var service   = Substitute.For<IService>();
            var db        = Substitute.For<IJobsDb>();
            db.GetNextJob().ReturnsForAnyArgs((Job)null);
            var worker    = new Worker(service, db) { PollRate = 1000 };
            worker.DoWork = false;

            // Act
            var sw = Stopwatch.StartNew();
            worker.Poll();
            sw.Stop();

            // Assert
            Assert.That(sw.ElapsedMilliseconds, Is.AtLeast(950));
        }

        [Test, Category("Unit")]
        public void WorkerSleepsUntilJobFound()
        {
            // Arrange
            var service = Substitute.For<IService>();
            var db = Substitute.For<IJobsDb>();
            var worker = new Worker(service, db) { PollRate = 500 };
            SubstituteExtensions.ReturnsForAnyArgs(
                service.CompareDynamic(Arg.Any<dynamic>(), Arg.Any<dynamic>(), Arg.Any<bool>()),
                new[] { new Comparison { Id = new ObjectId(Randomator.RandomFromSample(24, "0123456789ABCDEF")) } });

            // Third try will return a job
            var job = FakeJobs.PendingCompareJob;
            db.GetNextJob().ReturnsForAnyArgs(null, null, job);
            var counter = 0;

            // Stop work on the third call
            db.When(j => j.GetNextJob()).Do(x => { if (++counter == 4) worker.DoWork = false; });

            //  Act
            var sw = Stopwatch.StartNew();
            worker.Poll();
            sw.Stop();

            // Assert
            Assert.That(sw.ElapsedMilliseconds, Is.AtLeast(950));
            Assert.AreEqual("C", job.Status);
        }

        [Test, Category("Unit")]
        public void DoJobSuccess()
        {
            // Arrange
            var service = Substitute.For<IService>();
            var db = Substitute.For<IJobsDb>();
            var worker = new Worker(service, db);
            var id = Randomator.RandomFromSample(24, "0123456789ABCDEF");
            SubstituteExtensions.ReturnsForAnyArgs(
                service.CompareDynamic(Arg.Any<dynamic>(), Arg.Any<dynamic>(), Arg.Any<bool>()),
                new[] { new Comparison { Id = new ObjectId(id) } });

            // Act
            var job = FakeJobs.PendingCompareJob;
            worker.DoJob(job);

            // Assert
            Assert.AreEqual("C", job.Status);
            Assert.AreEqual(job.CompletedId.ToLower(), id.ToLower());
            db.Received(1).UpdateJob(Arg.Any<Job>());
        }

        [Test, Category("Unit")]
        public void DoJobError()
        {
            // Arrange
            var service = Substitute.For<IService>();
            var db = Substitute.For<IJobsDb>();
            var worker = new Worker(service, db);
            service.When(s => s.CompareDynamic(Arg.Any<dynamic>(), Arg.Any<dynamic>(), Arg.Any<bool>()))
                   .Do(x => { throw new Exception("Comparison failed"); });

            // Act
            var job = FakeJobs.PendingCompareJob;
            worker.DoJob(job);

            // Assert
            Assert.AreEqual("E", job.Status);
            Assert.IsTrue(string.IsNullOrWhiteSpace(job.CompletedId));
            Assert.IsTrue(job.ErrorText.Contains("Comparison failed"));
            db.Received(1).UpdateJob(Arg.Any<Job>());
        }

        [Test, Category("Unit")]
        public void GetJobReturnsPendingJob()
        {
            // Arrange
            var db = Substitute.For<IJobsDb>();
            db.GetNextJob().ReturnsForAnyArgs(FakeJobs.PendingCompareJob);

            // Act
            var job = db.GetNextJob();

            // Assert
            Assert.IsNotNull(job);
        }

        [Test, Category("Unit")]
        public void GetJobReturnsNull()
        {
            // Arrange
            var db = Substitute.For<IJobsDb>();

            // Act
            var job = db.GetNextJob();

            // Assert
            Assert.IsNull(job);
        }
    }
}