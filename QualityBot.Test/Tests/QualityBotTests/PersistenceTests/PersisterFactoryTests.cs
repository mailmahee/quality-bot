namespace QualityBot.Test.Tests.QualityBotTests.PersistenceTests
{
    using NUnit.Framework;
    using QualityBot.Enums;
    using QualityBot.Persistence;

    [TestFixture]
    class PersisterFactoryTests
    {
        private string _path = @"C:\test\";
        [Test, Category("Unit")]
        public void VerifyCreateScrapePersisterInstance()
        {
            //Arrange & Act
            var persister = PersisterFactory.CreateScrapePersisterInstance();

            //Assert
            Assert.IsTrue(persister.PersistenceMethod == PersistenceMethod.MongoDb);
            Assert.IsFalse(persister.OutputDir == _path);
        }
        [Test, Category("Unit")]
        public void VerifyCreateComparePersisterInstance()
        {
            //Arrange & Act
            var persister = PersisterFactory.CreateComparePersisterInstance();

            //Assert
            Assert.IsTrue(persister.PersistenceMethod == PersistenceMethod.MongoDb);
            Assert.IsFalse(persister.OutputDir == _path);
        }
        [Test, Category("Unit")]
        public void VerifyCreateRequestPersisterInstance()
        {
            //Arrange & Act
            var persister = PersisterFactory.CreateRequestPersisterInstance();

            //Assert
            Assert.IsTrue(persister.PersistenceMethod == PersistenceMethod.MongoDb);
            Assert.IsFalse(persister.OutputDir == _path);
        }
        [Test, Category("Unit")]
        public void VerifyCreateScrapePersisterInstanceToFile()
        {
            //Arrange & Act
            var persister = PersisterFactory.CreateScrapePersisterInstance(_path);

            //Assert
            Assert.IsTrue(persister.PersistenceMethod == PersistenceMethod.File);
            Assert.IsTrue(persister.OutputDir == _path);
        }
        [Test, Category("Unit")]
        public void VerifyCreateComparePersisterInstanceToFile()
        {
            //Arrange & Act
            var persister = PersisterFactory.CreateComparePersisterInstance(_path);

            //Assert
            Assert.IsTrue(persister.PersistenceMethod == PersistenceMethod.File);
            Assert.IsTrue(persister.OutputDir == _path);
        }
        [Test, Category("Unit")]
        public void VerifyCreateRequestPersisterInstanceToFile()
        {
            //Arrange & Act
            var persister = PersisterFactory.CreateRequestPersisterInstance(_path);

            //Assert
            Assert.IsTrue(persister.PersistenceMethod == PersistenceMethod.File);
            Assert.IsTrue(persister.OutputDir == _path);
        }
    }
}
