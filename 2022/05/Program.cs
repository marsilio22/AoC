var input = File.ReadAllLines("./input.txt");

Console.WriteLine(Move9000(input));
Console.WriteLine(Move9001(input));

Dictionary<int, Stack<char>> GetStacks(string[] input)
{
    Dictionary<int, Stack<char>> stacks = new Dictionary<int, Stack<char>>();

    for (int i = input.Length - 1; i >= 0; i--)
    {
        if (!input[i].StartsWith("["))
        {
            continue;
        }

        for (int j = 1; j <= 9; j++)
        {
            if (!stacks.ContainsKey(j))
            {
                stacks.Add(j, new Stack<char>());
            }

            var character = input[i][(j-1) * 4 + 1];

            if (character != ' ')
            {
                stacks[j].Push(character);
            }
        }
    }

    return stacks;
}

string Move9000(string[] input)
{
    var stacks = GetStacks(input);
    foreach(var line in input)
    {
        if (!line.StartsWith("move"))
        {
            continue;
        }

        var split = line.Split(" ");

        var numberToMove = int.Parse(split[1]);
        var source = int.Parse(split[3]);
        var dest = int.Parse(split[5]);

        for (int i = 0; i < numberToMove; i++)
        {
            stacks[dest].Push(stacks[source].Pop());
        }
    }

    return string.Join("", stacks.Values.Select(s => s.Peek()));
}

string Move9001(string[] input)
{
    var stacks = GetStacks(input);

    foreach(var line in input)
    {
        if (!line.StartsWith("move"))
        {
            continue;
        }

        var split = line.Split(" ");

        var numberToMove = int.Parse(split[1]);
        var source = int.Parse(split[3]);
        var dest = int.Parse(split[5]);

        var intermediateStack = new Stack<char>();
        for (int i = 0; i < numberToMove; i++)
        {
            intermediateStack.Push(stacks[source].Pop());
        }

        for (int i = 0; i < numberToMove; i++)
        {
            stacks[dest].Push(intermediateStack.Pop());
        }
    }

    return string.Join("", stacks.Values.Select(s => s.Peek()));
}