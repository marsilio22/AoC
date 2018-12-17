using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_17
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("./input.txt");

            List<Coordinate> coords = new List<Coordinate>();
            foreach(var line in lines)
            {
                if (line.StartsWith("x"))
                {
                    var split = line.Split(", ");

                    var x = int.Parse(split[0].Split('=')[1]);
                    var ys = split[1].Split('=')[1].Split("..").Select(s => int.Parse(s)).ToList();

                    for (int i = ys[0]; i < ys[1]; i++)
                    {
                        var coord = new Coordinate{
                            X = x,
                            Y = i,
                            IsClay = true
                        };
                        coords.Add(coord);
                    }
                }
                else if (line.StartsWith("y"))
                {
                    var split = line.Split(", ");

                    var y = int.Parse(split[0].Split('=')[1]);
                    var xs = split[1].Split('=')[1].Split("..").Select(s => int.Parse(s)).ToList();

                    for (int i = xs[0]; i < xs[1]; i++)
                    {
                        var coord = new Coordinate{
                            X = i,
                            Y = y,
                            IsClay = true
                        };
                        coords.Add(coord);
                    }
                }
            }

            var maximumY = coords.Max(c => c.Y);
            var minimumY = coords.Min(c => c.Y);
            Console.WriteLine(maximumY);
            Console.WriteLine(minimumY);

            var waterspring = new Coordinate{X = 500, Y = 0};
            List<(int x, int y)> waterXYs = new List<(int x, int y)>{(500, minimumY-1)};
            while (waterXYs.Any()){
                for(int k = 0; k < waterXYs.Count; k++)
                {
                    var xy = waterXYs[k];
                    // flow water down from this x
                    var minY = coords.Where(c => c.X == xy.x).Min(c => c.Y); // TODO check that there are any before doing min
                    
                    for (int i = xy.y; i < minY; i++)
                    {
                        if (!coords.Any(c => c.X == xy.x && c.Y == i))
                        {
                            coords.Add(new Coordinate{X = xy.x, Y = i, HasContainedWater = true, });
                        }
                    }

                    Coordinate previousLeft = null;
                    Coordinate previousRight = null;

                    // break this loop when we have new water xys to deal with
                    while(true)
                    {
                        var xs = coords.Where(c => c.Y == minY - 1 && c.IsClay).ToList();

                        var left = xs.Where(c => c.X < xy.x).OrderByDescending(c => c.X).FirstOrDefault();
                        var right = xs.Where(c => c.X > xy.x).OrderBy(c => c.X).FirstOrDefault();

                        // if both left and right are not null then the space between them contains water
                        // if one of left and right is not null, then the space between that and the first clay in the other direction on the next y is dry sand, the next x is one to the left/right of that clay
                        // if both left and right are null, then we care about the row below.
                        if (left != null && right != null)
                        {
                            // Hmm no, this doesn't check that the thing is contained along the bottom...
                            // The pattern:
                            //#          #
                            //       #####
                            // Would result in
                            //#~~~~~~~~~~#
                            //       #####
                            // Instead of
                            //#     |||||#
                            //      |#####
                            for (int i = left.X + 1; i < right.X; i++)
                            {
                                Coordinate coordinate;
                                if (!coords.Any(c => c.X == i && c.Y == minY - 1))
                                {
                                    coordinate = new Coordinate{X = i, Y = minY-1};
                                }
                                coordinate = coords.Single(c => c.X == i && c.Y == minY - 1);

                                coordinate.ContainsWater = true;
                            }
                            minY -= 1;
                        }
                        else if (left == null && right != null || right == null && left != null)
                        {

                        }
                        else if (left == null && right == null)
                        {

                        }
                    }
                }
            }
        }
    }

    class Coordinate{
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsClay { get; set; }
        public bool ContainsWater { get; set; }
        public bool HasContainedWater { get; set; }
    }
}
