namespace QualityBot.Scrapers.Facades
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;

    using Newtonsoft.Json;

    using QualityBot.RequestPocos;
    using QualityBot.ScrapePocos;
    using QualityBot.Scrapers.Interfaces;
    using QualityBot.Util;

    public class PhantomJsFacade : IBrowserFacade
    {
        private static string _phantomDir;

        private static string _phantomPath;

        private static long _counter;

        private string _callback;

        private string _html;

        private string _scriptFile;

        private Request _request;

        private string _elements;

        private string _screenshot;

        private string _cookies;

        private string _resources;

        private string _viewport;

        private string _scriptPath;

        public PhantomJsFacade(Request request)
        {
            _request = request;
            Config();
        }

        private void Config()
        {
            _cookies     = string.Empty;
            _scriptPath  = string.Empty;
            _screenshot  = string.Empty;
            _elements    = string.Empty;
            _resources   = string.Empty;
            _viewport    = string.Empty;
            _html        = string.Empty;
            _scriptFile  = string.Empty;

            // Only set once
            if (string.IsNullOrWhiteSpace(_phantomPath))
            {
                var file = IsLinux() ? "phantomjs" : "phantomjs.exe";
                _phantomPath = SettingsUtil.GetPathOfFile(file);
                _phantomDir = Path.GetDirectoryName(_phantomPath);
                _counter = 0;
            }
        }

        private void RunUserScript()
        {
            _callback = string.Format("{0}{1}", DateTime.Now.ToString("yyyyMMddHHmmss"), ++_counter);
            _scriptFile = String.Format("{0}script.js", _callback);
            _scriptPath = Path.Combine(_phantomDir, _scriptFile);
            File.WriteAllText(_scriptPath, _request.Script);
        }

        private string[] GetCookies()
        {
            var cookieLine = _cookies;
            var jCookies = new Dictionary<string, object>[] { };
            if (cookieLine != null)
            {
                jCookies = JsonConvert.DeserializeObject<Dictionary<string, object>[]>(cookieLine);
            }

            var cookies = (from c in jCookies
                           where c.ContainsKey("name")
                           let name    = c["name"].ToString()
                           let value   = c.ContainsKey("value")   ? c["value"].ToString()                : String.Empty
                           let domain  = c.ContainsKey("domain")  ? c["domain"].ToString()               : String.Empty
                           let path    = c.ContainsKey("path")    ? c["path"].ToString()                 : String.Empty
                           let expires = c.ContainsKey("expires") ? DateTime.Parse((string)c["expires"]) : (DateTime?)null
                           select string.Format("{0}={1}; expires={2}; path={3}; domain={4}", name, value, expires, path, domain)).ToArray();

            return cookies;
        }

        private Image GetScreenshotImage()
        {
            var pageImage = ImageUtil.Base64ToImage(_screenshot);

            return pageImage;
        }

        private IEnumerable<string> GetPageResources()
        {
            var resources = new string[] { };
            if (_resources != null)
            {
                resources = JsonConvert.DeserializeObject<string[]>(_resources);
            }

            return resources.NonNull(r => r).Where(r => !string.IsNullOrWhiteSpace(r));
        }

        private Size GetViewportSize()
        {
            var jSize = JsonConvert.DeserializeObject<dynamic>(_viewport);
            var size = new Size((int)jSize[0], (int)jSize[1]);

            return size;
        }

        public PageData ScrapeData(bool useCurrent = true)
        {
            RunUserScript();

            ScrapePage();

            var size = GetViewportSize();
            var cookies = GetCookies();
            var screenshot = GetScreenshotImage();
            var resources = GetPageResources();

            var pageData = new PageData
            {
                BrowserName = "PhantomJS",
                BrowserVersion = string.Empty,
                Platform = "Windows",
                Cookies = cookies,
                ElementsJson = _elements,
                Html = _html,
                Resources = resources,
                Screenshot = screenshot,
                Size = size,
                Url = _request.Url
            };

            return pageData;
        }

        private void ScrapePage()
        {
// ReSharper disable PossibleInvalidOperationException
            var args = string.Format(
                @"scrape.js {0} {1} {2} {3} {4} ""{5}"" {6}",
                _request.Url,
                _request.ViewportResolution.Value.Width,
                _request.ViewportResolution.Value.Height,
                _request.IncludeJquerySelector,
                _request.ExcludeJquerySelector,
                _scriptFile,
                _callback);
// ReSharper restore PossibleInvalidOperationException

            RunCmd(_phantomPath, args);

            var output = Path.Combine(_phantomDir, _callback);
            var fs = new FileStream(output, FileMode.Open);
            using (var sr = new StreamReader(fs))
            {
                _viewport   = sr.ReadLine();
                _resources  = sr.ReadLine();
                _screenshot = sr.ReadLine();
                _elements   = sr.ReadLine();
                _cookies    = sr.ReadLine();
                _html       = sr.ReadToEnd();
            }
            
            // Delete phantomjs output
            File.Delete(output);

            // Delete user script
            File.Delete(_scriptPath);
        }

        /// <summary>
        /// Determines if the current platform is Linux.
        /// </summary>
        /// <returns>True if the current platform is Linux.</returns>
        private static bool IsLinux()
        {
            var p = (int)Environment.OSVersion.Platform;
            return (p == 4) || (p == 6) || (p == 128);
        }

        /// <summary>
        /// Runs the specified executable.
        /// </summary>
        /// <param name="exePath">The path to the executable.</param>
        /// <param name="args">The arguments to pass to the executable.</param>
        /// <returns>The console output.</returns>
        private void RunCmd(string exePath, string args)
        {
            if (string.IsNullOrWhiteSpace(exePath)) throw new ArgumentNullException("exePath");

            var fileInfo = new FileInfo(exePath);
            var directoryName = fileInfo.DirectoryName;

            if (directoryName != null)
            {
                var pInfo = new ProcessStartInfo(exePath)
                {
                    WorkingDirectory = directoryName,
                    Arguments = args,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };
                
                var p = Process.Start(pInfo);
                p.BeginOutputReadLine();
                p.WaitForExit();
            }
        }

        public void Dispose()
        {
            // Nothing to do for this implementation
        }
    }
}