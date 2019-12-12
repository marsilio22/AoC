using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_12
{
    public class Program
    {
        static void Main (string[] args) {
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

            // RESET (TODO METHOD)
            moons = new List<Moon>();
            i = 0;

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

            time = 0;



            var Xs = new List<(int IOx, int IOxV, int CAx, int CAxV, int GAx, int GAxV, int EUx, int EUxV)>();
            var Ys = new List<(int IOy, int IOyV, int CAy, int CAyV, int GAy, int GAyV, int EUy, int EUyV)>();
            var Zs = new List<(int IOz, int IOzV, int CAz, int CAzV, int GAz, int GAzV, int EUz, int EUzV)>();
            var IO = moons.Single(m => m.Name.Equals("IO"));
            var CALLISTO = moons.Single(m => m.Name.Equals("CALLISTO"));
            var GANYMEDE = moons.Single(m => m.Name.Equals("GANYMEDE"));
            var EUROPA = moons.Single(m => m.Name.Equals("EUROPA"));

            Xs.Add((IO.X, IO.XVelocity, CALLISTO.X, CALLISTO.XVelocity, GANYMEDE.X, GANYMEDE.XVelocity, EUROPA.X, EUROPA.XVelocity));
            Ys.Add((IO.Y, IO.YVelocity, CALLISTO.Y, CALLISTO.YVelocity, GANYMEDE.Y, GANYMEDE.YVelocity, EUROPA.Y, EUROPA.YVelocity));
            Zs.Add((IO.Z, IO.ZVelocity, CALLISTO.Z, CALLISTO.ZVelocity, GANYMEDE.Z, GANYMEDE.ZVelocity, EUROPA.Z, EUROPA.ZVelocity));

            while(time < 1000000){
                MoveMoons(moons);
                IO = moons.Single(m => m.Name.Equals("IO"));
                CALLISTO = moons.Single(m => m.Name.Equals("CALLISTO"));
                GANYMEDE = moons.Single(m => m.Name.Equals("GANYMEDE"));
                EUROPA = moons.Single(m => m.Name.Equals("EUROPA"));

                Xs.Add((IO.X, IO.XVelocity, CALLISTO.X, CALLISTO.XVelocity, GANYMEDE.X, GANYMEDE.XVelocity, EUROPA.X, EUROPA.XVelocity));
                Ys.Add((IO.Y, IO.YVelocity, CALLISTO.Y, CALLISTO.YVelocity, GANYMEDE.Y, GANYMEDE.YVelocity, EUROPA.Y, EUROPA.YVelocity));
                Zs.Add((IO.Z, IO.ZVelocity, CALLISTO.Z, CALLISTO.ZVelocity, GANYMEDE.Z, GANYMEDE.ZVelocity, EUROPA.Z, EUROPA.ZVelocity));

                time++;
            }


            int indexOfRepeat = 0;

            var xPeriod = Xs.IndexOf(Xs[0], 1);
            var yPeriod = Ys.IndexOf(Ys[0], 1);
            var zPeriod = Zs.IndexOf(Zs[0], 1);

            long result = determineLCM(determineLCM(xPeriod, yPeriod), zPeriod);
            // figure out index of the first repeat, and then the period of the X, Y, and Z's for each moon.
            // the answer will be the LCM of the LCM's of the XYZ for each moon, plus that initial bit.



            // 8950163991284645888 is too big lol
            Console.WriteLine(result);
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

        public (int x, int y, int z, int velx, int vely, int velz) Data => (X, Y, Z, XVelocity, YVelocity, ZVelocity);
    }
}
