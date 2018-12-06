using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_3
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("./input.txt");
            
            var coords = new Dictionary<(int x, int y), string>();
            foreach(var line in lines)
            {
                var halves = line.Split(':');
                
                var id = halves[0].Split('@')[0];

                var xy = halves[0].Split('@')[1];
                var xyArray = xy.Trim().Split(',').Select(c => int.Parse(c)).ToArray();
                
                var size = halves[1].Trim().Split('x').Select(c => int.Parse(c)).ToArray();

                // Console.WriteLine($"xyArray = {xyArray[0]}, {xyArray[1]}; size = {size[0]}, {size[1]}");

                var allCoveredCoords = GetCoords(xyArray[0], xyArray[1], size[0], size[1]);
                var overlappedIds = new List<string>();

                foreach(var thing in allCoveredCoords)
                {
                    if (coords.ContainsKey(thing))
                    {
                        overlappedIds.Add(coords[thing]);
                        coords[thing] = "OVERLAP";
                    }
                    else
                    {
                        coords[thing] = id;
                    }
                }

                if (overlappedIds.Count > 0){
                    foreach(var thing in allCoveredCoords){
                        coords[thing] = "OVERLAP";
                    }

                    var overlappedCoords = coords.Where(c => overlappedIds.Contains(c.Value)).Select(c => c.Key).ToList();
                    foreach(var thing in overlappedCoords){
                        coords[thing] = "OVERLAP";
                    }
                }
            }
            var answer = coords.Values.Distinct();
            foreach(var thing in answer){
                Console.WriteLine(thing);
            }
        }

        public static List<(int x, int y)> GetCoords (int coordX, int coordY, int sizeX, int sizeY){
            var result = new List<(int x, int y)>();
            for (int i = 0; i < sizeX; i++)
            {
                for(int j = 0; j < sizeY; j++)
                {
                    result.Add((coordX + i, coordY + j));
                }
            }

            return result;
        }
    }
}
