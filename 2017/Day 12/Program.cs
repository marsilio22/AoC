using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_12
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadLines("./input.txt");

            IDictionary<int, ICollection<int>> pipes = new Dictionary<int, ICollection<int>>();
            foreach(var line in lines)
            {
                var splitLine = line.Split("<->");

                pipes[int.Parse(splitLine[0])] = splitLine[1].Split(", ").Select(s => int.Parse(s)).ToList();
            }

            var groups = 0;

            while(pipes.Any()){

                List<int> pipesToCheck = pipes.First().Value.ToList();
                var pipesUnderFirst = new List<int>();

                while (pipesToCheck.Any())
                {
                    var currentPipe = pipesToCheck[0];
                    pipesToCheck.Remove(currentPipe);
                    pipesUnderFirst.Add(currentPipe);

                    pipesToCheck.AddRange(pipes[currentPipe].ToList());

                    pipesToCheck.Distinct();

                    pipesToCheck.RemoveAll(p => pipesUnderFirst.Contains(p));
                }
                Console.WriteLine(pipesUnderFirst.Count());
                groups ++;
                foreach(var thing in pipesUnderFirst){
                    pipes.Remove(thing);
                }
            }

            Console.WriteLine(groups);
        }
    }
}
