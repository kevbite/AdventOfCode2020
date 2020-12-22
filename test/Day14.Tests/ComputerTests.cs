using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Day14.Tests
{
    public class ComputerTests
    {
        [Fact]
        public void Part1()
        {
            var computer = ComputerPart1.Initialize(@"mask = XXXXXXXXXXXXXXXXXXXXXXXXXXXXX1XXXX0X
mem[8] = 11
mem[7] = 101
mem[8] = 0");

            computer.Memory[7].Should().Be(101);
            computer.Memory[8].Should().Be(64);
        }
        
        [Fact]
        public void Part2Line1()
        {
            var computer = ComputerPart2.Initialize(@"mask = 000000000000000000000000000000X1001X
mem[42] = 100");

            computer.Memory.Should().BeEquivalentTo(
                new Dictionary<long, long>
                {
                    [26] = 100,
                    [27] = 100,
                    [58] = 100,
                    [59] = 100,
                });
        }
        
        [Fact]
        public void Part2Line2()
        {
            var computer = ComputerPart2.Initialize(@"mask = 00000000000000000000000000000000X0XX
mem[26] = 1");

            computer.Memory.Should().BeEquivalentTo(
                new Dictionary<long, long>
                {
                    [16] = 1,
                    [17] = 1,
                    [18] = 1,
                    [19] = 1,
                    [24] = 1,
                    [25] = 1,
                    [26] = 1,
                    [27] = 1
                });
        }
        
        [Fact]
        public void Part2()
        {
            var computer = ComputerPart2.Initialize(@"mask = 000000000000000000000000000000X1001X
mem[42] = 100
mask = 00000000000000000000000000000000X0XX
mem[26] = 1");

            computer.Memory.Values.Sum()
                .Should().Be(208);
        }
    }
}