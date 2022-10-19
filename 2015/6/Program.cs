var input = File.ReadAllLines("./input.txt");

var grid = new Dictionary < (int x, int y),
    int > ();

foreach (var instruction in input)
{
    var split = instruction.Split(" ").ToList();

    string onOff;
    ICollection<int> start;
    ICollection<int> end;

    if (instruction.StartsWith("toggle"))
    {
        onOff = "toggle";
        start = split[1].Split(",").Select(x => int.Parse(x)).ToList();
        end = split[3].Split(",").Select(x => int.Parse(x)).ToList();
    }
    else
    {
        onOff = split[1];
        start = split[2].Split(",").Select(x => int.Parse(x)).ToList();
        end = split[4].Split(",").Select(x => int.Parse(x)).ToList();
    }

    for (int j = start.Last(); j <= end.Last(); j++)
    {
        for (int i = start.First(); i <= end.First(); i++)
        {
            grid.TryGetValue((i, j), out var val);
            
            switch (onOff) {
                case "on":
                    grid[(i, j)] = val + 1;
                    break;
                case "toggle":
                    grid[(i, j)] = val + 2;
                    break;
                case "off":
                    grid[(i, j)] = Math.Max(0, val - 1);
                    break;
            }
        }
    }
}

Console.WriteLine(grid.Values.Sum());