namespace Tempus.Linq
{
	using System.Collections.Generic;

    /// <summary>
    /// Result of the comparison of two sequences. 
    /// </summary>
    /// <typeparam name="TLeft">The type of the elements of the first sequence.</typeparam>
    /// <typeparam name="TRight">The type of the elements of the second sequence.</typeparam>
    public class DiffResult<TLeft, TRight>
    {
        public DiffResult(
            IEnumerable<TLeft> onlyLeft,
            IEnumerable<DiffResultBoth<TLeft, TRight>> both,
            IEnumerable<TRight> onlyRight)
        {
            OnlyLeft = onlyLeft;
            Both = both;
            OnlyRight = onlyRight;
        }

        /// <summary>
        /// The elements present only on the first sequence.
        /// </summary>
        public IEnumerable<TLeft> OnlyLeft { get; }

        /// <summary>
        /// The elements present on both sequences.
        /// </summary>
        public IEnumerable<DiffResultBoth<TLeft, TRight>> Both { get; }

        /// <summary>
        /// The elements present only on the second sequence.
        /// </summary>
        public IEnumerable<TRight> OnlyRight { get; }
    }

    /// <summary>
    /// Result of elements present on both sequences.
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    public class DiffResultBoth<TLeft, TRight>
    {
        public DiffResultBoth(TLeft left, TRight right)
        {
            Left = left;
            Right = right;
        }

        /// <summary>
        /// The element of the first sequence.
        /// </summary>
        public TLeft Left { get; }

        /// <summary>
        /// Tle element of the second sequence.
        /// </summary>
        public TRight Right { get; }
    }

    /// <summary>
    /// Result of the comparison of two sequences. Support merge operations.
    /// </summary>
    /// <typeparam name="TLeft">The type of the elements of the first sequence.</typeparam>
    /// <typeparam name="TRight">The type of the elements of the second sequence.</typeparam>
    public class DiffResultEditable<TLeft, TRight> : DiffResult<TLeft, TRight>
    {
        public DiffResultEditable(
            ICollection<TLeft> sourceLeft,
            IEnumerable<TRight> sourceRight,
            IEnumerable<TLeft> onlyLeft,
            IEnumerable<DiffResultBoth<TLeft, TRight>> both,
            IEnumerable<TRight> onlyRight)
            : base(onlyLeft, both, onlyRight)
        {
            SourceLeft = sourceLeft;
            SourceRight = SourceRight;
        }

        /// <summary>
        /// The first sequence compared.
        /// </summary>
        public ICollection<TLeft> SourceLeft { get; }

        /// <summary>
        /// The second sequence compared.
        /// </summary>
        public IEnumerable<TLeft> SourceRight { get; }
    }
}
