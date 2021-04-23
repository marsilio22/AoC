using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_20
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("./input.txt");

            List<Point> points = new List<Point>(1000);

            var count = 0;

            foreach (var line in input)
            {
                var splitLine = line.Split(", ");
                
                var pos = ParseToCoord(splitLine[0]);
                var vel = ParseToCoord(splitLine[1]);
                var acc = ParseToCoord(splitLine[2]);

                points.Add(new Point{Id = count, Position = pos, Velocity = vel, Acceleration = acc});
                count++;
            }

            for(int i = 0; i < 10000; i++)
            {
                foreach(var point in points)
                {
                    point.Tick();
                }
            }

            ICollection<(int id, long distance)> distances = points.Select(p => (p.Id, p.CalculateDistanceFromOrigin())).ToList();

            var minDistance = distances.Single(e => e.distance == distances.Min(d => d.distance));

            Console.WriteLine(minDistance.id);

            // reset for part 2, todo split into method 
            points = new List<Point>(1000);
            count = 0;

            foreach (var line in input)
            {
                var splitLine = line.Split(", ");
                
                var pos = ParseToCoord(splitLine[0]);
                var vel = ParseToCoord(splitLine[1]);
                var acc = ParseToCoord(splitLine[2]);

                points.Add(new Point{Id = count, Position = pos, Velocity = vel, Acceleration = acc});
                count++;
            }

            for(int i = 0; i < 10000; i++)
            {
                foreach(var point in points)
                {
                    point.Tick();
                }

                // distances = points.Select(p => (p.Id, p.CalculatePositionSum())).ToList();

                // if (distances.Any(d => distances.Count(e => e.id != d.id && e.distance == d.distance) > 0))
                // {
                    
                // }

                List<Point> pointsToRemove = new List<Point>();

                foreach(var point in points)
                {
                    if (!pointsToRemove.Contains(point) && points.Count(p => p.Id != point.Id && p.Position == point.Position) > 0)
                    {
                        pointsToRemove.AddRange(points.Where(p => p.Position == point.Position));
                    }
                }

                points.RemoveAll(p => pointsToRemove.Contains(p));
            }


            Console.WriteLine(points.Count());
        }

        public static (long x, long y, long z) ParseToCoord(string input)
        {
            var thingList = input.Split('<')[1].Split('>')[0].Split(',').Select(s => long.Parse(s)).ToList();
            return (thingList[0], thingList[1], thingList[2]);
        }
    }

    public class Point
    {
        public int Id { get; set; }
        public (long x, long y, long z) Position {get;set;}
        public (long x, long y, long z) Velocity {get;set;}
        public (long x, long y, long z) Acceleration {get;set;}

        public void Tick()
        {
            this.Velocity = (
                this.Velocity.x + this.Acceleration.x, 
                this.Velocity.y + this.Acceleration.y, 
                this.Velocity.z + this.Acceleration.z
            );
            
            this.Position = (
                this.Position.x + this.Velocity.x,
                this.Position.y + this.Velocity.y,
                this.Position.z + this.Velocity.z
            );
        }

        public long CalculateDistanceFromOrigin()
        {
            return Math.Abs(Position.x) + Math.Abs(Position.y) + Math.Abs(Position.z);
        }

        public long CalculatePositionSum()
        {
            return Position.x + Position.y + Position.z;
        }
    }    
}
