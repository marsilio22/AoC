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

            var adjacents = new Dictionary<int, ICollection<(int tileId, string direction, D4 op)>>();


            foreach(var tile in edges)
            {
                adjacents.Add(tile.Key, new List<(int, string, D4)>());

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

                        D4 requiredOperation = new D4(D4_element.E);

                        // imagine we are rotating the tile anticlockwise so that the edge of interest
                        // is on the bottom

                        // Now. The operation which has to happen to the other tile, is 
                        // the inverse of the operation rotating this tile's edge to the bottom, 
                        // multiplied by whatever it needs to do itself to get its edge to the top.

                        // e.g. if Tile 1234's right hand edge matches the right hand edge of tile 5432. 
                        // - To rotate 1234's right to bottom we do 3 anticlockwise rotations.
                        // - The inverse of this is 1 anticlockwise rotation.
                        // - To rotate 5432's right to the top we do 1 anticlockwise rotation
                        // - The product of the inverse op to 1234 and the 5432 op is two anticlockwise rotations
                        // - It is seen that this equates to the operation needed for 5432's right edge to end up on the left.

                        // so the below will set the INVERSE of the operation required to get the edge to the bottom
                        switch(edge.Key)
                        {
                            case "left":
                                requiredOperation = new D4(D4_element.RRR);
                                break;
                            case "right":
                                requiredOperation = new D4(D4_element.R);
                                break;
                            case "top":
                                requiredOperation = new D4(D4_element.RR);
                                break;
                            case "bottom":
                                requiredOperation = new D4(D4_element.E);
                                break;
                        }

                        // now combine (POST MULTIPLY AS D4 IS NOT COMMUTATIVE) with the required operation for the other tile
                        
                        //----------------------------------------
                        // BECAUSE THE TOP EDGE OF ONE IS MATCHED TO THE BOTTOM OF THE OTHER
                        // THEY ARE READ (CLOCKWISE) AS REFLECTIONS OF EACH OTHER ANYWAY (CLOCKWISE), SO IF THEY MATCH UP 
                        // ALREADY, THEN WE NEED TO REFLECT
                        //----------------------------------------
                        switch(otherTileEdge.Key)
                        {
                            case "left":
                                // rotate 270 anticlockwise
                                // requiredOperation *= new D4(D4_element.RRR);
                                requiredOperation *= new D4(D4_element.RRRF);
                                break;
                            case "right":
                                // rotate 90 anticlockwise
                                // requiredOperation *= new D4(D4_element.R);
                                requiredOperation *= new D4(D4_element.RF);
                                break;
                            case "top":
                                // do nothing, bottom -> top is correct already
                                // requiredOperation *= new D4(D4_element.E);
                                requiredOperation *= new D4(D4_element.RF);
                                break;
                            case "bottom":
                                // rotate 180 anticlockwise
                                // requiredOperation *= new D4(D4_element.RR);
                                requiredOperation *= new D4(D4_element.RRF);
                                break;
                            case "left$":
                                // Rotate 270 anticlockwise and reflect
                                // requiredOperation *= new D4(D4_element.RRRF);
                                requiredOperation *= new D4(D4_element.RRR);
                                break;
                            case "right$":
                                // Rotate 90 anticlockwise and reflect
                                // requiredOperation *= new D4(D4_element.RF);
                                requiredOperation *= new D4(D4_element.R);
                                break;
                            case "top$":
                                // just reflect
                                // requiredOperation *= new D4(D4_element.F);
                                requiredOperation *= new D4(D4_element.E);
                                break;
                            case "bottom$":
                                // Rotate 180 anticlockwise and reflect
                                // requiredOperation *= new D4(D4_element.RRF);
                                requiredOperation *= new D4(D4_element.RR);
                                break;
                        }

                        Console.WriteLine($"Tile {tile.Key} is adjacent to tile {otherTile}, {edge.Key} to {otherTileEdge.Key}, tile {otherTile} requires the operation {requiredOperation}");
                        // adjacents[tile.Key].Add((otherTile, $"{edge.Key} to {otherTileEdge.Key}, {requiredOperation}"));
                        adjacents[tile.Key].Add((otherTile, edge.Key, requiredOperation));
                   }
                }
            }

            var corners = adjacents.Where(c => c.Value.Count == 2).Select(c => c.Key).ToList();

            // Part 1
            Console.WriteLine((long)corners[0] * (long)corners[1] * (long)corners[2] * (long)corners[3]);

            // need to keep track of whether or not the tile we're looking at right now has been reflected or not?
            var tileMap = new Dictionary<int, ((int x, int y) coord, D4 operationForOrientation, char reflectionAxis)>();
            
            // pick a random tile in the middle. 
            // This tile will determine the orientation by which we assemble the tiles.
            var t1 = adjacents.First(c => c.Value.Count == 4); 

            tileMap.Add(t1.Key, ((0, 0), new D4(D4_element.E), 'a'));

            var tilesToDo = new Queue<KeyValuePair<int, ICollection<(int tileId, string facing, D4 operation)>>>();
            tilesToDo.Enqueue(t1);

            while(tilesToDo.Any())
            {
                var tile = tilesToDo.Dequeue();
                var tileDetails = tileMap[tile.Key];

                // Take the tiles adjacent to each of the sides and rotate them (or just record the rotation required)
                // so that they are in the correct orientation to attach
                // Then add them to the total map, recording the combination of rotations needed to get them right
                var feVar = tile.Value.Where(v => !tileMap.ContainsKey(v.tileId));
                foreach (var t in feVar)
                {
                    // TODO need to keep track of whether the tile this tile is adj to has been reflected or not. probably one of the operations...
                    // var currentFacing = t.facing;

                    // if (tileDetails.reflectionAxis == 'x' && t.facing.Equals("top"))
                    // {
                    //     currentFacing = "bottom";
                    // }
                    // else if (tileDetails.reflectionAxis == 'x' && t.facing.Equals("bottom"))
                    // {
                    //     currentFacing = "top";
                    // }
                    // else if (tileDetails.reflectionAxis == 'y' && t.facing.Equals("left"))
                    // {
                    //     currentFacing = "right";
                    // }
                    // else if (tileDetails.reflectionAxis == 'y' && t.facing.Equals("right"))
                    // {
                    //     currentFacing = "left";
                    // }

                    var reflections = new List<D4_element>{D4_element.F, D4_element.RF, D4_element.RRF, D4_element.RRRF};

                    var newFacing = WorkOutFacing(t.facing, t.operation);
                    (int x, int y) nextCoord = (0, 0);
                    char reflectionAxis;

                    switch(newFacing){
                        case "top":
                            nextCoord = (tileDetails.coord.x, tileDetails.coord.y - 1);
                            if (reflections.Contains(t.operation.Value))
                            {
                                reflectionAxis = 'y';
                            }
                            else
                            {
                                reflectionAxis = 'a';
                            }
                            break;
                        case "left":
                            nextCoord = (tileDetails.coord.x - 1, tileDetails.coord.y);
                            if (reflections.Contains(t.operation.Value))
                            {
                                reflectionAxis = 'x';
                            }
                            else
                            {
                                reflectionAxis = 'a';
                            }
                            break;
                        case "right":
                            nextCoord = (tileDetails.coord.x + 1, tileDetails.coord.y);
                            if (reflections.Contains(t.operation.Value))
                            {
                                reflectionAxis = 'x';
                            }
                            else
                            {
                                reflectionAxis = 'a';
                            }
                            break;
                        case "bottom":
                            nextCoord = (tileDetails.coord.x, tileDetails.coord.y + 1);
                            if (reflections.Contains(t.operation.Value))
                            {
                                reflectionAxis = 'y';
                            }
                            else
                            {
                                reflectionAxis = 'a';
                            }
                            break;
                        default:
                            throw new Exception("Shouldn't happen");
                    }

                    // if (tileMap.Any(t => t.Value.coord == nextCoord))
                    // {
                    //     Console.WriteLine();
                    // }


                    tileMap.Add(t.tileId, (nextCoord, tileDetails.operationForOrientation * t.operation, reflectionAxis));
                    var tileToDoToAdd = adjacents.First(f => f.Key == t.tileId);

                    if (reflectionAxis == 'x')
                    {
                        (int tileId, string direction, D4 op) memSwap = (-1, "", new D4(D4_element.E));
                        // swap T and B
                        if (tileToDoToAdd.Value.Any(v => v.direction.Equals("top")))
                        {
                            memSwap = tileToDoToAdd.Value.Single(v => v.direction.Equals("top"));
                            tileToDoToAdd.Value.Remove(memSwap);
                        }

                        if (tileToDoToAdd.Value.Any(v => v.direction.Equals("bottom")))
                        {
                            var valToTop = tileToDoToAdd.Value.Single(v => v.direction.Equals("bottom"));
                            tileToDoToAdd.Value.Add((valToTop.tileId, "top", valToTop.op));
                            tileToDoToAdd.Value.Remove(valToTop);
                        }

                        if (memSwap.tileId != -1){
                            tileToDoToAdd.Value.Add((memSwap.tileId, "bottom", memSwap.op));
                        }
                    }
                    if (reflectionAxis == 'y')
                    {
                        // swap L and R
                        (int tileId, string direction, D4 op) memSwap = (-1, "", new D4(D4_element.E));
                        if (tileToDoToAdd.Value.Any(v => v.direction.Equals("left")))
                        {
                            memSwap = tileToDoToAdd.Value.Single(v => v.direction.Equals("left"));
                            tileToDoToAdd.Value.Remove(memSwap);
                        }

                        if (tileToDoToAdd.Value.Any(v => v.direction.Equals("right")))
                        {
                            var valToTop = tileToDoToAdd.Value.Single(v => v.direction.Equals("right"));
                            tileToDoToAdd.Value.Add((valToTop.tileId, "left", valToTop.op));
                            tileToDoToAdd.Value.Remove(valToTop);
                        }

                        if (memSwap.tileId != -1){
                            tileToDoToAdd.Value.Add((memSwap.tileId, "right", memSwap.op));
                        }
                    }

                    tilesToDo.Enqueue(tileToDoToAdd);
                }
            }


            var firstTile = adjacents.First();
            
            var tileMap2 = new Dictionary<(int x, int y), (int tileId, IDictionary<(int x, int y), char> tileValues)>();
            tileMap2.Add((0, 0), (firstTile.Key, tiles[firstTile.Key]));

            var queue = new Queue<KeyValuePair<int, ICollection<(int tileId, string direction, D4 op)>>>();

            var operations = new List<D4>
            {
                new D4(D4_element.E),
                new D4(D4_element.R),
                new D4(D4_element.RR),
                new D4(D4_element.RRR),
                new D4(D4_element.F),
                new D4(D4_element.RF),
                new D4(D4_element.RRF),
                new D4(D4_element.RRRF)
            };

            queue.Enqueue(firstTile);

            while(queue.Any())
            {
                // get this tile
                var currentTile = queue.Dequeue();
                var tile = tileMap2.Single(t => t.Value.tileId == currentTile.Key);
                var adjTileIds = currentTile.Value.Select(t => t.tileId).ToList();

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

            var iMin = tileMap2.Keys.Select(v => v.y).Min(); // probably -6
            var iMax = tileMap2.Keys.Select(v => v.y).Max(); // probably 6
            var jMin = tileMap2.Keys.Select(v => v.x).Min(); // as above
            var jMax = tileMap2.Keys.Select(v => v.x).Max();

            var bigPicture = new Dictionary<(int x, int y), char>();

            // for (int i = iMin; i <= iMax; i++)
            // {
            //     for (int j = jMin; j <= jMax; j++)
            //     {
            //         if (tileMap.Any(t => t.Value.coord == (j, i)))
            //         {
            //             Console.Write('#');
            //         }
            //         else
            //         {
            //             Console.Write('.');
            //         }
            //     }
            //     Console.WriteLine();
            // }

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

            // var coords = tileMap.Select(c => c.Value.coord).Distinct();

            // foreach(var coord in coords){
            //     Console.WriteLine($"for coord ({coord.x}, {coord.y}), there are {tileMap.Count(c => c.Value.coord == coord)} tiles");
            // }

            // Console.WriteLine();

            // var emptyDict = new Dictionary<(int, int), char>();
            // for (int i = 0; i < 10; i++){
            //     for (int j = 0; j < 10; j++)
            //         emptyDict[(i, j)] = '.';
            // }








            // for (int i = iMin; i <= iMax; i++)
            // {
            //     for (int j = jMin; j <= jMax; j++)
            //     {
            //         var tile = tileMap.Single(t => t.Value.coord == (j, i));

            //         // rotate the tile in the (j, i) coord of tilemap under it's rotation
            //         // read out the values from that tile into bigPicture(x,y)

            //         var values = Rotate(tiles[tile.Key], tile.Value.operationForOrientation, tile.Value.reflectionAxis);

            //         for (int k = 0; k < 10; k++)
            //         {
            //             for (int t = 0; t < 10; t++)
            //             {
            //                 bigPicture[(k + x, t + y)] = values[(k, t)];
            //             }
            //         }
            //         x += 10;
            //     }
            //     y += 10;
            //     x = 0;
            // }

            PrintDict(bigPicture);
            PrintDictToFile(bigPicture);

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

            PrintDictToFile(edgeCutBigPicture, 100);

            iMax = edgeCutBigPicture.Select(c => c.Key.y).Max();

            var seaMonsterTop    = new Regex("                  # ".Replace(' ', '.'));
            var seaMonsterMiddle = new Regex("#    ##    ##    ###".Replace(' ', '.'));
            var seaMonsterBottom = new Regex(" #  #  #  #  #  #   ".Replace(' ', '.'));
            var seaMonsterCount = 0;

            foreach(var operation in operations)
            {
                var edgeCutBigPicture2 = Rotate(edgeCutBigPicture, operation, 'x');
                PrintDictToFile(edgeCutBigPicture2, 100);

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

        public static IDictionary<(int x, int y), char> Rotate(IDictionary<(int x, int y), char> original, D4 op, char reflectionAxis)
        {
            if (op.Value == D4_element.E){
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

        public static string WorkOutFacing(string currentFacing, D4 operation/*, char reflectionAxis*/)
        {
            var facings = new List<string>{"top", "left", "bottom", "right"};
            // Func<bool> facingFlip = () => reflectionAxis == 'x' && currentFacing.Equals("top")|| currentFacing.Equals("bottom")
            //                            || reflectionAxis == 'y' && currentFacing.Equals("left")|| currentFacing.Equals("right");
            switch(operation.Value)
            {
                case D4_element.E:
                case D4_element.F:
                    return currentFacing;
                    // if (facingFlip()){
                    //     return facings[(facings.IndexOf(currentFacing) + 2) % 4];
                    // }
                    // return currentFacing;
                case D4_element.R:
                case D4_element.RF:
                    return facings[(facings.IndexOf(currentFacing) + 1) % 4];
                    //currentFacing = facings[(facings.IndexOf(currentFacing) + 1) % 4];
                    // if (facingFlip()){
                    //     return facings[(facings.IndexOf(currentFacing) + 2) % 4];
                    // }
                    // return currentFacing;
                case D4_element.RR:
                case D4_element.RRF:
                    return facings[(facings.IndexOf(currentFacing) + 2) % 4];
                    //currentFacing = facings[(facings.IndexOf(currentFacing) + 2) % 4];
                    // if (facingFlip()){
                    //     return facings[(facings.IndexOf(currentFacing) + 2) % 4];
                    // }
                    // return currentFacing;
                case D4_element.RRR:
                case D4_element.RRRF:
                    return facings[(facings.IndexOf(currentFacing) + 3) % 4];
                    //currentFacing = facings[(facings.IndexOf(currentFacing) + 3) % 4];
                    // if (facingFlip()){
                    //     return facings[(facings.IndexOf(currentFacing) + 2) % 4];
                    // }
                    // return currentFacing;
                default:
                    throw new Exception("shouldn't happen");
            }
        }
    }

    public class D4 
    {
        public D4(D4_element val)
        {
            this.Value = val;
        }

        public D4_element Value {get;set;}

        private static Dictionary<(D4_element, D4_element), D4_element> multiplicationTable = new Dictionary<(D4_element, D4_element), D4_element>
        {
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
        };

        public static D4 operator * (D4 a, D4 b)
        {
            if (a.Value == D4_element.E){
                return b;
            }

            if (b.Value == D4_element.E)
            {
                return a;
            }

            return new D4(multiplicationTable[(a.Value, b.Value)]);
        }

        public override string ToString()
        {
            return this.Value.ToString();
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
