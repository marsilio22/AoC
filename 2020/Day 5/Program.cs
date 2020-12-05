using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_5
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadLines("./input.txt");
            // var lines = new List<string>{"BFFFBBFRRR", "FFFBBBFRRR", "BBFFBBFRLL"};
            List<int> seats = new List<int>();

            foreach(var line in lines)
            {
                var rows = Enumerable.Range(0, 128).ToList();
                var cols = Enumerable.Range(0, 8).ToList();

                foreach(var character in line){
                    switch(character)
                    {
                        case 'F':
                            rows = rows.GetRange(0, rows.Count/2);
                            break;
                        case 'B':
                            rows = rows.GetRange(rows.Count/2, rows.Count/2);
                            break;
                        case 'L':
                            cols = cols.GetRange(0, cols.Count/2);
                            break;
                        case 'R':
                            cols = cols.GetRange(cols.Count/2, cols.Count/2);
                            break;
                    }
                }

                seats.Add(rows.Single() * 8 + cols.Single());
            }

            Console.WriteLine(seats.Max());

            seats.Sort();

            foreach(var seat in seats){
                if (seats.Contains(seat + 1) && seats.Contains(seat - 1)){
                    continue;
                }
                Console.WriteLine(seat);
            }
        }
    }
}
