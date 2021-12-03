var lines = File.ReadAllLines("./input.txt");

ICollection<(string dir, int value)> parsed = lines.Select(l => l.Split(" ")).Select(l => (l[0], int.Parse(l[1]))).ToList();

var grouped = parsed.GroupBy(p => p.dir);

IDictionary<string, int> values = new Dictionary<string, int>();

foreach(var group in grouped) 
{
    values[group.Key] = group.Sum(g => g.value);
}

var answer = values["forward"] * (values["down"]- values["up"]);

Console.WriteLine(answer);

var aim = 0;
var horizontal = 0;
var depth = 0;
foreach(var line in parsed)
{
    switch (line.dir) {
        case "forward":
            horizontal += line.value;
            depth += line.value * aim;
            break;
        case "up":
            aim -= line.value;
            break;
        case "down":
            aim += line.value;
            break;
        default:
            throw new Exception();
    }
}

answer = horizontal * depth;

Console.WriteLine(answer);