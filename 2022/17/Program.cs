var jets = File.ReadAllText("./input");

var map = new Dictionary<(int x, int y), char>();

for (int i = 0; i < 7; i++)
{
    map.Add((i, 0), '#');
}

var rocks = new Queue<Rock>();

// ####
rocks.Enqueue(new Rock
    {
        Type = 1,
        Shape = new HashSet<(int x, int y)> { (0, 0), (1, 0), (2, 0), (3, 0) },
        Lefts = new HashSet<(int x, int y)> { (-1, 0) },
        Rights = new HashSet<(int x, int y)>  { (4, 0) },
        Belows = new HashSet<(int x, int y)>  { (0, -1), (1, -1), (2, -1), (3, -1) }
    }
);

// .#.
// ###
// .#.
rocks.Enqueue(new Rock
    {
        Type = 2,
        Shape = new HashSet<(int x, int y)> { (1, 0), (0, 1), (1, 1), (2, 1), (1, 2) },
        Lefts = new HashSet<(int x, int y)> { (0, 2), (-1, 1), (0, 0) }, 
        Rights = new HashSet<(int x, int y)> { (2, 0), (3, 1), (2, 2) },
        Belows = new HashSet<(int x, int y)> { (0, 0), (1, -1), (2, 0) }
    }
);

// ..#
// ..#
// ###
rocks.Enqueue(new Rock
    {
        Type = 3,
        Shape = new HashSet<(int x, int y)> { (0, 0), (1, 0), (2, 0), (2, 1), (2, 2) },
        Lefts = new HashSet<(int x, int y)> { (-1, 0), (1, 1), (1, 2) },
        Rights = new HashSet<(int x, int y)> { (3, 0), (3, 1), (3, 2) },
        Belows = new HashSet<(int x, int y)> { (0, -1), (1, -1), (2, -1) }
    }
);

// #
// #
// #
// #
rocks.Enqueue(new Rock
    {
        Type = 4,
        Shape = new HashSet<(int x, int y)> { (0, 0), (0, 1), (0, 2), (0, 3) },
        Lefts = new HashSet<(int x, int y)> { (-1, 0), (-1, 1), (-1, 2), (-1, 3) },
        Rights = new HashSet<(int x, int y)> { (1, 0), (1, 1), (1, 2), (1, 3) },
        Belows = new HashSet<(int x, int y)> { (0, -1) }
    }
);

// ##
// ##
rocks.Enqueue(new Rock
    {
        Type = 5,
        Shape = new HashSet<(int x, int y)> { (0, 0), (0, 1), (1, 0), (1, 1) },
        Lefts = new HashSet<(int x, int y)> { (-1, 0), (-1, 1) },
        Rights = new HashSet<(int x, int y)> { (2, 0), (2, 1) },
        Belows = new HashSet<(int x, int y)> { (0, -1), (1, -1) }
    }
);

var count = 0;
var maxY = 0;
var jetOffset = 0;

var previousHeights = new Dictionary<int, int>();
var previousStates = new HashSet<(int, int)>();

var cycleStart=0;
var cycleLength = 0;

var previousCycleStates = new HashSet<Dictionary<(int x, int y), char>>();

