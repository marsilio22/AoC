var lines = File.ReadAllLines("input.txt");

var rules = new Dictionary<int, List<int>>();

var manuals = new List<int[]>();

foreach(var line in lines)
{
    if (line.Contains('|'))
    {
        var split = line.Split('|');

        var first = int.Parse(split[0]);
        var second = int.Parse(split[1]);

        var values = rules.GetValueOrDefault(first, new List<int>());

        values.Add(second);

        rules[first] = values;
    }
    else if (string.IsNullOrEmpty(line))
    {
        continue;
    }
    else 
    {
        manuals.Add(line.Split(",").Select(l => int.Parse(l)).ToArray());
    }
}

var validCount = 0;
var invalidCount = 0;

foreach(var manual in manuals)
{
    if (IsValid(manual, rules)) {
        validCount += manual[manual.Length /2];
    }
    else
    {
        var fixedMan = IsValidAndFix(manual, rules);

        invalidCount += fixedMan[fixedMan.Length /2];
    }
}

Console.WriteLine(validCount);
Console.WriteLine(invalidCount);



bool IsValid(int[] manual, Dictionary<int, List<int>> rules) 
{
    var valid = true;
    for (int i = 0; i < manual.Length; i++)
    {
        var page = manual[i];

        for(int j = i+1; j < manual.Length; j++)
        {
            if (rules.ContainsKey(manual[j]) && rules[manual[j]].Contains(page))
            {
                valid = false;
            }
        }
    }

    return valid;
}

int[] IsValidAndFix(int[] manual, Dictionary<int, List<int>> rules)
{
    for (int i = 0; i < manual.Length; i++)
    {
        var page = manual[i];

        for(int j = i + 1; j < manual.Length; j++)
        {
            if (rules.ContainsKey(manual[j]) && rules[manual[j]].Contains(page))
            {
                var newManual = manual;
                Swap(ref newManual[i], ref newManual[j]);
                return IsValidAndFix(newManual, rules);
            }
        }
    }

    return manual;
}

static void Swap(ref int a, ref int b)
{
    var temp = a;
    a = b;
    b = temp;
}