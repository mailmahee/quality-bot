namespace QualityBot.Test.Tests.QualityBotUtilTests
{
    using QualityBot.Enums;
    using System;
    using System.Linq;
    using QualityBot.Test.Tests.Base;
    using QualityBot.Util;
    using NUnit.Framework;

    [TestFixture]
    class RandomatorTests : BaseTest
    {
        [Test]
        public void VerifyRandomEnumValue()
        {
            //Arrange
            var theEnum = new PersistenceMethod();

            //Act
            var result = Randomator.RandomEnumValue(theEnum);

            //Assert
            Assert.IsTrue(result.GetType() == theEnum.GetType());
        }

        [Test]
        public void VerifyRandomNumber()
        {
            //Arrange
            var upper = 100;
            var lower = 10;

            //Act
            var result = Randomator.RandomNumber(lower, upper);

            //Assert
            Assert.IsTrue(result >= lower && result <= upper);

        }

        [Test]
        public void VerifyRandomDate()
        {
            //Arrange
            var earliest = new DateTime(2011,1,1);
            var latest = new DateTime(2013,12,31);

            //Act
            var result = Randomator.RandomDate(earliest, latest);

            //Assert
            Assert.IsTrue(result.CompareTo(earliest) > 0 && result.CompareTo(latest) < 0);
        }

        [Test]
        public void VerifyRandomUnicodeString()
        {
            //Arrange
            var unicodeCharacters = "ñëâôàæø¿ôéáóúえ退陣を否定愛知2例目も強毒イルスットアニメるろうに미국언론선정인상적미드필더Украиныачлподгтвку车船税法按排气量分档征税ενδχόμοτηπιβλής".ToCharArray();

            //Act
            var result = Randomator.RandomUnicodeString(10);

            //Assert
            Assert.IsTrue(result.IndexOfAny(unicodeCharacters) != -1);
        }

        [Test]
        public void VerifyRandomAlphaNumeric()
        {
            //Arrange
            var alphabeticCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            var numericCharacters = "0123456789".ToCharArray();
            String result = "";

            //Act
            for (int i = 0; i < 5; i++)
            {
                result = Randomator.RandomAlphanumeric(2);
            }

            //Assert
            Assert.IsTrue( ( result.IndexOfAny(alphabeticCharacters) != -1) && ( result.IndexOfAny(numericCharacters) != -1 ) );
        }

        [Test]
        public void VerifyLoremIpsumWords()
        {
            //Arrange
            #region Lorem Ipsum
            var loremIpsum = new[] 
            {
                "consetetur", "sadipscing", "elitr", "sed", "diam", "nonumy", "eirmod",
                "tempor", "invidunt", "ut", "labore", "et", "dolore", "magna", "aliquyam", "erat", "sed", "diam", "voluptua",
                "at", "vero", "eos", "et", "accusam", "et", "justo", "duo", "dolores", "et", "ea", "rebum", "stet", "clita",
                "kasd", "gubergren", "no", "sea", "takimata", "sanctus", "est", "lorem", "ipsum", "dolor", "sit", "amet",
                "lorem", "ipsum", "dolor", "sit", "amet", "consetetur", "sadipscing", "elitr", "sed", "diam", "nonumy", "eirmod",
                "tempor", "invidunt", "ut", "labore", "et", "dolore", "magna", "aliquyam", "erat", "sed", "diam", "voluptua",
                "at", "vero", "eos", "et", "accusam", "et", "justo", "duo", "dolores", "et", "ea", "rebum", "stet", "clita",
                "kasd", "gubergren", "no", "sea", "takimata", "sanctus", "est", "lorem", "ipsum", "dolor", "sit", "amet",
                "lorem", "ipsum", "dolor", "sit", "amet", "consetetur", "sadipscing", "elitr", "sed", "diam", "nonumy", "eirmod",
                "tempor", "invidunt", "ut", "labore", "et", "dolore", "magna", "aliquyam", "erat", "sed", "diam", "voluptua",
                "at", "vero", "eos", "et", "accusam", "et", "justo", "duo", "dolores", "et", "ea", "rebum", "stet", "clita",
                "kasd", "gubergren", "no", "sea", "takimata", "sanctus", "est", "lorem", "ipsum", "dolor", "sit", "amet", "duis",
                "autem", "vel", "eum", "iriure", "dolor", "in", "hendrerit", "in", "vulputate", "velit", "esse", "molestie",
                "consequat", "vel", "illum", "dolore", "eu", "feugiat", "nulla", "facilisis", "at", "vero", "eros", "et",
                "accumsan", "et", "iusto", "odio", "dignissim", "qui", "blandit", "praesent", "luptatum", "zzril", "delenit",
                "augue", "duis", "dolore", "te", "feugait", "nulla", "facilisi", "lorem", "ipsum", "dolor", "sit", "amet",
                "consectetuer", "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod", "tincidunt", "ut", "laoreet",
                "dolore", "magna", "aliquam", "erat", "volutpat", "ut", "wisi", "enim", "ad", "minim", "veniam", "quis",
                "nostrud", "exerci", "tation", "ullamcorper", "suscipit", "lobortis", "nisl", "ut", "aliquip", "ex", "ea",
                "commodo", "consequat", "duis", "autem", "vel", "eum", "iriure", "dolor", "in", "hendrerit", "in", "vulputate",
                "velit", "esse", "molestie", "consequat", "vel", "illum", "dolore", "eu", "feugiat", "nulla", "facilisis", "at",
                "vero", "eros", "et", "accumsan", "et", "iusto", "odio", "dignissim", "qui", "blandit", "praesent", "luptatum",
                "zzril", "delenit", "augue", "duis", "dolore", "te", "feugait", "nulla", "facilisi", "nam", "liber", "tempor",
                "cum", "soluta", "nobis", "eleifend", "option", "congue", "nihil", "imperdiet", "doming", "id", "quod", "mazim",
                "placerat", "facer", "possim", "assum", "lorem", "ipsum", "dolor", "sit", "amet", "consectetuer", "adipiscing",
                "elit", "sed", "diam", "nonummy", "nibh", "euismod", "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam",
                "erat", "volutpat", "ut", "wisi", "enim", "ad", "minim", "veniam", "quis", "nostrud", "exerci", "tation",
                "ullamcorper", "suscipit", "lobortis", "nisl", "ut", "aliquip", "ex", "ea", "commodo", "consequat", "duis",
                "autem", "vel", "eum", "iriure", "dolor", "in", "hendrerit", "in", "vulputate", "velit", "esse", "molestie",
                "consequat", "vel", "illum", "dolore", "eu", "feugiat", "nulla", "facilisis", "at", "vero", "eos", "et", "accusam",
                "et", "justo", "duo", "dolores", "et", "ea", "rebum", "stet", "clita", "kasd", "gubergren", "no", "sea",
                "takimata", "sanctus", "est", "lorem", "ipsum", "dolor", "sit", "amet", "lorem", "ipsum", "dolor", "sit",
                "amet", "consetetur", "sadipscing", "elitr", "sed", "diam", "nonumy", "eirmod", "tempor", "invidunt", "ut",
                "labore", "et", "dolore", "magna", "aliquyam", "erat", "sed", "diam", "voluptua", "at", "vero", "eos", "et",
                "accusam", "et", "justo", "duo", "dolores", "et", "ea", "rebum", "stet", "clita", "kasd", "gubergren", "no",
                "sea", "takimata", "sanctus", "est", "lorem", "ipsum", "dolor", "sit", "amet", "lorem", "ipsum", "dolor", "sit",
                "amet", "consetetur", "sadipscing", "elitr", "at", "accusam", "aliquyam", "diam", "diam", "dolore", "dolores",
                "duo", "eirmod", "eos", "erat", "et", "nonumy", "sed", "tempor", "et", "et", "invidunt", "justo", "labore",
                "stet", "clita", "ea", "et", "gubergren", "kasd", "magna", "no", "rebum", "sanctus", "sea", "sed", "takimata",
                "ut", "vero", "voluptua", "est", "lorem", "ipsum", "dolor", "sit", "amet", "lorem", "ipsum", "dolor", "sit",
                "amet", "consetetur", "sadipscing", "elitr", "sed", "diam", "nonumy", "eirmod", "tempor", "invidunt", "ut",
                "labore", "et", "dolore", "magna", "aliquyam", "erat", "consetetur", "sadipscing", "elitr", "sed", "diam",
                "nonumy", "eirmod", "tempor", "invidunt", "ut", "labore", "et", "dolore", "magna", "aliquyam", "erat", "sed",
                "diam", "voluptua", "at", "vero", "eos", "et", "accusam", "et", "justo", "duo", "dolores", "et", "ea",
                "rebum", "stet", "clita", "kasd", "gubergren", "no", "sea", "takimata", "sanctus", "est", "lorem", "ipsum" 
            };
            #endregion

            //Act
            var result = Randomator.LoremIpsumWords(10);

            //Assert
            Assert.IsTrue(loremIpsum.Any(result.Contains));
        }

        [Test]
        public void VerifyLoremIpsumLengthSingleWord()
        {
            //Arrange
            var length = 10;

            //Act
            var result = Randomator.LoremIpsumLength(length);

            //Assert
            Assert.IsTrue(result.Length == length);
        }

        [Test]
        public void VerifyLoremIpsumLengthMultiWord()
        {
            //Arrange
            var length = 20;

            //Act
            var result = Randomator.LoremIpsumLength(length);

            //Assert
            Assert.IsTrue(result.Length == length);
        }

        [Test]
        public void VerifyGetRandomColor()
        {
            //Arrange
            var firstColor = Randomator.GetRandomColor();

            //Act
            var secondColor = Randomator.GetRandomColor();

            //Assert
            Assert.IsTrue(firstColor != secondColor);
        }

        [Test]
        public void VerifyLoremIpsumTooLong()
        {
            //Arrange
            var length = 16;
            var value = "abcdefghijklm";

            //Act
            var result = Randomator.LoremIpsumLengthTooShort(value,length);

            //Assert
            Assert.IsTrue(result.Length == length);
        }

        [Test]
        public void VerifyLoremIpsumLengthZeroToTwenty()
        {
            //Arrange
            var length = 0;
            string result;

            //Act
            do
            {
                result = Randomator.LoremIpsumLength(length);
                Console.WriteLine(result);

                //Assert
                Assert.IsTrue(result.Length == length,"Expected: {0} Was: {1}",length,result.Length);

                length++;
            } while (length <= 21);

        }

        [Test]
        public void VerifyReplaceFirstOccurence()
        {
            var testItem = "TodayTomorrow Yesterday";
            var oldItem = "Yesterday";
            var newItem = "Never";

           
            var result = testItem.ReplaceFirstOccurrence(oldItem, newItem);
            Assert.IsTrue(result.Contains(newItem));

            result = result.ReplaceFirstOccurrence(" ", "-");
            Assert.IsTrue(result.Contains(newItem)); 
        }

        [Test]
        public void VerifyReplaceFirstOccurenceNullOriginal()
        {
            var result = Randomator.ReplaceFirstOccurrence(null, "", "");
            Console.WriteLine(result);
            Assert.IsTrue(result == "");
        }

        [Test]
        public void VerifyReplaceFirstOccurenceNullOldItem()
        {
            var result = "a".ReplaceFirstOccurrence(null, "");
            Console.WriteLine(result);
            Assert.IsTrue(result == "a");
        }

        [Test]
        public void VerifyReplaceFirstOccurenceNullNewItem()
        {
            var result = "a".ReplaceFirstOccurrence("b", null);
            Console.WriteLine(result);
            Assert.IsTrue(result == "a");
        }
    }
}
