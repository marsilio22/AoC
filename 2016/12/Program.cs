
using System.Security.Authentication;

var regs = new Dictionary<string, int>{
    ["a"] = 0,
    ["b"] = 0,
    ["c"] = 0,
    ["d"] = 0
};
Calculate(regs);
Console.WriteLine(regs["a"]);

regs = new Dictionary<string, int>{
    ["a"] = 0,
    ["b"] = 0,
    ["c"] = 1,
    ["d"] = 0
};
Calculate(regs);
Console.WriteLine(regs["a"]);

void Calculate(Dictionary<string, int> regs)
{
    var input = File.ReadAllLines("./input.txt");

    for (int i = 0; i < input.Length; i++)
    {
        var line = input[i].Split(" ");

        switch(line[0])
        {
            case "cpy":
                if (int.TryParse(line[1], out int blah))
                {
                    regs[line[2]] = blah;
                }
                else
                {
                    regs[line[2]] = regs[line[1]];
                }
                break;
            case "inc":
                regs[line[1]] = regs[line[1]] + 1;
                break;
            case "dec":
                regs[line[1]] = regs[line[1]] - 1;
                break;
            case "jnz":
                if (int.TryParse(line[1], out blah) && blah > 0 || regs[line[1]] > 0)
                {
                    i += int.Parse(line[2]) - 1;
                }
                break;
            default: throw new Exception();
        }
    }
}