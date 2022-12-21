var lines = File.ReadAllLines("./input");

IDictionary<string, long> monkeys = new Dictionary<string, long>();

while(!monkeys.ContainsKey("root"))
{
    foreach (var line in lines)
    {
        var split = line.Split(": ");

        if (monkeys.ContainsKey(split[0]))
        {
            continue;
        }
        else if (long.TryParse(split[1], out var val))
        {
            monkeys[split[0]] = val;
        }
        else
        {
            var subsplit = split[1].Split(" ");

            var m1 = subsplit[0];
            var m2 = subsplit[2];

            if (monkeys.TryGetValue(m1, out var m1Val) && monkeys.TryGetValue(m2, out var m2Val))
            {
                monkeys[split[0]] = subsplit[1] switch 
                {
                    "+" => m1Val + m2Val,
                    "-" => m1Val - m2Val,
                    "*" => m1Val * m2Val,
                    "/" => m1Val / m2Val
                };
            }
        }

    }
}
Console.WriteLine(monkeys["root"]);


// p2
monkeys = new Dictionary<string, long>();
var monkeyCount= 1;

while(monkeyCount != monkeys.Count)
{
    monkeyCount = monkeys.Count;

    foreach (var line in lines)
    {
        var split = line.Split(": ");

        if (split[0] == "humn")
        {
            continue;
        }

        if (monkeys.ContainsKey(split[0]))
        {
            continue;
        }
        else if (long.TryParse(split[1], out var val))
        {
            monkeys[split[0]] = val;
        }
        else
        {
            var subsplit = split[1].Split(" ");

            var m1 = subsplit[0];
            var m2 = subsplit[2];

            if (monkeys.TryGetValue(m1, out var m1Val) && monkeys.TryGetValue(m2, out var m2Val))
            {
                monkeys[split[0]] = subsplit[1] switch 
                {
                    "+" => m1Val + m2Val,
                    "-" => m1Val - m2Val,
                    "*" => m1Val * m2Val,
                    "/" => m1Val / m2Val
                };
            }
        }

    }
}

long target = 0;

var rootLine = lines.First(b => b.StartsWith("root"));

var spl = rootLine.Split(": ");
var leftName = spl[1].Split(" ")[0];
var rightName = spl[1].Split(" ")[2];

var left = monkeys.TryGetValue(leftName, out var leftVal);
var right= monkeys.TryGetValue(rightName, out var rightVal);

if (left)
{
    target = leftVal;
}
else if (right)
{
    target = rightVal;
}

var targ = left ? rightName : leftName;

while(targ != "humn")
{
    var targLine = lines.First(b => b.StartsWith(targ));

    spl = targLine.Split(": ");
    leftName = spl[1].Split(" ")[0];
    rightName = spl[1].Split(" ")[2];

    left = monkeys.TryGetValue(leftName, out leftVal);
    right= monkeys.TryGetValue(rightName, out rightVal);

    targ = left ? rightName : leftName;

    target = spl[1].Split(" ")[1] switch
    {
        "+" => target - (left ? leftVal : rightVal),
        "-" => left ? leftVal - target : target + rightVal,
        "*" => target / (left ? leftVal : rightVal),
        "/" => left ? (leftVal / target) : rightVal * target
    };
}

Console.WriteLine(target);