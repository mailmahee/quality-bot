namespace QualityBot.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Mapper;
    using Newtonsoft.Json;
    using QualityBot.ComparePocos;
    using QualityBot.Util;

    public sealed class ComparePersister : IPersister<Comparison>, IDisposable
    {
        private MediaServicePersister _mediaServicePersister;

        private MongoDbPersister _mongoDbPersister;

        private SpriteUtil _spriteUtil;

        private string _style;

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
            _spriteUtil = new SpriteUtil();
        }

        public Comparison RetrieveFromDisc(string file)
        {
            var compareData = File.ReadAllText(file);
            var compare = JsonConvert.DeserializeObject<Comparison>(compareData);

            return compare;
        }

        public IEnumerable<Comparison> RetrieveFromMongoDb(string id)
        {
            return MongoDbPersister.LoadFromMongoDb<Comparison>(id);
        }

        public void SaveToDisc(string outputDir, Comparison data)
        {
            data.TimeStamp = DateTime.Now;
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

            // Stitch together images
            var images = GetAllImages(data.Result);
            var tooManyDiffs = data.Result.ChangedItems.Count > 50 || data.Result.AddedItems.Count > 100 || data.Result.AddedItems.Count > 100;
            if (images.Any() && !tooManyDiffs)
            {
                var sprite = _spriteUtil.MapImagesToSprite(images);
                var spriteImage = _spriteUtil.DrawSprite(sprite);

                // Save sprite to disc
                var base64Sprite = ImageUtil.ImageToBase64(spriteImage, ImageFormat.Png);
                var spritePath = ImageUtil.SaveImageToDisc(outputDir, base64Sprite, "sprite");
                _style = @"width: {0}px; height: {1}px; background: transparent url('" + spritePath + "') -{2}px -{3}px no-repeat";

                // Update styles
                UpdateStyles(data.Result, sprite);
            }
            else if (images.Any() && tooManyDiffs)
            {
                Image image;
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("QualityBot.toomany.png"))
                {
                    image = Image.FromStream(stream);
                }

                // Save sprite to disc
                var base64Sprite = ImageUtil.ImageToBase64(image, ImageFormat.Png);
                var spritePath = ImageUtil.SaveImageToDisc(outputDir, base64Sprite, "sprite");
                _style = @"width: {0}px; height: {1}px; background: transparent url('" + spritePath + "') -{2}px -{3}px no-repeat";

                // Update styles
                UpdateStyles(data.Result, image);
            }

            if (images.Any())
            {
                // Dispose all images
                images.ForEach(i => i.Dispose());

                // Set images to null (for serializer)
                SetImagesToNull(data.Result);
            }

            // Save html difference
            var html = data.Result.HtmlDiff;
            var saveLocation = Path.Combine(outputDir, string.Format(@"{0}_HtmlDiff.html", now));
            data.Result.HtmlDiff = saveLocation;
            File.WriteAllText(saveLocation, html);
            
            // Save json
            var file = Path.Combine(outputDir, string.Format(@"{0}_comparison.json", now));
            data.Path.Value = file;
            var json = JsonConvert.SerializeObject(data);
            File.WriteAllText(file, json);
        }

        public void SaveToMongoDb(Comparison data)
        {
            SaveResourcesToMediaService(data);
            data.TimeStamp = DateTime.Now;
            MongoDbPersister.InsertItemInCollection(data);
        }
        //Was private
        internal void SaveResourcesToMediaService(Comparison data)
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

            // Stitch together images
            var images = GetAllImages(data.Result);
            var tooManyDiffs = data.Result.ChangedItems.Count > 50 || data.Result.AddedItems.Count > 100 || data.Result.AddedItems.Count > 100;
            if (images.Any() && !tooManyDiffs)
            {
                var sprite = _spriteUtil.MapImagesToSprite(images);
                var spriteImage = _spriteUtil.DrawSprite(sprite);

                // Save sprite to Media Service
                var base64Sprite = ImageUtil.ImageToBase64(spriteImage, ImageFormat.Png);
                var spritePath = _mediaServicePersister.SaveImageToMediaService(base64Sprite, "Image", "png");
                _style = @"width: {0}px; height: {1}px; background: transparent url('" + spritePath + "') -{2}px -{3}px no-repeat";

                // Update styles
                UpdateStyles(data.Result, sprite);
            }
            else if (images.Any() && tooManyDiffs)
            {
                Image image;
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("QualityBot.toomany.png"))
                {
                    image = Image.FromStream(stream);
                }

                // Save sprite to Media Service
                var base64Sprite = ImageUtil.ImageToBase64(image, ImageFormat.Png);
                var spritePath = _mediaServicePersister.SaveImageToMediaService(base64Sprite, "Image", "png");
                _style = @"width: {0}px; height: {1}px; background: transparent url('" + spritePath + "') -{2}px -{3}px no-repeat";

                // Update styles
                UpdateStyles(data.Result, image);
            }

            if (images.Any())
            {
                // Dispose all images
                images.ForEach(i => i.Dispose());

                // Set images to null (for serializer)
                SetImagesToNull(data.Result);
            }

            // Save html difference
            var html = data.Result.HtmlDiff;
            data.Result.HtmlDiff = _mediaServicePersister.SaveHtmlToMediaService(html, "HtmlDiff", "html");
        }
        //Was Private
        internal void SetImagesToNull(PageResult result)
        {
            foreach (var e in result.AddedItems)
            {
                e.Image        = null;
                e.ImageClipped = null;
                e.ImageMask    = null;
            }

            foreach (var e in result.RemovedItems)
            {
                e.Image        = null;
                e.ImageClipped = null;
                e.ImageMask    = null;
            }

            foreach (var e in result.ChangedItems.Where(e => e.PixelChanges != null))
            {
                e.PixelChanges.From        = null;
                e.PixelChanges.FromClipped = null;
                e.PixelChanges.FromMask    = null;
                e.PixelChanges.To          = null;
                e.PixelChanges.ToClipped   = null;
                e.PixelChanges.ToMask      = null;
                e.PixelChanges.Diff        = null;
            }
        }
        //Was Private
        internal void UpdateStyles(PageResult result, Image image)
        {
            var style = string.Format(_style, image.Width, image.Height, 0, 0);

            foreach (var e in result.AddedItems)
            {
                e.ImageStyle        = style;
                e.ImageClippedStyle = style;
                e.ImageMaskStyle    = style;
            }

            foreach (var e in result.RemovedItems)
            {
                e.ImageStyle        = style;
                e.ImageClippedStyle = style;
                e.ImageMaskStyle    = style;
            }

            foreach (var e in result.ChangedItems.Where(e => e.PixelChanges != null))
            {
                e.PixelChanges.FromStyle        = style;
                e.PixelChanges.FromClippedStyle = style;
                e.PixelChanges.FromMaskStyle    = style;
                e.PixelChanges.ToStyle          = style;
                e.PixelChanges.ToClippedStyle   = style;
                e.PixelChanges.ToMaskStyle      = style;
                e.PixelChanges.DiffStyle        = style;
            }
        }
        //Was Private
        internal void UpdateStyles(PageResult result, Sprite sprite)
        {
            foreach (var e in result.AddedItems)
            {
                e.ImageStyle        = GetStyle(sprite, e.Image);
                e.ImageClippedStyle = GetStyle(sprite, e.ImageClipped);
                e.ImageMaskStyle    = GetStyle(sprite, e.ImageMask);
            }

            foreach (var e in result.RemovedItems)
            {
                e.ImageStyle        = GetStyle(sprite, e.Image);
                e.ImageClippedStyle = GetStyle(sprite, e.ImageClipped);
                e.ImageMaskStyle    = GetStyle(sprite, e.ImageMask);
            }

            foreach (var e in result.ChangedItems.Where(e => e.PixelChanges != null))
            {
                e.PixelChanges.FromStyle        = GetStyle(sprite, e.PixelChanges.From);
                e.PixelChanges.FromClippedStyle = GetStyle(sprite, e.PixelChanges.FromClipped);
                e.PixelChanges.FromMaskStyle    = GetStyle(sprite, e.PixelChanges.FromMask);
                e.PixelChanges.ToStyle          = GetStyle(sprite, e.PixelChanges.To);
                e.PixelChanges.ToClippedStyle   = GetStyle(sprite, e.PixelChanges.ToClipped);
                e.PixelChanges.ToMaskStyle      = GetStyle(sprite, e.PixelChanges.ToMask);
                e.PixelChanges.DiffStyle        = GetStyle(sprite, e.PixelChanges.Diff);
            }
        }
        //Was Private
        internal string GetStyle(Sprite sprite, Image image)
        {
            var r = sprite.MappedImages[image];
            return string.Format(_style, r.Width, r.Height, r.X, r.Y);
        }
        //Was Private
        internal Image[] GetAllImages(PageResult result)
        {
            var images = new List<Image>();

            // Save images from added elements
            foreach (var e in result.AddedItems)
            {
                images.AddRange(new [] { e.Image, e.ImageClipped, e.ImageMask });
            }

            // Save images from removed elements
            foreach (var e in result.RemovedItems)
            {
                images.AddRange(new[] { e.Image, e.ImageClipped, e.ImageMask });
            }

            // Save images from changed elements
            foreach (var e in result.ChangedItems.Where(e => e.PixelChanges != null))
            {
                images.AddRange(new[] { e.PixelChanges.From, e.PixelChanges.FromClipped, e.PixelChanges.FromMask, e.PixelChanges.To, e.PixelChanges.ToClipped, e.PixelChanges.ToMask, e.PixelChanges.Diff });
            }

            return images.ToArray();
        }

        public void Dispose()
        {
            _mediaServicePersister.Dispose();
        }
    }
}