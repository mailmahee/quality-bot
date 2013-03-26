namespace QualityBot.Test.Tests.QualityBotTests.PersistenceTests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using NUnit.Framework;
    using QualityBot.Test.Tests.Base;
    using QualityBot.ComparePocos;
    using QualityBot.Persistence;

    [TestFixture]
    class ComparePersisterTests : BaseTest
    {
        readonly ComparePersister _cp = new ComparePersister();
        private string _path = @"Test";

        internal static Stream GetResourceStream(string resourceName)
        {
            var thisExe = Assembly.GetExecutingAssembly();
            var file = thisExe.GetManifestResourceStream(resourceName);
            return file;
        }

        [SetUp]
        public void Setup()
        {
            _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _path);
            Directory.CreateDirectory(_path);
        }

        [TearDown]
        public void Cleanup()
        {
            Directory.Delete(_path, true);
        }

        [Test, Category("Unit")]
        public void VerifyNotTooManyDiffs()
        {
            IFormatter formatter = new BinaryFormatter();
            var obj = (Comparison)formatter.Deserialize(GetResourceStream("QualityBot.Test.Tests.TestData.ComparisonNotTooMany.bin"));
            //Arrange
            _cp.SaveToDisc(_path, obj);
            Assert.IsTrue(Directory.GetFiles(_path).Length > 0);
            //Act
            var json = Directory.GetFiles(_path).First(file => file.Contains(".json"));
            var result = _cp.RetrieveFromDisc(json);
            //Assert
            Assert.IsTrue(result != null);
        }
        [Test, Category("Unit")]
        public void VerifyTooManyDiffs()
        {
            IFormatter formatter = new BinaryFormatter();
            var obj = (Comparison)formatter.Deserialize(GetResourceStream("QualityBot.Test.Tests.TestData.ComparisonIsTooMany.bin"));
            //Arrange
            _cp.SaveToDisc(_path, obj);
            Assert.IsTrue(Directory.GetFiles(_path).Length > 0);
            //Act
            var json = Directory.GetFiles(_path).First(file => file.Contains(".json"));
            var result = _cp.RetrieveFromDisc(json);
            //Assert
            Assert.IsTrue(result != null);
        }
    }
}