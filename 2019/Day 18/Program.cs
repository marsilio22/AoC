using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Day_18
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = //File.ReadAllLines("./input.txt");
                new [] {
                    "########################",
                    "#...............b.C.D.f#",
                    "#.######################",
                    "#.....@.a.B.c.d.A.e.F.g#",
                    "#########################"
                };

            var map = new Dictionary<(int x, int y), char>();
            int x = 0, y = 0;
            foreach (var line in input){
                foreach (char character in line){
                    if (character != '#'){
                        map.Add((x, y), character);
                    }
                    x ++;
                }
                y ++; x = 0;
            }

            var doorValues = Enumerable.Range(65, 27).ToList();
            var keyValues = Enumerable.Range(97, 27).ToList();

            var currentPosition = map.Single(m => m.Value == '@').Key;
            int totalSteps = 0;
            var keys = map.Where(m => keyValues.Contains(m.Value)).ToList();
            var doors = map.Where(m => doorValues.Contains(m.Value)).ToList();

            var distancesFromKeysToDoors = new Dictionary<char, int>();

            foreach(var key in keys){
                var distances = CalculateDistances(map, key.Key, new List<(int, int)>(), true);
                char doorChar = (char)(key.Value - 32);
                var door = doors.SingleOrDefault(d => d.Value == doorChar);

                if (door.Key.x > 0){
                    distancesFromKeysToDoors.Add(key.Value, distances[door.Key]);
                }
            }

            while (keys.Any()){
                var distances = CalculateDistances(map, currentPosition, doors.Select(m => m.Key).ToList(), false);

                var accessibleDoors = doors.Where(d => 
                    distances.ContainsKey((d.Key.x + 1, d.Key.y)) ||
                    distances.ContainsKey((d.Key.x - 1, d.Key.y)) ||
                    distances.ContainsKey((d.Key.x, d.Key.y + 1)) ||
                    distances.ContainsKey((d.Key.x, d.Key.y - 1))
                ).Select(d => d.Value).ToList();

                List<((int x, int y), int distance, char keyChar, int distanceToDoor, bool doorIsAccessible)> closestKeys = keys
                    .Where(k => distances.ContainsKey(k.Key))
                    .Select(k => (k.Key, distances[k.Key], k.Value, distancesFromKeysToDoors.ContainsKey(k.Value) ? distancesFromKeysToDoors[k.Value] : int.MinValue, accessibleDoors.Contains((char)(k.Value - 32))))
                    .ToList();

                // AT WHAT DISTANCE SHOULD AN INACCESSIBLE DOOR OVERRULE AN ACCESSIBLE ONE?
                ((int x, int y), int distance, char keyChar, int distanceToDoor, bool doorIsAccessible) bestKey = closestKeys.OrderByDescending(k => k.distance + k.distanceToDoor).First();
                
                totalSteps += bestKey.distance;
                currentPosition = bestKey.Item1;
                var keyChar = map[bestKey.Item1];
                var doorChar = keyChar - 32;

                var doorCoord = map.SingleOrDefault(m => m.Value == doorChar).Key;
                if (doorCoord.x > 0){
                    map[doorCoord] = '.';
                }
                keys = keys.Where(k => k.Value != keyChar).ToList();
                doors = doors.Where(d => d.Value != doorChar).ToList();

                map[bestKey.Item1] = '.';
            }

            // 6724 too high
            // 4496 too high
            Console.WriteLine(totalSteps);
        }

        static Dictionary<(int x, int y), int> CalculateDistances(
            Dictionary<(int x, int y), char> map, 
            (int x, int y) currentPosition, 
            List<(int x, int y)> doors,
            bool ignoreDoors)
        {
            Dictionary<(int x, int y), int> distances = new Dictionary<(int x, int y), int>();

            var steps = 0;
            distances.Add(currentPosition, 0);
            var nextCoords = map.Where(m => (ignoreDoors || !doors.Contains(m.Key)) && 
                                            !distances.ContainsKey(m.Key) && 
                                            (distances.ContainsKey((m.Key.x, m.Key.y + 1)) ||
                                            distances.ContainsKey((m.Key.x, m.Key.y - 1)) ||
                                            distances.ContainsKey((m.Key.x + 1, m.Key.y)) ||
                                            distances.ContainsKey((m.Key.x - 1, m.Key.y)))).Distinct().ToList();
            while (nextCoords.Any()){ 
                foreach(var coord in nextCoords){
                    distances.TryAdd(coord.Key, steps + 1);
                }
                steps ++;
                nextCoords = map.Where(m => (ignoreDoors || !doors.Contains(m.Key)) && 
                                            !distances.ContainsKey(m.Key) && 
                                            (distances.ContainsKey((m.Key.x, m.Key.y + 1)) ||
                                            distances.ContainsKey((m.Key.x, m.Key.y - 1)) ||
                                            distances.ContainsKey((m.Key.x + 1, m.Key.y)) ||
                                            distances.ContainsKey((m.Key.x - 1, m.Key.y)))).Distinct().ToList();

            }

            return distances;
        }
    }
}
