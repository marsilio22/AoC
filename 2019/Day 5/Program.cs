using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_5 {
    class Program {
        static void Main (string[] args) {
            var input = File.ReadLines ("./input.txt").ToList () [0].Split (",").Select (i => int.Parse (i)).ToList ();
            //var input = "3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99".Split (",").Select (i => int.Parse (i)).ToList ();
            //Calculate (input.ToList(), 1); // Part 1

            Calculate(input.ToList(), 5); // Part 2
        }

        /// <summary>
        /// Calculates the output state of the given set of codes, for the given noun and verb
        /// </summary>
        /// <returns>The value at index 0, after the program has run</returns>
        public static void Calculate (List<int> intcode, int input) {
            for (int i = 0; intcode[i] != 99;) {
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

                switch (INTstruction) {
                    case 1: // Add
                        intcode[intcode[i + 3]] =
                            (modeB == 0 ? intcode[intcode[i + 2]] : intcode[i + 2]) +
                            (modeC == 0 ? intcode[intcode[i + 1]] : intcode[i + 1]);
                        i += 4;
                        break;
                    case 2: // Multiply
                        intcode[intcode[i + 3]] =
                            (modeB == 0 ? intcode[intcode[i + 2]] : intcode[i + 2]) *
                            (modeC == 0 ? intcode[intcode[i + 1]] : intcode[i + 1]);
                        i += 4;
                        break;
                    case 3: // Input
                        intcode[intcode[i + 1]] = input;
                        i += 2;
                        break;
                    case 4: // Output
                        Console.WriteLine(intcode[intcode[i + 1]]);
                        i += 2;
                        break;
                    case 5: // Jump if true
                        int test = modeC == 0 ? intcode[intcode[i+1]] : intcode[i+1];
                        i = test == 0 ? i + 3 : (modeB == 0 ? intcode[intcode[i+2]] : intcode[i+2]); 
                        break;
                    case 6: // Jump if false
                        test = modeC == 0 ? intcode[intcode[i+1]] : intcode[i+1];
                        i = test != 0 ? i + 3 : (modeB == 0 ? intcode[intcode[i+2]] : intcode[i+2]); 
                        break;
                    case 7: // Less Than
                        bool condition = (modeB == 0 ? intcode[intcode[i + 2]] : intcode[i + 2]) >
                            (modeC == 0 ? intcode[intcode[i + 1]] : intcode[i + 1]);
                        intcode[intcode[i+3]] = condition ? 1 : 0;
                        i += 4;
                        break;
                    case 8: // Equals
                        condition = (modeB == 0 ? intcode[intcode[i + 2]] : intcode[i + 2]) ==
                            (modeC == 0 ? intcode[intcode[i + 1]] : intcode[i + 1]);
                        intcode[intcode[i+3]] = condition ? 1 : 0;
                        i += 4;
                        break;
                    case 99:
                        Console.WriteLine("Found a 99");
                        return;
                    default:
                        throw new ArgumentException();
                }
            }
        }
    }
}
