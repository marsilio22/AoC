using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_6
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines("./input.txt");
            var input = lines[0].Split('\t').Select(s => int.Parse(s)).ToArray();
            var previousConfigurations = new List<int[]>{};

            var currentConfiguration = input;
            while(previousConfigurations.FirstOrDefault(conf => ArrayEquals(conf, currentConfiguration)) == null)
            {
                int[] newArray = new int[currentConfiguration.Length];
                currentConfiguration.CopyTo(newArray, 0);
                previousConfigurations.Add(newArray);

                var blocks = currentConfiguration.Max();
                var index = Array.IndexOf(currentConfiguration, blocks); // This gets the first occurrence, which is what we want

                // Redistribute
                currentConfiguration[index] = 0;
                
                while(blocks != 0)
                {
                    index = (index + 1) % currentConfiguration.Length; // This will make the index wrap around
                    currentConfiguration[index]++;
                    blocks--;
                }
            }

            Console.WriteLine($"Cycles to repeat: {previousConfigurations.Count()}");

            var firstIndex = previousConfigurations.FindIndex(c => ArrayEquals(c, currentConfiguration));
            var loopSize = previousConfigurations.Count() - firstIndex;
            Console.WriteLine($"Loop Size: {loopSize}");
        }

        public static bool ArrayEquals(int[] first, int[] second){
            for (int i = 0; i < first.Length; i++){
                if(first[i] != second[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
