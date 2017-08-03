namespace Tempus.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class EnumerableExtensions
    {
        /// <summary>
        /// Returns the increment on the max value of a property of a sequence
        /// </summary>
        /// <typeparam name="T">The type of every element on the sequence</typeparam>
        /// <param name="source">The sequence</param>
        /// <param name="property">The property to get the max value</param>
        /// <param name="start">The minimum value to return. Default 1</param>
        /// <param name="increment">The value to increment on the max value of the list. Default 1</param>
        /// <returns></returns>
        public static int Next<T>(this IEnumerable<T> source, Func<T, int> property, int start = 1, int increment = 1)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (property == null)
                throw new ArgumentNullException(nameof(property));

            var items = source.Select(property).Where(i => i >= start);

            if (items.IsEmpty())
                return start;

            return items.Max() + increment;
        }

        /// <summary>
        /// Returns the increment on the max value a sequence
        /// </summary>
        /// <param name="source">The sequence</param>
        /// <param name="start">The minimum value to return. Default 1</param>
        /// <param name="increment">The value to increment on the max value of the list. Default 1</param>
        /// <returns></returns>
        public static int Next(this IEnumerable<int> source, int start = 1, int increment = 1)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.Next(i => i, start, increment);
        }

        /// <summary>
        /// Returns the increment on the max value of a property of a sequence
        /// </summary>
        /// <typeparam name="T">The type of every element on the sequence</typeparam>
        /// <param name="source">The sequence</param>
        /// <param name="property">The property to get the max value</param>
        /// <param name="start">The minimum value to return. Default 1</param>
        /// <param name="increment">The value to increment on the max value of the list. Default 1</param>
        /// <returns></returns>
        public static int Next<T>(this IEnumerable<T> source, Func<T, int?> property, int start = 1, int increment = 1)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (property == null)
                throw new ArgumentNullException(nameof(property));

            return source.Next(p => property(p) ?? start, start, increment);
        }

        /// <summary>
        /// Returns the increment on the max value a sequence
        /// </summary>
        /// <param name="source">The sequence</param>
        /// <param name="start">The minimum value to return. Default 1</param>
        /// <param name="increment">The value to increment on the max value of the list. Default 1</param>
        /// <returns></returns>
        public static int Next(this IEnumerable<int?> source, int start = 1, int increment = 1)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.Next(i => i ?? start, start, increment);
        }

        /// <summary>
        /// Checks if a value is present on a sequence
        /// </summary>
        /// <typeparam name="T">The type of the elements of the sequence</typeparam>
        /// <param name="source">The value to find for</param>
        /// <param name="items">The sequence to check</param>
        /// <returns>Returns true if the element is present on the sequence</returns>
        public static bool In<T>(this T source, IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            return items.Contains(source);
        }

        /// <summary>
        /// Checks if a value is present on a sequence
        /// </summary>
        /// <typeparam name="T">The type of the elements of the sequence</typeparam>
        /// <param name="source">The value to find for</param>
        /// <param name="items">The sequence to check</param>
        /// <returns>Returns true if the element is present on the sequence</returns>
        public static bool In<T>(this T source, params T[] items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            return items.Contains(source);
        }

        /// <summary>
        /// Checks if the sequence is null or empty
        /// </summary>
        /// <typeparam name="T">The type of the elements of the sequence</typeparam>
        /// <param name="source">The sequence</param>
        /// <returns>Returns true when the sequence is null or don't have any element</returns>
        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

        /// <summary>
        /// Checks if the sequence has element
        /// </summary>
        /// <typeparam name="T">The type of the elements of the sequence</typeparam>
        /// <param name="source">The sequence</param>
        /// <returns>Return true if the sequence is not null and has elements</returns>
        public static bool IsNotEmpty<T>(this IEnumerable<T> source)
        {
            return !source.IsEmpty();
        }
    }
}
