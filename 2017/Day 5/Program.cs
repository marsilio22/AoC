using System;
using System.IO;
using System.Linq;

namespace Day_5
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines("./input.txt");
            var indices = lines.Select(l => int.Parse(l)).ToArray();
            var currentIndex = 0;
            var count = 0;
            while(currentIndex < indices.Length)
            {
                count++;

                var nextIndex = currentIndex + indices[currentIndex];
                if (indices[currentIndex] < 3)
                {
                    indices[currentIndex]++;
                }
                else
                {
                    indices[currentIndex]--;
                }
                currentIndex = nextIndex;
            }

            Console.WriteLine(count);
        }
    }
}
