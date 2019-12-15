using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_15
{
    public enum Directions {
        North = 1,
        South = 2,
        West = 3,
        East = 4
    }

    class Program
    {
        static void Main(string[] args)
        {
            var program = File.ReadAllLines("./input.txt") [0].Split (",").Select (i => long.Parse (i)).ToList ();
            var computer = new IntcodeComputer(program.ToList());
            var droid = new Droid(computer);

            Dictionary<(int x, int y), char> map = new Dictionary<(int x, int y), char>();
            map.Add((0, 0), '.');

            var dir = Directions.North;
            while (true){ //map.All(m => m.Value != 'O')){
                var result = droid.Move(dir);

                switch(result){
                    case 0:
                        map.TryAdd(GetTargetedCoord(droid.Position, dir), '#');
                        break;
                    case 1:
                        droid.Position = GetTargetedCoord(droid.Position, dir);
                        map.TryAdd(droid.Position, '.');                        
                        break;
                    case 2:
                        droid.Position = GetTargetedCoord(droid.Position, dir);
                        map.Add(droid.Position, 'O');
                        break;
                }

                var peekNorth = droid.MoveWithoutMoving(Directions.North);
                var peekSouth = droid.MoveWithoutMoving(Directions.South);
                Console.WriteLine(peekSouth);
                Console.WriteLine(GetTargetedCoord(droid.Position, Directions.South));
                Console.WriteLine(GetCharForResult(1));
                var peekEast = droid.MoveWithoutMoving(Directions.East);
                var peekWest = droid.MoveWithoutMoving(Directions.West);
                map.TryAdd(GetTargetedCoord(droid.Position, Directions.North), GetCharForResult(peekNorth));
                map.TryAdd(GetTargetedCoord(droid.Position, Directions.South), GetCharForResult(peekSouth));
                map.TryAdd(GetTargetedCoord(droid.Position, Directions.West), GetCharForResult(peekWest));
                map.TryAdd(GetTargetedCoord(droid.Position, Directions.East), GetCharForResult(peekEast));

                DrawMap(map, droid.Position);
                var thing = Console.ReadKey();
                dir =  KeyPressToDirection(thing);//ChooseNewDirection(dir, map, droid.Position); 

            }
        }

        public static char GetCharForResult(int result){
            switch(result){
                case 0:
                    return '#';
                case 1:
                    return '.';
                case 2:
                    return 'O';
            }
            return ' ';
        }

        public static Directions KeyPressToDirection(ConsoleKeyInfo keyInfo){
            switch(keyInfo.Key){
                case ConsoleKey.UpArrow:
                    return Directions.North;
                case ConsoleKey.DownArrow:
                    return Directions.South;
                case ConsoleKey.LeftArrow:
                    return Directions.West;
                case ConsoleKey.RightArrow:
                    return Directions.East;
            }
            throw new KeyNotFoundException();
        }

        public static Directions ChooseNewDirection(Directions currentDirection, Dictionary<(int x, int y), char> map, (int x, int y) position){
            var targetCoord = GetTargetedCoord(position, currentDirection);
            switch(map[targetCoord]){
                case '.':
                    // turn left?
                    // N => E => S => W
                    switch(currentDirection){
                        case Directions.North:
                            return Directions.East;
                        case Directions.South:
                            return Directions.West;
                        case Directions.East:
                            return Directions.South;
                        case Directions.West:
                            return Directions.North;
                    }
                    break;
                case '#':
                    // turn right?
                    // N => W => S => E
                    switch(currentDirection){
                        case Directions.North:
                            return Directions.West;
                        case Directions.South:
                            return Directions.East;
                        case Directions.East:
                            return Directions.North;
                        case Directions.West:
                            return Directions.South;
                    }
                    break;
                default:
                    throw new KeyNotFoundException();
            }
            throw new KeyNotFoundException();
        }

        public static void DrawMap(Dictionary<(int x, int y), char> map, (int x, int y) currentPosition){
            int minX = map.Keys.Select(k => k.x).Min();
            int minY= map.Keys.Select(k => k.y).Min();
            int maxX= map.Keys.Select(k => k.x).Max();
            int maxY= map.Keys.Select(k => k.y).Max();

            for (int j = minY; j <= maxY; j++)
            {
                for (int i = minX; i <= maxX; i++){
                    if (i == currentPosition.x && j == currentPosition.y){
                        Console.Write('D');
                    }
                    else if (map.TryGetValue((i, j), out char value)){
                        Console.Write(value);
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }
                Console.WriteLine();
            }

        }

        public static (int x, int y) GetTargetedCoord ((int x, int y) startingPosition, Directions direction){
            switch (direction){
                case Directions.North:
                    return (startingPosition.x, startingPosition.y - 1);
                case Directions.South:
                    return (startingPosition.x, startingPosition.y + 1);
                case Directions.East:
                    return (startingPosition.x + 1, startingPosition.y);
                case Directions.West:
                    return (startingPosition.x - 1, startingPosition.y);
                default:
                    throw new KeyNotFoundException();
            }
        }
    }

    public class Droid{
        public (int x, int y) Position { get; set; }
        public IntcodeComputer Computer { get; set; }

        public Droid(IntcodeComputer computer){
            this.Computer = computer;
            this.Position = (0, 0);
        }

        public int Move(Directions direction){
            Queue<long> input = new Queue<long>();
            input.Enqueue((long)direction);

            this.Computer.Calculate(input);

            var output = (int)Computer.ReturnValues.Dequeue();

            // if (output == 0){
            //     Console.WriteLine($"Hit a wall heading {direction}");
            // }
            // else if (output == 1){
            //     Console.WriteLine($"Moved {direction}");
            // }
            // else {
            //     Console.WriteLine($"Found the oxygen thing heading {direction}");
            // }

            return output;
        }

        public int MoveWithoutMoving(Directions directions){
            Queue<long> input = new Queue<long>();
            input.Enqueue((long)directions);
            this.Computer.Calculate(input);

            var output = (int)Computer.ReturnValues.Dequeue();

            if (output == 0){
                return output;
            }
            else //if (output == 1 || output == 2)
            {
                // we've moved, so move back
                Directions oppositeDirection = Directions.North;
                switch(directions){
                    case Directions.North:
                        oppositeDirection = Directions.South;
                        break;
                    case Directions.South:
                        oppositeDirection = Directions.North;
                        break;
                    case Directions.East:
                        oppositeDirection = Directions.West;
                        break;
                    case Directions.West:
                        oppositeDirection = Directions.East;
                        break;
                    default:
                        throw new KeyNotFoundException();
                }

                input.Enqueue((long)oppositeDirection);
                Computer.Calculate(input);

                _ = Computer.ReturnValues.Dequeue();
                return output;
            }
        }
    }

    
    public class IntcodeComputer{
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
