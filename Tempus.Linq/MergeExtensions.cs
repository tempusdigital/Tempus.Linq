namespace Tempus.Linq
{
    using System;
    using System.Threading.Tasks;

    public static class MergeExtensions
    {
        /// <summary>
        /// Performs the specified action on each element present only on the second sequence. 
        /// These elements will be added to the first sequence on a merge operation.
        /// </summary>
        /// <typeparam name="TLeft">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TRight">The type of the elements of the second sequence.</typeparam>
        /// <param name="source">The result of the comparison of the two sequences. </param>
        /// <param name="action">
        /// The System.Action delegate to perform on each element present only on the second sequence. 
        /// These elements will be added to the first sequence on a merge operation.
        /// </param>
        /// <returns></returns>
        public static DiffResultEditable<TLeft, TRight> ForEachAddition<TLeft, TRight>(this DiffResultEditable<TLeft, TRight> source, Action<TRight> action)
        {
            foreach (var right in source.OnlyRight)
            {
                action(right);
            }

            return source;
        }

        /// <summary>
        /// Performs the specified action on each element present only on the second sequence. 
        /// These elements will be merge on a single element to the first sequence on a merge operation.
        /// </summary>
        /// <typeparam name="TLeft">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TRight">The type of the elements of the second sequence.</typeparam>
        /// <param name="source">The result of the comparison of the two sequences. </param>
        /// <param name="action">
        /// The System.Action delegate to perform on each element present only on the second sequence. 
        /// These elements will be merge on a single element to the first sequence on a merge operation.
        /// </param>
        /// <returns></returns>
        public static DiffResultEditable<TLeft, TRight> ForEachChange<TLeft, TRight>(this DiffResultEditable<TLeft, TRight> source, Action<TRight, TLeft> action)
        {
            foreach (var both in source.Both)
            {
                action(both.Right, both.Left);
            }

            return source;
        }

        /// <summary>
        /// Performs the specified action on each element present only on the second sequence. 
        /// These elements will be deleted from the first sequence on a merge operation.
        /// </summary>
        /// <typeparam name="TLeft">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TRight">The type of the elements of the second sequence.</typeparam>
        /// <param name="source">The result of the comparison of the two sequences.</param>
        /// <param name="action">
        /// The System.Action delegate to perform on each element present only on the second sequence. 
        /// These elements will be deleted from the first sequence on a merge operation.
        /// </param>
        /// <returns></returns>
        public static DiffResultEditable<TLeft, TRight> ForEachDeletion<TLeft, TRight>(this DiffResultEditable<TLeft, TRight> source, Action<TLeft> action)
        {
            foreach (var left in source.OnlyLeft)
            {
                action(left);
            }

            return source;
        }

        /// <summary>
        /// Add to the first sequence all the elements present only on the second sequence.
        /// </summary>
        /// <typeparam name="T">The type of elements of the sequences.</typeparam>
        /// <param name="source">The result of the comparison of the two sequences.</param>
        /// <returns></returns>
        public static DiffResultEditable<T, T> MergeAdditions<T>(this DiffResultEditable<T, T> source)
        {
            return MergeAdditions(source, r => r);
        }

        /// <summary>
        /// Add to the first sequence all the elements present only on the second sequence.
        /// </summary>
        /// <typeparam name="TLeft">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TRight">The type of the elements of the second sequence.</typeparam>
        /// <param name="source">The result of the comparison of the two sequences.</param>
        /// <param name="factory">A function to create a element to the first sequence based on a element present only on the second sequence.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Add to the first sequence all the elements present only on the second sequence.
        /// </summary>
        /// <typeparam name="TLeft">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TRight">The type of the elements of the second sequence.</typeparam>
        /// <param name="source">The result of the comparison of the two sequences.</param>
        /// <param name="factory">A function to create a element to the first sequence based on a element present only on the second sequence.</param>
        /// <returns></returns>
        public static async Task MergeAdditionsAsync<TLeft, TRight>(this DiffResultEditable<TLeft, TRight> source, Func<TRight, Task<TLeft>> factory)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            foreach (var add in source.OnlyRight)
            {
                var newItem = await factory(add);
                if (newItem != null)
                    source.SourceLeft.Add(newItem);
            }
        }

        /// <summary>
        /// Apply changes on a element of the first sequence based on the matching element on the second sequence.
        /// </summary>
        /// <typeparam name="TLeft">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TRight">The type of the elements of the second sequence.</typeparam>
        /// <param name="source">The result of the comparison of the two sequences.</param>
        /// <param name="mapChanges">A function apply changes on a element of the first sequence based on the matching element on the second sequence.</param>
        /// <returns></returns>
        public static DiffResultEditable<TLeft, TRight> MergeChanges<TLeft, TRight>(this DiffResultEditable<TLeft, TRight> source, Action<TLeft, TRight> mapChanges)
            where TLeft : class
        {
            return MergeChanges(source, (left, right) =>
            {
                mapChanges(left, right);
                return left;
            });
        }

        /// <summary>
        /// Add to the first sequence a result element from two matching elements.
        /// </summary>
        /// <typeparam name="TLeft">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TRight">The type of the elements of the second sequence.</typeparam>
        /// <param name="source">The result of the comparison of the two sequences.</param>
        /// <param name="mapChanges">A function to create a result element from two matching elements.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Apply changes on a element of the first sequence based on the matching element on the second sequence.
        /// </summary>
        /// <typeparam name="TLeft">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TRight">The type of the elements of the second sequence.</typeparam>
        /// <param name="source">The result of the comparison of the two sequences.</param>
        /// <param name="mapChanges">A function apply changes on a element of the first sequence based on the matching element on the second sequence.</param>
        /// <returns></returns>
        public static async Task MergeChangesAsync<TLeft, TRight>(this DiffResultEditable<TLeft, TRight> source, Func<TLeft, TRight, Task> mapChanges)
            where TLeft : class
        {
            await MergeChangesAsync(source, async (left, right) =>
            {
                await mapChanges(left, right);
                return left;
            });
        }

        /// <summary>
        /// Add to the first sequence a result element from two matching elements.
        /// </summary>
        /// <typeparam name="TLeft">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TRight">The type of the elements of the second sequence.</typeparam>
        /// <param name="source">The result of the comparison of the two sequences.</param>
        /// <param name="mapChanges">A function to create a result element from two matching elements.</param>
        /// <returns></returns>
        public static async Task MergeChangesAsync<TLeft, TRight>(this DiffResultEditable<TLeft, TRight> source, Func<TLeft, TRight, Task<TLeft>> mapChanges)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (mapChanges == null)
                throw new ArgumentNullException(nameof(mapChanges));

            foreach (var change in source.Both)
            {
                var newLeft = await mapChanges(change.Left, change.Right);

                if (!Object.ReferenceEquals(change.Left, newLeft))
                {
                    source.SourceLeft.Remove(change.Left);
                    source.SourceLeft.Add(newLeft);
                }
            }
        }



        /// <summary>
        /// Delete from the first sequence all the elements present only on the first sequence.
        /// </summary>
        /// <typeparam name="TLeft">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TRight">The type of the elements of the second sequence.</typeparam>
        /// <param name="source">The result of the comparison of the two sequences.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Apply the additions, changes e deletions to the first sequence.
        /// </summary>
        /// <typeparam name="T">The type of elements of the sequences.</typeparam>
        /// <param name="source"></param>
        public static void MergeAll<T>(this DiffResultEditable<T, T> source)
        {
            source
                .MergeAdditions()
                .MergeDeletions();
        }

        /// <summary>
        /// Apply the additions, changes e deletions to the first sequence.
        /// </summary>
        /// <typeparam name="TLeft">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TRight">The type of the elements of the second sequence.</typeparam>
        /// <param name="source">The result of the comparison of the two sequences.</param>
        /// <param name="mapAdditionsAndChanges">A function to map a element added or changed on the first sequence based on matching element on the second sequence or on a element present only on the second sequence.</param>
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

        /// <summary>
        /// Apply the additions, changes e deletions to the first sequence.
        /// </summary>
        /// <typeparam name="TLeft">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TRight">The type of the elements of the second sequence.</typeparam>
        /// <param name="source">The result of the comparison of the two sequences.</param>
        /// <param name="mapAdditionsAndChanges">A function to map a element added or changed on the first sequence based on matching element on the second sequence or on a element present only on the second sequence.</param>
        public static async Task MergeAllAsync<TLeft, TRight>(this DiffResultEditable<TLeft, TRight> source, Func<TLeft, TRight, Task> mapAdditionsAndChanges)
            where TLeft : class, new()
        {
            await source.MergeAdditionsAsync(async r =>
            {
                var l = new TLeft();
                await mapAdditionsAndChanges(l, r);
                return l;
            });

            await source.MergeChangesAsync(mapAdditionsAndChanges);

            source.MergeDeletions();
        }
    }
}
