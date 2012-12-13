namespace QualityBot.Test.Tests.QualityBotTests.PersistenceTests
{
    using System.IO;
    using NUnit.Framework;
    using QualityBot.Test.Tests.Base;
    using NSubstitute;
    using QualityBot.ComparePocos;
    using QualityBot.Persistence;

    [TestFixture]
    class ComparePersisterTests : BaseTest
    {
        readonly ComparePersister _cp = new ComparePersister();
        private const string _path = @"C:\Test\";
        private Comparison _comparison;

        [TestFixtureSetUp]
        public void Setup()
        {
            _comparison = new Comparison();
        }

        [TestFixtureTearDown]
        public void Cleanup()
        {
            foreach (var file in Directory.GetFiles(_path))
            {
                File.Delete(file);
            }

            Directory.Delete(_path);
        }

        [Test]
        public void VerifyRetrieveFromDisc()
        {
            //Arrange
            _cp.SaveToDisc(_path, _comparison);

            //Act
            var result = _cp.RetrieveFromDisc(Directory.GetFiles(_path)[0]);

            //Assert
            Assert.IsTrue(result != null);
        }

        [Test]
        public void VerifySaveToDisc()
        {
            //Arrange & Act
            _cp.SaveToDisc(_path, _comparison);

            //Assert
            Assert.IsTrue(Directory.GetFiles(_path).Length > 0);
        }

        //[Test]
        public void VerifyUpdateImageStyles()
        {
            Assert.Inconclusive("Not written");
        }

        //[Test]
        public void VerifyUpdateSpriteStyles()
        {
            Assert.Inconclusive("Not written");
        }

        //[Test]
        public void VerifyGetStyle()
        {
            Assert.Inconclusive("Not written");
        }

        //[Test]
        public void GetAllImages()
        {
            Assert.Inconclusive("Not written");
        }
    }
}