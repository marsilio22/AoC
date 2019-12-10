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

            // while i = 0; i < 200
            //     Delete closest upwards NaN
            //     i ++;
            //     for x > 0, y > 0
            //         select distinct gradients, order by gradient descending
            //         foreach gradient
            //             blam the asteroid with the closest distance and that grad.
            //             i ++;
            //     Delete closest rightwards 0
            //     i ++;
            //     for x > 0, y < 0
            //         select distinct gradients, order by gradient descending
            //         foreach gradient
            //             blam the asteroid with the closest distance and that grad.
            //             i ++;
            //     Delete closest downwards NaN
            //     i ++;
            //     for x < 0, y < 0
            //         select distinct gradients, order by gradient descending
            //         foreach gradient
            //             blam the asteroid with the closest distance and that grad.
            //             i ++;
            //     Delete closest leftwards 0
            //     i ++;
            //     for x < 0, y > 0
            //         select distinct gradients, order by gradient descending
            //         foreach gradient
            //             blam the asteroid with the closest distance and that grad.
            //             i ++;

            int asteroidCount = 0;

            // TODO remove repetition in this loop...
            while (asteroidCount <= Math.Min(200, asteroids.Count - 1)){
                var closestUpwardsNan = newMap.Where(m => double.IsNaN(m.Value.gradient) && m.Key.y < bestAsteroid.y).OrderBy(a => a.Value.distance).FirstOrDefault();
                if (closestUpwardsNan.Value.distance != 0) // default value indication
                {
                    newMap.Remove(closestUpwardsNan.Key);
                    asteroidCount ++; 
                    Console.WriteLine($"Killed asteroid {asteroidCount} at coordinate {closestUpwardsNan.Key.x},{closestUpwardsNan.Key.y}");
                }

                var topRightQuadrant = newMap.Where(m => m.Key.x > bestAsteroid.x && m.Key.y < bestAsteroid.y).ToList();
                var grads = topRightQuadrant.Select(q => q.Value.gradient).Distinct().OrderByDescending(g => g).ToList();
                foreach(var grad in grads){
                    var closestAsteroid = topRightQuadrant.Where(m => Math.Abs(m.Value.gradient - grad) < double.Epsilon).OrderBy(a => a.Value.distance).First();
                    newMap.Remove(closestAsteroid.Key);
                    asteroidCount ++;
                    Console.WriteLine($"Killed asteroid {asteroidCount} at coordinate {closestAsteroid.Key.x},{closestAsteroid.Key.y}, grad: {closestAsteroid.Value.gradient}");
                }




                var closestRightwards0 = newMap.Where(m => m.Value.gradient < double.Epsilon && m.Key.x > bestAsteroid.x).OrderBy(a => a.Value.distance).FirstOrDefault();
                if(closestRightwards0.Value.distance != 0){
                    newMap.Remove(closestRightwards0.Key);
                    asteroidCount++;
                    Console.WriteLine($"Killed asteroid {asteroidCount} at coordinate {closestRightwards0.Key.x},{closestRightwards0.Key.y}");
                }

                var bottomRightQuadrant = newMap.Where(m => m.Key.x > bestAsteroid.x && m.Key.y > bestAsteroid.y).ToList();
                grads = bottomRightQuadrant.Select(q => q.Value.gradient).Distinct().OrderByDescending(g => g).ToList();
                foreach(var grad in grads){
                    var closestAsteroid = bottomRightQuadrant.Where(m => Math.Abs(m.Value.gradient - grad) < double.Epsilon).OrderBy(a => a.Value.distance).First();
                    newMap.Remove(closestAsteroid.Key);
                    asteroidCount++;
                    Console.WriteLine($"Killed asteroid {asteroidCount} at coordinate {closestAsteroid.Key.x},{closestAsteroid.Key.y}, grad: {closestAsteroid.Value.gradient}");
                }


                var closestDownwardsNan = newMap.Where(m => double.IsNaN(m.Value.gradient) && m.Key.y > bestAsteroid.y).OrderBy(a => a.Value.distance).FirstOrDefault();
                if(closestDownwardsNan.Value.distance != 0){
                    newMap.Remove(closestDownwardsNan.Key);
                    asteroidCount++;
                    Console.WriteLine($"Killed asteroid {asteroidCount} at coordinate {closestDownwardsNan.Key.x},{closestDownwardsNan.Key.y}");
                }

                var bottomLeftQuadrant = newMap.Where(m => m.Key.x < bestAsteroid.x && m.Key.y > bestAsteroid.y).ToList();
                grads = bottomLeftQuadrant.Select(q => q.Value.gradient).Distinct().OrderByDescending(g => g).ToList();
                foreach(var grad in grads){
                    var closestAsteroid = bottomLeftQuadrant.Where(m => Math.Abs(m.Value.gradient - grad) < double.Epsilon).OrderBy(a => a.Value.distance).First();
                    newMap.Remove(closestAsteroid.Key);
                    asteroidCount++;
                    Console.WriteLine($"Killed asteroid {asteroidCount} at coordinate {closestAsteroid.Key.x},{closestAsteroid.Key.y}, grad: {closestAsteroid.Value.gradient}");
                }


                var closestLeftwards0 = newMap.Where(m => m.Value.gradient < double.Epsilon && m.Key.x < bestAsteroid.x).OrderBy(a => a.Value.distance).FirstOrDefault();
                if (closestLeftwards0.Value.distance != 0){
                    newMap.Remove(closestLeftwards0.Key);
                    asteroidCount++;
                    Console.WriteLine($"Killed asteroid {asteroidCount} at coordinate {closestLeftwards0.Key.x},{closestLeftwards0.Key.y}");
                }

                var topLeftQuadrant = newMap.Where(m => m.Key.x < bestAsteroid.x && m.Key.y < bestAsteroid.y).ToList();
                grads = topLeftQuadrant.Select(q => q.Value.gradient).Distinct().OrderByDescending(g => g).ToList();
                foreach(var grad in grads){
                    var closestAsteroid = topLeftQuadrant.Where(m => Math.Abs(m.Value.gradient - grad) < double.Epsilon).OrderBy(a => a.Value.distance).First();
                    newMap.Remove(closestAsteroid.Key);
                    asteroidCount++;
                    Console.WriteLine($"Killed asteroid {asteroidCount} at coordinate {closestAsteroid.Key.x},{closestAsteroid.Key.y}, grad: {closestAsteroid.Value.gradient}");
                }
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
