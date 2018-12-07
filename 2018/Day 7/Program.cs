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

            List<(string before, string after)> steps = new List<(string before, string after)>();
            foreach (var line in lines)
            {
                var words = line.Split(' ');
                steps.Add((words[1], words[7]));
            }

            Part1(steps);
        }

        public static void Part1(List<(string before, string after)> steps){
            var allChars = steps.SelectMany(s => new List<string>(){s.after, s.before}).Distinct().OrderBy(s => s);
            var befores = steps.Select(s => s.before).ToList();
            var afters = steps.Select(s => s.after).ToList();

            List<string> stepsThatCanNowBeCompleted = allChars.Where(s => befores.Contains(s) && !afters.Contains(s)).ToList();
            string currentCharacter = stepsThatCanNowBeCompleted.OrderBy(s => s).First();
            string order = currentCharacter;
            while (stepsThatCanNowBeCompleted.Any())
            {
                stepsThatCanNowBeCompleted.Remove(currentCharacter);
                // afters of the current letter where all of the befores for that after have been done.
                // done befores = order
                // Open afters = afters where 
                var aftersOfCurrent = steps.Where(t => t.before.Equals(currentCharacter)).Select(t => t.after).ToList();
                var beforesOfCurrentAfters = steps.Where(t => aftersOfCurrent.Contains(t.after)).ToLookup(t => t.after);

                foreach(var bef in beforesOfCurrentAfters)
                {
                    // if all the befores for the selected after are done, add to list
                    if(bef.All(b => order.Contains(b.before))){
                        stepsThatCanNowBeCompleted.Add(bef.First().after);
                    }
                }

                if (!stepsThatCanNowBeCompleted.Any())
                {
                    break;
                }

                currentCharacter = stepsThatCanNowBeCompleted.OrderBy(s => s).First();
                order += currentCharacter;
            }

            Console.WriteLine(order);
        }

        /// <summary>
        /// Need to do some Critical Path Analysis shiz here...
        /// </summary>
        public static void Part2()
        {

        }

        public class Step{
            public char Id { get; set; }

            public int TimeToComplete { get; set; }

            public List<Step> PrecedingSteps { get; set; }
        }
    }
}
