using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_11
{
    public enum Direction{
        Up,
        Down,
        Left,
        Right
    }

    class Robot
    {
        private Direction currentFacing = Direction.Up;
        private (int x, int y) position = (0, 0);
        private List<long> program;

        private IntcodeComputer computer;

        public Robot(List<long> program){
            this.program = program;

            this.computer = new IntcodeComputer(program);
        }

        private int GetColourOfCurrentPosition(IDictionary<(int x, int y), int> map){
            if (!map.ContainsKey(position)){
                map[position] = 0; // panels are black by default;
            }
            return map[position];
        }

        private void ChangeDirection(int instruction){
            // 0 is turn left, 1 is turn right
            switch(currentFacing) {
                case Direction.Up:
                    currentFacing = instruction == 0 ? Direction.Left : Direction.Right;
                    break;
                case Direction.Down:
                    currentFacing = instruction == 0 ? Direction.Right : Direction.Left;
                    break;
                case Direction.Left:
                    currentFacing = instruction == 0 ? Direction.Down : Direction.Up;
                    break;
                case Direction.Right:
                    currentFacing = instruction == 0 ? Direction.Up : Direction.Down;
                    break;
            }
        }

        private void Move(){
            switch(currentFacing){
                case Direction.Up:
                    position = (position.x, position.y + 1);
                    break;
                case Direction.Down:
                    position = (position.x, position.y - 1);
                    break;
                case Direction.Left:
                    position = (position.x - 1, position.y);
                    break;
                case Direction.Right:
                    position = (position.x + 1, position.y);
                    break;
            }
        }

        public void Run(IDictionary<(int x, int y), int> map){
            var lastDictionarySize = int.MinValue;
            var numberOfTimesDictionaryHasntChangedSize = 0;
            bool hasHalted = false;

            while(numberOfTimesDictionaryHasntChangedSize < 30000 && !hasHalted){
                if (map.Count() == lastDictionarySize){
                    numberOfTimesDictionaryHasntChangedSize ++;
                }
                lastDictionarySize = map.Count();

                var currentPanelColour = this.GetColourOfCurrentPosition(map);

                var input = new Queue<long>();
                input.Enqueue((long)currentPanelColour);

                hasHalted = this.computer.Calculate(input);
                
                var colour = this.computer.ReturnValues.Dequeue();
                var turn = this.computer.ReturnValues.Dequeue();

                map[position] = (int)colour;
                this.ChangeDirection((int)turn);
                this.Move();
                
                // Printer.PrintMap(map);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var program = File.ReadAllLines("./input.txt") [0].Split (",").Select (i => long.Parse (i)).ToList ();

            var robot = new Robot(program.ToList());

            var map = new Dictionary<(int x, int y), int>();

            // Part 1
            robot.Run(map);
            Console.WriteLine(map.Count);

            // Part 2
            var part2Map = new Dictionary<(int x, int y), int>();
            part2Map[(0, 0)] = 1;

            robot = new Robot(program.ToList());
            robot.Run(part2Map);

            Printer.PrintMap(part2Map);
        }
    }

    static class Printer{
        public static void PrintMap (IDictionary<(int x, int y), int> map){
            var maxX = map.Select(m => m.Key.x).OrderByDescending(x => x).First();
            var minX = map.Select(m => m.Key.x).OrderBy(x => x).First();
            var maxY = map.Select(m => m.Key.y).OrderByDescending(y => y).First();
            var minY = map.Select(m => m.Key.y).OrderBy(y => y).First();

            for (int i = maxY; i >= minY; i--)
            {
                for (int j = minX; j <= maxX; j++){
                    if (!map.ContainsKey((j, i)) || map[(j, i)] == 0){
                        Console.Write("  ");
                    }
                    else if (map[(j, i)] == 1){
                        Console.Write("■■");
                    }
                }
                Console.WriteLine();
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
