using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_21
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("./input.txt");

            var iterations = 18;

            var rules = new Dictionary<string, string>();

            foreach(var line in lines)
            {
                var splitLine = line.Split(" => ");
                rules.Add(splitLine[0], splitLine[1]);
            }

            var image = ".#./..#/###";
            for(int i = 0; i < iterations; i++)
            {
                if (image.EndsWith('/'))
                {
                    image = image.Substring(0, image.Length - 1);
                }

                Print(image);

                var imageLines = image.Split('/');
                var size = Math.Sqrt(image.Count(i => i != '/'));
                image = string.Empty;

                if (size % 2 == 0)
                {
                    // while there are still rows to process
                    for(int j = 0; j < imageLines.Count(); j += 2)
                    {
                        // take the next two rows
                        (string line1, string line2) linesOfInterest = (imageLines[j], imageLines[j+1]);
                        (string line1, string line2, string line3) next = (string.Empty, string.Empty, string.Empty);

                        // go through two columns at a time
                        for (int k = 0; k < linesOfInterest.line1.Length; k += 2)
                        {
                            char c1 = linesOfInterest.line1[k], c2 = linesOfInterest.line1[k+1], 
                                c3 = linesOfInterest.line2[k], c4 = linesOfInterest.line2[k+1];
                            // now rotate & flip until we find the rule in the dict
                            next = RotateFlipTransform2x2(c1, c2, c3, c4, next, rules);
                        }

                        image += string.Join('/', next.line1, next.line2, next.line3) + '/';
                    }
                }
                else if (size % 3 == 0)
                {
                    // while there are still rows to process
                    for(int j = 0; j < imageLines.Count(); j += 3)
                    {
                        // take the next two rows
                        (string line1, string line2, string line3) linesOfInterest = (imageLines[j], imageLines[j+1], imageLines[j+2]);
                        (string line1, string line2, string line3, string line4) next = (string.Empty, string.Empty, string.Empty, string.Empty);

                        // go through two columns at a time
                        for (int k = 0; k < linesOfInterest.line1.Length; k += 3)
                        {
                            char c1 = linesOfInterest.line1[k], c2 = linesOfInterest.line1[k+1], c3 = linesOfInterest.line1[k+2], 
                                c4 = linesOfInterest.line2[k], c5 = linesOfInterest.line2[k+1], c6 = linesOfInterest.line2[k+2],
                                c7 = linesOfInterest.line3[k], c8 = linesOfInterest.line3[k+1], c9 = linesOfInterest.line3[k+2];
                            // now rotate & flip until we find the rule in the dict
                            next = RotateFlipTransform3x3(c1, c2, c3, c4, c5, c6, c7, c8, c9, next, rules);
                        }

                        image += string.Join('/', next.line1, next.line2, next.line3, next.line4) + '/';
                    }
                }
            }

            // 132 too low
            // 133 is correct. Why am I off by 1?!

            // 2111399 too low
            // 2111400 too low - gonna have to fix this off by one somewhere I think.
            // 2221990
            Console.WriteLine(image.Count(c => c == '#'));
        }

        private static void Print(string image)
        {
            var splitImage = image.Split('/');

            foreach(var thing in splitImage){
                Console.WriteLine(thing);
            }
        }

        public static (string line1, string line2, string line3) RotateFlipTransform2x2(char c1, char c2, char c3, char c4, (string line1, string line2, string line3) next, IDictionary<string, string> rules)
        {
            char hold;
            for (int t = 0; t < 2; t++)
            {
                for (int q = 0; q < 4; q ++)
                {
                    var search = c1.ToString()+c2+'/'+c3+c4;
                    if (rules.TryGetValue(search, out string newLines))
                    {
                        // replace with the three rows from the rules
                        //Console.WriteLine($"Found match `{newLines}` for search `{search}`");
                        var splitNewLines = newLines.Split('/');
                        return (next.line1 + splitNewLines[0], next.line2 + splitNewLines[1], next.line3 + splitNewLines[2]);
                    }

                    //rotate
                    hold = c1;
                    c1 = c2;
                    c2 = c4;
                    c4 = c3;
                    c3 = hold;
                }

                // flip
                hold = c1;
                c1 = c2;
                c2 = hold;
                hold = c3;
                c3 = c4;
                c4 = hold;
            }

            throw new Exception("oh noes");
        }

        public static (string line1, string line2, string line3, string line4) RotateFlipTransform3x3(char c1, char c2, char c3, char c4, char c5, char c6, char c7, char c8, char c9, (string line1, string line2, string line3, string line4) next, IDictionary<string, string> rules)
        {
            char hold; 
            char hold2;
            for(int t = 0; t < 2; t++)
            {
                for(int q = 0; q < 4; q++)
                {
                    var search = c1.ToString()+c2+c3+'/'+c4+c5+c6+'/'+c7+c8+c9;
                    if (rules.TryGetValue(search, out string newLines))
                    {
                        // replace with the three rows from the rules
                        //Console.WriteLine($"Found match `{newLines}` for search `{search}`");
                        var splitNewLines = newLines.Split('/');
                        return (next.line1 + splitNewLines[0], next.line2 + splitNewLines[1], next.line3 + splitNewLines[2], next.line4 + splitNewLines[3]);
                    }

                    //rotate
                    hold = c1;
                    hold2 = c2;
                    c1 = c3;
                    c2 = c6;
                    c3 = c9;
                    c6 = c8;
                    c9 = c7;
                    c8 = c4;
                    c7 = hold;
                    c4 = hold2;
                }

                // flip
                hold = c1;
                c1 = c3;
                c3 = hold;
                hold = c4;
                c4 = c6;
                c6 = hold;
                hold = c7;
                c7 = c9;
                c9 = hold;
            }

            throw new Exception("oh noes");
        }
    }
}