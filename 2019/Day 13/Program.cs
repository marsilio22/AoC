using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Day_13
{
    public enum Tiles{
        Empty = 0,
        Wall = 1,
        Block = 2,
        Paddle = 3,
        Ball = 4
    }

    class Program
    {
        static void Main(string[] args)
        {
            var program = File.ReadAllLines("./input.txt") [0].Split (",").Select (i => long.Parse (i)).ToList ();
            var computer = new IntcodeComputer(program.ToList());

            computer.Calculate(new Queue<long>());

            var state = new Dictionary<(int x, int y), int>();

            state = GetState(computer.ReturnValues);

            // Part 1
            var answer = state.Count(s => s.Value == (int)Tiles.Block);
            Console.WriteLine(answer);

            var playableProgram = program.ToList();
            playableProgram[0] = 2;

            computer = new IntcodeComputer(playableProgram);

            var input = new Queue<long>();

            bool halted = false;

            while(!halted){
                halted = computer.Calculate(input);
                var newState = GetState(computer.ReturnValues);

                foreach(var entry in newState){
                    state[entry.Key] = entry.Value;
                }

                var direction = state.Single(s => s.Value == (int)Tiles.Ball).Key.x - state.Single(s => s.Value == (int)Tiles.Paddle).Key.x;

                if (direction < 0){
                    input.Enqueue(-1);
                }
                else if (direction == 0){
                    input.Enqueue(0);
                }
                else {
                    input.Enqueue(1);
                }
                // Uncomment/comment for interest/speed
                Console.Clear();
                Draw(state);
                Thread.Sleep(50);
                Console.Clear();
            }

            Console.WriteLine(state[(-1, 0)]);

        }

        public static void Draw(Dictionary<(int x, int y), int> map){
            int maxX = map.Select(m => m.Key.x).Max();
            int maxY = map.Select(m => m.Key.y).Max();

            for (int i = 0; i <= maxY; i++)
            {
                for (int j = 0; j <= maxX; j++)
                {
                    if (map.TryGetValue((j, i), out int tile)){
                        switch(tile){
                            case 0:
                                Console.Write(' ');
                                break;
                            case 1:
                                Console.Write('■');
                                break;
                            case 2:
                                Console.Write('#');
                                break;
                            case 3:
                                Console.Write('-');
                                break;
                            case 4:
                                Console.Write('o');
                                break;
                        }
                    }
                }
                Console.WriteLine();
            }
        }

        public static Dictionary<(int x, int y), int> GetState(Queue<long> output){

            var state = new Dictionary<(int x, int y), int>();

            while (output.Any())
            {
                var x = output.Dequeue();
                var y = output.Dequeue();

                var tileId = (int)output.Dequeue();
            
                state[((int)x, (int)y)] = tileId;
            }

            return state;
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
