using System.Linq.Expressions;

var lines = File.ReadAllLines("./input.txt");

var map = new Dictionary<(int x, int y), char>();

int x = 0, y = 0;

(int x, int y) start = (0, 0);

var maxX = lines[0].Length;
var maxY = lines.Length;

foreach (var line in lines)
{
    foreach(var character in line)
    {
        if (character == '#')
        {
            map.Add((x, y), character);
        }
        else if (character == '^')
        {
            start = (x, y);
        }
        x++;
    }
    y++;
    x = 0;
}

var facing = Facing.N;

var futhestYouCouldPossiblyGoInOneDirection = Math.Max(maxX, maxY); // maxX and maxY are probably the same
HashSet<(int, int)> visited = new HashSet<(int,int)>();
var pos = start;
visited.Add(pos);

var exited = false;

while (!exited)
{
    (int x, int y) direction = facing switch
    {
        Facing.N => (0, -1),
        Facing.E => (1, 0),
        Facing.S => (0, 1),
        Facing.W => (-1, 0)
    };

    for (int i = 1; i < futhestYouCouldPossiblyGoInOneDirection; i++)
    {
        (int x, int y) current = (pos.x + (i-1) * direction.x, pos.y + (i-1) * direction.y);
        visited.Add(current);

        if (map.ContainsKey((pos.x + i * direction.x, pos.y + i * direction.y)))
        {
            pos = current;
            facing = facing switch
            {
                Facing.N => Facing.E,
                Facing.E => Facing.S,
                Facing.S => Facing.W,
                Facing.W => Facing.N
            };
            break;
        }
        else if (current.x >= maxX || current.x < 0 || current.y >= maxY || current.y < 0)
        {
            Console.WriteLine(visited.Count - 1);
            exited = true;
            break;
        }
    }
}

var count = 0;

foreach(var visitedPosition in visited)
{
    var mapCopy = map.ToDictionary(k => k.Key, v => v.Value);
    mapCopy[visitedPosition] = '#';

    HashSet<(int, int)> visited2 = new HashSet<(int,int)>();
    pos = start;
    facing = Facing.N;
    visited2.Add(pos);

    exited = false;
    var loop = false;

    var visitedBeforeCount = 0;

    while (!exited && !loop)
    {
        (int x, int y) direction = facing switch
        {
            Facing.N => (0, -1),
            Facing.E => (1, 0),
            Facing.S => (0, 1),
            Facing.W => (-1, 0)
        };

        for (int i = 1; i < futhestYouCouldPossiblyGoInOneDirection; i++)
        {
            (int x, int y) current = (pos.x + (i-1) * direction.x, pos.y + (i-1) * direction.y);
            visitedBeforeCount += visited2.Add(current) ? 0 : 1;

            if (mapCopy.ContainsKey((pos.x + i * direction.x, pos.y + i * direction.y)))
            {
                pos = current;
                facing = facing switch
                {
                    Facing.N => Facing.E,
                    Facing.E => Facing.S,
                    Facing.S => Facing.W,
                    Facing.W => Facing.N
                };
                break;
            }
            else if (current.x >= maxX || current.x < 0 || current.y >= maxY || current.y < 0)
            {
                exited = true;
                break;
            }
        }
        loop = visitedBeforeCount > 1000; // heuristic
    }
    if (loop) 
    {
        count ++;
    }
}

Console.WriteLine(count);


enum Facing {
    N,
    E,
    S,
    W
}