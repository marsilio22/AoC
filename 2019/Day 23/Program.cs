using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_23
{
    class Program
    {
        static void Main(string[] args)
        {
            var program = File.ReadAllLines("./input.txt") [0].Split (",").Select (i => long.Parse (i)).ToList ();

            Dictionary<long, IntcodeComputer> network = new Dictionary<long, IntcodeComputer>();
            Dictionary<long, Queue<(long X, long Y)>> packets = new Dictionary<long, Queue<(long X, long Y)>>();
            for (long i = 0; i < 50; i++){
                network.Add(i, new IntcodeComputer(program.ToList()));
                var tellComputerItsAddress = new Queue<long>();
                tellComputerItsAddress.Enqueue(i);
                network[i].Calculate(tellComputerItsAddress);
                
                packets.Add(i, new Queue<(long X, long Y)>());
            }

            (long X, long Y) NAT = (0, 0);
            HashSet<(long X, long Y)> sentNatValues = new HashSet<(long X, long Y)>();

            while (true){
                bool idle = true;
                for (long i = 0; i< 50; i++){
                    var input = new Queue<long>();
                    var peek = packets[i].TryPeek(out var peekedValue);
                    if (peek){
                        var packet = packets[i].Dequeue();
                        input.Enqueue(packet.X);
                        input.Enqueue(packet.Y);
                        idle = false;
                    }
                    else
                    {
                        input.Enqueue(-1);   
                    }
                    network[i].Calculate(input);

                    while(network[i].ReturnValues.Any()){
                        var addr = network[i].ReturnValues.Dequeue();
                        var X = network[i].ReturnValues.Dequeue();
                        var Y = network[i].ReturnValues.Dequeue();
                        if (addr < 50){
                            packets[addr].Enqueue((X, Y));
                        }
                        else if (addr == 255){
                            NAT = (X, Y);
                        }
                    } 
                }

                if (idle){
                    packets[0].Enqueue(NAT);
                    if(!sentNatValues.Add(NAT)){
                        Console.WriteLine(NAT.Y); 
                        return;
                    }
                }
            }
        }
    }

    
    class IntcodeComputer{
        private int currentIndex;
        private long relativeBase;
        private List<long> program;

        public Queue<long> ReturnValues { get; }

        public IntcodeComputer(List<long> program){
            var tenThousand0s = new List<long>(new long[10000]);

            this.program = program;
            this.program.AddRange(tenThousand0s);

            // set defaults
            this.currentIndex = 0;
            this.relativeBase = 0;
            this.ReturnValues = new Queue<long>();
        }

        /// <summary>
        /// Calculates the output state of the given set of codes, for the given noun and verb
        /// </summary>
        /// <returns>A value indicating whether or not the process has halted (if not then it is awaiting more input)
        /// </returns>
        public bool Calculate (Queue<long> input) {
            for (int i = currentIndex;;) {
                var opcode = program[i].ToString ();
                string instruction, modes;
                if (opcode.Length > 2) {
                    instruction = opcode.Substring (opcode.Length - 2);
                    modes = opcode.Substring (0, opcode.Length - 2);
                    modes = modes.PadLeft (3, '0');
                } else {
                    instruction = opcode;
                    modes = "000";
                }

                var INTstruction = int.Parse (instruction);
                int modeA, modeB, modeC;
                // For the modes. 0 is Position Mode and 1 is Immediate Mode
                modeC = int.Parse (modes[2].ToString ());
                modeB = int.Parse (modes[1].ToString ());
                modeA = int.Parse (modes[0].ToString ());


                long indexA = 0, indexB = 0, indexC = 0;
                // Check if the opcode takes 0, 1, 2, or 3 arguments, and initialise the appripriate mode values
                var oneParameterOpcodes = new [] {3, 4, 9};
                var twoParameterOpcodes = new [] {5, 6};
                var threeParameterOpcodes = new []{1, 2, 7, 8};

                if (threeParameterOpcodes.Contains(INTstruction)){
                    indexA = GetIndexForMode (modeA, program, relativeBase, i, 3);
                    indexB = GetIndexForMode (modeB, program, relativeBase, i, 2);
                    indexC = GetIndexForMode (modeC, program, relativeBase, i, 1);
                }
                if (twoParameterOpcodes.Contains(INTstruction)){
                    indexB = GetIndexForMode (modeB, program, relativeBase, i, 2);
                    indexC = GetIndexForMode (modeC, program, relativeBase, i, 1);
                }
                if (oneParameterOpcodes.Contains(INTstruction)){
                    indexC = GetIndexForMode (modeC, program, relativeBase, i, 1);
                }
                switch (INTstruction) {
                    case 1: // Add
                        program[(int)indexA] = program[(int)indexC] + program[(int)indexB];
                        i += 4;
                        break;
                    case 2: // Multiply
                        program[(int)indexA] = program[(int)indexC] * program[(int)indexB];
                        i += 4;
                        break;
                    case 3: // Input
                        if (!input.Any ()) {
                            this.currentIndex = i;
                            return false;
                        }
                        
                        program[(int)indexC] = input.Dequeue ();
                        i += 2;
                        break;
                    case 4: // Output
                        ReturnValues.Enqueue(program[(int)indexC]);
                        i += 2;
                        break;
                    case 5: // Jump if true
                        long test = program[(int)indexC];
                        i = test == 0 ? i + 3 : (int)program[(int)indexB];
                        break;
                    case 6: // Jump if false
                        test = program[(int)indexC];
                        i = test != 0 ? i + 3 : (int)program[(int)indexB];
                        break;
                    case 7: // Less Than
                        bool condition = program[(int)indexC] < program[(int)indexB];
                        program[(int)indexA] = condition ? 1 : 0;
                        i += 4;
                        break;
                    case 8: // Equals
                        condition = program[(int)indexC] == program[(int)indexB];
                        program[(int)indexA] = condition ? 1 : 0;
                        i += 4;
                        break;
                    case 9: // Modify relative base
                        relativeBase += program[(int)indexC];
                        i += 2;
                        break;
                    case 99:
                        return true;
                    default:
                        throw new ArgumentException ();
                }
            }
        }

        private static long GetIndexForMode (int mode, List<long> intcode, long relativeBase, int i, int indexIncrease) {
            if (mode == 0) {
                return (int)intcode[i + indexIncrease];
            } else if (mode == 1) {
                return i + indexIncrease;
            } else if (mode == 2) {
                return (int)(relativeBase + intcode[i + indexIncrease]);
            } else {
                throw new KeyNotFoundException ();
            }
        }
    }
}
