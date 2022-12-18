﻿using System.Numerics;
using System.Reflection.Metadata;
using System.Transactions;
using Microsoft.VisualBasic;

var lines = File.ReadAllLines("./input");

var map = new Dictionary<Vector3, int>();

foreach(var line  in lines)
{
    var split = line.Split(",").Select(s => int.Parse(s)).ToArray();

    var v = new Vector3(split[0], split[1], split[2]);
    
    map.Add(v, 6);
}

foreach(var vec in map)
{
    var neighbours = map.Where( m => 
        m.Key.X == vec.Key.X && m.Key.Y == vec.Key.Y && m.Key.Z == vec.Key.Z + 1 ||
        m.Key.X == vec.Key.X && m.Key.Y == vec.Key.Y && m.Key.Z == vec.Key.Z - 1 ||
        m.Key.X == vec.Key.X && m.Key.Y == vec.Key.Y + 1 && m.Key.Z == vec.Key.Z ||
        m.Key.X == vec.Key.X && m.Key.Y == vec.Key.Y - 1 && m.Key.Z == vec.Key.Z ||
        m.Key.X == vec.Key.X + 1 && m.Key.Y == vec.Key.Y && m.Key.Z == vec.Key.Z ||
        m.Key.X == vec.Key.X - 1 && m.Key.Y == vec.Key.Y && m.Key.Z == vec.Key.Z);

    map[vec.Key] = 6 - neighbours.Count();
}

Console.WriteLine(map.Sum(m => m.Value));

var contiguousAreasOfStuff = new Dictionary<int, ICollection<Vector3>>();

var maxX = map.Max(m => m.Key.X);
var maxY = map.Max(m => m.Key.Y);
var maxZ = map.Max(m => m.Key.Z);

var queue = new Queue<Vector3>();

queue.Enqueue(new Vector3(0, 0, 0));

var bigAirBubble = new HashSet<Vector3>{new Vector3(0, 0, 0)};

while(queue.TryDequeue(out var current))
{
    var neighbours = new List<Vector3>{
        new Vector3(current.X    , current.Y    , current.Z + 1),
        new Vector3(current.X    , current.Y    , current.Z - 1),
        new Vector3(current.X    , current.Y + 1, current.Z),
        new Vector3(current.X    , current.Y - 1, current.Z),
        new Vector3(current.X + 1, current.Y    , current.Z),
        new Vector3(current.X - 1, current.Y    , current.Z)};

    foreach(var n in neighbours)
    {
        if (!map.ContainsKey(n) && !bigAirBubble.Contains(n) && n.X >= -2 && n.Y >= -2 && n.Z >= -2 && n.X <= maxX + 2 && n.Y <= maxY + 2 && n.Z <= maxZ + 2)
        {
            bigAirBubble.Add(n);
            queue.Enqueue(n);
        }
    }
}

var map2 = new Dictionary<Vector3, int>();


foreach(var vec in bigAirBubble)
{
    var neighbours = map.Where( m => 
        m.Key.X == vec.X && m.Key.Y == vec.Y && m.Key.Z == vec.Z + 1 ||
        m.Key.X == vec.X && m.Key.Y == vec.Y && m.Key.Z == vec.Z - 1 ||
        m.Key.X == vec.X && m.Key.Y == vec.Y + 1 && m.Key.Z == vec.Z ||
        m.Key.X == vec.X && m.Key.Y == vec.Y - 1 && m.Key.Z == vec.Z ||
        m.Key.X == vec.X + 1 && m.Key.Y == vec.Y && m.Key.Z == vec.Z ||
        m.Key.X == vec.X - 1 && m.Key.Y == vec.Y && m.Key.Z == vec.Z);

    map2[vec] = neighbours.Count();
}

Console.WriteLine(map2.Sum(v => v.Value));

