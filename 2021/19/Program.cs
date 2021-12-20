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

IDictionary<int, List<Beacon>> beaconsByScanner = scanners.ToDictionary(k => k.Key, k => k.Value.Select(v => new Beacon { x = v.x, y = v.y, z = v.z }).ToList());

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

var allOverlaps = new Dictionary < (int s1, int s2),
    List < (Beacon b1, Beacon b2) >>();

for (int i = 0; i < beaconsByScanner.Count; i++)
{
    for (int j = i + 1; j < beaconsByScanner.Count; j++)
    {
        List < (Beacon b1, Beacon b2) > ijPotentialOverlap = new List < (Beacon, Beacon) > ();

        foreach (var beacon in beaconsByScanner[i])
        {
            var overlaps = beaconsByScanner[j].Where(b => b.distancesToOtherBeaconsInTheSameScanner.Intersect(beacon.distancesToOtherBeaconsInTheSameScanner).Count() > 1).ToList();
            if (overlaps.Count() > 0)
            {
                ijPotentialOverlap.Add((beacon, overlaps.Single()));
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

var distinctPoints = scanners.ToDictionary(k => k.Key, k => k.Value);

foreach (var pair in allOverlaps)
{
    foreach (var beaconWithOverlaps in pair.Value)
    {
        distinctPoints[pair.Key.s2].Remove((beaconWithOverlaps.b2.x, beaconWithOverlaps.b2.y, beaconWithOverlaps.b2.z));
    }
}

Console.WriteLine(distinctPoints.Values.SelectMany(v => v).Count());

var actualOverlappingScanners = allOverlaps.Where(v => v.Value.Count > 11).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

Console.WriteLine();

Dictionary < int, (int x, int y, int z) > scannerLocations = new Dictionary < int, (int x, int y, int z) > { { 0, (0, 0, 0) } };

while (actualOverlappingScanners.Any(s => scannerLocations.ContainsKey(s.Key.s1) && !scannerLocations.ContainsKey(s.Key.s2) ||
        !scannerLocations.ContainsKey(s.Key.s1) && scannerLocations.ContainsKey(s.Key.s2)))
{
    var scannerPair = actualOverlappingScanners.First(s => scannerLocations.ContainsKey(s.Key.s1) && !scannerLocations.ContainsKey(s.Key.s2) ||
        !scannerLocations.ContainsKey(s.Key.s1) && scannerLocations.ContainsKey(s.Key.s2));

    // here known and unknown are in relation to the scanner's location
    var knownScannerPoints = scannerLocations.ContainsKey(scannerPair.Key.s1) ? scannerPair.Value.Select(v => v.b1).ToList() : scannerPair.Value.Select(v => v.b2).ToList();
    var unknownScannerPoints = scannerLocations.ContainsKey(scannerPair.Key.s1) ? scannerPair.Value.Select(v => v.b2).ToList() : scannerPair.Value.Select(v => v.b1).ToList();

    var unknownKey = scannerLocations.ContainsKey(scannerPair.Key.s1) ? scannerPair.Key.s2 : scannerPair.Key.s1;

    var knownXdiff1 = knownScannerPoints[0].x - knownScannerPoints[1].x;
    var knownYdiff1 = knownScannerPoints[0].y - knownScannerPoints[1].y;
    var knownZdiff1 = knownScannerPoints[0].z - knownScannerPoints[1].z;

    for (int i = 0; i < 8; i++) // why 8 though, that's too many no?
    {
        Func < (int x, int y, int z), (int x, int y, int z) > func;
        switch (i)
        {
            case 0:
                func = a => (a.x, a.y, a.z);
                break;
            case 1:
                func = a => (-1 * a.x, a.y, a.z);
                break;
            case 2:
                func = a => (a.x, -1 * a.y, a.z);
                break;
            case 3:
                func = a => (a.x, a.y, -1 * a.z);
                break;
            case 4:
                func = a => (-1 * a.x, a.y, -1 * a.z);
                break;
            case 5:
                func = a => (a.x, -1 * a.y, -1 * a.z);
                break;
            case 6:
                func = a => (-1 * a.x, -1 * a.y, a.z);
                break;
            case 7:
                func = a => (-1 * a.x, -1 * a.y, -1 * a.z);
                break;
            default:
                throw new Exception("oh noes");
        }

        for (int j = 0; j < 6; j++)
        {
            List < (int x, int y, int z) > transformedPoints;
            Func<(int x, int y, int z), (int x, int y, int z)> func2;
            switch (j)
            {
                case 0:
                    func2 = a => (a.x, a.y, a.z);

                    break;
                case 1:
                    func2 = a => (a.x, a.z, a.y);
                    break;
                case 2:
                    func2 = a => (a.y, a.x, a.z);
                    break;
                case 3:
                    func2 = a => (a.y, a.z, a.x);
                    break;
                case 4:
                    func2 = a => (a.z, a.x, a.y);
                    break;
                case 5:
                    func2 = a => (a.z, a.y, a.x);
                    break;
                default:
                    throw new Exception("oh noes");
            }
            
            transformedPoints = unknownScannerPoints.Select(u => func(func2((u.x, u.y, u.z)))).ToList();

            var unknownXdiff1 = transformedPoints[0].x - transformedPoints[1].x;
            var unknownYdiff1 = transformedPoints[0].y - transformedPoints[1].y;
            var unknownZdiff1 = transformedPoints[0].z - transformedPoints[1].z;

            if (unknownXdiff1 == knownXdiff1 && unknownYdiff1 == knownYdiff1 && unknownZdiff1 == knownZdiff1)
            {
                Console.WriteLine($"i {i}, j {j} between scanners {scannerPair.Key.s1} and {scannerPair.Key.s2}");

                // need to replace the existing set of matched locations by their rotated coords wrt the known scanner
                if (scannerLocations.ContainsKey(scannerPair.Key.s1))
                {
                    int k = 0;
                    var list = scannerPair.Value;

                    foreach (var thing in knownScannerPoints)
                    {
                        var tup = list[k];
                        list.Remove(tup);
                        tup.b2 = new Beacon { x = transformedPoints[k].x, y = transformedPoints[k].y, z = transformedPoints[k].z };
                        list.Add(tup);
                        k++;
                    }
                }
                else
                {
                    int k = 0;
                    var list = scannerPair.Value;
                    foreach (var thing in unknownScannerPoints)
                    {
                        var tup = list[k];
                        list.Remove(tup);
                        tup.b1 = new Beacon { x = transformedPoints[k].x, y = transformedPoints[k].y, z = transformedPoints[k].z };
                        list.Add(tup);
                        k++;
                    }
                }

                (int x, int y, int z)relativeScannerPos =
                    (knownScannerPoints[0].x - transformedPoints[0].x,
                        knownScannerPoints[0].y - transformedPoints[0].y,
                        knownScannerPoints[0].z - transformedPoints[0].z);

                if (scannerLocations.TryGetValue(scannerPair.Key.s1, out var s1pos))
                {
                    scannerLocations[scannerPair.Key.s2] = (s1pos.x + relativeScannerPos.x, s1pos.y + relativeScannerPos.y, s1pos.z + relativeScannerPos.z);
                }
                else
                {
                    var s2pos = scannerLocations[scannerPair.Key.s2];
                    scannerLocations[scannerPair.Key.s1] = (s2pos.x + relativeScannerPos.x, s2pos.y + relativeScannerPos.y, s2pos.z + relativeScannerPos.z);
                }

                // need to ALSO replace any reference to the rotated scanner in any other actual overlaps!!!
                var test = actualOverlappingScanners.Where(s => s.Key.s1 == unknownKey && !scannerLocations.ContainsKey(s.Key.s2) ||
                    s.Key.s2 == unknownKey && !scannerLocations.ContainsKey(s.Key.s1)).ToList();

                // so. for each of those, transform and replace either the b1 or b2 value, depending which way round they are

                foreach (var overlapSet in test)
                {
                    // figure out which one we're changing
                    if (overlapSet.Key.s1 == unknownKey)
                    {
                        // transform b1s
                        var newList = new List<(Beacon, Beacon)>();

                        foreach(var tup in overlapSet.Value)
                        {
                            var newTup = func(func2((tup.b1.x, tup.b1.y, tup.b1.z)));
                            newList.Add((new Beacon{x = newTup.x, y = newTup.y, z = newTup.z}, tup.b2));
                        }

                        actualOverlappingScanners[overlapSet.Key] = newList;
                    }
                    else
                    {
                        // transform b2s
                        var newList = new List<(Beacon, Beacon)>();

                        foreach(var tup in overlapSet.Value)
                        {
                            var newTup = func(func2((tup.b2.x, tup.b2.y, tup.b2.z)));
                            newList.Add((tup.b1, new Beacon{x = newTup.x, y = newTup.y, z = newTup.z}));
                        }
                        
                        actualOverlappingScanners[overlapSet.Key] = newList;
                    }
                }

                //lol cba
                i = 27;
                j = 27;
                break;
            }
        }
    }

}

var maxDistance = int.MinValue;

for (int i = 0; i < scannerLocations.Count; i++)
{
    for (int j = i + 1; j < scannerLocations.Count; j++)
    {
        var distance = Math.Abs(scannerLocations[i].x - scannerLocations[j].x) +
            Math.Abs(scannerLocations[i].y - scannerLocations[j].y) +
            Math.Abs(scannerLocations[i].z - scannerLocations[j].z);
        if (distance > maxDistance)
        {
            maxDistance = distance;
        }
    }
}

Console.WriteLine(maxDistance);

class Beacon
{
    public int x { get; set; }
    public int y { get; set; }
    public int z { get; set; }

    public ICollection<int> distancesToOtherBeaconsInTheSameScanner { get; set; } = new List<int>();

    public override string ToString()
    {
        return $"({x}, {y}, {z})";
    }
}