using System;
using System.Collections.Generic;
using System.Linq;

var input = 1350;
// var input = 10;

var map = new Dictionary<(int x, int y), char>();
var mapSize = 100;

for (int i = 0; i < mapSize; i++) 
{
    for (int j = 0; j < mapSize; j++)
    {
        var quadraticResult = i*i + 3*i + 2*i*j + j + j*j + input;
        var binaryString = Convert.ToString(quadraticResult, 2);

        var isWall = binaryString.Count(c => c == '1') % 2 == 1;

        map[(i, j)] = isWall ? '#' : '.';
    }
}

PrintMap(map, mapSize);

(int x, int y) me = (1, 1);
(int x, int y) target = (31, 39);
// (int x, int y) target = (7, 4);

var distances = new Dictionary<(int x, int y), int>{
    [me] = 0
};

var onlySpaces = map.Where(m => m.Value == '.').ToDictionary(d => d.Key, d => d.Value);

while(!distances.ContainsKey(target))
{
    var nextRound = onlySpaces.Where(m => !distances.ContainsKey(m.Key)).ToList();
    var maxDist = distances.Values.Max();

    var highestDistancesSoFar = distances.Where(d => d.Value == maxDist).ToList();

    var adj = new HashSet<(int x, int y)>();

    foreach(var highest in highestDistancesSoFar)
    {
        adj.Add((highest.Key.x + 1, highest.Key.y));
        adj.Add((highest.Key.x - 1, highest.Key.y));
        adj.Add((highest.Key.x, highest.Key.y + 1));
        adj.Add((highest.Key.x, highest.Key.y - 1));
    }

    adj = adj.Where(a => !distances.ContainsKey(a) && onlySpaces.ContainsKey(a)).ToHashSet();

    foreach(var a in adj)
    {
        distances.Add(a, maxDist + 1);
    }
}

Console.WriteLine(distances[target]);
Console.WriteLine(distances.Count(d => d.Value <= 50));

void PrintMap(IDictionary<(int x, int y), char> map, int mapSize)
{
    for(int j = 0; j < mapSize; j++)
    {
        for (int i = 0; i < mapSize; i++)
        {
            Console.Write(map[(i, j)]);
        }
        Console.WriteLine();
    }
}