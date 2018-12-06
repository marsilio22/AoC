using System;
using System.IO;
using System.Linq;

namespace Day_4
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines =  File.ReadAllLines("C:/dev/AoC/2017/Day 4/input.txt");

            int result = lines.Length; // the number of valid pass phrases, reduced each time one is invalid
            foreach(var line in lines)
            {
                string[] splitLine = line.Split(' ');
                foreach (var word in splitLine)
                {
                    if (splitLine.Count(w => IsAnagramOf(word, w)) >= 2){
                        result--;
                        break;
                    }
                }
            }

            Console.WriteLine(result);
        }

        public static bool IsAnagramOf(string first, string second)
        {
            var firstChars = first.ToCharArray();
            var secondChars = second.ToCharArray();
            if (firstChars.Length != secondChars.Length){
                return false;
            }

            foreach(var ch in firstChars)
            {
                // if all the characters in the second string are not equal to the 
                // current character in the first string, then the two cannot be 
                // anagrams of one another
                if (secondChars.All(c => !c.Equals(ch)))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
