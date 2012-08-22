namespace QualityBot.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;

    using QualityBot.ComparePocos;
    using QualityBot.Util;

    public class ComparePersister : IPersister<Comparison>
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

        public ComparePersister()
        {
            _mediaServicePersister = new MediaServicePersister();
        }

        public Comparison RetrieveFromDisc(string file)
        {
            var compareData = File.ReadAllText(file);
            var compare = JsonConvert.DeserializeObject<Comparison>(compareData);

            return compare;
        }

        public IEnumerable<Comparison> RetrieveFromMongoDb(Comparison data)
        {
            return MongoDbPersister.LoadFromMongoDb(data);
        }

        public void SaveToDisc(string outputDir, Comparison data)
        {
            Directory.CreateDirectory(outputDir);
            var now = DateTime.Now.ToString("yyyyMMddHHmmssfffffff");

            // Save pixel diffs
            for (var i = 0; i < data.Result.Pixels.Images.Count; i++)
            {
                data.Result.Pixels.Images[i] = ImageUtil.SaveImageToDisc(outputDir, data.Result.Pixels.Images[i], "PixelDiff");
            }

            // Save html diffs
            for (var i = 0; i < data.Result.Html.Images.Count; i++)
            {
                data.Result.Html.Images[i] = ImageUtil.SaveImageToDisc(outputDir, data.Result.Html.Images[i], "HtmlDiff");
            }

            // Save images from added elements
            foreach (var e in data.Result.AddedItems)
            {
                e.Image = ImageUtil.SaveImageToDisc(outputDir, e.Image, "Image");
                e.ImageClipped = ImageUtil.SaveImageToDisc(outputDir, e.ImageClipped, "ImageClipped");
                e.ImageMask = ImageUtil.SaveImageToDisc(outputDir, e.ImageMask, "ImageMask");
            }

            // Save images from removed elements
            foreach (var e in data.Result.RemovedItems)
            {
                e.Image = ImageUtil.SaveImageToDisc(outputDir, e.Image, "Image");
                e.ImageClipped = ImageUtil.SaveImageToDisc(outputDir, e.ImageClipped, "ImageClipped");
                e.ImageMask = ImageUtil.SaveImageToDisc(outputDir, e.ImageMask, "ImageMask");
            }

            // Save images from changed elements
            foreach (var e in data.Result.ChangedItems.Where(e => e.PixelChanges != null))
            {
                e.PixelChanges.From = ImageUtil.SaveImageToDisc(outputDir, e.PixelChanges.From, "From");
                e.PixelChanges.FromClipped = ImageUtil.SaveImageToDisc(outputDir, e.PixelChanges.FromClipped, "FromClipped");
                e.PixelChanges.FromMask = ImageUtil.SaveImageToDisc(outputDir, e.PixelChanges.FromMask, "FromMask");
                e.PixelChanges.To = ImageUtil.SaveImageToDisc(outputDir, e.PixelChanges.To, "To");
                e.PixelChanges.ToClipped = ImageUtil.SaveImageToDisc(outputDir, e.PixelChanges.ToClipped, "ToClipped");
                e.PixelChanges.ToMask = ImageUtil.SaveImageToDisc(outputDir, e.PixelChanges.ToMask, "ToMask");
                e.PixelChanges.Diff = ImageUtil.SaveImageToDisc(outputDir, e.PixelChanges.Diff, "Diff");
            }

            // Save html difference
            var html = data.Result.HtmlDiff;
            var saveLocation = Path.Combine(outputDir, string.Format(@"{0}_HtmlDiff.html", now));
            data.Result.HtmlDiff = saveLocation;
            File.WriteAllText(saveLocation, html);
            
            // Save json
            var file = Path.Combine(outputDir, string.Format(@"{0}_comparison.json", now));
            data.Path.Value = file;
            var json = JsonUtil.Serialize(data);
            File.WriteAllText(file, json);
        }

        public void SaveToMongoDb(Comparison data)
        {
            SaveResourcesToMediaService(data);
            MongoDbPersister.InsertItemInCollection(data);
        }

        private void SaveResourcesToMediaService(Comparison data)
        {
            // Save pixel diffs
            for (var i = 0; i < data.Result.Pixels.Images.Count; i++)
            {
                data.Result.Pixels.Images[i] = _mediaServicePersister.SaveImageToMediaService(data.Result.Pixels.Images[i], "PixelDiff", "png");
            }

            // Save html diffs
            for (var i = 0; i < data.Result.Html.Images.Count; i++)
            {
                data.Result.Html.Images[i] = _mediaServicePersister.SaveImageToMediaService(data.Result.Html.Images[i], "HtmlDiff", "png");
            }

            // Save images from added elements
            foreach (var e in data.Result.AddedItems)
            {
                e.Image = _mediaServicePersister.SaveImageToMediaService(e.Image, "Image", "png");
                e.ImageClipped = _mediaServicePersister.SaveImageToMediaService(e.ImageClipped, "ImageClipped", "png");
                e.ImageMask = _mediaServicePersister.SaveImageToMediaService(e.ImageMask, "ImageMask", "png");
            }

            // Save images from removed elements
            foreach (var e in data.Result.RemovedItems)
            {
                e.Image = _mediaServicePersister.SaveImageToMediaService(e.Image, "Image", "png");
                e.ImageClipped = _mediaServicePersister.SaveImageToMediaService(e.ImageClipped, "ImageClipped", "png");
                e.ImageMask = _mediaServicePersister.SaveImageToMediaService(e.ImageMask, "ImageMask", "png");
            }

            // Save images from changed elements
            foreach (var e in data.Result.ChangedItems.Where(e => e.PixelChanges != null))
            {
                e.PixelChanges.From = _mediaServicePersister.SaveImageToMediaService(e.PixelChanges.From, "From", "png");
                e.PixelChanges.FromClipped = _mediaServicePersister.SaveImageToMediaService(e.PixelChanges.FromClipped, "FromClipped", "png");
                e.PixelChanges.FromMask = _mediaServicePersister.SaveImageToMediaService(e.PixelChanges.FromMask, "FromMask", "png");
                e.PixelChanges.To = _mediaServicePersister.SaveImageToMediaService(e.PixelChanges.To, "To", "png");
                e.PixelChanges.ToClipped = _mediaServicePersister.SaveImageToMediaService(e.PixelChanges.ToClipped, "ToClipped", "png");
                e.PixelChanges.ToMask = _mediaServicePersister.SaveImageToMediaService(e.PixelChanges.ToMask, "ToMask", "png");
                e.PixelChanges.Diff = _mediaServicePersister.SaveImageToMediaService(e.PixelChanges.Diff, "Diff", "png");
            }

            // Save html difference
            var html = data.Result.HtmlDiff;
            data.Result.HtmlDiff = _mediaServicePersister.SaveHtmlToMediaService(html, "HtmlDiff", "html");

            //// Save scrapes data
            //foreach (dynamic scrape in data.scrapes)
            //{
            //    scrape.html = _mediaServicePersister.SaveHtmlToMediaService(html, "ScrapeHtml", "html");
            //    scrape.screenshot = _mediaServicePersister.SaveImageToMediaService(scrape.screenshot, "ScrapeScreenshot", "png");
            //}
        }
    }
}