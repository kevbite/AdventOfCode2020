using FluentAssertions;
using Xunit;

namespace Day6.Tests
{
    public class CustomsDeclarationFormTests
    {
        private readonly string _input = "abc\n\na\nb\nc\n\nab\nac\n\na\na\na\na\n\nb";

        [Fact]
        public void Part1()
        {
            CustomsDeclarationForm.GetYesAnswers(_input)
                .Should().Be(11);
        }
        
        [Fact]
        public void Part2()
        {
            CustomsDeclarationForm.GetYesAnsweredByAllGroup(_input)
                .Should().Be(6);
        }
    }
}