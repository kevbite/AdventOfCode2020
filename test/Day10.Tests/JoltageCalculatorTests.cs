using System;
using FluentAssertions;
using Xunit;

namespace Day10.Tests
{
    public class JoltageCalculatorTests
    {
        [Fact]
        public void Part1()
        {
            var input = @"16
10
15
5
1
11
7
19
6
12
4";
            JoltageCalculator.Part1(input)
                .Should().Be((7, 5));
        }
    }
}