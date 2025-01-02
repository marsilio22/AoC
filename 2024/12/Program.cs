using System.Runtime.CompilerServices;

var lines = File.ReadAllLines("./input.txt");

var map = new Dictionary<(int x, int y), char>();

int x = 0, y = 0;

foreach(var line in lines)
{
    foreach(var character in line)
    {
        map[(x, y)] = character;
        x++;
    }

    y++;
    x = 0;
}

var costs = new Dictionary<char, long>();

ICollection<(int x, int y)> directions = new [] { (0, 1), (0, -1), (1, 0), (-1, 0)};

var allValues = map.Values.Distinct();

var regions = new Dictionary<(int, char), Dictionary<(int x, int y), int>>();

var count = 0;
while(map.Any())
{
    // Console.WriteLine(map.Count());

    var next = map.First();
    map.Remove(next.Key);

    var currentRegion = new HashSet<(int, int)>();

    var toCheck = new Queue<(int, int)>();
    var checkedCoords = new HashSet<(int, int)>();

    toCheck.Enqueue(next.Key);

    while(toCheck.Any())
    {
        // Console.WriteLine(toCheck.Count());

        (int x, int y) current = toCheck.Dequeue();
        map.Remove(current);
        checkedCoords.Add(current);
        currentRegion.Add(current);
        var neighbourCoords = directions.Select(d => (d.x + current.x, d.y + current.y));
        var neighbours = map.Where(m => m.Value == next.Value && !toCheck.Contains(m.Key) && !checkedCoords.Contains(m.Key) && neighbourCoords.Contains(m.Key)).Select(m => m.Key);

        foreach(var neighbour in neighbours)
        {
            toCheck.Enqueue(neighbour);
        }
    }

    var regionWithFenceCount = new Dictionary<(int x, int y), int>();

    foreach((int x, int y) plot in currentRegion)
    {
        var neighbourCount = directions.Count(d => currentRegion.Contains((plot.x + d.x, plot.y + d.y)));

        regionWithFenceCount[plot] = 4 - neighbourCount;
    }

    regions[(count, next.Value)] = regionWithFenceCount;
    count++;
}

var total = 0;

foreach(var region in regions)
{
    // Console.WriteLine(region.Key);
    // Console.WriteLine($"\t {region.Value.Count}");
    // Console.WriteLine($"\t {region.Value.Values.Sum()}");

    total += region.Value.Count * region.Value.Values.Sum();
}

Console.WriteLine(total);

var total2 = 0;
foreach(var region in regions)
{
    // for each cell in the region, count how many corners it induces. The options are:
    /**
        000
        010  - no neighbours, 4 corners, one orientation
        000

        000
        010  - one neighbour, 2 corners, four orientations
        010

        010
        010  - two neighbours, 0 corners, two orientations
        010

        110
        110  - two neighbours, 1 corner, four orientations
        000

        010
        110  - two neighbours, 2 corners, four orientations
        000

        000
        111  - three neighbours, 2 corners, four orientations
        010

        000
        111  - three neighbours, 1 corner, eight orientations
        110

        000
        111  - three neighbours, no corners, four orientations
        111

        010
        111  - four neighbours, 4 corners, one orientation
        010

        111
        111  - four neighbours, 0 corners, one orientation
        111
    */

    var corners = 0;
    
    foreach(var coord in region.Value)
    {
        var neighbours = region.Value.Where(r => directions.Select(d => (coord.Key.x + d.x, coord.Key.y + d.y)).Contains(r.Key));
        var neighbourCount = neighbours.Count();

        if (neighbourCount == 0)
        {
            corners += 4;
        }

        if (neighbourCount == 1)
        {
            corners += 2;
        }

        if (neighbourCount == 2)
        {
            if (neighbours.All(n => n.Key.x == coord.Key.x) || neighbours.All(n => n.Key.y == coord.Key.y))
            {
                corners += 0; // this is a straight line with no corners
            }
            else
            {
                // one neighbour is horizontal, and one is vertial, so identify them as such
                var horizontalNeighbour = neighbours.Single(n => n.Key.y == coord.Key.y);
                var verticalNeighbour = neighbours.Single(n => n.Key.x == coord.Key.x);

                var diagonalNeighbourPresent = region.Value.ContainsKey((horizontalNeighbour.Key.x, verticalNeighbour.Key.y));

                if (diagonalNeighbourPresent)
                {
                    corners += 1;
                }
                else
                {
                    corners += 2;
                }
            }
        }

        if (neighbourCount == 3)
        {
            // split into whether there are two neighbours north and south, or two neighbours east and west
            var horizontalNeighbours = neighbours.Where(n => n.Key.y == coord.Key.y).ToArray();
            var verticalNeighbours = neighbours.Where(n => n.Key.x == coord.Key.x).ToArray();

            // todo conglomerate these...
            if (horizontalNeighbours.Count() == 2)
            {
                var verticallyDiagonalNeighbours = region.Value.Where(r => 
                    r.Key.y == verticalNeighbours.Single().Key.y && 
                    horizontalNeighbours.Select(h =>h.Key.x).Contains(r.Key.x));

                if (verticallyDiagonalNeighbours.Count() == 0)
                {
                    corners += 2;
                }

                if (verticallyDiagonalNeighbours.Count() == 1)
                {
                    corners += 1;
                }

                if (verticallyDiagonalNeighbours.Count() == 2)
                {
                    corners += 0;
                }
            }

            if (verticalNeighbours.Count() == 2)
            {
                var horizontallyDiagonalNeighbours = region.Value.Where(r => 
                    r.Key.x == horizontalNeighbours.Single().Key.x && 
                    verticalNeighbours.Select(h => h.Key.y).Contains(r.Key.y));

                if (horizontallyDiagonalNeighbours.Count() == 0)
                {
                    corners += 2;
                }

                if (horizontallyDiagonalNeighbours.Count() == 1)
                {
                    corners += 1;
                }

                if (horizontallyDiagonalNeighbours.Count() == 2)
                {
                    corners += 0;
                }
            }
        }

        if (neighbourCount == 4)
        {
            (int x, int y)[] diagonals = new [] {(1, 1), (-1, 1), (-1, -1), (1, -1)};

            var diagCount = region.Value.Count(r => diagonals.Select(d => (d.x + coord.Key.x, d.y + coord.Key.y)).Contains(r.Key));

            corners += (4 - diagCount);
        }
    }

    Console.WriteLine($"{region.Key}    {region.Value.Count} * {corners} = {region.Value.Count * corners}");

    total2 += region.Value.Count * corners;
}

Console.WriteLine(total2);


