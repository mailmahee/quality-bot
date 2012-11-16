namespace QualityBot.Util
{
    using System.Collections.Generic;
    using System.Drawing;

    public static class Iterators
    {
        /// <summary>
        /// Iterates through points in a top down direction starting at the upper left.
        /// </summary>
        /// <param name="minX">The min x.</param>
        /// <param name="minY">The min y.</param>
        /// <param name="maxX">The max x.</param>
        /// <param name="maxY">The max y.</param>
        /// <param name="stepX">The y step.</param>
        /// <param name="stepY">The x step.</param>
        /// <returns>
        /// An <c>Iterator</c>.
        /// </returns>
        public static IEnumerable<Point> TopDownIterator(int minX, int minY, int maxX, int maxY, int stepX, int stepY)
        {
            for (var x = minX; x < maxX; x += stepX)
            {
                for (var y = minY; y < maxY; y += stepY)
                {
                    yield return new Point(x, y);
                }
            }
        }
    }
}