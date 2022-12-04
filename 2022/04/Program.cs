var lines = File.ReadAllLines("./input.txt");

var totallyContained = 0;
var partiallyContained = 0;

foreach(var line in lines)
{
    var split = line.Split(",");

    var elf1 = split[0].Split("-").Select(e => int.Parse(e));
    var elf2 = split[1].Split("-").Select(e => int.Parse(e));

    var elf1Min = elf1.First();
    var elf1Max = elf1.Last();

    var elf2Min = elf2.First();
    var elf2Max = elf2.Last();

    if (elf1Min >= elf2Min && elf1Max <= elf2Max ||
        elf2Min >= elf1Min && elf2Max <= elf1Max)
    {
        totallyContained++;
    }

    if (elf1Min >= elf2Min && elf1Min <= elf2Max ||
        elf1Max <= elf2Max && elf1Max >= elf2Min ||

        elf2Min >= elf1Min && elf2Min <= elf1Max ||
        elf2Max <= elf1Max && elf2Max >= elf1Min)
        {
            partiallyContained++;
        }
}

Console.WriteLine(totallyContained);
Console.WriteLine(partiallyContained);