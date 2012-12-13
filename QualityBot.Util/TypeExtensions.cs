namespace QualityBot.Util
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    /// <summary>
    /// Useful extension methods.
    /// </summary>
    public static class TypeExtensions
    {
        public static decimal GetPercentageOfTotal(this decimal total, decimal value)
        {
            var percentage = total > 0 ? (value / total) * 100 : 0M;

            return percentage;
        }

        public static decimal GetPercentageOfTotal(this int total, decimal value)
        {
            return GetPercentageOfTotal((decimal)total, value);
        }

        /// <summary>
        /// Returns true if this rectangle encompasses r2.
        /// </summary>
        /// <param name="r1">First rectangle.</param>
        /// <param name="r2">Second rectangle.</param>
        /// <returns></returns>
        public static bool Encompasses(this Rectangle r1, Rectangle r2)
        {
            return (r1.Right >= r2.Right) && (r1.Bottom >= r2.Bottom) && (r1.Left <= r2.Left) && (r1.Top <= r2.Top);
        }

        /// <summary>
        /// Calculates the area of the rectangle.
        /// </summary>
        /// <param name="r1">The rectangle.</param>
        /// <returns>A long.</returns>
        public static long Area(this Rectangle r1)
        {
            return r1.Width * r1.Height;
        }

        /// <summary>
        /// Determines if this rectangle intersects the other.
        /// </summary>
        /// <param name="r1">The first rectangle.</param>
        /// <param name="r2">The second rectangle.</param>
        /// <returns>True if the rectangles intersect each other.</returns>
        public static bool HitTest(this Rectangle r1, Rectangle r2)
        {
            return (r1.Right >= r2.Left) && (r1.Left <= r2.Right) && (r1.Bottom >= r2.Top) && (r1.Top <= r2.Bottom);
        }

        /// <summary>
        /// Determines if the value is found in the list.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="list">
        /// The list.
        /// </param>
        /// <typeparam name="T">
        /// The type.
        /// </typeparam>
        /// <returns>
        /// True if the value is found in the list.
        /// </returns>
        public static bool In<T>(this T source, params T[] list)
        {
            return list.Contains(source);
        }

        /// <summary>
        /// Executes the specified action on the given items.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="items">
        /// The items.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items.ToArray())
            {
                action(item);
            }
        }

        /// <summary>
        /// Compares two dictionaries.
        /// </summary>
        /// <typeparam name="TKey">The key.</typeparam>
        /// <typeparam name="TValue">The value.</typeparam>
        /// <param name="first">The first dictionary.</param>
        /// <param name="second">The second dictionary.</param>
        /// <returns>True if the dictionaries are equal, false if they are not.</returns>
        public static bool DictionaryEqual<TKey, TValue>(this IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second)
        {
            if (first == second) return true;
            if ((first == null) || (second == null)) return false;
            if (first.Count != second.Count) return false;

            var comparer = EqualityComparer<TValue>.Default;

            foreach (var kvp in first)
            {
                TValue secondValue;
                if (!second.TryGetValue(kvp.Key, out secondValue)) return false;
                if (!comparer.Equals(kvp.Value, secondValue)) return false;
            }

            return true;
        }

        public static string QualifyUrl(this string value)
        {
            if (value.StartsWith("www."))
            {
                value = "http://" + value;
            }
            else if (!value.StartsWith("http://"))
            {
                value = "http://www." + value;
            }

            return value;
        }
    }
}
