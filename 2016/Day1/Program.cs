using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Day1
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> lines = File.ReadAllLines("./input.txt").ToList();

            var line = lines[0];
            var instructions = line.Split(", ");
            ICollection<(int x, int y)> locations = new List<(int, int)>();

            (int x, int y) pos = (0, 0);
            var facing = Facings.North;

            locations.Add(pos);

            foreach(var instruction in instructions)
            {
                var numeric = int.Parse(instruction.Substring(1));
                switch(facing)
                {
                    case Facings.North:
                        if( instruction[0] == 'L')
                        {
                            facing = Facings.West;
                            for(int i = 1; i <= numeric; i++)
                            {
                                var pos2 = (pos.x - i, pos.y);
                                if (locations.Contains(pos2))
                                {
                                    Console.WriteLine($"Visited {pos.x}, {pos.y} twice, total: {Math.Abs(pos.x) + Math.Abs(pos.y)}");
                                }
                                locations.Add(pos2);
                            }
                            pos = (pos.x - numeric, pos.y);
                        }
                        else
                        {
                            facing = Facings.East;
                            for(int i = 1; i <= numeric; i++)
                            {
                                var pos2 = (pos.x + i, pos.y);
                                if (locations.Contains(pos2))
                                {
                                    Console.WriteLine($"Visited {pos.x}, {pos.y} twice, total: {Math.Abs(pos.x) + Math.Abs(pos.y)}");
                                }
                                locations.Add(pos2);
                            }
                            pos = (pos.x + numeric, pos.y);
                        }
                        break;
                    case Facings.East:
                        if (instruction[0] == 'L')
                        {
                            facing = Facings.North;
                            for(int i = 1; i <= numeric; i++)
                            {
                                var pos2 = (pos.x, pos.y + i);
                                if (locations.Contains(pos2))
                                {
                                    Console.WriteLine($"Visited {pos.x}, {pos.y} twice, total: {Math.Abs(pos.x) + Math.Abs(pos.y)}");
                                }
                                locations.Add(pos2);
                            }
                            pos = (pos.x, pos.y + numeric);
                        }
                        else
                        {
                            facing = Facings.South;
                            for(int i = 1; i <= numeric; i++)
                            {
                                var pos2 = (pos.x, pos.y - i);
                                if (locations.Contains(pos2))
                                {
                                    Console.WriteLine($"Visited {pos.x}, {pos.y} twice, total: {Math.Abs(pos.x) + Math.Abs(pos.y)}");
                                }
                                locations.Add(pos2);
                            }
                            pos = (pos.x, pos.y - numeric);
                        }
                        break;                    
                    case Facings.South:
                        if( instruction[0] == 'L')
                        {
                            facing = Facings.East;
                            for(int i = 1; i <= numeric; i++)
                            {
                                var pos2 = (pos.x + i, pos.y);
                                if (locations.Contains(pos2))
                                {
                                    Console.WriteLine($"Visited {pos.x}, {pos.y} twice, total: {Math.Abs(pos.x) + Math.Abs(pos.y)}");
                                }
                                locations.Add(pos2);
                            }
                            pos = (pos.x + numeric, pos.y);
                        }
                        else
                        {
                            facing = Facings.West;
                            for(int i = 1; i <= numeric; i++)
                            {
                                var pos2 = (pos.x - i, pos.y);
                                if (locations.Contains(pos2))
                                {
                                    Console.WriteLine($"Visited {pos.x}, {pos.y} twice, total: {Math.Abs(pos.x) + Math.Abs(pos.y)}");
                                }
                                locations.Add(pos2);
                            }
                            pos = (pos.x - numeric, pos.y);
                        }
                        break;                    
                    case Facings.West:
                        if (instruction[0] == 'L')
                        {
                            facing = Facings.South;
                            for(int i = 1; i <= numeric; i++)
                            {
                                var pos2 = (pos.x, pos.y - i);
                                if (locations.Contains(pos2))
                                {
                                    Console.WriteLine($"Visited {pos.x}, {pos.y} twice, total: {Math.Abs(pos.x) + Math.Abs(pos.y)}");
                                }
                                locations.Add(pos2);
                            }
                            pos = (pos.x, pos.y - numeric);
                        }
                        else
                        {
                            facing = Facings.North;
                            for(int i = 1; i <= numeric; i++)
                            {
                                var pos2 = (pos.x, pos.y + i);
                                if (locations.Contains(pos2))
                                {
                                    Console.WriteLine($"Visited {pos.x}, {pos.y} twice, total: {Math.Abs(pos.x) + Math.Abs(pos.y)}");
                                }
                                locations.Add(pos2);
                            }
                            pos = (pos.x, pos.y + numeric);
                        }
                        break;
                    default:
                    throw new Exception("oh noes");
                }

                IDictionary<(int x, int y), char> thing = locations.ToDictionary(l => l, l => '#');
                DrawDict(thing);
            }

            Console.WriteLine($"Finished at {pos.x}, {pos.y}, total: {Math.Abs(pos.x) + Math.Abs(pos.y)}");

        }

        public static void DrawDict(IDictionary<(int x, int y), char> map)
        {
            var maxX = map.Select(m => m.Key.x).Max();
            var minX = map.Select(m => m.Key.x).Min();
            var maxY = map.Select(m => m.Key.y).Max();
            var minY = map.Select(m => m.Key.y).Min();

            for(int j = minY; j <= maxY; j ++)
            {
                for (int i = minX; i <= maxX; i++)
                {
                    var characters = map.ContainsKey((i, j)) ? "##" : "  ";
                    Console.Write(characters);
                }

                Console.WriteLine();
            }
        }
    }

    public enum Facings
    {
        North,
        East,
        South,
        West
    }
}
