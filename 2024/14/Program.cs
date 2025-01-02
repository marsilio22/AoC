var lines = File.ReadAllLines("./input.txt");

int width = 101;
int height = 103;

var positions = new List<(long x, long y)>();

long timeStep = 100;

foreach(var line in lines)
{
    var split = line.Split(" v=");

    var posPair = split[0].Split("p=")[1].Split(",").Select(s => int.Parse(s)).ToArray();
    var velPair = split[1].Split(",").Select(s => int.Parse(s)).ToArray();

    (long x, long y) pos = (posPair[0], posPair[1]);
    (long x, long y) vel = (velPair[0], velPair[1]);

    long xAfter100 = (pos.x + (vel.x * timeStep)) % width;
    long yAfter100 = (pos.y + (vel.y * timeStep)) % height;

    xAfter100 = xAfter100 < 0 ? xAfter100 + width : xAfter100;
    yAfter100 = yAfter100 < 0 ? yAfter100 + height : yAfter100;

    positions.Add((xAfter100, yAfter100));
}

long topLeft = positions.Count(p => p.x < width/2 && p.y < height/2);
long topRight = positions.Count(p => p.x > width/2 && p.y < height/2);
long bottomLeft = positions.Count(p => p.x < width/2 && p.y > height/2);
long bottomRight = positions.Count(p => p.x > width/2 && p.y > height/2);

Console.WriteLine(topLeft * topRight * bottomLeft * bottomRight);
// 81994721 - too low
// 227584000 - too high


var map = new List<((int x, int y) pos, (int x, int y) vel)>();

foreach(var line in lines)
{
    var split = line.Split(" v=");

    var posPair = split[0].Split("p=")[1].Split(",").Select(s => int.Parse(s)).ToArray();
    var velPair = split[1].Split(",").Select(s => int.Parse(s)).ToArray();

    (int x, int y) pos = (posPair[0], posPair[1]);
    (int x, int y) vel = (velPair[0], velPair[1]);

    map.Add((pos, vel));
}

var steps = 0;
var lastPrint = 0;

while (true)
{
    steps+=1;

    List<((int newX, int newY), (int x, int y) vel)> nextMap = map.Select(m => {
        var newX = (m.pos.x + m.vel.x) % width;
        var newY = (m.pos.y + m.vel.y) % height;

        newX = newX < 0 ? newX + width : newX;
        newY = newY < 0 ? newY + height : newY;
        
        return ((newX, newY), m.vel);
    }).ToList();
    map = nextMap;

    var mapCoords = map.Select(m => m.pos).ToHashSet();
    if (map.Count() - mapCoords.Count() == 0) // as a guess, they don't overlap when they make the picture
    {
        Console.WriteLine(steps);

        lastPrint = steps;

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (mapCoords.Contains((j, i)))
                {
                    Console.Write("X");
                }
                else
                {
                    Console.Write(" ");
                }
            }
            Console.WriteLine();
        }
    }

    if (lastPrint> 0)
    {
        break;
    }
}