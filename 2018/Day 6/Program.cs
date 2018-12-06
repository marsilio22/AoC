using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_6
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines("./input.txt");

            List<(int x, int y)> coordinates = 
                lines.Select(line => (int.Parse(line.Split(", ")[0]), int.Parse(line.Split(", ")[1]))).ToList();
            Dictionary<(int x, int y), char> namedCoordinates = new Dictionary<(int x, int y), char>();

            char c = 'A';

            foreach(var coordinate in coordinates){
                namedCoordinates.Add(coordinate, c);
                c++;
            }

            Part1(coordinates, namedCoordinates);

            // Part 2

            Part2(coordinates, namedCoordinates);

        }

        public static void Part1(List<(int x, int y)> coordinates, Dictionary<(int x, int y), char> namedCoordinates){
            
            var results = new Dictionary<(int x, int y), char>();

            foreach (var coordinate in namedCoordinates)
            {
                results.Add(coordinate.Key, coordinate.Value);
            }

            var biggestX = coordinates.Select(ch => ch.x).Max();
            var biggestY = coordinates.Select(ch => ch.y).Max();

            for (int i = 0; i < biggestX; i++)
            {
                for (int j=0; j < biggestY; j++)
                {
                    if (!results.ContainsKey((i, j))){
                        var distances = namedCoordinates.Select( co => (co.Value, ManhattanDistance(co.Key, (i, j)))).ToDictionary(tup => tup.Item1, tup => tup.Item2);
                        var closest = distances.Aggregate((x, y) => x.Value < y.Value ? x : y);

                        if (distances.Values.Count(d => d == closest.Value) == 1)
                        {
                            results[(i, j)] = closest.Key;
                        }
                        else
                        {
                            results[(i, j)] = '.';
                        }
                    }
                }
            }

            List<char> edgeValues = new List<char>();

            for(int i = 0; i < biggestX; i++){
                if (!edgeValues.Contains(results[(i, 0)]))
                {
                    edgeValues.Add(results[(i, 0)]);
                }

                if (!edgeValues.Contains(results[(i, biggestY - 1)]))
                {
                    edgeValues.Add(results[(i, biggestY - 1)]);
                }
            }

            
            for(int i = 0; i < biggestY; i++){
                if (!edgeValues.Contains(results[(0, i)]))
                {
                    edgeValues.Add(results[(0, i)]);
                }

                if (!edgeValues.Contains(results[(biggestX - 1, i)]))
                {
                    edgeValues.Add(results[(biggestX - 1, i)]);
                }
            }

            var validCoordinates = namedCoordinates.Where(co => !edgeValues.Contains(co.Value));

            foreach(var coord in validCoordinates){
                Console.WriteLine($"Coord {coord.Value} = {coord.Key}: Count = {results.Count(r => r.Value == coord.Value)}");
            }
        }

        public static void Part2(List<(int x, int y)> coordinates, Dictionary<(int x, int y), char> namedCoordinates){
            
            var results = new Dictionary<(int x, int y), int>();

            var biggestX = coordinates.Select(ch => ch.x).Max();
            var biggestY = coordinates.Select(ch => ch.y).Max();

            for (int i = 0; i < biggestX; i++)
            {
                for (int j=0; j < biggestY; j++)
                {
                    if (!results.ContainsKey((i, j))){
                        var distances = namedCoordinates.Select( co => (co.Value, ManhattanDistance(co.Key, (i, j)))).ToDictionary(tup => tup.Item1, tup => tup.Item2);
                        var sum = distances.Sum(x => x.Value);

                        results[(i, j)] = sum;
                    }
                }
            }

            List<char> edgeValues = new List<char>();

            // for(int i = 0; i < biggestX; i++){
            //     if (!edgeValues.Contains(results[(i, 0)]))
            //     {
            //         edgeValues.Add(results[(i, 0)]);
            //     }

            //     if (!edgeValues.Contains(results[(i, biggestY - 1)]))
            //     {
            //         edgeValues.Add(results[(i, biggestY - 1)]);
            //     }
            // }

            
            // for(int i = 0; i < biggestY; i++){
            //     if (!edgeValues.Contains(results[(0, i)]))
            //     {
            //         edgeValues.Add(results[(0, i)]);
            //     }

            //     if (!edgeValues.Contains(results[(biggestX - 1, i)]))
            //     {
            //         edgeValues.Add(results[(biggestX - 1, i)]);
            //     }
            // }

            // var validCoordinates = namedCoordinates.Where(co => !edgeValues.Contains(co.Value));

            Console.WriteLine($"number < 10000 = {results.Count(r => r.Value < 10000)}");
            
        }

        public static int ManhattanDistance((int x, int y) first, (int x, int y) second){
            var ans = Math.Abs(first.x - second.x) + Math.Abs(first.y - second.y);
            return ans;
        }
    }
}
