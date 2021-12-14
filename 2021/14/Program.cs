using System.Reflection.Emit;

var lines = File.ReadAllLines("./input.txt").ToList();

var starter = "FV";

lines = lines.Skip(2).ToList();

var instructions = new Dictionary<(char L, char R), char>();

foreach(var line in lines)
{
    var splitLine = line.Split(" -> ");

    var key = (splitLine[0][0], splitLine[0][1]);
    instructions[key] = splitLine[1][0];
}

var chain = new LinkedList<char>();

foreach(var character in starter)
{
    chain.AddLast(character);
}

Console.WriteLine(string.Join("", chain.ToList()));

for (int i = 0; i < 10; i++)
{
    var copy = chain.ToList();

    var current = chain.Find(chain.First());

    for(int j = 0; j < copy.Count() - 1; j++)
    {
        var newCharacter = instructions[(copy[j], copy[j+1])];

        chain.AddAfter(current, newCharacter);

        current = current.Next.Next;
    }
    
    Console.WriteLine(string.Join("", chain.ToList()));
}

var ans = chain.ToList().GroupBy(c => c).ToDictionary(c => c.Key, c => (long)c.Count()).OrderBy(d => d.Value);

Console.WriteLine(Math.Abs(ans.First().Value - ans.Last().Value));


// part2

chain = new LinkedList<char>();

foreach(var character in starter)
{
    chain.AddLast(character);
}

// for every pair in the chain, figure out what is produced after a few steps and when you can know everything about what will happen in 100 steps

foreach(var inst in instructions)
{
    var testStart = new LinkedList<char>();
    
    testStart.AddLast(inst.Key.L);
    testStart.AddLast(inst.Key.R);

    int count = 0;

    var previouslySeenFirstEntries = new List<char>{inst.Key.R};

    while(true)
    {
        var copy = testStart.ToList();

        var current = testStart.Find(testStart.First());

        for(int j = 0; j < copy.Count() - 1; j++)
        {
            var newCharacter = instructions[(copy[j], copy[j+1])];

            testStart.AddAfter(current, newCharacter);

            current = current.Next.Next;
        }

        count++;

        if (previouslySeenFirstEntries.Contains(testStart.ToList()[1]))
        {
            if (previouslySeenFirstEntries.First() == testStart.ToList()[1])
            {
                Console.WriteLine("head");
            }
            else
            {
                Console.WriteLine("tail only");
            }
            break;
        }
        else
        {
            previouslySeenFirstEntries.Add(testStart.ToList()[1]);
        }
        
    }

    Console.WriteLine(inst.Key.L.ToString() + inst.Key.R + " -> " +string.Join("", testStart.ToList()) + $" after {count} steps");
}