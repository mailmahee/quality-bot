namespace QualityBot.Test.ExtensionMethods
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Useful extension methods.
    /// </summary>
    public static class TypeExtensions
    {
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
            foreach (var item in items)
            {
                action(item);
            }
        }
    }
}