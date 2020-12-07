using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_7
{
    class Program
    {
        private static IDictionary<string, IDictionary<string, int>> rules;

        static void Main(string[] args)
        {
            var lines = File.ReadLines("./input.txt");

            rules = new Dictionary<string, IDictionary<string, int>>();

            foreach(var line in lines)
            {
                var splitLine = line.Split(" contain ");

                var bags = new Dictionary<string, int>();
                if (!splitLine[1].Equals("no other bags."))
                {
                    var stringbags = splitLine[1].Split(", ");
                    bags = stringbags.Select(b => b.Split(' ')).ToDictionary(b => string.Join(' ', new [] {b[1], b[2]}), b => int.Parse(b[0]));
                }

                rules[splitLine[0].Split(" bags")[0]] = bags;
            }

            Queue<string> queue = new Queue<string>();
            var checkedColours = new List<string>();

            queue.Enqueue("shiny gold");

            while (queue.Any())
            {
                var colour = queue.Dequeue();
                var newColoursToCheck = rules.Where(r => r.Value.ContainsKey(colour) && !checkedColours.Contains(r.Key)).Select(r => r.Key).ToList();
                
                foreach(var newColour in newColoursToCheck)
                {
                    checkedColours.Add(newColour); // There's no loops, so a counter would be good enough.
                    queue.Enqueue(newColour);
                }
            }

            // Part 1
            Console.WriteLine(checkedColours.Count);
            
            // Part 2
            Console.WriteLine(HowManyBagsUnder("shiny gold"));
        }

        public static int HowManyBagsUnder(string searchFor)
        {
            var total = 0;

            var containedBags = rules[searchFor];
            foreach(var bag in containedBags)
            {
                total += bag.Value + bag.Value * HowManyBagsUnder(bag.Key);
            }           

            return total;
        }
    }
}
