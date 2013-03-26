namespace QualityBot.Test.Tests.QualityBotTests.PersistenceTests
{
    using NSubstitute;
    using System;
    using QualityBot.Enums;
    using QualityBot.Persistence;
    using QualityBot.Test.Tests.Base;
    using QualityBot.Util;
    using NUnit.Framework;

    [TestFixture]
    class PersistTests :BaseTest
    {

        [Test, Category("Unit")]
        public void VerifySaveException()
        {
            //Arrange
            var persistable = Substitute.For<IPersist>();
            var persist = Substitute.For<IPersister<IPersist>>();
            var persister = new Persister<IPersist>(persist) { PersistenceMethod = (PersistenceMethod)3 };

            //Act-Assert
            Assert.Throws<ArgumentException>(() => persister.Save(persistable));
        }

        [Test, Category("Unit")]
        public void VerifySaveMongoDb()
        {
            //Arrange
            var persistable = Substitute.For<IPersist>();
            var persist = Substitute.For<IPersister<IPersist>>();
            var persister = new Persister<IPersist>(persist) { PersistenceMethod = PersistenceMethod.MongoDb };

            //Act-Assert
            Assert.DoesNotThrow(() => persister.Save(persistable));
        }

        [Test, Category("Unit")]
        public void VerifySaveFile()
        {
            //Arrange
            var persistable = Substitute.For<IPersist>();
            var persist = Substitute.For<IPersister<IPersist>>();
            var persister = new Persister<IPersist>(persist) { PersistenceMethod = PersistenceMethod.File };
            
            //Act-Assert
            Assert.DoesNotThrow(() => persister.Save(persistable));
        }

        [Test, Category("Unit")]
        public void VerifyLoadException()
        {
            //Arrange
            var persistable = Substitute.For<IPersist>();
            var persist = Substitute.For<IPersister<IPersist>>();
            var persister = new Persister<IPersist>(persist) { PersistenceMethod = (PersistenceMethod)3 };

            //Act-Assert
            Assert.Throws<ArgumentException>(() => persister.Load("-"));
        }

        [Test, Category("Unit")]
        public void VerifyLoadMongoDb()
        {
            //Arrange
            var persistable = Substitute.For<IPersist>();
            var persist = Substitute.For<IPersister<IPersist>>();
            var persister = new Persister<IPersist>(persist) { PersistenceMethod = PersistenceMethod.MongoDb };

            //Act-Assert
            Assert.DoesNotThrow(() => persister.Load(persistable.Id.ToString()));
        }

        [Test, Category("Unit")]
        public void VerifyLoadFile()
        {
            //Arrange
            var persistable = Substitute.For<IPersist>();
            persistable.Path = new StringAsReference {Value = "C:\\"};
            var persist = Substitute.For<IPersister<IPersist>>();
            var persister = new Persister<IPersist>(persist) { PersistenceMethod = PersistenceMethod.File };

            //Act-Assert
            Assert.DoesNotThrow(() => persister.Load(persistable.Path.Value));
        }
    }
}
