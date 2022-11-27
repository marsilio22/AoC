var lines = File.ReadAllLines("./input.txt");

var replacements = new Dictionary<string, ICollection<string>>();

foreach (var line in lines)
{
    if (line.Contains("=>"))
    {
        var split = line.Split(" => ", StringSplitOptions.TrimEntries);

        if (!replacements.TryGetValue(split[0], out var repl))
        {
            repl = new List<string>();
        }

        repl.Add(split[1]);

        replacements[split[0]] = repl;
    }
}

var target = lines.Last();

Console.WriteLine();

var newMols = new HashSet<string>();

foreach (var repl in replacements)
{
    for (int i = 0; i < target.Length;)
    {
        var index = target.IndexOf(repl.Key, i);

        if (index == -1)
        {
            break;
        }

        foreach (var m in repl.Value)
        {
            var newmol = target.Substring(0, index) + m + target.Substring(index + repl.Key.Length);
            newMols.Add(newmol);
        }

        i = index + 1;
    }
}

Console.WriteLine(newMols.Count());

var reverseDict = replacements.SelectMany(c => c.Value.Select(d => (c.Key, d))).ToDictionary(c => c.d, c => c.Key);

var groups = reverseDict.GroupBy(d => d.Key.Contains(d.Value + d.Value)).ToDictionary(d => d.Key, d => d.ToList());

var multipliers = groups[true].ToDictionary(d => d.Key, d => d.Value);
var nonMultipliers = groups[false].ToDictionary(d => d.Key, d => d.Value);

groups = nonMultipliers.GroupBy(d => d.Key.Contains(d.Value)).ToDictionary(d => d.Key, d => d.ToList());

var selfContainers = groups[true].ToDictionary(d => d.Key, d => d.Value);
var nonSelfContainers = groups[false].ToDictionary(d => d.Key, d => d.Value);

Console.WriteLine();

var count = 0;

while (target != "e")
{
    while (multipliers.Any(s => target.Contains(s.Key)))
    {
        foreach (var repl in multipliers)
        {
            var index = target.IndexOf(repl.Key);

            if (index == -1)
            {
                break;
            }
            
            target = target.Substring(0, index) + repl.Value + target.Substring(index + repl.Key.Length);
            count++;
        }
    }

    if (selfContainers.Any(s => target.Contains(s.Key)))
    {
        var replacer = selfContainers.First(d => target.Contains(d.Key));

        var index = target.IndexOf(replacer.Key);

        if (index == -1)
        {
            break;
        }
        
        target = target.Substring(0, index) + replacer.Value + target.Substring(index + replacer.Key.Length);
        count++;
        continue;
    }

    if (nonSelfContainers.Any(s => target.Contains(s.Key)))
    {
        var replacer = nonSelfContainers.First(d => target.Contains(d.Key));

        var index = target.IndexOf(replacer.Key);

        if (index == -1)
        {
            break;
        }
        
        target = target.Substring(0, index) + replacer.Value + target.Substring(index + replacer.Key.Length);
        count++;
        continue;
    }
}

Console.WriteLine(count);