using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

Console.WriteLine("Part 1");
Console.WriteLine(Stabiliser.GetOccupiedSeatsSeatsOnStabilisationPart1(PuzzleInput.Data));
Console.WriteLine("Part 2");
Console.WriteLine(Stabiliser.GetOccupiedSeatsSeatsOnStabilisationPart2(PuzzleInput.Data));

public static class Stabiliser
{
    public static int GetOccupiedSeatsSeatsOnStabilisation(string input, Func<Seating, Seating> func)
    {
        var seating = Seating.FromString(input);
        var lastOccupiedSeats = seating.OccupiedSeats;
        while (!seating.OccupiedSeats.SetEquals(lastOccupiedSeats) || lastOccupiedSeats.Count is 0)
        {
            lastOccupiedSeats = seating.OccupiedSeats;
            seating = func(seating);
        }

        return seating.OccupiedSeats.Count;
    }
    public static int GetOccupiedSeatsSeatsOnStabilisationPart1(string input)
        => GetOccupiedSeatsSeatsOnStabilisation(input, seating => seating.Part1NextRound());

    public static int GetOccupiedSeatsSeatsOnStabilisationPart2(string input)
        => GetOccupiedSeatsSeatsOnStabilisation(input, seating => seating.Part2NextRound());

}
public record SeatPosition(int X, int Y)
{
    public SeatPosition Left => new(X - 1, Y);
    public SeatPosition TopLeft => new(X - 1, Y - 1);
    public SeatPosition Top => new(X, Y - 1);
    public SeatPosition TopRight => new(X + 1, Y - 1);
    public SeatPosition Right => new(X + 1, Y);
    public SeatPosition BottomRight => new(X + 1, Y + 1);
    public SeatPosition Bottom => new(X, Y + 1);

    public SeatPosition BottomLeft => new(X - 1, Y + 1);

    

    public IEnumerable<SeatPosition> GetAdjacentSeats()
    {
        yield return Left;
        yield return TopLeft;
        yield return Top;
        yield return TopRight;
        yield return Right;
        yield return BottomRight;
        yield return Bottom;
        yield return BottomLeft;
    }
};

public record Seating
{
    private readonly int _maxX;
    private readonly int _maxY;

    public ISet<SeatPosition> SeatPositions { get; }

    public ISet<SeatPosition> OccupiedSeats { get; init; }
        = new HashSet<SeatPosition>();

    private Seating(ISet<SeatPosition> seatPositions)
    {
        SeatPositions = seatPositions;
        _maxX = seatPositions.Max(x => x.X);
        _maxY = seatPositions.Max(x => x.Y);
    }

    public Seating Part1NextRound()
    {
        var newOccupiedSeats = new HashSet<SeatPosition>(OccupiedSeats);
        foreach (var seatPosition in SeatPositions)
        {
            var adjacentSeats = CountOccupiedAdjacentSeats(seatPosition);
            if (!newOccupiedSeats.Contains(seatPosition) && adjacentSeats is 0)
            {
                newOccupiedSeats.Add(seatPosition);
            }

            if (newOccupiedSeats.Contains(seatPosition) && CountOccupiedAdjacentSeats(seatPosition) is >= 4)
            {
                newOccupiedSeats.Remove(seatPosition);
            }
        }

        return this with {OccupiedSeats = newOccupiedSeats};
    }
    
    public Seating Part2NextRound()
    {
        var newOccupiedSeats = new HashSet<SeatPosition>(OccupiedSeats);
        foreach (var seatPosition in SeatPositions)
        {
            var visibleSeats = CountVisibleSeats(seatPosition);
            if (!newOccupiedSeats.Contains(seatPosition) && visibleSeats is 0)
            {
                newOccupiedSeats.Add(seatPosition);
            }

            if (newOccupiedSeats.Contains(seatPosition) && visibleSeats is >= 5)
            {
                newOccupiedSeats.Remove(seatPosition);
            }
        }

        return this with {OccupiedSeats = newOccupiedSeats};
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        for (var y = 0; y <= _maxY; y++)
        {
            for (var x = 0; x <= _maxX; x++)
            {
                var seatPosition = new SeatPosition(x, y);
                var c =
                    (occupied: OccupiedSeats.Contains(seatPosition),
                            isSeat: SeatPositions.Contains(seatPosition)) switch
                        {
                            (true, _) => '#',
                            (false, true) => 'L',
                            _ => '.'
                        };

                sb.Append(c);
            }

            if (y != _maxY)
            {
                sb.AppendLine();
            }
        }

        return sb.ToString();
    }

