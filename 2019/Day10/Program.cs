using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day10 {
    class Program {
        static void Main (string[] args) {
            var input = File.ReadAllLines ("./input.txt");

            // exanple solution 33 at 5, 8
            input = new []{"......#.#.",
                           "#..#.#....",
                           "..#######.",
                           ".#.#.###..",
                           ".#..#.....",
                           "..#....#.#",
                           "#..#....#.",
                           ".##.#..###",
                           "##...#..#.",
                           ".#....####"};



            // yes, I know 1 is not prime.
            var primesTo100 = "1,2,3,5,7,11,13,17,19,23,29,31,37,41,43,47,53,59,61,67,71,73,79,83,89,97".Split (",").Select (p => int.Parse (p)).ToList ();
            Dictionary < (int x, int y), char > map = new Dictionary < (int x, int y), char > ();

            ICollection < (int x, int y) > primeComboCoordinateVectors = 
                (from x in Enumerable.Range(1, 100)
                 from y in Enumerable.Range(1, 100)
                 where x != y && (x == 1 || y == 1 || Gcd(x, y) == 1)
                 select (x, y)).ToList ();

            primeComboCoordinateVectors.Prepend ((1, 1));
            primeComboCoordinateVectors.Prepend ((0, 1));
            primeComboCoordinateVectors.Prepend ((1, 0));

            for (int y = 0; y < input.Length; y++) {
                var line = input[y];
                for (int x = 0; x < line.Length; x++) {
                    map[(x, y)] = line[x];
                }
            }

            // var actualUniqueDirections = new List<(int x, int y)>();
            // foreach(var primeCoordVector in primeComboCoordinateVectors){
            //     for (int count = 1; count < 100; count ++){
            //         if (primeCoordVector.x % count == 0 &&
            //             primeCoordVector.y % count == 0 &&
            //             actualUniqueDirections.Contains((primeCoordVector.x / count, primeCoordVector.y / count)))
            //         {
            //             continue;
            //         }
            //         else
            //         {
            //             actualUniqueDirections.Add(primeCoordVector);
            //             break;
            //         }
            //     }
            // }

            var asteroids = map.Where (m => m.Value.Equals ('#')).ToDictionary (d => d.Key, d => d.Value);
            int mostAsteroids = int.MinValue;
            foreach (var asteroid in asteroids) {
                int visibleAsteroidCount = 0;

                foreach ((int x, int y) primeCoordVector in primeComboCoordinateVectors) {
                    int numberOfSteps = 0;
                    (int x, int y) coordinate = primeCoordVector;
                    List < (int x, int y) > doneDirections = new List < (int x, int y) > ();

                    for (int direction = 0; direction < 4; direction++) {
                        //coordinate = GetDirection (direction, primeCoordVector, numberOfSteps, asteroid.Key);

                        if (doneDirections.Contains(coordinate)){
                            continue;
                        }
                        doneDirections.Add(coordinate);

                        while (map.ContainsKey (coordinate)) {
                            if (asteroids.ContainsKey (coordinate)) {
                                visibleAsteroidCount += 1;
                                break;
                            }
                            numberOfSteps += 1;
                            coordinate = GetDirection (direction, primeCoordVector, numberOfSteps, asteroid.Key);
                        }
                    }
                }
                Console.WriteLine (visibleAsteroidCount);
                if (visibleAsteroidCount > mostAsteroids) {
                    Console.WriteLine ($"Asteroid at {asteroid.Key.x}, {asteroid.Key.y} sees {visibleAsteroidCount} other asteroids");
                    mostAsteroids = visibleAsteroidCount;
                }
            }

            Console.WriteLine ("Most " + mostAsteroids);
        }
        public static (int x, int y) GetDirection (int direction, (int x, int y) coordinate, int numberOfSteps, (int x, int y) asteroidCoordinate) {
            switch (direction) {
                case 0:
                    return (asteroidCoordinate.x + coordinate.x * numberOfSteps, asteroidCoordinate.y + coordinate.y * numberOfSteps);
                case 1:
                    return (asteroidCoordinate.x + -1 * coordinate.x * numberOfSteps, asteroidCoordinate.y + coordinate.y * numberOfSteps);
                case 2:
                    return (asteroidCoordinate.x + coordinate.x * numberOfSteps, asteroidCoordinate.y + -1 * coordinate.y * numberOfSteps);
                case 3:
                    return (asteroidCoordinate.x + -1 * coordinate.x * numberOfSteps, asteroidCoordinate.y + -1 * coordinate.y * numberOfSteps);
                default:
                    return (0, 0);
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