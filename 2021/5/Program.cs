using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

var lines = File.ReadAllLines("./input.txt");


ICollection<((int x, int y) start, (int x, int y) finish)> coordinatePairs = lines.Select(l => Parse(l.Split(" -> "))).ToList();

//part1
var p1CoordinatePairs = coordinatePairs.Where(cp => cp.start.x == cp.finish.x || cp.start.y == cp.finish.y).ToList();

var map = new Dictionary<(int x, int y), int>(); // count how many plumes in each cell as we go

foreach(var pair in p1CoordinatePairs)
{
    // assume diagonals are always gradient 1 or -1
    var distance = Math.Max(Math.Abs(pair.finish.x - pair.start.x), Math.Abs(pair.finish.y - pair.start.y));
    (int x, int y) direction = ((pair.finish.x - pair.start.x) / distance, (pair.finish.y - pair.start.y) / distance);

    for (int j = 0; j <= distance; j++)
    {
        (int x, int y) coord;
        if (pair.start.x == pair.finish.x)
        {
            coord = (pair.start.x, pair.start.y + j*direction.y);
        }
        else if (pair.start.y == pair.finish.y)
        {
            coord = (pair.start.x + j*direction.x, pair.start.y);
        }
        else 
        {
            coord = (pair.start.x + j*direction.x, pair.start.y + j*direction.y);
        }

        map.TryGetValue(coord, out var value);
        map[coord] = value + 1; // default will be 0, so that's cool.
    }
}

var ans = map.Count(m => m.Value > 1);
Draw(map, "part1.txt");

Console.WriteLine(ans);


//part2
var p2CoordinatePairs = coordinatePairs.Where(cp => !p1CoordinatePairs.Contains(cp));

foreach(var pair in p2CoordinatePairs)
{
    // assume diagonals are always gradient 1 or -1
    var distance = Math.Max(Math.Abs(pair.finish.x - pair.start.x), Math.Abs(pair.finish.y - pair.start.y));
    (int x, int y) direction = ((pair.finish.x - pair.start.x) / distance, (pair.finish.y - pair.start.y) / distance);

    for (int j = 0; j <= distance; j++)
    {
        (int x, int y) coord;
        if (pair.start.x == pair.finish.x)
        {
            coord = (pair.start.x, pair.start.y + j*direction.y);
        }
        else if (pair.start.y == pair.finish.y)
        {
            coord = (pair.start.x + j*direction.x, pair.start.y);
        }
        else 
        {
            coord = (pair.start.x + j*direction.x, pair.start.y + j*direction.y);
        }

        map.TryGetValue(coord, out var value);
        map[coord] = value + 1; // default will be 0, so that's cool.
    }
}

ans = map.Count(m => m.Value > 1);
Draw(map, "part2.txt");

Console.WriteLine(ans);

static ((int x1, int y1), (int x2, int y2)) Parse(string[] strings)
{
    var x1y1 = strings[0].Split(",").Select(s => int.Parse(s)).ToList();
    var x2y2 = strings[1].Split(",").Select(s => int.Parse(s)).ToList();

    return ((x1y1[0], x1y1[1]),(x2y2[0], x2y2[1]));
}

static void Draw(IDictionary<(int x, int y), int> map, string filename)
{
    var maxX = map.Max(m => m.Key.x);
    var minX = map.Min(m => m.Key.x);
    var maxY = map.Max(m => m.Key.y);
    var minY = map.Min(m => m.Key.y);

    var sb = new StringBuilder();

    for (int j = minY; j <= maxY; j++)
    {
        for (int i = minX; i <= maxX; i++)
        {
            if (map.TryGetValue((i, j), out var value))
            {
                sb.Append(value);
            }
            else
            {
                sb.Append(" ");
            }
        }
        sb.Append(Environment.NewLine);
    }

    File.WriteAllText(filename, sb.ToString());
}