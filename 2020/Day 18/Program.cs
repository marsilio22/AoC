using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day_18
{
    public class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadLines(@"./input.txt").ToList();
            long ans = 0;

            foreach (string line in lines)
            {
                int i = 0;
                ans += DoMathsPart1(line, ref i);
            }

            Console.WriteLine("Part 1: " + ans);

            ans = 0;
            foreach (string line in lines)
            {
                ans += DoMathsPart2(line, false);
            }

            Console.WriteLine("Part 2:" + ans);
        }

        public static long DoMathsPart1(string maths, ref int i)
        {
            long ans = 0;
            char operation = '+'; // initial operation to make ans be the first value

            for (; i < maths.Length; i++)
            {
                char character = maths[i];
                if (character == ')')
                {
                    return ans;
                }

                if (character == '(')
                {
                    i++;
                    ans = DoSingleOp(ans, DoMathsPart1(maths, ref i), operation);
                }
                else if (int.TryParse(character.ToString(), out int num))
                {
                    ans = DoSingleOp(ans, num, operation);
                }
                else if (character == '+' || character == '*')
                {
                    operation = character;
                }
            }

            return ans;
        }

        public static long DoSingleOp(long a, long b, char op)
        {
            switch (op)
            {
                case '+':
                    return a + b;
                case '*':
                    return a * b;
            }
            throw new Exception("Shouldn't happen");
        }

        public static long DoMathsPart2(string line, bool log)
        {
            char[] nonNumericChars = new[] { '(', ')', '+', '*' };

            Regex additionRegex = new Regex(@"\d+ \+ \d+");
            Regex multiplicationRegex = new Regex(@"\d+ \* \d+");
            Regex multiplicationsInABracketRegex = new Regex(@"\(\d+ \* ?\d+ ?\*? ?\d* ?\*? ?\d* ?\*? ?\d* ?\*? ?\d* ?\*? ?\d* ?\*? ?\d* ?\*? ?\d* ?\*? ?\d*\)");
            Regex singleNumberSurroundedByBrackets = new Regex(@"\(\d+\)");

            string lineCopy = line;

            if (log) 
                Console.WriteLine(lineCopy);

            while (lineCopy.Any(l => nonNumericChars.Contains(l)))
            {
                while (additionRegex.Matches(lineCopy).Count > 0)
                {
                    Match test = additionRegex.Match(lineCopy);
                    string[] spl = test.Value.Split(" + ");
                    long result = long.Parse(spl[0]) + long.Parse(spl[1]);

                    lineCopy = lineCopy.Substring(0, test.Index) + result +
                               lineCopy.Substring(test.Index + test.Value.Length);

                    if (log)
                        Console.WriteLine(lineCopy);
                }

                if (!lineCopy.Contains('+'))
                {
                    while (multiplicationRegex.Matches(lineCopy).Count > 0)
                    {
                        Match test = multiplicationRegex.Match(lineCopy);
                        string[] spl = test.Value.Split(" * ");
                        long result = long.Parse(spl[0]) * long.Parse(spl[1]);

                        lineCopy = lineCopy.Substring(0, test.Index) + result +
                                   lineCopy.Substring(test.Index + test.Value.Length);

                        if (log)
                            Console.WriteLine(lineCopy);
                    }
                }
                else
                {
                    while (multiplicationsInABracketRegex.Matches(lineCopy).Count > 0)
                    {
                        Match test = multiplicationsInABracketRegex.Match(lineCopy);
                        List<string> spl = test.Value.Split(" * ").Select(c => c.Split('(').Last()).Select(c => c.Split(')').First()).ToList();
                        long result = long.Parse(spl[0]) * long.Parse(spl[1]);
                        string replstr = spl[0] + " * " + spl[1];

                        lineCopy = lineCopy.Substring(0, test.Index + 1) + result +
                                   lineCopy.Substring(test.Index + replstr.Length + 1);

                        if (log)
                            Console.WriteLine(lineCopy);
                    }
                }

                while (singleNumberSurroundedByBrackets.Matches(lineCopy).Count > 0)
                {
                    Match test = singleNumberSurroundedByBrackets.Match(lineCopy);
                    string spl = test.Value.Split('(')[1].Split(')')[0];

                    lineCopy = lineCopy.Substring(0, test.Index) + spl +
                               lineCopy.Substring(test.Index + test.Value.Length);

                    if (log)
                        Console.WriteLine(lineCopy);
                }
            }

            return long.Parse(lineCopy);
        }
    }
}
