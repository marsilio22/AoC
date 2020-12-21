using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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

            var edges = new Dictionary<int, IDictionary<string, string>>();

            foreach(var tile in tiles)
            {
                // NB we read edges in a clockwise direction
                var topEdge = string.Join(null, tile.Value.Where(v => v.Key.y == 0).Select(c => c.Value).ToList());
                var bottomEdge = string.Join(null, tile.Value.Where(v => v.Key.y == 9).Reverse().Select(c => c.Value).ToList());
                var leftEdge = string.Join(null, tile.Value.Where(v => v.Key.x == 0).Reverse().Select(c => c.Value).ToList());
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

            var adjacents = new Dictionary<int, ICollection<int>>();


            foreach(var tile in edges)
            {
                adjacents.Add(tile.Key, new List<int>());

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

                        //Console.WriteLine($"Tile {tile.Key} is adjacent to tile {otherTile}");
                        adjacents[tile.Key].Add(otherTile);
                   }
                }
            }

            var corners = adjacents.Where(c => c.Value.Count == 2).Select(c => c.Key).ToList();

            // Part 1
            Console.WriteLine((long)corners[0] * (long)corners[1] * (long)corners[2] * (long)corners[3]);

            var firstTile = adjacents.First();
            
            var tileMap2 = new Dictionary<(int x, int y), (int tileId, IDictionary<(int x, int y), char> tileValues)>();
            tileMap2.Add((0, 0), (firstTile.Key, tiles[firstTile.Key]));

            var queue = new Queue<KeyValuePair<int, ICollection<int>>>();

            var operations = new List<D4_element>
            {
                D4_element.E,
                D4_element.R,
                D4_element.RR,
                D4_element.RRR,
                D4_element.F,
                D4_element.RF,
                D4_element.RRF,
                D4_element.RRRF
            };

            queue.Enqueue(firstTile);

            while(queue.Any())
            {
                // get this tile
                var currentTile = queue.Dequeue();
                var tile = tileMap2.Single(t => t.Value.tileId == currentTile.Key);
                var adjTileIds = currentTile.Value.ToList();

                // for each adjacent to this tile
                var adjs = tiles.Where(t => adjTileIds.Contains(t.Key) && !tileMap2.Any(t2 => t2.Value.tileId == t.Key));

                // find the rotary operation which matches an unused edge
                var topEdge = string.Join(null, tile.Value.tileValues.Where(v => v.Key.y == 0).Select(c => c.Value).ToList());
                var bottomEdge = string.Join(null, tile.Value.tileValues.Where(v => v.Key.y == 9).Reverse().Select(c => c.Value).ToList());
                var leftEdge = string.Join(null, tile.Value.tileValues.Where(v => v.Key.x == 0).Reverse().Select(c => c.Value).ToList());
                var rightEdge = string.Join(null, tile.Value.tileValues.Where(v => v.Key.x == 9).Select(c => c.Value).ToList());

                var currentTileEdges = new Dictionary<string, string>{
                    { "top", topEdge },
                    { "bottom", bottomEdge },
                    { "left", leftEdge },
                    { "right", rightEdge }
                };

                var currentTileCoord = tileMap2.Single(t => t.Value.tileId == currentTile.Key).Key;

                foreach(var otherTile in adjs)
                {
                    // try every operation until we get a match
                    foreach(var operation in operations)
                    {
                        // rotate and reflect under the operation
                        var otherTile2 = Rotate(otherTile.Value, operation, 'x');
                        
                        // get the new edges. NB we read them backwards (anticlockwise) so that the strings will match 
                        // the forwards (clockwise) read edges of the currentTile
                        var otherTopEdge = string.Join(null, otherTile2.Where(v => v.Key.y == 0).Reverse().Select(c => c.Value).ToList());
                        var otherBottomEdge = string.Join(null, otherTile2.Where(v => v.Key.y == 9).Select(c => c.Value).ToList());
                        var otherLeftEdge = string.Join(null, otherTile2.Where(v => v.Key.x == 0).Select(c => c.Value).ToList());
                        var otherRightEdge = string.Join(null, otherTile2.Where(v => v.Key.x == 9).Reverse().Select(c => c.Value).ToList());

                        // put that tile in tilemap, in that orientation, at the right coord.
                        if (otherTopEdge.Equals(currentTileEdges["bottom"]))
                        {
                            tileMap2.Add((currentTileCoord.x, currentTileCoord.y + 1), (otherTile.Key, otherTile2));
                            queue.Enqueue(adjacents.Single(kvp => kvp.Key == otherTile.Key));
                            break;
                        }
                        else if (otherBottomEdge.Equals(currentTileEdges["top"]))
                        {
                            tileMap2.Add((currentTileCoord.x, currentTileCoord.y - 1), (otherTile.Key, otherTile2));
                            queue.Enqueue(adjacents.Single(kvp => kvp.Key == otherTile.Key));
                            break;
                        }
                        else if (otherRightEdge.Equals(currentTileEdges["left"]))
                        {
                            tileMap2.Add((currentTileCoord.x - 1, currentTileCoord.y), (otherTile.Key, otherTile2));
                            queue.Enqueue(adjacents.Single(kvp => kvp.Key == otherTile.Key));
                            break;
                        }
                        else if (otherLeftEdge.Equals(currentTileEdges["right"]))
                        {
                            tileMap2.Add((currentTileCoord.x + 1, currentTileCoord.y), (otherTile.Key, otherTile2));
                            queue.Enqueue(adjacents.Single(kvp => kvp.Key == otherTile.Key));
                            break;
                        }
                        else 
                        {
                            continue;
                        }
                    }
                }
            }



            // now we have the map tiles in place, we need to perform the operations on them, and 
            // stitch them into the big map.
            var x = 0;
            var y = 0;

            var iMin = tileMap2.Keys.Select(v => v.y).Min(); 
            var iMax = tileMap2.Keys.Select(v => v.y).Max(); 
            var jMin = tileMap2.Keys.Select(v => v.x).Min(); 
            var jMax = tileMap2.Keys.Select(v => v.x).Max();

            var bigPicture = new Dictionary<(int x, int y), char>();

            for (int i = iMin; i <= iMax; i++)
            {
                for (int j = jMin; j <= jMax; j++)
                {
                    var tile = tileMap2.Single(t => t.Key == (j, i)).Value.tileValues;

                    for (int k = 0; k < 10; k++)
                    {
                        for (int t = 0; t < 10; t++)
                        {
                            // bigPicture[(k + x, t + y)] = tile[(k, t)];
                            bigPicture[(k + x, t + y)] = tile[(k, t)];
                        }
                    }
                    x += 10;
                }
                y += 10;
                x = 0;
            }

            // PrintDictToFile(bigPicture);

            // remove the edges
            x = 0;
            y = 0;

            iMax = bigPicture.Select(c => c.Key.y).Max();
            jMax = bigPicture.Select(c => c.Key.x).Max();

            var edgeCutBigPicture = new Dictionary<(int x, int y), char>();

            for (int i = 0; i < iMax; i++)
            {
                if (i % 10 != 9 && i % 10 != 0)
                {
                    for (int j = 0; j < jMax; j++)
                    {
                        if (j % 10 != 9 && j % 10 != 0)
                        {
                            edgeCutBigPicture[(x, y)] = bigPicture[(j, i)];
                            x ++;
                        }
                    }
                    x = 0;
                    y ++;
                }
            }

            // PrintDictToFile(edgeCutBigPicture, 100);

            iMax = edgeCutBigPicture.Select(c => c.Key.y).Max();

            var seaMonsterTop    = new Regex("                  # ".Replace(' ', '.'));
            var seaMonsterMiddle = new Regex("#    ##    ##    ###".Replace(' ', '.'));
            var seaMonsterBottom = new Regex(" #  #  #  #  #  #   ".Replace(' ', '.'));
            var seaMonsterCount = 0;

            foreach(var operation in operations)
            {
                var edgeCutBigPicture2 = Rotate(edgeCutBigPicture, operation, 'x');

                // scan the middle rows for the middle of the seamonster, note index
                // scan adjacent rows at that index for those bits
                // add one to count if there is one
                for(int i = 1; i < iMax - 1; i++)
                {
                    var prevLine = new string(edgeCutBigPicture2.Where(e => e.Key.y == i-1).Select(e => e.Value).ToArray());
                    var line = new string(edgeCutBigPicture2.Where(e => e.Key.y == i).Select(e => e.Value).ToArray());
                    var nextLine = new string(edgeCutBigPicture2.Where(e => e.Key.y == i+1).Select(e => e.Value).ToArray());

                    // regex match the middle
                    var matches = seaMonsterMiddle.Matches(line);
                    Match prevMatch = null;
                    foreach(Match match in matches)
                    {
                        if (prevMatch != null && match.Index < prevMatch.Index + prevMatch.Length){
                            // just in case of weird overlaps
                            continue;
                        }

                        if (seaMonsterTop.Match(prevLine.Substring(match.Index, match.Length)).Success &&
                            seaMonsterBottom.Match(nextLine.Substring(match.Index, match.Length)).Success)
                            {
                                prevMatch = match;
                                seaMonsterCount ++;
                            }
                    }
                }

            }
            var seaMonsterHashes = 15 * seaMonsterCount;
            var totalHashes = edgeCutBigPicture.Count(c => c.Value == '#');

            var ans = totalHashes - seaMonsterHashes;

            Console.WriteLine(ans);
        }

        public static void PrintDict(IDictionary<(int x, int y), char> dict, int gapsOn = 10){
            var maxX = dict.Keys.Select(k => k.x).Max();
            var maxY = dict.Keys.Select(k => k.y).Max();

            for (int i = 0; i <= maxY; i++)
            {
                if (i % gapsOn == 0)
                {
                    Console.WriteLine();
                }
                for (int j = 0; j <= maxX; j++)
                {
                    if (j % gapsOn == 0){
                        Console.Write("  ");
                    }
                    Console.Write($"{dict[(j, i)]} ");
                }
                Console.WriteLine();
            }
        }

        public static void PrintDictToFile(IDictionary<(int x, int y), char> dict, int gapsOn = 10){
            var maxX = dict.Keys.Select(k => k.x).Max();
            var maxY = dict.Keys.Select(k => k.y).Max();
            
            var sb = new StringBuilder();

            for (int i = 0; i <= maxY; i++)
            {
                if (i % gapsOn == 0)
                {
                    sb.Append(Environment.NewLine);
                }
                for (int j = 0; j <= maxX; j++)
                {
                    if (j % gapsOn == 0){
                        sb.Append("  ");
                    }
                    sb.Append($"{dict[(j, i)]} ");
                }
                sb.Append(Environment.NewLine);
            }

            File.WriteAllText("./output.txt", sb.ToString());
        }

        public static IDictionary<(int x, int y), char> Rotate(IDictionary<(int x, int y), char> original, D4_element op, char reflectionAxis)
        {
            if (op == D4_element.E){
                return original;
            }

            var maxX = original.Select(c => c.Key.x).Max();
            var maxY = original.Select(c => c.Key.y).Max();

            var d4str = op.ToString();

            var newDict = original.ToDictionary(o => o.Key, o => o.Value);
            var memDict = new Dictionary<(int x, int y), char>();
            
            while(d4str.Any() && d4str[0] == 'R')
            {
                for (int i = 0; i <= maxY; i++)
                {
                    for (int j = maxX; j >= 0; j--)
                    {
                        memDict[(i, maxX - j)] = newDict[(j, i)];
                    }
                }

                newDict = memDict.ToDictionary(k => k.Key, k => k.Value);
                d4str = d4str.Substring(1);
            }

            if (d4str.Equals("F")){
                if (reflectionAxis == 'x')
                {
                    for(int i = 0; i <= maxY; i++)
                    {
                        for(int j = 0; j <= maxX; j++)
                        {
                            memDict[(i, j)] = newDict[(i, maxX -j)];
                        }
                    }
                }
                else if (reflectionAxis == 'y')
                {
                    for(int i = 0; i <= maxY; i++)
                    {
                        for(int j = 0; j <= maxX; j++)
                        {
                            memDict[(i, j)] = newDict[(maxY - i, j)];
                        }
                    }
                }

                newDict = memDict.ToDictionary(k => k.Key, k => k.Value);
            }

            return newDict;
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
