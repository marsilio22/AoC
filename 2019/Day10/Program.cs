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
                
                //Console.WriteLine($"Asteroid at {asteroid.Key.x}, {asteroid.Key.y}: {asteroidsVisibleToAsteroid[asteroid.Key].Count()}");

                if (asteroidsVisibleToAsteroid[asteroid.Key].Count > mostAsteroids) {
                    mostAsteroids = asteroidsVisibleToAsteroid[asteroid.Key].Count;
                    bestAsteroid = asteroid.Key;
                }
            }

            // Part 1
            Console.WriteLine ($"Most asteroids visible was {mostAsteroids}, at asteroid {bestAsteroid.x}, {bestAsteroid.y}");

            // starting from the best asteroid, work out the gradient of the line, and manhattan distance, to every other asteroid.

            var newMap = new Dictionary<(int x, int y), (double gradient, int distance)>();

            foreach(var asteroid in asteroids){
                var actualAsteroid = asteroid.Key;
                (int x, int y) relativeXY = (asteroid.Key.x - bestAsteroid.x, asteroid.Key.y - bestAsteroid.y);
                if (relativeXY.x == 0 && relativeXY.y == 0){
                    // it's the same asteroid, don't worry about it.
                    continue;
                }
                else if (relativeXY.x == 0){
                    newMap.Add((actualAsteroid.x, actualAsteroid.y), (double.NaN, Math.Abs(relativeXY.y)));
                }
                else if (relativeXY.y == 0){
                    newMap.Add((actualAsteroid.x, actualAsteroid.y), (0d, Math.Abs(relativeXY.x)));
                }
                else {
                    newMap.Add((actualAsteroid.x, actualAsteroid.y), ((double)relativeXY.x / (double)relativeXY.y, Math.Abs(relativeXY.x) + Math.Abs(relativeXY.y)));
                }
            }

            int asteroidCount = 0;

            // TODO remove repetition in this loop...
            while (asteroidCount <= Math.Min(200, asteroids.Count - 1)){
                var closestUpwardsNan = newMap.Where(m => double.IsNaN(m.Value.gradient) && m.Key.y < bestAsteroid.y).OrderBy(a => a.Value.distance).FirstOrDefault();
                DeleteSpecificAsteroid(closestUpwardsNan, newMap, ref asteroidCount);
                var topRightQuadrant = newMap.Where(m => m.Key.x > bestAsteroid.x && m.Key.y < bestAsteroid.y).ToList();
                KillGradientAsteroidsInQuadrant(topRightQuadrant, newMap, ref asteroidCount);

                var closestRightwards0 = newMap.Where(m => m.Value.gradient < double.Epsilon && m.Key.x > bestAsteroid.x).OrderBy(a => a.Value.distance).FirstOrDefault();
                DeleteSpecificAsteroid(closestRightwards0, newMap, ref asteroidCount);
                var bottomRightQuadrant = newMap.Where(m => m.Key.x > bestAsteroid.x && m.Key.y > bestAsteroid.y).ToList();
                KillGradientAsteroidsInQuadrant(bottomRightQuadrant, newMap, ref asteroidCount);

                var closestDownwardsNan = newMap.Where(m => double.IsNaN(m.Value.gradient) && m.Key.y > bestAsteroid.y).OrderBy(a => a.Value.distance).FirstOrDefault();
                DeleteSpecificAsteroid(closestDownwardsNan, newMap, ref asteroidCount);
                var bottomLeftQuadrant = newMap.Where(m => m.Key.x < bestAsteroid.x && m.Key.y > bestAsteroid.y).ToList();
                KillGradientAsteroidsInQuadrant(bottomLeftQuadrant, newMap, ref asteroidCount);

                var closestLeftwards0 = newMap.Where(m => m.Value.gradient < double.Epsilon && m.Key.x < bestAsteroid.x).OrderBy(a => a.Value.distance).FirstOrDefault();
                DeleteSpecificAsteroid(closestLeftwards0, newMap, ref asteroidCount);
                var topLeftQuadrant = newMap.Where(m => m.Key.x < bestAsteroid.x && m.Key.y < bestAsteroid.y).ToList();
                KillGradientAsteroidsInQuadrant(topLeftQuadrant, newMap, ref asteroidCount);
            }
        }

        public static void DeleteSpecificAsteroid(KeyValuePair<(int x, int y), (double gradient, int distance)> asteroid, Dictionary<(int x, int y), (double gradient, int distance)> map, ref int asteroidCount){
            if (asteroid.Value.distance != 0){
                map.Remove(asteroid.Key);
                asteroidCount ++;
                PrintInfo(asteroidCount, asteroid.Key, asteroid.Value.gradient);
            }
        }

        public static void KillGradientAsteroidsInQuadrant(List<KeyValuePair<(int x, int y), (double gradient, int distance)>> quadrant, Dictionary<(int x, int y), (double gradient, int distance)> map, ref int asteroidCount){
            var grads = quadrant.Select(q => q.Value.gradient).Distinct().OrderByDescending(g => g).ToList();
            foreach(var grad in grads){
                var closestAsteroid = quadrant.Where(m => Math.Abs(m.Value.gradient - grad) < double.Epsilon).OrderBy(a => a.Value.distance).First();
                map.Remove(closestAsteroid.Key);
                asteroidCount++;
                PrintInfo(asteroidCount, closestAsteroid.Key, closestAsteroid.Value.gradient);
            }
        }

        public static void PrintInfo(int asteroidCount, (int x, int y) closestAsteroid, double closestAsteroidGradient){
            if(asteroidCount % 50 == 0 && asteroidCount <= 200)
            {
                Console.WriteLine($"Killed asteroid {asteroidCount} at coordinate {closestAsteroid.x},{closestAsteroid.y}, grad: {closestAsteroidGradient}");
            }
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

        // Debug tool
        public static void DrawAsteroidField((int x, int y) spaceBase, List<(int x, int y)> map, int squareSize){
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

        // Debug tool
        public static void DrawDistancesAndGradients((int x, int y) spaceBase, Dictionary<(int x, int y), (double gradient, int distance)> map, int squareSize){
            for (int i = 0; i < squareSize; i ++){
                for (int j = 0; j < squareSize; j++){
                    if (map.ContainsKey((j, i))){
                        Console.Write(Math.Round(map[(j, i)].gradient, 3).ToString().PadRight(6));
                    }
                    else if (spaceBase.x == j && spaceBase.y == i)
                    {
                        Console.Write("X".PadRight(6));
                    }
                    else{
                        Console.Write(".".PadRight(6));
                    }
                }
                Console.WriteLine();
            }
        }
    }
} 
