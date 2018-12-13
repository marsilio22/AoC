using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("./input.txt");

            string fourHundredDots = "................................................................................................................................................................................................................................................................................................................................................................................................................";

            var initialState = fourHundredDots + lines[0].Split(':')[1].Trim() + fourHundredDots;

            Dictionary<string, char> rules = new Dictionary<string, char>();

            foreach(var line in lines){
                if (line.Contains("=>")){
                    var splitLine = line.Split("=>");
                    rules.Add(splitLine[0].Trim(), char.Parse(splitLine[1].Trim()));
                }
            }

            Dictionary<int, string> generations = new Dictionary<int, string>{{0, initialState}};
            int count = 1;
            while (true)
            {
                var nextGen = GetNextGeneration(generations[count - 1], rules);
                generations.Add(count, nextGen);

                var firstHash = nextGen.IndexOf('#');
                var lastHash = nextGen.LastIndexOf('#');

                if (generations[count - 1].Contains(generations[count].Substring(firstHash, lastHash - firstHash)))
                {
                    break;
                }
                count++;
            }

            // File.WriteAllLines("./output.txt", generations.Values);

            string firstStableLine = generations[count - 1].Substring(400); // remove the 400 dots from the front that we don't care about
            long finalForecastGeneration = 50000000000 - (count - 1);
            long result2 = 0;

            for (int i = 0; i < firstStableLine.Length; i++)
            {
                if (firstStableLine[i] == '#')
                {
                    result2 += i + finalForecastGeneration;
                }
            }

            Console.WriteLine(result2);
        }

        public static string GetNextGeneration(string currentGeneration, Dictionary<string, char> rules)
        {
            string result = string.Empty;
            for (int i = 0; i < currentGeneration.Length; i++)
            {
                if (i - 2 < 0 || i + 2 >= currentGeneration.Length)
                {
                    result += '.';
                    continue;
                }

                var important5 =
                    currentGeneration[i - 2].ToString() +
                    currentGeneration[i - 1] +
                    currentGeneration[i] +
                    currentGeneration[i + 1] +
                    currentGeneration[i + 2];

                result += rules[important5];
            }

            return result;
        }
    }
}
