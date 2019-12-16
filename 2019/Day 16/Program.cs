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
            //var input = File.ReadAllLines("./input.txt")[0];
            var input = "02935109699940807407585447034323";

            // Part 2
            var sb = new StringBuilder();
            for (int i = 0; i < 10000; i ++){
                sb.Append(input);
            }
            var signal = sb.ToString().ToCharArray().Select(c => int.Parse(c.ToString())).ToList();

            var startingIndex = int.Parse(input[0].ToString() + input[1] + input[2] + input[3] + input[4] + input[5] + input[6]);
            // for(int i = startingIndex; i < startingIndex + 8; i ++){
            //     Console.Write(signal[i]);
            // }

            var phase = 1;
            var signalLength = signal.Count();
            var pattern = ConstructPattern(1, signalLength);

            while (phase <= 100){
                Console.WriteLine($"Starting phase {phase}");
                for (int i = 0; i < signal.Count(); i ++){
                    //Console.WriteLine($"Calculating digit {i}, phase {phase}");
                    pattern = ConstructPattern(i + 1, signalLength);
                    var patternCount = pattern.Count();
                    long runningTotal = 0;
                    for (int j = pattern.IndexOf(1); j < signal.Count();){
                        runningTotal += (signal[j] * pattern[j % patternCount]);
                        
                        // skip 0 indexes

                        var next1 = pattern.IndexOf(1, j+1);
                        var nextminus1 = pattern.IndexOf(-1, j+1);

                        if (next1 > 0 && nextminus1 > 0){
                            j = Math.Min(next1, nextminus1);
                        }
                        else if (next1 > 0)
                        {
                            j = next1;
                        }
                        else if (nextminus1 > 0){
                            j = nextminus1;
                        }
                        else {
                            j = int.MaxValue;
                        }

                    }
                    signal[i] = (int)( Math.Abs(runningTotal) % 10);
                }
                phase ++;
            }

            for(int i = startingIndex; i < startingIndex + 8; i ++){
                Console.Write(signal[i]);
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

            while(result.Count() < totalLength){
                result.AddRange(result);
            }

            result.RemoveAt(0);
            result.Add(0);
            return result;
        }
    }
}
