var input = File.ReadAllLines("./input.txt");

var distances = new Dictionary<(string start, string end), int>();
var distinctPlaces = new HashSet<string>();


foreach (var line in input){
    var split = line.Split(" = ");

    var places = split[0].Split(" to ");
    var distance = int.Parse(split[1]);

    distinctPlaces.Add(places[0]);
    distinctPlaces.Add(places[1]);
    distances.Add((places[0], places[1]), distance);
}

var currShortest = int.MaxValue;
var currLongest = int.MinValue;

// could probs tidy this into a method by passing in a min max fn somehow, but cba
foreach(var place in distinctPlaces)
{
    var closestGraph = distinctPlaces.ToDictionary(d => d, _ => int.MaxValue);
    var furthestGraph = distinctPlaces.ToDictionary(d => d, _ => int.MinValue);

    var currentLocation = place;

    closestGraph[place] = 0;
    furthestGraph[place] = 0;

    while(closestGraph.Any(g => g.Value == int.MaxValue))
    {
        var closest = distances.Where(d => 
            (d.Key.start.Equals(currentLocation) || d.Key.end.Equals(currentLocation)) && 
            (closestGraph[d.Key.start] == int.MaxValue || closestGraph[d.Key.end] == int.MaxValue))
            .MinBy(c => c.Value);

        var dest = closest.Key.start.Equals(currentLocation) ? closest.Key.end : closest.Key.start;

        closestGraph[dest] = closest.Value;
        currentLocation = dest;
    }

    currentLocation = place;

    while(furthestGraph.Any(g => g.Value == int.MinValue))
    {
        var furthest = distances.Where(d => 
            (d.Key.start.Equals(currentLocation) || d.Key.end.Equals(currentLocation)) && 
            (furthestGraph[d.Key.start] == int.MinValue || furthestGraph[d.Key.end] == int.MinValue))
            .MaxBy(c => c.Value);

        var dest = furthest.Key.start.Equals(currentLocation) ? furthest.Key.end : furthest.Key.start;

        furthestGraph[dest] = furthest.Value;
        currentLocation = dest;
    }

    var smallDist = closestGraph.Values.Sum();
    var bigDist = furthestGraph.Values.Sum();

    if (smallDist < currShortest)
    {
        currShortest = smallDist;
    }

    if (bigDist > currLongest)
    {
        currLongest = bigDist;
    }
}

Console.WriteLine(currShortest);
Console.WriteLine(currLongest);