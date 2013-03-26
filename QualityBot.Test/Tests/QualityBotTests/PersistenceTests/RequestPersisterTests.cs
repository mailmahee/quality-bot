namespace QualityBot.Test.Tests.QualityBotTests.PersistenceTests
{
    using System;
    using NUnit.Framework;
    using QualityBot.Test.Tests.Base;
    using System.IO;
    using QualityBot.Persistence;
    using QualityBot.RequestPocos;

    [TestFixture]
    class RequestPersisterTests : BaseTest
    {
        private string _path = @"Test";

        [TestFixtureSetUp]
        public void Setup()
        {
            _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _path);
            Directory.CreateDirectory(_path);
        }

        [TestFixtureTearDown]
        public void Cleanup()
        {
            Directory.Delete(_path, true);
        }

        [Test, Category("Unit")]
        public void VerifyRetrieveFromDisc()
        {
            //Arrange
            var uTest = "www.duh.com";
            var req = new Request(url: uTest);
            var rP = new RequestPersister();

            //Act
            rP.SaveToDisc(_path,req);
            var result = rP.RetrieveFromDisc(Directory.GetFiles(_path)[0]);

            //Assert
            Assert.IsTrue(result.Url == uTest);
        }

        [Test, Category("Unit")]
        public void VerifySaveToDisc()
        {
            //Arrange
            var uTest = "www.duh.com";
            var req = new Request(url: uTest);
            var rP = new RequestPersister();

            //Act
            rP.SaveToDisc(_path, req);

            //Assert
            Assert.IsTrue(Directory.GetFiles(_path).Length > 0);
        }

    }
}
