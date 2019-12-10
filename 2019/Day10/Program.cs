using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day10 {
    class Program {
        static void Main (string[] args) {
            var input = File.ReadAllLines ("./input.txt");

            // exanple solution 33 at 5, 8
            // input = new []{"......#.#.",
            //                "#..#.#....",
            //                "..#######.",
            //                ".#.#.###..",
            //                ".#..#.....",
            //                "..#....#.#",
            //                "#..#....#.",
            //                ".##.#..###",
            //                "##...#..#.",
            //                ".#....####"};

            Dictionary < (int x, int y), char > map = new Dictionary < (int x, int y), char > ();

            ICollection < (int x, int y) > uniqueVectors = 
                (from x in Enumerable.Range(1, Math.Max(input.Length, input[0].Length))
                 from y in Enumerable.Range(1, Math.Max(input.Length, input[0].Length))
                 where x != y && (x == 1 || y == 1 || Gcd(x, y) == 1)
                 select (x, y)).ToList ();

            uniqueVectors = uniqueVectors.Prepend ((1, 1)).Prepend ((0, 1)).Prepend ((1, 0)).ToList();

            for (int y = 0; y < input.Length; y++) {
                var line = input[y];
                for (int x = 0; x < line.Length; x++) {
                    map[(x, y)] = line[x];
                }
            }

            var asteroids = map.Where (m => m.Value.Equals ('#')).ToDictionary (d => d.Key, d => d.Value);
            var asteroidsVisibleToAsteroid = new Dictionary<(int x, int y), List<(int x, int y)>>();

            int mostAsteroids = int.MinValue;
            (int x, int y) bestAsteroid = (0, 0);
            foreach (var asteroid in asteroids) {
                asteroidsVisibleToAsteroid[asteroid.Key] = new List<(int x, int y)>();

                List < (int x, int y) > doneCoordinates = new List < (int x, int y) > ();

                foreach ((int x, int y) primeCoordVector in uniqueVectors) {
                    (int x, int y) coordinate = primeCoordVector;

                    for (int direction = 0; direction < 4; direction++) {
                        var numberOfSteps = 1;
                        coordinate = GetDirection (direction, primeCoordVector, numberOfSteps, asteroid.Key);

                        if (doneCoordinates.Contains(coordinate)){
                            continue;
                        }
                        doneCoordinates.Add(coordinate);

                        while (map.ContainsKey (coordinate)) {
                            if (asteroids.ContainsKey (coordinate)) {
                                asteroidsVisibleToAsteroid[asteroid.Key].Add(coordinate);
                                numberOfSteps = 1;
                                break;
                            }

                            numberOfSteps += 1;
                            coordinate = GetDirection (direction, primeCoordVector, numberOfSteps, asteroid.Key);
                        }
                    }
                }
                
                Console.WriteLine($"Asteroid at {asteroid.Key.x}, {asteroid.Key.y}: {asteroidsVisibleToAsteroid[asteroid.Key].Distinct().Count()}");

                if (asteroidsVisibleToAsteroid[asteroid.Key].Count > mostAsteroids) {
                //    Console.WriteLine ($"Asteroid at {asteroid.Key.x}, {asteroid.Key.y} sees {visibleAsteroidCount} other asteroids");
                    mostAsteroids = asteroidsVisibleToAsteroid[asteroid.Key].Count;
                    bestAsteroid = asteroid.Key;
                }
            }

            Console.WriteLine ($"Most asteroids visible was {mostAsteroids}, at asteroid {bestAsteroid.x}, {bestAsteroid.y}");
        }
        public static (int x, int y) GetDirection (int direction, (int x, int y) travelVector, int numberOfSteps, (int x, int y) asteroidCoordinate) {
            switch (direction) {
                case 0:
                    return (asteroidCoordinate.x + travelVector.x * numberOfSteps, asteroidCoordinate.y + travelVector.y * numberOfSteps);
                case 1:
                    return (asteroidCoordinate.x + -1 * travelVector.x * numberOfSteps, asteroidCoordinate.y + travelVector.y * numberOfSteps);
                case 2:
                    return (asteroidCoordinate.x + travelVector.x * numberOfSteps, asteroidCoordinate.y + -1 * travelVector.y * numberOfSteps);
                case 3:
                    return (asteroidCoordinate.x + -1 * travelVector.x * numberOfSteps, asteroidCoordinate.y + -1 * travelVector.y * numberOfSteps);
                default:
                    throw new KeyNotFoundException();
            }
        }

        public static void Draw((int x, int y) spaceBase, List<(int x, int y)> map, int squareSize){
            for (int i = 0; i < squareSize; i ++){
                for (int j = 0; j < squareSize; j++){
                    if (map.Contains((j, i))){
                        Console.Write('#');
                    }
                    else if (spaceBase.x == j && spaceBase.y == i)
                    {
                        Console.Write('X');
                    }
                    else{
                        Console.Write('.');
                    }
                }
                Console.WriteLine();
            }
        }

        public static int Gcd(int m, int n)
        {
            var tmp = 0;
            if (m < n)
            {
                tmp = m;
                m = n;
                n = tmp;
            }
            while (n != 0)
            {
                tmp = m % n;
                m = n;
                n = tmp;
            }
            return m;
        }
    }
} 
