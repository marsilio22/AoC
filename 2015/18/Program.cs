var lines = File.ReadAllLines("./input.txt");

var map = new Dictionary<(int x, int y), bool>();

for (int j = 0; j< lines.Length; j++)
{
    for (int i = 0; i < lines[j].Length; i++)
    {
        map[(i, j)] = lines[j][i] == '#';
    }
}

// part 2
map[(0, 0)] = true;
map[(99, 0)] = true;
map[(0, 99)] = true;
map[(99, 99)] = true;

for (int t = 0; t < 100; t++)
{
    var newMap = new Dictionary<(int x, int y), bool>(10000);

    foreach(var p in map)
    {
        var neighbours = GetNeighbours(p.Key);

        var ons = 0;

        foreach(var n in neighbours)
        {
            if (map.TryGetValue(n, out var on) && on)
            {
                ons++;
            }
        }

        if (p.Value && ons == 2 || ons == 3 || p.Key.x % 99 == 0 && p.Key.y % 99 == 0)
        {
            newMap[p.Key] = true;
        }
        else 
        {
            newMap[p.Key] = false;
        }
    }

    map = newMap;
}

Console.WriteLine(map.Count(m => m.Value));

ICollection<(int x, int y)> GetNeighbours((int x, int y) point)
{
    var ans = new List<(int x, int y)>();
    for (int j = -1; j <= 1; j++)
    {
        for (int i = -1; i <= 1; i++)
        {
            if (j == 0 && i == 0)
            {
                continue;
            }

            ans.Add((point.x + j, point.y + i));
        }
    }

    return ans;
}