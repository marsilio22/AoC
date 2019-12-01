using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_1
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadLines("./input.txt");

            var fuels = new List<int>();
            // The input is a list of module weights. We want the fuel required to launch those modules.
            // Fuel is calculated as moduleweight / 3, rounded down, and minus 2
            foreach(var line in input){
                var doubleLine = double.Parse(line);
                fuels.Add((int)Math.Floor(doubleLine / 3d) - 2);
            }

            // total fuel:
            var totalFuel = fuels.Sum();
            Console.WriteLine(totalFuel); // part one

            // Now we must calculate the fuel required to launch the fuel we just calculated,
            // calculate this on a per module basis. only count fuels when the remainder is > 0
            // add fuel for each step of fuel which has been added previously.
            for(int i = 0; i < fuels.Count; i++){
                int remainder = fuels[i];
                while (remainder > 0){
                    remainder = (int)(Math.Floor((double)remainder / 3d) - 2);
                    fuels[i] += remainder > 0 ? remainder : 0;
                }
            }

            // new total fuel:
            totalFuel = fuels.Sum(); 
            Console.WriteLine(totalFuel); // part two
        }
    }
}
