using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var lines = File.ReadLines("./input.txt").ToList();

            long count = checkSlope(lines, 3);
            Console.WriteLine($"Part 1: {count}");

            count *= checkSlope(lines, 1);
            count *= checkSlope(lines, 5);
            count *= checkSlope(lines, 7);
            count *= checkSlope(lines, 1, 2);

            Console.WriteLine($"Part 2: {count}");
        }

        private static int checkSlope(List<string> lines, int slopeX, int slopeY = 1 ){
            var pos = 0;
            var len = lines[0].Length;
            var count = 0;

            for (int i = 0; i < lines.Count(); i+= slopeY)
            {   
                var line = lines[i];
                if (line[pos] == '#')
                {
                    count++;
                }
                pos = (pos + slopeX) % len;
            }
            return count;
        }
    }
}
