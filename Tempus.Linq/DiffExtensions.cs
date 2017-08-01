namespace Tempus.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public static class DiffExtensions
    {
        /// <summary>
        /// Compares the elements of two sequences and returns the differences. The default
        /// equality comparer is used to compare elements.
        /// </summary>
        /// <typeparam name="T">Type of elements of the sequences.</typeparam>
        /// <param name="left">First sequence to compare.</param>
        /// <param name="right">Second sequence to compare.</param>
        /// <returns>The differences.</returns>
        public static DiffResult<T, T> Diff<T>(this IEnumerable<T> left, IEnumerable<T> right)
        {
            return Diff(left, right, l => l, r => r);
        }

        /// <summary>
        /// Compares the elements of two sequences and returns the differences. The default
        /// equality comparer is used to compare elements. Support merge operations.
        /// </summary>
        /// <typeparam name="T">Type of elements of the sequences.</typeparam>
        /// <param name="left">The first sequence to compare.</param>
        /// <param name="right">The second sequence to compare.</param>
        /// <returns>The differences.</returns>
        public static DiffResultEditable<T, T> Diff<T>(this ICollection<T> left, IEnumerable<T> right)
        {
            return Diff(left, right, l => l, r => r);
        }

        /// <summary>
        /// Compares the elements of two sequences based on matching keys and returns the differences. The default
        /// equality comparer is used to compare keys.
        /// </summary>
        /// <typeparam name="TLeft">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TRight">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <param name="left">The first sequence to compare.</param>
        /// <param name="right">The second sequence to compare.</param>
        /// <param name="leftKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="rightKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <returns>The differences.</returns>
        public static DiffResult<TLeft, TRight> Diff<TLeft, TRight, TKey>(this IEnumerable<TLeft> left, IEnumerable<TRight> right, Func<TLeft, TKey> leftKeySelector, Func<TRight, TKey> rightKeySelector)
        {
            return Diff(left.ToList(), right, leftKeySelector, rightKeySelector);
        }

        /// <summary>
        /// Compares the elements of two sequences based on matching keys and returns the differences. The default
        /// equality comparer is used to compare keys. Support merge operations.
        /// </summary>
        /// <typeparam name="TLeft">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TRight">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <param name="left">The first sequence to compare.</param>
        /// <param name="right">The second sequence to compare.</param>
        /// <param name="leftKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="rightKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <returns>The differences.</returns>
        public static DiffResultEditable<TLeft, TRight> Diff<TLeft, TRight, TKey>(this ICollection<TLeft> left, IEnumerable<TRight> right, Func<TLeft, TKey> leftKeySelector, Func<TRight, TKey> rightKeySelector)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));

            if (right == null)
                throw new ArgumentNullException(nameof(right));

            if (leftKeySelector == null)
                throw new ArgumentNullException(nameof(leftKeySelector));

            if (rightKeySelector == null)
                throw new ArgumentNullException(nameof(rightKeySelector));

            var onlyLeft = new List<TLeft>();
            var both = new List<DiffResultBoth<TLeft, TRight>>();
            var onlyRight = new List<TRight>();

            if (left.Any() && right.Any())
            {
                foreach (var itemLeft in left)
                {
                    var foundLeft = false;

                    foreach (var itemRight in right)
                    {
                        if (leftKeySelector(itemLeft).Equals(rightKeySelector(itemRight)))
                        {
                            both.Add(new DiffResultBoth<TLeft, TRight>(itemLeft, itemRight));
                            foundLeft = true;
                            break;
                        }
                    }

                    if (!foundLeft)
                        onlyLeft.Add(itemLeft);
                }

                onlyRight = right.Except(both.Select(b => b.Right)).ToList();
            }
            else
            {
                onlyLeft = left.ToList();
                onlyRight = right.ToList();
            }

            return new DiffResultEditable<TLeft, TRight>(left, right, onlyLeft, both, onlyRight);
        }
    }
}