    private int CountOccupiedAdjacentSeats(SeatPosition seatPosition)
    {
        var adjacentSeats = seatPosition.GetAdjacentSeats()
            .Where(SeatPositionInBounds);

        return adjacentSeats.Count(x => OccupiedSeats.Contains(x));
    }

    private bool SeatPositionInBounds(SeatPosition position)
    {
        return position.X >= 0 && position.X <= _maxX
                && position.Y >= 0 && position.Y <= _maxY;
    }
    
    private int CountVisibleSeats(SeatPosition seatPosition)
    {
        var visibleSeats = new[]
        {
            GetVisibleSeatPosition(seatPosition, x => x.Left),
            GetVisibleSeatPosition(seatPosition, x => x.TopLeft),
            GetVisibleSeatPosition(seatPosition, x => x.Top),
            GetVisibleSeatPosition(seatPosition, x => x.TopRight),
            GetVisibleSeatPosition(seatPosition, x => x.Right),
            GetVisibleSeatPosition(seatPosition, x => x.BottomRight),
            GetVisibleSeatPosition(seatPosition, x => x.Bottom),
            GetVisibleSeatPosition(seatPosition, x => x.BottomLeft)
        };
        
        return visibleSeats.Where(x => x is not null)
            .Count(x => OccupiedSeats.Contains(x));
    }

    private SeatPosition GetVisibleSeatPosition(SeatPosition seatPosition, Func<SeatPosition, SeatPosition> func)
    {
        var visibleSeatPosition = func(seatPosition);
        while (!SeatPositions.Contains(visibleSeatPosition) && SeatPositionInBounds(visibleSeatPosition))
        {
            visibleSeatPosition = func(visibleSeatPosition);
        }

        return SeatPositionInBounds(visibleSeatPosition) ? visibleSeatPosition : null;
    }


    public static Seating FromString(string input)
    {
        var seats = new HashSet<SeatPosition>();
        var lines = input.Split(Environment.NewLine);

        for (var y = 0; y < lines.Length; y++)
        {
            var line = lines[y];
            for (var x = 0; x < line.Length; x++)
            {
                if (line[x] == 'L')
                {
                    seats.Add(new SeatPosition(x, y));
                }
            }
        }

        return new(seats);
    }
}

