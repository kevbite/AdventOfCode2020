using System;
using FluentAssertions;
using Xunit;

namespace Day11.Tests
{
    public class SeatingTests
    {
        private readonly string _startInput = @"L.LL.LL.LL
LLLLLLL.LL
L.L.L..L..
LLLL.LL.LL
L.LL.LL.LL
L.LLLLL.LL
..L.L.....
LLLLLLLLLL
L.LLLLLL.L
L.LLLLL.LL";

        [Fact]
        public void Part1()
        {
            Stabiliser.GetOccupiedSeatsSeatsOnStabilisationPart1(_startInput)
                .Should().Be(37);
        }  
        
        [Fact]
        public void Part1FirstRound()
        {
            var seating = Seating.FromString(_startInput);
            seating.Part1NextRound()
                .ToString().Should().Be(@"#.##.##.##
#######.##
#.#.#..#..
####.##.##
#.##.##.##
#.#####.##
..#.#.....
##########
#.######.#
#.#####.##");
        }  
        [Fact]
        public void Part1SecondRound()
        {
            var seating = Seating.FromString(_startInput);
            seating.Part1NextRound().Part1NextRound()
                .ToString().Should().Be(@"#.LL.L#.##
#LLLLLL.L#
L.L.L..L..
#LLL.LL.L#
#.LL.LL.LL
#.LLLL#.##
..L.L.....
#LLLLLLLL#
#.LLLLLL.L
#.#LLLL.##");
        }  
        
        [Fact]
        public void Part1ThirdRound()
        {
            var seating = Seating.FromString(_startInput);
            seating.Part1NextRound().Part1NextRound().Part1NextRound()
                .ToString().Should().Be(@"#.##.L#.##
#L###LL.L#
L.#.#..#..
#L##.##.L#
#.##.LL.LL
#.###L#.##
..#.#.....
#L######L#
#.LL###L.L
#.#L###.##");
        }  
        
        [Fact]
        public void Part2()
        {
            Stabiliser.GetOccupiedSeatsSeatsOnStabilisationPart2(_startInput)
                .Should().Be(26);
        }
        
        [Fact]
        public void Part2FirstRound()
        {
            var seating = Seating.FromString(_startInput);
            seating.Part2NextRound()
                .ToString().Should().Be(@"#.##.##.##
#######.##
#.#.#..#..
####.##.##
#.##.##.##
#.#####.##
..#.#.....
##########
#.######.#
#.#####.##");
        }  
        [Fact]
        public void Part2SecondRound()
        {
            var seating = Seating.FromString(_startInput);
            seating.Part2NextRound().Part2NextRound()
                .ToString().Should().Be(@"#.LL.LL.L#
#LLLLLL.LL
L.L.L..L..
LLLL.LL.LL
L.LL.LL.LL
L.LLLLL.LL
..L.L.....
LLLLLLLLL#
#.LLLLLL.L
#.LLLLL.L#");
        }  
        
        [Fact]
        public void Part2ThirdRound()
        {
            var seating = Seating.FromString(_startInput);
            seating.Part2NextRound().Part2NextRound().Part2NextRound()
                .ToString().Should().Be(@"#.L#.##.L#
#L#####.LL
L.#.#..#..
##L#.##.##
#.##.#L.##
#.#####.#L
..#.#.....
LLL####LL#
#.L#####.L
#.L####.L#");
        }  
    }
}