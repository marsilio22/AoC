using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Day6
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> lines = File.ReadAllLines("./input.txt").ToList();

            var messageP1 = "";
            var messageP2 = "";

            for (int i = 0; i < 8; i++)
            {
                var chars = new Dictionary<char, int>();
                foreach(var line in lines)
                {
                    var c = line[i];

                    chars[c] = chars.ContainsKey(c) ? chars[c]+1 : 1;
                }

                var orderedValues = chars.OrderByDescending(c => c.Value).Select(c => c.Key).ToList();

                messageP1 += orderedValues.First();
                messageP2 += orderedValues.Last();
            }

            Console.WriteLine(messageP1);
            Console.WriteLine(messageP2);
        }
    }
}
