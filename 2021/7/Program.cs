using System;
using System.Linq;

var line = File.ReadAllLines("./input.txt")[0];

var positions = line.Split(",").Select(l => int.Parse(l)).ToList();

var avg = positions.Average();

var intavg = Math.Round(avg);


var fuel = 0;
foreach(var crab in positions)
{
    fuel += (int)Math.Abs(crab - intavg);
}

Console.WriteLine(fuel); // 355875 too high


// ok brute force since I'm not clever, cba looking up median

var smallest = int.MaxValue;

for (int i = 0; i < positions.Max(); i++)
{
    var fuelUsed = positions.Sum(p => (int)Math.Abs(i - p));
    if (fuelUsed < smallest)
    {
        Console.WriteLine($"new smallest fuel at position {i}");
        smallest = fuelUsed;
    }
}

Console.WriteLine(smallest); // 335271 lol i wasn't even far off


// part 2
smallest = int.MaxValue;
for (int i = 0; i < positions.Max(); i++)
{
    // sum of numbers 1 -> n is n(n-1)/2, but here we include endpoints so n = n+1 = Abs(i-p) + 1 which implies
    // (|i-p|)(|i-p|+1)/2
    var fuelUsed = positions.Sum(p => (((int)Math.Abs(i - p)) * ((int)Math.Abs(i - p) + 1)) / 2);

    if (fuelUsed < smallest)
    {
        Console.WriteLine($"new smallest fuel at position {i}");
        smallest = fuelUsed;
    }
}

Console.WriteLine(smallest); // 95495476 too low