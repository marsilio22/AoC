using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Day_17
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("./input.txt");

            //var lines = new[]
            //{
            //    "x=495, y=2..7",
            //    "y=7, x=495..501",
            //    "x=501, y=3..7",
            //    "x=498, y=2..4",
            //    "x=506, y=1..2",
            //    "x=498, y=10..13",
            //    "x=504, y=10..13",
            //    "y=13, x=498..504"
            //};

            List<Coordinate> coords = new List<Coordinate>();
            foreach(var line in lines)
            {
                if (line.StartsWith("x"))
                {
                    var split = line.Split(", ");

                    var x = int.Parse(split[0].Split('=')[1]);
                    var ys = split[1].Split('=')[1].Split("..").Select(s => int.Parse(s)).ToList();

                    for (int i = ys[0]; i <= ys[1]; i++)
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

                    for (int i = xs[0]; i <= xs[1]; i++)
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
            var minX = coords.Min(c => c.X);
            var maxX = coords.Max(c => c.X);

            Console.WriteLine(maximumY);
            Console.WriteLine(minimumY);

            List<(int x, int y)> waterXYs = new List<(int x, int y)>{(500, minimumY-1)};
            while (waterXYs.Any()){
                for(int k = 0; k < waterXYs.Count; k++)
                {
                    if (coords.Count == 21568)
                    {
                        Print(coords);
                    }

                    var xy = waterXYs[k];
                    // flow water down from this x
                    var coordsBelowThisX = coords.Where(c => c.X == xy.x && c.Y > xy.y).ToList();
                    
                    if (!coordsBelowThisX.Any(c => c.IsClay))
                    {
                        // fill to the bottom of the map with hasContainedWater blocks, then stop everything for this loop and remove the xy from the list. Reset the counter
                        for (int i = xy.y; i <= maximumY; i++)
                        {
                            if (!coords.Any(c => c.X == xy.x && c.Y == i))
                            {
                                coords.Add(new Coordinate{X = xy.x, Y = i, HasContainedWater = true, });
                            }
                            else
                            {
                                coords.Single(c => c.X == xy.x && c.Y == i).HasContainedWater = true;
                            }
                        }

                        waterXYs.Remove(xy);
                        k = -1;
                        break;
                    }

                    var minY = coords.Where(c => c.X == xy.x && c.IsClay).Min(c => c.Y); // TODO check that there are any before doing min
                    
                    for (int i = xy.y; i < minY; i++)
                    {
                        if (!coords.Any(c => c.X == xy.x && c.Y == i))
                        {
                            coords.Add(new Coordinate{X = xy.x, Y = i, HasContainedWater = true, });
                        }
                        else
                        {
                            coords.Single(c => c.X == xy.x && c.Y == i).HasContainedWater = true;
                        }
                    }

                    // break this loop when we have new water xys to deal with
                    while(true)
                    {
                        var xs = coords.Where(c => c.Y == minY - 1 && c.IsClay).ToList();

                        var left = xs.Where(c => c.X < xy.x).OrderByDescending(c => c.X).FirstOrDefault();
                        var right = xs.Where(c => c.X > xy.x).OrderBy(c => c.X).FirstOrDefault();
                        
                        bool continuous = true;
                        // if both left and right are not null AND the space beneath them is all (clay OR water) then the space between them contains water
                        // if one of left and right is not null, OR the space beneath them is not all clay, then the space between that and the first clay in the other direction on the next y is dry sand, the next x is one to the left/right of that clay
                        // if both left and right are null, then we care about the row below.
                        if (left != null && right != null)
                        {
                            var myCoords = coords.Where(c => c.Y == minY && c.X >= left.X && c.X <= right.X).ToList();

                            // Check if there's clay or water all the way underneath the two things. If not then we want the other if...
                            for (int i = left.X; i <= right.X; i++)
                            {
                                if (!myCoords.Any(c => c.X == i && (c.IsClay || c.ContainsWater)))
                                {
                                    continuous = false;
                                    break;
                                }
                            }

                            if (continuous)
                            {
                                for (int i = left.X + 1; i < right.X; i++)
                                {
                                    Coordinate coordinate;
                                    if (!coords.Any(c => c.X == i && c.Y == minY - 1))
                                    {
                                        coordinate = new Coordinate{X = i, Y = minY-1};
                                        coords.Add(coordinate);
                                    }
                                    coordinate = coords.Single(c => c.X == i && c.Y == minY - 1);

                                    coordinate.ContainsWater = true;
                                }
                                minY -= 1;
                            }
                        }
                        
                        if (left == null || right == null || !continuous)
                        {
                            // need to figure out the furthest left and right the water can go. then add that coord to the xys, assuming it's not clay.
                            // these need to be set to HasContainedWater
                            //var furthestLeft = coords.Where(c => c.Y == minY && (c.IsClay || c.ContainsWater)).

                            for (int i = 0;; i++)
                            {
                                var block = coords.SingleOrDefault(c => c.Y == minY && c.X == xy.x - i);
                                // left, take i away. Break when i the block is null, then do right
                                if (block != null && (block.IsClay || block.ContainsWater))
                                {
                                    // Add a hascontainedwater cell above the one described above
                                    Coordinate coordinate;
                                    if (!coords.Any(c => c.X == block.X && c.Y == block.Y - 1))
                                    {
                                        coordinate = new Coordinate {X = block.X, Y = block.Y - 1};
                                        coords.Add(coordinate);
                                    }

                                    coordinate = coords.Single(c => c.X == block.X && c.Y == block.Y - 1);

                                    if (coordinate.IsClay)
                                    {
                                        // if the block is clay. Stop.
                                        break;
                                    }

                                    coordinate.HasContainedWater = true;
                                    continue;
                                }
                                else
                                {
                                    Coordinate coordinate;
                                    if (!coords.Any(c => c.X == xy.x - i && c.Y == minY - 1))
                                    {
                                        coordinate = new Coordinate {X = xy.x - i, Y = minY - 1};
                                        coords.Add(coordinate);
                                    }

                                    coordinate = coords.Single(c => c.X == xy.x - i && c.Y == minY - 1);
                                    coordinate.HasContainedWater = !coordinate.IsClay;

                                    if (!coordinate.IsClay)
                                    {
                                        waterXYs.Remove(xy);
                                        if (coordinate.Y < maximumY && coordinate.Y > minimumY)
                                        {
                                            waterXYs.Add((coordinate.X, coordinate.Y));
                                            k = -1;
                                        }
                                        break;
                                    }
                                }
                            }

                            for (int i = 0;; i++)
                            {
                                var block = coords.SingleOrDefault(c => c.Y == minY && c.X == xy.x + i);
                                // left, take i away. Break when i the block is null, then do right
                                if (block != null && (block.IsClay || block.ContainsWater))
                                {
                                    // Add a hascontainedwater cell above the one described above
                                    Coordinate coordinate;
                                    if (!coords.Any(c => c.X == block.X && c.Y == block.Y - 1))
                                    {
                                        coordinate = new Coordinate {X = block.X, Y = block.Y - 1};
                                        coords.Add(coordinate);
                                    }

                                    coordinate = coords.Single(c => c.X == block.X && c.Y == block.Y - 1);

                                    if (coordinate.IsClay)
                                    {
                                        // if the block is clay. Stop.
                                        break;
                                    }

                                    coordinate.HasContainedWater = true;
                                    continue;
                                }
                                else
                                {
                                    Coordinate coordinate;
                                    if (!coords.Any(c => c.X == xy.x + i && c.Y == minY - 1))
                                    {
                                        coordinate = new Coordinate {X = xy.x + i, Y = minY - 1};
                                        coords.Add(coordinate);
                                    }

                                    coordinate = coords.Single(c => c.X == xy.x + i && c.Y == minY - 1);
                                    coordinate.HasContainedWater = true;

                                    if (!coordinate.IsClay)
                                    {
                                        waterXYs.Remove(xy);
                                        if (coordinate.Y < maximumY && coordinate.Y > minimumY)
                                        {
                                            waterXYs.Add((coordinate.X, coordinate.Y));
                                            k = -1;
                                        }
                                        break;
                                    }
                                }
                            }

                            break;
                        }
                    }
                }
            }

            Console.WriteLine(coords.Count(c => (c.ContainsWater || c.HasContainedWater) && c.Y <= maximumY && c.Y >= minimumY));
            Console.ReadLine();
        }

        static void Print(List<Coordinate> coords)
        {
            //Dictionary<(int x, int y), Coordinate> coordsordered = coords.OrderBy(c => c.Y).ThenBy(c => c.X).ToDictionary(c => (c.X, c.Y), c => c);
            var coordsordered = coords.OrderBy(c => c.Y).ThenBy(c => c.X).ToLookup(c => (c.X, c.Y));

            var maxY = coords.Max(c => c.Y);
            var minY = coords.Min(c => c.Y);
            var minX = coords.Min(c => c.X);
            var maxX = coords.Max(c => c.X);

            var sb = new StringBuilder();

            for (int i = minY; i <= maxY; i++)
            {
                for (int j = minX; j <= maxX; j++)
                {
                    if (coordsordered.Contains((j, i)))
                    {
                        var coord = coordsordered[(j, i)].First();
                        sb.Append(coord.IsClay ? '#' : coord.ContainsWater ? '~' : coord.HasContainedWater ? '|' : '.');
                    }
                    else
                    {
                        sb.Append('.');
                    }
                }

                sb.AppendLine();
            }

            Console.WriteLine(sb.ToString());
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