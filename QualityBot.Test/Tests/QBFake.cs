using System.Configuration;
using System.IO;

namespace QualityBot.Test.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Linq;
    using System.Net;
    using AutoPoco;
    using AutoPoco.Engine;
    using MongoDB.Bson;
    using ScrapePocos;
    using ComparePocos;
    using RequestPocos;
    using Util;

    internal class FakeScrapeParams
    {
        private string _id;
        private string _exclude;
        private string _include;
        private string _script;
        private Rectangle? _bounding;
        private string _path;
        private string _html;
        private string _htmlRef;
        private string _url;
        private string _screenShotRef;
        private string _screenShot;
        private Size? _viewportSize;
        private string _browser;
        private string _browserVersion;
        private DateTime? _timeStamp;
        private string _platform;
        private List<string> _cookies;

        public FakeScrapeParams(string id = "505b74b57b350d1ac4b55993", string exclude = "\'\'", string include = "\'body *\'", string script = "", 
            Rectangle? bounding = null, string path = "null", List<ScrapedElement> elements = null, Resource[] resources = null, 
            string html = "http://m.mfcreativedev.com/File/a9155038-07ee-4b3a-a04a-3267e4723f96?mimeType=text/html", 
            string htmlRef = "http://m.mfcreativedev.com/File/a9155038-07ee-4b3a-a04a-3267e4723f96?mimeType=text/html",
            string url = "http://dna.ancestrydev.com/", string screenShot = null, string screenShotRef = "", 
            Size? viewportSize = null, string browser = "firefox", string browserVersion = "10", DateTime? timeStamp = null, string platform = "windows", List<string> cookies = null)
        {
            _id = id;
            _exclude = exclude;
            _include = include;
            _script = script;
            _bounding = bounding;
            _path = path;
            Elements = elements;
            Resources = resources;
            _html = html;
            _htmlRef = htmlRef;
            _url = url;
            _screenShot = screenShot;
            _screenShotRef = screenShotRef;
            _viewportSize = viewportSize;
            _browser = browser;
            _browserVersion = browserVersion;
            _timeStamp = timeStamp;
            _platform = platform;
            _cookies = cookies;
        }

        public string Id
        {
            get { return _id; }
        }

        public string Exclude
        {
            get { return _exclude; }
        }

        public string Include
        {
            get { return _include; }
        }

        public string Script
        {
            get { return _script; }
        }

        public Rectangle? Bounding
        {
            get { return _bounding; }
        }

        public string Path
        {
            get { return _path; }
        }

        public List<ScrapedElement> Elements { get; set; }

        public Resource[] Resources { get; set; }

        public string Html
        {
            get { return _html; }
        }

        public string HtmlRef
        {
            get { return _htmlRef; }
        }

        public string Url
        {
            get { return _url; }
        }

        public string ScreenShot
        {
            get { return _screenShot ?? ConfigurationManager.AppSettings["staticBase64"]; }
            set { _screenShot = value; }
        }

        public string ScreenShotRef
        {
            get { return _screenShotRef; }
        }

        public Size? ViewportSize
        {
            get { return _viewportSize; }
        }

        public string Browser
        {
            get { return _browser; }
        }

        public string BrowserVersion
        {
            get { return _browserVersion; }
        }

        public DateTime? TimeStamp
        {
            get { return _timeStamp; }
        }

        public string Platform
        {
            get { return _platform; }
        }

        public List<string> Cookies
        {
            get { return _cookies; }
            set { _cookies = value; }
        }
    }

    internal class FakeComparisonResult
    {
        private List<ElementChangeResult> _changedResult;
        private List<ElementAddRemoveResult> _addedResult;
        private List<ElementAddRemoveResult> _removedResult;
        private int _unchangedItems;
        private decimal _cssPercent;
        private decimal _textPercent;
        private decimal _overallPercent;
        private PixelResult _pixelResult;
        private HtmlResult _htmlResult;
        private string _htmlDiff;

        public FakeComparisonResult(List<ElementChangeResult> changedResult = null, List<ElementAddRemoveResult> addedResult = null, List<ElementAddRemoveResult> removedResult = null, int unchangedItems = 0, decimal cssPercent = 0, decimal textPercent = 0, decimal overallPercent = 0, PixelResult pixelResult = null, HtmlResult htmlResult = null, string htmlDiff = "null")
        {
            _changedResult = changedResult;
            _addedResult = addedResult;
            _removedResult = removedResult;
            _unchangedItems = unchangedItems;
            _cssPercent = cssPercent;
            _textPercent = textPercent;
            _overallPercent = overallPercent;
            _pixelResult = pixelResult;
            _htmlResult = htmlResult;
            _htmlDiff = htmlDiff;
        }

        public List<ElementChangeResult> ChangedResult
        {
            get { return _changedResult; }
        }
        public List<ElementAddRemoveResult> AddedResult
        {
            get { return _addedResult; }
        }
        public List<ElementAddRemoveResult> RemovedResult
        {
            get { return _removedResult; }
        }
        public int UnchangedItems
        {
            get { return _unchangedItems; }
        }
        public decimal CssPercent
        {
            get { return _cssPercent; }
        }
        public decimal TextPercent
        {
            get { return _textPercent; }
        }
        public decimal OverallPercent
        {
            get { return _overallPercent; }
        }
        public PixelResult PixelResults
        {
            get { return _pixelResult; }
        }
        public HtmlResult HtmlResults
        {
            get { return _htmlResult; }
        }
        public string HtmlDiff
        {
            get { return _htmlDiff; }
        }

    }

    internal class FakeComparisonParams
    {
        private ScrapeHybrid[] _scrapeHybrids;
        private ObjectId? _objectId;
        private StringAsReference _path;
        private PageResult _result;

        public FakeComparisonParams(ScrapeHybrid[] scrapeHybrids = null, ObjectId? objectId = null, StringAsReference path = null, PageResult result = null)
        {
            _scrapeHybrids = scrapeHybrids;
            _objectId = objectId;
            _path = path;
            _result = result;
        }

        public ScrapeHybrid[] ScrapeHybrids
        {
            get { return _scrapeHybrids; }
        }

        public ObjectId? ObjectId
        {
            get { return _objectId; }
        }

        public StringAsReference Path
        {
            get { return _path; }
        }

        public PageResult Result
        {
            get { return _result; }
        }
    }

    internal class FakePageDataParams
    {
        public Size Size { get; set; }

        public string Html { get; set; }

        public string[] Cookies { get; set; }

        public string Elements { get; set; }

        public Image Screenshot { get; set; }

        public string BrowserName { get; set; }

        public string BrowserVersion { get; set; }

        public string Platform { get; set; }

        public IEnumerable<object> Resources { get; set; }

        public string Url { get; set; }

    }

    class QBFake
    {
        public Image DownloadImage(string _URL)
        {
            Image _tmpImage = null;

            try
            {
                // Open a connection
                System.Net.HttpWebRequest _HttpWebRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(_URL);

                _HttpWebRequest.AllowWriteStreamBuffering = true;

                // You can also specify additional header values like the user agent or the referer: (Optional)
                _HttpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)";
                _HttpWebRequest.Referer = "http://www.google.com/";

                // set timeout for 20 seconds (Optional)
                _HttpWebRequest.Timeout = 20000;

                // Request response:
                System.Net.WebResponse _WebResponse = _HttpWebRequest.GetResponse();

                // Open data stream:
                System.IO.Stream _WebStream = _WebResponse.GetResponseStream();

                // convert webstream to image
                _tmpImage = Image.FromStream(_WebStream);

                // Cleanup
                _WebResponse.Close();
                _WebResponse.Close();
            }
            catch (Exception _Exception)
            {
                // Error
                Console.WriteLine("Exception caught in process: {0}", _Exception.ToString());
                return null;
            }

            return _tmpImage;
        }

        public IEnumerable<Scrape> FakeScrape(params FakeScrapeParams[] scrapeParams)
        {
            return scrapeParams.Select(FakeScrape);
        }

        public IGenerationSession GetSession()
        {
            IGenerationSessionFactory sessionFactory = AutoPocoContainer.Configure(x => x.Conventions(c => c.UseDefaultConventions()));
            IGenerationSession session = sessionFactory.CreateSession();
            return session;
        }

        public Scrape FakeScrape(FakeScrapeParams fakeScrapeParams)
        {

            fakeScrapeParams.Cookies = new List<string>();

            #region Header/Cookie Content

            if (!fakeScrapeParams.Cookies.Any())
            {
                fakeScrapeParams.Cookies.AddRange(new[]
                {
                    "s_cc=true; path=/; domain=.ancestrydev.com",
                    "JSESSIONID=091AB8DFA1387A5C1D781CA73C0CE84C; path=/; domain=dna.ancestrydev.com",
                    "s_vi=[CS]v1|282DBA32050102D5-6000011060408FA5[CE]; expires=Tue 09/19/2017 19:54:13 UTC; path=/; domain=.ancestrydev.com"
                    ,
                    "s_sq=%5B%5BB%5D%5D; path=/; domain=.ancestrydev.com",
                    "gpv_pn=ancestry%20us%20%3A%20dna%20%3A%20home%20%3A%20home; expires=Thu 09/20/2012 20:24:13 UTC; path=/; domain=.ancestrydev.com"
                    ,
                    "ATT=0; path=/; domain=.ancestrydev.com",
                    "VARS=LCID=1033; expires=Mon 09/20/2032 19:54:14 UTC; path=/; domain=.ancestrydev.com",
                    "VARSESSION=S=DA756eGxlkuNRJNdI2TBOQ%3d%3d&SLI=0&FIRSTSESSION=1&ITT=0; path=/; domain=.ancestrydev.com"
                    ,
                    "ANCATT=0; path=/; domain=.ancestrydev.com",
                    "mbox=check#true#1348170914|session#1348170852049-734884#1348172714|PC#1348170852049-734884.19#1350762854; expires=Sat 10/20/2012 19:54:14 UTC; path=/; domain=.ancestrydev.com"
                    ,
                    "ANCUUID=kbsVmYy03ukhQj84pJHxqD; expires=Mon 09/20/2032 19:54:12 UTC; path=/; domain=.ancestrydev.com"
                });
            }
            var headerOne = new List<string>()
            {
                "Content-Length:194",
                "Cache-Control:public, must-revalidate",
                "Content-Type:application/x-javascript",
                "Date:Thu, 20 Sep 2012 17:15:03 GMT",
                "ETag:JsJt380DknGc4kAEEn76og=="
            };
            var headerTwo = new List<string>()
            {
                "Content-Length:17423",
                "Cache-Control:public, must-revalidate",
                "Content-Type:application/x-javascript",
                "Date:Thu, 20 Sep 2012 17:15:03 GMT",
                "ETag:qloGz7WY45YMKQ1Fmuuw8A=="
            };
            var headerThree = new List<string>()
            {
                "Content-Length:2552",
                "Cache-Control:public, must-revalidate",
                "Content-Type:image/gif",
                "Date:Thu, 20 Sep 2012 17:15:03 GMT",
                "ETag:UAFdRlkmdsJ1EGIoGalWng=="
            };

            #endregion

            if (fakeScrapeParams.Resources == null)
            {
                //Uri, statusCode, StatusDesc, Headers
                var first = GetSession().List<Resource>(3).First(1)
                    .Impose(x => x.Uri, "http://c.mfcreativedev.com/webparts/banner/Banner.js?v=c5589edb")
                    .Impose(x => x.StatusCode, HttpStatusCode.OK)
                    .Impose(x => x.StatusDescription, "OK")
                    .Impose(x => x.Headers, headerOne)
                    .Next(1)
                    .Impose(x => x.Uri, "http://c.mfcreativedev.com/webparts/header/HeaderV1_2.js?v=730f5c7b1")
                    .Impose(x => x.StatusCode, HttpStatusCode.OK)
                    .Impose(x => x.StatusDescription, "OK")
                    .Impose(x => x.Headers, headerTwo)
                    .Next(1)
                    .Impose(x => x.Uri, "http://c.mfcreativedev.com/s/0/p/0/i/ances_logo.gif")
                    .Impose(x => x.StatusCode, HttpStatusCode.OK)
                    .Impose(x => x.StatusDescription, "OK")
                    .Impose(x => x.Headers, headerThree)
                    .All().Get().ToArray();
                fakeScrapeParams.Resources = first;
            }
            
            if (fakeScrapeParams.Elements == null)
            {
                var elements = new List<ScrapedElement>();
                var ele1 = new ScrapedElement()
                {
                    Attributes = new Dictionary<string, string>() {{"id", "mngb"}},
                    CorrespondingScrapedElement = null,
                    Css = new Dictionary<string, string>(),
                    Html = "<div id=\"mngb\"></div>",
                    Location = new Rectangle(0, 0, 800, 30),
                    LocationOnScreenshot = new Rectangle(0, 0, 800, 30),
                    Tag = "div",
                    Text = ""
                };
                elements.Add(ele1);
                fakeScrapeParams.Elements = new List<ScrapedElement>(elements);
            }

            Scrape scr = GetSession().Single<Scrape>()
                .Impose(x => x.Id, new ObjectId(fakeScrapeParams.Id))
                .Impose(x => x.ExcludeJquerySelector, fakeScrapeParams.Exclude)
                .Impose(x => x.IncludeJquerySelector, fakeScrapeParams.Include)
                .Impose(x => x.Script, fakeScrapeParams.Script)
                .Impose(x => x.BoundingRectangle, fakeScrapeParams.Bounding)
                .Impose(x => x.Path, new StringAsReference {Value = fakeScrapeParams.Path})
                .Impose(x => x.Elements, fakeScrapeParams.Elements)
                .Impose(x => x.Resources, fakeScrapeParams.Resources)
                .Impose(x => x.Html, fakeScrapeParams.Html)
                .Impose(x => x.HtmlRef, new StringAsReference {Value = fakeScrapeParams.HtmlRef})
                .Impose(x => x.Url, fakeScrapeParams.Url)
                .Impose(x => x.Screenshot, fakeScrapeParams.ScreenShot)
                .Impose(x => x.ScreenshotRef, new StringAsReference {Value = fakeScrapeParams.ScreenShotRef})
                .Impose(x => x.ViewportSize,
                        fakeScrapeParams.ViewportSize == null ? new Size(800, 600) : fakeScrapeParams.ViewportSize.Value)
                .Impose(x => x.Browser, fakeScrapeParams.Browser)
                .Impose(x => x.BrowserVersion, fakeScrapeParams.BrowserVersion)
                .Impose(x => x.TimeStamp,
                        fakeScrapeParams.TimeStamp == null ? DateTime.Now : fakeScrapeParams.TimeStamp.Value)
                .Impose(x => x.Platform, fakeScrapeParams.Platform)
                .Impose(x => x.Cookies, fakeScrapeParams.Cookies)
                .Get();
            return scr;
        }

        public Comparison CompareFakeScrapes(Scrape fakeScr1, Scrape fakeScr2)
        {
            var compare = new Comparer();
            Comparison comparison = compare.Compare(fakeScr1, fakeScr2);
            return comparison;
        }

        public PageResult FakePageResult(FakeComparisonResult fakeComparisonResult)
        {
            var result = GetSession().Single<PageResult>()
                .Impose(x => x.ChangedItems, fakeComparisonResult.ChangedResult)
                .Impose(x => x.AddedItems, fakeComparisonResult.AddedResult)
                .Impose(x => x.RemovedItems, fakeComparisonResult.RemovedResult)
                .Impose(x => x.UnchangedItems, fakeComparisonResult.UnchangedItems)
                .Impose(x => x.CssChangePercentage, fakeComparisonResult.CssPercent)
                .Impose(x => x.TextChangePercentage, fakeComparisonResult.TextPercent)
                .Impose(x => x.OverallElementPositionChangePercentage, fakeComparisonResult.OverallPercent)
                .Impose(x => x.Pixels, fakeComparisonResult.PixelResults)
                .Impose(x => x.Html, fakeComparisonResult.HtmlResults)
                .Impose(x => x.HtmlDiff, fakeComparisonResult.HtmlDiff)
                .Get();
            return result;
        }

        public Comparison FakeComparison(FakeComparisonParams fakeComparisonParams)
        {
            var hybrid1 = new ScrapeHybrid();
            var hybrid2 = new ScrapeHybrid();
            var result = new PageResult();
            if (fakeComparisonParams.ScrapeHybrids == null)
            {
                var idString = new StringAsReference {Value = "a1b2c3d4e5f6a7b8c9d1e"};
                var htmlString = new StringAsReference {Value = ""};

                hybrid1 = GetSession().Single<ScrapeHybrid>()
                    .Impose(x => x.ExcludeJquerySelector, @"''")
                    .Impose(x => x.IncludeJquerySelector, @"'body*'")
                    .Impose(x => x.Script, "")
                    .Impose(x => x.BoundingRectangle, new Rectangle(0, 0, 0, 0))
                    .Impose(x => x.Description, "")
                    .Impose(x => x.IdString, idString)
                    .Impose(x => x.Path, null)
                    .Impose(x => x.Resources, null)
                    .Impose(x => x.Html, htmlString)
                    .Impose(x => x.Url, "http://www.google.com")
                    .Impose(x => x.Screenshot, null)
                    .Impose(x => x.ViewportSize, new Size(800, 600))
                    .Impose(x => x.Browser, "Chrome")
                    .Impose(x => x.BrowserVersion, "10")
                    .Impose(x => x.TimeStamp, DateTime.Now)
                    .Impose(x => x.Platform, "Windows")
                    .Impose(x => x.Cookies, null)
                    .Get();
                hybrid2 = GetSession().Single<ScrapeHybrid>()
                    .Impose(x => x.ExcludeJquerySelector, @"''")
                    .Impose(x => x.IncludeJquerySelector, @"'body*'")
                    .Impose(x => x.Script, "")
                    .Impose(x => x.BoundingRectangle, new Rectangle(10, 10, 10, 10))
                    .Impose(x => x.Description, "")
                    .Impose(x => x.IdString, idString)
                    .Impose(x => x.Path, null)
                    .Impose(x => x.Resources, null)
                    .Impose(x => x.Html, htmlString)
                    .Impose(x => x.Url, "http://www.ancestry.com")
                    .Impose(x => x.Screenshot, null)
                    .Impose(x => x.ViewportSize, new Size(800, 600))
                    .Impose(x => x.Browser, "FireFox")
                    .Impose(x => x.BrowserVersion, "10")
                    .Impose(x => x.TimeStamp, DateTime.Now.AddDays(10))
                    .Impose(x => x.Platform, "Windows")
                    .Impose(x => x.Cookies, null)
                    .Get();
            }

            if (fakeComparisonParams.Result == null)
            {
                result = FakePageResult(new FakeComparisonResult());
            }

            Comparison fakedComparison = GetSession().Single<Comparison>()
                .Impose(x => x.Scrapes, fakeComparisonParams.ScrapeHybrids ?? new[] {hybrid1, hybrid2})
                .Impose(x => x.Scrapes, fakeComparisonParams.ScrapeHybrids ?? new[] {hybrid1, hybrid2})
                .Impose(x => x.Scrapes, fakeComparisonParams.ScrapeHybrids ?? new[] {hybrid1, hybrid2})
                .Impose(x => x.Id, fakeComparisonParams.ObjectId ?? new ObjectId("555b77b99b111d2aa4b33333"))
                .Impose(x => x.Path, fakeComparisonParams.Path ?? new StringAsReference())
                .Impose(x => x.Result, fakeComparisonParams.Result ?? result)
                .Get();

            return fakedComparison;
        }

        public Request FakeRequest(string url = null, string browser = null, string browserversion = null, Size? viewportSize = null)
        {
           Request rqst = GetSession().Single<Request>()
               .Impose(x => x.Browser, browser)
               .Impose(x => x.BrowserVersion, browserversion)
               .Impose(x => x.Script, null)
               .Impose(x => x.Url, url)
               .Get();
            return rqst ;
        }
       
        public PageData FakePageData(Size? size = null, string html = null, string[] cookies = null, string elements = null, Image screenshot = null, string browserName = "", string browserVersion = "", string platform = "", IEnumerable<object> resources = null, string url = null)
        {
            PageData pgdata = GetSession().Single<PageData>()
                .Impose(x => x.Size, size)
                .Impose(x => x.Html, html)
                .Impose(x => x.Cookies, cookies)
                .Impose(x => x.ElementsJson, elements)
                .Impose(x => x.Screenshot, screenshot)
                .Impose(x => x.BrowserName, browserName)
                .Impose(x => x.BrowserVersion, browserVersion)
                .Impose(x => x.Platform, platform)
                .Impose(x => x.Resources, resources)
                .Impose(x => x.Url, url)
                .Get();
            return pgdata;

        }

        public PageData FakePageData(FakePageDataParams fpd)
        {
            return FakePageData(fpd.Size,
                                fpd.Html,
                                fpd.Cookies,
                                fpd.Elements,
                                fpd.Screenshot,
                                fpd.BrowserName,
                                fpd.BrowserVersion,
                                fpd.Platform,
                                fpd.Resources,
                                fpd.Url);

        }

    }
}
