using System;
using FluentAssertions;
using Xunit;

namespace Day13.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Part1()
        {
            var busRoute = new BusRoute("7,13,x,x,59,x,31,19");

            busRoute.GetNextDeparture(939)
                .Should().Be((59,944,5));
        }
        
        [Fact]
        public void Part2()
        {
            var busRoute = new BusRoute("7,13,x,x,59,x,31,19");

            busRoute.Part2()
                .Should().Be(1068781);
        }
    }
}