using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_14 {
    public class Chemical {
        public string Name { get; set; }
        public List < (double amount, string name) > ProducedBy { get; set; }
        public int AmountProduced { get; set; }
    }

    class Program {
        static void Main (string[] args) {
            var input = File.ReadAllLines ("./input.txt");
            int output = 0;
            // Part 1 answer = 31
            output = Part1(new [] {"10 ORE => 10 A", "1 ORE => 1 B", "7 A, 1 B => 1 C", "7 A, 1 C => 1 D", "7 A, 1 D => 1 E", "7 A, 1 E => 1 FUEL"}); 
            Console.WriteLine($"Expected = {31}, Actual = {output}");

            // Part 1 answer = 165
            output = Part1(new [] {"9 ORE => 2 A", "8 ORE => 3 B", "7 ORE => 5 C", "3 A, 4 B => 1 AB", "5 B, 7 C => 1 BC", "4 C, 1 A => 1 CA", "2 AB, 3 BC, 4 CA => 1 FUEL"});
            Console.WriteLine($"Expected = {165}, Actual = {output}");
            
            // Part 1 answer = 13312
            output = Part1(new [] {"157 ORE => 5 NZVS", "165 ORE => 6 DCFZ", "44 XJWVT, 5 KHKGT, 1 QDVJ, 29 NZVS, 9 GPVTF, 48 HKGWZ => 1 FUEL", "12 HKGWZ, 1 GPVTF, 8 PSHF => 9 QDVJ", "179 ORE => 7 PSHF", "177 ORE => 5 HKGWZ", "7 DCFZ, 7 PSHF => 2 XJWVT", "165 ORE => 2 GPVTF", "3 DCFZ, 7 NZVS, 5 HKGWZ, 10 PSHF => 8 KHKGT"});
            Console.WriteLine($"Expected = {13312}, Actual = {output}");

            // Part 1 answer = 180697
            output = Part1(new []{"2 VPVL, 7 FWMGM, 2 CXFTF, 11 MNCFX => 1 STKFG", "17 NVRVD, 3 JNWZP => 8 VPVL", "53 STKFG, 6 MNCFX, 46 VJHF, 81 HVMC, 68 CXFTF, 25 GNMV => 1 FUEL", "22 VJHF, 37 MNCFX => 5 FWMGM", "139 ORE => 4 NVRVD", "144 ORE => 7 JNWZP", "5 MNCFX, 7 RFSQX, 2 FWMGM, 2 VPVL, 19 CXFTF => 3 HVMC", "5 VJHF, 7 MNCFX, 9 VPVL, 37 CXFTF => 6 GNMV", "145 ORE => 6 MNCFX", "1 NVRVD => 8 CXFTF", "1 VJHF, 6 MNCFX => 4 RFSQX", "176 ORE => 6 VJHF"});
            Console.WriteLine($"Expected = {180697}, Actual = {output}");

            // Part 1 answer = 2210736
            output = Part1(new [] {"171 ORE => 8 CNZTR", "7 ZLQW, 3 BMBT, 9 XCVML, 26 XMNCP, 1 WPTQ, 2 MZWV, 1 RJRHP => 4 PLWSL", "114 ORE => 4 BHXH", "14 VRPVC => 6 BMBT", "6 BHXH, 18 KTJDG, 12 WPTQ, 7 PLWSL, 31 FHTLT, 37 ZDVW => 1 FUEL", "6 WPTQ, 2 BMBT, 8 ZLQW, 18 KTJDG, 1 XMNCP, 6 MZWV, 1 RJRHP => 6 FHTLT", "15 XDBXC, 2 LTCX, 1 VRPVC => 6 ZLQW", "13 WPTQ, 10 LTCX, 3 RJRHP, 14 XMNCP, 2 MZWV, 1 ZLQW => 1 ZDVW", "5 BMBT => 4 WPTQ", "189 ORE => 9 KTJDG", "1 MZWV, 17 XDBXC, 3 XCVML => 2 XMNCP", "12 VRPVC, 27 CNZTR => 2 XDBXC", "15 KTJDG, 12 BHXH => 5 XCVML", "3 BHXH, 2 VRPVC => 7 MZWV", "121 ORE => 7 VRPVC", "7 XCVML => 6 RJRHP", "5 BHXH, 4 VRPVC => 5 LTCX"});
            Console.WriteLine($"Expected = {2210736}, Actual = {output}");

            
        }

        public static int Part1(ICollection<string> input){
            var reactions = new List<Chemical> ();

            foreach (var line in input) {
                var parts = line.Split (" => ");
                var inputsString = parts[0];
                var outputsString = parts[1];

                var inputs = inputsString.Split (", ").Select (i => (double.Parse (i.Split (' ') [0]), i.Split (' ') [1]));

                var outputs = outputsString.Split (' ');

                reactions.Add (new Chemical {
                    Name = outputs[1],
                        AmountProduced = int.Parse (outputs[0]),
                        ProducedBy = inputs.ToList ()
                });
            }

            var fuel = reactions.Single (r => r.Name.Equals ("FUEL"));

            var baseUnits = WhatCanIMakeWith (reactions, new List<string> { "ORE" }).Select (c => c.Name).ToList ();

            var spareChems = new Dictionary<string, int>();

            while (true) // todo
            {
                var currentFuelProducedBy = fuel.ProducedBy.Where (p => !baseUnits.Contains (p.name)).ToList ();
                if (currentFuelProducedBy.Count == 0) {
                    break;
                }

                foreach (var fuelProductionChemical in currentFuelProducedBy) {
                    var chemFuelProductionChemical = reactions.Single (r => r.Name.Equals (fuelProductionChemical.name));
                    // Remove the chemical from the fuel producedBy and replace it with the things that it is producedBy
                    fuel.ProducedBy.Remove (fuelProductionChemical);
                    fuel.ProducedBy.AddRange(
                        chemFuelProductionChemical.ProducedBy.Select(
                            production => 
                            { 
                                double amountToProduce = production.amount;
                                if (spareChems.ContainsKey(production.name) && production.amount > spareChems[production.name]){
                                    amountToProduce -= spareChems[production.name];
                                    spareChems[production.name] = 0;
                                }
                                else if (spareChems.ContainsKey(production.name) && production.amount < spareChems[production.name]){
                                    amountToProduce = 0;
                                    spareChems[production.name] = (int)(spareChems[production.name] - production.amount);
                                }

                                return ((double) amountToProduce  * Math.Ceiling(fuelProductionChemical.amount / (double) chemFuelProductionChemical.AmountProduced), production.name); 
                            }));

                    List<(string chemName, int amount)> newSpareChems = chemFuelProductionChemical.ProducedBy.Select(production => (production.name, chemFuelProductionChemical.AmountProduced % (int)fuelProductionChemical.amount)).ToList();
                    foreach (var chem in newSpareChems){
                        spareChems.TryGetValue(chem.chemName, out int spareAmount);
                        spareChems[chem.chemName] = spareAmount + chem.amount;
                    }
                }
            }

            var totalOre = 0d;

            foreach (var chem in baseUnits) {
                var chemchem = reactions.Single (r => r.Name.Equals (chem));

                var totalAmountNeeded = fuel.ProducedBy.Where (p => p.name.Equals (chem)).Select (p => p.amount).Sum ();

                var numberToProduce = Math.Ceiling (totalAmountNeeded / chemchem.AmountProduced);
                totalOre += numberToProduce * chemchem.ProducedBy.Single ().amount;
            }

            return (int)totalOre;
        }

        public static void DrawTree (List<Chemical> reactions) {
            var creatableChemicals = new List<string> { "ORE" };
            Console.WriteLine ("ORE,");

            while (!creatableChemicals.Contains ("FUEL")) {
                var newCreatableChemicals = WhatCanIMakeWith (reactions, creatableChemicals);

                foreach (var newChem in newCreatableChemicals.Where (n => !creatableChemicals.Contains (n.Name))) {
                    Console.Write (newChem.Name + ", ");
                }
                Console.WriteLine ();

                creatableChemicals = creatableChemicals.Union (newCreatableChemicals.Select (c => c.Name)).ToList ();
            }
        }

        // public static void DoTHing(int amountOfOre, List<Chemical> reactions){

        // }

        // This horribly overestimates, because it doesn't keep track of the amount of stuff it will need to make to make earlier stuff.
        // e.g. for the first worked example, it mines A 4 times instead of 3, because C, D, E and FUEL all need 7A, but by the time
        // it has mined A three times (30) it can do all four reactions (28 required).
        public static (double oreToProduce, int amountProduced) OreToProduce (Chemical target, List<Chemical> reactions /*, Dictionary<string, (int produced, int used)> chemicalAmountData*/ ) {
            if (target.ProducedBy.Count == 1 && target.ProducedBy.Single ().name.Equals ("ORE")) {
                return (target.ProducedBy.Single ().amount, target.AmountProduced);
            } else {
                var totalOreToProduce = 0d;
                foreach (var productionChemicalAmount in target.ProducedBy) {
                    if (productionChemicalAmount.name.Equals ("ORE")) {
                        totalOreToProduce += productionChemicalAmount.amount;
                    } else {
                        Chemical subTarget = reactions.Single (r => r.Name.Equals (productionChemicalAmount.name));
                        var oreForProductionChemical = OreToProduce (subTarget, reactions);
                        var amountProduced = 0;
                        while (amountProduced < productionChemicalAmount.amount) {
                            totalOreToProduce += oreForProductionChemical.oreToProduce;
                            amountProduced += oreForProductionChemical.amountProduced;
                        }
                    }
                }

                return (totalOreToProduce, target.AmountProduced);
            }
        }

        public static List<Chemical> WhatCanIMakeWith (List<Chemical> reactions, List<string> currentIngredientNames) {
            return reactions.Where (r => r.ProducedBy.All (p => currentIngredientNames.Contains (p.name))).ToList ();
        }

    }
}