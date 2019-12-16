using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Day_16
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("./input.txt")[0];
            //var input = "03081770884921959731165446850517";
            var signal = input.ToCharArray().Select(c => int.Parse(c.ToString())).ToList();

            var phase = 1;
            var signalLength = signal.Count();
            var pattern = ConstructPattern(1, signalLength);

            while (phase <= 100){
                for (int i = 0; i < signal.Count(); i ++){
                    pattern = ConstructPattern(i + 1, signalLength);
                    var patternCount = pattern.Count();
                    long runningTotal = 0;
                    for (int j = pattern.IndexOf(1); j < signal.Count(); j++){
                        runningTotal += (signal[j] * pattern[j % patternCount]);
                    }
                    signal[i] = (int)( Math.Abs(runningTotal) % 10);
                }
                phase ++;
            }

            for(int i = 0; i < 8; i ++){
                Console.Write(signal[i]);
            }
            Console.WriteLine();

            // Part 2
            var sb = new StringBuilder();
            for (int i = 0; i < 10000; i ++){
                sb.Append(input);
            }
            signal = sb.ToString().ToCharArray().Select(c => int.Parse(c.ToString())).ToList();

            var startingIndex = int.Parse(input[0].ToString() + input[1] + input[2] + input[3] + input[4] + input[5] + input[6]);

            signal.Reverse();
            var offset = signal.Count - startingIndex;

            var nextSignal = new List<int>(signal.GetRange(0, offset));
            for (int i = 0; i < 100; i++){
                // Console.WriteLine($"phase {i}");
                nextSignal[0] = signal[0];
                for (int j = 1; j < offset; j++){
                    nextSignal[j] = (signal[j] + nextSignal[j-1]) % 10;
                }
                signal = nextSignal;
            }

            for (int i = 1; i < 9; i++){
                Console.Write(signal[signal.Count - i]);
            }
            Console.WriteLine();
        }

        public static List<int> ConstructPattern(int stage, int totalLength){
            List<int> result = new List<int>();
            var pattern = new [] {0, 1, 0, -1};

            for (int i = 0; i < 4; i ++){
                var thing = pattern[i];
                result.AddRange((from x in Enumerable.Range(1, stage) select thing).ToList()); // off by one?
            }

            result.RemoveAt(0);
            result.Add(0);
            return result;
        }
    }
}
