
var weapons = new List<Item>{
    new Item{ cost = 8, damage = 4 },
    new Item{ cost = 10, damage = 5},
    new Item{ cost = 25, damage = 6},
    new Item{ cost = 40, damage = 7},
    new Item{ cost = 74, damage = 8}
};

var armour = new List<Item>{
    new Item{ cost = 13, armour = 1},
    new Item{ cost = 31, armour = 2},
    new Item{ cost = 53, armour = 3},
    new Item{ cost = 75, armour = 4},
    new Item{ cost = 102, armour = 5}
};

var rings = new List <Item>{
    new Item{ cost = 25, damage = 1},
    new Item{ cost = 50, damage = 2},
    new Item{ cost = 100, damage = 3},
    new Item{ cost = 20, armour = 1},
    new Item{ cost = 40, armour = 2},
    new Item{ cost = 80, armour = 3}
};

ICollection<ICollection<int>> ringCombos = new List<ICollection<int>>{
    new List<int> {},

    new List<int> {0},
    new List<int> {1},
    new List<int> {2},
    new List<int> {3},
    new List<int> {4},
    new List<int> {5},

    new List<int> {0, 1},
    new List<int> {0, 2},
    new List<int> {0, 3},
    new List<int> {0, 4},
    new List<int> {0, 5},
    
    new List<int> {1, 2},
    new List<int> {1, 3},
    new List<int> {1, 4},
    new List<int> {1, 5},
    
    new List<int> {2, 3},
    new List<int> {2, 4},
    new List<int> {2, 5},

    new List<int> {3, 4},
    new List<int> {3, 5},
    
    new List<int> {4, 5}
};

var minspend = int.MaxValue;
var maxspend = int.MinValue;

foreach(var weapon in weapons)
{
    foreach (var armorment in armour)
    {
        foreach(var ringCombo in ringCombos)
        {
            var selectedRings = ringCombo.Select(r => rings[r]);
            var goldSpent = weapon.cost + armorment.cost + selectedRings.Sum(r => r.cost);

            (int hp, int damage, int armour) boss = (103, 9, 2);
            (int hp, int damage, int armour) me = (100, weapon.damage + selectedRings.Sum(r => r.damage), armorment.armour + selectedRings.Sum(r => r.armour));

            var turn = 0;

            while( me.hp > 0 && boss.hp > 0)
            {
                if (turn % 2 == 0)
                {
                    // my turn
                    boss = (boss.hp - Math.Max(me.damage - boss.armour, 1), boss.damage, boss.armour);
                }
                else
                {
                    // boss turn
                    me = (me.hp - Math.Max(boss.damage - me.armour, 1), me.damage, me.armour);
                }

                turn++;
            }

            if (me.hp > 0 && goldSpent < minspend)
            {
                Console.WriteLine($"I won after spending {goldSpent}");
                minspend = goldSpent;
            }
            if (boss.hp > 0 && goldSpent > maxspend)
            {
                Console.WriteLine($"I lost after spending {goldSpent}");
                maxspend = goldSpent;
            }
        }
    }
}





class Item
{
    public  int cost { get; set; }
    public string name { get; set; }
    public int damage { get; set; }

    public int armour { get; set; }
}