using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace Day_25
{
    class Program
    {
        static void Main(string[] args)
        {
            var coords = File.ReadAllLines("./input.txt").Select(a =>
                new Coordinate
                {
                    X = int.Parse(a.Split(',')[0]),
                    Y = int.Parse(a.Split(',')[1]),
                    Z = int.Parse(a.Split(',')[2]),
                    T = int.Parse(a.Split(',')[3])
                }).ToList();

            List<List<Coordinate>> constellations = new List<List<Coordinate>>();

            foreach (var coord in coords)
            {
                var localCluster = coords.Where(l => Manhattan(l, coord) <= 3).ToList();

                var mergeConstellations = new List<List<Coordinate>>();

                foreach (var star in localCluster)
                {
                    if (constellations.Any(c => c.Contains(star)))
                    {
                        mergeConstellations.AddRange(constellations.Where(c => c.Contains(star)).ToList());
                    }
                }

                mergeConstellations = mergeConstellations.Distinct().ToList();

                foreach (var con in mergeConstellations)
                {
                    constellations.Remove(con);
                }

                mergeConstellations.Add(localCluster);
                constellations.Add(mergeConstellations.SelectMany(m => m).Distinct().ToList());
            }

            Console.WriteLine(constellations.Count());
            Console.ReadLine();
        }

        static int Manhattan(Coordinate a, Coordinate b)
        {
            var res = Math.Abs(a.X - b.X) +
                      Math.Abs(a.Y - b.Y) +
                      Math.Abs(a.Z - b.Z) +
                      Math.Abs(a.T - b.T);
            return res;
        }
    }

    public class Coordinate
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int T { get; set; }
    }
}
