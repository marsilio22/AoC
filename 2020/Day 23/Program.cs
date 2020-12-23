using System;
using System.Collections.Generic;
using System.Linq;

namespace Day_23
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = "135468729".Select(c => int.Parse(c.ToString())).ToList();
            //input = "389125467".Select(c => int.Parse(c.ToString())).ToList();

            LinkedList<int> cups = new LinkedList<int>();

            foreach(var c in input)
            {
                cups.AddLast(c);
            }

            var currentcup = cups.Find(cups.First()); 

            // part 1
            // for (int i = 0; i < 100; i ++)
            // {
            //     var cup = cups.Find(currentcup.Value);

            //     var moving = new List<int>();
            //     cup = cup.Next;
            //     for (int k = 0; k < 3; k ++)
            //     {
            //         moving.Add(cup.Value);
            //         cup = cup.Next;
            //         if (cup == null)
            //         {
            //             cup = cups.Find(cups.First());
            //             cups.RemoveLast();
            //         }
            //         else{
            //             cups.Remove(cup.Previous);
            //         }
            //     }

            //     var dest = cups.Find(currentcup.Value - 1);
            //     var j = 2;
            //     while (dest == null)
            //     {
            //         if (currentcup.Value - j < 1)
            //         {
            //             j = currentcup.Value - cups.Max();
            //         }
            //         dest = cups.Find(currentcup.Value - j);
            //         j++;
            //     }

            //     for (j = 0; j < 3; j++)
            //     {
            //         cups.AddAfter(dest, moving[j]);
            //         dest = dest.Next;
            //     }

            //     currentcup = currentcup.Next;
            //     if (currentcup == null)
            //     {
            //         currentcup = cups.Find(cups.First());
            //     }
            // }

            // currentcup = cups.Find(1).Next;

            // for (int i = 0; i < cups.Count - 1; i ++)
            // {
            //     if (currentcup == null)
            //     {
            //         currentcup = cups.Find(cups.First());
            //     }
            //     Console.Write(currentcup.Value);
            //     currentcup = currentcup.Next;
            // }
            Console.WriteLine();
            Console.WriteLine();

            // each key contains the next value
            var cups2 = new Dictionary<int, int>();
            for (int i = 0; i < input.Count() - 1; i++)
            {
                cups2.Add(input[i], input[i+1]);
            }

            // cups2.Add(input.Last(), cups2.First().Key);
            cups2.Add(input.Last(), input.Max() + 1);

            for(int i = cups.Max() + 1; i < 1_000_000; i++)
            {
                cups2.Add(i, i+1);
            }

            cups2.Add(1_000_000, cups2.First().Key);

            var currentCup = cups2.First(); 

            for (int i = 0; i < 10_000_000; i ++ )
            {
                var nextCup = new KeyValuePair<int, int>(currentCup.Value, cups2[currentCup.Value]);

                // the first cup we move is the one immediately after the current one
                // i.e. the value of the current cup
                var movingStart = nextCup.Key;

                // the last cup we move is the third one after that
                var movingEnd = cups2[nextCup.Value];

                // remove them by setting the current cup's value, to the value of
                // the cup after the end of the section we're moving
                cups2[currentCup.Key] = cups2[movingEnd];

                // find where we're moving them to;
                var destVal = currentCup.Key - 1;
                if (destVal < 1)
                {
                    destVal = cups2.Keys.Max();
                }

                while (destVal == movingStart ||
                    destVal == cups2[movingStart] ||
                    destVal == movingEnd)
                {       
                    destVal--;
                    if (destVal < 1)
                    {
                        destVal = cups2.Keys.Max();
                    }
                }

                var afterDest = cups2[destVal];
                cups2[destVal] = movingStart;
                cups2[movingEnd] = afterDest;

                currentCup = new KeyValuePair<int, int>(cups2[currentCup.Key], cups2[cups2[currentCup.Key]]);

                // if (i % 10000 == 0)
                // {
                //     Console.WriteLine($"{DateTime.Now}: {i/10000}% complete");
                // }
            }

            var starCup = cups.Find(1).Next;

            long ans = (long)starCup.Next.Value * (long)starCup.Next.Next.Value;

            Console.WriteLine(ans);
        }
    }
}
