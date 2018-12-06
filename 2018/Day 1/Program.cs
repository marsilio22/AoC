using System;
using System.Collections.Generic;

namespace Day_1
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = System.IO.File.ReadAllLines(@"C:\dev\AoC\2018\Day 1\input.txt");
            List<int> previousVals = new List<int>();

            int total = 0;
            while(true)
            {
                foreach(var line in lines)
                {
                    (string sign, int number) input = (line.Substring(0,1), int.Parse(line.Substring(1)));

                    switch (input.sign)
                    {
                        case "+":
                            total += input.number;
                            break;
                        case "-":
                            total -= input.number;
                            break;
                        default:
                            throw new InvalidOperationException("oops");
                    }

                    if(previousVals.Contains(total))
                    {
                        Console.WriteLine(total);
                        Console.WriteLine($"{previousVals.Count} / {lines.Length} = {(double)previousVals.Count/(double)lines.Length}");
                        return;
                    }
                    else
                    {
                        previousVals.Add(total);
                    }
                }
            }
        }
    }
}
