namespace Tempus.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class MergeExtensions
    {
        public static DiffResultEditable<TLeft, TRight> ForAdditions<TLeft, TRight>(this DiffResultEditable<TLeft, TRight> source, Action<TRight> action)
        {
            foreach (var right in source.OnlyRight)
            {
                action(right);
            }

            return source;
        }

        public static DiffResultEditable<TLeft, TRight> ForChanges<TLeft, TRight>(this DiffResultEditable<TLeft, TRight> source, Action<TRight, TLeft> action)
        {
            foreach (var both in source.Both)
            {
                action(both.Right, both.Left);
            }

            return source;
        }

        public static DiffResultEditable<TLeft, TRight> ForDeletions<TLeft, TRight>(this DiffResultEditable<TLeft, TRight> source, Action<TLeft> action)
        {
            foreach (var left in source.OnlyLeft)
            {
                action(left);
            }

            return source;
        }

        public static DiffResultEditable<T, T> MergeAdditions<T>(this DiffResultEditable<T, T> source)
        {
            return MergeAdditions(source, r => r);
        }

        public static DiffResultEditable<TLeft, TRight> MergeAdditions<TLeft, TRight>(this DiffResultEditable<TLeft, TRight> source, Func<TRight, TLeft> factory)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            foreach (var add in source.OnlyRight)
            {
                var newItem = factory(add);
                if (newItem != null)
                    source.SourceLeft.Add(newItem);
            }

            return source;
        }

        public static DiffResultEditable<TLeft, TRight> MergeChanges<TLeft, TRight>(this DiffResultEditable<TLeft, TRight> source, Action<TLeft, TRight> mapChanges)
        {
            return MergeChanges(source, (left, right) =>
            {
                mapChanges(left, right);
                return left;
            });
        }

        public static DiffResultEditable<TLeft, TRight> MergeChanges<TLeft, TRight>(this DiffResultEditable<TLeft, TRight> source, Func<TLeft, TRight, TLeft> mapChanges)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (mapChanges == null)
                throw new ArgumentNullException(nameof(mapChanges));

            foreach (var change in source.Both)
            {
                var newLeft = mapChanges(change.Left, change.Right);

                if (!Object.ReferenceEquals(change.Left, newLeft))
                {
                    source.SourceLeft.Remove(change.Left);
                    source.SourceLeft.Add(newLeft);
                }
            }

            return source;
        }

        public static DiffResultEditable<TLeft, TRight> MergeDeletions<TLeft, TRight>(this DiffResultEditable<TLeft, TRight> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            foreach (var delete in source.OnlyLeft)
            {
                source.SourceLeft.Remove(delete);
            }

            return source;
        }

        public static void MergeAll<T>(this DiffResultEditable<T, T> source)
        {
            source
                .MergeAdditions()
                .MergeDeletions();
        }

        public static void MergeAll<TLeft, TRight>(this DiffResultEditable<TLeft, TRight> source, Action<TLeft, TRight> mapAdditionsAndChanges)
            where TLeft : class, new()
        {
            source
                .MergeAdditions(r =>
                {
                    var l = new TLeft();
                    mapAdditionsAndChanges(l, r);
                    return l;
                })
                .MergeChanges(mapAdditionsAndChanges)
                .MergeDeletions();
        }
    }
}
