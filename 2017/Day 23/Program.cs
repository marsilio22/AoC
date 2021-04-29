using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Day_23
{
    class Program
    {
        public static void Main(string[] args)
        {
            var lines = File.ReadAllLines("./input.txt").ToList();
            
            var runner = new Runner(lines);

            runner.Run();

            // 267848112 too high
            // 133924056 too high
            // 6264 ( = 106500/17) too high
            // 1000 incorrect
            // 5194 incorrect
            // 2326 incorrect
            // 4652 incorrect

            var b = 106500;
            var h = 0;
            for (int i = 0; i <= 1000; i++)
            {
                var x = (b + (17 * i));
                for (var d = 2; d <= x/2; d++)
                {
                    if (x % d == 0)
                    {
                        h++;
                        break;
                    }
                }
            }

            Console.WriteLine(h);
        }
    }

    public class Runner
    {
        public List<string> Program { get; set; }

        public Runner(List<string> program)
        {
            this.Program = program;
        }

        public void Run()
        {
            var registers = new Dictionary<string, long>{ {"a", 0} };
            long numberOfMuls = 0;

            for (long i = 0; i < Program.Count() && i >= 0; i++)
            {
                var line = Program[(int)i];
                var splitLine = line.Split(' ');
                long val1 = 0, val2 = 0;

                if (!long.TryParse(splitLine[1], out val1))
                {
                    registers.TryGetValue(splitLine[1], out val1);
                }

                if (splitLine.Count() == 3 && !long.TryParse(splitLine[2], out val2))
                {
                    registers.TryGetValue(splitLine[2], out val2);
                }

                switch(splitLine[0])
                {
                    case "set":
                        registers[splitLine[1]] = val2;
                        break;
                    case "sub":
                        registers[splitLine[1]] = val1 - val2;
                        break;
                    case "mul":
                        registers[splitLine[1]] = val1 * val2;
                        numberOfMuls++;
                        Console.WriteLine(numberOfMuls);
                        break;
                    case "jnz":
                        if (val1 != 0)
                        {
                            i += val2 - 1;
                        }
                        break;
                    default:
                        throw new Exception("oh noes");
                }
            }
        }
    }
}
