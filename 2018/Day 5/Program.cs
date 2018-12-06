using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day_5
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            var line = File.ReadAllLines("./input.txt")[0];
            var result = new Dictionary<char, int>();
            var chars = new [] {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'};

            foreach(var character in chars)
            {
                var list = line.ToCharArray().Where(c => !string.Equals(c.ToString(), character.ToString(), StringComparison.OrdinalIgnoreCase)).Select(c => c.ToString()).ToList();

                list = React(list);
                result.Add(character, list.Count());
            }

            result = result.OrderBy(r => r.Value).ToDictionary(r => r.Key, r => r.Value);;

            foreach(var thing in result){
                Console.WriteLine($"Character {thing.Key} = {thing.Value}");
            }

            timer.Stop();
            Console.WriteLine(timer.ElapsedMilliseconds);
        }

        public static List<string> React(List<string> characters){
            while (true)
            {
                bool changedThisLoop = false;
                for(int i = 0; i < characters.Count(); i++) 
                {
                    if(i == characters.Count() - 1){
                        break;
                    }

                    if (string.Equals(characters[i], characters[i+1], StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(characters[i], characters[i+1], StringComparison.Ordinal))
                    {
                        characters.RemoveAt(i+1);
                        characters.RemoveAt(i);
                        changedThisLoop = true;
                    }
                }

                if (!changedThisLoop){
                    return characters;
                }
            }
        }
    }
}
