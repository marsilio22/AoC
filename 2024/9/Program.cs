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

var linkedMap = new LinkedList<(string id, int width)>(map);

while(linkedMap.Any(m => m.id.Equals("free")))
{
    var nextGap = linkedMap.First(m => m.id.Equals("free"));
    var nextFill = linkedMap.Last(m => !m.id.Equals("free"));

    var gapNode = linkedMap.Find(nextGap);
    var fillNode = linkedMap.FindLast(nextFill);

    if (nextGap.width > nextFill.width)
    {
        linkedMap.Remove(fillNode);
        linkedMap.AddBefore(gapNode, fillNode);
        gapNode.ValueRef.width = nextGap.width - nextFill.width;
    }
    else if (nextGap.width == nextFill.width)
    {
        linkedMap.Remove(fillNode);
        linkedMap.AddBefore(gapNode, fillNode);
        linkedMap.Remove(gapNode);
    }
    else if (nextGap.width < nextFill.width)
    {
        linkedMap.AddBefore(gapNode, new LinkedListNode<(string id, int width)>((nextFill.id, nextGap.width)));
        linkedMap.Remove(gapNode);
        fillNode.ValueRef.width = nextFill.width - nextGap.width;
    }
    
    while (linkedMap.Last.Value.id.Equals("free"))
    {
        linkedMap.RemoveLast();
    }
}

long sum = 0;
var index = 0;

foreach (var thing in linkedMap)
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
Console.WriteLine(sum);













fileId--;
linkedMap = new LinkedList<(string id, int width)>(map);

while(fileId >= 0)
{
    
    // foreach (var thing in linkedMap)
    // {
    //     var notFree = int.TryParse(thing.id, out int intId);

    //     if (notFree)
    //     {
    //         for (int j = 0; j < thing.width; j++)
    //         {
    //             Console.Write(intId);
    //         }
    //     }
    //     else
    //     {
    //         for (int j = 0; j < thing.width; j++)
    //         {
    //             Console.Write('.');
    //         }
    //     }
    // }
    // Console.WriteLine();

    var nextFill = linkedMap.Last(m => m.id.Equals(fileId.ToString()));
    var nextGap = linkedMap.FirstOrDefault(m => m.id.Equals("free") && m.width >= nextFill.width);

    if (nextGap == default)
    {
        fileId --;
        continue;
    }

    var gapNode = linkedMap.Find(nextGap);
    var fillNode = linkedMap.FindLast(nextFill);

    var gapFound = false;
    var fillFound = false;

    foreach(var thing in linkedMap)
    {
        if (thing == nextGap)
        {
            gapFound = true;
        }

        if (thing == nextFill)
        {
            fillFound = true;
        }

        if (gapFound || fillFound)
        {
            break;
        }
    }

    if (fillFound)
    {
        fileId--;
        continue;
    }

    if (nextGap.width > nextFill.width)
    {
        linkedMap.AddAfter(fillNode, new LinkedListNode<(string id, int width)>(("X", fillNode.Value.width)));
        linkedMap.Remove(fillNode);
        linkedMap.AddBefore(gapNode, fillNode);
        gapNode.ValueRef.width = nextGap.width - nextFill.width;
    }
    else if (nextGap.width == nextFill.width)
    {        
        linkedMap.AddAfter(fillNode, new LinkedListNode<(string id, int width)>(("X", fillNode.Value.width)));
        linkedMap.Remove(fillNode);
        linkedMap.AddBefore(gapNode, fillNode);
        linkedMap.Remove(gapNode);
    }
    
    fileId--;
}

sum = 0;
index = 0;

foreach (var thing in linkedMap)
{
    var notFree = int.TryParse(thing.id, out int intId);

    if (notFree)
    {
        for (int j = 0; j < thing.width; j++)
        {
            sum += intId * index;
            // Console.Write(intId);
            index++;
        }
    }
    else
    {
        // for (int j = 0; j < thing.width; j++)
        // {
        //     Console.Write('.');
        // }

        index+=thing.width;
    }
}
Console.WriteLine();
Console.WriteLine(sum);

// 0099211177744.333..5555.6666..8888
// 00992111777.44.333....5555.6666.....8888..