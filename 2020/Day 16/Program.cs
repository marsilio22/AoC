using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_16
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadLines("./input.txt").ToList();
            int i = 0;
            var line = lines[i];

            var ranges = new Dictionary<string, ICollection<(int min, int max)>>();

            while (line.Split('-').Count() > 2)
            {
                var split = line.Split(": ");

                var range = split[1].Split(" or ").Select(c => 
                {
                    var rs = c.Split('-');
                    return (int.Parse(rs[0]), int.Parse(rs[1]));
                }).ToList();

                ranges.Add(split[0], range);

                i++;
                line = lines[i];
            }

            i = lines.IndexOf("nearby tickets:")+1;

            var ticketValues = new List<ICollection<int>>();
            for(; i < lines.Count; i++)
            {
                line = lines[i];
                ticketValues.Add(line.Split(',').Select(c => int.Parse(c)).ToList());
            }

            // Part 1
            
            var invalidTicketNumbers = new List<int>();

            foreach(var nums in ticketValues){
                foreach(var num in nums){
                    var shouldAdd = false;

                    shouldAdd = ranges.All(r => num < r.Value.First().min || num > r.Value.Last().max || num < r.Value.Last().min && num > r.Value.First().max);
                    
                    if (shouldAdd)
                    {
                        invalidTicketNumbers.Add(num);
                        break;
                    }
                }
            }
            
            Console.WriteLine(invalidTicketNumbers.Sum());
            
            foreach(var num in invalidTicketNumbers){
                var removed = ticketValues.RemoveAll(t => t.Contains(num));
            }

            var associations = new Dictionary<string, int>();
            var externalPossibilities = new Dictionary<string, ICollection<int>>();

            var myTicket = lines[lines.IndexOf("your ticket:") + 1].Split(',').Select(c => int.Parse(c)).ToList();
            ticketValues.Add(myTicket);

            while(associations.Count < ranges.Count)
            {
                var unresolvedRanges = ranges.Where(c => !associations.ContainsKey(c.Key));

                foreach(var range in unresolvedRanges){
                    var possibilities = new List<int>();
                    for(i = 0; i < ticketValues.First().Count(); i++)
                    {
                        if (associations.Values.Contains(i))
                        {
                            continue;
                        }
                        var column = ticketValues.Select(c => c.ToList()[i]).ToList();

                        if (column.All(c => c >= range.Value.First().min && c <= range.Value.First().max ||
                                            c >= range.Value.Last().min && c <= range.Value.Last().max))
                        {
                            possibilities.Add(i);
                        }
                    }

                    if (possibilities.Count == 1)
                    {
                        associations[range.Key] = possibilities.Single();
                    }
                    else
                    {
                        externalPossibilities[range.Key] = possibilities;
                    }
                }
            }

            var depts = associations.Where(c => c.Key.StartsWith("departure")).Select(c => c.Value);

            long ans = 1;
            foreach(var dept in depts){
                ans *= myTicket[dept];
            }

            Console.WriteLine(ans);
        }
    }
}
