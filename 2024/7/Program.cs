var lines = File.ReadAllLines("./input.txt");

long count = 0;

foreach(var line in lines)
{
    var split = line.Split(": ");

    var target = long.Parse(split[0]);
    var operands = split[1].Split(" ").Select(s => long.Parse(s)).ToArray();

    var results = new HashSet<long>{operands[0]};

    operands = operands.TakeLast(operands.Length - 1).ToArray();

    foreach(var operand in operands)
    {
        var temp = results.SelectMany(r => new List<long> { r * operand, r + operand, long.Parse(r.ToString() + operand.ToString())}).ToHashSet();
        results = temp;
    }

    if (results.Contains(target))
    {
        count += target;
    }
}

Console.WriteLine(count);
