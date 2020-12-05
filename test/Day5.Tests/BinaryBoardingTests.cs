using System;
using FluentAssertions;
using Xunit;

namespace Day5.Tests
{
    public class BinaryBoardingTests
    {
        [Theory]
        [InlineData("FBFBBFFRLR", 357)]
        [InlineData("BFFFBBFRRR", 567)]
        [InlineData("FFFBBBFRRR", 119)]
        [InlineData("BBFFBBFRLL", 820)]
        public void ShouldReturnSeatId(string seat, int seatId)
        {
            BinaryBoarding.GetSeatId(seat)
                .Should().Be(seatId);
        }

        [Theory]
        [InlineData(13, 1, 5, 10, 11, 12, 14, 15, 16, 20, 25)]
        [InlineData(19, 1, 2, 3, 10, 15, 16, 17, 18, 20, 21, 22, 40)]
        public void ShouldFindEmptySeat(int emptySeatId, params int[] filledSeats)
        {
            BinaryBoarding.FindEmptySeat(filledSeats)
                .Should().Be(emptySeatId);
        }
    }
}