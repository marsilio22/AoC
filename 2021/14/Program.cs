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

for (int j = 10;j <= 40; j+=30)
{
    var dictionaryOfResults = instructions.ToDictionary(i => i.Key, _ => (long)0);
    for (int k = 0; k < starter.Length - 1; k ++)
    {
        dictionaryOfResults.TryGetValue((starter[k], starter[k+1]), out var value);
        value += 1;
        dictionaryOfResults[(starter[k], starter[k+1])] = value;
    }

    for( int i = 0; i < j; i++)
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

    // the first and last characters only appear in one pair each, so pretend they don't
    var val = alphabet[starter.Last()];
    alphabet[starter.Last()] = val + 1;
    val = alphabet[starter.First()];
    alphabet[starter.First()] = val + 1;

    var alphabetCop = alphabet.OrderBy(a => a.Value).ToList();

    Console.WriteLine(Math.Abs(alphabetCop.First().Value / 2 - alphabetCop.Last().Value / 2));
}