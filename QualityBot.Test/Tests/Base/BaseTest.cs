namespace QualityBot.Test.Tests.Base
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    using QualityBot.ComparePocos;
    using QualityBot.Persistence;
    using QualityBot.ScrapePocos;
    using QualityBot.Util;
    using Webinator;

    /// <summary>
    /// Contains the base objects needed by most tests.
    /// </summary>
    public class BaseTest
    {
        protected readonly string SeleniumGridHubUri = AppConfigUtil.AppConfig("seleniumGrid2Hub");
        protected readonly string DebugOutputDirectory = AppConfigUtil.AppConfig("debugOutputDirectory");
        protected readonly bool OutputToFileForDebugging = AppConfigUtil.AppConfig<bool>("debugToOutputDirectory");

        public Config GetFirefoxRemote()
        {
            return new Config
            {
                BaseUrl = "www.google.com",
                Browser = Config.AvailableBrowsers.Firefox,
                HighlightElements = false,
                Framework = Config.AvailableFrameworks.WebDriverRemote,
                LogRealTime = false,
                LogScreenshots = false,
                LogLevel = Config.AvailableLogLevels.None,
                DesiredCapabilities = new Dictionary<string, object> { { "version", "10.1" } },
                SeleniumGridHubUri = SeleniumGridHubUri
            };
        }

        public Config GetChromeRemote()
        {
            return new Config
            {
                BaseUrl = "www.google.com",
                Browser = Config.AvailableBrowsers.Chrome,
                HighlightElements = false,
                Framework = Config.AvailableFrameworks.WebDriverRemote,
                LogRealTime = false,
                LogScreenshots = false,
                LogLevel = Config.AvailableLogLevels.None,
                SeleniumGridHubUri = SeleniumGridHubUri
            };
        }

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
    }
}