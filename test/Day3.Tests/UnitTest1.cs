using FluentAssertions;
using Xunit;

namespace Day3.Tests
{
    public class UnitTest1
    {
        private string _input = "..##.......\n#...#...#..\n.#....#..#.\n..#.#...#.#\n.#...##..#.\n..#.##.....\n.#.#.#....#\n.#........#\n#.##...#...\n#...##....#\n.#..#...#.#";

        [Fact]
        public void Part1()
        {

            Day3Solver.Part1(_input)
                .Should().Be(7);
        }
        
        [Fact]
        public void Part2()
        {
            Day3Solver.Part2(_input)
                .Should().Be(336);
        }
    }
}