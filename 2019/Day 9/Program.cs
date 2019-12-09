using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_9 {
    class Program {
        static void Main (string[] args) {
            var program = File.ReadAllLines ("./input.txt") [0].Split (",").Select (i => long.Parse (i)).ToList ();

            var input = new Queue<long>();
            input.Enqueue(1);
            // Part 1
            var result = Calculate(program, input, 0);
            Console.WriteLine(result);

            // Part 2
            input.Enqueue(2);
            result = Calculate(program, input, 0);
            Console.WriteLine(result);
        }

        /// <summary>
        /// Calculates the output state of the given set of codes, for the given noun and verb
        /// </summary>
        /// <returns>
        /// A tuple containing: 
        /// 1. The return value, if any
        /// 2. A value indicating whether or not the process has halted (if not then it is awaiting more input)
        /// 3. The value of the current index of the running program, if appropriate
        /// </returns>
        public static (long returnValue, bool didHalt, int currentIndex) Calculate (List<long> intcode, Queue<long> input, int startingIndex) {
            long returnValue = 0;
            long relativeBase = 0;
            var tenThousand0s = new List<long>(new long[10000]);
            intcode.AddRange(tenThousand0s);

            for (int i = startingIndex;;) {
                var opcode = intcode[i].ToString ();
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
                    indexA = GetIndexForMode (modeA, intcode, relativeBase, i, 3);
                    indexB = GetIndexForMode (modeB, intcode, relativeBase, i, 2);
                    indexC = GetIndexForMode (modeC, intcode, relativeBase, i, 1);
                }
                if (twoParameterOpcodes.Contains(INTstruction)){
                    indexB = GetIndexForMode (modeB, intcode, relativeBase, i, 2);
                    indexC = GetIndexForMode (modeC, intcode, relativeBase, i, 1);
                }
                if (oneParameterOpcodes.Contains(INTstruction)){
                    indexC = GetIndexForMode (modeC, intcode, relativeBase, i, 1);
                }
                switch (INTstruction) {
                    case 1: // Add
                        intcode[(int)indexA] = intcode[(int)indexC] + intcode[(int)indexB];
                        i += 4;
                        break;
                    case 2: // Multiply
                        intcode[(int)indexA] = intcode[(int)indexC] * intcode[(int)indexB];
                        i += 4;
                        break;
                    case 3: // Input
                        if (!input.Any ()) {
                            return (returnValue, false, i);
                        }
                        
                        intcode[(int)indexC] = input.Dequeue ();
                        i += 2;
                        break;
                    case 4: // Output
                        returnValue = intcode[(int)indexC];
                        //Console.WriteLine(returnValue);
                        i += 2;
                        break;
                    case 5: // Jump if true
                        long test = intcode[(int)indexC];
                        i = test == 0 ? i + 3 : (int)intcode[(int)indexB];
                        break;
                    case 6: // Jump if false
                        test = intcode[(int)indexC];
                        i = test != 0 ? i + 3 : (int)intcode[(int)indexB];
                        break;
                    case 7: // Less Than
                        bool condition = intcode[(int)indexC] < intcode[(int)indexB];
                        // (modeB == 0 ? intcode[intcode[i + 2]] : intcode[i + 2]) >
                        // (modeC == 0 ? intcode[intcode[i + 1]] : intcode[i + 1]);
                        intcode[(int)indexA] = condition ? 1 : 0;
                        i += 4;
                        break;
                    case 8: // Equals
                        condition = intcode[(int)indexC] == intcode[(int)indexB];
                        // (modeB == 0 ? intcode[intcode[i + 2]] : intcode[i + 2]) ==
                        // (modeC == 0 ? intcode[intcode[i + 1]] : intcode[i + 1]);
                        intcode[(int)indexA] = condition ? 1 : 0;
                        i += 4;
                        break;
                    case 9: // Modify relative base
                        relativeBase += intcode[(int)indexC];
                        i += 2;
                        break;
                    case 99:
                        return (returnValue, true, 0);
                    default:
                        throw new ArgumentException ();
                }
            }
        }

        public static long GetIndexForMode (int mode, List<long> intcode, long relativeBase, int i, int indexIncrease) {
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
