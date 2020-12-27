using System;
using System.IO;
using System.Linq;

namespace Day_25
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadLines("./input.txt").ToList();

            long doorPub = int.Parse(lines[0]);
            long cardPub = int.Parse(lines[1]);

            // example
            // doorPub = 17807724;
            // cardPub = 5764801;

            long doorLoopSize = 0;
            long cardLoopSize = 0;

            long subj = 7;
            long mod = 20201227;
            
            long value = 1;
            var doorDone = false;
            var cardDone = false;
            while (!doorDone || !cardDone)
            {
                value *= subj;
                value %= mod;

                if (value == doorPub && !doorDone)
                {
                    doorDone = true;
                    doorLoopSize++;
                }
                else if (!doorDone)
                {
                    doorLoopSize++;
                }

                if (value == cardPub && !cardDone)
                {
                    cardDone = true;
                    cardLoopSize++;
                }
                else if (!cardDone)
                {
                    cardLoopSize ++;
                }
            }

            subj = doorPub;
            value = 1;

            for (long i = 0; i < cardLoopSize; i++)
            {
                value *= subj;
                value %= mod;
            }

            Console.WriteLine(value);
        }
    }
}
