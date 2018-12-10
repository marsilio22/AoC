using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_10
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("./input.txt");

            List<Light> state = new List<Light>();

            var id = 1;
            foreach (var line in lines)
            {
                var splitLine = line.Split('<');
                var positionStart = splitLine[1].Split('>')[0].Split(',').Select(s => int.Parse(s)).ToList();
                var velocityStart = splitLine[2].Split('>')[0].Split(',').Select(s => int.Parse(s)).ToList();

                state.Add(
                    new Light
                    {
                        Id = id, 
                        InitialPosition = (positionStart[0], positionStart[1]), 
                        Velocity = (velocityStart[0], velocityStart[1]), 
                        Second = 0
                    });
                id++;
            }

            var sec = 0;
            int minimumDistance = int.MaxValue;
            Dictionary<int, List<Light>> states = new Dictionary<int, List<Light>>();
            while (true){
                var stateThisSecond = state.Select(s => new Light
                    {
                        Id = s.Id, 
                        InitialPosition = s.InitialPosition, 
                        Velocity = s.Velocity, 
                        Second = sec
                    }).ToList();
                states.Add(sec, stateThisSecond);

                var thing = stateThisSecond.Select(s => s.Position);

                var smallestX = thing.Aggregate((x, y) => x.x > y.x ? x : y);
                var biggestX = thing.Aggregate((x, y) => x.x < y.x ? x : y);
                var smallestY = thing.Aggregate((x, y) => x.y > y.y ? x : y);
                var biggestY = thing.Aggregate((x, y) => x.y < y.y ? x : y);

                var spreadX = Math.Abs((biggestX.x - smallestX.x));
                var spreadY = Math.Abs((biggestY.y - smallestY.y));

                var spread = spreadX + spreadY;
                if (minimumDistance < spread){
                    break;
                }
                minimumDistance = spread;
                sec++;
            }

            for(int i = -10; i < 0; i++){
                WriteState(states[states.Count + i]);
            }
        }

        public static void WriteState(List<Light> endState)
        {
            var maxY = endState.Aggregate((x, y) => x.Position.y > y.Position.y ? x : y);
            var minY = endState.Aggregate((x, y) => x.Position.y < y.Position.y ? x : y);
            var maxX = endState.Aggregate((x, y) => x.Position.x > y.Position.x ? x : y);
            var minX = endState.Aggregate((x, y) => x.Position.x < y.Position.x ? x : y);


            for (int i = minY.Position.y; i <= maxY.Position.y; i++)
            {
                for (int j = minX.Position.x; j <= maxX.Position.x; j++)
                {
                    if (endState.FirstOrDefault(s => s.Position.x == j && s.Position.y == i) != null){
                        Console.Write('#');
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }
                Console.WriteLine();
            }
        }
    }

    public class Light
    {
        public int Id { get; set; }
        public (int x, int y) Position => 
            (InitialPosition.x + Second * Velocity.x, InitialPosition.y + Second * Velocity.y);

        public (int x, int y) Velocity { get; set; }
        public (int x, int y) InitialPosition { get; set; }

        public int Second { get; set; }
    }
}
