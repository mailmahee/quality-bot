namespace QualityBot.Scrapers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using OpenQA.Selenium;
    using QualityBot.RequestPocos;
    using QualityBot.Scrapers.Facades;
    using QualityBot.Scrapers.Interfaces;
    using QualityBot.Util;
    using Webinator;

    public static class FacadeFactory
    {
        private static string _seleniumGridHub;

        static FacadeFactory()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("QualityBot.seleniumGridSettings.json"))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var result = reader.ReadToEnd();
                        var settings = SettingsUtil.ReadSettingsFromString(result);
                        _seleniumGridHub = SettingsUtil.GetConfig("seleniumGrid2Hub", settings);
                    }
                }
            }
        }

        public static IBrowserFacade CreateFacade(Request request)
        {
            IBrowserFacade facade;

            if (request.Browser == null || request.Browser.ToLower() != "phantomjs")
            {
                var config = GetConfig(request);
                var web = GetWebDriver(config);
                facade = new WebDriverFacade(web, request);
            }
            else
            {
                facade = new PhantomJsFacade(request);
            }

            return facade;
        }

        public static IBrowserFacade CreateFacade(IWebDriver webDriver, Request request)
        {
            IBrowserFacade facade;

            if (request.Browser == null || request.Browser.ToLower() != "phantomjs")
            {
                facade = new WebDriverFacade(webDriver, request);
            }
            else
            {
                facade = new PhantomJsFacade(request);
            }

            return facade;
        }

        /// <summary>
        /// Instantiates a configuration for Webinator.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// A <c>Config</c> instance.
        /// </returns>
        private static Config GetConfig(Request request)
        {
            Config.AvailableBrowsers browserEnum;
            if (!Enum.TryParse(request.Browser, true, out browserEnum))
            {
                browserEnum = Config.AvailableBrowsers.Firefox;
            }

            var config = new Config
            {
                LogScreenshots = false,
                LogLevel = Config.AvailableLogLevels.None,
                BaseUrl = request.Url,
                Browser = browserEnum,
                Framework = Config.AvailableFrameworks.WebDriverRemote,
                LogDirectory = Path.Combine("Logs", DateTime.Now.ToString("yyyyMMddHHmmssfffffff")),
                HighlightElements = false,
                SeleniumGridHubUri = _seleniumGridHub,
                CommandTimeout = new TimeSpan(0, 1, 0),
                DesiredCapabilities = new Dictionary<string, object> { { "platform", "WINDOWS" } }
            };

            if (!string.IsNullOrWhiteSpace(request.BrowserVersion))
            {
                config.DesiredCapabilities.Add("version", request.BrowserVersion);
            }

            return config;
        }

        /// <summary>
        /// Instantiates Webinator.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns>
        /// An <c>IWebManager</c> instance.
        /// </returns>
        private static IWebDriver GetWebDriver(Config config)
        {
            var web = WebManagerFactory.CreateInstance(config);
            var engine = (IWebDriver)web.GetEngine();

            return engine;
        }
    }
}