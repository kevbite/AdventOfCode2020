using System;
using FluentAssertions;
using Xunit;

namespace Day12.Tests
{
    public class NavigatorTests
    {
        [Fact]
        public void Part1()
        {
            Navigator.GetEndPositionPart1(@"F10
N3
F7
R90
F11").Should().Be((17, 8));
        }
        
        [Fact]
        public void Part2()
        {
            Navigator.GetEndPositionPart2(@"F10
N3
F7
R90
F11").Should().Be((214, -72));
        }
    }

}