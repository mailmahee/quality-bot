namespace QualityBot.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using Newtonsoft.Json;
    using QualityBot.ScrapePocos;
    using QualityBot.Util;

    public class ScrapePersister : IPersister<Scrape>
    {
        private MediaServicePersister _mediaServicePersister;

        private MongoDbPersister _mongoDbPersister;

        private MongoDbPersister MongoDbPersister
        {
            get
            {
                return _mongoDbPersister ?? (_mongoDbPersister = new MongoDbPersister());
            }
        }

        public ScrapePersister()
        {
            _mediaServicePersister = new MediaServicePersister();
        }

        public Scrape RetrieveFromDisc(string file)
        {
            var scrapeData = File.ReadAllText(file);
            var scrape = JsonConvert.DeserializeObject<Scrape>(scrapeData);

            // Retrieve html
            if (string.IsNullOrWhiteSpace(scrape.Html))
            {
                var html = File.ReadAllText(scrape.HtmlRef.Value);
                scrape.Html = html;
            }

            // Retrieve image
            if (string.IsNullOrWhiteSpace(scrape.Screenshot))
            {
                var image = Image.FromFile(scrape.ScreenshotRef.Value);
                var base64Image = ImageUtil.ImageToBase64(image, ImageFormat.Png);
                scrape.Screenshot = base64Image;
            }

            return scrape;
        }

        public IEnumerable<Scrape> RetrieveFromMongoDb(Scrape data)
        {
            return MongoDbPersister.LoadFromMongoDb(data);
        }

        public void SaveToDisc(string outputDir, Scrape data)
        {
            Directory.CreateDirectory(outputDir);
            var now = DateTime.Now.ToString("yyyyMMddHHmmssfffffff");

            // Save image
            var screenshot = data.Screenshot;
            data.ScreenshotRef.Value = ImageUtil.SaveImageToDisc(outputDir, screenshot, "Screenshot");

            // Save html
            var html = data.Html;
            var saveLocation = Path.Combine(outputDir, string.Format(@"{0}_Html.html", now));
            data.HtmlRef.Value = saveLocation;
            File.WriteAllText(saveLocation, html);

            // Eliminate circular references
            EliminateCircularReferences(data);
            
            // Save json
            var file = Path.Combine(outputDir, string.Format(@"{0}_scrape.json", now));
            data.Path.Value = file;
            data.Screenshot = string.Empty;
            data.Html = string.Empty;
            var json = JsonUtil.Serialize(data);
            File.WriteAllText(file, json);

            // Restore scrape to original state
            data.Screenshot = screenshot;
            data.Html = html;
        }

        public void SaveToMongoDb(Scrape data)
        {
            var html = data.Html;
            var screenshot = data.Screenshot;

            SaveResourcesToMediaService(data);

            // Eliminate circular references
            EliminateCircularReferences(data);

            data.Screenshot = string.Empty;
            data.Html = string.Empty;
            MongoDbPersister.InsertItemInCollection(data);

            // Restore scrape to original state
            data.Screenshot = screenshot;
            data.Html = html;
        }

        private static void EliminateCircularReferences(Scrape data)
        {
            foreach (var element in data.Elements)
            {
                element.CorrespondingElement = null;
            }
        }

        private void SaveResourcesToMediaService(Scrape data)
        {
            // Save image
            data.ScreenshotRef.Value = _mediaServicePersister.SaveImageToMediaService(data.Screenshot, "Screenshot", "png");

            // Save html
            var html = data.Html;
            data.HtmlRef.Value = _mediaServicePersister.SaveHtmlToMediaService(html, "Html", "html");
        }
    }
}