using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_7 {
    class Program {
        static void Main (string[] args) {
            var program = File.ReadAllLines ("./input.txt") [0].Split (",").Select (i => int.Parse (i)).ToList ();

            // Part 1
            //var program = "3,15,3,16,1002,16,10,16,1,16,15,15,4,15,99,0,0".Split (",").Select (i => int.Parse (i)).ToList();
            Part1 (program, new List<int> { 0, 1, 2, 3, 4 });

            // Part 2
            // solution permutation 98765 = 139629729
            //program = "3,26,1001,26,-4,26,3,27,1002,27,2,27,1,27,26,27,4,27,1001,28,-1,28,1005,28,6,99,0,0,5".Split (",").Select (i => int.Parse (i)).ToList();

            // solution permutation 97856 = 18216
            //program = "3,52,1001,52,-5,52,3,53,1,52,56,54,1007,54,5,55,1005,55,26,1001,54,-5,54,1105,1,12,1,53,54,53,1008,54,0,55,1001,55,1,55,2,53,55,53,4,53,1001,56,-1,56,1005,56,6,99,0,0,0,0,10".Split (",").Select (i => int.Parse (i)).ToList();

            Part2 (program, new List<int> { 5, 6, 7, 8, 9 });
        }

        /// <summary>
        /// Calculate part 1
        /// </summary>
        /// <param name="program">The program</param>
        /// <param name="phases">The valid phases to permute through</param>
        public static void Part1 (ICollection<int> program, List<int> phases) {
            var permutations = GetPermutations (phases, 5).ToList ();
            var nextInput = 0;
            var largestOutput = int.MinValue;

            // Find which permutation produces the largest output 
            foreach (var phasePermutation in permutations) {
                nextInput = 0;
                foreach (var amplifier in phasePermutation) {
                    // queue input, to be dequeued by the input commands
                    // First param is the amplifier phase, second is the output from 
                    // the previous amplifier, or 0 if this is the first amplifier.
                    var input = new Queue<int> ();
                    input.Enqueue (amplifier);
                    input.Enqueue (nextInput);

                    nextInput = Calculate (program.ToList (), input, 0).returnValue;
                }

                largestOutput = largestOutput < nextInput ? nextInput : largestOutput;
            }

            Console.WriteLine (largestOutput);
        }

        /// <summary>
        /// Calculate part 2
        /// </summary>
        /// <param name="program">The program</param>
        /// <param name="phases">The valid phases to permute through</param>
        public static void Part2 (ICollection<int> program, List<int> phases) {
            var permutations = GetPermutations (phases, 5).ToList ();

            var nextInput = 0;
            var largestOutput = int.MinValue;

            // This time there is a feedback loop, Amplifier E inputs to Amplifier A, until all have halted.
            foreach (var phasePermutation in permutations) {
                // Programs are not restarted, so keep track of their individual states, and current index
                var amplifierPrograms = new Dictionary < int,
                    (List<int> ampProgram, int? currentIndex) > { { 0, (program.ToList (), null) }, // amp A
                        { 1, (program.ToList (), null) },
                        { 2, (program.ToList (), null) },
                        { 3, (program.ToList (), null) },
                        { 4, (program.ToList (), null) } // amp E
                    };

                nextInput = 0;
                int i = 0;

                while (true) {
                    var input = new Queue<int> ();
                    // We only want to add the Phase to the input queue if this is the first time the program has been run
                    if (amplifierPrograms[i].currentIndex == null) {
                        input.Enqueue (phasePermutation.ToList () [i]);
                        var prog = amplifierPrograms[i];
                        prog.currentIndex = 0;

                        amplifierPrograms[i] = prog;
                    }
                    // We always want to input the next value (initialised to 0 for the first run)
                    input.Enqueue (nextInput);

                    var calculationResult = Calculate (amplifierPrograms[i].ampProgram, input, amplifierPrograms[i].currentIndex.Value);
                    nextInput = calculationResult.returnValue;

                    if (calculationResult.didHalt && i == 4) {
                        // Stop if this is Amplifier E (index 4) and the calculation halted (rather than awaiting new input)
                        break;
                    } else {
                        // otherwise, update the current index of this amplifier, and iterate i to the next amplifier
                        var ampProg = amplifierPrograms[i];
                        amplifierPrograms[i] = (ampProg.ampProgram, calculationResult.currentIndex);
                        i = (i + 1) % amplifierPrograms.Count;
                    }
                }

                // Keep track of the largest output
                largestOutput = largestOutput < nextInput ? nextInput : largestOutput;
            }

            Console.WriteLine (largestOutput);
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
        public static (int returnValue, bool didHalt, int currentIndex) Calculate (List<int> intcode, Queue<int> input, int startingIndex) {
            var returnValue = 0;
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
                        if (!input.Any ()) {
                            return (returnValue, false, i);
                        }
                        intcode[intcode[i + 1]] = input.Dequeue ();
                        i += 2;
                        break;
                    case 4: // Output
                        // For this day, don't actually return at this point, because we need to carry on
                        // until we reach either a halt, or a 99.
                        returnValue = intcode[intcode[i + 1]];
                        i += 2;
                        break;
                    case 5: // Jump if true
                        int test = modeC == 0 ? intcode[intcode[i + 1]] : intcode[i + 1];
                        i = test == 0 ? i + 3 : (modeB == 0 ? intcode[intcode[i + 2]] : intcode[i + 2]);
                        break;
                    case 6: // Jump if false
                        test = modeC == 0 ? intcode[intcode[i + 1]] : intcode[i + 1];
                        i = test != 0 ? i + 3 : (modeB == 0 ? intcode[intcode[i + 2]] : intcode[i + 2]);
                        break;
                    case 7: // Less Than
                        bool condition = (modeB == 0 ? intcode[intcode[i + 2]] : intcode[i + 2]) >
                            (modeC == 0 ? intcode[intcode[i + 1]] : intcode[i + 1]);
                        intcode[intcode[i + 3]] = condition ? 1 : 0;
                        i += 4;
                        break;
                    case 8: // Equals
                        condition = (modeB == 0 ? intcode[intcode[i + 2]] : intcode[i + 2]) ==
                            (modeC == 0 ? intcode[intcode[i + 1]] : intcode[i + 1]);
                        intcode[intcode[i + 3]] = condition ? 1 : 0;
                        i += 4;
                        break;
                    case 99:
                        return (returnValue, true, 0);
                    default:
                        throw new ArgumentException ();
                }
            }
        }

        static IEnumerable<IEnumerable<T>> GetPermutations<T> (IEnumerable<T> list, int length) {
            if (length == 1) return list.Select (t => new T[] { t });

            return GetPermutations (list, length - 1)
                .SelectMany (t => list.Where (e => !t.Contains (e)),
                    (t1, t2) => t1.Concat (new T[] { t2 }));
        }
    }
}