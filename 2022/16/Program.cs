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

            //if (new (int, int)[]
            //        { (20, 23), (61, 22), (184, 19), (260, 18), (336, 17), (414, 16), (492, 15), (573, 14) }
            //    .Contains((s, remaining)))
            //{
            //    Console.WriteLine();
            //}

            //if (new (int, int)[]
            //        { (20, 22), (61, 21), (184, 18), (260, 17), (336, 16), (414, 15), (492, 14), (573, 13) }
            //    .Contains((s, remaining)))
            //{
            //    Console.WriteLine();
            //}

            //if (new[]
            //    {
            //        //(23, 20),
            //        //(22, 61),
            //        //(21, 102),
            //        //(20, 143),
            //        //(19, 184),
            //        //(18, 260),
            //        (17, 336),
            //        (16, 414),
            //        (15, 492),
            //        (14, 573),
            //        (13, 654),
            //        (12, 735),
            //        (11, 816),
            //        (10, 897),
            //        (9, 978),
            //        (8, 1059),
            //        (7, 1140),
            //        (6, 1221),
            //        (5, 1302),
            //        (4, 1383),
            //        (3, 1464),
            //        (2, 1545),
            //        (1, 1626),
            //        (0, 1707)
            //    }.Contains((remaining, s)))
            //{
            //    Console.WriteLine();
            //}

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

// var time = 0;
// var currentLocation = "AA";

// var ans = 0;

// while (time < 30)
// {
//     var algLocation = currentLocation;
//     Dictionary<string, (int distance, double worth, string firstStep)> worths = 
//         valves.ToDictionary(v => v.Key, v => (int.MaxValue, 0d, string.Empty));
//     var unvisited = valves.Keys.ToList();

//     worths[algLocation] = 
//     (   0,
//         (valves[algLocation].Open) ? 
//             0 : 
//             valves[algLocation].Flow * (30 - (time + 1)), 
//         string.Empty
//     );

//     while(unvisited.Any())
//     {
//         algLocation = worths.Where(w => unvisited.Contains(w.Key)).MinBy(w => w.Value.distance).Key;
//         var currentWorth = worths[algLocation];

//         foreach(var neighbour in valves[algLocation].Neighbours)
//         {
//             var val = worths[neighbour];

//             if (val.distance > currentWorth.distance + 1)
//             {
//                 val = 
//                 (
//                     currentWorth.distance + 1, 
//                     // punish distance a little more than we should so we don't try and go too too far away
//                     valves[neighbour].Open ? 0 : ((double)valves[neighbour].Flow / (currentWorth.distance * punishment + 1)) * (30 - (time + currentWorth.distance + 2)), // +2 for the extra minute it takes to turn the valve
//                     worths[algLocation].distance == 0 ? neighbour : worths[algLocation].firstStep
//                 );

//                 worths[neighbour] = val;
//             }
//         }

//         unvisited.Remove(algLocation);
//     }

//     var best = worths.MaxBy(w => w.Value.worth);
//     var best2 = worths.Where(w => w.Value.worth > 0).OrderBy(w => w.Value.distance).ThenByDescending(w => w.Value.worth).FirstOrDefault();
//     var best3 = worths.OrderByDescending(w => w.Value.worth).Take(3).MaxBy(w => w.Value.distance);

//     if (best.Value.distance + time < 30){
//         Console.Write(best.Key + ", ");
//         currentLocation = best.Key;
//         time += best.Value.distance + 1;
//         ans += (best.Value.distance + 1) * valves.Values.Where(v => v.Open).Sum(v => v.Flow);
//         valves[currentLocation].Open = true;
//     }
//     else 
//     {
//         while (time < 30)
//         {
//             time++;
//             ans +=  valves.Values.Where(v => v.Open).Sum(v => v.Flow);
//         }
//     }
// }

// Console.WriteLine(ans); // 1870 too high // 1768 too low // 1769 too low

record Valve
{
    public string Name { get; set; }

    public int Flow { get; set; }

    public ICollection<string> Neighbours { get; set; }

    public bool Open { get; set; } = false;

    public IDictionary<string, (int distance, List<string> path)> Distances { get; set; }
}
