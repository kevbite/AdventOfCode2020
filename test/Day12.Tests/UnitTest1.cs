using System;
using FluentAssertions;
using Xunit;

namespace Day12.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Part1()
        {
            Navigator.GetEndPosition(@"F10
N3
F7
R90
F11").Should().Be((17, 8));
        }
    }
}