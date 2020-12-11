using System;
using FluentAssertions;
using Xunit;

namespace Day9.Tests
{
    public class XmasTests
    {
        private readonly string _input = @"35
20
15
25
47
40
62
55
65
95
102
117
150
182
127
219
299
277
309
576";

        [Fact]
        public void Part1()
        {
            Xmas.FindInvalidNumber(_input, windowSize: 5)
                .Should().Be(127);
        }
        
        [Fact]
        public void Part2()
        {
            Xmas.FindEncryptionWeakness(_input, invalidNumber: 127)
                .Should().Be(62);
        }
    }
}