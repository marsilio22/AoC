var lines = File.ReadAllLines("./input");

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
//var yOfInterest = 10;

var maxX = distances.Keys.Max(k => k.x);
var minX = distances.Keys.Min(k => k.x);
var maxDist = distances.Values.Max();

distances = distances.OrderBy(o => Math.Abs(yOfInterest - o.Key.y)).ToDictionary(k => k.Key, k => k.Value);
var count = 0;

// todo can speed this up easy using the part2 method
for (int i = minX - maxDist; i < maxX + maxDist;)
{
    var coverers = distances.Where(d => Math.Abs(d.Key.x - i) + Math.Abs(d.Key.y - yOfInterest) <= d.Value).ToArray();
    
    if (!coverers.Any())
    {
        i++; 
    }

    while (coverers.Any())
    {
        i++;
        count++; // no beacon can exist on a spot covered by another sensor
        coverers = distances.Where(d => Math.Abs(d.Key.x - i) + Math.Abs(d.Key.y - yOfInterest) < d.Value).ToArray();
    }
}

count = count - beacons.Where(b => b.Value.y == yOfInterest).Select(v => v.Value).Distinct().Count();

Console.WriteLine(count);

var coordsToCheck = new HashSet<(int x, int y)>();

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

        var testY1= distances.Where(d => Math.Abs(d.Key.x - x) + Math.Abs(d.Key.y - y1) <= d.Value);
        var testY2 = distances.Where(d => Math.Abs(d.Key.x - x) + Math.Abs(d.Key.y - y2) <= d.Value);
        if (!testY1.Any())
        {
            Console.WriteLine($"x: {x}, y: {y1}, ans: {x * 4_000_000 + y1}");
            return;
        }
        else if (!testY2.Any())
        {
            Console.WriteLine($"x: {x}, y: {y2}, ans: {x * 4_000_000 + y2}");
            return;
        }
    }
}

Console.WriteLine(); // 849139071 too low 
// 849139072 too low