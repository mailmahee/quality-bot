namespace QualityBot.Test.Tests.QualityBotFake
{
    using System.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using AutoPoco;
    using AutoPoco.Engine;
    using MongoDB.Bson;
    using QualityBot.Compare;
    using QualityBot.ScrapePocos;
    using QualityBot.ComparePocos;
    using QualityBot.RequestPocos;
    using QualityBot.Util;

    internal class ResourceManagement
    {
        internal static Stream GetResourceStream(string resourceName)
        {
            var thisExe = Assembly.GetExecutingAssembly();
            var file = thisExe.GetManifestResourceStream(resourceName);
            return file;
        }
    }

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
            _id             = id;
            _exclude        = exclude;
            _include        = include;
            _script         = script;
            _bounding       = bounding;
            _path           = path;
            Elements        = elements;
            Resources       = resources;
            _html           = html;
            _htmlRef        = htmlRef;
            _url            = url;
            _screenShot     = screenShot;
            _screenShotRef  = screenShotRef;
            _viewportSize   = viewportSize;
            _browser        = browser;
            _browserVersion = browserVersion;
            _timeStamp      = timeStamp;
            _platform       = platform;
            _cookies        = cookies;
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
            get { return _cookies ?? new List<string>(); }
            set { _cookies = value; }
        }
    }

    internal class FakeComparisonResult
    {
        private ElementAddRemoveResult[] _addedResult;
        private ElementAddRemoveResult[] _removedResult;
        private ElementChangeResult[] _changedResult;
        private int _unchangedItems;
        private decimal _cssPercent;
        private decimal _textPercent;
        private decimal _overallPercent;
        private PixelResult _pixelResult;
        private HtmlResult _htmlResult;
        private string _htmlDiff;
        private readonly string _fakeImage = ConfigurationManager.AppSettings["staticBase64"];

        private readonly Image _dummyImage = Image.FromStream(ResourceManagement.GetResourceStream("QualityBot.Test.Tests.TestData.DummyImage.jpg"));

        public FakeComparisonResult(ElementChangeResult[] changedResult = null, ElementAddRemoveResult[] addedResult = null, 
            ElementAddRemoveResult[] removedResult = null, int unchangedItems = 0, decimal cssPercent = 0, decimal textPercent = 0, 
            decimal overallPercent = 0, PixelResult pixelResult = null, HtmlResult htmlResult = null, string htmlDiff = null)
        {
            _changedResult  = changedResult;
            _addedResult    = addedResult;
            _removedResult  = removedResult;
            _unchangedItems = unchangedItems;
            _cssPercent     = cssPercent;
            _textPercent    = textPercent;
            _overallPercent = overallPercent;
            _pixelResult    = pixelResult;
            _htmlResult     = htmlResult;
            _htmlDiff       = htmlDiff;
        }
        public ElementChangeResult[] TooMany()
        {
            var ch = new ElementChangeResult[51];
            for (int i = 0; i < 51; i++)
            {
                ch[i] = new ElementChangeResult
                {

                    PixelChanges = new PixelChange
                    {
                        From             = new Bitmap(_dummyImage),
                        FromStyle        = "",
                        FromClipped      = new Bitmap(_dummyImage),
                        FromClippedStyle = "",
                        FromMask         = new Bitmap(_dummyImage),
                        FromMaskStyle    = "",
                        To               = new Bitmap(_dummyImage),
                        ToStyle          = "",
                        ToClipped        = new Bitmap(_dummyImage),
                        ToClippedStyle   = "",
                        ToMask           = new Bitmap(_dummyImage),
                        ToMaskStyle      = "",
                        Diff             = new Bitmap(_dummyImage),
                        DiffStyle        = ""
                    }
                };
            }
            return ch;
        }
        public ElementChangeResult[] ChangedResult {
            get
            {
                return _changedResult ?? TooMany();
            }
            set { _changedResult = value; }
        }
        public ElementAddRemoveResult[] AddedResult
        {
            get
            {
                return _addedResult ?? new[]
                {
                    new ElementAddRemoveResult
                    {
                        Image             = new Bitmap(_dummyImage),
                        ImageStyle        = "",
                        ImageClipped      = new Bitmap(_dummyImage),
                        ImageClippedStyle = "",
                        ImageMask         = new Bitmap(_dummyImage),
                        ImageMaskStyle    = ""
                    }
                };
            }
            set { _addedResult = value; }
        }
        public ElementAddRemoveResult[] RemovedResult
        {
            get
            {
                return _removedResult ?? new[]
                {
                    new ElementAddRemoveResult
                    {
                        Image             = new Bitmap(_dummyImage),
                        ImageStyle        = "",
                        ImageClipped      = new Bitmap(_dummyImage),
                        ImageClippedStyle = "",
                        ImageMask         = new Bitmap(_dummyImage),
                        ImageMaskStyle    = ""
                    }
                };
            }
            set { _removedResult = value; }
        }
        public int UnchangedItems { get; set; }
        public decimal CssPercent { get; set; }
        public decimal TextPercent { get; set; }
        public decimal OverallPercent { get; set; }
        public string HtmlDiff 
        {
            get
            {
                return _htmlDiff ?? GetFromResources("QualityBot.Test.Tests.TestData.FakeHTML.txt");
            }
            set
            {
                _htmlDiff = value;
            } 
        }
        public PixelResult PixelResults
        {
            get 
            { return _pixelResult ?? new PixelResult
                {
                    Images = new[] { _fakeImage },
                    PercentChanged = 0
                }; 
            }
            set { _pixelResult = value; }
        }
        public HtmlResult HtmlResults
        {
            get 
            { return _htmlResult ?? new HtmlResult
                {
                    Images = new[] { _fakeImage },
                    PercentChanged = 0
                }; 
            }
            set { _htmlResult = value; }
        }
        internal string GetFromResources(string resourceName)
        {
            var assem = GetType().Assembly;
            using (var stream = assem.GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }

    internal class FakeComparisonParams
    {
        public FakeComparisonParams(ScrapeHybrid[] scrapeHybrids = null, ObjectId? objectId = null, StringAsReference path = null, PageResult result = null)
        {
            ScrapeHybrids = scrapeHybrids;
            ObjectId      = objectId;
            Path          = path;
            Result        = result;
        }

        public ScrapeHybrid[] ScrapeHybrids { get; set; }

        public ObjectId? ObjectId { get; set; }

        public StringAsReference Path { get; set; }

        public PageResult Result { get; set; }
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
                HttpWebRequest _HttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(_URL);
                _HttpWebRequest.AllowWriteStreamBuffering = true;
                // You can also specify additional header values like the user agent or the referer: (Optional)
                _HttpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)";
                _HttpWebRequest.Referer = "http://www.google.com/";
                // set timeout for 20 seconds (Optional)
                _HttpWebRequest.Timeout = 20000;
                // Request response:
                WebResponse _WebResponse = _HttpWebRequest.GetResponse();
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
        public string[] FakeCookies()
        {
            var cookies = new[]
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
                };
            return cookies;
        }
        public Scrape FakeScrape(FakeScrapeParams fakeScrapeParams)
        {

            fakeScrapeParams.Cookies = new List<string>();

            #region Header/Cookie Content

            if (!fakeScrapeParams.Cookies.Any())
            {
                fakeScrapeParams.Cookies.AddRange(FakeCookies());
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
                    Attributes                  = new Dictionary<string, string>() {{"id", "mngb"}},
                    CorrespondingScrapedElement = null,
                    Css                         = new Dictionary<string, string>(),
                    Html                        = "<div id=\"mngb\"></div>",
                    Location                    = new Rectangle(0, 0, 800, 30),
                    LocationOnScreenshot        = new Rectangle(0, 0, 800, 30),
                    Tag                         = "div",
                    Text                        = ""
                };
                elements.Add(ele1);
                fakeScrapeParams.Elements = new List<ScrapedElement>(elements);
            }
            var scr = new Scrape
            {
                Id                    = new ObjectId(fakeScrapeParams.Id),
                ExcludeJquerySelector = fakeScrapeParams.Exclude,
                IncludeJquerySelector = fakeScrapeParams.Include,
                Script                = fakeScrapeParams.Script,
                BoundingRectangle     = fakeScrapeParams.Bounding,
                Path                  = new StringAsReference {Value = fakeScrapeParams.Path},
                Elements              = fakeScrapeParams.Elements,
                Resources             = fakeScrapeParams.Resources,
                Html                  = fakeScrapeParams.Html,
                HtmlRef               = new StringAsReference {Value = fakeScrapeParams.HtmlRef},
                Url                   = fakeScrapeParams.Url,
                Screenshot            = fakeScrapeParams.ScreenShot,
                ScreenshotRef         = new StringAsReference {Value = fakeScrapeParams.ScreenShotRef},
                ViewportSize          = fakeScrapeParams.ViewportSize == null ? new Size(800 , 600) : fakeScrapeParams.ViewportSize.Value,
                Browser               = fakeScrapeParams.Browser,
                BrowserVersion        = fakeScrapeParams.BrowserVersion,
                TimeStamp             = fakeScrapeParams.TimeStamp == null ? DateTime.Now : fakeScrapeParams.TimeStamp.Value,
                Platform              = fakeScrapeParams.Platform,
                Cookies               = fakeScrapeParams.Cookies
            };
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
            var result = new PageResult
            {
                ChangedItems                           = fakeComparisonResult.ChangedResult,
                AddedItems                             = fakeComparisonResult.AddedResult,
                RemovedItems                           = fakeComparisonResult.RemovedResult,
                UnchangedItems                         = fakeComparisonResult.UnchangedItems,
                CssChangePercentage                    = fakeComparisonResult.CssPercent,
                TextChangePercentage                   = fakeComparisonResult.TextPercent,
                OverallElementPositionChangePercentage = fakeComparisonResult.OverallPercent,
                Pixels                                 = fakeComparisonResult.PixelResults,
                Html                                   = fakeComparisonResult.HtmlResults,
                HtmlDiff                               = fakeComparisonResult.HtmlDiff
            };
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

                hybrid1 = new ScrapeHybrid
                {
                    ExcludeJquerySelector = @"''",
                    IncludeJquerySelector = @"'body*'",
                    Script                = "",
                    BoundingRectangle     = new Rectangle(0, 0, 0, 0),
                    Description           = "",
                    IdString              = idString,
                    Path                  = null,
                    Resources             = null,
                    Html                  = htmlString,
                    Url                   = "http://www.google.com",
                    Screenshot            = null,
                    ViewportSize          = new Size(800, 600),
                    Browser               = "Chrome",
                    BrowserVersion        = "10",
                    TimeStamp             = DateTime.Now,
                    Platform              = "Windows",
                    Cookies               = null
                };
                hybrid2 = new ScrapeHybrid
                {
                    ExcludeJquerySelector = @"''",
                    IncludeJquerySelector = @"'body*'",
                    Script                = "",
                    BoundingRectangle     = new Rectangle(10, 10, 10, 10),
                    Description           = "",
                    IdString              = idString,
                    Path                  = null,
                    Resources             = null,
                    Html                  = htmlString,
                    Url                   = "http://www.ancestry.com",
                    Screenshot            = null,
                    ViewportSize          = new Size(800, 600),
                    Browser               = "FireFox",
                    BrowserVersion        = "10",
                    TimeStamp             = DateTime.Now.AddDays(10),
                    Platform              = "Windows",
                    Cookies               = null
                };
            }

            if (fakeComparisonParams.Result == null)
            {
                result = FakePageResult(new FakeComparisonResult());
            }
            var fakedComparison = new Comparison
            {
                Scrapes = fakeComparisonParams.ScrapeHybrids ?? new[] { hybrid1, hybrid2 },
                Id      = fakeComparisonParams.ObjectId      ?? new ObjectId("555b77b99b111d2aa4b33333"),
                Path    = fakeComparisonParams.Path          ?? new StringAsReference(),
                Result  = fakeComparisonParams.Result        ?? result
            };

            return fakedComparison;
        }
        public Request FakeRequest(string url = null, string browser = null, string browserversion = null, Size? viewportSize = null)
        {
            return new Request
            {
                Browser        = browser,
                BrowserVersion = browserversion,
                Script         = null,
                Url            = url
            };
        }    
        public PageData FakePageData(Size? size = null, string html = null, string[] cookies = null, 
            string elements = null, Image screenshot = null, string browserName = "", string browserVersion = "", 
            string platform = "", IEnumerable<object> resources = null, string url = null)
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