var lines = File.ReadAllLines("input.txt");

var dict = new Dictionary<string, int>();

var part1 = GetA(lines, dict);

dict = new Dictionary<string, int>();
dict["b"] = part1;

var part2 = GetA(lines, dict);

Console.WriteLine(part1);
Console.WriteLine(part2);

static int GetA(string[] lines, Dictionary<string, int> dict)
{
    while (!dict.ContainsKey("a"))
    {
        foreach (var line in lines)
        {
            var split = line.Split(" -> ");
            var instr = split[0];
            var output = split[1];

            if (dict.ContainsKey(output))
            {
                continue;
            }

            if (int.TryParse(instr, out var value) || dict.TryGetValue(instr, out value))
            {
                dict.Add(output, value);
                continue;
            }

            var splitInstr = instr.Split(" ");

            var firstExists = dict.TryGetValue(splitInstr.First(), out int first);
            var secondExists = dict.TryGetValue(splitInstr.Last(), out int second);

            if (!firstExists && int.TryParse(splitInstr.First(), out value))
            {
                firstExists = true;
                first = value;
            }
            if (!secondExists && int.TryParse(splitInstr.Last(), out value))
            {
                secondExists = true;
                second = value;
            }

            if (instr.Contains("AND") && firstExists && secondExists)
            {
                dict[output] = first & second;
            }
            else if (instr.Contains("OR") && firstExists && secondExists)
            {
                dict[output] = first | second;
            }
            else if (instr.Contains("LSHIFT") && firstExists)
            {
                var amount = int.Parse(splitInstr.Last());
                dict[output] = first << amount;
            }
            else if (instr.Contains("RSHIFT") && firstExists)
            {
                var amount = int.Parse(splitInstr.Last());
                dict[output] = first >> amount;
            }
            else if (instr.Contains("NOT") && secondExists)
            {
                dict[output] = ~second;
            }

        }

        dict = dict.ToDictionary(d => d.Key, d => d.Value < 0 ? d.Value + 65536 : d.Value);
    }

    var result = dict["a"];
    return result;
}