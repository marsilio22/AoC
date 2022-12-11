var lines = File.ReadAllLines("./input");

var registers = new Dictionary<char, long>{
    ['a'] = 0,
    ['b'] = 0
};

for (int i = 0; i < lines.Length; i++)
{
    var split = lines[i].Split(" ");

    switch(split[0])
    {
        case "hlf":
            registers[split[1][0]] = registers[split[1][0]] / 2;
            break;
        case "tpl":
            registers[split[1][0]] = registers[split[1][0]] * 3;
            break;
        case "inc":
            registers[split[1][0]] = registers[split[1][0]] + 1;
            break;
        case "jmp":
            i += int.Parse(split[1]) - 1; // off by one probs
            break;
        case "jie":
            if (registers[split[1][0]] % 2 == 0)
            {
                i += int.Parse(split[2]) - 1;
            }
            break;
        case "jio":
            if (registers[split[1][0]] == 1)
            {
                i+= int.Parse(split[2]) - 1;
            }
            break;
    }

    Console.WriteLine($"{i}, {lines[i]}");
    Console.WriteLine($"a: {registers['a']}, b: {registers['b']}");
}

Console.WriteLine(registers['b']);



// p2

long a = 1;
long b = 0;

a *= 3; // 17
a++;//18
a++;//19
a*=3;//20
a++;//21
a++;//22
a*=3;//23
a*=3;//24
a++;//25
a++;//26
a*=3; //27
a++; //28
a*=3; //29
a++; //30
a*=3; //31
a++; // 32
a++; //33
a*=3; //34
a++; //35
a*=3; //36
a*=3; //37
a++; //38

while (a != 1) // 39
{
    b++; // 40

    if (a % 2 == 0)
    {
        a /= 2;
    }
    else 
    {
        a *=3;
        a++;
    }
}

Console.WriteLine(b);