while (count < 10000)
{
    //PrintArea(map, maxY);
    (int x, int y) newRockPos = (2, maxY+4);

    previousHeights.Add(count, maxY);

    var rock = rocks.Dequeue();
    rocks.Enqueue(rock);

    while(true)
    {
        // push rock
        var jet = jets[jetOffset];

        if (jetOffset == 0 && cycleLength == 0)
        {
            foreach(var rockNode in rock.Shape)
            {
                map.Add((newRockPos.x + rockNode.x, newRockPos.y + rockNode.y), '#');
            }

            PrintArea(map, newRockPos.y + 4);

            var top20rows = map.Where(m => m.Key.y >= maxY - 20).ToDictionary(k => (k.Key.x, k.Key.y - maxY), v => v.Value);

            if (cycleStart == 0)
            {
                foreach(var state in previousCycleStates)
                {
                    if (top20rows.Keys.All(t => state.ContainsKey(t)) && state.Keys.All(s => top20rows.ContainsKey(s)))
                    {
                        cycleStart = count;
                    }
                }

                if (cycleStart != 0)
                {
                    previousCycleStates.Clear();
                }

                previousCycleStates.Add(top20rows);
            }
            else if (cycleLength == 0)
            {
                foreach(var state in previousCycleStates)
                {
                    if (top20rows.Keys.All(t => state.ContainsKey(t)) && state.Keys.All(s => top20rows.ContainsKey(s)))
                    {
                        cycleLength = count - cycleStart;
                        cycleStart = cycleStart - cycleLength; // we overcounted above, because we had to do at least 2 cycles to determine cycle start
                    }
                }
            }

            foreach(var rockNode in rock.Shape)
            {
                map.Remove((newRockPos.x + rockNode.x, newRockPos.y + rockNode.y));
            }
        }

        jetOffset = (jetOffset + 1) % jets.Length;
        bool movable = true;
        if (jet == '<')
        {
            foreach(var left in rock.Lefts)
            {
                movable = movable && newRockPos.x + left.x >= 0 && !map.ContainsKey((newRockPos.x + left.x, newRockPos.y + left.y));
            }

            if (movable)
            {
                newRockPos = (newRockPos.x - 1, newRockPos.y);
            }
        }
        else
        {
            foreach(var right in rock.Rights)
            {
                movable = movable && newRockPos.x + right.x < 7 && !map.ContainsKey((newRockPos.x + right.x, newRockPos.y + right.y));
            }

            if (movable)
            {
                newRockPos = (newRockPos.x + 1, newRockPos.y);
            }
        }

        movable = true;
        // fall rock
        foreach(var below in rock.Belows)
        {
            movable = movable && !map.ContainsKey((newRockPos.x + below.x, newRockPos.y + below.y));
        }

        if (movable)
        {
            newRockPos = (newRockPos.x, newRockPos.y - 1);
        }
        else 
        {
            foreach(var rockNode in rock.Shape)
            {
                map.Add((newRockPos.x + rockNode.x, newRockPos.y + rockNode.y), '#');
            }

            maxY = map.Max(m => m.Key.y);
            count++;

            break;
        }
    }
}

Console.WriteLine(maxY);

var startingHeight = (long)previousHeights[cycleStart];
var numberOfCycles = (1_000_000_000_000-cycleStart) / (cycleLength);
var heightPerCycle = previousHeights[cycleStart + cycleLength] - previousHeights[cycleStart];
var remainingRockCount = (1_000_000_000_000-cycleStart) % (cycleLength);
var remainingHeight = previousHeights[cycleStart + (int)remainingRockCount] - previousHeights[cycleStart];

var ans =  startingHeight + numberOfCycles*heightPerCycle + remainingHeight;

Console.WriteLine(ans); // 1539965497389 too high
                        // 1539823008831 wrong
                        // 1539823008831

void PrintArea(Dictionary<(int x, int y), char> map, int yRegion)
{
    for (int j = 0; j > -15; j--)
    {
        Console.Write(j.ToString().PadRight(5));
        for (int i = 0; i < 7; i++)
        {
            if (map.ContainsKey((i, j+yRegion)))
            {
                Console.Write("#");
            }
            else
            {
                Console.Write(" ");
            }
        }
        Console.WriteLine();
    }
}


class Rock 
{
    public int Type { get; set; }
    public HashSet<(int x, int y)> Shape { get; set; }
    public HashSet<(int x, int y)> Lefts { get; set; }
    public HashSet<(int x, int y)> Rights { get; set; }
    public HashSet<(int x, int y)> Belows { get; set; }
}