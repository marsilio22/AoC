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
            var depth = 8103;
            (int x, int y) target = (9, 758);
            int overshoot = 10;
            Dictionary<(int x, int y), Coordinate> map = new Dictionary<(int x, int y), Coordinate>();

            for (int i = 0; i <= target.x + overshoot; i++)
            {
                for (int j = 0; j <= target.y + overshoot; j++)
                {
                    long geoInd = 0;
                    if (i == 0 && j == 0 || i == target.x && j == target.y)
                    {
                        // do nothing
                    }
                    else if (i == 0)
                    {
                        geoInd = j * 48271;
                    }
                    else if (j == 0)
                    {
                        geoInd = i * 16807;
                    }
                    else
                    {
                        var other1 = map[(i - 1, j)];
                        var other2 = map[(i, j - 1)];
                        geoInd = other1.ErosionLevel * other2.ErosionLevel;
                    }


                    var coord = new Coordinate
                    {
                        X = i,
                        Y = j,
                        GeologicalIndex = geoInd,
                        ErosionLevel = (geoInd + depth) % 20183
                    };

                    map.Add((i, j), coord);
                }
            }

            var risk = map.Values.Where(c => c.X <= target.x && c.Y <= target.y).Select(c => c.Type == '.' ? 0 : c.Type == '=' ? 1 : 2).Sum();

            Console.WriteLine(risk);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i <= target.x + overshoot; i++)
            {
                for (int j = 0; j <= target.y + overshoot; j++)
                {
                    if (i == 0 && j == 0 || i == target.x && j == target.y)
                    {
                        sb.Append('X');
                    }
                    else
                    {
                        sb.Append(map[(i, j)].Type);                    
                    }
                }
                sb.AppendLine();
            }

            File.WriteAllText("./output.txt", sb.ToString());
            
            (int x, int y)[] dxy = new [] {(0, 1), (0, -1), (1, 0), (-1, 0)};

            List<(int x, int y, Equipment e, int distance)> coordsToCheckFrom = new List<(int x, int y, Equipment e, int distance)>();
            coordsToCheckFrom.Add((0, 0, Equipment.Torch, 0));
            var prevs = new List<(int x, int y, Equipment e, int distance)>();

            while (coordsToCheckFrom.Count > 0)
            {
                var xye = coordsToCheckFrom.First();
                coordsToCheckFrom.Remove(xye);
                prevs.Add(xye);
                
                (int x, int y)[] validNewXYs = dxy
                    .Select(d => (xye.x + d.x, xye.y + d.y))
                    .Where(c => 
                        c.Item1 >=0 && c.Item2 >= 0 && 
                        c.Item1 <= target.x + overshoot && c.Item2 <= target.y + overshoot &&
                        !prevs.Any(d => d.x == c.Item1 && d.y == c.Item2 && d.distance <= xye.distance)
                    ).ToArray();

                foreach (var xy in validNewXYs)
                {
                    if (xy.y < xye.y && Math.Abs(xy.x - target.x) + Math.Abs(xy.y - target.y) > 30)
                    {
                        continue;
                    }

                    var next = map[xy];

                    var equ = xye.e;
                    int time = 0;
                    switch (next.Type)
                    {
                        case '.': //rocky
                            time = equ == Equipment.Neither ? 8 : 1;
                            break;
                        case '=': //wet
                            time = equ == Equipment.Torch ? 8 : 1;
                            break;
                        case '|': //narrow
                            time = equ == Equipment.Climbing ? 8 : 1;
                            break;
                    }

                    int nextDistance = xye.distance + time;
                    
                    // if we swapped equipment, we need to add BOTH new equipment options to the list of new coordinates to check...

                    if (time == 1)
                    {
                        (int x, int y, Equipment e, int distance) nextF = (next.X, next.Y, xye.e, nextDistance);

                        if (coordsToCheckFrom.Any(c => c.x == nextF.x && c.y == nextF.y &&  c.distance < nextDistance))
                        {
                
                        }
                        else
                        {
                            var toRemove = coordsToCheckFrom.Where(c => c.x == nextF.x && c.y == nextF.y && c.distance > nextDistance).ToList();
                            foreach (var thing in toRemove)
                            {
                                coordsToCheckFrom.Remove(thing);
                            }
                            coordsToCheckFrom.Add(nextF);
                        }
                    }
                    else if (time == 8)
                    {
                        var eqs = new[] {Equipment.Climbing, Equipment.Torch, Equipment.Neither};
                        (int x, int y, Equipment e, int distance) nextA = (next.X, next.Y, eqs.First(e => e != xye.e), nextDistance);
                        (int x, int y, Equipment e, int distance) nextB = (next.X, next.Y, eqs.Last (e => e != xye.e), nextDistance);

                        if (coordsToCheckFrom.Any(c => c.x == nextA.x && c.y == nextA.y && c.distance < nextDistance))
                        {
                     
                        }
                        else
                        {
                            var toRemove = prevs.Where(c => c.x == nextA.x && c.y == nextA.y && c.distance > nextDistance).ToList();
                            foreach (var thing in toRemove)
                            {
                                coordsToCheckFrom.Remove(thing);
                            }
                            coordsToCheckFrom.Add(nextA);
                        }

                        if (coordsToCheckFrom.Any(c => c.x == nextB.x && c.y == nextB.y && c.distance < nextDistance))
                        {
                            
                        }
                        else
                        {
                            var toRemove = prevs.Where(c => c.x == nextB.x && c.y == nextB.y && c.distance > nextDistance).ToList();
                            foreach (var thing in toRemove)
                            {
                                coordsToCheckFrom.Remove(thing);
                            }
                            coordsToCheckFrom.Add(nextB);
                        }
                    }
                }

                coordsToCheckFrom = coordsToCheckFrom.Distinct().ToList();
                //prevs = prevs.Distinct().ToList();
            }

            File.WriteAllLines("./output.txt", prevs.Select(p => $"x = {p.x}, y = {p.y}, d = {p.distance}, e = {p.e}").ToList());
            Console.ReadLine();
        }
    }

    public enum Equipment
    {
        Neither,
        Torch,
        Climbing
    }

    public class Coordinate
    {
        public int X { get; set; }
        public int Y { get; set; }
        public long GeologicalIndex { get; set; }
        public long ErosionLevel { get; set; }

        // dot is rocky, equals is wet, pipe is narrow
        public char Type => this.ErosionLevel % 3 == 0 ? '.' : this.ErosionLevel % 3 == 1 ? '=' : '|';

        public int CurrentShortestTime { get; set; } = int.MaxValue;
        public Equipment ShortestCurrentEquipment { get; set; }

    }
}
