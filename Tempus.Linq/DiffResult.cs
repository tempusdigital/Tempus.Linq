namespace Tempus.Linq
{
	using System.Collections.Generic;

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

        public IEnumerable<TLeft> OnlyLeft { get; }

        public IEnumerable<DiffResultBoth<TLeft, TRight>> Both { get; }

        public IEnumerable<TRight> OnlyRight { get; }
    }

    public class DiffResultBoth<TLeft, TRight>
    {
        public DiffResultBoth(TLeft left, TRight right)
        {
            Left = left;
            Right = right;
        }

        public TLeft Left { get; }

        public TRight Right { get; }
    }

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

        public ICollection<TLeft> SourceLeft { get; }

        public IEnumerable<TLeft> SourceRight { get; }
    }
}
