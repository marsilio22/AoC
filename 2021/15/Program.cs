using System.ComponentModel.DataAnnotations;

var lines = File.ReadAllLines("./input.txt").ToList();

var map = new Dictionary<(int x, int y), Node>();

for (int c = 0; c < 5; c ++)
{
    for(int d = 0; d < 5; d ++)
    {
        for(int j = 0; j < lines.Count(); j++)
        {
            var line = lines[j];
            for (int i = 0; i < line.Length; i++)
            {
                map[(i + d * line.Length, j + c * line.Length)] = new Node((i + d * line.Length, j + c * lines.Count), int.Parse(line[i].ToString() )+ c + d, 5 * lines.Count - 1);
            }
        }
    }
}
Console.WriteLine();

// var maxX = map.MaxBy(m => m.Key.x).Key.x;
// var maxY = map.MaxBy(m => m.Key.y).Key.y;

// for(int j = 0; j <= maxY; j++)
// {
//     for (int i = 0; i <= maxX; i++)
//     {
//         Console.Write(map[(i,j)].Risk);
//     }
//     Console.WriteLine();
// }

(int x, int y) current = (0, 0);
(int x, int y) end = map.Last().Key;

var adjacents = new List<(int x, int y)>();

adjacents.AddRange(map[current].Adjacents);

map[current].Distance = 0;

while(adjacents.Any() && map[end].Distance == int.MaxValue)
{
    var next = adjacents.MinBy(a => map[a].Risk + map[map[a].Adjacents.MinBy(b => map[b].Distance)].Distance); 

    map[next].Distance = map[next].Risk + map[map[next].Adjacents.MinBy(a => map[a].Distance)].Distance;

    adjacents.Remove(next);

    adjacents.AddRange(map[next].Adjacents.Where(a => map[a].Distance > map[next].Distance));

    adjacents = adjacents.Distinct().ToList();
}

Console.WriteLine(map[end].Distance);


public class Node
{
    public (int x, int y) Coord {get; set;}

    public int Risk {get; set;}

    public int Distance {get; set;}

    public ICollection<(int x, int y)> Adjacents {get; set;}

    public Node((int x, int y) coord, int risk, int max)
    {
        this.Coord = coord;
        this.Risk = risk;

        while (Risk >= 10)
        {
            Risk -= 9;
        }

        this.Distance = int.MaxValue;

        this.Adjacents = new List<(int x, int y)> 
        {
            (coord.x, coord.y - 1),
            (coord.x, coord.y + 1),
            (coord.x - 1, coord.y),
            (coord.x + 1, coord.y)
        };

        this.Adjacents = this.Adjacents.Where(a => a.x >= 0 && a.y >= 0 && a.x <= max && a.y <= max).ToList();
    }
}