var lines = File.ReadAllLines("./input.txt");

IDictionary < int, List < (int x, int y, int z) >> scanners = new Dictionary < int, List < (int, int, int) >>();
int currentScanner = 0;
foreach (var line in lines)
{
    if (line.Length == 0)
    {
        continue;
    }

    if (line.Contains("scanner"))
    {
        var num = int.Parse(line.Split(" ")[2]);
        scanners[num] = new List < (int, int, int) > ();
        currentScanner = num;
    }
    else
    {
        var split = line.Split(",");
        scanners[currentScanner].Add((int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2])));
    }
}

IDictionary<int, List<Beacon>> beaconsByScanner = scanners.ToDictionary(k => k.Key, k => k.Value.Select(v => new Beacon{x = v.x, y = v.y, z = v.z}).ToList());

foreach (var scanner in scanners)
{
    for (int i = 0; i < scanner.Value.Count; i++)
    {
        for (int j = 0; j < scanner.Value.Count; j++)
        {
            if (j == i)
            {
                continue;
            }

            var p1 = scanner.Value[i];
            var p2 = scanner.Value[j];

            var distance =
                Math.Pow(p1.x - p2.x, 2) +
                Math.Pow(p1.y - p2.y, 2) +
                Math.Pow(p1.z - p2.z, 2);

            beaconsByScanner[scanner.Key][i].distancesToOtherBeaconsInTheSameScanner.Add((int)distance);
        }
    }
}

Console.WriteLine();

var allOverlaps = new Dictionary<(int s1, int s2), Dictionary<Beacon, Beacon>>();

for (int i = 0; i < beaconsByScanner.Count; i++)
{
    for (int j = i+1; j < beaconsByScanner.Count; j++)
    {
        var ijPotentialOverlap = new Dictionary<Beacon, Beacon>();

        foreach(var beacon in beaconsByScanner[i])
        {
            var overlaps = beaconsByScanner[j].Where(b => b.distancesToOtherBeaconsInTheSameScanner.Intersect(beacon.distancesToOtherBeaconsInTheSameScanner).Count() > 1).ToList();
            if (overlaps.Count() > 0)
            {
                ijPotentialOverlap.Add(beacon, overlaps.Single());
            }
        }

        Console.WriteLine($"Between scanner {i} and scanner {j} there were {ijPotentialOverlap.Count()} potential overlapping points");

        allOverlaps.Add((i, j), ijPotentialOverlap);
    }
}

// hmm this doesn't work because some of the overlapping points overlap between multiple pairs
// var thing = scanners.Values.SelectMany(v => v).Count() - test.Values.Sum();

// try again with proper info about which beacons overlap

Console.WriteLine();

var test = scanners.ToDictionary(k => k.Key, k => k.Value);

foreach(var scannerPair in allOverlaps)
{
    foreach(var beaconWithOverlaps in scannerPair.Value)
    {
        test[scannerPair.Key.s2].Remove((beaconWithOverlaps.Value.x, beaconWithOverlaps.Value.y, beaconWithOverlaps.Value.z));
    }
}

Console.WriteLine(test.Values.SelectMany(v => v).Count());




class Beacon
{
    public int x { get; set; }
    public int y { get; set; }
    public int z { get; set; }

    public ICollection<int> distancesToOtherBeaconsInTheSameScanner {get;set;} = new List<int>();

    public override string ToString()
    {
        return $"({x}, {y}, {z})";
    }
}