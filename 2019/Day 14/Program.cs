using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_14 {
    public class Chemical {
        public string Name { get; set; }
        public List < (int requiredToProduce, string name) > ProducedBy { get; set; }
        public int AmountProduced { get; set; }
    }

    class Program {
        public static Dictionary<string, int> spareChems;
        public static void Main (string[] args) {
            var input = File.ReadAllLines ("./input.txt");
            long output = 0;
            // Part 1 answer = 31
            output = Part1 (new [] { "10 ORE => 10 A", "1 ORE => 1 B", "7 A, 1 B => 1 C", "7 A, 1 C => 1 D", "7 A, 1 D => 1 E", "7 A, 1 E => 1 FUEL" });
            Console.WriteLine ($"Expected = {31}, Actual = {output}");

            // Part 1 answer = 165
            output = Part1 (new [] { "9 ORE => 2 A", "8 ORE => 3 B", "7 ORE => 5 C", "3 A, 4 B => 1 AB", "5 B, 7 C => 1 BC", "4 C, 1 A => 1 CA", "2 AB, 3 BC, 4 CA => 1 FUEL" });
            Console.WriteLine ($"Expected = {165}, Actual = {output}");

            // Part 1 answer = 13312
            output = Part1 (new [] { "157 ORE => 5 NZVS", "165 ORE => 6 DCFZ", "44 XJWVT, 5 KHKGT, 1 QDVJ, 29 NZVS, 9 GPVTF, 48 HKGWZ => 1 FUEL", "12 HKGWZ, 1 GPVTF, 8 PSHF => 9 QDVJ", "179 ORE => 7 PSHF", "177 ORE => 5 HKGWZ", "7 DCFZ, 7 PSHF => 2 XJWVT", "165 ORE => 2 GPVTF", "3 DCFZ, 7 NZVS, 5 HKGWZ, 10 PSHF => 8 KHKGT" });
            Console.WriteLine ($"Expected = {13312}, Actual = {output}");

            // Part 1 answer = 180697
            output = Part1 (new [] { "2 VPVL, 7 FWMGM, 2 CXFTF, 11 MNCFX => 1 STKFG", "17 NVRVD, 3 JNWZP => 8 VPVL", "53 STKFG, 6 MNCFX, 46 VJHF, 81 HVMC, 68 CXFTF, 25 GNMV => 1 FUEL", "22 VJHF, 37 MNCFX => 5 FWMGM", "139 ORE => 4 NVRVD", "144 ORE => 7 JNWZP", "5 MNCFX, 7 RFSQX, 2 FWMGM, 2 VPVL, 19 CXFTF => 3 HVMC", "5 VJHF, 7 MNCFX, 9 VPVL, 37 CXFTF => 6 GNMV", "145 ORE => 6 MNCFX", "1 NVRVD => 8 CXFTF", "1 VJHF, 6 MNCFX => 4 RFSQX", "176 ORE => 6 VJHF" });
            Console.WriteLine ($"Expected = {180697}, Actual = {output}");

            // Part 1 answer = 2210736
            output = Part1 (new [] { "171 ORE => 8 CNZTR", "7 ZLQW, 3 BMBT, 9 XCVML, 26 XMNCP, 1 WPTQ, 2 MZWV, 1 RJRHP => 4 PLWSL", "114 ORE => 4 BHXH", "14 VRPVC => 6 BMBT", "6 BHXH, 18 KTJDG, 12 WPTQ, 7 PLWSL, 31 FHTLT, 37 ZDVW => 1 FUEL", "6 WPTQ, 2 BMBT, 8 ZLQW, 18 KTJDG, 1 XMNCP, 6 MZWV, 1 RJRHP => 6 FHTLT", "15 XDBXC, 2 LTCX, 1 VRPVC => 6 ZLQW", "13 WPTQ, 10 LTCX, 3 RJRHP, 14 XMNCP, 2 MZWV, 1 ZLQW => 1 ZDVW", "5 BMBT => 4 WPTQ", "189 ORE => 9 KTJDG", "1 MZWV, 17 XDBXC, 3 XCVML => 2 XMNCP", "12 VRPVC, 27 CNZTR => 2 XDBXC", "15 KTJDG, 12 BHXH => 5 XCVML", "3 BHXH, 2 VRPVC => 7 MZWV", "121 ORE => 7 VRPVC", "7 XCVML => 6 RJRHP", "5 BHXH, 4 VRPVC => 5 LTCX" });
            Console.WriteLine ($"Expected = {2210736}, Actual = {output}");

            // holy moly, the real input
            output = Part1 (input);
            Console.WriteLine (output);

            output = Part2(input);
            Console.WriteLine(output);
        }

        public static List<Chemical> ProduceInput(ICollection<string> input){
            var reactions = new List<Chemical> ();

            foreach (var line in input) {
                var parts = line.Split (" => ");
                var inputsString = parts[0];
                var outputsString = parts[1];

                var inputs = inputsString.Split (", ").Select (i => (int.Parse (i.Split (' ') [0]), i.Split (' ') [1]));

                var outputs = outputsString.Split (' ');

                reactions.Add (new Chemical {
                    Name = outputs[1],
                        AmountProduced = int.Parse (outputs[0]),
                        ProducedBy = inputs.ToList ()
                });
            }

            return reactions;
        }

        public static long Part1 (ICollection<string> input) {
            var reactions = ProduceInput(input);
            var fuel = reactions.Single (r => r.Name.Equals ("FUEL"));

            // rassafrassa public static stuff blehhhh
            spareChems = new Dictionary<string, int> ();
            var oreRequired = OreToProduce (fuel, reactions, 1);

            return oreRequired;
        }

        public static long Part2 (ICollection<string> input) {
            var reactions = ProduceInput(input);
            var fuel = reactions.Single (r => r.Name.Equals ("FUEL"));

// > 199999
            var fuelCount = 1;
            var fuelIncrement = 99999;
            var previousOreRequired = OreToProduce(fuel, reactions, fuelCount);
            while(true){
                spareChems = new Dictionary<string, int> ();
                var newOreRequired = OreToProduce(fuel, reactions, fuelCount + fuelIncrement);
                Console.WriteLine(newOreRequired);
                if (newOreRequired > 1000000000000 && previousOreRequired < 1000000000000 ||
                    newOreRequired < 1000000000000 && previousOreRequired > 1000000000000){
                    Console.WriteLine(fuelCount);
                    if (Math.Abs(fuelIncrement) > 3){
                        Console.WriteLine("Reduce increment and multiply by -1");
                        fuelIncrement /= 3;
                        fuelIncrement *= -1;
                    }
                }

                if (Math.Abs(fuelIncrement) < 3333){
                    Console.WriteLine($"{fuelCount} produced ore {newOreRequired}");
                }

                previousOreRequired = newOreRequired;
                fuelCount+= fuelIncrement;
            }
            return fuelCount;
        }

        public static long OreToProduce (Chemical target, List<Chemical> reactions, int amountNeeded) {
            var amountProduced = 0;
            long totalOreToProduce = 0;

            // if the target is produced SOLELY by ORE, then we can figure out exactly how much ORE 
            // we need to spend to produce the AMOUNTNEEDED, then work out the remainder to add 
            // to SPARECHEMS
            if (target.ProducedBy.Count == 1 && target.ProducedBy.Single ().name.Equals ("ORE")) {
                spareChems.TryGetValue (target.Name, out var spareAmount);
                while (amountProduced + spareAmount < amountNeeded) {
                    amountProduced += target.AmountProduced;
                    totalOreToProduce += target.ProducedBy.Single ().requiredToProduce;
                }

                spareChems[target.Name] = spareAmount + amountProduced - amountNeeded;
            } else {
                // Otherwise, the chemical is NOT SOLELY PRODUCED BY ORE. We need to determine how much ORE we need 
                // to spend to create EACH of it's constituent parts. 
                spareChems.TryGetValue (target.Name, out var spareAmount);
                while (amountProduced + spareAmount < amountNeeded) {
                    amountProduced += target.AmountProduced;
                    foreach (var chem in target.ProducedBy) {
                        Chemical chemchem = reactions.Single (c => c.Name.Equals (chem.name));
                        totalOreToProduce += OreToProduce (chemchem, reactions, chem.requiredToProduce);
                    }
                }
                spareChems[target.Name] = spareAmount + amountProduced - amountNeeded;
            }
            return totalOreToProduce;
        }
    }
}