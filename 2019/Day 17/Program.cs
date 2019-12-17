using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_17
{
    class Program
    {
        static void Main(string[] args)
        {
            var program = File.ReadAllLines("./input.txt") [0].Split (",").Select (i => long.Parse (i)).ToList ();
            var computer = new IntcodeComputer(program.ToList());

            computer.Calculate(new Queue<long>());

            // while(computer.ReturnValues.Any()){
            //     var character = computer.ReturnValues.Dequeue();
            //     Console.Write((char)character);
            // }
            // Console.WriteLine();

            var map = new Dictionary<(int x, int y), char>();
            int x = 0, y = 0;
            while (computer.ReturnValues.Any()){
                var character = (char)computer.ReturnValues.Dequeue();
                if (character == 10)
                {
                    y ++;
                    x = 0;
                    continue;
                }
                map.Add((x, y), character);
                x ++;
            }

            var crossingPoints = map.Where(m => 
                m.Value == '#' &&
                map.ContainsKey((m.Key.x + 1, m.Key.y)) && map[(m.Key.x + 1, m.Key.y)] == '#' &&
                map.ContainsKey((m.Key.x, m.Key.y + 1)) && map[(m.Key.x, m.Key.y + 1)] == '#' &&
                map.ContainsKey((m.Key.x - 1, m.Key.y)) && map[(m.Key.x - 1, m.Key.y)] == '#' &&
                map.ContainsKey((m.Key.x, m.Key.y - 1)) && map[(m.Key.x, m.Key.y - 1)] == '#' 
            ).ToList();

            var result = crossingPoints.Select(m => m.Key.x * m.Key.y).Sum();

            Console.WriteLine(result);

            /*
            From observation, the program needed is:
                L8, R10, L8, R8, L12, R8, R8, L8, R10, L8, R8, L8, R6, R6, R10, L8, L8, R6, R6, R10, L8, L8, R10, L8, R8, L12, R8, R8, L8, R6, R6, R10, L8, L12, R8, R8, L12, R8, R8
            Which splits into
                L8, R10, L8, R8, 
                L12, R8, R8, 
                L8, R10, L8, R8, 
                L8, R6, R6, R10, L8, 
                L8, R6, R6, R10, L8, 
                L8, R10, L8, R8, 
                L12, R8, R8, 
                L8, R6, R6, R10, L8, 
                L12, R8, R8, 
                L12, R8, R8
            So create the parts
                A = L8, R10, L8, R8
                B = L12, R8, R8
                C = L8, R6, R6, R10, L8
            And run them in the order
                A B A C C A B C B B
            */

            Queue<long> input = new Queue<long>();
            var movementRoutine = "A,B,A,C,C,A,B,C,B,B\n".ToCharArray();
            foreach(var character in movementRoutine){
                input.Enqueue((long)character);
            }

            var A = "L,8,R,10,L,8,R,8\n".ToCharArray();
            var B = "L,12,R,8,R,8\n".ToCharArray();
            var C = "L,8,R,6,R,6,R,10,L,8\n".ToCharArray();
            var continuousVideo = "n\n";

            foreach(var character in A.Concat(B).Concat(C).Concat(continuousVideo)){
                input.Enqueue((long)character);
            }

            program[0] = 2;
            computer = new IntcodeComputer(program);

            computer.Calculate(new Queue<long>());
            computer.Calculate(input);
            
            while (computer.ReturnValues.Count != 1){
                Console.Write((char)computer.ReturnValues.Dequeue());
            }

            Console.WriteLine(computer.ReturnValues.Dequeue());
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
