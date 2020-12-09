using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_9
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadLines("./input.txt").ToList();

            var nums = new List<int>();

            for(int i = 0; i < 25; i ++)
            {
                nums.Add(int.Parse(lines[i]));
            }

            int num = 0;

            for(int i = 25; i < lines.Count; i++)
            {
                num = int.Parse(lines[i]);

                ICollection<(int, int)> tup = new List<(int, int)>();
                if (nums.Distinct().Count() == nums.Count())
                {
                     tup = (from x in nums
                        from y in nums
                        where x + y == num && x != y
                        select (x, y)).ToList();
                }
                else 
                {
                     tup = (from x in nums
                        from y in nums
                        where x + y == num
                        select (x, y)).ToList();
                }

                if (tup.Any())
                {
                    nums.Add(num);
                    nums.RemoveAt(0);
                }
                else 
                {

                    Console.WriteLine(num);
                    break;
                }
            }

            List<int> numbers = new List<int>();
            int index = 0;
            while(numbers.Sum() != num)
            {
                while (numbers.Sum() <= num){
                    numbers.Add(int.Parse(lines[index]));
                    index ++;
                }

                while (numbers.Sum() > num)
                {
                    numbers.RemoveAt(0);
                }
            }

            Console.WriteLine(numbers.Max() + numbers.Min());
        }
    }
}
