using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_16
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

            var device = new Device();

            var testResults = new Dictionary<int, List<List<string>>>();

            bool fin = false;
            int i = 0;
            int part1Result = 0;
            while (!fin)
            {
                if (lines[i].StartsWith("Before:"))
                {
                    var initialState = lines[i].Split('[')[1].Split(']')[0].Split(", ").Select(s => int.Parse(s)).ToList();
                    i++;
                    var instruction = new Instruction(lines[i].Split(' ').Select(s => int.Parse(s)).ToList());
                    i++;
                    var finalState = lines[i].Split('[')[1].Split(']')[0].Split(", ").Select(s => int.Parse(s)).ToList();
                    i += 2;

                    var testResult = device.TestInstruction(initialState, instruction, finalState);

                    if (!testResults.ContainsKey(instruction.OpCode))
                    {
                        testResults[instruction.OpCode] = new List<List<string>>();
                    }

                    testResults[instruction.OpCode].Add(testResult);

                    if (testResult.Count >= 3){
                        part1Result++;
                    }
                }
                else
                {
                    fin = true;
                }
            }

            Console.WriteLine(part1Result);

            var finalisedResults = new Dictionary<int, List<string>>();

            foreach(var thing in testResults)
            {
                // This is the aggregate intersection of all the results, which will still be more than 1 thing in many cases
                finalisedResults.Add(thing.Key, thing.Value.Aggregate((x, y) => x.Intersect(y).ToList()));
            }

            Dictionary<int, string> instructionLookup = new Dictionary<int, string>();
            int j = 0;
            while(instructionLookup.Count < 16){
                var lengthIResults = finalisedResults.Where(r => r.Value.Count == j).ToList();
                j++;
                foreach(var result in lengthIResults)
                {
                    result.Value.RemoveAll(s => instructionLookup.Values.Contains(s));
                    if (result.Value.Count == 1){
                        instructionLookup.Add(result.Key, result.Value.Single());
                    }
                }
                

                if (j > 16){
                    j = 0;
                }
            }

            foreach(var thing in instructionLookup)
            {
                Console.WriteLine($"{thing.Key}: {thing.Value}");
            }

            device.InstructionLookup = instructionLookup;
            device.ResetState();
            i += 2; // first line of the next set of input

            while (i < lines.Count)
            {
                var instruction = new Instruction(lines[i].Split(' ').Select(s => int.Parse(s)).ToList());
                device.ParseInstruction(instruction);
                i++;
            }

            Console.WriteLine(Device.Registers[0]);
        }
    }

    public class Instruction{
        public int OpCode { get; set; }
        public int InputA { get; set; }
        public int InputB { get; set; }
        public int OutputC { get; set; }

        public Instruction(List<int> values){
            this.OpCode = values[0];
            this.InputA = values[1];
            this.InputB = values[2];
            this.OutputC = values[3];
        }
    }

    public class Device
    {
        public static List<int> Registers {get; private set;} = new List<int> {0, 0, 0, 0};

        public Dictionary<int, string> InstructionLookup {get; set;} = new Dictionary<int, string>();

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

        public List<string> TestInstruction(List<int> initialState, Instruction instruction, List<int> finalState)
        {
            var result = new List<string>();
            foreach(var possibility in Instructions)
            {
                this.SetState(initialState);
                string ins = possibility.Value(instruction);
                
                if( finalState[0] == Registers[0] && 
                    finalState[1] == Registers[1] && 
                    finalState[2] == Registers[2] && 
                    finalState[3] == Registers[3])
                {
                    result.Add(ins);
                }
            }

            return result;
        }

        private void SetState(List<int> state)
        {
            for(int i = 0; i < 4; i++){
                Registers[i] = state[i];
            };
        }

        public void ResetState(){
            this.SetState(new List<int> {0, 0, 0, 0});
        }

        public void ParseInstruction(Instruction instruction)
        {
            string instructionKey = InstructionLookup[instruction.OpCode];
            Instructions[instructionKey](instruction); // lol functional
        }
    }
}
