var input = File.ReadAllLines("./input");

var map = new Dictionary<(int x, int y), (char height, int distance)>();

for (int j = 0; j < input.Length; j++)
{
    for (int i = 0; i < input[j].Length; i++)
    {
        map[(i, j)] = (input[j][i], int.MaxValue);
    }
}

var S = map.Single(d => d.Value.height == 'S');
var E = map.Single(d => d.Value.height == 'E');

map[S.Key] = ('a', int.MaxValue);
map[E.Key] = ('z', 0);

var visited = new HashSet<(int, int)>{E.Key};
var noMoreValidNeighbours = new HashSet<(int, int)>();
var visitedLastRound = 0;

// take advantage of knowing what part 2 is, search backwards from the end just once, then p1 is the distance to S, and p2 is the closest 'a'
while(visited.Count() > visitedLastRound)
{
    var visitedKVPs = map.Where(m => visited.Contains(m.Key) && !noMoreValidNeighbours.Contains(m.Key));
    visitedLastRound = visited.Count();
    
    foreach(var kvp in visitedKVPs)
    {
        var neighbours = Neighbours(map, kvp.Key).Where(n => kvp.Value.height - n.Value.height <= 1 && n.Value.distance > kvp.Value.distance + 1);

        if (!neighbours.Any())
        { 
            // could probably change this so that we don't need the noMoreValidNeighbours collection, 
            // but loop condition would need to change too and I don't want to think about it
            noMoreValidNeighbours.Add(kvp.Key);
        }
        
        foreach(var n in neighbours)
        {
            map[n.Key] = (n.Value.height, kvp.Value.distance + 1);
            visited.Add(n.Key);
        }
    }
}

Console.WriteLine(map[S.Key].distance);

var ans = map.Where(m => m.Value.height == 'a').Min(m => m.Value.distance);
Console.WriteLine(ans);

ICollection<KeyValuePair<(int x, int y), (char height, int distance)>> Neighbours(IDictionary<(int x, int y), (char height, int distance)> map, (int x, int y) pos)
{
    return map.Where(m => (m.Key.x == pos.x && (m.Key.y == pos.y - 1 || m.Key.y == pos.y + 1)) ||
                            m.Key.y == pos.y && (m.Key.x == pos.x - 1 || m.Key.x == pos.x + 1)).ToList();
}