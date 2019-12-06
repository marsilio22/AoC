using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_6 {
    class Program {
        private static IDictionary<string, List<string>> mapData;
        private static IDictionary<string, int> orbitCheckSums = new Dictionary<string, int> ();

        static void Main (string[] args) {
            mapData = File.ReadLines ("./input.txt")
                .Select (i => i.Split (')'))
                .GroupBy (i => i[0])
                .ToDictionary (g => g.Key, g => g.Select (i => i[1]).ToList ());

            // var thing = "COM)B,B)C,C)D,D)E,E)F,B)G,G)H,D)I,E)J,J)K,K)L,K)YOU,I)SAN";
            // mapData = thing.Split(",")
            //         .Select(i => i.Split(')'))
            //         .GroupBy(i => i[0])
            //         .ToDictionary(g => g.Key, g => g.Select(i => i[1]).ToList());

            var centre = "COM";

            CountSubOrbits (centre, 0); // part 1
            Console.WriteLine (orbitCheckSums.Values.Sum ());

            int minDistanceToMeAndSanta = int.MaxValue;
            var listOfPlanets = mapData.Select (m => m.Key).Union (mapData.SelectMany (m => m.Value)).Distinct ();
            foreach (var planet in listOfPlanets) {
                orbitCheckSums = new Dictionary<string, int> ();
                CountSubOrbits (planet, 0);

                if (orbitCheckSums.TryGetValue ("SAN", out int distanceToSanta) &&
                    orbitCheckSums.TryGetValue ("YOU", out int distanceToMe)) {
                    if (distanceToMe + distanceToSanta < minDistanceToMeAndSanta) {
                        minDistanceToMeAndSanta = distanceToMe + distanceToSanta;

                        Console.WriteLine ($"{planet}");
                        Console.WriteLine ($"SAN: {orbitCheckSums["SAN"]}");
                        Console.WriteLine ($"YOU: {orbitCheckSums["YOU"]}");
                    }
                }
            }

            Console.WriteLine (minDistanceToMeAndSanta - 4);
        }

        public static void CountSubOrbits (string node, int indirectOrbits) {
            int directOrbits = 1;
            if (node.Equals ("COM")) {
                directOrbits = 0;
            }

            if (!mapData.TryGetValue (node, out List<string> subDirectOrbits)) {
                if (!orbitCheckSums.TryGetValue (node, out int orbits)) {
                    orbits = 0;
                    orbitCheckSums.Add (node, orbits);
                }

                orbitCheckSums[node] += (indirectOrbits + directOrbits);
                return;
            } else {
                if (!orbitCheckSums.TryGetValue (node, out int orbits)) {
                    orbits = 0;
                    orbitCheckSums.Add (node, orbits);
                }

                orbitCheckSums[node] += (indirectOrbits + directOrbits);

                foreach (var body in subDirectOrbits) {
                    CountSubOrbits (body, indirectOrbits + directOrbits);
                }

            }
        }
    }
}