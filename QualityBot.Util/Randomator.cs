using System.Globalization;

namespace QualityBot.Util
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// A class that generates random data.
    /// </summary>
    public static class Randomator
    {
        /// <summary>
        /// Random class.
        /// </summary>
        private static readonly Random _random = new Random((int)DateTime.Now.Ticks);

        #region Lorem Ipsum

        /// <summary>
        /// An array of lorem ipsum words.
        /// </summary>
        private static readonly string[] _loremIpsumWords = new[] 
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

        /// <summary>
        /// Returns a random value from the source enum.
        /// </summary>
        /// <param name="excludes">Items to exclude from the random selection.</param>
        /// <typeparam name="TSourceEnum">The source enum.</typeparam>
        /// <returns>
        /// A randomly selected value from the source enum.
        /// </returns>
        public static TSourceEnum RandomEnumValue<TSourceEnum>(params TSourceEnum[] excludes)
        {
            return Enum.GetValues(typeof(TSourceEnum)).Cast<TSourceEnum>().Except(excludes).OrderBy(x => _random.Next()).FirstOrDefault();
        }

        /// <summary>
        /// Generates a random number.
        /// </summary>
        /// <param name="inclusiveLowerBound">The inclusive lower bound.</param>
        /// <param name="exclusiveUpperBound">The exclusive upper bound.</param>
        /// <returns>
        /// The random number generated.
        /// </returns>
        public static int RandomNumber(int inclusiveLowerBound, int exclusiveUpperBound)
        {
            return _random.Next(inclusiveLowerBound, exclusiveUpperBound);
        }

        /// <summary>
        /// Generates a random date.
        /// </summary>
        /// <param name="earliestDate">The earliest date.</param>
        /// <param name="latestDate">The latest date.</param>
        /// <returns>
        /// The random date generated.
        /// </returns>
        public static DateTime RandomDate(DateTime earliestDate, DateTime latestDate)
        {
            int range = (latestDate - earliestDate).Days;
            return earliestDate.AddDays(_random.Next(range));
        }

        /// <summary>
        /// Generates a random unicode string of a given length.
        /// </summary>
        /// <param name="length">The length of the string desired.</param>
        /// <returns>
        /// A random unicode string of the given length.
        /// </returns>
        public static string RandomUnicodeString(int length)
        {
            // Don't need these characters: abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ
            var unicodeCharacters = "ñëâôàæø¿ôéáóúえ退陣を否定愛知2例目も強毒イルスットアニメるろうに미국언론선정인상적미드필더Украиныачлподгтвку车船税法按排气量分档征税ενδχόμοτηπιβλής".ToCharArray();
            StringBuilder result = new StringBuilder();

            for (var i = 0; i < length; i++)
            {
                result.Append(unicodeCharacters[_random.Next(0, unicodeCharacters.Length)]);
            }

            return result.ToString();
        }

        /// <summary>
        /// Generates a random alphanumeric string of a given length.
        /// </summary>
        /// <param name="length">The length of the string desired.</param>
        /// <returns>
        /// A random alphanumeric string of the given length.
        /// </returns>
        public static string RandomAlphanumeric(int length)
        {
            var alphanumericCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
            StringBuilder result = new StringBuilder();
            
            for (var i = 0; i < length; i++)
            {
                result.Append(alphanumericCharacters[_random.Next(0, alphanumericCharacters.Length)]);
            }

            var value = result.ToString();

            if (!Regex.IsMatch(value, "[a-zA-Z]") || !Regex.IsMatch(value, "[0-9]")) return RandomAlphanumeric(length);

            return value;
        }

        /// <summary>
        /// Generates a random string of a given length from the given sample characters.
        /// </summary>
        /// <param name="length">The length of the string desired.</param>
        /// <param name="sample">The string containing the characters you wish to include.</param>
        /// <returns>
        /// A random string of the given length from the given sample characters.
        /// </returns>
        public static string RandomFromSample(int length, string sample)
        {
            var sampleChars = sample.ToCharArray();
            StringBuilder result = new StringBuilder();

            for (var i = 0; i < length; i++)
            {
                result.Append(sampleChars[_random.Next(0, sampleChars.Length)]);
            }

            return result.ToString();
        }

        /// <summary>
        /// Generates a string of "Lorem Ipsum" text of a given number of words.
        /// </summary>
        /// <param name="numberOfWords">The number of words desired.</param>
        /// <returns>
        /// A "Lorem Ipsum" string of the given number of words.
        /// </returns>
        public static string LoremIpsumWords(int numberOfWords)
        {
            StringBuilder result = new StringBuilder();

            for (var i = 0; i < numberOfWords; i++)
            {
                if (i == 0)
                {
                    result.Append(_loremIpsumWords[_random.Next(0, _loremIpsumWords.Length)]);
                }
                else
                {
                    result.Append(" " + _loremIpsumWords[_random.Next(0, _loremIpsumWords.Length)]);
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Generates a string of "Lorem Ipsum" text of a given length.
        /// </summary>
        /// <param name="length">The length of the string desired.</param>
        /// <returns>
        /// A "Lorem Ipsum" string of the given length.
        /// </returns>
        public static string LoremIpsumLength(int length)
        {
            var result = new StringBuilder();

            // If length <= 12, then a single word will suffice
            if (length <= 12)
            {
                result.Append(GetRandomLoremIpsumWord(length));
            }
            else
            {
                do
                {
                    var nextWord = _loremIpsumWords[_random.Next(0, _loremIpsumWords.Length)];
                    if (result.Length == 0)
                    {
                        result.Append((result.Length + nextWord.Length) > length
                                          ? GetRandomLoremIpsumWord(length - result.Length)
                                          : nextWord);
                    }
                    else
                    {
                        result.Append((result.Length + nextWord.Length + 1) > length
                                          ? string.Format(" {0}", GetRandomLoremIpsumWord(length - (result.Length + 1)))
                                          : string.Format(" {0}", nextWord));
                    }
                }
                while (result.Length < length);
            }

            // Trim
            var value = result.ToString().Trim();

            // Final tweaks - if trimmed string is shorter then replace one word with a bigger word or two words
            value = LoremIpsumLengthTooShort(value, length);

            return value;
        }

        /// <summary>
        /// Helper method used by the LoremIpsumLength Method, made public to test.
        /// </summary>
        /// <param name="value">The string that has need of modification.</param>
        /// <param name="length">The length of the string desired.</param>
        /// <returns>
        /// A modified string to be consumed internally by LoremIpsumLength().
        /// </returns>
        public static string LoremIpsumLengthTooShort(string value, int length)
        {
            
            if (value.Length < length)
            {
                var findSubstringOfLength = 12 - (length - value.Length);
                var find = string.Format("\\b[a-zA-Z]{{2,{0}}}\\b", findSubstringOfLength);
                Match oldWord = Regex.Match(value, find);
                string newWord;

                // If no match found then take any word and split it in two
                if (oldWord.Length == 0)
                {
                    find = "\\b[a-zA-Z]{1,}\\b";
                    oldWord = Regex.Match(value, find);
                    var paddingNeeded = length - value.Length;
                    int halfA = oldWord.Length / 2;
                    int halfB = halfA + (oldWord.Length % 2) + paddingNeeded - 1;

                    newWord = string.Format("{0} {1}", GetRandomLoremIpsumWord(halfA), GetRandomLoremIpsumWord(halfB));
                }
                else
                {
                    var paddingNeeded = oldWord.Length + (length - value.Length);
                    newWord = GetRandomLoremIpsumWord(paddingNeeded);
                }

                value = value.ReplaceFirstOccurrence(oldWord.Value, newWord);
            }
            return value;
        }
        /// <summary>
        /// Generates a random color.
        /// </summary>
        /// <returns>
        /// A random color.
        /// </returns>
        public static Color GetRandomColor()
        {
            return Color.FromArgb(RandomNumber(0, 256), RandomNumber(0, 256), RandomNumber(0, 256));
        }

        /// <summary>
        /// Generates a random Lorem Ipsum word of a given length.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns>
        /// A Lorem Ipsum word of the desired length.
        /// </returns>
        public static string GetRandomLoremIpsumWord(int length)
        {
            List<string> words = _loremIpsumWords.Where(word => word.Length == length).ToList();

            string randomWord = string.Empty;
            if (words.Count != 0)
            {
                randomWord = words[_random.Next(0, words.Count)];
            }
            if(length == 1)
            {
                var rdmWord = _loremIpsumWords[_random.Next(0, _loremIpsumWords.Length - 1)].ToCharArray();
                randomWord = rdmWord[_random.Next(0,rdmWord.Length-1)].ToString(CultureInfo.InvariantCulture);
            }
            return randomWord;
        }

        /// <summary>
        /// Replaces the first occurrence of the specified string with the specified replacement string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>
        /// The modified string.
        /// </returns>
        public static string ReplaceFirstOccurrence(this string value, string oldValue, string newValue)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(oldValue))
            {
                return value;
            }

            if (string.IsNullOrEmpty(newValue))
            {
                newValue = string.Empty;
            }

            int index = value.IndexOf(oldValue, StringComparison.Ordinal);

            if (index == -1)
            {
                return value;
            }

            return value.Remove(index, oldValue.Length).Insert(index, newValue);
        }
    }
}
