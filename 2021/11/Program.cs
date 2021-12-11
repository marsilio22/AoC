var lines = File.ReadAllLines("./input.txt");

IDictionary<(int x, int y), Octopus> map = new Dictionary<(int x, int y), Octopus>();

for(int j = 0; j < 10; j++)
{
    for (int i = 0; i < 10; i++)
    {
        map[(i, j)] = new Octopus((i, j), int.Parse(lines[j][i].ToString()));
    }
}

int flashes = 0;
var k = 1; // we count steps from 1
while (true)
{
    foreach(var octopus in map)
    {
        octopus.Value.Energy++;
    }

    while(map.Any(m => m.Value.Energy > 9 && !m.Value.HasFlashedThisStep))
    {   
        var flashers = map.Where(m => m.Value.Energy > 9 && !m.Value.HasFlashedThisStep).ToList();

        foreach(var flasher in flashers)
        {
            flasher.Value.HasFlashedThisStep = true;
            flashes++;

            foreach(var coord in flasher.Value.Adjacents)
            {
                map[coord].Energy++;
            }
        }
    }

    if (k == 100)
    {
        Console.WriteLine($"After 100 steps there were {flashes} flashes");
    }

    if (map.All(o => o.Value.HasFlashedThisStep))
    {
        Console.WriteLine($"All octopuses flashed on step {k}");
        break;
    }

    foreach(var octopus in map)
    {
        octopus.Value.Reset();
    }
    k++;
}

public class Octopus 
{
    public (int x, int y) Position;

    public int Energy;

    public bool HasFlashedThisStep;

    public ICollection<(int x, int y)> Adjacents;

    public Octopus((int x, int y) position, int initialEnergy)
    {
        this.Position = position;
        this.Energy = initialEnergy;

        // work out the adjacent cells on init, so that we aren't doing it every step
        this.Adjacents = new List<(int x, int y)>
        {
            (Position.x - 1, Position.y - 1),
            (Position.x    , Position.y - 1),
            (Position.x + 1, Position.y - 1),
            
            (Position.x - 1, Position.y),
            (Position.x + 1, Position.y),
            
            (Position.x - 1, Position.y + 1),
            (Position.x    , Position.y + 1),
            (Position.x + 1, Position.y + 1)
        };

        // limit adjacents to known 10*10 grid size
        this.Adjacents = this.Adjacents.Where(a => a.x >= 0 && a.x <= 9 && a.y >= 0 && a.y <= 9).ToList();
    }

    public void Reset()
    {
        if (this.HasFlashedThisStep)
        {
            this.HasFlashedThisStep = false;
            this.Energy = 0;
        }
    }
}