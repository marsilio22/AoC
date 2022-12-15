using System.Diagnostics;

var lines = File.ReadAllLines("./input");

var sw = new Stopwatch();
sw.Start();

var beacons = new Dictionary<(int x, int y), (int x, int y)>();
var distances = new Dictionary<(int x, int y), int>();

foreach (var line in lines)
{
    var split = line.Split(" ");

    var sensorX = int.Parse(split[2].Split(",")[0].Split("=")[1]);
    var sensorY = int.Parse(split[3].Split(":")[0].Split("=")[1]);

    var beaconX = int.Parse(split[8].Split(",")[0].Split("=")[1]);
    var beaconY = int.Parse(split[9].Split("=")[1]);

    beacons.Add((sensorX, sensorY), (beaconX, beaconY));
    distances.Add((sensorX, sensorY), Math.Abs(sensorX - beaconX) + Math.Abs(sensorY - beaconY));
}

Console.WriteLine();

var yOfInterest = 2_000_000;
// yOfInterest = 10;

var count = 0;

var ranges = new List<(int min, int max)>();

foreach(var distance in distances)
{
    int x1, x2;

    x1 = distance.Key.x - (distance.Value - Math.Abs(distance.Key.y - yOfInterest));
    x2 = distance.Key.x + (distance.Value - Math.Abs(distance.Key.y - yOfInterest));
    
    ranges.Add((x1, x2));
}

var conglomerateRange = new List<int>();

foreach(var range in ranges)
{
    if (range.min <= range.max){
        conglomerateRange.AddRange(Enumerable.Range(range.min, range.max - range.min + 1));
    }
}

count = conglomerateRange.ToHashSet().Count() - beacons.Where(b => b.Value.y == yOfInterest).Select(v => v.Value).Distinct().Count();

Console.WriteLine(count);
Console.WriteLine($"{sw.ElapsedMilliseconds}ms");

sw.Reset();
sw.Start();

var maxXY = 4_000_000;
// var maxXY = 20;

foreach(var distance in distances)
{
    var xMin = distance.Key.x - distance.Value - 1;
    var xMax = distance.Key.x + distance.Value + 1;

    for (long x = Math.Max(0, xMin); x <= Math.Min(xMax, maxXY); x++)
    {
        long y1, y2;
        if (x < distance.Key.x)
        {
            y1 = distance.Key.y + (x - xMin);
            y2 = distance.Key.y + (xMin - x);
        }
        else
        {
            y1 = distance.Key.y + (x - xMax);
            y2 = distance.Key.y + (xMax - x);
        }
    
        y1 = Math.Min(Math.Max(y1, 0), maxXY);
        y2 = Math.Min(Math.Max(y2, 0), maxXY);        

        var testY1 = !distances.Any(d => Math.Abs(d.Key.x - x) + Math.Abs(d.Key.y - y1) <= d.Value);
        if (testY1)
        {
            Console.WriteLine($"x: {x}, y: {y1}, ans: {x * 4_000_000 + y1}");
            Console.WriteLine($"{sw.ElapsedMilliseconds}ms");
            return;
        }

        var testY2 = !distances.Any(d => Math.Abs(d.Key.x - x) + Math.Abs(d.Key.y - y2) <= d.Value);
        if (testY2)
        {
            Console.WriteLine($"x: {x}, y: {y2}, ans: {x * 4_000_000 + y2}");
            Console.WriteLine($"{sw.ElapsedMilliseconds}ms");
            return;
        }
    }
}