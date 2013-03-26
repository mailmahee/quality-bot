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
        /// <summary>
        /// Initializes a new instance of the <see cref="ComparePersister" /> class.
        /// </summary>
        public ComparePersister()
        {
            _mediaServicePersister = new MediaServicePersister();
            _spriteUtil = new SpriteUtil();
        }
        /// <summary>
        /// Retrieves from disc.
        /// </summary>
        /// <param name="file">The file path.</param>
        /// <returns>A Comparison object</returns>
        public Comparison RetrieveFromDisc(string file)
        {
            var compareData = File.ReadAllText(file);
            var compare = JsonConvert.DeserializeObject<Comparison>(compareData);

            return compare;
        }
        /// <summary>
        /// Retrieves from mongo db.
        /// </summary>
        /// <param name="id">The mongoDB object id.</param>
        /// <returns>A IEnumerable object of Type Comparison</returns>
        public IEnumerable<Comparison> RetrieveFromMongoDb(string id)
        {
            return MongoDbPersister.LoadFromMongoDb<Comparison>(id);
        }
        /// <summary>
        /// Saves the Comparison to disc.
        /// </summary>
        /// <param name="outputDir">The output dir.</param>
        /// <param name="data">The Comparison data.</param>
        public void SaveToDisc(string outputDir, Comparison data)
        {
            data.TimeStamp = DateTime.Now;
            Directory.CreateDirectory(outputDir);
            var now = DateTime.Now.ToString("yyyyMMddHHmmssfffffff");

            // Stitch together images
            StitchImages(data, false, outputDir, now);

            // Save json
            var file = Path.Combine(outputDir, string.Format(@"{0}_comparison.json", now));
            data.Path.Value = file;
            var json = JsonConvert.SerializeObject(data);
            File.WriteAllText(file, json);
        }
        /// <summary>
        /// Saves the Comparison to mongo db.
        /// </summary>
        /// <param name="data">The Comparison data.</param>
        public void SaveToMongoDb(Comparison data)
        {
            StitchImages(data, true);
            data.TimeStamp = DateTime.Now;
            MongoDbPersister.InsertItemInCollection(data);
        }
        /// <summary>
        /// Stitches the images together and saves them.
        /// </summary>
        /// <param name="data">The Comparison data.</param>
        /// <param name="mediaPersister">if set to <c>true</c> it will save the data to the media service,
        ///  if set to <c>false</c> it will save the data to disc and will require the outputDir variable to work properly.</param>
        /// <param name="outputDir">The output directory, required if mediaPersister is set to <c>false</c>.</param>
        /// <param name="now">A <c>DateTime.Now.ToString()</c> result is required.</param>
        private void StitchImages(Comparison data, bool mediaPersister, string outputDir = null, string now = null)
        {
            // Save pixel diffs
            if (data.Result != null && data.Result.Pixels != null && data.Result.Pixels.Images != null)
            {
                for (var i = 0; i < data.Result.Pixels.Images.Length; i++)
                {
                    if (mediaPersister)
                    {
                        data.Result.Pixels.Images[i] = _mediaServicePersister.SaveImageToMediaService(data.Result.Pixels.Images[i], "PixelDiff", "png");
                    }
                    else
                    {
                        data.Result.Pixels.Images[i] = ImageUtil.SaveImageToDisc(outputDir, data.Result.Pixels.Images[i], "PixelDiff");
                    }
                }
            }
            // Save html diffs
            if (data.Result != null && data.Result.Html != null && data.Result.Html.Images != null)
            {
                for (var i = 0; i < data.Result.Html.Images.Length; i++)
                {
                    if (mediaPersister)
                    {
                        data.Result.Html.Images[i] = _mediaServicePersister.SaveImageToMediaService(data.Result.Html.Images[i], "HtmlDiff", "png");
                    }
                    else
                    {
                        data.Result.Html.Images[i] = ImageUtil.SaveImageToDisc(outputDir, data.Result.Html.Images[i], "HtmlDiff");
                    }
                }
            }

            var images = GetAllImages(data.Result);
            var changedItems = data.Result != null && data.Result.ChangedItems != null ? data.Result.ChangedItems.Length : 0;
            var addedItems = data.Result != null && data.Result.AddedItems != null ? data.Result.AddedItems.Length : 0;
            var removedItems = data.Result != null && data.Result.RemovedItems != null ? data.Result.RemovedItems.Length : 0;

            var tooManyDiffs = changedItems > 50 || addedItems > 100 || removedItems > 100;
            if (images.Any() && !tooManyDiffs)
            {
                var sprite = _spriteUtil.MapImagesToSprite(images);
                var spriteImage = _spriteUtil.DrawSprite(sprite);
                
                var base64Sprite = ImageUtil.ImageToBase64(spriteImage, ImageFormat.Png);
                var spritePath = "";
                if (mediaPersister)
                {
                    // Save sprite to Media Service
                    spritePath = _mediaServicePersister.SaveImageToMediaService(base64Sprite, "Image", "png");
                }
                else if (!mediaPersister)
                {
                    // Save sprite to disc
                    spritePath = ImageUtil.SaveImageToDisc(outputDir, base64Sprite, "sprite");
                }
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

                var base64Sprite = ImageUtil.ImageToBase64(image, ImageFormat.Png);
                var spritePath = "";
                if(mediaPersister)
                {
                    // Save sprite to Media Service
                    spritePath = _mediaServicePersister.SaveImageToMediaService(base64Sprite, "Image", "png");
                }
                else if (!mediaPersister)
                {
                    // Save sprite to Disc
                    spritePath = ImageUtil.SaveImageToDisc(outputDir, base64Sprite, "sprite");
                }

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
            if (data.Result != null)
            {
                var html = data.Result.HtmlDiff;
                if (mediaPersister)
                {
                    data.Result.HtmlDiff = _mediaServicePersister.SaveHtmlToMediaService(html, "HtmlDiff", "html");
                }
                else
                {
                    var saveLocation = Path.Combine(outputDir, string.Format(@"{0}_HtmlDiff.html", now));
                    data.Result.HtmlDiff = saveLocation;
                    File.WriteAllText(saveLocation, html);
                }
            }
        }
        /// <summary>
        /// Sets the images to null.
        /// </summary>
        /// <param name="result">The result.</param>
        private void SetImagesToNull(PageResult result)
        {
            if (result.AddedItems != null)
            {
                foreach (var e in result.AddedItems.Where(a => a != null))
                {
                    e.Image = null;
                    e.ImageClipped = null;
                    e.ImageMask = null;
                }
            }

            if (result.RemovedItems != null)
            {
                foreach (var e in result.RemovedItems.Where(r => r != null))
                {
                    e.Image = null;
                    e.ImageClipped = null;
                    e.ImageMask = null;
                }
            }

            if (result.ChangedItems != null)
            {
                foreach (var e in result.ChangedItems.Where(e => e != null && e.PixelChanges != null))
                {
                    e.PixelChanges.From = null;
                    e.PixelChanges.FromClipped = null;
                    e.PixelChanges.FromMask = null;
                    e.PixelChanges.To = null;
                    e.PixelChanges.ToClipped = null;
                    e.PixelChanges.ToMask = null;
                    e.PixelChanges.Diff = null;
                }
            }
        }
        /// <summary>
        /// Updates the image styles.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="image">The image.</param>
        private void UpdateStyles(PageResult result, Image image)
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
        /// <summary>
        /// Updates the sprite styles.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="sprite">The sprite.</param>
        private void UpdateStyles(PageResult result, Sprite sprite)
        {
            if (result.AddedItems != null)
            {
                foreach (var e in result.AddedItems)
                {
                    e.ImageStyle = GetStyle(sprite, e.Image);
                    e.ImageClippedStyle = GetStyle(sprite, e.ImageClipped);
                    e.ImageMaskStyle = GetStyle(sprite, e.ImageMask);
                }
            }

            if (result.RemovedItems != null)
            {
                foreach (var e in result.RemovedItems)
                {
                    e.ImageStyle = GetStyle(sprite, e.Image);
                    e.ImageClippedStyle = GetStyle(sprite, e.ImageClipped);
                    e.ImageMaskStyle = GetStyle(sprite, e.ImageMask);
                }
            }

            if (result.ChangedItems != null)
            {
                foreach (var e in result.ChangedItems.Where(e => e.PixelChanges != null))
                {
                    e.PixelChanges.FromStyle = GetStyle(sprite, e.PixelChanges.From);
                    e.PixelChanges.FromClippedStyle = GetStyle(sprite, e.PixelChanges.FromClipped);
                    e.PixelChanges.FromMaskStyle = GetStyle(sprite, e.PixelChanges.FromMask);
                    e.PixelChanges.ToStyle = GetStyle(sprite, e.PixelChanges.To);
                    e.PixelChanges.ToClippedStyle = GetStyle(sprite, e.PixelChanges.ToClipped);
                    e.PixelChanges.ToMaskStyle = GetStyle(sprite, e.PixelChanges.ToMask);
                    e.PixelChanges.DiffStyle = GetStyle(sprite, e.PixelChanges.Diff);
                }
            }
        }
        /// <summary>
        /// Gets the style.
        /// </summary>
        /// <param name="sprite">The sprite.</param>
        /// <param name="image">The image.</param>
        /// <returns>A string value</returns>
        private string GetStyle(Sprite sprite, Image image)
        {
            var r = sprite.MappedImages[image];
            return string.Format(_style, r.Width, r.Height, r.X, r.Y);
        }
        /// <summary>
        /// Gets all images.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>An array of type Image</returns>
        private Image[] GetAllImages(PageResult result)
        {
            var images = new List<Image>();

            // Save images from added elements
            if (result != null && result.AddedItems != null)
            {
                foreach (var e in result.AddedItems.Where(a => a != null))
                {
                    images.AddRange(new [] { e.Image, e.ImageClipped, e.ImageMask });
                }
            }

            // Save images from removed elements
            if (result != null && result.RemovedItems != null)
            {
                foreach (var e in result.RemovedItems.Where(r => r != null))
                {
                    images.AddRange(new[] { e.Image, e.ImageClipped, e.ImageMask });
                }
            }

            // Save images from changed elements
            if (result != null && result.ChangedItems != null)
            {
                foreach (var e in result.ChangedItems.Where(e => e.PixelChanges != null))
                {
                    images.AddRange(new[] { e.PixelChanges.From, e.PixelChanges.FromClipped, e.PixelChanges.FromMask, e.PixelChanges.To, e.PixelChanges.ToClipped, e.PixelChanges.ToMask, e.PixelChanges.Diff });
                }
            }

            return images.ToArray();
        }
        /// <summary>
        /// Releases all the resources being used by the MediaServicePersister.
        /// </summary>
        public void Dispose()
        {
            _mediaServicePersister.Dispose();
        }
    }
}