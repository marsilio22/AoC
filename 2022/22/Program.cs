var lines = File.ReadAllLines("./input");

var map = new Dictionary<(int x, int y), char>();

var x = 1;
var y = 1;

foreach(var line in lines)
{
    if (!line.Contains("R") && !string.IsNullOrEmpty(line))
    {
        foreach(var character in line)
        {
            if (character != ' ')
            {
                map[(x, y)] = character;
            }
            x++;
        }
    }

    y++;
    x = 1;
}

Console.WriteLine();

var instr = lines.Last();
var dirs = instr.Where(i => i == 'L' || i == 'R').ToArray();
var distances = instr.Split("L").SelectMany(p => p.Split("R")).Select(r => int.Parse(r)).ToArray();

var loc = map.Where(m => m.Key.y == 1 && m.Value == '.').MinBy(m => m.Key.x).Key;
var facing = 'E';


// input instr starts with a number and ends with a number

for (int i = 0; i < distances.Length; i ++)
{
    // do distance
    (int x, int y) moveDirection = facing switch {
        'N' => (0, -1), // this is the stupid compsci way round
        'S' => (0, 1),
        'E' => (1, 0),
        'W' => (-1, 0),
        _ => throw new Exception()
    };

    var dist = distances[i];
    for (int j = 0; j < dist; j++)
    {
        var nextCoord = (loc.x + moveDirection.x, loc.y + moveDirection.y);

        if (!map.ContainsKey(nextCoord))
        {
            nextCoord = facing switch {
                'N' => map.Where(m => m.Key.x == loc.x).MaxBy(m => m.Key.y).Key,
                'S' => map.Where(m => m.Key.x == loc.x).MinBy(m => m.Key.y).Key,
                'E' => map.Where(m => m.Key.y == loc.y).MinBy(m => m.Key.x).Key,
                'W' => map.Where(m => m.Key.y == loc.y).MaxBy(m => m.Key.x).Key,
                _ => throw new Exception()
            };
        }

        if (map[nextCoord] == '.')
        {
            loc = nextCoord;
        }
    }

    // turn if dir exists otherwise calculate answer
    if (i < dirs.Length)
    {
        var turn = dirs[i];

        facing = (facing, turn) switch {
            ('N', 'L') or ('S', 'R') => 'W',
            ('S', 'L') or ('N', 'R') => 'E',
            ('E', 'L') or ('W', 'R') => 'N',
            ('W', 'L') or ('E', 'R') => 'S',
            _ => throw new Exception()
        };
    }
    else 
    {
        var facingScore = facing switch {
            'N' => 3,
            'E' => 0,
            'S' => 1,
            'W' => 2,
            _ => throw new Exception()
        };

        var ans = 1000 * loc.y + 4 * loc.x + facingScore;
        Console.WriteLine(ans);
    }

}

// part 2
loc = map.Where(m => m.Key.y == 1 && m.Value == '.').MinBy(m => m.Key.x).Key;
facing = 'E';

// input instr starts with a number and ends with a number

