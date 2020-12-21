using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_21
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadLines("./input.txt");

            var allergens = new Dictionary<string, string>();
            var pendingAllergens = new Dictionary<int, (ICollection<string> ing, ICollection<string> allg)>();
            var index = 0;
            foreach(var line in lines)
            {
                var spl = line.Split(" (");

                ICollection<string> allergs = new List<string>();
                if (spl.Count() > 1)
                {
                    allergs = spl[1].Split(')')[0].Substring(9).Split(' ').Select(c => c.Split(',')[0]).ToList();
                }

                var ingredients = spl[0].Split(' ');

                pendingAllergens[index] = (ingredients, allergs);
                index++;
            }

            var allAllergens = pendingAllergens.SelectMany(c => c.Value.allg).Distinct().ToList();

            while (allAllergens.Any())
            {
                
                foreach(var allergen in allAllergens)
                {
                    var allIngredients = pendingAllergens.Where(c => c.Value.allg.Contains(allergen)).Select(c => c.Value.ing).ToList();
                    List<string> ingredients = allIngredients.First().ToList();

                    foreach(var ingList in allIngredients)
                    {
                        ingredients = ingredients.Intersect(ingList).ToList();
                    }

                    foreach(var knownAllergens in allergens){
                        if (ingredients.Contains(knownAllergens.Key))
                        {
                            ingredients.Remove(knownAllergens.Key);
                        }
                    }

                    if (ingredients.Count == 1){
                        allergens[ingredients[0]] = allergen;
                    }
                }

                allAllergens = allAllergens.Where(a => !allergens.Values.Contains(a)).ToList();
            }

            var safeIngredients = pendingAllergens.SelectMany(i => i.Value.ing).Distinct().Where(i => !allergens.Keys.Contains(i));

            var ans = 0;

            foreach(var pair in pendingAllergens)
            {
                var ings = pair.Value.ing;
                ans += ings.Count(c => safeIngredients.Contains(c));
            }

            Console.WriteLine(ans);

            // look at me go, doing part 2 in part 1 ;)
            var orderedAllergens = allergens.OrderBy(a => a.Value);
            string str = string.Empty;
            foreach(var a in orderedAllergens){
                str += a.Key + ",";
            }

            str = str.Substring(0, str.Length - 1);

            Console.WriteLine(str);
        }
    }
}
