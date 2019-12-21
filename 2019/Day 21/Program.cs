using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Day_21
{
    class Program
    {
        static void Main(string[] args)
        {
            var program = File.ReadAllLines("./input.txt") [0].Split (",").Select (i => long.Parse (i)).ToList ();
            var computer = new IntcodeComputer(program.ToList());

            // Part 1 program
            var inputProgram = new [] {
                // "NOT A T", // T = !A
                // "NOT C J", // J = !C
                // "AND D J", // J = D && !C
                // "OR T J", // J = !A || (!C && D)
            // The above works for part 1, but it's not that neat and makes part 2 a nightmare.....
            // The below works, and makes much more sense. now part 2.
                // "OR A T",  // T = A -> because J initialises to false
                // "AND B T", // T = B && A
                // "AND C T", // T = C && B && A
                // "NOT T J", // J = !(A && B && C)
                // "AND D J", // J = D && !(A && B && C)
                // "WALK"
            // Part 2
                "OR A T",  // T = A -> because T initialises to false

                "OR E J",  // J = E
                "OR H J",  // J = H || E
                "AND D J", // J = D && (H || E)

                "AND B T", // T = B && A
                "AND C T", // T = C && B && A
                "NOT T T", // T = !(A && B && C)

                "AND T J", // J = !(A && B && C) && D && (H || E)
                "RUN"
            };


            // Part 1 "tests"
            // #####.###########  Jump if the square immediately in front is empty J = !A
            //     X
            // #####..#.########  Jump if the one of the immediate squares is empty, and the jump target exists J = !(A & B & C) & D
            //    X

            // Part 2
            // #####.#.#...#.###  Jump if D is available, and there's a gap between here and there, but not if the following jump target (H) is empty J = !(A & B & C) & D & H
            //     X   X   X
            // #####...####.####  J = !(A & B & C) & D & (H || E)
            //     X    XXX 
            // From previous attempts there are others, but apparently the above is good enough! :D

            var input = new Queue<long>();

            foreach(var line in inputProgram){
                foreach(char character in line){
                    input.Enqueue(character);
                }
                input.Enqueue((long)'\n');
            }

            computer.Calculate(input);

            while(computer.ReturnValues.Count() > 1){
                Console.Write((char)computer.ReturnValues.Dequeue());
            }

            Console.WriteLine();
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
