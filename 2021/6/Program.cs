var line = File.ReadAllLines("./input.txt")[0].Split(",").Select(s => int.Parse(s)).ToList();

var parsed = new Dictionary<int, long>
{
    {0, 0},
    {1, 0},
    {2, 0},
    {3, 0},
    {4, 0},
    {5, 0},
    {6, 0},
    {7, 0},
    {8, 0}
}; // keyed by day, value is the number of fish

foreach(var num in line)
{
    parsed[num] = parsed[num] + 1;
}

Iterate(parsed, 80);

Console.WriteLine(parsed.Values.Sum());

Iterate(parsed, 256-80);

Console.WriteLine(parsed.Values.Sum());

static void Iterate(Dictionary<int, long> fish, int iterations)
{
    for (int i = 0; i < iterations; i++)
    {
        var copy = fish.ToDictionary(p => p.Key, p => p.Value);

        // this will do lol
        fish[0] = copy[1];
        fish[1] = copy[2];
        fish[2] = copy[3];
        fish[3] = copy[4];
        fish[4] = copy[5];
        fish[5] = copy[6];
        fish[6] = copy[7] + copy[0];
        fish[7] = copy[8];
        fish[8] = copy[0];
    }
}