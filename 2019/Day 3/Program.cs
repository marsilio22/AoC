using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_3 {
    class Program {
        static void Main (string[] args) {
            // Note. The method used here is insane and shouldn't have been done. instead take the two wires as
            // lists of coordinates, and intersect them, which is much MUCH easier.....

            var wires = File.ReadAllLines ("./input.txt");
            // string[] wires = new[] {"R75,D30,R83,U83,L12,D49,R71,U7,L72","U62,R66,U55,R34,D71,R55,D58,R83"};
            // string[] wires = new[] {"R98,U47,R26,D63,R33,U87,L62,D20,R33,U53,R51","U98,R91,D20,R16,D67,R40,U7,R15,U6,R7"};

            var map = new Dictionary < (int x, int y),
                (string wireCharacter, int wireNumber, int stepsToHere) > ();

            map.Add ((0, 0), ("O", 0, 0));
            var wireNumber = 1;

            // Add each wire to the map. 
            foreach (var wire in wires) {
                // This keeps track of how many steps we've taken to each point. 
                // Pass by reference to the calc method
                int stepsToHere = 0;
                var directions = wire.Split (',');
                (int x, int y) currentLocation = (0, 0);
                (int x, int y) nextLocation = (0, 0);

                // Parse the instructions
                foreach (var vector in directions) {
                    nextLocation = CalculateCoordinatesFromInstruction (
                        vector,
                        currentLocation,
                        ref stepsToHere,
                        map,
                        wireNumber
                    );

                    currentLocation = nextLocation;
                }

                // do the next wire.
                wireNumber++;
            }

            // Find all the crossing points.
            var crossingPoints = map.Where (m => m.Value.wireCharacter.Equals ("X"));
            var shortestDistance = int.MaxValue;

            // part 1, manhattan distance to each point, Take shortest distance
            foreach (var coordinate in crossingPoints) {
                var distance = Math.Abs (coordinate.Key.x) + Math.Abs (coordinate.Key.y);

                Console.WriteLine ($"distance {distance} at coordinate {coordinate.Key.x}, {coordinate.Key.y}");

                shortestDistance = distance < shortestDistance ? distance : shortestDistance;
            }

            Console.WriteLine (shortestDistance); // part 1

            // part 2, figure out the total number of steps to each crossing, but that's ok because we've recorded it :D
            var shortestTime = int.MaxValue;
            foreach (var coordinate in crossingPoints) {
                var time = coordinate.Value.stepsToHere;
                Console.WriteLine ($"time {time} at coordinate {coordinate.Key.x}, {coordinate.Key.y}");
                shortestTime = time < shortestTime ? time : shortestTime;
            }

            Console.WriteLine (shortestTime); // part 2
        }

        /// <summary>
        /// Calculate the intermediate coordinates based on the instruction and current location
        /// </summary>
        /// <returns>The next location</returns>
        public static (int x, int y) CalculateCoordinatesFromInstruction (
            string instruction,
            (int x, int y) currentLocation,
            ref int stepsToHere,
            IDictionary < (int x, int y), (string wireCharacter, int wireNumber, int stepsToHere) > map,
            int wireNumber
        ) {
            int movementAmount = int.Parse (instruction.Substring (1));
            (int deltax, int deltay) delta = (0, 0);

            switch (instruction[0]) {
                case 'U':
                    delta = (0, 1);
                    break;
                case 'D':
                    delta = (0, -1);
                    break;
                case 'L':
                    delta = (-1, 0);
                    break;
                case 'R':
                    delta = (1, 0);
                    break;
            }

            var nextLocation = (currentLocation.x + delta.deltax * movementAmount, currentLocation.y + delta.deltay * movementAmount);

            for (int i = 1; i < movementAmount; i++) {
                stepsToHere++;

                (int x, int y) intermediateLocation = (currentLocation.x + delta.deltax * i, currentLocation.y + delta.deltay * i);

                if (!map.TryGetValue (intermediateLocation, out var existingWire)) {
                    map.Add (intermediateLocation, ("w", wireNumber, stepsToHere));
                } else {
                    if (existingWire.wireNumber != wireNumber) {
                        map[intermediateLocation] = ("X", 0, existingWire.stepsToHere + stepsToHere);
                    }
                }
            }
            stepsToHere++;
            map.Add (nextLocation, ("+", wireNumber, stepsToHere));

            return nextLocation;
        }
    }
}