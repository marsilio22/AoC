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
            var lines = File.ReadAllLines("./input.txt");

            var initialState = lines[0].Split(':')[1].Trim();

            Dictionary<string, char> rules = new Dictionary<string, char>();

            foreach(var line in lines){
                if (line.Contains("=>")){
                    var splitLine = line.Split("=>");
                    rules.Add(splitLine[0].Trim(), char.Parse(splitLine[1].Trim()));
                }
            }

            Dictionary<int, string> generations = new Dictionary<int, string>{{0, initialState}};

            for (int i = 1; i <= 20; i++)
            {
                var nextGen = GetNextGeneration(generations[i-1], rules);
                generations.Add(i, nextGen);
                Console.WriteLine(nextGen);
            }

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
        }

        public static string GetNextGeneration(string currentGeneration, Dictionary<string, char> rules){
            currentGeneration = "....." + currentGeneration + ".....";
            var array = currentGeneration.ToCharArray();

            string result = string.Empty;
            for (int i = 2; i < currentGeneration.Length - 2; i++)
            {
                var important5 = 
                    currentGeneration[i-2].ToString() +
                    currentGeneration[i-1].ToString() +
                    currentGeneration[i].ToString() +
                    currentGeneration[i+1].ToString() +
                    currentGeneration[i+2].ToString();

                result += rules[important5];
            }
            return result;
        }
    }
}
