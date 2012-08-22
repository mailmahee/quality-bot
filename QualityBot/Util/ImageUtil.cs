namespace QualityBot.Util
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;

    public static class ImageUtil
    {
        /// <summary>
        /// Saves the specified image to disc.
        /// </summary>
        /// <param name="outputDir">The target directory.</param>
        /// <param name="base64Image">The image as a base64 encoded string.</param>
        /// <param name="imageSuffix">The filename suffix.</param>
        /// <returns>The file name.</returns>
        public static string SaveImageToDisc(string outputDir, string base64Image, string imageSuffix)
        {
            var now = DateTime.Now.ToString("yyyyMMddHHmmssfffffff");
            var saveLocation = Path.Combine(outputDir, string.Format(@"{0}_{1}.png", now, imageSuffix));
            var image = Base64ToImage(base64Image);
            image.Save(saveLocation, ImageFormat.Png);
            return saveLocation;
        }

        /// <summary>
        /// Converts an image to a base 64 encoded string.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="format">The image format.</param>
        /// <returns>A base 64 encoded string.</returns>
        public static string ImageToBase64(Image image, ImageFormat format)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, format);
                var imageBytes = ms.ToArray();
                return BytesToBase64(imageBytes);
            }
        }

        /// <summary>
        /// Converts an image to a byte array.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="format">The image format.</param>
        /// <returns>A byte array.</returns>
        public static byte[] ImageToBytes(Image image, ImageFormat format)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, format);
                var imageBytes = ms.ToArray();
                return imageBytes;
            }
        }

        /// <summary>
        /// Converts a byte array to a base 64 encoded string.
        /// </summary>
        /// <param name="bytes">The byte array.</param>
        /// <returns>A base 64 encoded string.</returns>
        public static string BytesToBase64(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Converts a base 64 encoded string to a byte array.
        /// </summary>
        /// <param name="base64">The base 64 encoded image.</param>
        /// <returns>A byte array.</returns>
        public static byte[] Base64ToBytes(string base64)
        {
            var bytes = Convert.FromBase64String(base64);
            return bytes;
        }
        
        /// <summary>
        /// Converts a base 64 encoded string to an image.
        /// </summary>
        /// <param name="base64">The base 64 encoded image.</param>
        /// <returns>An image.</returns>
        public static Image Base64ToImage(string base64)
        {
            var bytes = Convert.FromBase64String(base64);
            return BytesToImage(bytes);
        }

        /// <summary>
        /// Converts a byte array to an image.
        /// </summary>
        /// <param name="imageBytes">The byte array.</param>
        /// <returns>An image.</returns>
        public static Image BytesToImage(byte[] imageBytes)
        {
            var ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            ms.Write(imageBytes, 0, imageBytes.Length);
            var image = Image.FromStream(ms, true);
            ms.Dispose();
            return image;
        }

        /// <summary>
        /// Crops the image to the specified rectangle.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="cropArea">The cropping rectangle.</param>
        /// <returns>An image.</returns>
        public static Image CropImage(Image image, Rectangle cropArea)
        {
            var bmpImage = new Bitmap(image);
            var croppedImage = bmpImage.Clone(cropArea, bmpImage.PixelFormat);
            bmpImage.Dispose();
            return croppedImage;
        }
        
        /// <summary>
        /// Compares two bitmaps pixel-by-pixel and returns a bitmap mask of the difference.
        /// </summary>
        /// <param name="bitmapA">The first bitmap.</param>
        /// <param name="bitmapB">The second bitmap.</param>
        /// <param name="ia">The image attributes.</param>
        /// <param name="percentageDifferent">The difference between the two images as a percentage.</param>
        /// <returns>A bitmap.</returns>
        public static Bitmap BitmapDiff(Bitmap bitmapA, Bitmap bitmapB, ImageAttributes ia, out decimal percentageDifferent)
        {
            var wA = bitmapA.Width;
            var wB = bitmapB.Width;
            var hA = bitmapA.Height;
            var hB = bitmapB.Height;

            var maxW = wA > wB ? wA : wB;
            var maxH = hA > hB ? hA : hB;
            var minW = wA < wB ? wA : wB;
            var minH = hA < hB ? hA : hB;

            var mask = new Bitmap(maxW, maxH);
            var fastMask = new FastBitmap(mask);
            var fastBitmapA = new FastBitmap(bitmapA);
            var fastBitmapB = new FastBitmap(bitmapB);
            fastMask.LockImage();
            fastBitmapA.LockImage();
            fastBitmapB.LockImage();

            var changed = 0;
            foreach (var pt in Iterators.TopDownIterator(0, 0, minW, minH, 1, 1).Where(pt => !IsColorMatch(fastBitmapA.GetPixel(pt.X, pt.Y), fastBitmapB.GetPixel(pt.X, pt.Y))))
            {
                changed++;
                fastMask.SetPixel(pt.X, pt.Y, Color.Red);
            }

            fastMask.UnlockImage();
            fastBitmapA.UnlockImage();
            fastBitmapB.UnlockImage();

            var uidPixels = mask;
            if (maxW != minW || maxH != minH)
            {
                var addRegion = new Region(new Rectangle(0, 0, maxW, maxH));
                addRegion.Exclude(new Rectangle(0, 0, minW, minH));
                uidPixels = new Bitmap(maxW, maxH);
                using (var g = Graphics.FromImage(uidPixels))
                {
                    g.DrawImage(mask, 0, 0);
                    g.FillRegion(new SolidBrush(Color.Red), addRegion);
                }
                addRegion.Dispose();
            }

            var img = new Bitmap(maxW, maxH);
            using (var g = Graphics.FromImage(img))
            {
                g.DrawImage(bitmapB, 0, 0);
                g.DrawImage(uidPixels, new Rectangle(0, 0, maxW, maxH), 0, 0, maxW, maxH, GraphicsUnit.Pixel, ia);
            }

            var addedDeleted = RectangleUtil.AreaChange(new Rectangle(0, 0, wA, hA), new Rectangle(0, 0, wB, hB));
            var total = addedDeleted + (minW * minH);
            var delta = changed + addedDeleted;
            percentageDifferent = total > 0 ? (delta / total) * 100 : 0;

            // Dispose resources
            uidPixels.Dispose();
            mask.Dispose();

            return img;
        }
        
        /// <summary>
        /// Determines if two colors are equivalent.
        /// </summary>
        /// <param name="color1">The first color.</param>
        /// <param name="color2">The second color.</param>
        /// <returns>True if the colors are equivalent.</returns>
        public static bool IsColorMatch(Color color1, Color color2)
        {
            return color1.B == color2.B && color1.R == color2.R && color1.G == color2.G && color1.A == color2.A;
        }
        
        /// <summary>
        /// Draws the region as green, and the inverted region as red.
        /// </summary>
        /// <param name="size">The size of the image.</param>
        /// <param name="region">The clipping region.</param>
        /// <param name="image">The image.</param>
        /// <param name="ia">The iamge attributes (such as alpha blending) to use when drawing the masks.</param>
        /// <returns>A bitmap of the image with the mask overlays.</returns>
        public static Bitmap DrawRegionAsMasks(Size size, Region region, Image image, ImageAttributes ia)
        {
            // mask
            var mask = new Bitmap(size.Width, size.Height);
            using (var g = Graphics.FromImage(mask))
            {
                g.FillRegion(new HatchBrush(HatchStyle.Cross, Color.Green, Color.White), region);
                region.Xor(new Region(new Rectangle(0, 0, size.Width, size.Height)));
                g.FillRegion(new HatchBrush(HatchStyle.BackwardDiagonal, Color.Red, Color.White), region);
            }

            // alpha blend mask to area of comparison
            var imageA = (Bitmap)image.Clone();
            using (var g = Graphics.FromImage(imageA))
            {
                g.DrawImage(mask, new Rectangle(0, 0, size.Width, size.Height), 0, 0, size.Width, size.Height, GraphicsUnit.Pixel, ia);
            }

            mask.Dispose();

            return imageA;
        }

        /// <summary>
        /// Clips an image to the specified region.
        /// </summary>
        /// <param name="size">The size of the image.</param>
        /// <param name="image">The image.</param>
        /// <param name="region">The region to clip.</param>
        /// <returns>A bitmap.</returns>
        public static Bitmap GetClippedImage(Size size, Image image, Region region)
        {
            var clippedImage = new Bitmap(size.Width, size.Height);
            using (var g = Graphics.FromImage(clippedImage))
            {
                g.Clip = region;
                g.DrawImage(image, 0, 0);
            }

            return clippedImage;
        }

        /// <summary>
        /// Calculates the region of the rectangle that is not overlapped by siblings or children.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="potentialIntersections">An array of potentially intersecting rectangles.</param>
        /// <returns>A region.</returns>
        public static Region GetClippedRegion(Rectangle rectangle, IEnumerable<Rectangle> potentialIntersections)
        {
            var region = new Region(rectangle);

            // Exclude children and intersecting siblings
            RectangleUtil.ChildrenAndIntersectingSiblings(rectangle, potentialIntersections).ForEach(region.Exclude);

            // Set X and Y to 0
            region.Translate(-rectangle.Left, -rectangle.Top);

            return region;
        }
    }
}