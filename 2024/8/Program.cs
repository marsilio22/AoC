using System.Globalization;

var lines = File.ReadAllLines("./input.txt");

var map = new Dictionary<char, (int x, int y)[]>();

int x = 0, y = 0;

int maxX = lines[0].Length;
int maxY = lines.Length;

foreach(var line in lines)
{
    foreach (var character in line)
    {
        if (character != '.')
        {
            var antennae = map.GetValueOrDefault(character, Array.Empty<(int, int)>());

            var newAntennae= antennae.Append((x, y));
            map[character] = newAntennae.ToArray();
        }
        x++;
    }
    y++;
    x = 0;
}

var antinodes  = new HashSet<(int x, int y)>();

foreach(var frequency in map)
{
    var antennae = frequency.Value;

    for(int i = 0; i < antennae.Length; i++)
    {
        for(int j = i+1; j < antennae.Length; j++)
        {
            (int x, int y) distance = (antennae[j].x - antennae[i].x, (antennae[j].y - antennae[i].y));

            antinodes.Add((antennae[j].x + distance.x, antennae[j].y + distance.y));
            antinodes.Add((antennae[i].x - distance.x, antennae[i].y - distance.y));
        }
    }
}

Console.WriteLine(antinodes.Count(a => a.x >= 0 && a.x < maxX && a.y >= 0 && a.y < maxY));

antinodes  = new HashSet<(int x, int y)>();

foreach(var frequency in map)
{
    var antennae = frequency.Value;

    for(int i = 0; i < antennae.Length; i++)
    {
        for(int j = i+1; j < antennae.Length; j++)
        {
            (int x, int y) distance = (antennae[j].x - antennae[i].x, (antennae[j].y - antennae[i].y));

            for(int k = 0; k < maxX + maxY; k++)
            {
                antinodes.Add((antennae[j].x + k * distance.x, antennae[j].y + k * distance.y));
                antinodes.Add((antennae[i].x - k * distance.x, antennae[i].y - k * distance.y));
            }
        }
    }
}

Console.WriteLine(antinodes.Count(a => a.x >= 0 && a.x < maxX && a.y >= 0 && a.y < maxY));
