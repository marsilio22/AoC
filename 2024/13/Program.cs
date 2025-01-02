var lines = File.ReadAllLines("./input.txt");

long tokens = 0;
long tokensPart2 = 0;

for (int i = 0; i < lines.Length; i += 4)
{
    var buttonA = lines[i].Split(": ")[1].Split(", ").Select(s => double.Parse(s.Split("+")[1])).ToArray();
    var buttonB = lines[i + 1].Split(": ")[1].Split(", ").Select(s => double.Parse(s.Split("+")[1])).ToArray();
    var prize = lines[i + 2].Split(": ")[1].Split(", ").Select(s => double.Parse(s.Split("=")[1])).ToArray();
    var prizePart2 = lines[i + 2].Split(": ")[1].Split(", ").Select(s => double.Parse(s.Split("=")[1]) + 10000000000000d).ToArray();
    // the simultaneous equations are actually
    // P * buttonA[0] + Q * buttonB[0] = prize[0]
    // P * buttonA[1] + Q * buttonB[1] = prize[1]

    // So P = (prize[0] - Q * buttonB[0]) / buttonA[0]

    // so                 prize[1] = (prize[0] - Q * buttonB[0]) / buttonA[0] * buttonA[1] + Q * buttonB[1]
    //              prize[1] * buttonA[0] = prize[0] * buttonA[1] - Q * buttonB[0] * buttonA[1] + Q * buttonB[1] * buttonA[0]
    //              prize[1] * buttonA[0] - prize[0] * buttonA[1] = Q * (buttonB[1] * buttonA[0] - buttonB[0] * buttonA[1])

    // Q = (prize[1] * buttonA[0] - prize[0] * buttonA[1]) / (buttonB[1] * buttonA[0] - buttonB[0] * buttonA[1])
    // P = (prize[0] - Q * buttonB[0]) / buttonA[0]

    var q = (prize[1] * buttonA[0] - prize[0] * buttonA[1]) / (buttonB[1] * buttonA[0] - buttonB[0] * buttonA[1]);
    var p = (prize[0] - q * buttonB[0]) / buttonA[0];

    var qPart2 = (prizePart2[1] * buttonA[0] - prizePart2[0] * buttonA[1]) / (buttonB[1] * buttonA[0] - buttonB[0] * buttonA[1]);
    var pPart2 = (prizePart2[0] - qPart2 * buttonB[0]) / buttonA[0];

    // Console.WriteLine($"P = {p}, Q = {q}");

    if (p > 0 && q > 0 && p % 1 == 0 && q % 1 == 0)
    {
        tokens += (3 * (long)p + (long)q);
    }

    if (pPart2 > 0 && qPart2 > 0 && pPart2 % 1 == 0 && qPart2 % 1 == 0)
    {
        tokensPart2 += (3 * (long)pPart2 + (long)qPart2);
    }
}

Console.WriteLine(tokens);
Console.WriteLine(tokensPart2);