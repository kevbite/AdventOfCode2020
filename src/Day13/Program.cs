using System;
using System.Collections.Generic;
using System.Linq;

Console.WriteLine("Part 1");
var busIds =
    "19,x,x,x,x,x,x,x,x,41,x,x,x,x,x,x,x,x,x,823,x,x,x,x,x,x,x,23,x,x,x,x,x,x,x,x,17,x,x,x,x,x,x,x,x,x,x,x,29,x,443,x,x,x,x,x,37,x,x,x,x,x,x,13";
var busRoute = new BusRoute(busIds);
var (busId, timestamp, timeToWait)
    = busRoute.GetNextDeparture(1005162);
Console.WriteLine(busId * timeToWait);

public class BusRoute
{
    private readonly Dictionary<int, int> _busIds;

    public BusRoute(string busIds)
    {
        _busIds = busIds.Split(',')
            .Select((busId, index) => (busId,index))
            .Where(x => x.busId is not "x")
            .ToDictionary(x =>  x.index, 
                x => int.Parse(x.busId));
    }

    private IEnumerable<(int busId, long timestamp)[]> GetBusDepartures()
    {
        var count = 0L;
        while (true)
        {
            var buses = _busIds.Values.Where(busId => count % busId == 0)
                .Select(busId => (busId, timestamp: count))
                .ToArray();

            yield return buses;
            count++;
        }
    }

    public (int busId, long timestamp, long timeToWait) GetNextDeparture(int currentTimestamp)
    {
        var first = GetBusDepartures()
            .Skip(currentTimestamp)
            .First(x => x.Any());

        var ( busId,  timestamp) = first.Single();
        return (busId, timestamp, timestamp - currentTimestamp );
    }

    public long Part2()
    {
        var startBusId = _busIds[0];
        var dic = new Dictionary<int, int>(_busIds);
        var count = 0;
        foreach(var item in GetBusDepartures())
        {
            if (item.Any(x => x.busId == startBusId))
            {
                count = 0;
                dic = new Dictionary<int, int>(_busIds);
                dic.Remove(count);
            }
            else if(dic.TryGetValue(count, out var busId) && item.Any(x => x.busId == busId))
            {
                dic.Remove(count);
                if (dic.Count == 0)
                {
                    return item.First().timestamp - count;
                }
            }

            count++;
        }

        return -1;
    }
}