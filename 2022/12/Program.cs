using System.Diagnostics;

var input = File.ReadAllLines("./input");

var sw = new Stopwatch();

sw.Start();

var map = new Dictionary<(int x, int y), (char height, int distance)>();

for (int j = 0; j < input.Length; j++)
{
    for (int i = 0; i < input[j].Length; i++)
    {
        map[(i, j)] = (input[j][i], int.MaxValue);
    }
}

var start = map.Single(d => d.Value.height == 'S');
var target = map.Single(d => d.Value.height == 'E');

map[start.Key] = ('a', 0);
map[target.Key] = ('z', int.MaxValue);

var visited = new HashSet<(int, int)>{start.Key};
var noMoreValidNeighbours = new HashSet<(int, int)>();

while(!visited.Contains(target.Key))
{
    var visitedKVPs = map.Where(m => visited.Contains(m.Key) && !noMoreValidNeighbours.Contains(m.Key));
    
    foreach(var kvp in visitedKVPs)
    {
        var neighbours = Neighbours(map, kvp.Key).Where(n => n.Value.height - kvp.Value.height <= 1 && n.Value.distance > kvp.Value.distance + 1);

        if (!neighbours.Any())
        {
            noMoreValidNeighbours.Add(kvp.Key);
        }

        foreach(var n in neighbours)
        {
            map[n.Key] = (n.Value.height, kvp.Value.distance + 1);
            visited.Add(n.Key);
        }
    }
}

Console.WriteLine(map[target.Key].distance);
Console.WriteLine(sw.ElapsedMilliseconds);

// p2
map = new Dictionary<(int x, int y), (char height, int distance)>();

for (int j = 0; j < input.Length; j++)
{
    for (int i = 0; i < input[j].Length; i++)
    {
        map[(i, j)] = (input[j][i], int.MaxValue);
    }
}

var S = map.Single(d => d.Value.height == 'S');
start = map.Single(d => d.Value.height == 'E');

map[S.Key] = ('a', int.MaxValue);
map[start.Key] = ('z', 0);

visited = new HashSet<(int, int)>{start.Key};
noMoreValidNeighbours = new HashSet<(int, int)>();
var visitedLastRound = 0;


while(visited.Count() > visitedLastRound)
{
    var visitedKVPs = map.Where(m => visited.Contains(m.Key) && !noMoreValidNeighbours.Contains(m.Key));
    visitedLastRound = visited.Count();
    
    foreach(var kvp in visitedKVPs)
    {
        var neighbours = Neighbours(map, kvp.Key).Where(n => kvp.Value.height - n.Value.height <= 1 && n.Value.distance > kvp.Value.distance + 1);

        if (!neighbours.Any())
        {
            noMoreValidNeighbours.Add(kvp.Key);
        }
        
        foreach(var n in neighbours)
        {
            map[n.Key] = (n.Value.height, kvp.Value.distance + 1);
            visited.Add(n.Key);
        }
    }
}

var ans = map.Where(m => m.Value.height == 'a').Min(m => m.Value.distance);

Console.WriteLine(ans);

Console.WriteLine(sw.ElapsedMilliseconds);




ICollection<KeyValuePair<(int x, int y), (char height, int distance)>> Neighbours(IDictionary<(int x, int y), (char height, int distance)> map, (int x, int y) pos)
{
    return map.Where(m => (m.Key.x == pos.x && (m.Key.y == pos.y - 1 || m.Key.y == pos.y + 1)) ||
                            m.Key.y == pos.y && (m.Key.x == pos.x - 1 || m.Key.x == pos.x + 1)).ToList();
}