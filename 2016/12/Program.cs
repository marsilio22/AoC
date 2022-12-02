var input = File.ReadAllLines("./input.txt");

(int a, int b, int c, int d) regs = (0, 0, 0, 0);

for (int i = 0; i<input.Length; i++)
{
    var line = input[i].Split(" ");

    switch(line[0])
    {
        case "cpy":
            
            break;
        case "inc":
            break;
        case "dec":
            break;
        case "jnz":
            break;
        default: throw new Exception();
    }
}