for (int i = 0; i < distances.Length; i ++)
{
    var dist = distances[i];
    for (int j = 0; j < dist; j++)
    {

        // do distance
        (int x, int y) moveDirection = facing switch {
            'N' => (0, -1), // this is the stupid compsci way round
            'S' => (0, 1),
            'E' => (1, 0),
            'W' => (-1, 0),
            _ => throw new Exception()
        };

        var nextCoord = (loc.x + moveDirection.x, loc.y + moveDirection.y);
        var nextFacing = facing;

        if (!map.ContainsKey(nextCoord))
        {
            if (facing == 'N') {
                if (loc.x >= 1 && loc.x <= 50)
                {
                    nextCoord = (51, 50 + loc.x); // top of 5 to left of 3
                    nextFacing = 'E';
                }
                else if (loc.x >= 51 && loc.x <= 100)
                {
                    nextCoord = (1, 100 + loc.x); // top of 2 to left of 6
                    nextFacing = 'E';
                }
                else if (loc.x >= 100 && loc.x <= 150)
                {
                    nextCoord = (loc.x - 100, 200); // top of 1 to bottom of 6
                    // facing does not change
                }
            }
            else if (facing == 'S')
            {
                if (loc.x >= 1 && loc.x <= 50)
                {
                    nextCoord = (loc.x + 100, 1); // bottom of 6 to top of 1
                    // facing does not change
                }
                else if (loc.x >= 51 && loc.x <= 100)
                {
                    nextCoord = (50, 100 + loc.x); // bottom of 4 to right of 6
                    nextFacing = 'W';
                }
                else if (loc.x >= 101 && loc.x <= 150)
                {
                    nextCoord = (100, loc.x - 50); // bottom of 1 to right of 3
                    nextFacing = 'W';
                }
            }
            else if (facing == 'E')
            {
                if (loc.y >= 1 && loc.y <= 50) 
                {
                    nextCoord = (100, 151 - loc.y); // right of 1 to right of 4 (flip upside down)
                    nextFacing = 'W';
                }
                else if (loc.y >= 51 && loc.y <= 100) 
                {
                    nextCoord = (50 + loc.y, 50); // right of 3 to bottom of 1
                    nextFacing = 'N';
                }
                else if (loc.y >= 101 && loc.y <= 150) 
                {
                    nextCoord = (150, 151 - loc.y); // right of 4 to right of 1 (flip upside down)
                    nextFacing = 'W';
                }
                else if (loc.y >= 151 && loc.y <= 200) 
                {
                    nextCoord = (loc.y-100, 150); // right of 6 to bottom of 4
                    nextFacing = 'N';
                }
            }
            else if (facing == 'W')
            {
                if (loc.y >= 1 && loc.y <= 50)
                {
                    nextCoord = (1, 151 - loc.y); // left of 2 to left of 5 (flip upside down)
                    nextFacing = 'E';
                }
                else if (loc.y >= 51 && loc.y <= 100)
                {
                    nextCoord = (loc.y - 50, 101); // left of 3 to top of 5
                    nextFacing = 'S';
                }
                else if (loc.y >= 101 && loc.y <= 150)
                {
                    nextCoord = (51, 151 - loc.y); // left of 5 to left of 2 (flip upside down)
                    nextFacing = 'E';
                }
                else if (loc.y >= 151 && loc.y <= 200)
                {
                    nextCoord = (loc.y - 100, 1); // left of 6 to top of 2
                    nextFacing = 'S';
                }
            }

            // Print(map, loc, nextCoord, facing, nextFacing);
        }

        if (map[nextCoord] == '.')
        {
            loc = nextCoord;
            facing = nextFacing;
        }
    }

    // turn if dir exists otherwise calculate answer
    if (i < dirs.Length)
    {
        var turn = dirs[i];

        facing = (facing, turn) switch {
            ('N', 'L') or ('S', 'R') => 'W',
            ('S', 'L') or ('N', 'R') => 'E',
            ('E', 'L') or ('W', 'R') => 'N',
            ('W', 'L') or ('E', 'R') => 'S',
            _ => throw new Exception()
        };
    }
    else 
    {
        var facingScore = facing switch {
            'N' => 3,
            'E' => 0,
            'S' => 1,
            'W' => 2,
            _ => throw new Exception()
        };

        var ans = 1000 * loc.y + 4 * loc.x + facingScore;
        Console.WriteLine(ans); // 163029 too high
                                // 116301 too high
                                // 81319 too low
                                // 95291 correct
    }
}

void Print(IDictionary<(int x, int y), char> map, (int x, int y) currentLocation, (int x, int y) nextLocation, char currentFacing, char nextFacing)
{
    for (int j = 1; j <= 200; j++)
    {
        for (int i = 1; i <= 150; i ++)
        {
            if (map.ContainsKey((i, j)))
            {
                if (currentLocation == (i, j))
                {
                    Console.Write(currentFacing);
                }
                else if (nextLocation == (i, j))
                {
                    Console.Write(nextFacing);
                }
                else
                {
                    Console.Write(map[(i, j)]);
                }
            }
            else
            {
                Console.Write(' ');
            }
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}