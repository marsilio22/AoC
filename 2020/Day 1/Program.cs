using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_1
{
    public class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadLines("./input.txt");

            List<int> nums = new List<int>();

            foreach (var str in input)
            {
                var intLine = int.Parse(str);
                nums.Add(intLine);
            }

            Console.WriteLine("Part 1");
            var ans = ReturnTargetSumPairFromList(2020, nums);
            Console.WriteLine($"{ans.a} + {ans.b} = {ans.a+ans.b}, and {ans.a} * {ans.b} = {ans.a*ans.b}");

            Console.WriteLine("Part 2");

            foreach (var num in nums){
                var target = 2020 - num;
                try{
                    ans = ReturnTargetSumPairFromList(target, nums);
                    Console.WriteLine($"{num} + {ans.a} + {ans.b} = {num+ans.a+ans.b}, and {num} * {ans.a} * {ans.b} = {num * ans.a * ans.b}");
                    break;
                }
                catch (Exception){
                    continue;
                }
            }
        }

        public static (int a, int b) ReturnTargetSumPairFromList(int target, List<int> nums){
            foreach (var num in nums){
                if (nums.Contains(target - num)){
                    return (num, target - num);
                }
            }

            throw new Exception("couldn't do it m8");
        }
    }
}
