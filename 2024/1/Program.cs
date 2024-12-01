var lines = File.ReadAllLines("./input.txt");

var lefts = new List<int>();
var rights = new List<int>();

foreach (var line in lines)
{
    var split = line.Split(' ');

    lefts.Add(int.Parse(split.First()));
    rights.Add(int.Parse(split.Last()));
}

lefts.Sort();
rights.Sort();

var tot = 0;

for(int i = 0; i < lefts.Count; i++)
{
    tot += Math.Abs(lefts[i] - rights[i]);
}

Console.WriteLine(tot);

var sim = 0;

foreach(var left in lefts)
{
    sim += left * rights.Count(r => r == left);
}

Console.WriteLine(sim);