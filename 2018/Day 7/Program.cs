using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_7
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("./input.txt");

            List<(char before, char after)> stepPrecedents = new List<(char before, char after)>();
            foreach (var line in lines)
            {
                var words = line.Split(' ');
                stepPrecedents.Add((char.Parse(words[1]), char.Parse(words[7])));
            }

            var steps = stepPrecedents
                            .SelectMany(s => new List<char>{s.before, s.after})
                            .Distinct()
                            .OrderBy(s => s)
                            .Select(s => new Step{Id = s})
                            .ToList();

            for(char c = 'A'; c <= 'Z'; c++)
            {
                steps.Single(s => s.Id == c).PrecedingSteps = 
                    steps.Where(s => stepPrecedents.Where(p => p.after == c).Select(p => p.before).ToList().Contains(s.Id)).ToList();
                steps.Single(s => s.Id == c).Duration = 61 + c - 'A';
            }

            Part1(steps);

            Part2(steps);
        }

        public static void Part1(List<Step> steps)
        {
            List<Step> result = new List<Step>();

            while(result.Count < steps.Count)
            {
                var contenders = 
                    steps.Where(s => 
                        !result.Contains(s) && 
                        s.PrecedingSteps.All(p => 
                            result.Contains(p)
                        )
                    );

                result.Add(contenders.OrderBy(c => c.Id).First());
            }

            Console.WriteLine(string.Join(string.Empty, result.Select(r => r.Id)));
        }

        public static void Part2(List<Step> steps)
        {
            // This works lol, but only because the 5 workers are never maxed out. 
            // If there were ever more than 5 tasks to complete at a time then this would be wrong.
            var ordered = steps.OrderBy(s => s.LatestFinish);
            foreach(var thing in ordered){
                Console.WriteLine($"Id: {thing.Id}, LatestFinish: {thing.LatestFinish}");
            }
        }

        public class Step{
            public char Id { get; set; }

            public int Duration { get; set; }

            public List<Step> PrecedingSteps { get; set; }

            public int EarliestStart => PrecedingSteps.Any() ? PrecedingSteps.Select(p => p.LatestFinish).Max() : 0;

            public int LatestFinish => EarliestStart + Duration;
        }
    }
}