public static class PuzzleInput
{
    public const string Data =
        @"LLLLLL.LLLLLL.LLLLL.LLLLLLLLLLLLLLL.LLLLLLL.LLLLL.L.LLLLLLL.LLLLLLLLLLLLLL.LLL.L.L.LLLLLLLLL.LLLLL
LLLLLL.LLLLLL.LLLLLLLLLLLLLLL.LLLLL.LLLLLLL.LLLLLLLLLLLLLLLL.LLLLLLLLLLLLLLLLLLL.LLLLLLLLLL..LLLLL
LLLLLLLLLLLLL.L.LLL.LLLLLLLL.LLLLLLLLLLLLLL.LLLLLLLLLL.LLLLL.LLLLLLLL.LLLLLLLLLL.LLLLL.LLLLL.LLLLL
LLLLLL.LLLLLLLLLLLL.LLLLLLLL.L.LLLL.LLLLLLLLLLLLL.LLLL.LLLLL.LL.LLLLL.LLLLLLLLLL.LLLLL.LLLLLL.LLLL
LLLLLL...LLLLLLLLLLLLLLLLLLL.LLLLLL.LLLLLLL.LLLLL.LLLLLLLLLL.LLLLLLLL.LLLLLLLLLLLLLLLL.LLLLL.LLLLL
LLLLLLLLLLLLL.LLLLL.LLLLLLLLLLLLLLLLLLLLLLL.LLLL.LLLL.LLLLLL.LLLLLLLLLLLLLLLLLLL.LLLLL.LLLLL.L.LLL
LLLLLLLLLLLLL.LLLLL.LLLLLLLL.LLLLLLLLLLLLLL..LLLL.LLLL.LLLLL.LLLLLLLL.LLLLLL.LLL.LLLLLLLLLLLLLLLLL
LLLLLLLLLLLLL.LLLLL.LLLLLLLL.LLLLLLLLLLLLLLLLLLLLLLLLL.LLLLL.LLLLLLL..LLLL.LLLLL.LLLLL.L.LLLLLLLLL
LLLLL..LLLLLL.LLLLLLLLLLLLLL.LLLLLLLLLLLLLLLLLLLL.LLLL.LLLLLLLLLLLLLL.LLLL.LLLLLLLLL.L.LLLLL.LLLLL
LLLLLL.LLLLLL.LLLLL.LLLLLLLL.LLLLLL.LLLLLLL.LLLLL.LLLL.LLLLLLLLLLLLLLLLLLL.LLL...LLLLL.LLLLLLLLLLL
...L...L....LL...LL.........L...LL.L...LLLL......LLLLLL.L.L.....L....L.LL.L.L.L.L...L.L..L.....L..
LLLLLL.LLLLLL.LLLLL.LLLLLLLL.LL.LLL.LLLLLLLLLLLLL.LL.L.LLLLLLLLLLLLLL.LL.LLLLL.LLLLLLLLLLLLLLLLLLL
LLLL.LLLLLLLLLLLLLL.LLLLLLLL.LLLLLLLLLLLLLL.LLL.L.LLLL..LLLL.LLLLL.LLLLLLL.LLLLLLLLLLL.LLLLL.LLLLL
LLLL.LLLLLL.L.LLLLL.LLLLLLLLLLLLLLL.L.LLLLL.LLLLL.L.LL.LLLLL.LLLLLLLL.LLLLLLLLLL.LLLLL.LLLLL.LLLLL
LLLLLL.LLLLLLLLLLLL.LLLLLLLLLL.LLLLLLLLLLLL.LLLLL.LLLLLLLLLL.LLLLLLLL.LLLL.LLLLLLLLLLLLLLLLLLLLLLL
LLLLLLLLLLLLLLLLLLL.LLLLLLLLLLLLLLL.LLL.LLLLLLLLL.LLLLLLLLLLLLLLLLLLL.LLLL.L.LLLLLLLLLLL.LLLLL.LLL
LLLLLLLLLLLLL.LLLL..LLLLLLLL.LLLLLLLLLLLLLL.LLLLL.LLLL.LLLLL.LLLLLLLL.LLLL.LLLL.LLLLLL.LLLLL.LLLLL
LLLLLLLLLLLLL.LLLLL..LLLLLL..LLLLLL.LLLLLLLLLLLLLLLLLL.LLLLL.LLLLLLLLLLLLL.LLLLLLLLLLLLLLLLLLLLLLL
......L...LL.....LL..L.L...L...L..L.L.L.....L.LL..LLL.....L........L..........LL.........LLLL.....
LL.LLLLLLLLL.LLLLLL.LLLLLLLL.LLLLLL.LLLLLLL.LLLLL.LLLL.LLLLL.LLLLLLLLLLLLL.LLLLL.LLLLLLLLLLL.LLLLL
.L.LLL.LLLLLL.LLLLL.LLLLLLLLLLLLLLL.LLLLLLL.LLLLL.LLLL.LLLLL.LL.LLLLLLLLLL.LLL.LLLLLLLLLLLLL.LLLLL
LLLLLL..LLLLLLLLLLL.LLLLLLLL..LLLLLLL.LLLLL.LLLLLL.LLLLLLL.L.LLLLLLLL.LLLLLLLLLL.LLLLLLLLLLLL.LLLL
LLLLLL.LLLLLL.LLLLL.LLLLLLLL.LLLLLL.LLLLLLL.LLLLL..LLL.LLLLL.LLLLLLLL.LLLL.LLLLL.LLLLL.LLLLLLLLLLL
.L....L....L.L.LLLLL..L....L......L....LL.LLL.L.LL....LL..........L....L.L...LLLL.L..LLL.LL.L.....
LLLLLL.LLLLLL.LLLLL.LLLLLLLL.LLLLLLLLLL..LL.LLLLL.LLLLLLLLLL.LLLLLLLL.LLLLLLLLLLLLLLLL.LL.LL.LLLLL
LL.LLL.LLLLLL.LLLLLLLLLLLLLLLLLLLLL.LLLLLLL.LLLL..LLLL.LLLLL.LL.LLLLL.LL.LLLLLLL.LLLLL.LLL.LLLLLLL
LLL.LLLLLLLLL.LLLLL.LLLLLLLL.LLLLLL.LL.L.LLLLLLLL.LLLL.LLLLLLLLLLLLLL.LLLLLLLL.L.LLLLL.LLLLLLLLLLL
LLLLL.LLLLLLLLLLLLL.LLLLLLLLLLLLLLLLLLLLLLL.LLLLL.LLL..LLLLLLLLLLLLLL..LLL.LLLLL.LLLLLLLLLLL.LLLLL
...L.....LL.L..LL..L.LLL..LL...L.LL..L....LL.....L.L.L.LL....L.L.L.......L....L.....L........LLL..
LLLLLL.LLLLLL.LLLLL.LLLLLLLLLLLLLLL.LLLLLLL.LLLLL.LLLL.LLLLLLLLLLLLLLLLLLL.LLLLL.LLLLL.LLLLLL.LLLL
LLLLLL.LL.LLLLL.L.LLLLL.LLLLLLLLLLLLLLLLLLLLLLLLLLLLLL.LLLLL.LLLLLLLL.LLLLLLLLLL.L.LLLLLL.LL.LLLLL
LLLLLL.LLLLLL.LLLLL.LLLLLLLLLLLLLLL.LLLLLL..LLLLL.LLLL.L.LLL.LLLLLLLL.LLLL.LLLLL.LLLLLLLLLLL.L.LLL
LLLLLL.LLLLLLLLLLLL.LLLLLLLL.LLLL.LLLLLLLLL.LLL.LL.LLL.LLLLL..LLLLLLL.LLLL.LLLLL.LLLLLLLLLL..LLLLL
LLLLLL.LLLLLL..LLLLLLLLLLLLL.LLLLLL.LLLLLLL.LLLLL.LLLL.LLLLL.L.LLLLLL.LLLLLLLLLL.LLLLL.LLLLL.LLL.L
LLLLLLLLLLLLL.LLLLL.LLLLLLLLLLLLLLLLLLLLLLL.LLLLLLLLLL.LLLLL.LLLLLLLL.LLLL.LLLLL.LLLLL.LLLLLLLLLLL
LLLLLLLLLLLLL.LLLLL.LLLLLLLL.LLLLLLLLLLLLLLLLLLLL.LLLLLLLLLL.LLLLLLLL.LL.L.LLLLLLLLLLLLLLLLLLL.LLL
L.LL..LL....L.L......L...LLL......LL.L.....L.L.LL.L...L..L....L.LLL..LLL..L.L.L..LL...L.LL..LL....
LLLLLL.LLLLLLL.L.LL.LLLLLLLLLLLLLLL.LLLLLLL..LLLL.LLLL.LLL.L.LLLLLLLLLLLLL.LLLLL.LLLL.LLLLLL.LL.LL
LLLLLLLLLLLLLLLLLLLLLLLLLLLL.LLLLL.LLL.LLLLLLLLLL.LLLL.LLLLL.L.LLL.LL.LLLL.LLL.LLLLLLLLLLLLL.LLLLL
LLLLLLLLLLLLLLLLLLL.LLL.LLLLL.LLLLL.L.L.LLLLLLLLL.LLLL.LLLLL.LLLLLLLLLLLLL.LLLLL.LLLLL.LLLLL.LLLLL
L.LLLL.LLLLLL.LLLLLLL.LLLLLL.LL.LLLLLLLLLLL.LLLLLLLLLLLLLLLLLLLLLLLL...LLL.LLLL..LLLLLLLLLLL.LLLLL
LLLLLL.LLLLLL.LLL.L.LLLLLLLLLLLLLLLLLLLLLLL.LLLLL.LLLL.L.LLL.LLLLLLLL.LLLL.LLLLL.LLLL..LLLLLLLLLLL
LLLLLL.LLLLLLLLLLLL.LLLLLLLL.L.LLLL..LLLLLLLLLLLL.LLLLLLLLLL.LLLLLLLLLL.LLLLLLLL.LLLLL.LLLLL.LLLLL
..L..LL....L.....L..L.LL.L..L.................L....L.....L....L..L...L..L....L...LL...LLL..LL.....
LLLLLL.LLLLLLLLLLLL.LLLLLLLL.LLLLLL.LLLLLLLLLLLLL.LLLL.LLLLLLLLL.LLLLLL..L.LLLLL.LLL.L.LLLLL.LLLLL
LLLLLL.LLLLLL.L.LLLLLLLLLLLLLLLL.LL..LLLLLL.LLLLL.LLLL.LLLLL.LLLLLLLL.LLLL.LLLLL.LLLLLLLLLLLLLLLLL
LLLLLL.LLLLLLLLLLLLLLLLLLLLL.LLLLLL.LLLLLLLLLLLLL.LLLL.LLLLL.LLLLLLLL..LLL.LLLLL.LL.LL.LLLLL.LLLLL
LLLLLL.LLLLLL..LLLLLLLLLLLLLLLLLLLL.LLLLLLL.LLL.L.LLLLLLLLLLLLLLLLL.LLLLLL.LLLL.LLLLLL.LLLLL.LLLLL
LLLLLL.LLLLLL.LLLLLLLLLLLLLL.LLLLLL.LLLLLLL.LLLLLLLLLLLLLLLL.LLLLLLLLLLLLL.LLLLL.LLLLL.LL.LLLLLLLL
L.L...L..LL.L....LL...L..L.L.LL...L..L..L..L...L.LL.LL......L.L..L.L...LL.....L........LL...L.....
LLLLLLLLLLLLL.LLLLL.LLLLLLLL.LLLLLL.LLLLLLL.LLLLL.LLLLLLLLL..LLLLLLLL.LLLL.LLLLLLLLLLLLLLLLL.LLLLL
LLLLLL.LLLLLLLLLLLLLLLLLLLLL.LLLLLLLLLLLLLL.LLLLLLLL.LLLLLLLLLLLLLLLL.LLLL.LLLLL.LLLLL.LLLLLLLLLLL
LLLLLL.LLLLLL.LLLLL.LLLLLLLLLLLLLLL.LLLLLLL.LLLLL.LLLL.LLLLL.LLLLLLLL.LLLL.LLLLLLLLLLL.LLLLL.LLLL.
LLLLLLLLLLLLLLLLLLL.LLLLLLLLLLLLLLLLLLLLLLL.LLLLLLLLLL.LLLLL.LLLLLLL.LLLLLLLLLLL.LLLLLLLLLLL.LLLLL
LLLLLL.LLLLLLLLLLLL.LLLLLLLLLL.LLLLLLLLLLLLLLLLL.LLLLLLLLLLLL..LLLLLLLLLLL.LLLLLLL.LLL.LLLLL.LLLLL
L.LLLL.LLLLLL.L.LLLLLLLLLLLL.LLLLLL.LLLLLLLLLLLLL.LLL.LLLL.LLLLLLLLLL.LLLLLLLLLL.LLLLLLLLLLL.LLLLL
...L..L.L...LL.....LL......L...L.....L.L......L..LLL.....L..LL....LLL...L..LL..LL.L.L.L.L..L...L.L
.LLLLL.LLLLLL.LLLLL..LLLLLLL.LLLLLLLL.LLLL..LLLLL.LL.L.LLLLL.LLL.LLLLLLLLL.LLLLLLLLLLLLLLLLL.LLL.L
LLLLLLLLLLLLL.LL.LL.LLLLLLLLLLLLLLL.LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL.LLLL.LLLLL.LLLLLLLLLLL..LLLL
LLLL.LLLLLL.LLLLLLL.LLLLLLLL.LLLLLLLLLLLLLLLLLLLL.LLLL..LLLL.LLLLLLLL.LLL...LLLL.LLLLL.LLLLLLLLLLL
LLL.LLLLLLLLL.LL.LL.LLLLLLLLLLLLLLL.LLLLLLL.LLLLLLLLLLL.L.LL.LLLLLLLLLLLLL.LLLLL.LLLLL.LLLLL.LLLLL
LLLLLL.LLLLLLLLLLLLLLLLLLLLL.LLLLLL.LLLLLLLLLLLLL.LLLL.LLLLLLLLL.LLLLLLL.L.LLLLL.LLL.L.LLLLL.L.LLL
..L......LL.........L.L...L.....L.L...LLL.L.....L....L.L...L.L.L.L..L.....L..L...L..L.....L.L...L.
LLLLLL.LLLLLL.LLLLL.LLLL.LLLL.LLLLL.LLLLLLL.LLLLL.LLLL.LLLLL.LLLLLLLL.LLLLLLL.LL.LLLLLLLLLLL.LLLLL
.LLLLLLLLL.LL.L.LLL.LLLLLLLLLLLLLLL.LLLLLLLLLL.LLLLLLL.LLLLLLLLLLLLLLLLLLL.LLLLL.LLLLLLLLLLLLLLLLL
LLLLLL.LLLLLLLLLLLLLL.LLLLLL.LL.LLLL.LLLLLL.LLLLLLLLL..LLLLL.LLLLLLLL.LLLLLLLLLLLLLLLL.LLLLL.LLLLL
LLLLL..LLLLLL.LLLLL.LLLLLLLLLLLLL.L.LLLLLLL.LLLLLLLLLL.LLLLL.LLLLLLLL.LLLL.LLLLL.LLLLLLLLLLL.LLLLL
LLLLLL.LLLLLL.LLLLL.LLLLLLLL.LLLLLL.LLLLLLLLLLLLLLLLL..LLLLLLLLLLLLLLLLLLLLLLLLL.LLLLL.LLL.LLLLLLL
LLLLLL.LLLLLL..LLLL.LLLLLLL..L.LLLLLLLLLLLL.LL.LL.LLLL.LLLLL.LLLLLLLL.LLLLLLLLLL.L.LLL.LLL.L.LLLLL
LLLLLL.LLLLLL.LLLLL.LLLLLLLL.LLLLL..LLLLLLL.LLLL.LLLLL.LLLLLLLLLLLLLL.LLLL.LLLLL.LLLLL.LLLLL.LLLLL
LLLLLL.L.LLLL.LLLLL.LLL.LLL..LLLLLL.LLLLLLL.LLLLL.LLLL.LLLLL.L.LLLLLLLLLLL.LLLLLL.LLLL.LLLLL.LLLLL
.......L......LLL.....L....LL..L....L.L..L..L.L..L.L.....L...L..........L.....L.L.....L.L..LLL.L..
LLLLLL.LLLLLL..LLLL.LLLLLLLL.LLLLLL.LLLLLLLLLLLLLLLLLL.LL.LL.LLLLLLLL.LLLLLLLLLLLLLLLL.LLLLL.LLLLL
LLLLLL.LLLLLL.LLLLL.L.LLLLLL.LLLLLL.LLL.LLL.LLLLL.LLLL.LLL.LLLLLLLLLLLLL.L.LL.LL.LLLLLL.LLLL.LL.LL
LLLLLL.LLL.LLLL.LLL.LL.LLLLL.LL.LLLLLLLLLLLLLLLLL.L.LL.LLLLLLLLLLLLLL.LLLLLLLLLLLLLLLLLL.LLLLLLLLL
LLL.LL.LLLLLLLLLLLL.LLLLLLLL.LLLL.LLLLLLLLL.LLLLLLLLLL.LLLLLLLLLLL.LLLLLLL.LLLLL..LLL..LLLLL.LLLLL
LLLLLL..LLLLLLLLLLL.LLLLLLLL.LLLLLL.LLLL.LLLLLLL..LLLL.LLLLL.LLLLLLLL..LLL.LLLLLLLLLLL.LL.LL.LLLLL
LLLLLLLLLLLLL.LLLLLLLLLLLLLL.LLLLLL.LLLLLLL.LLLLL.LL.LLLLLLL.LLLLLLLLLLLLL.LLLLL.LLLLLLLLLLLLLLLLL
LLLLLLLLLLLLLLLLLLL.LLLLLL.L.LLLLLL.LLLLLLL.LLLLLL.L.L.LLLLL.LLLLLL.L.LLLLLLLLLL.LLLLL.LLLLL.LLLLL
.LLLLL.LLLLLLLLLLLLLLLLLLLLL.LLLLLLLLLLLLLL.LLLL..LLLL.LLLLL.LLLLLLLL.LLLL.LLLLLLLLLLL.LLLLLLLLLLL
LLLLLLLLLLLLL.LLLLL.LLLLLLLL.LLLL.LLLLLLLLLLLLLLLLLLLLLLLLLL.LLLLLLLLLLLLL.LLLLL.LLLLL.LLLLLLLLLLL
.L.LL....L..L...L.LLL.......L.LL.LL.L.L.L........LL..LL..L.......LL.L.LL...LLL.....L...L...L....L.
LLLLLL.LLLLLL.LLL.LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL.LLLLLLLLLL.LLLLLLLL.LLLLLLLLLL.LLLLLLLLLLL.LLLLL
LLLLLL.LLLLLLLLLLLL.LLLLLLLLLLLLLLL.LLLLLLL.LLLLL.L.LL.LLLLLLLLLLLLLL..LLL.LLLLL.LLL.L.LLLLLLLLLL.
LLLLLL.LL.LLL.LLLLL.LLLLLLLL.LLLLLLLLLLLLLL..LLLL.LLLL.LLLLL.LLLLLLLL..LLLLLLLL.LLLLL..LLLLL.LLL.L
.LLLLLLLLLLLL.LLLLL.LLLLLLLL.LLLLLL.LLLLLLL..LLLLLLLLL.LLLLLLLLLLLLLL.LLLL..LLLLLLLLLL.LLLLL.LLLLL
LLLLLLLLLLLLL.LLLLL.LLLLLL.LLLLLLLL.LLLLL.LLLLLLL.LLLL.LLLLLLLLLLLLLL.LLLLLLLLLLLLL.LL.LLLLL.LLLLL
LLLLLLLLLLLLL.LLLLL.LLLLLLLLLL.LLLL.LLLLLLL.LLLLL.LLLL.LLLLL.LLLLLLLL.LLLL.LLLLLLLLLLL..LLLL.LLLLL
LLLLLL.LLLLLL.LLLLL.LLLLLLLL.LLLLLLLLLLLLLLLLLLLL.LLLL.LLLLL.LLLLLLLLL.LLLLLLLLL.LLLLLLLLLLL.LLLLL
L........L........L.L.L...L........L..............L....L..L....L...L..L.LL.L..L.....L....LL..LL..L
LLLLLLLLLLLLLLLLLLL.LLLLLLLLLLLLLLL.LLLLLLLLLLLLL.LL.L.LLLLL.LLLLLLLLLLLLL.LLLLLLLLLLL.LLLLL.LLLLL
LLLLLLL.LLLLL.LLLLLLLLLLL..L.LL.LLLLLLLLLLLLLLLLLLLLLLLLL.LL.L.LLLLLL.L.LL.LLLLLLLLLLLLLLLLLLLLLLL
.LLLLL.LLLLL..LLLLL.LLLLLLLL.LLLLLL.LL.LLLL..LLL..LLLL.LLLLL.LLLLLLLL.LLLLLLLLLL.LLLLL.LLLLL.LLLLL
LLLLLL.LLLLLLL.LLLL.LLLLLLLL.LLLLLLLLLL.LLL.LLL.LL.LLL.LLLL.LLLLLLLLLLLLLLLLLLLL.LLLLL.LLLLLLLLLLL
LL.LL..LLLLLLLLLLLLLLLLLLLLL.LLLLLL.LLLL.LL.LLLLL.LLLL.L.LLL.LLLLLLLLLLLLL.LLLLL.LLLLL.LLLLL.LL.LL
LLLLLLLLLLLLL.LLLLL.LLLLLLLL.LLLLLL.LLLLLLL.LLLLL.LLLLLLLLLLLLLLLLLLL.LLL.LLLLLL.LLLLL.LLLLLLLLLLL
LLLLLL.LLLLLLLLLLLL..LLLLLLL.LLLLLL..LLLLLL.LLLLLLLLLLLLLLLL.LLLLLLLLLLLLL.LLL.L.LLLLL.LLLLLLLLLLL";
}