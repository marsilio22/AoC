using System;
using System.IO;

namespace Day_2
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(@"C:\dev\AoC\2017\Day 2\input.txt");
            
            int chk = 0;
            
            foreach(var line in lines)
            {
                var splitLine = line.Split('\t');
                int biggest = 0, smallest = int.MaxValue;

                for(var i = 0; i < splitLine.Length; i++)
                {
                    int value = int.Parse(splitLine[i]);

                    for(var j = 0; j < splitLine.Length; j++)
                    {
                        if (i == j) continue;

                        int iParsed = int.Parse(splitLine[i]), jParsed = int.Parse(splitLine[j]);

                        if (iParsed % jParsed == 0 || jParsed % iParsed == 0){
                            biggest = jParsed > iParsed ? jParsed : iParsed;
                            smallest = jParsed > iParsed ? iParsed : jParsed;
                        }
                    }
                }

                chk += biggest / smallest;
            }

            Console.WriteLine(chk);
        }
    }
}
