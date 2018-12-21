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
            //var lines = File.ReadAllLines("./input.txt").ToList();

            //var instructions = new List<Instruction>();
            //foreach(var line in lines){
            //    if (line.StartsWith('#'))
            //    {
            //        continue;
            //    }

            //    var parameters = line.Split(' ');
            //    var opName = parameters[0];
            //    var numbers = new List<int>{int.Parse(parameters[1]), int.Parse(parameters[2]), int.Parse(parameters[3])};

            //    instructions.Add(new Instruction(opName, numbers));
            //}

            //var device = new Device();
            //var ipRegister = int.Parse(lines[0].Split(' ')[1]);

            //var lastreg0 = Device.Registers[0];

            //Device.Registers[0] = 0;

            //List<long> previouses = new List<long>();

            //while (Device.Registers[ipRegister] < instructions.Count)
            //{
            //    var instructionNumber = Device.Registers[ipRegister];
            //    var instruction = instructions[(int)instructionNumber];
            //    device.ParseInstruction(instruction);
            //    if (Device.Registers[ipRegister] == 28)
            //    {
            //        Console.WriteLine("Ip is 28");
            //        Console.WriteLine($"{Device.Registers[0]}, {Device.Registers[1]}, {Device.Registers[2]}, {Device.Registers[3]}, {Device.Registers[4]}, {Device.Registers[5]}" );
            //        previouses.Add(Device.Registers[2]);
            //    }

            //    if (previouses.Count(c => c == Device.Registers[2]) > 1)
            //    {
            //        Console.WriteLine($"Register might have looped");
            //    }

            //    Console.WriteLine($"{Device.Registers[0]}, {Device.Registers[1]}, {Device.Registers[2]}, {Device.Registers[3]}, {Device.Registers[4]}, {Device.Registers[5]}" );
            //    Device.Registers[ipRegister] += 1;
            //}

            //Console.WriteLine(Device.Registers[0]);

            long r0 = 0, r1 = 0, r2 = 0, r3 = 0, r4 = 0, r5 = 0;
            
            r5 = r2 | 65536;
            r2 = 16123384;

            var previouses = new List<long>();

            while (r0 != r2)
            {
                r3 = r5 & 255;
                r2 += r3;
                r2 &= 16777215;
                r2 *= 65899;
                r2 &= 16777215;

                if (256 > r5)
                {
                    Console.WriteLine(r2);
                    
                    previouses.Add(r2);

                    if (previouses.Count(c => c == r2) > 1)
                    {
                        break;
                    }

                    if (r2 == r0)
                    {
                        break;
                    }
                    else
                    {
                        r5 = r2 | 65536;
                        r2 = 16123384;
                        continue;
                    }
                }
                else
                {
                    r3 = (long)Math.Floor((r5) / (double) 256);
                }

                r5 = r3;
            }

            Console.WriteLine("Last = " + previouses.Last());
            Console.WriteLine("Second Last (answer) = " + previouses.Last(c => c != previouses.Last()));
            Console.ReadLine();
        }
    }

    public class Instruction{
        public string Op { get; set; }
        public int InputA { get; set; }
        public int InputB { get; set; }
        public int OutputC { get; set; }

        public Instruction(string opName, List<int> values){
            this.Op = opName;
            this.InputA = values[0];
            this.InputB = values[1];
            this.OutputC = values[2];
        }
    }

    public class Device
    {
        public static List<long> Registers {get; private set;} = new List<long> {0, 0, 0, 0, 0, 0};

        public Dictionary<string, Func<Instruction, string>> Instructions = new Dictionary<string, Func<Instruction, string>>
        {
            {"addr", ins => {Registers[ins.OutputC] = Registers[ins.InputA] + Registers[ins.InputB]; return "addr";}},
            {"addi", ins => {Registers[ins.OutputC] = Registers[ins.InputA] + ins.InputB; return "addi";}},
            
            {"mulr", ins => {Registers[ins.OutputC] = Registers[ins.InputA] * Registers[ins.InputB]; return "mulr";}},
            {"muli", ins => {Registers[ins.OutputC] = Registers[ins.InputA] * ins.InputB; return "muli";}},
            
            {"banr", ins => {Registers[ins.OutputC] = Registers[ins.InputA] & Registers[ins.InputB]; return "banr";}},
            {"bani", ins => {Registers[ins.OutputC] = Registers[ins.InputA] & ins.InputB; return "bani";}},
            
            {"borr", ins => {Registers[ins.OutputC] = Registers[ins.InputA] | Registers[ins.InputB]; return "borr";}},
            {"bori", ins => {Registers[ins.OutputC] = Registers[ins.InputA] | ins.InputB; return "bori";}},
            
            {"setr", ins => {Registers[ins.OutputC] = Registers[ins.InputA]; return "setr";}},
            {"seti", ins => {Registers[ins.OutputC] = ins.InputA; return "seti";}},
            
            {"gtir", ins => {Registers[ins.OutputC] = ins.InputA > Registers[ins.InputB] ? 1 : 0; return "gtir";}},
            {"gtri", ins => {Registers[ins.OutputC] = Registers[ins.InputA] > ins.InputB ? 1 : 0; return "gtri";}},
            {"gtrr", ins => {Registers[ins.OutputC] = Registers[ins.InputA] > Registers[ins.InputB] ? 1 : 0; return "gtrr";}},

            {"eqir", ins => {Registers[ins.OutputC] = ins.InputA == Registers[ins.InputB] ? 1 : 0; return "eqir";}},
            {"eqri", ins => {Registers[ins.OutputC] = Registers[ins.InputA] == ins.InputB ? 1 : 0; return "eqri";}},
            {"eqrr", ins => {Registers[ins.OutputC] = Registers[ins.InputA] == Registers[ins.InputB] ? 1 : 0; return "eqrr";}}
        };
        
        public void ParseInstruction(Instruction instruction)
        {
            Func<Instruction, string> instructionFunc = Instructions[instruction.Op];
            instructionFunc(instruction);
        }
        
        private void SetState(List<int> state)
        {
            for(int i = 0; i < 6; i++){
                Registers[i] = state[i];
            };
        }

        public void ResetState(){
            this.SetState(new List<int> {0, 0, 0, 0, 0, 0});
        }
    }
}