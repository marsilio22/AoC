using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Day3
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> lines = File.ReadAllLines("./input.txt").ToList();

            var count = 0;

            foreach(var line in lines)
            {
                var nums = line.Split(' ').Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => int.Parse(s)).ToList();
                if (nums[0] + nums[1] > nums[2] && nums[0] + nums[2] > nums[1] && nums[1] + nums[2] > nums[0])
                {
                    count++;
                }
            }

            Console.WriteLine(count);

            count = 0;
            for(int i = 0; i < lines.Count; i += 3)
            {
                var nums1 = lines[i].Split(' ').Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => int.Parse(s)).ToList();
                var nums2 = lines[i+1].Split(' ').Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => int.Parse(s)).ToList();
                var nums3 = lines[i+2].Split(' ').Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => int.Parse(s)).ToList();

                for (int j = 0; j < 3; j++)
                {
                    if (nums1[j] + nums2[j] > nums3[j] && nums1[j] + nums3[j] > nums2[j] && nums2[j] + nums3[j] > nums1[j]) 
                    {
                        count++;
                    }
                }
            }

            Console.WriteLine(count);
        }
    }
}
