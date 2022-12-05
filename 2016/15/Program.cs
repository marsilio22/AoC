var lines = File.ReadAllLines("./input.txt");

var discs = new Dictionary<int, Disc>();

foreach(var line in lines)
{
    var split = line.Split(" ");

    var disc = new Disc {
        number = int.Parse(split[1][1].ToString()),
        positions = int.Parse(split[3]),
        initialPosition = int.Parse(split[11].Split(".")[0])
    };

    discs.Add(disc.number, disc);
}

for (int i = 0; i< 1_000_000; i++)
{
    var goodTime = true;
    for (int j = 1; j <= 6; j++)
    {
        goodTime = goodTime && ((discs[j].initialPosition + i + j) % discs[j].positions) == 0;
    }

    if (goodTime)
    {
        Console.WriteLine(i);
        break;
    }
}

var newDisc = new Disc
{
    number = 7,
    positions = 11,
    initialPosition = 0
};

discs.Add(7, newDisc);

for (int i = 0; i< 10_000_000; i++)
{
    var goodTime = true;
    for (int j = 1; j <= 7; j++)
    {
        goodTime = goodTime && ((discs[j].initialPosition + i + j) % discs[j].positions) == 0;
    }

    if (goodTime)
    {
        Console.WriteLine(i);
        break;
    }
}

class Disc 
{
    public int number { get; set; }
    public int positions { get; set; }
    public int initialPosition { get; set; }
}