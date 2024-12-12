var lines = File.ReadAllLines("./input.txt");

var map = new Dictionary<(int x, int y), int>();

int x = 0, y = 0;

foreach (var line in lines)
{
    foreach (var character in line)
    {
        map[(x, y)] = int.Parse(character.ToString());
        x++;
    }
    x = 0; y++;
}

var distinctNines = map.Where(m => m.Value == 9).ToDictionary();
var zeroes = map.Where(m => m.Value == 0).ToDictionary();

var trailheadScores = new Dictionary<(int x, int y), ICollection<(int x, int y)>>();

foreach(var zero in zeroes)
{
    trailheadScores.Add(zero.Key, FindValidTrails(zero.Key, map));
}

Console.WriteLine(trailheadScores.Values.Sum(v => v.Distinct().Count()));
Console.WriteLine(trailheadScores.Values.Sum(v => v.Count));

ICollection<(int, int)> FindValidTrails((int x, int y) pos, IDictionary<(int, int), int> map)
{
    if (map[pos] == 9)
    {
        return new List<(int, int)>{pos};
    }

    var dirs = new [] {(pos.x, pos.y - 1), (pos.x + 1, pos.y), (pos.x, pos.y + 1), (pos.x - 1, pos.y)};

    var validNextSteps = map.Where(m => dirs.Contains(m.Key) && m.Value == map[pos] + 1);

    if (validNextSteps.Any())
    {
        var result = new List<(int, int)>();
        foreach(var step in validNextSteps)
        {
            var valid = FindValidTrails(step.Key, map);

            foreach(var thing in valid)
            {
                result.Add(thing);
            }
        }
        return result;
    }

    return new List<(int, int)>();
}