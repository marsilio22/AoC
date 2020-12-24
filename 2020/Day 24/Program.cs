using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Day_24
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadLines("./input.txt");

            var coords = new Dictionary<(int x, int y), char>();

            foreach (var line in lines)
            {
                string direction = string.Empty;
                (int x, int y) coord = (0,0);

                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i] == 'e' || line[i] == 'w')
                    {
                        direction = line[i].ToString();
                    }
                    else
                    {
                        direction = line[i].ToString() + line[i+1].ToString();
                        i++;
                    }

                    switch(direction)
                    {
                        case "e":
                            coord = (coord.x + 1, coord.y);
                            break;
                        case "w":
                            coord = (coord.x - 1, coord.y);
                            break;
                        case "ne":
                            coord = (coord.x, coord.y + 1);
                            break;
                        case "sw":
                            coord = (coord.x, coord.y - 1);
                            break;
                        case "se":
                            coord = (coord.x + 1, coord.y - 1);
                            break;
                        case "nw":
                            coord = (coord.x - 1, coord.y + 1);
                            break;
                    }
                }

                if (!coords.TryGetValue(coord, out char val))
                {
                    val = ' ';
                    coords.Add(coord, val);
                }

                coords[coord] = val == ' ' ? '#': ' ';
            }

            var ans = coords.Count(c => c.Value == '#');
            Console.WriteLine(ans);

            // part 2
            var turns = 100;
            var hexNeighbours = new List<(int x, int y)>{
                (1, 0),
                (0, 1),
                (-1, 1),
                (-1, 0),
                (0, -1),
                (1, -1)
            };

            for (int i = 0; i < turns; i++)
            {
                var next = coords.ToDictionary(c => c.Key, c => c.Value);

                // first add any missing neighbours to the dict
                foreach(var coord in coords)
                {
                    var neighbours = hexNeighbours.Select(c => (coord.Key.x + c.x, coord.Key.y + c.y));
                    foreach(var n in neighbours)
                    {
                        if (!next.TryGetValue(n, out char val))
                        {
                            next.Add(n, ' ');
                        }
                    }
                }

                var next2 = next.ToDictionary(c => c.Key, c => c.Value);

                foreach(var coord in next)
                {
                    var countBlack = 0;
                    var neighbours = hexNeighbours.Select(c => (coord.Key.x + c.x, coord.Key.y + c.y));
                    foreach(var n in neighbours)
                    {
                        if (next.TryGetValue(n, out char val))
                        {
                            countBlack += (val == '#' ? 1 : 0);
                        }
                    }

                    if (coord.Value == ' ' && countBlack == 2)
                    {
                        next2[coord.Key] = '#';
                    }
                    else if (coord.Value == '#' && (countBlack == 0 || countBlack > 2))
                    {
                        next2[coord.Key] = ' ';
                    }
                }

                coords = next2.Where(c => c.Value == '#').ToDictionary(c => c.Key, c => c.Value);
            }

            ans = coords.Count(c => c.Value == '#');
            Console.WriteLine(ans);

            PrintDict(coords);
        }

        static void PrintDict(Dictionary<(int x, int y), char> coords)
        {
            var maxI = coords.Select(c => c.Key.y).Max();
            var maxJ = coords.Select(c => c.Key.x).Max();
            var minI = coords.Select(c => c.Key.y).Min();
            var minJ = coords.Select(c => c.Key.x).Min();
            var sb = new StringBuilder();
            
            for (int i = minI; i <= maxI; i++)
            {
                if (i % 2 == 0)
                {
                    sb.Append("  ");
                }
                for (int j = minJ; j <= maxJ; j++)
                {
                    if (coords.ContainsKey((i, j)))
                    {
                        sb.Append(" " + coords[(i, j)] + " ");
                    }
                    else
                    {
                        sb.Append("   ");
                    }
                }
                sb.Append(Environment.NewLine);
            }

            File.WriteAllText("./output.txt", sb.ToString());
        }
    }
}
