var line = File.ReadAllLines("./input.txt")[0];

var start = line.Split(" ").Select(n => long.Parse(n)).ToArray();

var seenBefore = new HashSet<long>();

while (start.Any())
{
    // foreach(var item in start)
    // {
    //     Console.Write(item.ToString() + " ");
    // }
    // Console.WriteLine();

    var next = new List<long>();

    foreach(var thing in start)
    {
        if(!seenBefore.Contains(thing))
        {
            seenBefore.Add(thing);
            
            if (thing == 0)
            {
                next.Add(1);
            }
            else if (thing.ToString().Length % 2 == 0)
            {
                var str = thing.ToString();

                var first = str.Substring(0, str.Length / 2);
                var second = str.Substring(str.Length / 2);

                next.Add(long.Parse(first));
                next.Add(long.Parse(second));
            }
            else
            {
                next.Add(thing * 2024);
            }
        }
    }

    start = next.ToArray();
}

Console.WriteLine(seenBefore.Count);