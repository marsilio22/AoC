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

for (int i = 0; i < 256; i++)
{
    var copy = parsed.ToDictionary(p => p.Key, p => p.Value);

    // this will do lol
    parsed[0] = copy[1];
    parsed[1] = copy[2];
    parsed[2] = copy[3];
    parsed[3] = copy[4];
    parsed[4] = copy[5];
    parsed[5] = copy[6];
    parsed[6] = copy[7] + copy[0];
    parsed[7] = copy[8];
    parsed[8] = copy[0];
}

Console.WriteLine(parsed.Values.Sum());

