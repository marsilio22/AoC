using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Day_10
{
    class Program
    {
        static void Main(string[] args)
        {
            var joltages = File.ReadLines("./input.txt").Select(l => int.Parse(l)).ToList();
            // var joltages = File.ReadLines("./testInput2.txt").Select(l => int.Parse(l)).ToList();

            var builtin = joltages.Max() + 3;
            joltages.Add(builtin);

            var threes = 0; 
            var ones = 0;
            var diffs = new List<int>();

            joltages.Sort();
            joltages = joltages.Prepend(0).ToList();
            for (int i = 1; i < joltages.Count; i++){
                if(joltages[i] - joltages[i-1] == 1)
                {
                    ones++;
                    diffs.Add(1);
                }
                else if (joltages[i] - joltages[i-1] == 3){
                    threes++;
                    diffs.Add(3);
                }
            }

            Console.WriteLine(ones * threes);

            long ans = 1;

            var sb = new StringBuilder();

            foreach (var diff in diffs){
                sb.Append(diff);
            }

            var diffsString = sb.ToString();

            var diffLengths = diffsString.Split('3').Select(d => d.Count()).ToList();

            foreach(var diffLen in diffLengths){
                switch(diffLen - 1){
                    case -1:
                    case -2:
                    case 0:
                        // ans *= 1;
                        break;
                    case 1:
                        ans *= 2;
                        break;
                    case 2:
                        ans *= 4;
                        break;
                    case 3:
                        ans *= 7; // 2^3 - 1 because we need at least one
                        break;
                    case 4: 
                        ans *= 13; // 2^4 - 3, at least one, and not just either end one
                        break;
                    case 5:
                        ans *= 24; // 2^5 - 8, at least one, not just the end ones, not just the two end ones
                        break;
                    default:
                        throw new Exception();

                }
            }

            // 4406248556628825088 too high
            Console.WriteLine(ans);



            // for(int i = 0; i < joltages.Count; i++)
            // {
            //     List<int> nextChoices = joltages.Where(j => j <= joltages[i] + 3 && j > joltages[i]).ToList();
            //     if (!nextChoices.Any())
            //         break;
                
            //     List<int> nextNextChoices = joltages.Where(j => j <= nextChoices.Max() + 3 && j > nextChoices.Max()).ToList();

            //     List<(int number, bool requiredBelow, bool requiredAbove)> nextChoicesRequired2 = joltages.Select(n => (
            //         n, 
            //         joltages.Contains(n - 3) && !joltages.Contains(n-2) && !joltages.Contains(n-1),
            //         joltages.Contains(n + 3) && !joltages.Contains(n+2) && !joltages.Contains(n+1))).ToList();

            //     // n! / n-m! where m is the number of the following three (nextchoices.Max()+3) which are missing, so to speak, from the list
                
            // }

            // List<(int number, bool required)> joltagesRequired = joltages.Select(n => (
            //         n,
            //         n == 0 ||
            //         joltages.Contains(n - 3) && !joltages.Contains(n-2) && !joltages.Contains(n-1) ||
            //         joltages.Contains(n + 3) && !joltages.Contains(n+2) && !joltages.Contains(n+1))).ToList();



            //  var ans2 = Math.Pow(2, joltagesRequired.Count(n => !n.Item2));

            

            // 2541865828329 wrong
            // Console.WriteLine(ans2);
        }
    }
}
