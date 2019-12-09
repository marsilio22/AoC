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


                long parameterA = 0, parameterB = 0, parameterC = 0;
                // Check if the opcode takes 0, 1, 2, or 3 arguments, and initialise the appripriate mode values
                // parameterA = GetParameterForMode (modeA, intcode, relativeBase, i, 3);
                var oneParameterOpcodes = new []{9};
                var twoParameterOpcodes = new []{1, 2, 5, 6, 7, 8};
                if (twoParameterOpcodes.Contains(INTstruction)){
                    parameterB = GetParameterForMode (modeB, intcode, relativeBase, i, 2);
                    parameterC = GetParameterForMode (modeC, intcode, relativeBase, i, 1);
                }
                if (oneParameterOpcodes.Contains(INTstruction)){
                    parameterC = GetParameterForMode (modeC, intcode, relativeBase, i, 1);
                }
                switch (INTstruction) {
                    case 1: // Add
                        intcode[(int)intcode[i + 3]] = parameterC + parameterB;
                        // (modeB == 0 ? intcode[intcode[i + 2]] : intcode[i + 2]) +
                        // (modeC == 0 ? intcode[intcode[i + 1]] : intcode[i + 1]);
                        i += 4;
                        break;
                    case 2: // Multiply
                        intcode[(int)intcode[i + 3]] = parameterC * parameterB;
                        // (modeB == 0 ? intcode[intcode[i + 2]] : intcode[i + 2]) *
                        // (modeC == 0 ? intcode[intcode[i + 1]] : intcode[i + 1]);
                        i += 4;
                        break;
                    case 3: // Input
                        if (!input.Any ()) {
                            return (returnValue, false, i);
                        }
                        var index = intcode[i+1];
                        // WHYYYYYYYYY
                        if (modeC == 2){
                            index = relativeBase + index;
                        }
                        intcode[(int)index] = input.Dequeue ();
                        i += 2;
                        break;
                    case 4: // Output
                        // For this day, don't actually return at this point, because we need to carry on
                        // until we reach either a halt, or a 99.
                        returnValue = intcode[(int)intcode[i + 1]];
                        Console.WriteLine(returnValue);
                        i += 2;
                        break;
                    case 5: // Jump if true
                        long test = parameterC;
                        i = test == 0 ? i + 3 : (int)parameterB;
                        break;
                    case 6: // Jump if false
                        test = parameterC;
                        i = test != 0 ? i + 3 : (int)parameterB;
                        break;
                    case 7: // Less Than
                        bool condition = parameterC < parameterB;
                        // (modeB == 0 ? intcode[intcode[i + 2]] : intcode[i + 2]) >
                        // (modeC == 0 ? intcode[intcode[i + 1]] : intcode[i + 1]);
                        intcode[(int)intcode[i + 3]] = condition ? 1 : 0;
                        i += 4;
                        break;
                    case 8: // Equals
                        condition = parameterC == parameterB;
                        // (modeB == 0 ? intcode[intcode[i + 2]] : intcode[i + 2]) ==
                        // (modeC == 0 ? intcode[intcode[i + 1]] : intcode[i + 1]);
                        intcode[(int)intcode[i + 3]] = condition ? 1 : 0;
                        i += 4;
                        break;
                    case 9: // Modify relative base
                        relativeBase = parameterC;
                        i += 2;
                        break;
                    case 99:
                        return (returnValue, true, 0);
                    default:
                        throw new ArgumentException ();
                }
            }
        }

        public static long GetParameterForMode (int mode, List<long> intcode, long relativeBase, int i, int indexIncrease) {
            if (mode == 0) {
                return intcode[(int)intcode[i + indexIncrease]];
            } else if (mode == 1) {
                return intcode[i + indexIncrease];
            } else if (mode == 2) {
                return intcode[(int)(relativeBase + intcode[i + indexIncrease])];
            } else {
                throw new KeyNotFoundException ();
            }
        }
    }
}