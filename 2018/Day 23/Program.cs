using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Xml.Linq;

namespace Day_21
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("./input.txt");

            List<Coordinate> coords = new List<Coordinate>();

            foreach (var line in lines)
            {
                var xyz = line.Split('>')[0].Split('<')[1].Split(',').Select(long.Parse).ToList();
                var range = long.Parse(line.Split("r=")[1]);

                var c = new Coordinate
                {
                    Range = range,
                    X = xyz[0],
                    Y = xyz[1],
                    Z = xyz[2]
                };

                coords.Add(c);
            }

            //Part1
            var largestRangeCoord = coords.Aggregate((x, y) => x.Range >= y.Range ? x : y);
            int count = 0;

            foreach (var coordinate in coords)
            {
                if (ManhattanDistance(coordinate, largestRangeCoord) <= largestRangeCoord.Range)
                {
                    count++;
                }
            }

            Console.WriteLine(count);

            var naiive = (coords.Select(c => c.X).Average(), coords.Select(c => c.Y).Average(), coords.Select(c => c.Z).Average());
            
            Coordinate winner = new Coordinate{X = (long)Math.Round(naiive.Item1), Y = (long)Math.Round(naiive.Item2), Z = (long)Math.Round(naiive.Item3)};
            var coordsCount = coords.Count(c => ManhattanDistance(c, winner) <= c.Range);

            // sieve by powers of 10
            for (int t = 0; t < 7; t++)
            {
                Coordinate tempWinner = winner;
                int pow = (int)Math.Pow(10, t);
                int start = -50000000 / pow;
                int end = 50000000 / pow;
                int step = Math.Max(2000000 / pow, 1);

                for (int i = start; i < end; i += step)
                {
                    for (int j = start; j < end; j += step)
                    {
                        for (int k = start; k < end; k += step)
                        {
                            var naiiveC = new Coordinate
                            {
                                X = tempWinner.X + i, 
                                Y = tempWinner.Y + j,
                                Z = tempWinner.Z + k
                            };

                            var abc = coords.Count(c => ManhattanDistance(c, naiiveC) <= c.Range);
                            tempWinner = coordsCount >= abc ? tempWinner : naiiveC;
                            coordsCount = coordsCount >= abc ? coordsCount : abc;
                        }
                    }
                }

                bool winnerWillChange = coords.Count(c => ManhattanDistance(c, tempWinner) <= c.Range) >
                                        coords.Count(c => ManhattanDistance(c, winner) <= c.Range);
                winner = winnerWillChange
                    ? tempWinner
                    : winner;

                Console.WriteLine($"t = {t}, coordCount = {coordsCount}, coord: X = {winner.X}, Y = {winner.Y}, Z = {winner.Z}");

                if (winnerWillChange)
                {
                    t = -1;
                }
            }

            // Now for completeness check again with a sieve decreasing in size by a half each time (slightly finer than above)
            // TODO Methodise this for sieve size t
            for (int t = 0; t < 26; t++)
            {
                Coordinate tempWinner = winner;
                int pow = (int)Math.Pow(2, t);
                int start = -50000000 / pow;
                int end = 50000000 / pow;
                int step = Math.Max(2000000 / pow, 1);

                for (int i = start; i < end; i += step)
                {
                    for (int j = start; j < end; j += step)
                    {
                        for (int k = start; k < end; k += step)
                        {
                            var naiiveC = new Coordinate
                            {
                                X = tempWinner.X + i, 
                                Y = tempWinner.Y + j,
                                Z = tempWinner.Z + k
                            };

                            var abc = coords.Count(c => ManhattanDistance(c, naiiveC) <= c.Range);
                            tempWinner = coordsCount >= abc ? tempWinner : naiiveC;
                            coordsCount = coordsCount >= abc ? coordsCount : abc;
                        }
                    }
                }

                bool winnerWillChange = coords.Count(c => ManhattanDistance(c, tempWinner) <= c.Range) >
                                        coords.Count(c => ManhattanDistance(c, winner) <= c.Range);
                winner = winnerWillChange
                    ? tempWinner
                    : winner;

                Console.WriteLine($"t = {t}, coordCount = {coordsCount}, coord: X = {winner.X}, Y = {winner.Y}, Z = {winner.Z}");

                if (winnerWillChange)
                {
                    t = -1;
                }
            }

            Console.ReadLine();
        }

        public static long ManhattanDistance(Coordinate a, Coordinate b)
        {
            long res = Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y) + Math.Abs(a.Z - b.Z);

            return res;
        }
    }

    public class Coordinate
    {
        public long X { get; set; }
        public long Y { get; set; }
        public long Z { get; set; }
        public long Range { get; set; }
    }
}
