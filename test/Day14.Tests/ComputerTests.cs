using System;
using FluentAssertions;
using Xunit;

namespace Day14.Tests
{
    public class ComputerTests
    {
        private const string Input = @"mask = XXXXXXXXXXXXXXXXXXXXXXXXXXXXX1XXXX0X
mem[8] = 11
mem[7] = 101
mem[8] = 0";
        [Fact]
        public void Part1()
        {
            var computer = Computer.Initialize(Input);

            computer.Memory[7].Should().Be(101);
            computer.Memory[8].Should().Be(64);
        }
    }
}