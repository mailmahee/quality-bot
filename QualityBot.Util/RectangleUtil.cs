namespace QualityBot.Util
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    public static class RectangleUtil
    {
        /// <summary>
        /// Returns true if the x, y, width, and height of the rectangle are zero.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns>True or false.</returns>
        public static bool AllZeroes(Rectangle rectangle)
        {
            return rectangle.Width == 0 && rectangle.Height == 0 && rectangle.X == 0 && rectangle.Y == 0;
        }

        /// <summary>
        /// Returns true if the x and y of the rectangle are greater than or equal to zero.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns>True or false.</returns>
        public static bool PositiveOrZeroCoordinates(Rectangle rectangle)
        {
            return rectangle.X >= 0 && rectangle.Y >= 0;
        }

        /// <summary>
        /// Returns true if the width and height of the rectangle are greater than zero.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns>True or false.</returns>
        public static bool PositiveDimensions(Rectangle rectangle)
        {
            return rectangle.Width > 0 && rectangle.Height > 0;
        }

        /// <summary>
        /// Calculates the area difference between two rectangles.
        /// </summary>
        /// <param name="r1">The first rectangle.</param>
        /// <param name="r2">The second rectangle.</param>
        /// <returns>A long value.</returns>
        public static long AreaDifferenceBetweenRectangles(Rectangle r1, Rectangle r2)
        {
            return Math.Abs(r1.Area() - r2.Area());
        }

        /// <summary>
        /// Calculates the distance between two rectangles.
        /// </summary>
        /// <param name="r1">The first rectangle.</param>
        /// <param name="r2">The second rectangle.</param>
        /// <returns>A floating point value.</returns>
        public static double DistanceBetweenRectangles(Rectangle r1, Rectangle r2)
        {
            return Math.Sqrt(Math.Pow(Math.Abs(r1.X - r2.X), 2) + Math.Pow(Math.Abs(r1.Y - r2.Y), 2));
        }

        /// <summary>
        /// Determines the area change from the first rectangle to the second.
        /// </summary>
        /// <param name="r1">The first rectangle.</param>
        /// <param name="r2">The second rectangle.</param>
        /// <returns>A percentage indicating area change.</returns>
        public static decimal AreaChangeAsPercent(Rectangle r1, Rectangle r2)
        {
            var minWidth = Math.Min(r1.Width, r2.Width);
            var minHeight = Math.Min(r1.Height, r2.Height);
            var maxWidth = Math.Max(r1.Width, r2.Width);
            var maxHeight = Math.Max(r1.Height, r2.Height);
            var startingArea = r1.Area();

            long areaDelta = 0;
            if (r1.Width != r2.Width) areaDelta += ((maxWidth - minWidth) * minHeight);
            if (r1.Height != r2.Height) areaDelta += ((maxHeight - minHeight) * minWidth);
            if ((r1.Height > r2.Height && r1.Width > r2.Width) ||
                (r2.Height > r1.Height && r2.Width > r1.Width)) areaDelta += ((maxHeight - minHeight) * (maxWidth - minWidth));

            return areaDelta == 0 ? 0 : ((decimal)areaDelta / startingArea) * 100;
        }

        /// <summary>
        /// Determines the area change from the first rectangle to the second.
        /// </summary>
        /// <param name="r1">The first rectangle.</param>
        /// <param name="r2">The second rectangle.</param>
        /// <returns>A percentage indicating area change.</returns>
        public static decimal AreaChange(Rectangle r1, Rectangle r2)
        {
            var minWidth = Math.Min(r1.Width, r2.Width);
            var minHeight = Math.Min(r1.Height, r2.Height);
            var maxWidth = Math.Max(r1.Width, r2.Width);
            var maxHeight = Math.Max(r1.Height, r2.Height);

            long areaDelta = 0;
            if (r1.Width != r2.Width) areaDelta += ((maxWidth - minWidth) * minHeight);
            if (r1.Height != r2.Height) areaDelta += ((maxHeight - minHeight) * minWidth);
            if ((r1.Height > r2.Height && r1.Width > r2.Width) ||
                (r2.Height > r1.Height && r2.Width > r1.Width)) areaDelta += ((maxHeight - minHeight) * (maxWidth - minWidth));

            return areaDelta;
        }

        /// <summary>
        /// Returns all rectangles that intersect with the target rectangle.
        /// </summary>
        /// <param name="rectangle">The target rectangle.</param>
        /// <param name="rectangles">The candidate rectangles.</param>
        /// <returns>A collection of rectangles.</returns>
        public static IEnumerable<Rectangle> ChildrenAndIntersectingSiblings(Rectangle rectangle, IEnumerable<Rectangle> rectangles)
        {
            return rectangles.Where(r => rectangle.HitTest(r) && !rectangle.Equals(r) && !r.Encompasses(rectangle));
        }
    }
}
