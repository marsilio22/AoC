using System.Text.RegularExpressions;

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

foreach(var repl in replacements)
{
    for (int i = 0; i < target.Length;)
    {
        var index = target.IndexOf(repl.Key, i);

        if (index == -1)
        {
            break;
        }

        foreach(var m in repl.Value)
        {
            var newmol = target.Substring(0, index) + m + target.Substring(index + repl.Key.Length);
            newMols.Add(newmol);
        }

        i = index+1;
    }
}

Console.WriteLine(newMols.Count()); // 112 too low