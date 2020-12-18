using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CSharp;
using System.CodeDom;
using System.Text.RegularExpressions;

namespace Day_18
{
    public class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadLines("testinput.txt");
            long ans = 0;

            foreach(var line in lines){
                int i = 0;
                ans += DoMathsPart1(line, ref i);
            }

            Console.WriteLine(ans);

            // part 2 
            var nonNumericChars = new [] {'(', ')', '+', '*'};

            // foreach(var line in lines){
            //     while (line.Any(l => nonNumericChars.Contains(l)))
            //     {
            //         //sort out the lowest level of bracketed numbers
            //     }
            // }

            var additionRegex = new Regex(@"\d+ \+ \d+");
            var multiplicationRegex = new Regex(@"\d+ \* \d+");
            var multiplicationInBracketsRegex = new Regex(@"\(\d+ \* \d+");
            var multiplicationInBracketsFollowedImmediatelyByAnAdditionToABracketRegex = new Regex(@"\(\d+ \* \d+ \+ \(");

            var singleNumberSurroundedByBrackets = new Regex(@"\(\d+\)");
            foreach(var line in lines){
                var lineCopy = line;
                Console.WriteLine(lineCopy);

                while (lineCopy.Any(l => nonNumericChars.Contains(l))){
                    while(additionRegex.Matches(lineCopy).Count > 0)
                    {
                        var test = additionRegex.Match(lineCopy);
                        var spl = test.Value.Split(" + ");
                        var result = long.Parse(spl[0]) + long.Parse(spl[1]);

                        lineCopy = lineCopy.Replace(test.Value, result.ToString());
                Console.WriteLine(lineCopy);
                    }

                    if (!lineCopy.Contains('+'))
                    {
                        while(multiplicationRegex.Matches(lineCopy).Count > 0)
                        {
                            var test = multiplicationRegex.Match(lineCopy);
                            var spl = test.Value.Split(" * ");
                            var result = long.Parse(spl[0]) * long.Parse(spl[1]);

                            lineCopy = lineCopy.Replace(test.Value, result.ToString());
                Console.WriteLine(lineCopy);
                        }
                    }
                    else 
                    {
                        // argh this is wrong I think
                        // need to do the multiplication when the regex matches 
                        // "things that are in brackets that need multiplying, where the brackets contain no other open bracket characters
            var myThought = new Regex(@"\(\d+ \* ?\d+ ?\*? ?\d* ?\*? ?\d* ?\*? ?\d*\)");
            var t = myThought.Match(lineCopy);
                        while(myThought.Matches(lineCopy).Count > 0)
                        {
                            var test = multiplicationRegex.Match(lineCopy);
                            var spl = test.Value.Split(" * ");
                            var result = long.Parse(spl[0]) * long.Parse(spl[1]);

                            lineCopy = lineCopy.Replace(spl[0] + " * " + spl[1], result.ToString());
                Console.WriteLine(lineCopy);
                        }
                //         while(multiplicationInBracketsRegex.Matches(lineCopy).Count > 0 && 
                //             !multiplicationInBracketsFollowedImmediatelyByAnAdditionToABracketRegex.Match(lineCopy).Value.Contains(multiplicationInBracketsRegex.Match(lineCopy).Value))
                //         {
                //             var matchesWeDontWantToCompute = multiplicationInBracketsFollowedImmediatelyByAnAdditionToABracketRegex.Matches(lineCopy).Select(m => m.Value);
                //             var test = multiplicationInBracketsRegex.Matches(lineCopy).First(m => !matchesWeDontWantToCompute.Any(m2 => m2.Contains(m.Value)));

                //             var spl = test.Value.Split('(')[1].Split(" * ");
                //             var result = long.Parse(spl[0]) * long.Parse(spl[1]);

                //             lineCopy = lineCopy.Replace(test.Value.Split('(')[1], result.ToString());
                // Console.WriteLine(lineCopy);
                //         }
                    }

                    while(singleNumberSurroundedByBrackets.Matches(lineCopy).Count > 0)
                    {
                        var test = singleNumberSurroundedByBrackets.Match(lineCopy);
                        var spl = test.Value.Split('(')[1].Split(')')[0];

                        lineCopy = lineCopy.Replace(test.Value, spl);
                Console.WriteLine(lineCopy);
                    }
                }
                
                Console.WriteLine(lineCopy);
                Console.WriteLine();
            }
        }

        public static long DoMathsPart1(string maths, ref int i)
        {
            long ans = 0;
            var operation = '+'; // initial operation to make ans be the first value

            for(; i< maths.Length; i++)
            {
                var character = maths[i];
                if(character == ')')
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

        public static long DoSingleOp(long a, long b, char op){
            switch(op){
                case '+':
                    return a + b;
                case '*':
                    return a * b;
            }
            throw new Exception("Shouldn't happen");
        }
    }

    // public class MyNumber
    // {
    //     private long v;

    //     public MyNumber(long v)
    //     {
    //         this.value = v;
    //     }

    //     public long value {get;set;}

    //     public static MyNumber operator +(MyNumber a, MyNumber b)
    //     {
    //         return new MyNumber(a.value*b.value);
    //     }

    //     public static MyNumber operator *(MyNumber a, MyNumber b)
    //     {
    //         return new MyNumber(a.value + b.value);
    //     }
    // }
}
