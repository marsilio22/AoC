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
                steps.Single(s => s.Id == c).TimeToComplete = 61 + c - 'A';
            }

            Part1(steps);
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

            Console.WriteLine(string.Join('\0', result.Select(r => r.Id)));
        }

        // public static void Part1(List<(string before, string after)> steps){
        //     var allChars = steps.SelectMany(s => new List<string>(){s.after, s.before}).Distinct().OrderBy(s => s);
        //     var befores = steps.Select(s => s.before).ToList();
        //     var afters = steps.Select(s => s.after).ToList();

        //     List<string> stepsThatCanNowBeCompleted = allChars.Where(s => befores.Contains(s) && !afters.Contains(s)).ToList();
        //     string currentCharacter = stepsThatCanNowBeCompleted.OrderBy(s => s).First();
        //     string order = currentCharacter;
        //     while (stepsThatCanNowBeCompleted.Any())
        //     {
        //         stepsThatCanNowBeCompleted.Remove(currentCharacter);
        //         // afters of the current letter where all of the befores for that after have been done.
        //         // done befores = order
        //         // Open afters = afters where 
        //         var aftersOfCurrent = steps.Where(t => t.before.Equals(currentCharacter)).Select(t => t.after).ToList();
        //         var beforesOfCurrentAfters = steps.Where(t => aftersOfCurrent.Contains(t.after)).ToLookup(t => t.after);

        //         foreach(var bef in beforesOfCurrentAfters)
        //         {
        //             // if all the befores for the selected after are done, add to list
        //             if(bef.All(b => order.Contains(b.before))){
        //                 stepsThatCanNowBeCompleted.Add(bef.First().after);
        //             }
        //         }

        //         if (!stepsThatCanNowBeCompleted.Any())
        //         {
        //             break;
        //         }

        //         currentCharacter = stepsThatCanNowBeCompleted.OrderBy(s => s).First();
        //         order += currentCharacter;
        //     }

        //     Console.WriteLine(order);
        // }

        // /// <summary>
        // /// Need to do some Critical Path Analysis shiz here...
        // /// </summary>
        // public static void Part2()
        // {
            
        // }

        public class Step{
            public char Id { get; set; }

            public int TimeToComplete { get; set; }

            public List<Step> PrecedingSteps { get; set; }
        }
    }
}
