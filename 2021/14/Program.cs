using System.Reflection.Emit;

var lines = File.ReadAllLines("./input.txt").ToList();

var starter = lines.First();

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

// chain = new LinkedList<char>();

// foreach(var character in starter)
// {
//     chain.AddLast(character);
// }

// // for every pair in the chain, figure out what is produced after a few steps and when you can know everything about what will happen in 100 steps

// foreach(var inst in instructions)
// {
//     var testStart = new LinkedList<char>();
    
//     testStart.AddLast(inst.Key.L);
//     testStart.AddLast(inst.Key.R);

//     int count = 0;

//     var previouslySeenFirstEntries = new List<char>{inst.Key.R};

//     var previousStates = new Dictionary<int, string>();
//     previousStates.Add(count, inst.Key.L.ToString() + inst.Key.R);

//     while(true)
//     {
//         var copy = testStart.ToList();

//         var current = testStart.Find(testStart.First());

//         for(int j = 0; j < copy.Count() - 1; j++)
//         {
//             var newCharacter = instructions[(copy[j], copy[j+1])];

//             testStart.AddAfter(current, newCharacter);

//             current = current.Next.Next;
//         }

//         count++;

//         previousStates.Add(count, string.Join("", testStart.ToList()));

//         if (previouslySeenFirstEntries.Contains(testStart.ToList()[1]))
//         {
//             if (previouslySeenFirstEntries.First() == testStart.ToList()[1])
//             {
//                 Console.WriteLine("head");
//             }
//             else
//             {
//                 Console.WriteLine("tail only");
//             }
//             break;
//         }
//         else
//         {
//             previouslySeenFirstEntries.Add(testStart.ToList()[1]);
//         }
        
//     }

//     var loopPair = testStart.ToList()[0].ToString() + testStart.ToList()[1];

//     // find first occurrence of loop pair in previous states
//     // determine remaining steps to 100
//     // do I need to do recursion instead??

//     Console.WriteLine(inst.Key.L.ToString() + inst.Key.R + " -> " + string.Join("", testStart.ToList()) + $" after {count} steps");
// }


var dictionaryOfResults = instructions.ToDictionary(i => i.Key, _ => (long)0);
for (int k = 0; k < starter.Length - 1; k ++)
{
    dictionaryOfResults.TryGetValue((starter[k], starter[k+1]), out var value);
    value += 1;
    dictionaryOfResults[(starter[k], starter[k+1])] = value;
}

for( int i = 0; i < 10; i++)
{
    var copy = dictionaryOfResults.ToList();

    foreach(var resi in copy)
    {
        if (resi.Value == 0)
        {
            continue;
        }

        var insertChar = instructions[resi.Key];
        
        var value = dictionaryOfResults[(resi.Key.L, insertChar)];
        value += resi.Value;
        dictionaryOfResults[(resi.Key.L, insertChar)] = value;

        value = dictionaryOfResults[(insertChar, resi.Key.R)];
        value += resi.Value;
        dictionaryOfResults[(insertChar, resi.Key.R)] = value;

        value = dictionaryOfResults[resi.Key];
        value -= resi.Value;
        dictionaryOfResults[resi.Key] = value;
    }
}

var alphabet = new Dictionary<char, long>();

foreach (var entry in dictionaryOfResults)
{
    alphabet.TryGetValue(entry.Key.L, out var value);
    value += entry.Value;
    alphabet[entry.Key.L] = value;

    alphabet.TryGetValue(entry.Key.R, out value);
    value += entry.Value;
    alphabet[entry.Key.R] = value;
}

var val = alphabet[starter.Last()];
alphabet[starter.Last()] = val + 1; // why?

var alphabetCop = alphabet.OrderBy(a => a.Value).ToList();

Console.WriteLine(Math.Abs(alphabetCop.First().Value / 2 - alphabetCop.Last().Value / 2));
// 3390034818250 too high