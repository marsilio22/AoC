using System.Diagnostics.CodeAnalysis;

var input = File.ReadAllLines("./input.txt");

var lines = new List<string[]>();
foreach (var line in input)
{
    var newLine = line.Replace(" can fly ", ",");
    newLine = newLine.Replace(" km/s for ", ",");
    newLine = newLine.Replace(" seconds, but then must rest for ", ",");
    newLine = newLine.Replace(" seconds.", "");

    lines.Add(newLine.Split(",").ToArray());
}

var totalTime = 2503;

ICollection<(string name, int vel, int runTime, int restTime)> reindeer = lines.Select(l => (l[0], int.Parse(l[1]), int.Parse(l[2]), int.Parse(l[3]))).ToList();

IDictionary<string, (int distance, int points)> race = lines.ToDictionary(l => l[0], _ => (0, 0));

for (int i = 0; i < totalTime; i++)
{
    foreach(var rein in reindeer)
    {
        var isrunning = (i % (rein.runTime + rein.restTime)) < rein.runTime;
        race[rein.name] = (race[rein.name].distance + (isrunning ? rein.vel : 0), race[rein.name].points);
    }

    var firstPlaces = race.Where(r => r.Value.distance == race.MaxBy(r => r.Value.distance).Value.distance).ToList();

    foreach(var firstPlace in firstPlaces)
    {
        race[firstPlace.Key] = (firstPlace.Value.distance, firstPlace.Value.points + 1);
    }
}

var winnerPoints = race.MaxBy(r => r.Value.points).Value.points;
var winnerDistance = race.MaxBy(r => r.Value.distance).Value.distance;
Console.WriteLine("the biggest distance was " + winnerDistance);
Console.WriteLine("the biggest points was " + winnerPoints);
