using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_25 {
    class Program {
        static void Main (string[] args) {
            var program = File.ReadAllLines ("./input.txt") [0].Split (",").Select (i => long.Parse (i)).ToList ();

            var computer = new IntcodeComputer (program.ToList());
            computer.Calculate (new Queue<long> ());

            string response = "";
            while (computer.ReturnValues.Count () > 0) {
                response += (char) computer.ReturnValues.Dequeue ();
            }
            var validDirections = response.Split("Doors here lead:")[1].Split("Items here:")[0].Split("\n").Select(s => s.Split("- ")).Where(s => s.Count() > 1).Select(s => s[1]).ToList();

            var directions = new [] { "north", "south", "east", "west" };

            var map = new Dictionary <string, int> ();
            var items = new List<string>();

            int attempt = 0;

            var itemsTried = new Dictionary<int, List<string>>();
            Random random = new Random();

            while (true) {
                string command = "";
                if (map.ContainsKey(" Security Checkpoint ")){
                    while(itemsTried.Any(i => ListEquivalence(items, i.Value)) && items.Any()){
                        var itemToDrop = items[random.Next(items.Count)];
                        command = $"drop {itemToDrop}";
                        items.Remove(itemToDrop);
                        computer.Calculate(StringToInput(command));
                    }

                    command = "north";
                }
                else {
                    command = validDirections.ToList()[random.Next(validDirections.Count())];
                }

                var input = StringToInput (command);
                computer.Calculate (input);

                response = "";
                while (computer.ReturnValues.Count () > 0) {
                    response += (char) computer.ReturnValues.Dequeue ();
                }
                Console.WriteLine(response);

                var room = response.Split("==")[1];
                map.TryAdd(room, 0);
                map[room] = map[room] + 1;

                if (room.Contains("Pressure-Sensitive Floor")){
                    if (response.Contains("Alert!")){
                        computer = new IntcodeComputer(program.ToList());
                        computer.Calculate(new Queue<long>());

                        itemsTried.Add(attempt, items.ToList());
                        items = new List<string>();
                        attempt++;

                        map = new Dictionary <string, int> ();
                        response = "";
                        while (computer.ReturnValues.Count () > 0) {
                            response += (char) computer.ReturnValues.Dequeue ();
                        }
                    }
                    else{
                        break;
                    }
                }

                var itemsOnFloor = response.Contains("Items here:") ?
                    response.Split("Items here:")[1].Split("\n").Select(s => s.Split("- ")).Where(s => s.Count() > 1).Select(s => s[1]).ToList() :
                    new List<string>();
                
                var badItems = new [] {"molten lava", "giant electromagnet", "infinite loop", "photons", "escape pod"};
                foreach(var item in itemsOnFloor){
                    if (!badItems.Contains(item)){
                        command = $"take {item}";
                        computer.Calculate(StringToInput(command));
                        items.Add(item);
                    }
                }

                validDirections = response.Split("Doors here lead:")[1].Split("Items here:")[0].Split("\n").Select(s => s.Split("- ")).Where(s => s.Count() > 1).Select(s => s[1]).ToList();
            }


            // molten lava -- too hot
            // giant electromagnet -- can't move
            // infinite loop -- erm...
            // photons -- too dark
        }

        static bool ListEquivalence(List<string> A, List<string> B){
            if (A.Count != B.Count){
                return false;
            }
            foreach(var a in A){
                if (!B.Contains(a)){
                    return false;
                }
            }
            return true;
        }

        static void DrawMap (Dictionary < (int x, int y), int > map) {
            var minX = map.Select (m => m.Key.x).Min ();
            var minY = map.Select (m => m.Key.y).Min ();
            var maxX = map.Select (m => m.Key.x).Max ();
            var maxY = map.Select (m => m.Key.y).Max ();

            for (int y = minY; y <= maxY; y++) {
                for (int x = minX; x <= maxX; x++) {
                    if (map.ContainsKey ((x, y))) {
                        if (map[(x, y)] == 0) {
                            Console.Write ('.');
                        } else if (map[(x, y)] == 1) {
                            Console.Write ('X');
                        }
                        else{
                            Console.Write(' ');
                        }
                    }
                    else {
                        Console.Write(' ');
                    }
                }
                Console.WriteLine ();
            }
            Console.WriteLine();
        }

        static Queue<long> StringToInput (string input) {
            var queueInput = new Queue<long> ();
            foreach (var character in input) {
                queueInput.Enqueue (character);
            }
            queueInput.Enqueue ((long)
                '\n');

            return queueInput;
        }

    }

    class IntcodeComputer {
        private int currentIndex;
        private long relativeBase;
        private List<long> program;

        public Queue<long> ReturnValues { get; }

        public IntcodeComputer (List<long> program) {
            var tenThousand0s = new List<long> (new long[10000]);

            this.program = program;
            this.program.AddRange (tenThousand0s);

            // set defaults
            this.currentIndex = 0;
            this.relativeBase = 0;
            this.ReturnValues = new Queue<long> ();
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
                var oneParameterOpcodes = new [] { 3, 4, 9 };
                var twoParameterOpcodes = new [] { 5, 6 };
                var threeParameterOpcodes = new [] { 1, 2, 7, 8 };

                if (threeParameterOpcodes.Contains (INTstruction)) {
                    indexA = GetIndexForMode (modeA, program, relativeBase, i, 3);
                    indexB = GetIndexForMode (modeB, program, relativeBase, i, 2);
                    indexC = GetIndexForMode (modeC, program, relativeBase, i, 1);
                }
                if (twoParameterOpcodes.Contains (INTstruction)) {
                    indexB = GetIndexForMode (modeB, program, relativeBase, i, 2);
                    indexC = GetIndexForMode (modeC, program, relativeBase, i, 1);
                }
                if (oneParameterOpcodes.Contains (INTstruction)) {
                    indexC = GetIndexForMode (modeC, program, relativeBase, i, 1);
                }
                switch (INTstruction) {
                    case 1: // Add
                        program[(int) indexA] = program[(int) indexC] + program[(int) indexB];
                        i += 4;
                        break;
                    case 2: // Multiply
                        program[(int) indexA] = program[(int) indexC] * program[(int) indexB];
                        i += 4;
                        break;
                    case 3: // Input
                        if (!input.Any ()) {
                            this.currentIndex = i;
                            return false;
                        }

                        program[(int) indexC] = input.Dequeue ();
                        i += 2;
                        break;
                    case 4: // Output
                        ReturnValues.Enqueue (program[(int) indexC]);
                        i += 2;
                        break;
                    case 5: // Jump if true
                        long test = program[(int) indexC];
                        i = test == 0 ? i + 3 : (int) program[(int) indexB];
                        break;
                    case 6: // Jump if false
                        test = program[(int) indexC];
                        i = test != 0 ? i + 3 : (int) program[(int) indexB];
                        break;
                    case 7: // Less Than
                        bool condition = program[(int) indexC] < program[(int) indexB];
                        program[(int) indexA] = condition ? 1 : 0;
                        i += 4;
                        break;
                    case 8: // Equals
                        condition = program[(int) indexC] == program[(int) indexB];
                        program[(int) indexA] = condition ? 1 : 0;
                        i += 4;
                        break;
                    case 9: // Modify relative base
                        relativeBase += program[(int) indexC];
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
                return (int) intcode[i + indexIncrease];
            } else if (mode == 1) {
                return i + indexIncrease;
            } else if (mode == 2) {
                return (int) (relativeBase + intcode[i + indexIncrease]);
            } else {
                throw new KeyNotFoundException ();
            }
        }
    }
}