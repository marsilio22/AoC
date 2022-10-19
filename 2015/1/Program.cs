using System.Net.NetworkInformation;

var input = File.ReadLines("./input.txt").First();

var floor = 0;
var pos = 1;

foreach(var character in input)
{
    if (character == ')')
    {
        floor --;
        if (floor == -1)
            Console.WriteLine($"entered basement at position: {pos}");
    }
    else if (character == '(')
    {
        floor ++;
    }
    pos++;
}

Console.WriteLine(floor);