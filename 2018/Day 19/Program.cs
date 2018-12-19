using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_19
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("./input.txt").ToList();
            // var lines = new List<string>{
            //     "Before: [3, 2, 1, 1]",
            //     "9 2 1 2",
            //     "After: [3, 2, 2, 1]",
            //     "",
            //     ""
            // };

            var instructions = new List<Instruction>();
            foreach(var line in lines){
                if (line.StartsWith('#'))
                {
                    continue;
                }

                var parameters = line.Split(' ');
                var opName = parameters[0];
                var numbers = new List<int>{int.Parse(parameters[1]), int.Parse(parameters[2]), int.Parse(parameters[3])};

                instructions.Add(new Instruction(opName, numbers));
            }

            var device = new Device();
            var ipRegister = int.Parse(lines[0].Split(' ')[1]);

            var lastreg0 = Device.Registers[0];

            while (Device.Registers[ipRegister] < instructions.Count)
            {
                var instructionNumber = Device.Registers[ipRegister];
                var instruction = instructions[instructionNumber];
                device.ParseInstruction(instruction);
                if (lastreg0 != Device.Registers[0])
                {
                    Console.WriteLine(
                        $"{Device.Registers[0]}, {Device.Registers[1]}, {Device.Registers[2]}, {Device.Registers[3]}, {Device.Registers[4]}, {Device.Registers[5]}" );
                    lastreg0 = Device.Registers[0];
                }
                Device.Registers[ipRegister] += 1;
            }

            Console.WriteLine(Device.Registers[0]);

            // Part 2
            device = new Device();
            device.ResetState();

            ipRegister = int.Parse(lines[0].Split(' ')[1]);
            Device.Registers[0] = 1;
            lastreg0 = Device.Registers[0];

            while (Device.Registers[ipRegister] < instructions.Count)
            {
                var instructionNumber = Device.Registers[ipRegister];
                var instruction = instructions[instructionNumber];
                device.ParseInstruction(instruction);
                                if (lastreg0 != Device.Registers[0])
                {
                    Console.WriteLine(
                        $"{Device.Registers[0]}, {Device.Registers[1]}, {Device.Registers[2]}, {Device.Registers[3]}, {Device.Registers[4]}, {Device.Registers[5]}" );
                    lastreg0 = Device.Registers[0];
                }
                Device.Registers[ipRegister] += 1;
            }

            Console.WriteLine(Device.Registers[0]);
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
        public static List<int> Registers {get; private set;} = new List<int> {0, 0, 0, 0, 0, 0};

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
