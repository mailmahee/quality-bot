
namespace QualityBot
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Linq;
    using System.Web.Script.Serialization;

    using QualityBot.ComparePocos;
    using QualityBot.ScrapePocos;
    using QualityBot.Util;

    public class Comparer
    {
        /// <summary>
        /// Compares the specified pages and returns an object detailing the differences.
        /// </summary>
        /// <param name="jsonA">The first page.</param>
        /// <param name="jsonB">The second page.</param>
        /// <returns>An object of type CompareResult.</returns>
        public Comparison Compare(string jsonA, string jsonB)
        {
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            var pageA = serializer.Deserialize<Scrape>(jsonA);
            var pageB = serializer.Deserialize<Scrape>(jsonB);

            return Compare(pageA, pageB);
        }

        /// <summary>
        /// Compares the specified pages and returns an object detailing the differences.
        /// </summary>
        /// <param name="pageA">The first page.</param>
        /// <param name="pageB">The second page.</param>
        /// <returns>An object of type CompareResult.</returns>
        public Comparison Compare(Scrape pageA, Scrape pageB)
        {
            var comparison = ComputeDiff(pageA, pageB);
            return comparison;
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
            var cssKeysA = css1.Select(k => k.Key).ToArray();
            var cssKeysB = css2.Select(k => k.Key).ToArray();
            var addedKeys = cssKeysB.Except(cssKeysA).ToArray();
            var deletedKeys = cssKeysA.Except(cssKeysB).ToArray();
            var commonKeys = cssKeysA.Except(deletedKeys);
            var changedCss = (from key in commonKeys
                              where css1[key] != css2[key]
                              select new CssChangeDetail { From = css1[key], To = css2[key], Key = key }).ToList();

            var length = css1.Count > css2.Count ? css1.Count : css2.Count;
            var delta = (changedCss.Count + addedKeys.Length + deletedKeys.Length);
            percentageChange = length > 0 ? (delta / (decimal)length) * 100 : 0;

            return new CssChange
            {
                Added = addedKeys.ToDictionary(key => key, key => css2[key]),
                Deleted = deletedKeys.ToDictionary(key => key, key => css1[key]),
                Changed = changedCss
            };
        }

        /// <summary>
        /// Returns information about the given element.
        /// </summary>
        /// <param name="pageScreenshot">The screenshot of the page containing the element.</param>
        /// <param name="element">The element.</param>
        /// <param name="page">The page containing the element.</param>
        /// <param name="ia">The image attributes.</param>
        /// <returns>An ElementAddRemoveResult object.</returns>
        private static ElementAddRemoveResult GetElementData(Image pageScreenshot, ElementInfo element, Scrape page, ImageAttributes ia)
        {
            var originalA = ImageUtil.CropImage(pageScreenshot, element.LocationOnScreenshot);
            var regionA = ImageUtil.GetClippedRegion(element.LocationOnScreenshot, page.Elements.Select(e => e.LocationOnScreenshot));
            var clippedA = ImageUtil.GetClippedImage(new Size(element.LocationOnScreenshot.Width, element.LocationOnScreenshot.Height), originalA, regionA);

            var add = new ElementAddRemoveResult
            {
                Attributes = element.Attributes,
                Html = element.Html,
                Text = element.Text,
                Location = element.LocationOnScreenshot,
                Tag = element.Tag,
                Image = ImageUtil.ImageToBase64(originalA, ImageFormat.Png),
                ImageClipped = ImageUtil.ImageToBase64(clippedA, ImageFormat.Png),
                ImageMask = ImageUtil.ImageToBase64(ImageUtil.DrawRegionAsMasks(new Size(element.LocationOnScreenshot.Width, element.LocationOnScreenshot.Height), regionA, originalA, ia), ImageFormat.Png)
            };

            return add;
        }

        /// <summary>
        /// Finds and returns elements with similar html tags.
        /// </summary>
        /// <param name="tag">The tag to match.</param>
        /// <param name="candidateElements">The elements that might contain a match.</param>
        /// <returns>A collection of matching elements.</returns>
        private static IEnumerable<ElementInfo> GetElementsBySimilarTag(string tag, IEnumerable<ElementInfo> candidateElements)
        {
            IEnumerable<ElementInfo> similarElements;
            var parentTags = new[]
                {
                    "body", "div", "span", "table", "tr", "td", "th", "tbody", 
                    "thead", "tfoot", "form", "fieldset", "optgroup"
                };
            var listTags = new[] { "ul", "ol", "dl" };
            var listItemTags = new[] { "li", "dt", "dd" };
            var textTags = new[]
                {
                    "h1", "h2", "h3", "h4", "h5", "h6", "p", "strong", 
                    "em", "b", "i", "tt", "sub", "sup", "big", "small"
                };

            if (tag.In(parentTags))
            {
                similarElements = candidateElements.Where(cE => cE.Tag.In(parentTags));
            }
            else if (tag.In(listTags))
            {
                similarElements = candidateElements.Where(cE => cE.Tag.In(listTags));
            }
            else if (tag.In(listItemTags))
            {
                similarElements = candidateElements.Where(cE => cE.Tag.In(listItemTags));
            }
            else if (tag.In(textTags))
            {
                similarElements = candidateElements.Where(cE => cE.Tag.In(textTags));
            }
            else
            {
                similarElements = candidateElements.Where(cE => cE.Tag == tag);
            }

            return similarElements;
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
            percentageChange = commonArea > 0 ? 100 - ((commonArea / (decimal)r1.Area()) * 100) : 100;

            return sb.ToString();
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
        /// <param name="ia">The image attributes to apply to the masks.</param>
        /// <param name="percentageChange">The change as a percentage.</param>
        /// <returns>An object containing information about the pixel differences.</returns>
        private static PixelChange GetPixelChanges(Image psA, Image psB, ElementInfo eA, ElementInfo eB, Scrape pA, Scrape pB, ImageAttributes ia, out decimal percentageChange)
        {
            PixelChange pixelChange = null;

            var originalA = ImageUtil.CropImage(psA, eA.LocationOnScreenshot);
            var originalB = ImageUtil.CropImage(psB, eB.LocationOnScreenshot);
            var regionA = ImageUtil.GetClippedRegion(eA.LocationOnScreenshot, pA.Elements.Select(e => e.LocationOnScreenshot));
            var regionB = ImageUtil.GetClippedRegion(eB.LocationOnScreenshot, pB.Elements.Select(e => e.LocationOnScreenshot));
            var clippedA = ImageUtil.GetClippedImage(new Size(eA.LocationOnScreenshot.Width, eA.LocationOnScreenshot.Height), originalA, regionA);
            var clippedB = ImageUtil.GetClippedImage(new Size(eB.LocationOnScreenshot.Width, eB.LocationOnScreenshot.Height), originalB, regionB);
            var diffMask = ImageUtil.BitmapDiff(clippedA, clippedB, ia, out percentageChange);

            if (percentageChange > 0 || eB.LocationOnScreenshot.Width != eA.LocationOnScreenshot.Width || eB.LocationOnScreenshot.Height != eA.LocationOnScreenshot.Height)
            {
                pixelChange = new PixelChange
                {
                    From = ImageUtil.ImageToBase64(originalA, ImageFormat.Png),
                    FromClipped = ImageUtil.ImageToBase64(clippedA, ImageFormat.Png),
                    FromMask = ImageUtil.ImageToBase64(ImageUtil.DrawRegionAsMasks(new Size(eA.LocationOnScreenshot.Width, eA.LocationOnScreenshot.Height), regionA, originalA, ia), ImageFormat.Png),
                    To = ImageUtil.ImageToBase64(originalB, ImageFormat.Png),
                    ToClipped = ImageUtil.ImageToBase64(clippedB, ImageFormat.Png),
                    ToMask = ImageUtil.ImageToBase64(ImageUtil.DrawRegionAsMasks(new Size(eB.LocationOnScreenshot.Width, eB.LocationOnScreenshot.Height), regionB, originalB, ia), ImageFormat.Png),
                    Diff = ImageUtil.ImageToBase64(diffMask, ImageFormat.Png)
                };
            }

            originalA.Dispose();
            originalB.Dispose();
            regionA.Dispose();
            regionB.Dispose();
            clippedA.Dispose();
            clippedB.Dispose();
            diffMask.Dispose();

            return pixelChange;
        }

        /// <summary>
        /// Pretty prints as html the difference between two strings.
        /// </summary>
        /// <param name="string1">The first string.</param>
        /// <param name="string2">The second string.</param>
        /// <param name="diffEngine">The levenshtein difference engine.</param>
        /// <param name="percentageChange">The change as a percentage.</param>
        /// <returns>A string.</returns>
        private static string GetStringChanges(string string1, string string2, diff_match_patch diffEngine, out decimal percentageChange)
        {
            var diffs = diffEngine.diff_main(string1, string2);
            var length = string1.Length > string2.Length ? string1.Length : string2.Length;
            var delta = diffEngine.diff_levenshtein(diffs);
            percentageChange = length > 0 ? (delta / (decimal)length) * 100 : 0;
            return diffEngine.diff_prettyHtml(diffs);
        }

        /// <summary>
        /// Computes the levenshtein distance between two strings.
        /// </summary>
        /// <param name="text1">The first string.</param>
        /// <param name="text2">The second string.</param>
        /// <param name="diffEngine">The levenshtein difference engine.</param>
        /// <returns>An integer value.</returns>
        private static int TextDistance(string text1, string text2, diff_match_patch diffEngine)
        {
            if (string.IsNullOrWhiteSpace(text1) && string.IsNullOrWhiteSpace(text2)) return 0;
            return diffEngine.diff_levenshtein(diffEngine.diff_main(text1, text2));
        }

        /// <summary>
        /// Determines the attribute similarity of two given attribute dictionaries.
        /// </summary>
        /// <param name="eA">The first element.</param>
        /// <param name="eB">The second element.</param>
        /// <param name="diffEngine">The levenshtein difference engine.</param>
        /// <returns>A decimal value.</returns>
        private decimal AttributesSimilarity(ElementInfo eA, ElementInfo eB, diff_match_patch diffEngine)
        {
            var keysA = eA.Attributes.Select(k => k.Key).ToArray();
            var keysB = eB.Attributes.Select(k => k.Key).ToArray();
            var added = keysB.Except(keysA).ToArray();
            var deleted = keysA.Except(keysB).ToArray();
            var common = keysA.Except(deleted).ToArray();
            var changed = (from key in common
                           where eA.Attributes[key] != eB.Attributes[key]
                           select new AttributeChangeDetail { From = eA.Attributes[key], To = eB.Attributes[key], Key = key }).ToArray();
            var unchanged = common.Except(changed.Select(i => i.Key)).ToArray();

            decimal totalPercentage = (added.Length * 100) + (deleted.Length * 100);
            foreach (var detail in changed)
            {
                var maxLength = detail.From.Length > detail.To.Length ? detail.From.Length : detail.To.Length;
                var distance = diffEngine.diff_levenshtein(diffEngine.diff_main(detail.From, detail.To));
                totalPercentage += maxLength > 0 ? (distance / (decimal)maxLength) * 100 : 0;
            }

            var total = (unchanged.Length + changed.Length + deleted.Length + added.Length) * 100;
            var change = total > 0 ? Math.Round((totalPercentage / total) * 100, 3) : 0;
            return change;
        }

        /// <summary>
        /// Compares the specified pages and returns an object detailing the differences.
        /// </summary>
        /// <param name="pageA">The first page.</param>
        /// <param name="pageB">The second page.</param>
        /// <returns>An object of type Comparison.</returns>
        private Comparison ComputeDiff(Scrape pageA, Scrape pageB)
        {
            // Ensure output directory exists
            var comparison = new Comparison
                {
                    Scrapes =
                        new[]
                            {
                                new ScrapeHybrid
                                    {
                                        Description    = "Baseline",
                                        Id             = pageA.Id,
                                        Path           = pageA.Path,
                                        Resources      = pageA.Resources,
                                        Cookies        = pageA.Cookies,
                                        Html           = pageA.HtmlRef,
                                        Url            = pageA.Url,
                                        Screenshot     = pageA.ScreenshotRef,
                                        ViewportSize   = pageA.ViewportSize,
                                        Browser        = pageA.Browser,
                                        BrowserVersion = pageA.BrowserVersion,
                                        TimeStamp      = pageA.TimeStamp,
                                        Platform       = pageA.Platform
                                    },
                                new ScrapeHybrid
                                    {
                                        Description    = "Delta",
                                        Id             = pageB.Id,
                                        Path           = pageB.Path,
                                        Resources      = pageB.Resources,
                                        Cookies        = pageB.Cookies,
                                        Html           = pageB.HtmlRef,
                                        Url            = pageB.Url,
                                        Screenshot     = pageB.ScreenshotRef,
                                        ViewportSize   = pageB.ViewportSize,
                                        Browser        = pageB.Browser,
                                        BrowserVersion = pageB.BrowserVersion,
                                        TimeStamp      = pageB.TimeStamp,
                                        Platform       = pageB.Platform
                                    }
                            }
                };

            var result = new PageResult();
            var diffEngine = new diff_match_patch { Diff_Timeout = 0 };
            var cm = new ColorMatrix { Matrix33 = 0.25f };
            var ia = new ImageAttributes();
            ia.SetColorMatrix(cm);
            var pageScreenshotA = ImageUtil.Base64ToImage(pageA.Screenshot);
            var pageScreenshotB = ImageUtil.Base64ToImage(pageB.Screenshot);

            /* Compute html diff (code)
             * This only does a text based comparison so you can't 
             *  really derive html nodes from it
             */
            result.HtmlDiff = diffEngine.diff_prettyHtml(diffEngine.diff_wordMode(pageA.Html, pageB.Html));

            /* Pixel diff
             * This is more significant than a byte diff, and 
             * only takes slightly longer to compute
             */
            decimal pixelPercentageDifferent;
            var pixelResult = new PixelResult();
            var bitmapDiff = ImageUtil.BitmapDiff((Bitmap)pageScreenshotA, (Bitmap)pageScreenshotB, ia, out pixelPercentageDifferent);
            pixelResult.Images.Add(ImageUtil.ImageToBase64(bitmapDiff, ImageFormat.Png));
            pixelResult.PercentChanged = Math.Round(pixelPercentageDifferent, 3);
            result.Pixels = pixelResult;
            bitmapDiff.Dispose();

            /* Find differences (added, deleted or modified)
             */
            ElementInfo[] unchanged; ElementInfo[] changed; ElementInfo[] added; ElementInfo[] deleted;
            FindCorrespondingElements(pageA, pageB, out unchanged, out changed, out added, out deleted, diffEngine);

            // Check changed, determine what was changed
            var changeMap = new Dictionary<ElementChangeResult, ElementInfo>();
            foreach (var eB in changed)
            {
                var eA = eB.CorrespondingElement;
                var itemResult = new ElementChangeResult();

                // Location changed
                decimal percentageChange = 0;
                if (!eA.Location.Equals(eB.Location)) itemResult.LocationChanges = GetLocationChanges(eA.Location, eB.Location, out percentageChange);
                itemResult.LocationPercentageChange = Math.Round(percentageChange, 3);

                // Css changed
                percentageChange = 0;
                if (!eA.Css.DictionaryEqual(eB.Css)) itemResult.CssChanges = GetCssChanges(eA.Css, eB.Css, out percentageChange);
                itemResult.CssPercentageChange = Math.Round(percentageChange, 3);

                // Html changed
                percentageChange = 0;
                if (eA.Html != eB.Html) itemResult.HtmlChanges = GetStringChanges(eA.Html, eB.Html, diffEngine, out percentageChange);
                itemResult.HtmlPercentageChange = Math.Round(percentageChange, 3);

                // Text changed
                percentageChange = 0;
                if (eA.Text != eB.Text) itemResult.TextChanges = GetStringChanges(eA.Text, eB.Text, diffEngine, out percentageChange);
                itemResult.TextPercentageChange = Math.Round(percentageChange, 3);

                // Pixels changed
                itemResult.PixelChanges = GetPixelChanges(pageScreenshotA, pageScreenshotB, eA, eB, pageA, pageB, ia, out percentageChange);
                itemResult.PixelPercentageChange = Math.Round(percentageChange, 3);

                changeMap.Add(itemResult, eB);
                result.ChangedItems.Add(itemResult);
            }

            // Check unchanged for pixel diff
            var moveList = new List<ElementInfo>();
            foreach (var e in unchanged)
            {
                decimal percentageChange;
                var pixelChanges = GetPixelChanges(pageScreenshotA, pageScreenshotB, e, e, pageA, pageB, ia, out percentageChange);
                if (pixelChanges != null)
                {
                    var itemResult = new ElementChangeResult { PixelChanges = pixelChanges, PixelPercentageChange = Math.Round(percentageChange, 3) };
                    result.ChangedItems.Add(itemResult);
                    changeMap.Add(itemResult, e);
                    moveList.Add(e);
                }
            }

            unchanged = unchanged.Except(moveList).ToArray();
            changed = changed.Concat(moveList).ToArray();

            foreach (var e in added)
            {
                result.AddedItems.Add(GetElementData(pageScreenshotB, e, pageB, ia));
            }

            foreach (var e in deleted)
            {
                result.RemovedItems.Add(GetElementData(pageScreenshotA, e, pageA, ia));
            }

            var total = (unchanged.Length + changed.Length + deleted.Length + added.Length) * 100;
            var delta = result.ChangedItems.Sum(i => i.CssPercentageChange) + (added.Length * 100) + (deleted.Length * 100);
            result.CssChangePercentage = total > 0 ? Math.Round((delta / total) * 100, 3) : 0;

            delta = result.ChangedItems.Sum(i => i.HtmlPercentageChange) + (added.Length * 100) + (deleted.Length * 100);
            var htmlChangePercentage = total > 0 ? Math.Round((delta / total) * 100, 3) : 0;

            delta = result.ChangedItems.Sum(i => i.TextPercentageChange) + (added.Length * 100) + (deleted.Length * 100);
            result.TextChangePercentage = total > 0 ? Math.Round((delta / total) * 100, 3) : 0;

            delta = result.ChangedItems.Sum(i => i.LocationPercentageChange) + (added.Length * 100) + (deleted.Length * 100);
            result.OverallElementPositionChangePercentage = total > 0 ? Math.Round((delta / total) * 100, 3) : 0;

            result.UnchangedItems = unchanged.Length;

            var htmlResult = new HtmlResult
            {
                PercentChanged = htmlChangePercentage
            };

            // Output Added and Changed Elements image
            if (added.Length > 0 || changed.Length > 0)
            {
                var addChangeImage = (Image)pageScreenshotB.Clone();
                using (var g = Graphics.FromImage(addChangeImage))
                {
                    // ReSharper disable AccessToDisposedClosure
                    added.Select(e => e.LocationOnScreenshot).ForEach(r =>
                            {
                                var rO = r;
                                rO.Width--;
                                rO.Height--;
                                g.DrawRectangle(new Pen(Color.Green, 1f), rO);
                            });
                    result.ChangedItems
                        .Where(c => c.LocationPercentageChange > 0 || c.PixelPercentageChange > 0 || c.TextPercentageChange > 0)
                        .Select(c => changeMap[c].LocationOnScreenshot)
                        .ForEach(r =>
                            {
                                var rO = r;
                                rO.Width--;
                                rO.Height--;
                                g.DrawRectangle(new Pen(Color.Yellow, 1f), rO);
                            });
                    // ReSharper restore AccessToDisposedClosure
                }

                htmlResult.Images.Add(ImageUtil.ImageToBase64(addChangeImage, ImageFormat.Png));
                addChangeImage.Dispose();
            }

            // Output Deleted Elements image
            if (deleted.Length > 0)
            {
                var deleteImage = (Image)pageScreenshotA.Clone();
                using (var g = Graphics.FromImage(deleteImage))
                {
                    // ReSharper disable AccessToDisposedClosure
                    deleted.Select(e => e.LocationOnScreenshot).ForEach(r =>
                            {
                                var rO = r;
                                rO.Width--;
                                rO.Height--;
                                g.DrawRectangle(new Pen(Color.Red, 1f), rO);
                            });
                    // ReSharper restore AccessToDisposedClosure
                }

                htmlResult.Images.Add(ImageUtil.ImageToBase64(deleteImage, ImageFormat.Png));
                deleteImage.Dispose();
            }

            result.Html = htmlResult;
            comparison.Result = result;

            pageScreenshotA.Dispose();
            pageScreenshotB.Dispose();

            return comparison;
        }

        /// <summary>
        /// Attempts to determine which element on page A corresponds to which element on page B.
        /// </summary>
        /// <returns>A collection of mappings.</returns>
        private void FindCorrespondingElements(Scrape pageA, Scrape pageB, out ElementInfo[] unchanged, out ElementInfo[] changed, out ElementInfo[] added, out ElementInfo[] deleted, diff_match_patch diffEngine)
        {
            /* Find exact matches (html, css, and location is equivalent)
             *  The pixels may have changed, but we'll check that later
             */
            var unchangedListA = new List<ElementInfo>();
            var unchangedListB = new List<ElementInfo>();

            foreach (var eA in pageA.Elements)
            {
                var a = eA;
                foreach (var eB in pageB.Elements.Where(eB => a == eB))
                {
                    eB.CorrespondingElement = eA;
                    eA.CorrespondingElement = eB;
                    unchangedListA.Add(eA);
                    unchangedListB.Add(eB);
                    break;
                }
            }
            var pageElementsA = pageA.Elements.Except(unchangedListA).ToList();
            var pageElementsB = pageB.Elements.Except(unchangedListB).ToList();

            // Find corresponding elements by ID
            var changeList = new List<ElementInfo>();
            var matchedListA = new List<ElementInfo>();
            var matchedListB = new List<ElementInfo>();
            foreach (var eB in pageElementsB)
            {
                foreach (var eA in pageElementsA)
                {
                    string idB;
                    string idA;
                    if (!eB.Attributes.TryGetValue("id", out idB)) continue;
                    if (!eA.Attributes.TryGetValue("id", out idA)) continue;
                    if (idA != idB) continue;

                    changeList.Add(eB);
                    eB.CorrespondingElement = eA;
                    eA.CorrespondingElement = eB;
                    matchedListA.Add(eA);
                    matchedListB.Add(eB);
                    break;
                }
            }

            pageElementsA = pageElementsA.Except(matchedListA).ToList();
            pageElementsB = pageElementsB.Except(matchedListB).ToList();

            // If no similar candidates are found, then assume addition
            var addList = new List<ElementInfo>();
            foreach (var eB in pageElementsB)
            {
                var match = MostLikelyCandidate(eB, pageElementsA, diffEngine);
                if (match != null)
                {
                    changeList.Add(eB);
                    eB.CorrespondingElement = match;
                    match.CorrespondingElement = eB;
                    pageElementsA.Remove(match);
                }
                else
                {
                    addList.Add(eB);
                }
            }

            changed = changeList.ToArray();
            added = addList.ToArray();
            deleted = pageElementsA.ToArray();
            unchanged = unchangedListA.ToArray();
        }
        
        /// <summary>
        /// Determines the element most likely to correspond to the target element.
        /// Returns null if no match was found.
        /// </summary>
        /// <param name="targetElement">The target element.</param>
        /// <param name="candidateElements">The candidate elements.</param>
        /// <param name="diffEngine">The levenshtein difference engine.</param>
        /// <returns>An ElementInfo object.</returns>
        private ElementInfo MostLikelyCandidate(ElementInfo targetElement, IEnumerable<ElementInfo> candidateElements, diff_match_patch diffEngine)
        {
            /*  - Get elements with similar tag names
             *      - Order by text similarity, then attributes, then tag name, then distance, and then area
             */
            ElementInfo[] candidates = GetElementsBySimilarTag(targetElement.Tag, candidateElements).ToArray();

            var targetElementTextLength = targetElement.Text.Length;

            candidates = (from candidate in candidates
                         let maxLength = Math.Max(candidate.Text.Length, targetElementTextLength)
                         let textDistance = maxLength > 0 ? (TextDistance(candidate.Text, targetElement.Text, diffEngine) / (decimal)maxLength) * 100 : 0
                         let attributeDistance = AttributesSimilarity(targetElement, candidate, diffEngine)
                         where textDistance <= 15 && attributeDistance <= 40
                         orderby textDistance, attributeDistance, candidate.Tag.Equals(targetElement.Tag), 
                            RectangleUtil.DistanceBetweenRectangles(candidate.Location, targetElement.Location),
                            RectangleUtil.AreaDifferenceBetweenRectangles(candidate.Location, targetElement.Location)
                         select candidate).ToArray();

            return candidates.FirstOrDefault();
        }
    }
}