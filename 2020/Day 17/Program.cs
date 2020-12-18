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
            var lines = File.ReadLines("./input.txt").ToList();

            // part1 is just part2 but with w = 0
            // this bool will indicate whether w is fixed at 0
            var parts = new [] {true, false};

            // TODO I think this would be easier to parse if we could just call it with 
            // the different dimensions. Might be fun too but would have to key by like
            // arrays, or something mad.
            foreach(var fixW in parts)
            {
                var coords = new Dictionary<(int x, int y, int z, int w), char>();
                var x = 0;
                var y = 0;
                foreach(var line in lines)
                {
                    foreach(var character in line)
                    {
                        coords[(x, y, 0, 0)] = character;
                        x++;
                    }
                    x = 0;
                    y++;
                }

                for (int i = 0; i < 6; i++)
                {
                    // need to add edge inactive neighbours to the list each time
                    var maxX = coords.Keys.Select(c => c.x).Max();
                    var minX = coords.Keys.Select(c => c.x).Min();
                    var maxY = coords.Keys.Select(c => c.y).Max();
                    var minY = coords.Keys.Select(c => c.y).Min();
                    var maxZ = coords.Keys.Select(c => c.z).Max();
                    var minZ = coords.Keys.Select(c => c.z).Min();
                    var maxW = coords.Keys.Select(c => c.w).Max();
                    var minW = coords.Keys.Select(c => c.w).Min();

                    if (fixW)
                    {
                        maxW = 0;
                        minW = 0;
                    }

                    var edges = coords.Keys.Where(c => c.x == maxX || c.x == minX || c.y == maxY || c.y == minY || c.z == maxZ || c.z == minZ || c.w == maxW || c.w == minW).ToList();

                    foreach(var c in edges)
                    {
                        var neighbours = new List<(int x, int y, int z, int w)>();
                        var range = Enumerable.Range(-1, 3);
                        neighbours = (from r in range
                                    from s in range
                                    from t in range
                                    from u in range
                                    select (c.x + r, c.y + s, c.z + t, c.w + u)).ToList();

                        if (fixW)
                        {
                            neighbours.RemoveAll(n => n.w != 0);
                        }

                        foreach(var neighbour in neighbours)
                        {
                            if (!coords.ContainsKey(neighbour))
                            {
                                coords.Add(neighbour, '.');
                            }
                        }
                    }

                    var next = new Dictionary<(int, int, int, int), char>();

                    foreach(var coord in coords)
                    {
                        var neighbours = new List<(int x, int y, int z, int w)>();
                        var range = Enumerable.Range(-1, 3);

                        neighbours = (from r in range
                                    from s in range
                                    from t in range
                                    from u in range
                                    select (coord.Key.x + r, coord.Key.y + s, coord.Key.z + t, coord.Key.w + u)).ToList();
                        neighbours.Remove(coord.Key);

                        if (fixW)
                        {
                            neighbours.RemoveAll(n => n.w != 0);
                        }

                        // var count = neighbours.Count(n => coords.ContainsKey(n) && coords[n] == '#'); 

                        // maybe slightly more efficient if we short circuit when > 4 ???
                        // perhaps only noticable if we were at even more dimensions
                        var count = 0; 
                        int k = 0;
                        while (count < 4 && k < neighbours.Count)
                        {
                            var n = neighbours[k];
                            if (coords.ContainsKey(n) && coords[n] == '#')
                            {
                                count++;
                            }
                            k++;
                        }

                        if (coord.Value == '#' && count < 2 || count > 3)
                        {
                            next[coord.Key] = '.';
                        }
                        else if (coord.Value == '.' && count == 3)
                        {
                            next[coord.Key] = '#';
                        }
                        else
                        {
                            next[coord.Key] = coord.Value;
                        }
                    }
                    coords = next;
                }

                Console.WriteLine(coords.Count(c => c.Value == '#'));
            }
        }
    }
}
