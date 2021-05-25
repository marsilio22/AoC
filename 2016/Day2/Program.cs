using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Day2
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> lines = File.ReadAllLines("./input.txt").ToList();

            var part1 = new Dictionary<(int x, int y), char>()
            {
                {(0, 0), '1'},
                {(1, 0), '2'},
                {(2, 0), '3'},
                {(0, 1), '4'},
                {(1, 1), '5'},
                {(2, 1), '6'},
                {(0, 2), '7'},
                {(1, 2), '8'},
                {(2, 2), '9'}
            };

            var part2 = new Dictionary<(int x, int y), char>
            {
                {(2, 0), '1'},
            
                {(1, 1), '2'},
                {(2, 1), '3'},
                {(3, 1), '4'},

                {(0, 2), '5'},
                {(1, 2), '6'},
                {(2, 2), '7'},
                {(3, 2), '8'},
                {(4, 2), '9'},

                {(1, 3), 'A'},
                {(2, 3), 'B'},
                {(3, 3), 'C'},

                {(2, 4), 'D'},
            };


            foreach (var dict in new [] {part1, part2})
            {
                (int x, int y) pos = dict.Single(d => d.Value == '5').Key;
                foreach(var line in lines)
                {
                    foreach (var character in line)
                    {
                        switch(character)
                        {
                            case 'U':
                                var newPos = (pos.x, pos.y - 1);
                                pos = dict.ContainsKey(newPos) ? newPos : pos;
                                break;
                            case 'D':
                                newPos = (pos.x, pos.y + 1);
                                pos = dict.ContainsKey(newPos) ? newPos : pos;
                                break;
                            case 'L':
                                newPos = (pos.x - 1, pos.y);
                                pos = dict.ContainsKey(newPos) ? newPos : pos;
                                break;
                            case 'R':
                                newPos = (pos.x + 1, pos.y);
                                pos = dict.ContainsKey(newPos) ? newPos : pos;
                                break;
                            default:
                                throw new Exception("oh noes");
                        }
                    }

                    Console.Write(dict[pos]);
                }
                Console.WriteLine();
            }
        }
    }
}
