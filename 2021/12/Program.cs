using System.Diagnostics;

var lines = File.ReadAllLines ("./input.txt");

var network = new Dictionary<string, Node>();

foreach (var line in lines)
{
    var splitLine = line.Split("-");

    if (!network.TryGetValue(splitLine[0], out Node value))
    {
        value = new Node(splitLine[0]);
        network[splitLine[0]] = value;
    }

    value.Adjacents.Add(splitLine[1]);

    if (!network.TryGetValue(splitLine[1], out value))
    {
        value = new Node(splitLine[1]);
        network[splitLine[1]] = value;
    }

    value.Adjacents.Add(splitLine[0]);
}

Console.WriteLine();

var start = network["start"];
var end = network["end"];

var lastThreeResults = new Queue<int>();

lastThreeResults.Enqueue(0);
lastThreeResults.Enqueue(0);
lastThreeResults.Enqueue(0);

var r = new Random();
var listOfPaths = new HashSet<string>();
var t = 1d;

// it's not good. it's not pretty. but it ***is*** fun.
while(true)
{
    Console.WriteLine($"starting a run of {t} second(s)");

    var timer = new Stopwatch();
    timer.Start();
    while(timer.Elapsed < TimeSpan.FromSeconds(t))
    {
        var path = new List<string>{"start"};
        var current = network["start"];
        var visitedASmallCaveTwice = false;
        while (current != end)
        {
            current.VisitCount++;

            var validNext = network.Where(n => 
                    n.Value.Name != "start" && 
                    current.Adjacents.Contains(n.Key) && 
                    (n.Value.IsBig || !visitedASmallCaveTwice || !path.Contains(n.Value.Name))
                ).ToList();

            if (!validNext.Any())
            {
                // Console.WriteLine("Found a dead end");
                break;
            }
            else 
            {
                Node next = validNext[r.Next(validNext.Count())].Value; 

                if (!next.IsBig && path.Contains(next.Name))
                {
                    visitedASmallCaveTwice = true;
                }

                path.Add(next.Name);
                current = next;
            }
        }

        listOfPaths.Add(string.Join(",", path));
        // Console.WriteLine(string.Join(",", path));
    }

    Console.WriteLine(listOfPaths.Count(p => p.StartsWith("start") && p.EndsWith("end"))); //146548 too low
    if (lastThreeResults.All(r => r == listOfPaths.Count(p => p.StartsWith("start") && p.EndsWith("end"))))
    {
        Console.WriteLine($"potential solution {lastThreeResults.Peek()}, got it four times in a row");
        break;
    }
    else
    {
        t = Math.Min(t * 2, 2048);
    }

    lastThreeResults.Dequeue();
    lastThreeResults.Enqueue(listOfPaths.Count(p => p.StartsWith("start") && p.EndsWith("end")));
}

public class Node
{
    public bool IsBig { get; set; }

    public HashSet<string> Adjacents { get; set; }

    public string Name {get;set;}

    public int VisitCount {get;set;}

    public Node (string name)
    {
        this.Name = name;
        this.Adjacents = new HashSet<string>();

        if (name != "start" && name != "end" && name[0] <= 90)
        {
            IsBig = true;
        }
    }
}