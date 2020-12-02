using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit;

namespace Day2.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Part1()
        {
            PasswordValidator.ValidatePasswords1(@"1-3 a: abcde
1-3 b: cdefg
2-9 c: ccccccccc").Should().Be(2);
        }
        
        [Fact]
        public void Part2()
        {
            PasswordValidator.ValidatePasswords2(@"1-3 a: abcde
1-3 b: cdefg
2-9 c: ccccccccc").Should().Be(1);
        }
    }
}