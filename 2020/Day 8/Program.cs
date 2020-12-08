using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_8
{
    class Program
    {
        static void Main(string[] args)
        {
            List<(string instruction, int value)> origLines = File.ReadLines("./input.txt").Select(l =>
            {
                var split = l.Split(' ');
                return (split[0], int.Parse(split[1]));
            }).ToList();
            var comp = new Computer();

            for(int j = 0; j < origLines.Count; j++)
            {
                var lines = origLines.ToList();

                if (origLines[j].instruction.Equals("nop"))
                {
                    lines[j] = ("jmp", lines[j].value);
                }
                else if (origLines[j].instruction.Equals("jmp"))
                {
                    lines[j] = ("nop", lines[j].value);
                }
                else
                {
                    continue;
                }

                (int acc, bool terminatedProperly) = comp.Calculate(lines);

                Console.WriteLine(acc);

                if (terminatedProperly)
                {
                    Console.WriteLine("TerminatedProperly");
                    break;
                }
            }
        }
    }

    public class Computer
    {
        public (int acc, bool terminatedProperly) Calculate(List<(string instruction, int value)> lines)
        {
            var visitedLines = new List<int>();
            var acc = 0;
            var terminatedProperly = false;
            int i = 0;

            while (true)
            {
                if (visitedLines.Contains(i)){
                    break;
                }
                if (i == lines.Count())
                {
                    terminatedProperly = true;
                    break;
                }
                visitedLines.Add(i);
                var line = lines[i];

                switch(line.instruction)
                {
                    case "nop":
                        i++;
                        break;
                    case "jmp":
                        i += line.value;
                        break;
                    case "acc":
                        acc += line.value;
                        i++;
                        break;
                }
            }

            return (acc, terminatedProperly);
        }
    }
}
