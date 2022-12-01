var lines = File.ReadAllLines("./input.txt");

List<long> elves = new List<long>{0};

foreach(var line in lines)
{
if (line.Equals(string.Empty))
{
    elves.Add(0);
}
else
{
    elves[elves.Count()-1] += int.Parse(line);
}

}


elves = elves.OrderByDescending(c => c).ToList();

Console.WriteLine(elves[0]);
Console.WriteLine(elves[0]+ elves[1]+ elves[2]);