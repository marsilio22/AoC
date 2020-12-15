using System;
using System.Linq;
using System.Collections.Generic;

namespace Day_15
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = "14,3,1,0,9,5";
            var nums = input.Split(',').Select(c => int.Parse(c)).ToList();

            ICollection<int> parts = new [] {2020, 30000000};

            foreach(var part in parts)
            {
                // this will be keyed by the numbers spoken, and the value will be the last turn they were said, 
                // which might need some mental gymnastics to think about further down
                var saidSoFar = new Dictionary<long, int>();
                var nextNumber = 0;
                var lastSpoken = 0;

                for (int i = 0; i < part; i++)
                {
                    if (i < nums.Count())
                    {
                        saidSoFar.Add(nums[i], i);
                        lastSpoken = nums[i];
                        nextNumber = i - saidSoFar[lastSpoken];
                    }
                    else
                    {
                        var currentNumber = nextNumber;
                        nextNumber = saidSoFar.ContainsKey(currentNumber) ? i - saidSoFar[currentNumber] : 0;
                        saidSoFar[currentNumber] = i;
                        lastSpoken = currentNumber;
                    }
                }

                Console.WriteLine(lastSpoken);
            }
        }
    }
}
