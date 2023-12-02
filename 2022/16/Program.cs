var lines = File.ReadAllLines("./input");

IDictionary<string, Valve> valves = new Dictionary<string, Valve>();

foreach (var line in lines)
{
    var split = line.Split("; ");

    var val = new Valve();

    var splitFirst = split[0].Split(" ");

    val.Name = splitFirst[1];

    val.Flow = int.Parse(splitFirst[4].Split("=")[1]);

    val.Neighbours = split[1].Split(" ")[4..].Select(n => n.Split(",")[0]).ToList();

    valves.Add(val.Name, val);
}

foreach (var thing in valves)
{
    var algLocation = thing.Key;

    Dictionary<string, (int distance, List<string> path)> worths =
        valves.ToDictionary(v => v.Key, v => (int.MaxValue, new List<string>()));
    var unvisited = valves.Keys.ToList();

    worths[algLocation] = (0, new List<string>());

    while (unvisited.Any())
    {
        algLocation = worths.Where(w => unvisited.Contains(w.Key)).MinBy(w => w.Value.distance).Key;
        var currentWorth = worths[algLocation];

        foreach (var neighbour in valves[algLocation].Neighbours)
        {
            var val = worths[neighbour];

            if (val.distance > currentWorth.distance + 1)
            {
                val = (currentWorth.distance + 1, currentWorth.path.Append(neighbour).ToList());
                worths[neighbour] = val;
            }
        }

        unvisited.Remove(algLocation);
    }

    thing.Value.Distances = worths;
}

int _best = 0;

Console.WriteLine(Recurse(valves, "AA", 0, 30));
Console.WriteLine(Recurse2(valves, "AA", "AA", "", "", 0, 26));

int Recurse(IDictionary<string, Valve> valveMap, string current, int score, int remainingTime)
{
    var unopened = valveMap.Where(v => v.Value.Flow > 0 && !v.Value.Open && v.Value.Distances[current].distance + 1 < remainingTime).ToList();
    var scorePerTime = valveMap.Values.Where(v => v.Open).Sum(v => v.Flow);

    if (!unopened.Any())
    {
        return score + remainingTime * scorePerTime;
    }

    var best = 0;

    foreach (var valve in unopened)
    {
        var d = valve.Value.Distances[current];
        var s = score + (d.distance + 1) * scorePerTime;
        var remaining = remainingTime - (d.distance + 1);

        var newMap = valveMap.ToDictionary(v => v.Key, v => v.Value with { });
        newMap[valve.Key].Open = true;

        best = Math.Max(Recurse(newMap, valve.Key, s, remaining), best);
    }

    return best;
}

int Recurse2(IDictionary<string, Valve> valveMap, string currentMe, string currentEle, string currentMeTarget, string currentEleTarget, int score, int remainingTime)
{
    var unopened = valveMap.Where(v => v.Value.Flow > 0 && !v.Value.Open && (v.Value.Distances[currentMe].distance + 1 < remainingTime || v.Value.Distances[currentEle].distance + 1 < remainingTime)).OrderByDescending(d => d.Value.Flow).ToList();
    var scorePerTime = valveMap.Values.Where(v => v.Open).Sum(v => v.Flow);

    if (!unopened.Any())
    {
        var b = score + remainingTime * scorePerTime;
        return b;
    }

    var best = 0;

    var meMax = string.IsNullOrEmpty(currentMeTarget) ? unopened.Count : 1;
    var eleMax = string.IsNullOrEmpty(currentEleTarget) ? unopened.Count : 1;

    for (int me = 0; me < meMax; me++)
    {
        var meValve = string.IsNullOrEmpty(currentMeTarget) ? unopened[me] : valveMap.Single(v => v.Key.Equals(currentMeTarget));
        for (int ele = 0; ele < eleMax; ele++)
        {
            var eleValve = string.IsNullOrEmpty(currentEleTarget) ? unopened[ele] : valveMap.Single(v => v.Key.Equals(currentEleTarget));

            if (meValve.Key.Equals(eleValve.Key) && meMax != 1)
            {
                continue;
            }

            var dMe = meValve.Value.Distances[currentMe].distance;
            var dEle = eleValve.Value.Distances[currentEle].distance;

            var winner = Math.Min(dMe, dEle);

            var mePath = valveMap[currentMe].Distances[meValve.Key].path;
            var elePath = valveMap[currentEle].Distances[eleValve.Key].path;

            string mePos, elePos;

            if (winner != 0)
            {
                mePos = dMe == winner ? meValve.Key : mePath[winner - 1];
                elePos = dEle == winner ? eleValve.Key : elePath[winner - 1];
            }
            else
            {
                mePos = currentMe;
                elePos = currentEle;
            }

            var s = score + (winner + 1) * scorePerTime;
            var remaining = remainingTime - (winner + 1);

            var newMap = valveMap.ToDictionary(v => v.Key, v => v.Value with { });

            string meTarget = string.Empty;
            string eleTarget = string.Empty;

            if (mePos.Equals(meValve.Key))
            {
                newMap[meValve.Key].Open = true;
            }
            else
            {
                // move one more on, because the elephant will open it's valve
                mePos = mePath[(winner)];
                meTarget = meValve.Key;
            }

            if (elePos.Equals(eleValve.Key))
            {
                newMap[eleValve.Key].Open = true;
            }
            else
            {
                // move one more on, because me will open me's valve
                elePos = elePath[(winner)];
                eleTarget = eleValve.Key;
            }

            best = Math.Max(Recurse2(newMap, mePos, elePos, meTarget, eleTarget, s, remaining), best);
        }
    }

    if (best > _best)
    {
        _best = best;
        Console.WriteLine(best); // 2085 too low // 2114 too low // 2351 correct
    }

    return best;
}

record Valve
{
    public string Name { get; set; }

    public int Flow { get; set; }

    public ICollection<string> Neighbours { get; set; }

    public bool Open { get; set; } = false;

    public IDictionary<string, (int distance, List<string> path)> Distances { get; set; }
}
