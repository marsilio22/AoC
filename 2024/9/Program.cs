var line = File.ReadAllLines("./input.txt")[0];

int i = 0;
var map = new List<(string id, int width)>(line.Length);

var fileId = 0;

foreach(var character in line)
{
    if (i % 2 == 0)
    {
        map.Add((fileId.ToString(), int.Parse(character.ToString())));
        fileId++;
    }
    else
    {
        map.Add(("free", int.Parse(character.ToString())));
    }

    i++;
}

while(map.Any(m => m.id.Equals("free")))
{
    // foreach (var thing in map)
    // {
    //     var notFree = int.TryParse(thing.id, out int intId);

    //     for (int j = 0; j < thing.width; j++)
    //     {
    //         if (notFree)
    //         {
    //             Console.Write(intId);
    //         }
    //         else
    //         {
    //             Console.Write('.');
    //         }
    //     }
    // }
    // Console.WriteLine();
    
    var nextGap = map.First(m => m.id.Equals("free"));
    var nextFill = map.Last(m => !m.id.Equals("free"));
    var fillIndex = map.LastIndexOf(nextFill);
    var gapIndex = map.IndexOf(nextGap);

    if (nextGap.width > nextFill.width)
    {
        map.RemoveAt(fillIndex);
        map[gapIndex] = (nextGap.id, nextGap.width - nextFill.width);
        map.Insert(gapIndex, nextFill);
    }
    else if (nextGap.width == nextFill.width)
    {
        map.RemoveAt(fillIndex);
        map[gapIndex] = nextFill;
    }
    else if (nextGap.width < nextFill.width)
    {
        map.Insert(gapIndex, (nextFill.id, nextGap.width));
        map.RemoveAt(gapIndex);

        map[fillIndex] = (nextFill.id, nextFill.width - nextGap.width);
    }
    
    while (map[map.Count - 1].id.Equals("free"))
    {
        map.RemoveAt(map.Count - 1);
    }
}

long sum = 0;
var index = 0;

foreach (var thing in map)
{
    var intId = int.Parse(thing.id);

    for (int j = 0; j < thing.width; j++)
    {
        sum += intId * index;
        // Console.Write(intId);
        index++;
    }
}
Console.WriteLine();

Console.WriteLine(sum); // 889144615 too low
