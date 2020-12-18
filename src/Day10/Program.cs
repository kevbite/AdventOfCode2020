using System;
using System.Collections.Generic;
using System.Linq;

Console.WriteLine("Part 1");
var part1 = JoltageCalculator.Part1(PuzzleInput.Data);
Console.WriteLine(part1.diffsOfOne * part1.diffsOfThree);
Console.WriteLine("Part 2");
Console.WriteLine(JoltageCalculator.Part2(PuzzleInput.Data));

public static class JoltageCalculator
{
    public static (int diffsOfOne, int diffsOfThree) Part1(string input)
    {
        var joltageRatings = ReadJoltageRatings(input);

        var lastRating = 0;

        var diffs = new List<int>();
        foreach (var joltageRating in joltageRatings)
        {
            diffs.Add(joltageRating - lastRating);
            lastRating = joltageRating;
        }

        var diffsOfOne = diffs.Count(x => x is 1);
        var diffsOfThree = diffs.Count(x => x is 3) + 1;

        return (diffsOfOne, diffsOfThree);
    }

    private static List<int> ReadJoltageRatings(string input)
    {
        return input.Split(Environment.NewLine)
            .Select(int.Parse)
            .OrderBy(x => x)
            .ToList();
    }

    // (0), 1, 4, 5, 6, 7, 10, 11, 12, 15, 16, 19, (22)
    //         ^  *  *  *
    // (0), 1, 4, 6, 7, 10, 11, 12, 15, 16, 19, (22)

    // (0), 1, 4, 7, 10, 11, 12, 15, 16, 19, (22)

    public static long Part2(string input)
    {
        var joltageRatings = ReadJoltageRatings(input);
        joltageRatings.Insert(0, 0);
        joltageRatings.Add(joltageRatings.Last() + 3);
        Dictionary<int, long> hashSet = new();
        
        long Part2(int start)
        {
            if (hashSet.TryGetValue(start, out var count))
                return count;
            count = 1L;
            for (var i = start; i < joltageRatings.Count; i++)
            {
                if (i + 2 < joltageRatings.Count && joltageRatings[i + 2] <= joltageRatings[i] + 3)
                {
                    count += Part2(i + 2);
                }

                if (i + 3 < joltageRatings.Count && joltageRatings[i + 3] <= joltageRatings[i] + 3)
                {
                    count += Part2(i + 3);
                }
            }

            hashSet.Add(start, count);
        
            return count;
        }
        
        return Part2(0);
    }

}

public static class PuzzleInput
{
    public const string Data = @"133
157
39
74
108
136
92
55
86
46
111
58
80
115
84
67
98
30
40
61
71
114
17
9
123
142
49
158
107
139
104
132
155
96
91
15
11
23
54
6
63
126
3
10
116
87
68
72
109
62
134
103
1
16
101
117
35
120
151
102
85
145
135
79
2
147
33
41
93
52
48
64
81
29
20
110
129
43
148
36
53
26
42
156
154
77
88
73
27
34
12
146
78
47
28
97";
}