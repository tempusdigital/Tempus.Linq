namespace Tempus.Linq.Tests
{
    using Shouldly;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class DiffTests
    {
        [Fact]
        public void Only_Left()
        {
            var left = new[] { 1, 2, 3 };
            var right = new int[] { };

            var result = left.Diff(right);

            result.ShouldNotBeNull();
            result.OnlyLeft.ShouldBe(left);
            result.Both.ShouldBeEmpty();
            result.OnlyRight.ShouldBeEmpty();
        }

        [Fact]
        public void Only_Right()
        {
            var left = new int[] { };
            var right = new[] { 1, 2, 3 };

            var result = left.Diff(right);

            result.ShouldNotBeNull();
            result.OnlyLeft.ShouldBeEmpty();
            result.Both.ShouldBeEmpty();
            result.OnlyRight.ShouldBe(right);
        }

        [Theory]
        [InlineData(new[] { 1, 2, 3, 4 }, new[] { 1, 2, 3, 4 })]
        [InlineData(new[] { 1, 2, 3, 4 }, new[] { 2, 4, 3, 1 })]
        public void Only_Both(IEnumerable<int> left, IEnumerable<int> right)
        {
            var result = left.Diff(right);

            result.ShouldNotBeNull();
            result.OnlyLeft.ShouldBeEmpty();
            result.Both.Select(b => b.Left).ShouldBe(left);
            result.Both.Select(b => b.Right).ShouldBe(left);
            result.OnlyRight.ShouldBeEmpty();
        }

        [Theory]
        [InlineData(new[] { 1, 2, 3, 4 }, new[] { 3, 4, 5, 6 })]
        [InlineData(new[] { 1, 2, 3, 4 }, new[] { 6, 4, 3, 5 })]
        public void Left_And_Right_And_Both(IEnumerable<int> left, IEnumerable<int> right)
        {
            var result = left.Diff(right);

            result.ShouldNotBeNull();
            result.OnlyLeft.ShouldBe(new[] { 1, 2 });
            result.Both.Select(b => b.Left).ShouldBe(new[] { 3, 4 });
            result.Both.Select(b => b.Right).ShouldBe(new[] { 3, 4 });
            result.OnlyRight.OrderBy(b => b).ShouldBe(new[] { 5, 6 });
        }

        [Fact]
        public void Lists_Of_Different_Types()
        {
            var left = new[] { 1, 2, 3, 4 };
            var right = new[] { "3", "4", "5", "6" };

            var result = left.Diff(right, e => e, d => int.Parse(d));

            result.ShouldNotBeNull();
            result.OnlyLeft.ShouldBe(new[] { 1, 2 });
            result.Both.Select(b => b.Left).ShouldBe(new[] { 3, 4 });
            result.Both.Select(b => b.Right).ShouldBe(new[] { "3", "4" });
            result.OnlyRight.ShouldBe(new[] { "5", "6" });
        }

        [Fact]
        public void Lists_Of_Different_Types_With_Complex_Key()
        {
            var left = new[] { 1, 2, 3, 4 };
            var right = new[] { "3", "4", "5", "6" };

            var result = left.Diff(right,
                e => new { Key1 = e, Key2 = e * 10 },
                d => new { Key1 = int.Parse(d), Key2 = int.Parse(d) * 10 });

            result.ShouldNotBeNull();
            result.OnlyLeft.ShouldBe(new[] { 1, 2 });
            result.Both.Select(b => b.Left).ShouldBe(new[] { 3, 4 });
            result.Both.Select(b => b.Right).ShouldBe(new[] { "3", "4" });
            result.OnlyRight.ShouldBe(new[] { "5", "6" });
        }

        [Fact]
        public void Override_Equal()
        {
            var left = new[] { new OverrideEquals(1), new OverrideEquals(2) };
            var right = new[] { new OverrideEquals(2), new OverrideEquals(3) };

            var result = left.Diff(right);

            result.ShouldNotBeNull();
            result.OnlyLeft.Select(l => l.Id).ShouldBe(new[] { 1 });
            result.Both.Select(l => l.Left.Id).ShouldBe(new[] { 2 });
            result.OnlyRight.Select(l => l.Id).ShouldBe(new[] { 3 });
        }

        class OverrideEquals
        {
            public OverrideEquals(int id)
            {
                Id = id;
            }

            public int Id { get; }

            public override bool Equals(object obj)
            {
                return Id == ((OverrideEquals)obj).Id;
            }

            public override int GetHashCode()
            {
                return Id.GetHashCode();
            }
        }

    }
}
