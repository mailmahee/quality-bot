using System.Collections.Generic;
using System.Drawing;
using OpenQA.Selenium;
using QualityBot.RequestPocos;
using QualityBot.ScrapePocos;
using Webinator;

namespace QualityBot
{
    public interface IScraper
    {
        /// <summary>
        /// Instantiates a configuration for Webinator.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// A <c>Config</c> instance.
        /// </returns>
        Config GetConfig(Request request);

        /// <summary>
        /// Instantiates Webinator.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns>
        /// An <c>IWebManager</c> instance.
        /// </returns>
        IWebDriver GetWebDriver(Config config);

        /// <summary>
        /// Scrapes the page as defined by the request object.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>An object containing information about the web page as a Scrape object.</returns>
        Scrape Scrape(Request request);

        /// <summary>
        /// Scrapes pages as defined by the request objects.
        /// Will attempt to optimize by parallelizing the work across multiple threads.
        /// </summary>
        /// <param name="requests">The requests.</param>
        /// <returns>A collection of objects containing information about each request as a Scrape object.</returns>
        Scrape[] Scrape(params Request[] requests);

        /// <summary>
        /// Scrapes the current page.
        /// </summary>
        /// <param name="webDriver">The WebDriver instance to use.</param>
        /// <returns>An object containing information about the web page as a Scrape object.</returns>
        Scrape ScrapeCurrent(IWebDriver webDriver);

        /// <summary>
        /// Scrapes the current page.
        /// </summary>
        /// <param name="webManager">The Webinator WebManager instance to use.</param>
        /// <returns>An object containing information about the web page as a Scrape object.</returns>
        Scrape ScrapeCurrent(dynamic webManager);

        /// <summary>
        /// Scrapes the current page.
        /// </summary>
        /// <param name="webDriver">The WebDriver instance to use.</param>
        /// <param name="request">The request.</param>
        /// <returns>An object containing information about the web page as a Scrape object.</returns>
        Scrape ScrapeCurrent(IWebDriver webDriver, Request request);

        /// <summary>
        /// Scrapes the current page.
        /// </summary>
        /// <param name="webManager">The Webinator WebManager instance to use.</param>
        /// <param name="request">The request.</param>
        /// <returns>An object containing information about the web page as a Scrape object.</returns>
        Scrape ScrapeCurrent(dynamic webManager, Request request);

        /// <summary>
        /// Sets the browser viewport to the specified size.
        /// </summary>
        /// <param name="web">The web manager.</param>
        /// <param name="resolution">The desired size.</param>
        void SetViewportSize(IWebDriver web, Size resolution);
    }
}