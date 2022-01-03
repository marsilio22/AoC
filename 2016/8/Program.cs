var lines = File.ReadAllLines("./input.txt");

Dictionary<(int x, int y), char> screen = new Dictionary<(int x, int y), char>();

for (int j = 0; j < 6; j++)
{
    for (int i = 0; i < 50; i++)
    {
        screen[(i, j)] = ' ';
    }
}

foreach (var line in lines)
{
    if (line.StartsWith("rect "))
    {
        var split = line.Split(" ")[1].Split("x").Select(s => int.Parse(s)).ToList();

        var A = split[0];
        var B = split[1];

        for (int j = 0; j < B; j ++)
        {
            for (int i = 0; i < A; i ++)
            {
                screen[(i, j)] = '#';
            }
        }
    }
    else if (line.StartsWith("rotate row "))
    {
        var split = line.Split("=")[1].Split(" by ").Select(s => int.Parse(s)).ToList();

        var A = split[0];
        var B = split[1];

        var copy = screen.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        for (int i = 0; i < 50; i++)
        {
            
            var correspondingX = i - B;

            while (correspondingX < 0)
            {
                correspondingX += 50;
            }

            if (copy[(correspondingX, A)] == '#')
            {
                screen[(i, A)] = '#';
            }
            else 
            {
                screen[(i, A)] = ' ';
            }
        }        
    }
    else if (line.StartsWith("rotate column "))
    {
        var split = line.Split("=")[1].Split(" by ").Select(s => int.Parse(s)).ToList();

        var A = split[0];
        var B = split[1];

        var copy = screen.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        for (int j = 0; j < 6; j++)
        {
            
            var correspondingY = j - B;

            while(correspondingY < 0)
            {
                correspondingY += 6;
            }

            if (copy[(A, correspondingY)] == '#')
            {
                screen[(A, j)] = '#';
            }
            else 
            {
                screen[(A, j)] = ' ';
            }
        }
    }
}

Console.WriteLine(screen.Count(c => c.Value == '#'));


for (int j = 0; j < 6; j++)
{
    for (int i = 0; i < 50; i++)
    {
        Console.Write(screen[(i, j)]); // UPOJFLBCEZ
    }
    Console.WriteLine();
}
