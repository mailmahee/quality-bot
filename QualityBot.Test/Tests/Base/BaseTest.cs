namespace QualityBot.Test.Tests.Base
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using NUnit.Framework;
    using QualityBot.ComparePocos;
    using QualityBot.Persistence;
    using QualityBot.ScrapePocos;
    using QualityBot.Util;

    /// <summary>
    /// Contains the base objects needed by most tests.
    /// </summary>
    public class BaseTest
    {
        protected readonly string SeleniumGridHubUri = AppConfig("seleniumGrid2Hub");
        protected readonly string DebugOutputDirectory = AppConfig("debugOutputDirectory");
        protected readonly bool OutputToFileForDebugging = AppConfig<bool>("debugToOutputDirectory");

        /// <summary>
        /// Runs test and fails inconclusive if an error is thrown that isn't an assertion.
        /// </summary>
        /// <param name="action">The test.</param>
        public void Test(Action action)
        {
            try
            {
                if (action != null)
                {
                    action();
                }
            }
            catch (AssertionException)
            {
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.Inconclusive("Error in test code: {0}", e);
            }
        }

        /// <summary>
        /// Outputs the specified objects to file.
        /// </summary>
        /// <param name="comparison">The comparison.</param>
        /// <param name="scrapes">The scrapes.</param>
        protected void OutputToFile(Comparison comparison, params Scrape[] scrapes)
        {
            if (!OutputToFileForDebugging) return;

            var sp = PersisterFactory.CreateScrapePersisterInstance(DebugOutputDirectory);
            scrapes.ForEach(sp.Save);
            scrapes.ForEach(s => Console.WriteLine("Wrote scrape to:{0}", s.Path.Value));

            if (comparison != null)
            {
                var cp = PersisterFactory.CreateComparePersisterInstance(DebugOutputDirectory);
                cp.Save(comparison);
                Console.WriteLine("Wrote comparsion to:{0}", comparison.Path.Value);
            }
        }

        /// <summary>
        /// Gets the specified setting from app.config.
        /// </summary>
        /// <param name="setting">
        /// The setting.
        /// </param>
        /// <typeparam name="T">
        /// The type of object to return.
        /// </typeparam>
        /// <returns>
        /// The setting as T.
        /// </returns>
        public static T AppConfig<T>(string setting)
        {
            if (typeof(T).IsEnum)
            {
                return (T)Enum.Parse(typeof(T), ConfigurationManager.AppSettings[setting], true);
            }

            var converter = TypeDescriptor.GetConverter(typeof(T));
            return (T)converter.ConvertFrom(ConfigurationManager.AppSettings[setting]);
        }

        /// <summary>
        /// Gets the specified setting from app.config.
        /// </summary>
        /// <param name="setting">
        /// The setting.
        /// </param>
        /// <returns>
        /// The setting as a string.
        /// </returns>
        public static string AppConfig(string setting)
        {
            return ConfigurationManager.AppSettings[setting];
        }
    }
}