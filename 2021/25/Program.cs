using System.Reflection;

var lines = File.ReadAllLines("./input.txt");

var map = new Dictionary<(int x, int y), char>();

var width = lines.First().Length;

for (int j = 0; j < lines.Count(); j++)
{
    for (int i = 0; i < width; i++)
    {
        map[(i, j)] = lines[j][i];
    }
}

var count = 1;

while(true)
{
    var moved = 0;

    // east first
    var copy = map.ToDictionary(m => m.Key, m => m.Value);
    var movers = map.Where(c => c.Value == '>').ToList();

    foreach(var mover in movers)
    {
        if (map.ContainsKey((mover.Key.x+1, mover.Key.y)))
        {
            if (copy[(mover.Key.x+1, mover.Key.y)] == '.')
            {
                map[mover.Key] = '.';
                map[(mover.Key.x + 1, mover.Key.y)] = '>';
                moved++;
            }
        }
        else
        {
            if (copy[(0, mover.Key.y)] == '.')
            {
                map[mover.Key] = '.';
                map[(0, mover.Key.y)] = '>';
                moved++;
            }
        }
    }

    // then south
    copy = map.ToDictionary(m => m.Key, m => m.Value);
    movers = map.Where(c => c.Value == 'v').ToList();

    foreach(var mover in movers)
    {
        if (map.ContainsKey((mover.Key.x, mover.Key.y+1)))
        {
            if (copy[(mover.Key.x, mover.Key.y+1)] == '.')
            {
                map[mover.Key] = '.';
                map[(mover.Key.x, mover.Key.y+1)] = 'v';
                moved++;
            }
        }
        else
        {
            if (copy[(mover.Key.x, 0)] == '.')
            {
                map[mover.Key] = '.';
                map[(mover.Key.x, 0)] = 'v';
                moved++;
            }
        }
    }

    if (moved == 0)
    {
        Console.WriteLine(count);
        break;
    }

    count++;
}