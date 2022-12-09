var input = File.ReadAllLines("./input");

var map = new Dictionary<(int x, int y), int>();

var ropePositions = new List<(int x, int y)>();

// change max i for p1/p2
for (int i = 0; i < 10; i++)
{
    ropePositions.Add((0, 0));
}

map[ropePositions.Last()] = 1;

foreach(var line in input)
{
    var split = line.Split(" ");

    (int x, int y) direction = split[0] switch 
    {
        "R" => (1, 0),
        "L" => (-1, 0),
        "U" => (0, 1),
        "D" => (0, -1)
    };

    var distance = int.Parse(split[1]);

    for( int i = 0; i < distance; i ++ )
    {
        ropePositions[0] = (ropePositions[0].x + direction.x, ropePositions[0].y + direction.y);


        for (int j = 1; j < ropePositions.Count; j++)
        {
            (int x, int y) diff = (ropePositions[j-1].x - ropePositions[j].x, ropePositions[j-1].y - ropePositions[j].y);

            if (Math.Abs(diff.x) <= 1 && Math.Abs(diff.y) <= 1)
            {
                continue;
            }
            
            var diag = Math.Abs(diff.x) > 0 && Math.Abs(diff.y) > 0;

            if (!diag)
            {
                (int x, int y) tailDir = (diff.x / 2, diff.y / 2);
                ropePositions[j] = (ropePositions[j].x + tailDir.x, ropePositions[j].y + tailDir.y);
            }
            else 
            {
                (int x, int y) tailDir = diff switch
                {
                    (2, 1) or (1, 2) => (1, 1),
                    (2, -1) or (1, -2 )=> (1, -1),
                    (-2, 1) or (-1, 2) => (-1, 1),
                    (-2, -1) or (-1, -2) => (-1, -1),
                    (2, 2) => (1, 1),
                    (2, -2) => (1, -1),
                    (-2, 2) => (-1, 1),
                    (-2, -2) => (-1, -1),
                    _ => throw new Exception()
                };

                ropePositions[j] = (ropePositions[j].x + tailDir.x, ropePositions[j].y + tailDir.y);
            }

            if (j == ropePositions.Count - 1)
            {
                if (!map.TryGetValue(ropePositions[j], out var val))
                {
                    map[ropePositions[j]] = 0;
                }

                map[ropePositions[j]] = val + 1;
            }
        }
    }
}

Console.WriteLine(map.Count());