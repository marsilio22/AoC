using System.Diagnostics;
using System.Text;

var lines = File.ReadAllLines("./input");

var map = new Dictionary<(int x, int y), char>();

foreach(var line in lines)
{
    var split = line.Split(" -> ");

    for (int k = 0; k < split.Length - 1; k++)
    {
        var start = split[k].Split(",").Select(c => int.Parse(c)).ToArray();
        var end = split[k+1].Split(",").Select(c => int.Parse(c)).ToArray();

        var xdiff = end[0] - start[0];
        var ydiff = end[1] - start[1];

        if (xdiff < 0 || ydiff < 0)
        {
            var temp = end;
            end = start;
            start = temp;
        }

        for (int j = start[1]; j <= end[1]; j++)
        {
            for (int i = start[0]; i <= end[0]; i++)
            {
                map[(i, j)] = '#';
            }
        }
    }

}

// todo combine parts 1 and 2
(int x, int y) sandPoint = (500, 0);
var abyss = false;

var sw = new Stopwatch();
sw.Start();

while (!abyss)
{
    var currentSandX = sandPoint.x;
    var currentSandY = sandPoint.y;

    var atRest = false;

    while (!atRest)
    {
        var nextBottomPoints = map.Where(m => m.Key.y > currentSandY && m.Key.x == currentSandX);

        if (!nextBottomPoints.Any())
        {
            abyss = true;
            break;
        }

        var nextBottomPoint = nextBottomPoints.MinBy(m => m.Key.y);

        currentSandY = nextBottomPoint.Key.y - 1;
        
        if (!map.ContainsKey((nextBottomPoint.Key.x - 1, nextBottomPoint.Key.y)))
        {
            currentSandX--;
            currentSandY++;
        }
        else if(!map.ContainsKey((nextBottomPoint.Key.x + 1, nextBottomPoint.Key.y)))
        {
            currentSandX++;
            currentSandY++;
        }
        else 
        {
            map[(currentSandX, currentSandY)] = 'o';
            atRest = true;
        }
    }
}

// Print(map, "Part1");

var ans = map.Count(c => c.Value == 'o');

Console.WriteLine(ans);
Console.WriteLine(sw.ElapsedMilliseconds + "ms");
Console.WriteLine();

// continue for part 2

var maxY = map.Max(m => m.Key.y);

for (int i = 0; i < 1000; i++)
{
    map[(i, maxY + 2)] = '#';
}

while (!map.ContainsKey(sandPoint))
{
    var currentSandX = sandPoint.x;
    var currentSandY = sandPoint.y;

    var atRest = false;

    while (!atRest)
    {
        var nextBottomPoints = map.Where(m => m.Key.y > currentSandY && m.Key.x == currentSandX);
        var nextBottomPoint = nextBottomPoints.MinBy(m => m.Key.y);

        // todo optimise this
        ///////// hmm this would skip corners
        // if (nextBottomPoints.Any(m => m.Key == (nextBottomPoint.Key.x, nextBottomPoint.Key.y + 1)))
        // {
        //     // we are atop a stack of sand, so go diagonally left/right a long way instead of doing this step by step;
        //     //left y = -x
        //     var diagPoint = map.Where(m => m.Key.y == -1* m.Key.x + nextBottomPoint.Key.y + 1).MinBy(m => m.Key.y);
        //     if (diagPoint.Key.y != nextBottomPoint.Key.y)
        //     {
        //         nextBottomPoint = diagPoint;
        //     }
        //     else 
        //     {
        //         diagPoint = map.Where(m => m.Key.y == m.Key.x + nextBottomPoint.Key.y + 1).MinBy(m => m.Key.y);
        //         nextBottomPoint = diagPoint.Key.y != nextBottomPoint.Key.y ? diagPoint : nextBottomPoint;
        //     }
        // }

        currentSandY = nextBottomPoint.Key.y - 1;
        // currentSandX = nextBottomPoint.Key.x; // this wasn't necessary before because we couldn't move diagonally
        
        
        if (!map.ContainsKey((nextBottomPoint.Key.x - 1, nextBottomPoint.Key.y)))
        {
            currentSandX--;
            currentSandY++;
        }
        else if(!map.ContainsKey((nextBottomPoint.Key.x + 1, nextBottomPoint.Key.y)))
        {
            currentSandX++;
            currentSandY++;
        }
        else 
        {
            map[(currentSandX, currentSandY)] = 'o';
            atRest = true;
        }
    }
}

ans = map.Count(c => c.Value == 'o');
Console.WriteLine(ans);
Console.WriteLine(sw.ElapsedMilliseconds + "ms"); // ~515 seconds lol
Console.WriteLine();

// Print(map, "Part2");


static void Print(IDictionary<(int x, int y), char> coords, string fileName)
{
    var maxY = coords.Max(c => c.Key.y);
    var minY = coords.Min(c => c.Key.y);
    var minX = coords.Min(c => c.Key.x);
    var maxX = coords.Max(c => c.Key.x);

    var sb = new StringBuilder();

    for (int j = minY; j <= maxY; j++)
    {
        for (int i = minX; i <= maxX; i++)
        {
            if (coords.TryGetValue((i, j), out var c))
            {
                sb.Append(c);
            }
            else
            {
                sb.Append(' ');
            }
        }

        sb.AppendLine();
    }

    File.WriteAllText($"./{fileName}", sb.ToString());
}