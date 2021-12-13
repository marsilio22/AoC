var lines = File.ReadAllLines("./input.txt");

var map = new Dictionary<(int x, int y), int>();

foreach(var line in lines)
{
    var splitLine = line.Split(",").ToList();

    if (splitLine.Count() > 1)
    {
        var coords = splitLine.Select(s => int.Parse(s)).ToList();
        map[(coords[0], coords[1])] = 1;
    }
    else
    {
        splitLine = line.Split(" ").Last().Split("=").ToList();
        if (splitLine[0] == "x") {
            var num = int.Parse(splitLine[1]);
            // things to the right have a higher coord
            var entriesToMove = map.Where(c => c.Key.x > num).ToList();

            // things move to line - (x - line)
            foreach(var thing in entriesToMove)
            {
                map.Remove(thing.Key);

                var newKey = (num - (thing.Key.x - num), thing.Key.y);
                map[newKey] = 1;
            }

            Console.WriteLine($"Folded on x = {num}, {map.Count()} dots remain");
        }
        else
        {
            if (line.Length == 0)
            {
                continue;
            }

            var num = int.Parse(splitLine[1]);
            // things below have a higher coord
            var entriesToMove = map.Where(c => c.Key.y > num).ToList();

            // things move to line - (y - line)
            foreach(var thing in entriesToMove)
            {
                map.Remove(thing.Key);

                var newKey = (thing.Key.x, num - (thing.Key.y - num));
                map[newKey] = 1;
            }

            Console.WriteLine($"Folded on y = {num}, {map.Count()} dots remain");
        }
    }
}

Console.WriteLine(map.Count());

var minX = map.MinBy(m => m.Key.x).Key.x;
var maxX = map.MaxBy(m => m.Key.x).Key.x;
var minY = map.MinBy(m => m.Key.y).Key.y;
var maxY = map.MaxBy(m => m.Key.y).Key.y;

for (int j = minY; j <= maxY; j++)
{
    for (int i = minX; i <= maxX; i++)
    {
        var character = map.TryGetValue((i, j), out var val) ? '#' : ' ';
        Console.Write(character);
    }
    Console.WriteLine();
}