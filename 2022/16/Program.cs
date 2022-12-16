var lines = File.ReadAllLines("./input2");

IDictionary<string, Valve> valves = new Dictionary<string, Valve>();

foreach(var line in lines)
{
    var split = line.Split("; ");

    var val = new Valve();

    var splitFirst = split[0].Split(" ");

    val.Name = splitFirst[1];

    val.Flow = int.Parse(splitFirst[4].Split("=")[1]);

    val.Neighbours = split[1].Split(" ")[4..].Select(n => n.Split(",")[0]).ToList();

    valves.Add(val.Name, val);
}

foreach(var thing in valves)
{
    var algLocation = thing.Key;

    Dictionary<string, (int distance, List<string> path)> worths = 
        valves.ToDictionary(v => v.Key, v => (int.MaxValue, new List<string>()));
    var unvisited = valves.Keys.ToList();

    worths[algLocation] = (0, new List<string>());

    while(unvisited.Any())
    {
        algLocation = worths.Where(w => unvisited.Contains(w.Key)).MinBy(w => w.Value.distance).Key;
        var currentWorth = worths[algLocation];

        foreach(var neighbour in valves[algLocation].Neighbours)
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

var nonzerovalves = valves.Where(v => v.Value.Flow != 0).ToDictionary(k => k.Key, k => k.Value);

Console.WriteLine(Recurse(valves, "AA", 0, 30));
Console.WriteLine(Recurse2(valves, "AA", "AA", 0, 26));

int Recurse(IDictionary<string, Valve> valveMap, string current, int score, int remainingTime)
{
    var unopened = valveMap.Where(v => v.Value.Flow > 0 && !v.Value.Open && v.Value.Distances[current].distance + 1 < remainingTime).ToList();
    var scorePerTime = valveMap.Values.Where(v => v.Open).Sum(v => v.Flow);

    if (!unopened.Any())
    {
        return score + remainingTime * scorePerTime;
    }

    var best = 0;

    foreach(var valve in unopened)
    {
        var d = valve.Value.Distances[current];
        var s = score + (d.distance + 1) * scorePerTime;
        var remaining = remainingTime - (d.distance + 1);

        var newMap = valveMap.ToDictionary(v => v.Key, v => v.Value with {});
        newMap[valve.Key].Open = true;

        best = Math.Max(Recurse(newMap, valve.Key, s, remaining), best);
    }

    return best;
}

int Recurse2(IDictionary<string, Valve> valveMap, string currentMe, string currentEle, int score, int remainingTime)
{
    var unopened = valveMap.Where(v => v.Value.Flow > 0 && !v.Value.Open && (v.Value.Distances[currentMe].distance + 1 < remainingTime || v.Value.Distances[currentEle].distance + 1 < remainingTime)).ToList();
    var scorePerTime = valveMap.Values.Where(v => v.Open).Sum(v => v.Flow);

    if (!unopened.Any())
    {
        return score + remainingTime * scorePerTime;
    }

    var best = 0;

    foreach(var meValve in unopened)
    {
        foreach(var eleValve in unopened)
        {
            if (meValve.Key == eleValve.Key)
            {
                continue;
            }

            // todo
            // figure out whether me or the elephant is closest to their valve
            // move to that valve, and the path location which is that far along for the other person
            // recurse with those new locations/score
            // hope for the best

            var dMe = meValve.Value.Distances[currentMe].distance;
            var dEle = eleValve.Value.Distances[currentEle].distance;

            var winner = Math.Min(dMe, dEle);

            var mePath = meValve.Value.Distances[currentMe].path;
            var elePath = eleValve.Value.Distances[currentEle].path;

            string mePos, elePos;

            if (winner != 0)
            {
                mePos = dMe == winner ? meValve.Key : mePath[mePath.Count - (winner)];
                elePos = dEle == winner ? eleValve.Key : elePath[elePath.Count - (winner)];
            }
            else
            {
                mePos = currentMe;
                elePos = currentEle;
            }

            // var mePos = meValve.Value.Distances[currentMe].path[Math.Min(dMe-1, Math.Max(winner-1, 0))]; // + 1???
            // var elePos = eleValve.Value.Distances[currentEle].path[Math.Min(dEle-1, Math.Max(winner-1, 0))]; // might end up out of bounds or something

            var s = score + (winner + 1) * scorePerTime;
            var remaining = remainingTime - (winner + 1);

            var newMap = valveMap.ToDictionary(v => v.Key, v => v.Value with {});

            if (mePos.Equals(meValve.Key))
            {
                newMap[meValve.Key].Open = true;
            }
            else 
            {
                // move one more on, because the elephant will open it's valve
                mePos = mePath[mePath.Count - winner - 1];
            }

            if (elePos.Equals(eleValve.Key))
            {
                newMap[eleValve.Key].Open = true;
            }
            else 
            {
                // move one more on, because me will open me's valve
                elePos = elePath[elePath.Count - winner - 1];
            }

            best = Math.Max(Recurse2(newMap, mePos, elePos, s, remaining), best);
        }
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




record Valve {
    public string Name { get; set; }

    public int Flow { get; set; }

    public ICollection<string> Neighbours { get; set; }

    public bool Open { get; set; } = false;

    public IDictionary<string, (int distance, List<string> path)> Distances { get; set; }
}