var line = File.ReadAllLines("./input.txt")[0];

var start = line.Split(" ").Select(n => long.Parse(n)).ToArray();

// a record of each possible value, and what they turn into.
var cache = new Dictionary<long, ICollection<long>>();

while (start.Any())
{
    var nextSet = new List<long>();

    foreach(var thing in start)
    {
        if(!cache.ContainsKey(thing))
        {
            var next = new List<long>();
            
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
            
            cache.Add(thing, next);
            nextSet.AddRange(next);
        }
    }

    start = nextSet.ToArray();
}

// a record of how many of each value we currently have
Dictionary<long, long> values = line.Split(" ").Select(n => long.Parse(n)).ToDictionary(c => c, _ => 1l);

for (int i = 0; i < 75; i++)
{
    // terminology here is insanely confusing but basically
    // - Take the current set of values, and the counts of how many they are
    // - for each one, look in the Cache and see what they turn into
    // - add COUNT of those to the next set of values
    // - repeat
    var nextValues = new Dictionary<long, long>();

    foreach((long val, long existingCount) in values)
    {
        var nexts = cache[val];

        foreach(var next in nexts)
        {
            var nextCountSoFar = nextValues.GetValueOrDefault(next, 0);
            nextValues[next] = nextCountSoFar + existingCount;
        }
    }

    values = nextValues;
    if (i == 24 || i == 74)
    {
        Console.WriteLine(values.Values.Sum());
    }
}
