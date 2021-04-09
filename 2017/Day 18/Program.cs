using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Day_18
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var lines = File.ReadAllLines("./input.txt").ToList();
            
            var runner0 = new Runner(lines, null, 0);
            var runner1 = new Runner(lines, runner0, 1);
            runner0.OtherRunner = runner1;

            new Thread(() => runner0.Run()).Start();
            new Thread(() => runner1.Run()).Start();

            // 13716 too high
        }
    }

    public class Runner
    {
        public List<string> Program { get; set; }

        public Runner OtherRunner { get; set; }

        public BlockingCollection<long> Queue { get; set; } = new BlockingCollection<long>();

        public int Name { get; set; }

        public Runner(List<string> program, Runner otherRunner, int name)
        {
            this.Program = program;
            this.OtherRunner = otherRunner;
            this.Name = name;
        }

        public void Run()
        {
            var registers = new Dictionary<string, long>{{"p", this.Name}};
            long numberOfMessages = 0;

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
                    case "snd":
                        this.OtherRunner.Queue.Add(val1);
                        numberOfMessages++;
                        break;
                    case "set":
                        registers[splitLine[1]] = val2;
                        break;
                    case "add":
                        registers[splitLine[1]] = val1 + val2;
                        break;
                    case "mul":
                        registers[splitLine[1]] = val1 * val2;
                        break;
                    case "mod":
                        registers[splitLine[1]] = val1 % val2;
                        break;
                    case "rcv":
                        // Console.WriteLine("Last played frequency was " + lastFrequency);
                        if (!this.Queue.TryTake(out var val, 1000))
                        {
                            Console.WriteLine("Runner " + this.Name + " sent " + numberOfMessages + " before deadlock");
                            return;
                        }
                        else
                        {
                            registers[splitLine[1]] = val;
                        }
                        break;
                    case "jgz":
                        if (val1 > 0)
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
