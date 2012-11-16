namespace QualityBot.Test.Tests
{
    using NUnit.Framework;
    using Base;
    using System.IO;
    using QualityBot.Persistence;
    using RequestPocos;
    using NSubstitute;

    [TestFixture]
    class RequestPersisterTests : BaseTest
    {
        [Test]
        public void VerifyRetrieveFromDisc()
        {
            //Arrange
            var path = "C:\\Test\\";
            var uTest = "www.duh.com";
            var req = new Request(url: uTest);
            var rP = new RequestPersister();

            //Act
            rP.SaveToDisc(path,req);
            var result = rP.RetrieveFromDisc(Directory.GetFiles(path)[0]);

            //Assert
            Assert.IsTrue(result.Url == uTest);

            //Test-Cleanup
            foreach (var file in Directory.GetFiles(path))
            {
                File.Delete(file);
            }
            Directory.Delete(path);
        }

        [Test]
        public void VerifySaveToDisc()
        {
            //Arrange
            var path = "C:\\Test\\";
            var uTest = "www.duh.com";
            var req = new Request(url: uTest);
            var rP = new RequestPersister();

            //Act
            rP.SaveToDisc(path, req);

            //Assert
            Assert.IsTrue(Directory.GetFiles(path).Length > 0);

            //Test-Cleanup
            foreach (var file in Directory.GetFiles(path))
            {
                File.Delete(file);
            }
            Directory.Delete(path);
        }

    }
}
