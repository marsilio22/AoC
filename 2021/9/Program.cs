using Microsoft.Win32;

var lines = File.ReadAllLines("input.txt");

var map = new Dictionary<(int x, int y), int>();
var y = 0;
foreach(var line in lines)
{
    var x = 0;
    foreach(var character in line)
    {
        map[(x, y)] = int.Parse(character.ToString());
        if (character == '9')
        {
            Console.Write("9");
        }
        else
        {
            Console.Write(" ");
        }

        x++;
    }
    Console.WriteLine();
    y++;
}

var lowpoints = new List<(int x, int y)>();
foreach(var cell in map)
{
    var adjacents = map.Where(m => 
        (m.Key.x == cell.Key.x && (m.Key.y == cell.Key.y - 1 || m.Key.y == cell.Key.y + 1)) ||
        (m.Key.y == cell.Key.y && (m.Key.x == cell.Key.x - 1 || m.Key.x == cell.Key.x + 1))
    );

    if (adjacents.Select(a => a.Value).All(a => a > cell.Value))
    {
        lowpoints.Add(cell.Key);
    }
}

var ans = map.Where(m => lowpoints.Contains(m.Key)).Select(v => v.Value).Sum(v => v+1);

Console.WriteLine(ans);

var buckets = new List<ICollection<(int x, int y)>>();

foreach(var lowpoint in lowpoints)
{
    // I'm glad I checked this, but it's true for every lowpoint by the looks of the end result
    if (!buckets.Any(b => b.Contains(lowpoint)))
    {
        // create new bucket 
        var bucket = new List<(int x, int y)>();
        bucket.Add(lowpoint);

        // iterate around and populate it
        var adjacentsToCheck = new List<(int x, int y)>();
        
        adjacentsToCheck.AddRange(map.Where(m => 
            (m.Key.x == lowpoint.x && (m.Key.y == lowpoint.y - 1 || m.Key.y == lowpoint.y + 1)) ||
            (m.Key.y == lowpoint.y && (m.Key.x == lowpoint.x - 1 || m.Key.x == lowpoint.x + 1))
        ).Select(m => m.Key).ToList());

        while(adjacentsToCheck.Any())
        {
            var point = adjacentsToCheck.First();
            adjacentsToCheck.Remove(point);

            if(map[point] != 9)
            {
                bucket.Add(point);
                // the grind is real
                adjacentsToCheck.AddRange(map.Where(m => 
                    !bucket.Contains(m.Key) && 
                    !adjacentsToCheck.Contains(m.Key) && 
                    ((m.Key.x == point.x && (m.Key.y == point.y - 1 || m.Key.y == point.y + 1)) ||
                    (m.Key.y == point.y && (m.Key.x == point.x - 1 || m.Key.x == point.x + 1)))
                ).Select(m => m.Key).ToList());
            }
        }
        buckets.Add(bucket);
    }
}

var counts = buckets.Select(b => b.Count).OrderByDescending(c => c).ToList();

Console.WriteLine(counts[0] * counts[1] * counts[2]);