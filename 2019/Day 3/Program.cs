using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_3
{
    class Program
    {
        static void Main(string[] args)
        {
            var wires = File.ReadAllLines("./input.txt");
            // string[] wires = new[] {"R75,D30,R83,U83,L12,D49,R71,U7,L72","U62,R66,U55,R34,D71,R55,D58,R83"};
            // string[] wires = new[] {"R98,U47,R26,D63,R33,U87,L62,D20,R33,U53,R51","U98,R91,D20,R16,D67,R40,U7,R15,U6,R7"};

            var map = new Dictionary<(int x, int y), (string wireCharacter, int wireNumber, int stepsToHere)>();

            map.Add((0, 0), ("O", 0, 0));
            var wireNumber = 1;
            foreach(var wire in wires)
            {
                int stepsToHere = 0;
                var directions = wire.Split(',');
                (int x, int y) currentLocation = (0, 0);
                (int x, int y) nextLocation = (0, 0);

                foreach(var vector in directions)
                {
                    int movementAmount = int.Parse(vector.Substring(1));
                    
                    switch(vector[0]){
                        case 'U':
                            nextLocation = (currentLocation.x, currentLocation.y + movementAmount);

                            for (int i=1;i<movementAmount; i++)
                            {
                                stepsToHere++;
                                (int x, int y) intermediateLocation = (currentLocation.x, currentLocation.y + i);
                                if (!map.TryGetValue(intermediateLocation, out var existingWire)){
                                    map.Add(intermediateLocation, ("|", wireNumber, stepsToHere));
                                }
                                else
                                {
                                    if (existingWire.wireCharacter.Equals("-") && existingWire.wireNumber != wireNumber){
                                        map[intermediateLocation] = ("X", 0, existingWire.stepsToHere + stepsToHere);
                                    }
                                }
                            }
                                stepsToHere++;
                            map.Add(nextLocation, ("+", wireNumber, stepsToHere));
                            break;
                        case 'D':
                            nextLocation = (currentLocation.x, currentLocation.y - movementAmount);
                            for (int i=1;i<movementAmount; i++)
                            {
                                stepsToHere++;

                                (int x, int y) intermediateLocation = (currentLocation.x, currentLocation.y - i);
                                if (!map.TryGetValue(intermediateLocation, out var existingWire)){
                                    map.Add(intermediateLocation, ("|", wireNumber, stepsToHere));
                                }
                                else
                                {
                                    if (existingWire.wireCharacter.Equals("-") && existingWire.wireNumber != wireNumber){
                                        map[intermediateLocation] = ("X", 0, existingWire.stepsToHere + stepsToHere);
                                    }
                                }
                            }
                                stepsToHere++;
                            map.Add(nextLocation, ("+", wireNumber, stepsToHere));
                            break;
                        case 'L':
                            nextLocation = (currentLocation.x - movementAmount, currentLocation.y);
                            for (int i=1;i<movementAmount; i++)
                            {
                                stepsToHere++;

                                (int x, int y) intermediateLocation = (currentLocation.x - i, currentLocation.y);
                                if (!map.TryGetValue(intermediateLocation, out var existingWire)){
                                    map.Add(intermediateLocation, ("-", wireNumber, stepsToHere));
                                }
                                else
                                {
                                    if (existingWire.wireCharacter.Equals("|") && existingWire.wireNumber != wireNumber){
                                        map[intermediateLocation] = ("X", 0, existingWire.stepsToHere + stepsToHere);
                                    }
                                }
                            }
                                stepsToHere++;
                            map.Add(nextLocation, ("+", wireNumber, stepsToHere));
                            break;
                        case 'R':
                            nextLocation = (currentLocation.x + movementAmount, currentLocation.y);
                            for (int i=1;i<movementAmount; i++)
                            {
                                stepsToHere++;

                                (int x, int y) intermediateLocation = (currentLocation.x + i, currentLocation.y);
                                if (!map.TryGetValue(intermediateLocation, out var existingWire)){
                                    map.Add(intermediateLocation, ("-", wireNumber, stepsToHere));
                                }
                                else
                                {
                                    if (existingWire.wireCharacter.Equals("|") && existingWire.wireNumber != wireNumber){
                                        map[intermediateLocation] = ("X", 0, existingWire.stepsToHere + stepsToHere);
                                    }
                                }
                            }
                                stepsToHere++;

                            map.Add(nextLocation, ("+", wireNumber, stepsToHere));
                            break;
                    }

                    currentLocation = nextLocation;
                }
                wireNumber ++;
            }

            var crossingPoints = map.Where(m => m.Value.wireCharacter.Equals("X"));
            var shortestDistance = int.MaxValue;

            foreach(var coordinate in crossingPoints){
                var distance = Math.Abs(coordinate.Key.x) + Math.Abs(coordinate.Key.y);

                Console.WriteLine($"distance {distance} at coordinate {coordinate.Key.x}, {coordinate.Key.y}");

                shortestDistance = distance < shortestDistance ? distance : shortestDistance;
            }

            Console.WriteLine(shortestDistance); // part 1

            var shortestTime = int.MaxValue;
            foreach(var coordinate in crossingPoints){
                var time = coordinate.Value.stepsToHere;
                Console.WriteLine($"time {time} at coordinate {coordinate.Key.x}, {coordinate.Key.y}");
                shortestTime = time < shortestTime ? time : shortestTime;
            }

            Console.WriteLine(shortestTime);
        }

    }
}
