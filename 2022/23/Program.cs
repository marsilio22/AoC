var lines = File.ReadAllLines("./input");

Console.WriteLine(DateTime.Now);
var elves = new HashSet<(int x, int y)>();
var row = 0;
foreach(var line in lines)
{
    var col = 0;
    foreach(var character in line)
    {
        if (character == '#')
        {
            elves.Add((col, row));
        }
        col++;
    }
    row++;
}

var moveDict = new Dictionary<(int x, int y), ICollection<(int x, int y)>>{
    [(0, -1)] = new [] { (-1, -1), (0, -1), (1, -1)},
    [(0, 1)] = new [] { (-1, 1), (0, 1), (1, 1)},
    [(-1, 0)] = new [] { (-1, -1), (-1, 0), (-1, 1)},
    [(1, 0)] = new [] { (1, -1), (1, 0), (1, 1)},
};

var moves = new LinkedList<KeyValuePair<(int x, int y), ICollection<(int x, int y)>>>();
foreach(var thing in moveDict)
{
    moves.AddLast(thing);
}

var firstMoveToTry = moves.First;

var round = 0;

while (true)
{
    var elfProposedLocations = new Dictionary<(int x, int y), (int x, int y)>();

    var elvesWhoCouldMove = elves.Where(e => 
        // only elves who have at least one neighbour will consider moving
        new [] {
            (e.x - 1, e.y - 1),
            (e.x - 1, e.y),
            (e.x - 1, e.y + 1),
            (e.x, e.y + 1),
            (e.x + 1, e.y + 1),
            (e.x + 1, e.y),
            (e.x + 1, e.y - 1),
            (e.x, e.y - 1),
        }.Any(b => elves.Contains(b)));

        if (!elvesWhoCouldMove.Any())
        {
            Console.WriteLine($"{DateTime.Now}: {round + 1}");
            return;
        }
        
        elvesWhoCouldMove = elvesWhoCouldMove.Where(e => 
            // only elves who don't have two diagonally opposite neighbours have a chance of actually moving
            !
            new [] {
                (e.x - 1, e.y - 1),
                (e.x + 1, e.y + 1)
            }.All(b => elves.Contains(b))
            &&
            !
            new [] {
                (e.x + 1, e.y - 1),
                (e.x - 1, e.y + 1)
            }.All(b => elves.Contains(b))
        ).ToHashSet();

    foreach(var elf in elvesWhoCouldMove)
    {
        var move = firstMoveToTry;
        do{
            if (elves.Any(e => move.Value.Value.Select(d => (elf.x + d.x, elf.y + d.y)).Contains(e)))
            {
                move = move.Next ?? moves.First;
            }
            else 
            {
                elfProposedLocations[elf] = (elf.x + move.Value.Key.x, elf.y + move.Value.Key.y);
                break;
            }
        } while (move != firstMoveToTry);
    }

    var locationProposingElves = elfProposedLocations.GroupBy(e => e.Value);

    foreach(var proposedLocation in locationProposingElves)
    {
        if (proposedLocation.Count() > 1)
        {
            foreach(var elf in proposedLocation)
            {
                elfProposedLocations.Remove(elf.Key);
            }
        }
    }

    foreach(var elf in elfProposedLocations)
    {
        elves.Remove(elf.Key);
        if (!elves.Add(elf.Value))
        {
            throw new Exception();
        }
    }

    // Print (elves);
    // Console.WriteLine("---------------------");
    firstMoveToTry = firstMoveToTry.Next ?? moves.First;
    round++;

    if (round == 10)
    {
        var minX = elves.Min(e => e.x);
        var maxX = elves.Max(e => e.x);
        var minY = elves.Min(e => e.y);
        var maxY = elves.Max(e => e.y);

        var ans = (maxX - minX + 1) * (maxY - minY + 1) - elves.Count;

        Console.WriteLine($"{DateTime.Now}: {ans}");
    }
}


void Print(HashSet<(int x, int y)> elves)
{
    var minX = elves.Min(e => e.x);
    var maxX = elves.Max(e => e.x);
    var minY = elves.Min(e => e.y);
    var maxY = elves.Max(e => e.y);

    for(int j = minY; j <= maxY; j++)
    {
        for (int i = minX; i <= maxX; i++)
        {
            if (elves.Contains((i, j)))
            {
                Console.Write('#');
            }
            else 
            {
                Console.Write('.');
            }
        }
        Console.WriteLine();
    }
}