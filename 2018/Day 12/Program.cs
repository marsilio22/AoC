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
            Console.ReadLine();
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

            for (int i = 1; i <= 1000; i++)
            {
                var nextGen = GetNextGeneration(generations[i-1], rules);
                generations.Add(i, nextGen);
            }

            //File.WriteAllLines(@"C:\dev\test\ConsoleApp1\ConsoleApp1\output.txt", generations.Values);

            var final = generations.Last().Value.ToCharArray();
            var offset = final.Length - initialState.Length;
            int result = 0;
            for (int i = 0; i < final.Length; i++)
            {
                if (final[i] == '#')
                {
                    result += (int)(i - ((double)offset / 2));
                }
            }
            

            Console.WriteLine(result);


            string firstStableLine = "............................................####.#.....###.#.....####.#.....###.#.....###.#.....###.#....####.#.....###.#....####.#....####.#.....###.#.....####.#...####.#....###.#.....####.#....###.#.....###.#.....####.#....####.#.............................................................................................................................................................................................................................................................................";
            long result2 = 0;

            for (int i = 0; i < firstStableLine.Length; i++)
            {
                if (firstStableLine[i] == '#')
                {
                    result2 += i + 50000000000 -134;
                }
            }

            Console.WriteLine(result2);
            
            Console.ReadLine();
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
