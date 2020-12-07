using System;
using System.Collections.Generic;

namespace Day_15
{
    class Program
    {
        static void Main(string[] args)
        {
            long genA = 277;
            long genB = 349;

            long genAMult = 16807;
            long genBMult = 48271;
            
            long division = 2147483647;

            long reps = 40000000;

            long count = 0;

            for(int i = 0; i < reps; i++)
            {
                genA = (genA * genAMult) % division;
                genB = (genB * genBMult) % division;

                var binA = Convert.ToString(genA, 2).PadLeft(16);
                var binB = Convert.ToString(genB, 2).PadLeft(16);

                if (binA.Substring(binA.Length - 16).Equals(binB.Substring(binB.Length - 16)))
                {
                    count++;
                }
            }

            Console.WriteLine(count);

            reps = 5000000;
            var genAVals = new List<string>();
            var genBVals = new List<string>();
            genA = 277;
            genB = 349;

            while(genAVals.Count <= reps)
            {
                genA = (genA * genAMult) % division;
                var binA = Convert.ToString(genA, 2).PadLeft(16);
                if (binA.Substring(binA.Length - 2).Equals("00"))
                {
                    genAVals.Add(binA);
                }
            }

            while(genBVals.Count <= reps)
            {
                genB = (genB * genBMult) % division;
                var binB = Convert.ToString(genB, 2).PadLeft(16);
                if (binB.Substring(binB.Length - 3).Equals("000"))
                {
                    genBVals.Add(binB);
                }
            }

            count = 0;
            for(int i = 0; i < reps; i++)
            {
                var genAVal = genAVals[i].Substring(genAVals[i].Length - 16);
                var genBVal = genBVals[i].Substring(genBVals[i].Length - 16);

                if (genAVal.Equals(genBVal)){
                    count++;
                }
            }

            Console.WriteLine(count);
        }
    }
}
