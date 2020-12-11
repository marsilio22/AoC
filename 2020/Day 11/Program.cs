using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_11
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadLines("./input.txt").ToList();

            IDictionary<(int x, int y), char> coords = new Dictionary<(int x, int y), char>();

            for (int x = 0; x < lines.Count; x ++)
            {
                var line = lines[x];
                
                for (int y = 0; y < line.Length; y++)
                {
                    if (line[y] != '.')
                    {
                        coords[(x, y)] = line[y];
                    }
                }
            }

            var visibleCoordRelationships = new Dictionary<(int x, int y), ICollection<(int x, int y)>>();

            foreach(var coord in coords){
                // Part 1, coordinates can see the coordinates next to them in all 8 directions
                visibleCoordRelationships[coord.Key] = 
                    coords.Keys.Where(d => 
                        (coord.Key.x != d.x || coord.Key.y != d.y) && (
                            Math.Abs(d.x - coord.Key.x) <= 1 && 
                            Math.Abs(d.y - coord.Key.y) <= 1)
                        ).ToList();
            }

            Calculate(coords.ToDictionary(c => c.Key, c => c.Value), visibleCoordRelationships, 4);



            visibleCoordRelationships = new Dictionary<(int x, int y), ICollection<(int x, int y)>>();

            foreach(var coord in coords){
                // Part 2, coordinates can see the coordinates at any distance in the 8 directions
                visibleCoordRelationships[coord.Key] = GetVisibleCoords(coord.Key, coords.Keys);
            }

            Calculate(coords.ToDictionary(c => c.Key, c => c.Value), visibleCoordRelationships, 5);
        }

        public static void Calculate(
            IDictionary<(int x, int y), char> coords, 
            IDictionary<(int x, int y), ICollection<(int x, int y)>> visibleCoordRelationships, 
            int numberFilledSeatsThatMatter){
            
            while (true)
            {
                //Console.Clear();
                //PrintDict(coords);
                List<KeyValuePair<(int x, int y), char>> coordsToHash = new List<KeyValuePair<(int x, int y), char>>();
                List<KeyValuePair<(int x, int y), char>> coordsToL = new List<KeyValuePair<(int x, int y), char>>();

                foreach( var coord in coords){
                    var visibleCoords = visibleCoordRelationships[coord.Key];

                    if (coord.Value == 'L' && visibleCoords.All(d => coords[d] != '#')){
                        coordsToHash.Add(coord);
                    }
                    
                    if (coord.Value == '#' && visibleCoords.Where(d => coords[d] == '#').Count() >= numberFilledSeatsThatMatter)
                    {
                        coordsToL.Add(coord);
                    }
                }

                if (!coordsToHash.Any() && !coordsToL.Any())
                {
                    break;
                }

                foreach(var coord in coordsToHash)
                {
                    coords[coord.Key] = '#';
                }
                foreach(var coord in coordsToL){
                    coords[coord.Key] = 'L';
                }
            }

            Console.WriteLine(coords.Values.Count(v => v == '#'));
        }

        public static void PrintDict (IDictionary<(int x, int y), char> coords){
            var x = coords.Keys.Select(c => c.x).Max();
            var y = coords.Keys.Select(c => c.y).Max();
            for (int i = 0; i <= x; i++){
                for (int j = 0; j <= y; j++){
                    if (coords.ContainsKey((i, j)))
                    {
                        Console.Write(coords[(i, j)]);
                    }
                    else
                    {
                        Console.Write('.');
                    }
                }
                Console.WriteLine();
            }
        }

        public static ICollection<(int x, int y)> GetVisibleCoords((int x, int y) coord, ICollection<(int x, int y)> coords){
            var north = coords.Where(c => c.y == coord.y && c.x < coord.x).OrderBy(c => Math.Abs(c.x - coord.x)).ToList();
            var south = coords.Where(c => c.y == coord.y && c.x > coord.x).OrderBy(c => Math.Abs(c.x - coord.x)).ToList();
            
            var east = coords.Where(c => c.x == coord.x && c.y > coord.y).OrderBy(c => Math.Abs(c.y - coord.y)).ToList();
            var west = coords.Where(c => c.x == coord.x && c.y < coord.y).OrderBy(c => Math.Abs(c.y - coord.y)).ToList();

            var northWest = coords.Where(c => c.x < coord.x && c.x - c.y == coord.x - coord.y).OrderBy(c => Math.Abs(c.x - coord.x) + Math.Abs(c.y - coord.y)).ToList();
            var southEast = coords.Where(c => c.x > coord.x && c.x - c.y == coord.x - coord.y).OrderBy(c => Math.Abs(c.x - coord.x) + Math.Abs(c.y - coord.y)).ToList();

            var northEast = coords.Where(c => c.x < coord.x && c.x + c.y == coord.x + coord.y).OrderBy(c => Math.Abs(c.x - coord.x) + Math.Abs(c.y - coord.y)).ToList();
            var southWest = coords.Where(c => c.x > coord.x && c.x + c.y == coord.x + coord.y).OrderBy(c => Math.Abs(c.x - coord.x) + Math.Abs(c.y - coord.y)).ToList();

            var northC = north.Any() ? north.First() : (-1, -1);
            var southC = south.Any() ? south.First() : (-1, -1);
            var eastC = east.Any() ? east.First() : (-1, -1);
            var westC = west.Any() ? west.First() : (-1, -1);
            var northWestC = northWest.Any() ? northWest.First() : (-1, -1);
            var southEastC = southEast.Any() ? southEast.First() : (-1, -1);
            var northEastC = northEast.Any() ? northEast.First() : (-1, -1);
            var southWestC = southWest.Any() ? southWest.First() : (-1, -1);

            var ans = new List<(int x, int y)>{
                northC,
                southC,
                eastC,
                westC,
                northWestC,
                southEastC,
                northEastC,
                southWestC
            }.Where(c => !c.Equals((-1, -1)));

            return ans.ToList();
        }
    }
}
