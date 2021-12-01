using System;
using System.IO;
using System.Linq;

namespace _1
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("./input.txt");
            var nums = lines.Select(l => int.Parse(l)).ToList();

            int val = 0;
            for (int i = 1; i < nums.Count(); i++)
            {
                if (nums[i] > nums[i-1])
                {
                    val++;
                }
            }

            Console.WriteLine(val);
            
            val = 0;
            for (int i = 0; i < nums.Count()-3; i++)
            {
                var v1 = nums[i] + nums[i+1] + nums[i+2];
                var v2 = nums[i+1] + nums[i+2] + nums[i+3];

                if (v1 < v2)
                {
                    val++;
                }
            }
            Console.WriteLine(val);
        }
    }
}
