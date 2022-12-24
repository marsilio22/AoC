var lines = File.ReadAllLines("./input");

var map = new Dictionary<(int x, int y), ICollection<char>>();

var row = 0;
foreach(var line in lines)
{
    var col = 0;
    foreach( var character in line)
    {
        if (character != '.')
        {
            map[(col, row)] = new List<char>{character};
        }
        else 
        {
            map[(col, row)] = new List<char>();
        }
        col++;
    }
    row++;
}

((int x, int y) key, int time, Dictionary<(int x, int y), ICollection<char>> map) start = ((1, 0), 0, map);
var goal = map.Where(m => !m.Value.Any()).MaxBy(m => m.Key.y).Key;
bool goalAlreadyVisited = false;

var distanceToGoal = goal.x - 1 + goal.y;

var queue = new PriorityQueue<((int x, int y) key, int time, Dictionary<(int x, int y), ICollection<char>> map), int>(500000);
var minTimeToGoal = int.MaxValue;
ICollection<(int x, int y)> elfMoves = new [] {  (1, 0), (0, 1), (0, -1), (-1, 0), (0, 0)};
var previouslySeenStates = new HashSet<(int time, (int x, int y) pos)>();

queue.Enqueue(start, distanceToGoal);

while (queue.TryDequeue(out var state, out int prio))
{
    if (!previouslySeenStates.Add((state.time, state.key)))
    {
        continue;
    }

    map = state.map;

    if (state.key == goal)
    {
        Console.WriteLine(state.time);
        if (!goalAlreadyVisited)
        {
            goalAlreadyVisited = true;
            var temp = goal;
            goal = start.key;
            start = (temp, state.time, map);

            queue.Clear();
            previouslySeenStates.Clear();
            queue.Enqueue(start, distanceToGoal);
        }
        else if (goalAlreadyVisited && state.key == (1, 0))
        {
            // head back to goal again
            var temp = goal;
            goal = start.key;
            start = (temp, state.time, map);

            queue.Clear();
            previouslySeenStates.Clear();
            queue.Enqueue(start, distanceToGoal);
        }
        else
        {
            return;
        }
        continue;
    }

    // Console.WriteLine(state.time);
    // Print(map, state.key);
    // Console.WriteLine();

    // move blizzards
    var nextMapState = map.ToDictionary(m => m.Key, m => m.Value.Contains('#') ? m.Value : (ICollection<char>) new List<char>());
    foreach(var node in map)
    {
        if (node.Value.FirstOrDefault() == '#')
        {
            continue;
        }

        foreach(var blizz in node.Value)
        {
            var next = blizz switch {
                '>' => (node.Key.x + 1, node.Key.y),
                '<' => (node.Key.x - 1, node.Key.y),
                '^' => (node.Key.x, node.Key.y - 1),
                'v' => (node.Key.x, node.Key.y + 1),
                _ => throw new Exception()
            };

            if (map[next].Contains('#'))
            {
                var wall = blizz switch {
                    '>' => map.Where(m => m.Key.y == node.Key.y).MinBy(m => m.Key.x).Key,
                    '<' => map.Where(m => m.Key.y == node.Key.y).MaxBy(m => m.Key.x).Key,
                    '^' => map.Where(m => m.Key.x == node.Key.x).MaxBy(m => m.Key.y).Key,
                    'v' => map.Where(m => m.Key.x == node.Key.x).MinBy(m => m.Key.y).Key,
                    _ => throw new Exception()
                };

                next = blizz switch {
                    '>' => (wall.x + 1, wall.y),
                    '<' => (wall.x - 1, wall.y),
                    '^' => (wall.x, wall.y - 1),
                    'v' => (wall.x, wall.y + 1),
                    _ => throw new Exception()
                };
            }
            
            nextMapState[next].Add(blizz);
        }
    }
    
    // move me
    var pos = state.key;

    ICollection<(int x, int y)> moves = elfMoves.Select(e => (pos.x + e.x, pos.y + e.y)).Where(e => nextMapState.ContainsKey(e) && !nextMapState[e].Any()).ToArray();

    foreach( var move in moves)
    {
        var dist = Math.Abs(goal.x - move.x) + Math.Abs(goal.y - move.y);
        queue.Enqueue((move, state.time + 1, nextMapState), dist + state.time + 1);
    }
}

Console.WriteLine(minTimeToGoal);

void Print(IDictionary<(int x, int y), ICollection<char>> map, (int x, int y) pos)
{
    var minx = map.Min(m => m.Key.x);
    var maxx = map.Max(m => m.Key.x);
    var miny = map.Min(m => m.Key.y);
    var maxy = map.Max(m => m.Key.y);

    for (int j = miny; j <= maxy; j++)
    {
        for (int i = minx; i <= maxx; i++)
        {
            if (map[(i, j)].Contains('#'))
            {
                Console.Write('#');
            }
            else if (map[(i, j)].Count == 1)
            {
                Console.Write(map[(i, j)].Single());
            }
            else if (map[(i, j)].Count > 1)
            {
                Console.Write(map[(i, j)].Count);
            }
            else
            {
                if ((i, j) == pos)
                {
                    Console.Write('E');
                }
                else
                {
                    Console.Write(' ');
                }
            }
        }
        Console.WriteLine();
    }
}