using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_10
{
    class Program
    {
        static void Main(string[] args)
        {
            var line = File.ReadLines("./input.txt").ToList()[0];
            // part 1
            //var lengths = line.Split(',').Select(s => int.Parse(s)).ToList();

            // part 2
            var lengths = line.ToCharArray().Select(c => (int)c).ToList();
            lengths.AddRange(new int[] {17, 31, 73, 47, 23});

            var list = new LinkedList<int> (Enumerable.Range(0, 256).ToList());

            var skip = 0;
            var pos = list.First;
            var pos2 = list.First;

            for(int i = 0; i < 64; i++){

                foreach(var length in lengths)
                {
                    var listToReverse = new List<int>();

                    for (int j = 0; j < length; j++){
                        listToReverse.Add(pos.Value);
                        pos = pos.Next;
                        if (pos == null)
                        {
                            pos = list.First;
                        }
                    }

                    listToReverse.Reverse();

                    foreach(var reversedValue in listToReverse)
                    {
                        pos2.Value = reversedValue;
                        pos2 = pos2.Next;
                        if (pos2 == null)
                        {
                            pos2 = list.First;
                        }
                    }

                    for(int j = 0; j < skip; j++){
                        pos = pos.Next;
                        if (pos == null)
                        {
                            pos = list.First;
                        }

                        pos2 = pos2.Next;
                        if (pos2 == null)
                        {
                            pos2 = list.First;
                        }
                    }

                    skip ++;
                }
            }

            // part 1
            // var thing2 = list.ToList();
            // Console.WriteLine(thing2[0] * thing2[1]);

            var denseHash = new List<int>();
            var totalpos = list.First;
            for (int i = 0; i < 16; i++)
            {
                denseHash.Add(totalpos.Value);
                totalpos = totalpos.Next;
                for (int j = 0; j < 15; j++)
                {
                    denseHash[denseHash.Count - 1] ^= totalpos.Value;
                    totalpos = totalpos.Next;
                }
            }

            foreach(var val in denseHash)
            {
                Console.Write(val.ToString("X").ToLowerInvariant());
            }

            Console.WriteLine();
        }
    }
}
