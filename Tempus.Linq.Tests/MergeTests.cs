namespace Tempus.Linq.Tests
{
    using Shouldly;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class MergeTests
    {
        public DiffResultEditable<Item<int>, Item<int>> DiffOfSameType()
        {
            var onlyLeft = new[] { new Item<int>(1), new Item<int>(2) };
            var both = new[]
            {
                new DiffResultBoth<Item<int>, Item<int>>(new Item<int>(3), new Item<int>(3)),
                new DiffResultBoth<Item<int>, Item<int>>(new Item<int>(4), new Item<int>(4))
            };
            var onlyRight = new[] { new Item<int>(5), new Item<int>(6) };

            var left = onlyLeft.Concat(both.Select(b => b.Left)).ToList();
            var right = onlyRight.Concat(both.Select(b => b.Right)).ToList();

            return new DiffResultEditable<Item<int>, Item<int>>(left, right, onlyLeft, both, onlyRight);
        }

        public DiffResultEditable<Item<int>, Item<string>> DiffOfDifferentTypes()
        {
            var onlyLeft = new[] { new Item<int>(1), new Item<int>(2) };
            var both = new[]
            {
                new DiffResultBoth<Item<int>, Item<string>>(new Item<int>(3), new Item<string>("3")),
                new DiffResultBoth<Item<int>, Item<string>>(new Item<int>(4), new Item<string>("4"))
            };
            var onlyRight = new[] { new Item<string>("5"), new Item<string>("6") };

            var left = onlyLeft.Concat(both.Select(b => b.Left)).ToList();
            var right = onlyRight.Concat(both.Select(b => b.Right)).ToList();

            return new DiffResultEditable<Item<int>, Item<string>>(left, right, onlyLeft, both, onlyRight);
        }

        [Fact]
        public void Merge_Additions_Of_Same_Type()
        {
            var diff = DiffOfSameType();

            diff.MergeAdditions();

            var result = diff.SourceLeft.Select(a => a.Value);

            result.ShouldBe(new[] { 1, 2, 3, 4, 5, 6 });
        }

        [Fact]
        public void Merge_Additions_Of_Different_Types()
        {
            var diff = DiffOfDifferentTypes();

            Func<Item<string>, Item<int>> factory = right =>
            {
                var i = int.Parse(right.Value);
                return new Item<int>(i);
            };

            diff.MergeAdditions(factory);

            var result = diff.SourceLeft.Select(a => a.Value);

            result.ShouldBe(new[] { 1, 2, 3, 4, 5, 6 });
        }

        [Fact]
        public void Merge_Deletions_Of_Same_Type()
        {
            var diff = DiffOfSameType();

            diff.MergeDeletions();

            var result = diff.SourceLeft.Select(a => a.Value);

            result.ShouldBe(new[] { 3, 4 });
        }

        [Fact]
        public void Merge_Deletions_Of_Different_Types()
        {
            var diff = DiffOfDifferentTypes();

            diff.MergeDeletions();

            var result = diff.SourceLeft.Select(a => a.Value);

            result.ShouldBe(new[] { 3, 4 });
        }

        [Fact]
        public void Merge_Changes_And_Keep_Left_Value_Reference()
        {
            var diff = DiffOfSameType();

            diff.MergeChanges((left, right) =>
            {
                left.Value *= 10;
            });

            var result = diff.SourceLeft.Select(a => a.Value);

            result.ShouldBe(new[] { 1, 2, 30, 40 });
        }

        [Fact]
        public void Merge_Changes_And_Change_Left_Value_Reference()
        {
            var diff = DiffOfDifferentTypes();

            diff.MergeChanges((left, right) =>
            {
                return new Item<int>(left.Value * 100);
            });

            var result = diff.SourceLeft.Select(a => a.Value);

            result.ShouldBe(new[] { 1, 2, 300, 400 });
        }

        [Fact]
        public void Merge_All_Of_Same_Type()
        {
            var diff = DiffOfSameType();

            diff.MergeAll();

            var result = diff.SourceLeft.Select(a => a.Value);

            result.ShouldBe(new[] { 3, 4, 5, 6 });
        }

        [Fact]
        public void Merge_All_Of_Different_Types()
        {
            var diff = DiffOfDifferentTypes();

            diff.MergeAll((left, right) =>
            {
                left.Value = int.Parse(right.Value) * 10;
            });

            var result = diff.SourceLeft.Select(a => a.Value);

            result.ShouldBe(new[] { 30, 40, 50, 60 });
        }

        public class Item<T>
        {
            public Item()
            {

            }

            public Item(T value)
            {
                Value = value;
            }

            public T Value { get; set; }           
        }
    }
}
