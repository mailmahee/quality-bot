namespace QualityBot.Test.Tests.QualityBotTests.ScrapersTests
{
    using System.Drawing;
    using System.IO;
    using NUnit.Framework;
    using QualityBot.Scrapers;

    [TestFixture]
    class ElementProviderTests
    {
        private ElementProvider _eProvider;
        private Rectangle _boundingRectangle;
        private string _elements;
        private string _html;

        internal string GetFromResources(string resourceName)
        {
            var assem = this.GetType().Assembly;
            using (var stream = assem.GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        [TestFixtureSetUp]
        public void Setup()
        {
            _elements = GetFromResources("QualityBot.Test.Tests.TestData.FakeElementJSON.txt");
            _html = GetFromResources("QualityBot.Test.Tests.TestData.FakeHTML.txt");
            _boundingRectangle = new Rectangle(0,0,800,600);
            _eProvider = new ElementProvider();
        }

        [Test, Category("Unit")]
        public void VerifyElements()
        {
            //Arrange
            _eProvider.Load(_elements, _html, _boundingRectangle);
            //Act
            foreach (var ele in _eProvider.Elements())
            {
                //Assert
                Assert.IsTrue(ele != null);
            }
        }
    }
}
