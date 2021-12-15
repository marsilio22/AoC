for (int tesselationWidth = 1; tesselationWidth <= 5; tesselationWidth += 4)
{
    var lines = File.ReadAllLines("./input.txt").ToList();

    var map = new Dictionary<(int x, int y), Node>();

    for (int c = 0; c < tesselationWidth; c ++)
    {
        for(int d = 0; d < tesselationWidth; d ++)
        {
            for(int j = 0; j < lines.Count(); j++)
            {
                var line = lines[j];
                for (int i = 0; i < line.Length; i++)
                {
                    map[(i + d * line.Length, j + c * line.Length)] = new Node((i + d * line.Length, j + c * lines.Count), int.Parse(line[i].ToString() )+ c + d, tesselationWidth * lines.Count - 1);
                }
            }
        }
    }

    // draw
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

    var adjacents = new Dictionary<(int x, int y), int>();

    foreach(var adj in map[current].Adjacents)
    {
        adjacents.Add(adj, map[adj].Risk);
    }

    map[current].Distance = 0;

    while(adjacents.Any() && map[end].Distance == int.MaxValue)
    {
        var next = adjacents.MinBy(a => a.Value);

        map[next.Key].Distance = next.Value;

        adjacents.Remove(next.Key);

        var newAdjacents = map[next.Key].Adjacents.Where(a => map[a].Distance > next.Value);
        foreach(var adj in newAdjacents)
        {
            adjacents[adj] = next.Value + map[adj].Risk;
        }
    }

    Console.WriteLine(map[end].Distance);

}
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