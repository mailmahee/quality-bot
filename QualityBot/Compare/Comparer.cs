namespace QualityBot.Compare
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using Diff;
    using QualityBot.ComparePocos;
    using QualityBot.ScrapePocos;
    using QualityBot.Util;

    public class Comparer
    {
        private ElementMapper _elementMapper;
        private ConflictResolver<ScrapedElement> _conflictResolver;
        private DiffMatchPatch _diffEngine;
        private ImageAttributes _ia;
        
        public Comparer()
        {
            _diffEngine       = new DiffMatchPatch { Diff_Timeout = 0 };
            var cm            = new ColorMatrix    { Matrix33 = 0.25f };
            _ia               = new ImageAttributes();
            _ia.SetColorMatrix(cm);
            _elementMapper    = new ElementMapper(_diffEngine);
            _conflictResolver = new ConflictResolver<ScrapedElement>();
        }

        /// <summary>
        /// Compares the specified pages and returns an object detailing the differences.
        /// </summary>
        /// <param name="pageA">The first page.</param>
        /// <param name="pageB">The second page.</param>
        /// <returns>An object of type CompareResult.</returns>
        public Comparison Compare(Scrape pageA, Scrape pageB)
        {
            return ComputeDiff(pageA, pageB);
        }

        /// <summary>
        /// Determines the difference between two CSS dictionaries.
        /// </summary>
        /// <param name="css1">The first CSS dictionary.</param>
        /// <param name="css2">The second CSS dictionary.</param>
        /// <param name="percentageChange">The change as a percentage.</param>
        /// <returns>An object of type CssChange.</returns>
        private static CssChange GetCssChanges(Dictionary<string, string> css1, Dictionary<string, string> css2, out decimal percentageChange)
        {
            var cssKeysA    = css1.Select(k => k.Key).ToArray();
            var cssKeysB    = css2.Select(k => k.Key).ToArray();
            var addedKeys   = cssKeysB.Except(cssKeysA).ToArray();
            var deletedKeys = cssKeysA.Except(cssKeysB).ToArray();
            var commonKeys  = cssKeysA.Except(deletedKeys);
            var changedCss  = (from key in commonKeys
                              where css1[key] != css2[key]
                              select new CssChangeDetail { From = css1[key], To = css2[key], Key = key }).ToArray();

            var length       = Math.Max(css1.Count, css2.Count);
            var delta        = changedCss.Length + addedKeys.Length + deletedKeys.Length;
            percentageChange = length.GetPercentageOfTotal(delta);

            return new CssChange
            {
                Added   = addedKeys.ToDictionary(key => key, key => css2[key]),
                Deleted = deletedKeys.ToDictionary(key => key, key => css1[key]),
                Changed = changedCss
            };
        }

        /// <summary>
        /// Returns the difference between two rectangles as a string description.
        /// </summary>
        /// <param name="r1">The first rectangle.</param>
        /// <param name="r2">The second rectangle.</param>
        /// <param name="percentageChange">The change as a percentage.</param>
        /// <returns>A string description.</returns>
        private static string GetLocationChanges(Rectangle r1, Rectangle r2, out decimal percentageChange)
        {
            var sb = new System.Text.StringBuilder();

            if (r1.Width != r2.Width)
            {
                sb.AppendLine(string.Format("Element width {0} by {1} pixels", r1.Width > r2.Width ? "decreased" : "increased", Math.Abs(r1.Width - r2.Width)));
            }

            if (r1.Height != r2.Height)
            {
                sb.AppendLine(string.Format("Element height {0} by {1} pixels", r1.Height > r2.Height ? "decreased" : "increased", Math.Abs(r1.Height - r2.Height)));
            }

            if (r1.X != r2.X)
            {
                sb.AppendLine(string.Format("Element moved {0} {1} pixels", r1.X > r2.X ? "left" : "right", Math.Abs(r1.X - r2.X)));
            }

            if (r1.Y != r2.Y)
            {
                sb.AppendLine(string.Format("Element moved {0} {1} pixels", r1.Y > r2.Y ? "up" : "down", Math.Abs(r1.Y - r2.Y)));
            }

            var commonArea = Rectangle.Intersect(r1, r2).Area();
            percentageChange = commonArea > 0 ? (commonArea / (decimal)r1.Area()) * 100 : 100;

            return sb.ToString();
        }
        
        /// <summary>
        /// Calculates the area of the rectangle.
        /// </summary>
        /// <param name="sideA">The first side.</param>
        /// <param name="sideB">The second side.</param>
        /// <returns>A long.</returns>
        private static double Hypotenuse(int sideA, int sideB)
        {
            return Math.Sqrt(Math.Pow(sideA, 2) + Math.Pow(sideB, 2));
        }

        private ScrapeHybrid AssembleScrapeHybrid(Scrape page, bool isBaseline)
        {
            return new ScrapeHybrid
                {
                    IncludeJquerySelector = page.IncludeJquerySelector,
                    ExcludeJquerySelector = page.ExcludeJquerySelector,
                    Script = page.Script,
                    BoundingRectangle = page.BoundingRectangle,
                    Description = isBaseline ? "Baseline" : "Delta",
                    IdString = page.IdString,
                    Path = page.Path,
                    Resources = page.Resources,
                    Cookies = page.Cookies,
                    Html = page.HtmlRef,
                    Url = page.Url,
                    Screenshot = page.ScreenshotRef,
                    ViewportSize = page.ViewportSize,
                    Browser = page.Browser,
                    BrowserVersion = page.BrowserVersion,
                    TimeStamp = page.TimeStamp,
                    Platform = page.Platform
                };
        }

        /// <summary>
        /// Compares the specified pages and returns an object detailing the differences.
        /// </summary>
        /// <param name="pageA">The first page.</param>
        /// <param name="pageB">The second page.</param>
        /// <returns>An object of type Comparison.</returns>
        private Comparison ComputeDiff(Scrape pageA, Scrape pageB)
        {
            Comparison comparison;

            using (Image pageScreenshotA = ImageUtil.Base64ToImage(pageA.Screenshot), pageScreenshotB = ImageUtil.Base64ToImage(pageB.Screenshot))
            {
                // Html diff
                var htmlDiff = GetHtmlDiffReport(pageA.Html, pageB.Html);

                // Pixel diff
                var pixelDiff = PixelDiff(pageScreenshotA, pageScreenshotB);

                // Match up elements
                var largestDiagonal = (decimal)Math.Max(
                        Hypotenuse(pageScreenshotA.Width, pageScreenshotA.Height),
                        Hypotenuse(pageScreenshotB.Width, pageScreenshotB.Height));
                CorrespondingElements(pageA, pageB, largestDiagonal);

                var matches = pageA.Elements.Where(e => e.CorrespondingScrapedElement != null).ToArray();

                // Get changes
                var changedItems = GetChanges(matches, pageScreenshotA, pageScreenshotB, pageA, pageB).ToArray();

                var unchanged = matches.Count(m => !m.HasChanges);
                var changed = matches.Where(m => m.HasChanges).ToArray();
                var deleted = pageA.Elements.Where(e => e.CorrespondingScrapedElement == null).ToArray();
                var added = pageB.Elements.Where(e => e.CorrespondingScrapedElement == null).ToArray();

                var removedItems = deleted.Select(e => GetElementData(pageScreenshotA, e, pageA)).ToArray();
                var addedItems = added.Select(e => GetElementData(pageScreenshotB, e, pageB)).ToArray();

                // Compute overall percentages
                var total = (added.Length + deleted.Length + unchanged + changed.Length) * 100;
                var addedDeleted = (added.Length + deleted.Length) * 100;

                var cssChangePercentage = TotalChangePercentage(total, changedItems.Sum(i => i.CssPercentageChange), addedDeleted);
                var textChangePercentage = TotalChangePercentage(total, changedItems.Sum(i => i.TextPercentageChange), addedDeleted);
                var overallElementPositionChangePercentage = TotalChangePercentage(total, changedItems.Sum(i => i.LocationPercentageChange), addedDeleted);
                var htmlPercentChanged = TotalChangePercentage(total, changedItems.Sum(i => i.HtmlPercentageChange), addedDeleted);

                // Create Added/Changed elements and Deleted elements images
                var outlinedImages = OutlinedImagesAsBase64(pageScreenshotA, pageScreenshotB, added, deleted, changedItems).ToArray();

                var scrapeHybridA = AssembleScrapeHybrid(pageA, true);
                var scrapeHybridB = AssembleScrapeHybrid(pageB, false);

                comparison = new Comparison
                {
                    Scrapes = new[] { scrapeHybridA, scrapeHybridB },
                    Result = new PageResult
                        {
                            HtmlDiff       = htmlDiff,
                            Pixels         = pixelDiff,
                            ChangedItems   = changedItems,
                            AddedItems     = addedItems,
                            RemovedItems   = removedItems,
                            UnchangedItems = unchanged,
                            CssChangePercentage  = cssChangePercentage,
                            TextChangePercentage = textChangePercentage,
                            OverallElementPositionChangePercentage = overallElementPositionChangePercentage,
                            Html = new HtmlResult { PercentChanged = htmlPercentChanged, Images = outlinedImages },
                        }
                };
            }
            
            return comparison;
        }
        private Image DrawRectanglesOnImage(IEnumerable<Rectangle> rectangles, Image image, Color color)
        {
            var newImage = (Image)image.Clone();
            using (var g = Graphics.FromImage(newImage))
            {
                foreach (var r in rectangles)
                {
                    var rO = r;
                    rO.Width--;
                    rO.Height--;
                    g.DrawRectangle(new Pen(color, 1f), rO);
                }
            }

            return newImage;
        }

        private IEnumerable<ElementChangeResult> GetChanges(IEnumerable<ScrapedElement> matches, Image pageScreenshotA, Image pageScreenshotB, Scrape pageA, Scrape pageB)
        {
            foreach (var match in matches)
            {
                ElementChangeResult change;
                if (HasChanges(match, pageScreenshotA, pageScreenshotB, pageA, pageB, out change))
                {
                    match.HasChanges = true;
                    yield return change;
                }
            }
        }

        /// <summary>
        /// Returns information about the given element.
        /// </summary>
        /// <param name="pageScreenshot">The screenshot of the page containing the element.</param>
        /// <param name="scrapedElement">The element.</param>
        /// <param name="page">The page containing the element.</param>
        /// <returns>An ElementAddRemoveResult object.</returns>
        private ElementAddRemoveResult GetElementData(Image pageScreenshot, ScrapedElement scrapedElement, Scrape page)
        {
            var originalA = ImageUtil.CropImage(pageScreenshot, scrapedElement.LocationOnScreenshot);
            var regionA   = ImageUtil.GetClippedRegion(scrapedElement.LocationOnScreenshot, page.Elements.Select(e => e.LocationOnScreenshot));
            var clippedA  = ImageUtil.GetClippedImage(new Size(scrapedElement.LocationOnScreenshot.Width, scrapedElement.LocationOnScreenshot.Height), originalA, regionA);
            var imageMask = ImageUtil.DrawRegionAsMasks(new Size(scrapedElement.LocationOnScreenshot.Width, scrapedElement.LocationOnScreenshot.Height), regionA, originalA, _ia);

            var add = new ElementAddRemoveResult
            {
                Attributes   = scrapedElement.Attributes,
                Html         = scrapedElement.Html,
                Text         = scrapedElement.Text,
                Location     = scrapedElement.LocationOnScreenshot,
                Tag          = scrapedElement.Tag,
                Image        = originalA,
                ImageClipped = clippedA,
                ImageMask    = imageMask
            };

            return add;
        }
        
        private string GetHtmlDiffReport(string html1, string html2)
        {
            var wordDiffs = _diffEngine.DiffWordMode(html1, html2);
            var prettyHtml = _diffEngine.DiffPrettyHtml(wordDiffs);

            return prettyHtml;
        }

        // TODO: high value testing area
        private bool HasChanges(ScrapedElement element, Image pageScreenshotA, Image pageScreenshotB, Scrape pageA, Scrape pageB, out ElementChangeResult changes)
        {
            var correspondingScrapedElement = element.CorrespondingScrapedElement;
            changes = new ElementChangeResult();

            var changed = false;

            // Location changed
            decimal percentageChange = 0;
            if (!correspondingScrapedElement.Location.Equals(element.Location))
            {
                changed = true;
                changes.LocationChanges = GetLocationChanges(correspondingScrapedElement.Location, element.Location, out percentageChange);
            }

            changes.LocationPercentageChange = percentageChange;

            // Css changed
            percentageChange = 0;
            if (!correspondingScrapedElement.Css.DictionaryEqual(element.Css))
            {
                changed = true;
                changes.CssChanges = GetCssChanges(correspondingScrapedElement.Css, element.Css, out percentageChange);
            }

            changes.CssPercentageChange = percentageChange;

            // Html changed
            percentageChange = 0;
            if (correspondingScrapedElement.Html != element.Html)
            {
                changed = true;
                changes.HtmlChanges = GetStringChanges(correspondingScrapedElement.Html, element.Html, out percentageChange);
            }

            changes.HtmlPercentageChange = percentageChange;

            // Text changed
            percentageChange = 0;
            if (correspondingScrapedElement.Text != element.Text)
            {
                changed = true;
                changes.TextChanges = GetStringChanges(correspondingScrapedElement.Text, element.Text, out percentageChange);
            }

            changes.TextPercentageChange = percentageChange;

            // Pixels changed
            changes.PixelChanges = GetPixelChanges(pageScreenshotA, pageScreenshotB, element, correspondingScrapedElement, pageA, pageB, out percentageChange);
            changes.PixelPercentageChange = percentageChange;
            if (percentageChange > 0M)
            {
                changed = true;
            }

            // Store location on screenshot
            changes.LocationOnScreenshot = element.LocationOnScreenshot;

            return changed;
        }

        /// <summary>
        /// Calculates the pixel difference between two elements.
        /// </summary>
        /// <param name="psA">The screenshot of the first page.</param>
        /// <param name="psB">The screenshot of the second page.</param>
        /// <param name="eA">The first element.</param>
        /// <param name="eB">The second element.</param>
        /// <param name="pA">The information about the first page.</param>
        /// <param name="pB">The information about the second page.</param>
        /// <param name="percentageChange">The change as a percentage.</param>
        /// <returns>An object containing information about the pixel differences.</returns>
        private PixelChange GetPixelChanges(Image psA, Image psB, ScrapedElement eA, ScrapedElement eB, Scrape pA, Scrape pB, out decimal percentageChange)
        {
            PixelChange pixelChange = null;

            Image originalA       = ImageUtil.CropImage(psA, eA.LocationOnScreenshot);
            Image originalB       = ImageUtil.CropImage(psB, eB.LocationOnScreenshot);
            Region regionA        = ImageUtil.GetClippedRegion(eA.LocationOnScreenshot, pA.Elements.Select(e => e.LocationOnScreenshot));
            Region regionB        = ImageUtil.GetClippedRegion(eB.LocationOnScreenshot, pB.Elements.Select(e => e.LocationOnScreenshot));
            Bitmap clippedA       = ImageUtil.GetClippedImage(new Size(eA.LocationOnScreenshot.Width, eA.LocationOnScreenshot.Height), originalA, regionA);
            Bitmap clippedB       = ImageUtil.GetClippedImage(new Size(eB.LocationOnScreenshot.Width, eB.LocationOnScreenshot.Height), originalB, regionB);
            Bitmap diffMask       = ImageUtil.BitmapDiff(clippedA, clippedB, _ia, out percentageChange);
            Bitmap fromRegionMask = ImageUtil.DrawRegionAsMasks(new Size(eA.LocationOnScreenshot.Width, eA.LocationOnScreenshot.Height), regionA, originalA, _ia);
            Bitmap toRegionMask   = ImageUtil.DrawRegionAsMasks(new Size(eB.LocationOnScreenshot.Width, eB.LocationOnScreenshot.Height), regionB, originalB, _ia);

            if (percentageChange > 0 || eB.LocationOnScreenshot.Width != eA.LocationOnScreenshot.Width || eB.LocationOnScreenshot.Height != eA.LocationOnScreenshot.Height)
            {
                pixelChange = new PixelChange
                {
                    From        = originalA,
                    FromClipped = clippedA,
                    FromMask    = fromRegionMask,
                    To          = originalB,
                    ToClipped   = clippedB,
                    ToMask      = toRegionMask,
                    Diff        = diffMask
                };
            }

            regionA.Dispose();
            regionB.Dispose();

            return pixelChange;
        }

        /// <summary>
        /// Pretty prints as html the difference between two strings.
        /// </summary>
        /// <param name="string1">The first string.</param>
        /// <param name="string2">The second string.</param>
        /// <param name="percentageChange">The change as a percentage.</param>
        /// <returns>A string.</returns>
        private string GetStringChanges(string string1, string string2, out decimal percentageChange)
        {
            var diffs        = _diffEngine.DiffMain(string1, string2);
            var prettyHtml   = _diffEngine.DiffPrettyHtml(diffs);
            var length       = Math.Max(string1.Length, string2.Length);
            var delta        = _diffEngine.DiffLevenshtein(diffs);
            percentageChange = length.GetPercentageOfTotal(delta);

            return prettyHtml;
        }

        private IEnumerable<string> OutlinedImagesAsBase64(Image pageScreenshotA, Image pageScreenshotB, IEnumerable<ScrapedElement> addedElements, IEnumerable<ScrapedElement> deletedElements, IEnumerable<ElementChangeResult> changedItems)
        {
            var rectangles = changedItems.Where(c => c.LocationPercentageChange > 0 || c.PixelPercentageChange > 0 || c.TextPercentageChange > 0)
                                         .Select(c => c.LocationOnScreenshot)
                                         .ToArray();
            if (rectangles.Any())
            {
                yield return OutlineImage(rectangles, pageScreenshotA, Color.Yellow);
            }

            rectangles = deletedElements.Select(c => c.LocationOnScreenshot).ToArray();
            if (rectangles.Any())
            {
                yield return OutlineImage(rectangles, pageScreenshotA, Color.Red);
            }

            rectangles = addedElements.Select(c => c.LocationOnScreenshot).ToArray();
            if (rectangles.Any())
            {
                yield return OutlineImage(rectangles, pageScreenshotB, Color.Green);
            }
        }

        private string OutlineImage(IEnumerable<Rectangle> rectangles, Image image, Color color)
        {
            string base64;
            using (var imageWithOutlines = DrawRectanglesOnImage(rectangles, image, color))
            {
                base64 = ImageUtil.ImageToBase64(imageWithOutlines, ImageFormat.Png);
            }

            return base64;
        }

        private PixelResult PixelDiff(Image pageScreenshotA, Image pageScreenshotB)
        {
            decimal pixelPercentageDifferent;
            var pixelResult = new PixelResult();

            using (var bitmapDiff = ImageUtil.BitmapDiff((Bitmap)pageScreenshotA, (Bitmap)pageScreenshotB, _ia, out pixelPercentageDifferent))
            {
                var base64 = ImageUtil.ImageToBase64(bitmapDiff, ImageFormat.Png);
                pixelResult.Images = new[] { base64 };
                pixelResult.PercentChanged = pixelPercentageDifferent;
            }

            return pixelResult;
        }

        private decimal TotalChangePercentage(decimal total, params decimal[] changes)
        {
            var delta = changes.Sum();
            var cssChangePercentage = total.GetPercentageOfTotal(delta);

            return cssChangePercentage;
        }

        /// <summary>
        /// Attempts to determine which element on page A corresponds to which element on page B.
        /// </summary>
        /// <returns>A collection of mappings.</returns>
        private void CorrespondingElements(Scrape pageA, Scrape pageB, decimal maxDistance)
        {
            var elementMatches = new List<ElementMatch<ScrapedElement>>();

            foreach (var element in pageA.Elements)
            {
                ScrapedElement match;
                Tuple<ElementMatch<ScrapedElement>, decimal>[] matches;

                if (_elementMapper.HasExactMatch(element, pageB.Elements, out match) || _elementMapper.HasIdMatch(element, pageB.Elements, out match))
                {
                    element.CorrespondingScrapedElement = match;
                    match.CorrespondingScrapedElement = element;
                }
                else if (_elementMapper.HasSimilarElements(element, pageB.Elements, maxDistance, out matches))
                {
                    var elementMatch = new ElementMatch<ScrapedElement>
                    {
                        This = element,
                        Matches = new Queue<Tuple<ElementMatch<ScrapedElement>, decimal>>(matches)
                    };

                    elementMatch.SetToNext();
                    elementMatches.Add(elementMatch);
                }
            }

            // Resolve conflicts
            _conflictResolver.ResolveAllConflicts(elementMatches.ToArray());

            foreach (var element in elementMatches.Where(m => m.Match != null))
            {
                element.This.CorrespondingScrapedElement = element.Match.This;
                element.Match.This.CorrespondingScrapedElement = element.This;
            }
        }
    }
}