using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_2 {
    class Program {
        static void Main (string[] args) {
            var line = File.ReadLines ("./input.txt").First ();

            // int code program
            var intcodeProgram = line.Split (',').Select (i => int.Parse (i)).ToList();

            // part 1 - recreate 1202 error
            Console.WriteLine (Calculate (intcodeProgram.ToList(), 12, 2));

            // part 2 - find given input that produces outputs
            int soughtValue = 19690720; // puzzle input

            // Try noun and verb values between 0 and 100 until we have the desired value
            for (int noun = 0; noun < 100; noun++) {
                for (int verb = 0; verb < 100; verb++) {
                    try {
                        var answer = Calculate(intcodeProgram.ToList(), noun, verb);

                        if (answer == soughtValue) 
                        {
                            Console.WriteLine ($"{noun}, {verb}");
                        }
                    } catch (Exception e){
                        Console.WriteLine($"{noun}, {verb} caused an exception {e.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the output state of the given set of codes, for the given noun and verb
        /// </summary>
        /// <param name="intcode">The intcode program</param>
        /// <param name="noun">The noun of the program, replaces index 1</param>
        /// <param name="verb">The verb of the program, replaces index 2</param>
        /// <returns>The value at index 0, after the program has run</returns>
        public static int Calculate (List<int> intcode, int noun, int verb) {
            intcode[1] = noun;
            intcode[2] = verb;

            for (int i = 0; intcode[i] != 99; i += 4) {
                switch (intcode[i]) {
                    case 1:
                        intcode[intcode[i + 3]] = intcode[intcode[i + 2]] + intcode[intcode[i + 1]];
                        break;
                    case 2:
                        intcode[intcode[i + 3]] = intcode[intcode[i + 2]] * intcode[intcode[i + 1]];
                        break;
                    case 99:
                        break;
                }
            }

            return intcode[0];
        }
    }
}