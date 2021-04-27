using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_22
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("./input.txt");

            Dictionary<(int x, int y), char> map = new Dictionary<(int x, int y), char>();

            long iterations = 10_000;

            for(int i = 0; i < lines.Count(); i++)
            {
                var line = lines[i];
                for(int j = 0; j < line.Length; j++)
                {
                    map[(j, i)] = line[j];
                }
            }

            var pos = (map.Max(m => m.Key.x) / 2, map.Max(m => m.Key.y) / 2);
            var currentFacing = Facing.Up;

            var count = 0;

            for(int i = 0; i < iterations; i++)
            {
                var currentNode = map[pos];

                if (currentNode == '#')
                {
                    // turn right
                    currentFacing = Turn(currentFacing, Direction.Right);
                    // disinfect
                    map[pos] = '.';
                }
                else
                {
                    // turn left
                    currentFacing = Turn(currentFacing, Direction.Left);
                    // infect
                    map[pos] = '#';
                    count++;
                }

                // move forward
                pos = Move(pos, currentFacing);

                if (!map.ContainsKey(pos))
                {
                    map[pos] = '.';
                }
            }

            Console.WriteLine(count);

            // part 2
            map = new Dictionary<(int x, int y), char>();

            iterations = 10_000_000;

            for(int i = 0; i < lines.Count(); i++)
            {
                var line = lines[i];
                for(int j = 0; j < line.Length; j++)
                {
                    map[(j, i)] = line[j];
                }
            }

            pos = (map.Max(m => m.Key.x) / 2, map.Max(m => m.Key.y) / 2);
            currentFacing = Facing.Up;

            count = 0;

            for(int i = 0; i < iterations; i++)
            {
                var currentNode = map[pos];

                if (currentNode == '#')
                {
                    // turn right
                    currentFacing = Turn(currentFacing, Direction.Right);
                    // flag
                    map[pos] = 'F';
                }
                else if (currentNode == '.')
                {
                    // turn left
                    currentFacing = Turn(currentFacing, Direction.Left);
                    // weaken
                    map[pos] = 'W';
                }
                else if (currentNode == 'F')
                {
                    // turn around
                    currentFacing = Turn(Turn(currentFacing, Direction.Left), Direction.Left);
                    // clean
                    map[pos] = '.';
                }
                else if (currentNode == 'W')
                {
                    // don't turn, infect
                    map[pos] = '#';
                    count++;
                }

                // move forward
                pos = Move(pos, currentFacing);

                if (!map.ContainsKey(pos))
                {
                    map[pos] = '.';
                }
            }

            Console.WriteLine(count);
        }

        public static (int x, int y) Move((int x, int y) pos, Facing facing)
        {
            switch(facing)
            {
                case Facing.Up:
                    return (pos.x, pos.y - 1);
                case Facing.Left:
                    return (pos.x - 1, pos.y);
                case Facing.Down:
                    return (pos.x, pos.y + 1);
                case Facing.Right:
                    return (pos.x + 1, pos.y);
                default:
                    throw new Exception("oh noes");
            }
        }

        public static Facing Turn(Facing currentFacing, Direction direction)
        {
            switch(currentFacing)
            {
                case Facing.Up:
                        return direction == Direction.Left ? Facing.Left : Facing.Right;
                case Facing.Left:
                        return direction == Direction.Left ? Facing.Down : Facing.Up;
                case Facing.Down:
                        return direction == Direction.Left ? Facing.Right : Facing.Left;
                case Facing.Right:
                        return direction == Direction.Left ? Facing.Up : Facing.Down;
                default:
                    throw new Exception("oh noes");
            }
        }
    }

    public enum Direction
    {
        Left,
        Right
    }

    public enum Facing
    {
        Up,
        Left,
        Down,
        Right
    }
}
