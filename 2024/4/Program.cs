var lines = File.ReadAllLines("input.txt");

var map = new Dictionary<(int x, int y), char>();

int x = 0;
int y = 0;
int maxX = 0;

foreach(var line in lines)
{
    foreach(var character in line)
    {
        map[(x, y)] = character;
        x++;
    }
    maxX = x;
    x = 0;
    y++;
}

int maxY = y;

var count = 0;

foreach(var starter in map)
{
    if (starter.Value == 'X')
    {
        // up
        if (starter.Key.y - 3 >= 0)
        {
            if (map[(starter.Key.x, starter.Key.y-1)] == 'M' &&
                map[(starter.Key.x, starter.Key.y-2)] == 'A' &&
                map[(starter.Key.x, starter.Key.y-3)] == 'S'
                )
                {
                    count++;
                }
        }
        // upright
        if (starter.Key.y - 3 >= 0 && starter.Key.x + 3 < maxX)
        {
            if (map[(starter.Key.x+1, starter.Key.y-1)] == 'M' &&
                map[(starter.Key.x+2, starter.Key.y-2)] == 'A' &&
                map[(starter.Key.x+3, starter.Key.y-3)] == 'S'
                )
                {
                    count++;
                }
        }
        // right
        if (starter.Key.x + 3 < maxX)
        {
            if (map[(starter.Key.x+1, starter.Key.y)] == 'M' &&
                map[(starter.Key.x+2, starter.Key.y)] == 'A' &&
                map[(starter.Key.x+3, starter.Key.y)] == 'S'
                )
                {
                    count++;
                }
        }
        // downright
        if (starter.Key.y + 3 < maxY && starter.Key.x + 3 < maxX)
        {
            if (map[(starter.Key.x+1, starter.Key.y+1)] == 'M' &&
                map[(starter.Key.x+2, starter.Key.y+2)] == 'A' &&
                map[(starter.Key.x+3, starter.Key.y+3)] == 'S'
                )
                {
                    count++;
                }
        }
        // down
        if (starter.Key.y + 3 < maxY)
        {
            if (map[(starter.Key.x, starter.Key.y+1)] == 'M' &&
                map[(starter.Key.x, starter.Key.y+2)] == 'A' &&
                map[(starter.Key.x, starter.Key.y+3)] == 'S'
                )
                {
                    count++;
                }
        }
        // downleft
        if (starter.Key.y + 3 < maxY && starter.Key.x - 3 >= 0)
        {
            if (map[(starter.Key.x-1, starter.Key.y+1)] == 'M' &&
                map[(starter.Key.x-2, starter.Key.y+2)] == 'A' &&
                map[(starter.Key.x-3, starter.Key.y+3)] == 'S'
                )
                {
                    count++;
                }
        }
        // left
        if (starter.Key.x - 3 >= 0)
        {
            if (map[(starter.Key.x-1, starter.Key.y)] == 'M' &&
                map[(starter.Key.x-2, starter.Key.y)] == 'A' &&
                map[(starter.Key.x-3, starter.Key.y)] == 'S'
                )
                {
                    count++;
                }
        }
        // upleft
        if (starter.Key.y - 3 >= 0 && starter.Key.x - 3 >= 0)
        {
            if (map[(starter.Key.x-1, starter.Key.y-1)] == 'M' &&
                map[(starter.Key.x-2, starter.Key.y-2)] == 'A' &&
                map[(starter.Key.x-3, starter.Key.y-3)] == 'S'
                )
                {
                    count++;
                }
        }
    }
}

Console.WriteLine(count);

count = 0;

foreach (var starter in map)
{
    if (starter.Value == 'A')
    {
        var upleft = (starter.Key.x - 1, starter.Key.y - 1);
        var upright = (starter.Key.x + 1, starter.Key.y - 1);
        var downleft = (starter.Key.x - 1, starter.Key.y + 1);
        var downright = (starter.Key.x + 1, starter.Key.y + 1);

        if (map.ContainsKey(upleft) && map.ContainsKey(upright) && map.ContainsKey(downleft) && map.ContainsKey(downright))
        {
            if ((map[upleft] == 'M' && map[downright] == 'S' || map[upleft] == 'S' && map[downright] == 'M') &&
                (map[downleft] == 'M' && map[upright] == 'S' || map[downleft] == 'S' && map[upright] == 'M'))
                {
                    count++;
                }
        }
    }
}
Console.WriteLine(count);