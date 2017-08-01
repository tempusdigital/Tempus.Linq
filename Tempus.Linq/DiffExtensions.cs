namespace Tempus.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public static class DiffExtensions
    {
        public static DiffResult<T, T> Diff<T>(this IEnumerable<T> left, IEnumerable<T> right)
        {
            return Diff(left, right, l => l, r => r);
        }

        public static DiffResultEditable<T, T> Diff<T>(this ICollection<T> left, IEnumerable<T> right)
        {
            return Diff(left, right, l => l, r => r);
        }

        public static DiffResult<TLeft, TRight> Diff<TLeft, TRight, TKey>(this IEnumerable<TLeft> left, IEnumerable<TRight> right, Func<TLeft, TKey> leftKeySelector, Func<TRight, TKey> rightKeySelector)
        {
            return Diff(left.ToList(), right, leftKeySelector, rightKeySelector);
        }

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
