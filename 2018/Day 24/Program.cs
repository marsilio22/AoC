using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace Day_24
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = new[]
            {
                "Immune System:",
                "5711 units each with 6662 hit points (immune to fire; weak to slashing) with an attack that does 9 bludgeoning damage at initiative 14",
                "2108 units each with 8185 hit points (weak to radiation, bludgeoning) with an attack that does 36 slashing damage at initiative 13",
                "1590 units each with 3940 hit points with an attack that does 24 cold damage at initiative 5",
                "2546 units each with 6960 hit points with an attack that does 25 slashing damage at initiative 2",
                "1084 units each with 3450 hit points (immune to bludgeoning) with an attack that does 27 slashing damage at initiative 11",
                "265 units each with 8223 hit points (immune to radiation, bludgeoning, cold) with an attack that does 259 cold damage at initiative 12",
                "6792 units each with 6242 hit points (immune to slashing; weak to bludgeoning, radiation) with an attack that does 9 slashing damage at initiative 18",
                "3336 units each with 12681 hit points (weak to slashing) with an attack that does 28 fire damage at initiative 6",
                "752 units each with 5272 hit points (immune to slashing; weak to bludgeoning, radiation) with an attack that does 69 radiation damage at initiative 4",
                "96 units each with 7266 hit points (immune to fire) with an attack that does 738 bludgeoning damage at initiative 8",

                "Infection:",
                "1492 units each with 47899 hit points (weak to fire, slashing; immune to cold) with an attack that does 56 bludgeoning damage at initiative 15",
                "3065 units each with 39751 hit points (weak to bludgeoning, slashing) with an attack that does 20 slashing damage at initiative 1",
                "7971 units each with 35542 hit points (weak to bludgeoning, radiation) with an attack that does 8 bludgeoning damage at initiative 10",
                "585 units each with 5936 hit points (weak to cold; immune to fire) with an attack that does 17 slashing damage at initiative 17",
                "2449 units each with 37159 hit points (immune to cold) with an attack that does 22 cold damage at initiative 7",
                "8897 units each with 6420 hit points (immune to bludgeoning, slashing, fire; weak to radiation) with an attack that does 1 bludgeoning damage at initiative 19",
                "329 units each with 31704 hit points (weak to fire; immune to cold, radiation) with an attack that does 179 bludgeoning damage at initiative 16",
                "6961 units each with 11069 hit points (weak to fire) with an attack that does 2 radiation damage at initiative 20",
                "2837 units each with 29483 hit points (weak to cold) with an attack that does 20 bludgeoning damage at initiative 9",
                "8714 units each with 7890 hit points with an attack that does 1 cold damage at initiative 3"
            };

            //var lines = new[]
            //{
            //    "Immune System:",
            //    "17 units each with 5390 hit points (weak to radiation, bludgeoning) with an attack that does 4507 fire damage at initiative 2",
            //    "989 units each with 1274 hit points (immune to fire; weak to bludgeoning, slashing) with an attack that does 25 slashing damage at initiative 3",

            //    "Infection:",
            //    "801 units each with 4706 hit points (weak to radiation) with an attack that does 116 bludgeoning damage at initiative 1",
            //    "4485 units each with 2961 hit points (immune to radiation; weak to fire, cold) with an attack that does 12 slashing damage at initiative 4"
            //};

            var units = new List<Unit>();
            var allegience = Allegience.ImmuneSystem;
            foreach (var line in lines)
            {
                if (line.Equals("Infection:"))
                {
                    allegience = Allegience.Infection;
                }

                if (line.Split(' ').Length < 3)
                {
                    continue;
                }

                var splitLine = line.Split(' ');

                var count = int.Parse(splitLine[0]);
                var hp = int.Parse(splitLine[4]);

                var immunities = new List<AttackType>();
                var weaknesses = new List<AttackType>();
                if (line.Contains('('))
                {
                    // immune to bludgeoning, slashing, fire; weak to radiation, cold
                    var weaknessesAndImmunities = line.Split('(')[1].Split(')')[0].Split("; ").SelectMany(s => s.Trim().Split(',')).SelectMany(s => s.Trim().Split(' ')).ToArray();

                    List<AttackType> currentList = new List<AttackType>(); // this will always be overwritten by either the weakness or immunity list

                    for (int i = 0; i < weaknessesAndImmunities.Length; i++)
                    {
                        if (weaknessesAndImmunities[i].Equals("weak"))
                        {
                            currentList = weaknesses;
                        }

                        if (weaknessesAndImmunities[i].Equals("immune"))
                        {
                            currentList = immunities;
                        }

                        if (Enum.TryParse(typeof(AttackType), weaknessesAndImmunities[i], true, out var woi))
                        {
                            currentList.Add((AttackType)woi);
                        }
                        else if (!weaknessesAndImmunities[i].Equals("to") && 
                                 !weaknessesAndImmunities[i].Equals("weak") &&
                                 !weaknessesAndImmunities[i].Equals("immune"))
                        {
                            throw new Exception();
                        }
                    }
                }

                var attack = line.Split("does ")[1].Split(' ');

                var ap = int.Parse(attack[0]);
                AttackType at = (AttackType)Enum.Parse(typeof(AttackType), attack[1]);
                var init = int.Parse(attack.Last());

                var unit = new Unit
                {
                    Allegience = allegience,
                    Count = count,
                    HP = hp,
                    Immunities = immunities,
                    Weaknesses = weaknesses,
                    AttackType = at,
                    AttackDamage = ap,
                    Initiative = init
                };
                units.Add(unit);
            }

            var originalUnits = units.Select(u => new Unit
            {
                Allegience = u.Allegience,
                AttackType = u.AttackType,
                AttackDamage = u.AttackDamage,
                Count = u.Count,
                HP = u.HP,
                Immunities = u.Immunities,
                Initiative = u.Initiative,
                Weaknesses = u.Weaknesses
            }).ToList();

            var boost = 0;
            while (true)
            {
                boost++;
                units = originalUnits.Select(u => new Unit
                {
                    Allegience = u.Allegience,
                    AttackType = u.AttackType,
                    AttackDamage = u.AttackDamage + (u.Allegience == Allegience.ImmuneSystem ? boost : 0),
                    Count = u.Count,
                    HP = u.HP,
                    Immunities = u.Immunities,
                    Initiative = u.Initiative,
                    Weaknesses = u.Weaknesses
                }).ToList();

                Console.WriteLine($"Starting boost = {boost}");
                while (units.Select(u => u.Allegience).Distinct().Count() > 1)
                {
                    var immuneSystem = units.Where(u => u.Allegience == Allegience.ImmuneSystem);
                    var infection = units.Where(u => u.Allegience == Allegience.Infection);

                    var immuneSystemAttacks = immuneSystem.Select(u => u.AttackType).Distinct().ToList();
                    var infectionAttacks = infection.Select(u => u.AttackType).Distinct().ToList();

                    int totallyImmuneUnits = 0;
                    foreach (var unit in immuneSystem)
                    {
                        if (infectionAttacks.All(a => unit.Immunities.Contains(a)))
                        {
                            totallyImmuneUnits++;
                        }
                    }
                    foreach (var unit in infection)
                    {
                        if (immuneSystemAttacks.All(a => unit.Immunities.Contains(a)))
                        {
                            totallyImmuneUnits++;
                        }
                    }

                    if (totallyImmuneUnits == units.Count())
                    {
                        Console.WriteLine($"boost {boost} resulted in an infinite loop");
                        break;
                    }
                    

                    units = units.OrderByDescending(u => u.EffectivePower).ThenByDescending(u => u.Initiative).ToList();

                    List<(Unit attacker, Unit target)> combatRound = new List<(Unit attacker, Unit target)>();

                    //target selection;
                    foreach (var unit in units)
                    {
                        var validTargets = units.Where(u => u.Allegience != unit.Allegience);
                        var ep = unit.EffectivePower;

                        var damage = validTargets.Select(u =>
                                new KeyValuePair<long, Unit>(
                                    u.Immunities.Contains(unit.AttackType) ? 0 :
                                    u.Weaknesses.Contains(unit.AttackType) ? 2 * ep : ep, u)).ToLookup(u => u.Key)
                            .OrderByDescending(c => c.Key);

                        Unit target = null;
                        for (int i = 0; i < damage.Count(); i++)
                        {
                            if (damage.ToList()[i].Key != 0)
                            {
                                target = damage
                                    .ToList()[i]
                                    .Select(c => c.Value)
                                    .Where(c => !combatRound.Select(r => r.target).Contains(c))
                                    .OrderByDescending(u => u.EffectivePower)
                                    .ThenByDescending(u => u.Initiative)
                                    .FirstOrDefault();
                            }

                            if (target != null)
                            {
                                break;
                            }
                        }


                        if (target != null)
                        {
                            combatRound.Add((unit, target));
                        }
                    }

                    if (combatRound.Count == 0)
                    {
                        break;
                    }

                    combatRound = combatRound.OrderByDescending(c => c.attacker.Initiative).ToList();
                    bool anythingChanged = false;
                    //attack
                    foreach (var round in combatRound)
                    {
                        if (round.attacker.Count > 0 && round.target.Count > 0)
                        {
                            var weaknessMultiplier = round.target.Weaknesses.Contains(round.attacker.AttackType) ? 2 :
                                round.target.Immunities.Contains(round.attacker.AttackType) ? 0 : 1;
                            var remaining =
                                Math.Ceiling(
                                    (double) ((round.target.HP * round.target.Count) -
                                              (round.attacker.EffectivePower * weaknessMultiplier)) / round.target.HP);
                            if (!anythingChanged)
                            {
                                anythingChanged = remaining != round.target.Count;
                            }

                            round.target.Count = (int) remaining;

                            if (remaining <= 0)
                            {
                                units.Remove(round.target);
                            }
                        }
                    }

                    if (!anythingChanged)
                    {
                        Console.WriteLine($"boost {boost} reached a stalemate");
                        break;
                    }
                }
                Console.WriteLine("Infection Units Remaining: " + units.Where(u => u.Allegience == Allegience.Infection).Sum(c => c.Count));
                Console.WriteLine("Immune System Units Remaining: " + units.Where(u => u.Allegience == Allegience.ImmuneSystem).Sum(c => c.Count));

                if (units.Where(u => u.Allegience == Allegience.Infection).Count() == 0)
                {
                    break;
                }
            }

            Console.ReadLine();
        }
    }

    public class Unit
    {
        public int Count { get; set; }
        public int HP { get; set; }
        public Allegience Allegience { get; set; }
        public List<AttackType> Immunities { get; set; }
        public List<AttackType> Weaknesses { get; set; }
        public AttackType AttackType { get; set; }
        public int AttackDamage { get; set; }
        public int Initiative { get; set; }

        public long EffectivePower => AttackDamage * Count;
    }

    public enum Allegience
    {
        ImmuneSystem,
        Infection
    }

    public enum AttackType
    {
        fire,
        slashing,
        bludgeoning,
        radiation,
        cold
    }
}
