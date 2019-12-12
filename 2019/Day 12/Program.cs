using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_12
{
    public class Program
    {
        public static List<Moon> CreateMoons(){
            var input = File.ReadAllLines("./input.txt");
            List<Moon> moons = new List<Moon>();
            
            var moonNames = new List<string> {"IO", "CALLISTO", "GANYMEDE", "EUROPA"};
            var i = 0;

            foreach(var line in input){
                var newLine = line.Substring(1, line.Length - 2);
                var moonXYZ = newLine.Split(", ").Select(s => int.Parse(s.Substring(2))).ToList();
                var moon = new Moon{
                    Name = moonNames[i],
                    X = moonXYZ[0],
                    Y = moonXYZ[1],
                    Z = moonXYZ[2]
                };

                moons.Add(moon);
                i++;
            }

            return moons;
        }

        static void Part1(){
            var moons = CreateMoons();
            long time = 0;

            while (time < 1000){
                MoveMoons(moons);
                time ++;
            }

            var energy = 0;
            foreach(var moon in moons){
                energy += moon.CalculateEnergy();
            }

            // Part 1
            Console.WriteLine(energy);
        }

        static void Part2(){
            var moons = CreateMoons();

            var Xs = new HashSet<(int IOx, int IOxV, int CAx, int CAxV, int GAx, int GAxV, int EUx, int EUxV)>();
            var Ys = new HashSet<(int IOy, int IOyV, int CAy, int CAyV, int GAy, int GAyV, int EUy, int EUyV)>();
            var Zs = new HashSet<(int IOz, int IOzV, int CAz, int CAzV, int GAz, int GAzV, int EUz, int EUzV)>();

            var IO = moons.Single(m => m.Name.Equals("IO"));
            var CALLISTO = moons.Single(m => m.Name.Equals("CALLISTO"));
            var GANYMEDE = moons.Single(m => m.Name.Equals("GANYMEDE"));
            var EUROPA = moons.Single(m => m.Name.Equals("EUROPA"));

            Xs.Add((IO.X, IO.XVelocity, CALLISTO.X, CALLISTO.XVelocity, GANYMEDE.X, GANYMEDE.XVelocity, EUROPA.X, EUROPA.XVelocity));
            Ys.Add((IO.Y, IO.YVelocity, CALLISTO.Y, CALLISTO.YVelocity, GANYMEDE.Y, GANYMEDE.YVelocity, EUROPA.Y, EUROPA.YVelocity));
            Zs.Add((IO.Z, IO.ZVelocity, CALLISTO.Z, CALLISTO.ZVelocity, GANYMEDE.Z, GANYMEDE.ZVelocity, EUROPA.Z, EUROPA.ZVelocity));

            while (true){
                MoveMoons(moons);
                var xRepeating = Xs.Add((IO.X, IO.XVelocity, CALLISTO.X, CALLISTO.XVelocity, GANYMEDE.X, GANYMEDE.XVelocity, EUROPA.X, EUROPA.XVelocity));
                var yRepeating = Ys.Add((IO.Y, IO.YVelocity, CALLISTO.Y, CALLISTO.YVelocity, GANYMEDE.Y, GANYMEDE.YVelocity, EUROPA.Y, EUROPA.YVelocity));
                var zRepeating = Zs.Add((IO.Z, IO.ZVelocity, CALLISTO.Z, CALLISTO.ZVelocity, GANYMEDE.Z, GANYMEDE.ZVelocity, EUROPA.Z, EUROPA.ZVelocity));

                if (!xRepeating && !yRepeating && !zRepeating){
                    break;
                }
            }

            // Work out the period. This assumes that it's the initial condition which is repeated, and there's
            // no non-equilibrium interval at the start, where the moons "fall" into the equilibrium loop
            
            // 167624
            var xPeriod = Xs.Count();
            
            // 231614
            var yPeriod = Ys.Count();

            // 116328
            var zPeriod = Zs.Count();

            // The result (when all moons are in their initial state) is the Lowest Common Multiple of the periods
            long result = determineLCM(determineLCM(xPeriod, yPeriod), zPeriod);

            Console.WriteLine(result);
        }

        static void Main (string[] args) {
            Part1();

            Part2();            
        }

        public static void MoveMoons(List<Moon> moons){
            foreach(var moon in moons){
                moon.XVelocity += (moons.Count(m => m.X > moon.X) - moons.Count(m => m.X < moon.X));
                moon.YVelocity += (moons.Count(m => m.Y > moon.Y) - moons.Count(m => m.Y < moon.Y));
                moon.ZVelocity += (moons.Count(m => m.Z > moon.Z) - moons.Count(m => m.Z < moon.Z));
            }

            foreach(var moon in moons){
                moon.Move();
            }
        }

        // shamelessly stolen from the internet.
        public static long determineLCM(long a, long b)
        {
            long num1, num2;
            if (a > b)
            {
                num1 = a; num2 = b;
            }
            else
            {
                num1 = b; num2 = a;
            }

            for (long i = 1; i < num2; i++)
            {
                if ((num1 * i) % num2 == 0)
                {
                    return i * num1;
                }
            }
            return num1 * num2;
        }
    }

    public class Moon {
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int XVelocity { get; set; } = 0;
        public int YVelocity { get; set; } = 0;
        public int ZVelocity { get; set; } = 0;

        public void Move(){
            this.X += XVelocity;
            this.Y += YVelocity;
            this.Z += ZVelocity;
        }

        public int CalculateEnergy() {
            var potential = Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
            var kinetic = Math.Abs(XVelocity) + Math.Abs(YVelocity) + Math.Abs(ZVelocity);
            return potential * kinetic;
        }
    }
}
