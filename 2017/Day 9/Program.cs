using System;
using System.IO;
using System.Linq;

namespace Day_9
{
    class Program
    {
        static void Main(string[] args)
        {
            var RealLine = File.ReadLines("./input.txt").ToList()[0];

            var line1 = "{}";
            var line2 = "{{{}}}";
            var line3 = "{{},{}}";
            var line4 = "{{{},{},{{}}}}";
            var line5 = "{<a>,<a>,<a>,<a>}";
            var line6 = "{{<ab>},{<ab>},{<ab>},{<ab>}}";
            var line7 = "{{<!!>},{<!!>},{<!!>},{<!!>}}";
            var line8 = "{{<a!>},{<a!>},{<a!>},{<ab>}}";

            var line9 = "{<>}";
            var line10 = "{<random characters>}";
            var line11 = "{<<<<>}";
            var line12 = "{<{!>}>}";
            var line13 = "{<!!>}";
            var line14 = "{<!!!>>}";
            var line15 = "{<{o\"i!a,<{i<a>}";

            var testLines = new [] {line1, line2, line3, line4, line5, line6, line7, line8, line9, line10, line11, line12, line13, line14, line15, RealLine};


            foreach(var line in testLines){
                char previousCharacter = line[0];
                var currentScore = 1;
                var currentWorth = 2;
                bool currentlyInGarbage = false;
                bool currentCharacterIsNegated = false;
                var garbageCharacterCount = 0;

                for(var i = 1; i < line.Length; i++){
                    if (line[i] == '{' && !currentlyInGarbage)
                    {
                        currentScore += currentWorth;
                        currentWorth++;
                    }

                    if (line[i] == '}' && !currentlyInGarbage)
                    {
                        currentWorth--;
                    }

                    if (line[i] == '<' && !currentlyInGarbage)
                    {
                        currentlyInGarbage = true;
                        continue;
                    }

                    if (line[i] == '>' && currentlyInGarbage && !currentCharacterIsNegated)
                    {
                        currentlyInGarbage = false;
                        continue;
                    }

                    if (line[i] == '!' && currentlyInGarbage && !currentCharacterIsNegated)
                    {
                        currentCharacterIsNegated = true;
                        continue;
                    }

                    if(currentlyInGarbage && !currentCharacterIsNegated){
                        garbageCharacterCount++;
                    }

                    if (currentCharacterIsNegated)
                    {
                        currentCharacterIsNegated = false;
                    }
                }

                Console.WriteLine($"Score: {currentScore}");
                Console.WriteLine($"Garbage: {garbageCharacterCount}");

            }

            // 4805 too high
            // 4804 too high
        }
    }
}
