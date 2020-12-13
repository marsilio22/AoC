using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_13
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadLines("./input.txt").ToList();

            var earliestTime = int.Parse(lines[0]);
            var times = lines[1].Split(',').Where(c => !c.Equals("x")).Select(c => int.Parse(c)).ToList();

            times.Sort();
            var max = times.Max();

            for (int i = 0; i < max; i++)
            {
                var thing = times.Select(t => (t, (earliestTime + i) % t)).ToList();
                if (thing.Any(c => c.Item2 == 0))
                {
                    var thing2 = thing.Single(c => c.Item2 == 0);

                    Console.WriteLine(i * thing2.Item1);
                    break;
                }
            }

            var newTimes = lines[1].Split(',').Select(c => int.TryParse(c, out int bus) ? bus : 1).ToList();

            List<(int time, int index)> timesAndIndices = newTimes.Where(n => n > 1).Select(n => (n, n - newTimes.IndexOf(n))).ToList();

            while(timesAndIndices.Any(t => t.index < 0))
            {
                timesAndIndices = timesAndIndices.Select(c => (c.time, (c.index + c.time) % c.time)).ToList();
            }

            // t + index = 0 modulo number, find t
            // nb from observation, all bus numbers are prime...

            // adapted from rosettacode because good god I don't remember how to do the CRT
            // did have to make it work with such big numbers though :D
            long result = ChineseRemainderTheorem.Solve(timesAndIndices);

            for(int i = 0; i < timesAndIndices.Count; i++)
            {
                Console.WriteLine($"{result} ≡ {timesAndIndices[i].index} (mod {timesAndIndices[i].time})");
            }
        }
    }
 
    public static class ChineseRemainderTheorem
    {
        public static long Solve(List<(int n, int a)> congruences)
        {
            long prod = 1;

            foreach(var mod in congruences)
            {
                prod *= (long)mod.n;
            }

            long sum = 0;
            for (int i = 0; i < congruences.Count; i++)
            {
                // the CRT says that if t = a mod N and t = b mod M, then we can find j, k such that t = aj + bk mod NM,
                // here we solve this each time to find the j's, where the "second" congruence is the one for the product of all
                // the other moduli
                long p = prod / congruences[i].n;
                sum += congruences[i].a * ModularMultiplicativeInverse(p, congruences[i].n) * p;
            }

            // sum is probably not > prod, but hey
            return sum % prod;
        }
 
        private static long ModularMultiplicativeInverse(long a, int mod)
        {
            // we're only dealing with small numbers, so just iterate here instead of euclid
            long b = a % mod;
            for (int x = 1; x < mod; x++)
            {
                if ((b * x) % mod == 1)
                {
                    return x;
                }
            }
            return 1;
        }
    }
}