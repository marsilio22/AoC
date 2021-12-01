using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Day7
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> lines = File.ReadAllLines("./input.txt").ToList();

            var searchCollection = (from a in Enumerable.Range('a', 26)
            from b in Enumerable.Range('a', 26)
            where a != b
            select(((char)a).ToString() + (char)b + (char)b + (char)a)).ToList();

            var count = 0;

            foreach(var line in lines)
            {
                string lineOutsideBrackets = string.Empty;
                string lineInsideBrackets = string.Empty;

                char lastBracket = ']';

                var line2 = line;

                while (line2.Contains('[') || line2.Contains(']'))
                {
                    var nextBracketIndex = line2.IndexOfAny(new []{'[', ']'});

                    var nextBracket = line2[nextBracketIndex];

                    if (nextBracket == lastBracket)
                    {
                        // two of the same bracket in a row
                        continue;
                    }

                    if (lastBracket == '[')
                    {
                        lineInsideBrackets += line2.Substring(0, nextBracketIndex) + '|';
                    }
                    else if (lastBracket == ']')
                    {
                        lineOutsideBrackets += line2.Substring(0, nextBracketIndex) + '|';
                    }

                    lastBracket = line2[nextBracketIndex];
                    line2 = line2.Substring(nextBracketIndex + 1);
                }

                lineOutsideBrackets += line2;


                if (!searchCollection.Any(a => lineInsideBrackets.Contains(a)) && searchCollection.Any(a => lineOutsideBrackets.Contains(a)))
                {
                    count++;
                }
            }
            
            Console.WriteLine(count);
        }
    }
}
