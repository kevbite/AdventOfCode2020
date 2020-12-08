using System;
using FluentAssertions;
using Xunit;

namespace Day8.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var input = @"nop +0
acc +1
jmp +4
acc +3
jmp -3
acc -99
acc +1
jmp -4
acc +6";
            
            HandheldGameConsole.GetAccumulatorAtLoopInfiniteLoopStart(input)
                .Should().Be(5);
            
        }
        
        [Fact]
        public void Test2()
        {
            var input = @"nop +0
acc +1
jmp +4
acc +3
jmp -3
acc -99
acc +1
jmp -4
acc +6";
            
            HandheldGameConsole.RunAndFix(input)
                .Should().Be(8);
            
        }
    }
}