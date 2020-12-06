using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_13
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadLines("./input.txt").ToList();

            var part1 = CalculateSeverity(lines, 0, false);

            Console.WriteLine(part1);

            var part2 = part1;
            var delay = 0;

            while(part2 != 0)
            {
                delay++;
                part2 = CalculateSeverity(lines, delay, true);
                
            }

            Console.WriteLine(delay); 
        }

        public static int CalculateSeverity(ICollection<string> lines, int startingDelay, bool part2)
        {
            int ans = 0;
            foreach(var line in lines)
            {
                var depthRange = line.Split(":").Select(n => int.Parse(n)).ToList();

                if ((depthRange[0] + startingDelay) % ((depthRange[1] * 2) - 2)  == 0)
                {
                    ans += depthRange[0] * depthRange[1];
                    
                    // if we ever get caught in part 2, return, to save time on running lots
                    if (part2)
                    {
                        return 1;
                    }
                }
            }
            return ans;
        }
    }
}
