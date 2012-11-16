using System;
using MongoDB.Driver;
using NSubstitute;
using Newtonsoft.Json;
using QualityBot.ComparePocos;
using QualityBot.Persistence;

namespace QualityBot.Test.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NUnit.Framework;
    using QualityBot.Test.Tests.Base;

    [TestFixture]
    class ComparePersisterTests : BaseTest
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void VerifyRetrieveFromDisc()
        {
            
        }

        [Test]
        public void VerifyRetrieveFromMongoDb()
        {
        }

        [Test]
        public void VerifySaveToDisc()
        {
            
        }

        [Test]
        public void VerifySaveToMongoDb()
        {
            
        }

        [Test]
        public void VerifySaveResourcesToMediaServices()
        {
            
        }

        [Test]
        public void VerifySetImagesToNull()
        {
            //Arrange
            var pageResults = Substitute.For<PageResult>();
            var comp = new ComparePersister();

            //Act
            comp.SetImagesToNull(pageResults);

            //Assert
            foreach (var e in pageResults.AddedItems)
            {
                Assert.IsTrue(e.Image == null);
                Assert.IsTrue(e.ImageClipped == null);
                Assert.IsTrue(e.ImageClippedStyle == null);
            }
            foreach (var e in pageResults.RemovedItems)
            {
                Assert.IsTrue(e.Image == null);
                Assert.IsTrue(e.ImageClipped == null);
                Assert.IsTrue(e.ImageMask == null);
            }
            foreach (var e in pageResults.ChangedItems)
            {
                Assert.IsTrue(e.PixelChanges.From == null);
                Assert.IsTrue(e.PixelChanges.FromClipped == null);
                Assert.IsTrue(e.PixelChanges.FromMask == null);
                Assert.IsTrue(e.PixelChanges.To == null);
                Assert.IsTrue(e.PixelChanges.ToClipped == null);
                Assert.IsTrue(e.PixelChanges.ToMask == null);
                Assert.IsTrue(e.PixelChanges.Diff == null);
            }
            
        }

        [Test]
        public void VerifyUpdateImageStyles()
        {
            
        }

        [Test]
        public void VerifyUpdateSpriteStyles()
        {
            
        }

        [Test]
        public void VerifyGetStyle()
        {
            
        }

        [Test]
        public void GetAllImages()
        {
            
        }
    }
}
