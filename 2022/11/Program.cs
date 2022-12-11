var input = File.ReadAllLines("./input");

var multiple = input.Where(line => line.Contains("Test")).Select(line => int.Parse(line.Split(" ").Last())).Aggregate((a, b) => a*b);

(int reps, Func<long, long> worry) part1 = (20, d => d/3);
(int reps, Func<long, long> worry) part2 = (10000, d => d % multiple);

foreach (var part in new [] {part1, part2})
{
    var monkeys = ParseInput(input);
    for (int i = 0; i < part.reps; i ++)
    {
        foreach(var monkey in monkeys.Values)
        {
            foreach (var item in monkey.Items)
            {
                var newWorry = part.worry(monkey.Inspection(item));
                var target = monkey.Test(newWorry) ? monkey.TrueTarget : monkey.FalseTarget;
                monkey.InspectedItems++;
                monkeys[target].Items.Add(newWorry);
            }

            monkey.Items.Clear();
        }
    }

    var ordered = monkeys.OrderByDescending(m => m.Value.InspectedItems);

    var ans = ordered.Select(o => o.Value.InspectedItems).Take(2).Aggregate((a, b) => a*b);

    Console.WriteLine(ans);
}

Dictionary<int, Monkey> ParseInput(string[] input)
{
    var monkeys = new Dictionary<int, Monkey>();

    Monkey current = new Monkey();

    foreach(var line in input)
    {
        if (line.StartsWith("Monkey"))
        {
            var monkey = new Monkey();
            monkeys.Add(int.Parse(line.Split(" ")[1].Split(":")[0]), monkey);

            current = monkey;
        }

        if (line.Contains("Starting items"))
        {
            var items = line.Split(":")[1].Split(", ").Select(s => long.Parse(s)).ToList();

            current.Items = items;
        }

        if (line.Contains("Operation"))
        {
            var op = line.Split("= ")[1];

            var plus = op.Split(" + ");
            var mult = op.Split(" * ");

            if (plus.Length > 1)
            {
                var num = int.Parse(plus[1]);
                current.Inspection = d => d + num; 
            }
            else
            {
                if (mult[1].Equals("old"))
                {
                    current.Inspection = d => d * d;
                }
                else
                {
                    var num = int.Parse(mult[1]);
                    current.Inspection = d => d * num;
                }
            }
        }

        if (line.Contains("Test"))
        {
            var divisibleBy = int.Parse(line.Split(" ").Last());
            current.Test = d => (d % divisibleBy) == 0;
        }

        if (line.Contains("true"))
        {
            var targ = int.Parse(line.Split(" ").Last());
            current.TrueTarget = targ;
        }

        if (line.Contains("false"))
        {
            var targ = int.Parse(line.Split(" ").Last());
            current.FalseTarget = targ;
        }
    }

    return monkeys;
}

record Monkey
{
    public List<long> Items { get; set; }
    
    public Func<long, long> Inspection { get; set; }

    public Func<long, bool> Test { get; set; }

    public int TrueTarget { get; set; }

    public int FalseTarget { get; set; }

    public long InspectedItems { get; set; }
}