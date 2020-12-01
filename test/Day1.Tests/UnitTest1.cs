using System;
using FluentAssertions;
using Xunit;

namespace Day1.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Part1()
        {
            var rows = new[]
            {
                1721,
                979,
                366,
                299,
                675,
                1456
            };

            ExpenseReport.FindSum(rows, 2020)
                .Should().BeEquivalentTo(new[] {1721, 299});
        }
        
        [Fact]
        public void Part2()
        {
            var rows = new[]
            {
                1721,
                979,
                366,
                299,
                675,
                1456
            };

            ExpenseReport.FindSum2(rows, 2020)
                .Should().BeEquivalentTo(new[] {979, 366, 675});
        }
    }
}