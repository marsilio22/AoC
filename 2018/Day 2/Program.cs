using System;
using System.IO;
using System.Linq;

namespace Day_2
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(@"C:\dev\AoC\2018\Day 2\input.txt");

            Part1(lines);
            Part2(lines);
        }

        public static void Part1(string[] lines){
            int twos = 0;
            int threes = 0;

            foreach (var line in lines){
                var arrayLine = line.ToCharArray();

                twos += arrayLine.Any(character => arrayLine.Count(c => c == character) == 2) ? 1 : 0;
                threes += arrayLine.Any(character => arrayLine.Count(c => c == character) == 3) ? 1 : 0;
            }

            Console.WriteLine($"Twos:   {twos}");
            Console.WriteLine($"Threes: {threes}");
            Console.WriteLine("-------");
            Console.WriteLine($"Total:  {twos * threes}");
        }

        public static void Part2(string[] lines)
        {
            foreach(var line in lines){
                var bingo = lines.FirstOrDefault(l => StrDiffComplement(l, line).Length == line.Length - 1);

                if (bingo != null){
                    Console.WriteLine($"Bingo: {StrDiffComplement(bingo, line)}");
                    return;
                }
            }
        }

        public static string StrDiffComplement(string firstString, string secondString){
            var firstChars = firstString.ToCharArray();
            var secondChars = secondString.ToCharArray();
            string output = string.Empty;

            for (int i = 0; i < firstChars.Length; i++){
                if (firstChars[i] == secondChars[i])
                {
                    output += firstChars[i];
                }
            }

            return output;
        }
    }
}
