using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_11
{
    class Program
    {
        static void Main(string[] args)
        {
            var line = File.ReadLines("./input.txt").ToList()[0];

            var x = 0;
            var y = 0;
            var biggestAbs = 0;

            var directions = line.Split(',');

            // n = y+1
            // s = y-1
            // ne = x+1
            // sw = x-1
            // nw = y+1 && x-1
            // se = y-1 && x+1

            foreach (var dir in directions)
            {
                switch(dir)
                {
                    case "n": y+=1; break;
                    case "s": y-=1; break;
                    case "ne": x+=1; break;
                    case "sw": x-=1; break;
                    case "nw": y+=1; x-=1; break;
                    case "se": y-=1; x+=1; break;
                }

                var abs = new List<int>{Math.Abs(x), Math.Abs(y)}.Max();
                if (abs > biggestAbs){
                    biggestAbs = abs;
                }
            }

            Console.WriteLine($"x{x}, y{y}");
            Console.WriteLine(biggestAbs);
        }
    }
}
