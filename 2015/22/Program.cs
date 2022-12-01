var shieldEffect = new Effect{ duration = 6, armour = 7};
var poisonEffect = new Effect{ duration = 6, damage = 3};
var rechargeEffect = new Effect{ duration = 5, manaRegen = 101};

var spells = new List<Spell>{
    new Spell{ manaCost = 53, damage = 4},
    new Spell{ manaCost = 73, damage = 2, heal = 2},
    new Spell{ manaCost = 113, effect = shieldEffect},
    new Spell{ manaCost = 173, effect = poisonEffect},
    new Spell{ manaCost = 229, effect = rechargeEffect}
};

var minMana = int.MaxValue;

while (true)
{
    (int hp, int mana, int armour, ICollection<Effect> effects) me = (50, 500, 0, new List<Effect>());
    (int hp, int damage, ICollection<Effect> effects) boss = (58, 9, new List<Effect>());

    var spentMana = 0;

    while(me.hp > 0 && boss.hp > 0 && me.mana >= 53)
    {
        // simulate 
    }

    if (me.hp <=0 || me.mana < 53)
    {
        // Console.WriteLine("I lost");
    }
    else if (boss.hp <= 0 && spentMana < minMana)
    {
        minMana = spentMana;
        Console.WriteLine($"I won, and spent {spentMana}");
    }
}








class Spell {
    public int manaCost { get; set; }
    public int damage { get; set; }
    public int heal { get; set; }
    public Effect effect { get; set; }
}

class Effect {
    public int duration { get; set; }
    public int remainingTime { get; set; }
    public int damage { get; set; }
    public int manaRegen { get; set; }
    public int armour { get; set; }
}