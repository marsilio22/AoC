var input = File.ReadAllLines("./input");

var screen = new Dictionary<(int x, int y), char>();

var register = 1;
var tick = 1;

var signal = 0;

foreach(var line in input)
{
    if (line.StartsWith("addx"))
    {
        var split = line.Split(" ");

        var val = int.Parse(split[1]);

        for (int i = 0; i < 2; i++) {
            (tick, signal) = ProcessTick(screen, tick, register, signal);
        }

        register += val;
    }
    else
    {
        (tick, signal) = ProcessTick(screen, tick, register, signal);
    }
}

Console.WriteLine(signal);
Console.WriteLine();
Draw(screen); // BACEKLHF

(int, int) ProcessTick(Dictionary<(int x, int y), char> dict, int tick, int regx, int signal)
{
    if ((tick - 20) % 40 == 0)
    {
        signal += (tick) * register;
    }

    tick = AddPixel(dict, tick, register);

    return (tick, signal);
}

int AddPixel(Dictionary<(int x, int y), char> dict, int tick, int regx)
{
    var row = (tick-1)/40;
    var column = (tick-1) % 40;

    if (Math.Abs(column - regx) <= 1)
    {
        dict[(row, column)] = '#';
    }
    else
    {
        dict[(row, column)] = ' ';
    }

    return tick+1;
}

void Draw (Dictionary<(int x, int y), char> dict)
{
    var minx = dict.Keys.Min(d => d.x);
    var miny = dict.Keys.Min(d => d.y);
    var maxx = dict.Keys.Max(d => d.x);
    var maxy = dict.Keys.Max(d => d.y);
    
    for(int j = minx; j <= maxx; j++)
    {
        for (int i = miny; i <= maxy; i++)
        {
            Console.Write(dict[(j, i)]);
        }
        Console.WriteLine();
    }
}