using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Day_18
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("./input.txt");

            // var lines = new [] {".#.#...|#.",
            // ".....#|##|",
            // ".|..|...#.",
            // "..|#.....#",
            // "#.#|||#|#|",
            // "...#.||...",
            // ".|....|...",
            // "||...#|.#|",
            // "|.||||..|.",
            // "...#.|..|."};
            
            var coords = new List<Coordinate>();

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                for (int j = 0; j < line.Length; j++)
                {
                    coords.Add(new Coordinate{ X = j, Y = i, Contents = line[j]});
                }
            }

            foreach(var coord in coords){
                coord.AdjacentCoordinates = coords.Where(c => 
                    Math.Abs(c.X - coord.X) == 1 && Math.Abs(c.Y - coord.Y) <= 1 || 
                    Math.Abs(c.Y - coord.Y) == 1 && Math.Abs(c.X - coord.X) <= 1).ToList();
            }   

            Dictionary<int, int> generationResults = new Dictionary<int, int>();

            Dictionary<int, List<Coordinate>> gens = new Dictionary<int, List<Coordinate>>();

            for (int i = 0; i < 1000; i++)
            {
                gens[i] = coords;
                var res = coords.Count(c => c.Contents == '|') * (coords.Count(c => c.Contents == '#') * 10);
                generationResults[i] = res;

                if(generationResults.Count(g => g.Value == res) >= 2){
                    foreach(var gen in generationResults.Where(g => g.Value == res)){
                        Print(gens[gen.Key], $"./ouput_gen{gen.Key}.txt");
                        Console.WriteLine($"In Generation {i}, printed generation {gen.Key} to file");
                    }
                }

                var nextGen = new List<Coordinate>();

                foreach(var coord in coords){
                    nextGen.Add(new Coordinate
                    {
                        X = coord.X,
                        Y = coord.Y,
                        Contents = coord.NextGen(),
                    });
                }

                coords = nextGen;

                foreach(var coord in coords)
                {
                    coord.AdjacentCoordinates = coords.Where(c => 
                        Math.Abs(c.X - coord.X) == 1 && Math.Abs(c.Y - coord.Y) <= 1 || 
                        Math.Abs(c.Y - coord.Y) == 1 && Math.Abs(c.X - coord.X) <= 1).ToList();
                }
            }

            Console.WriteLine(coords.Count(c => c.Contents == '|') * coords.Count(c => c.Contents == '#'));
        }

            public static void Print(List<Coordinate> coords, string filename){
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i <= coords.Max(x => x.Y); i++)
                {
                    for (int j = 0; j <= coords.Max(y => y.X); j++){
                        sb.Append(coords.First(c => c.X == j && c.Y == i).Contents);
                    }
                    sb.AppendLine();
                }

                File.WriteAllText(filename, sb.ToString());
            }
    }

    class Coordinate
    {
        public int X { get; set; }
        public int Y { get; set; } 
        public char Contents { get; set; }

        public List<Coordinate> AdjacentCoordinates {get; set;}
        
        public override bool Equals(object other){
            if (other as Coordinate != null){
                return this.Equals((Coordinate)other);
            }
            else{
                return false;
            }
        }

        public bool Equals(Coordinate other)
        {
            return other.X == this.X && other.Y == this.Y && other.Contents == this.Contents;
        }

        public char NextGen()
        {
            switch (this.Contents)
            {
                case '.':
                    if (AdjacentCoordinates.Count(c => c.Contents == '|') >= 3)
                    {
                        return '|';
                    }
                    break;
                case '|':
                    if (AdjacentCoordinates.Count(c => c.Contents == '#') >= 3)
                    {
                        return '#';
                    }
                    break;
                case '#':
                    if (AdjacentCoordinates.Count(c => c.Contents == '#') >= 1 && AdjacentCoordinates.Count(c => c.Contents == '|') >= 1)
                    {
                        return '#';
                    }
                    else
                    {
                        return '.';
                    }
                default:
                    throw new Exception("Oops");
            }
            return Contents;
        }
    }
}
