using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_20
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadLines("./input.txt").ToList();
            var tiles = new Dictionary<int, IDictionary<(int x, int y), char>>();
            int num = 0;

            for(int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];

                if (line.StartsWith("Tile"))
                {
                    num = int.Parse(line.Split(" ")[1].Split(':')[0]);
                    tiles.Add(num, new Dictionary<(int, int), char>());
                }
                else if (line.Contains('#') || line.Contains('/'))
                {
                    for(int j = 0; j < 10; j++)
                    {
                        line = lines[i+j];
                        for (int k = 0; k < 10; k++)
                        {
                            tiles[num].Add((k, j), line[k]);
                        }
                    }
                    i += 10;
                }
            }

            var bigPicture = new Dictionary<(int x, int y), char>();
            var edges = new Dictionary<int, IDictionary<string, string>>();

            foreach(var tile in tiles)
            {
                var topEdge = string.Join(null, tile.Value.Where(v => v.Key.y == 0).Select(c => c.Value).ToList());
                var bottomEdge = string.Join(null, tile.Value.Where(v => v.Key.y == 9).Select(c => c.Value).ToList());
                var leftEdge = string.Join(null, tile.Value.Where(v => v.Key.x == 0).Select(c => c.Value).ToList());
                var rightEdge = string.Join(null, tile.Value.Where(v => v.Key.x == 9).Select(c => c.Value).ToList());

                edges.Add(tile.Key, new Dictionary<string, string>{
                    { "top", topEdge },
                    { "bottom", bottomEdge },
                    { "left", leftEdge },
                    { "right", rightEdge }
                });
            }

            var potentialMatches = new Dictionary<int, List<int>>();
            var tileEdgeCounts = edges.Select(e =>new KeyValuePair<int, ICollection<int>>(e.Key, e.Value.Select(c => c.Value.Count(v => v == '#')).ToList()));

            foreach(var tile in edges)
            {
                var tileCounts = tile.Value.Select(c => c.Value.Count(v => v == '#')).ToList();

                var matches = tileEdgeCounts.Where(c => c.Key != tile.Key && c.Value.Any(v => tileCounts.Contains(v))).Select(m => m.Key).ToList();
                potentialMatches.Add(tile.Key, matches);
            }
            
            var edgesCopy = edges.ToDictionary(c => c.Key, c => c.Value);

            var adjacents = new Dictionary<int, ICollection<(int, string)>>();


            foreach(var tile in edges)
            {
                adjacents.Add(tile.Key, new List<(int, string)>());

                foreach(var otherTile in potentialMatches[tile.Key])
                {
                    var otherTileEdges = edges[otherTile];
                    
                    // would doing this be faster? Could compare longs instead of strings
                    // var thisEdgesNumeric = tile.Value.Select(c => Convert.ToInt64(c.Value.Replace('#', '1').Replace('.', '0'), 2)).ToList();
                    // addendum, it's fast anyway so doesn't matter.

                    var otherTileEdgesValues = 
                        otherTileEdges.Concat(
                            otherTileEdges.Select(e => (
                                e.Key + "$", 
                                new string(e.Value.Reverse().ToArray())
                            )).ToDictionary(e => e.Item1, e => e.Item2))
                        .ToDictionary(e => e.Key, e => e.Value);

                    var matchingEdges = tile.Value.Where(v => otherTileEdgesValues.Values.Contains(v.Value)).ToList();

                    if(matchingEdges.Any())
                    {
                        var edge = matchingEdges.Single();
                        var otherTileEdge = otherTileEdgesValues.Single(e => e.Value.Equals(edge.Value));

                        var requiredOperation = D4_element.E;
                        // assume for now that edge.Key == bottom. Sort this out later

                        switch(otherTileEdge.Key)
                        {
                            case "left":
                                // rotate 90 clockwise
                                requiredOperation = D4_element.R;
                                break;
                            case "right":
                                // rotate 270 clockwise
                                requiredOperation = D4_element.RRR;
                                break;
                            case "top":
                                // do nothing, bottom -> top is correct already
                                break;
                            case "bottom":
                                // rotate 180
                                requiredOperation = D4_element.RR;
                                break;
                            
                            // REFLECTIONS CHANGE THE DIRECION OF TURNING, SO ROTATE FIRST
                            case "left$":
                                // Rotate 90 reflect
                                requiredOperation = D4_element.RF;
                                break;
                            case "right$":
                                // Rotate 270 and reflect
                                requiredOperation = D4_element.RRRF;
                                break;
                            case "top$":
                                // just reflect
                                requiredOperation = D4_element.F;
                                break;
                            case "bottom$":
                                // Rotate 180 and reflect
                                requiredOperation = D4_element.RRF;
                                break;
                        }

                        Console.WriteLine($"Tile {tile.Key} is adjacent to tile {otherTile}, {edge.Key} to {otherTileEdge.Key}");
                        adjacents[tile.Key].Add((otherTile, $"{edge.Key} to {otherTileEdge.Key}"));
                   }
                }
            }

            var corners = adjacents.Where(c => c.Value.Count == 2).Select(c => c.Key).ToList();

            // Part 1
            Console.WriteLine((long)corners[0] * (long)corners[1] * (long)corners[2] * (long)corners[3]);

            // need to keep track of whether or not the tile we're looking at right now has been reflected or not

            var tileMap = new Dictionary<(int x, int y), string>();
            // may as well start at a corner
            var currentTile = adjacents.Single(c => c.Key ==corners.First());
            var cornertiles = adjacents.Where(c => corners.Contains(c.Key) ).ToList();

            while(tileMap.Count < tiles.Count)
            {
                // tileMap.Add((0, 0), currentTile.Key)
            }
        }
    }

    public class D4 
    {
        public D4_element Value {get;set;}

        private static Dictionary<(D4_element, D4_element), D4_element> multiplicationTable = 
            new Dictionary<(D4_element, D4_element), D4_element>{
                {(D4_element.R, D4_element.R), D4_element.RR},
                {(D4_element.R, D4_element.RR), D4_element.RRR},
                {(D4_element.R, D4_element.RRR), D4_element.E},
                {(D4_element.R, D4_element.F), D4_element.RF},
                {(D4_element.R, D4_element.RF), D4_element.RRF},
                {(D4_element.R, D4_element.RRF), D4_element.RRRF},
                {(D4_element.R, D4_element.RRRF), D4_element.F},
                
                {(D4_element.RR, D4_element.R), D4_element.RRR},
                {(D4_element.RR, D4_element.RR), D4_element.E},
                {(D4_element.RR, D4_element.RRR), D4_element.R},
                {(D4_element.RR, D4_element.F), D4_element.RRF},
                {(D4_element.RR, D4_element.RF), D4_element.RRRF},
                {(D4_element.RR, D4_element.RRF), D4_element.F},
                {(D4_element.RR, D4_element.RRRF), D4_element.RF},
                
                {(D4_element.RRR, D4_element.R), D4_element.E},
                {(D4_element.RRR, D4_element.RR), D4_element.R},
                {(D4_element.RRR, D4_element.RRR), D4_element.RR},
                {(D4_element.RRR, D4_element.F), D4_element.RRRF},
                {(D4_element.RRR, D4_element.RF), D4_element.F},
                {(D4_element.RRR, D4_element.RRF), D4_element.RF},
                {(D4_element.RRR, D4_element.RRRF), D4_element.RRF},
                
                {(D4_element.F, D4_element.R), D4_element.RRRF},
                {(D4_element.F, D4_element.RR), D4_element.RRF},
                {(D4_element.F, D4_element.RRR), D4_element.RF},
                {(D4_element.F, D4_element.F), D4_element.E},
                {(D4_element.F, D4_element.RF), D4_element.RRR},
                {(D4_element.F, D4_element.RRF), D4_element.RR},
                {(D4_element.F, D4_element.RRRF), D4_element.R},
                
                {(D4_element.RF, D4_element.R), D4_element.F},
                {(D4_element.RF, D4_element.RR), D4_element.RRRF},
                {(D4_element.RF, D4_element.RRR), D4_element.RRF},
                {(D4_element.RF, D4_element.F), D4_element.R},
                {(D4_element.RF, D4_element.RF), D4_element.E},
                {(D4_element.RF, D4_element.RRF), D4_element.RRR},
                {(D4_element.RF, D4_element.RRRF), D4_element.RR},
                
                {(D4_element.RRF, D4_element.R), D4_element.RF},
                {(D4_element.RRF, D4_element.RR), D4_element.F},
                {(D4_element.RRF, D4_element.RRR), D4_element.RRRF},
                {(D4_element.RRF, D4_element.F), D4_element.RR},
                {(D4_element.RRF, D4_element.RF), D4_element.R},
                {(D4_element.RRF, D4_element.RRF), D4_element.E},
                {(D4_element.RRF, D4_element.RRRF), D4_element.RRR},
                
                {(D4_element.RRRF, D4_element.R), D4_element.RRF},
                {(D4_element.RRRF, D4_element.RR), D4_element.RF},
                {(D4_element.RRRF, D4_element.RRR), D4_element.F},
                {(D4_element.RRRF, D4_element.F), D4_element.RRR},
                {(D4_element.RRRF, D4_element.RF), D4_element.RR},
                {(D4_element.RRRF, D4_element.RRF), D4_element.R},
                {(D4_element.RRRF, D4_element.RRRF), D4_element.E}
            }

        public static D4 operator * (D4 a, D4 b)
        {
            if (a.Value == D4_element.E){
                return b;
            }

            if (b.Value == D4_element.E)
            {
                return a;
            }

            if (multiplicationTable.ContainsKey((a.Value, b.Value)))
            {
                return new D4{Value = multiplicationTable[(a.Value, b.Value)]};
            }
            else
            {
                return new D4{Value = multiplicationTable[(a.Value, b.Value)]};
            }
        }
    }

    public enum D4_element
    {
        E,
        R,
        RR,
        RRR,
        F,
        RF,
        RRF,
        RRRF
    }
}
