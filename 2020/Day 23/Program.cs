using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Day_23
{
    class Program
    {
        static void Main(string[] args)
        {
            List<int> input = "135468729".Select(c => int.Parse(c.ToString())).ToList();
            
            // uncomment to do example instead
            //input = "389125467".Select(c => int.Parse(c.ToString())).ToList();

            IDictionary<int, int> part1 = ShuffleCups(input, 100, input.Max());
            string ans1 = string.Empty;

            int num = part1[1];
            Console.Write("Part 1: ");
            while(num != 1)
            {
                Console.Write(num);
                num = part1[num];
            }
            Console.WriteLine();

            IDictionary<int, int> part2 = ShuffleCups(input, 10_000_000, 1_000_000);
            long ans2 = (long)part2[1] * (long)part2[part2[1]];

            Console.WriteLine($"Part 2: {ans2}");
        }

        public static IDictionary<int, int> ShuffleCups(List<int> input, int turns, int biggestCup)
        {
            // each key contains the next value
            Dictionary<int, int> cups = new Dictionary<int, int>();
            for (int i = 0; i < input.Count() - 1; i++)
            {
                cups.Add(input[i], input[i+1]);
            }

            for(int i = cups.Keys.Max() + 1; i < biggestCup; i++)
            {
                cups.Add(i, i+1);
            }

            cups.Add(biggestCup, cups.First().Key);

            KeyValuePair<int, int> currentCup = cups.First();
            Stopwatch sw = new Stopwatch();

            sw.Start();
            for (int i = 0; i < turns; i ++ )
            {
                KeyValuePair<int, int> nextCup = new KeyValuePair<int, int>(currentCup.Value, cups[currentCup.Value]);

                // the first cup we move is the one immediately after the current one
                // i.e. the value of the current cup
                int movingStart = nextCup.Key;

                // the last cup we move is the one after the one after that
                int movingEnd = cups[nextCup.Value];

                // "remove" them by setting the current cup's value to the value of
                // the cup after the end of the section we're moving (so that the cup after
                // the current cup is now the cup after the last moving cup)
                cups[currentCup.Key] = cups[movingEnd];

                // find where we're moving them to;
                int destVal = currentCup.Key - 1;
                if (destVal < 1)
                {
                    destVal = cups.Keys.Max();
                }

                while (destVal == movingStart ||
                    destVal == cups[movingStart] ||
                    destVal == movingEnd)
                {       
                    destVal--;
                    if (destVal < 1)
                    {
                        destVal = cups.Keys.Max();
                    }
                }

                int afterDest = cups[destVal];
                cups[destVal] = movingStart;
                cups[movingEnd] = afterDest;

                currentCup = new KeyValuePair<int, int>(cups[currentCup.Key], cups[cups[currentCup.Key]]);
            }
            sw.Stop();

            Console.WriteLine($"Shuffling {biggestCup} cups for {turns} turns took {sw.Elapsed}");

            return cups;
        }
    }
}
