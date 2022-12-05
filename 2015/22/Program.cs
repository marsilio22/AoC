using System.Linq.Expressions;

var shieldEffect = new Effect{ id = "shield", initialDuration = 6, armour = 7};
var poisonEffect = new Effect{ id = "poison", initialDuration = 6, damage = 3};
var rechargeEffect = new Effect{ id = "recharge", initialDuration = 5, manaRegen = 101};

var spells = new List<Spell>{
    new Spell{name = "Missle", manaCost = 53, damage = 4},
    new Spell{name = "Drain", manaCost = 73, damage = 2, heal = 2},
    new Spell{name = "Shield", manaCost = 113, effect = shieldEffect},
    new Spell{name = "Poison", manaCost = 173, effect = poisonEffect},
    new Spell{name = "Recharge", manaCost = 229, effect = rechargeEffect}
};

var minMana = int.MaxValue;


Queue<GameState> states = new Queue<GameState>();

states.Enqueue(new GameState{me = new Me{ hp = 50, mana = 500}, boss = new Boss{hp = 58, damage = 9}, activeEffects = new List<Effect>(), spentMana = 0, myTurn = true});


// examples
// states.Enqueue(new GameState{me = new Me{ hp = 50, mana = 500}, boss = new Boss{hp = 71, damage = 10}, activeEffects = new List<Effect>(), spentMana = 0, myTurn = true});
// states.Enqueue(new GameState{ me = new Me{hp =10,mana = 250}, boss = new Boss{hp =13, damage= 8}, activeEffects = new List<Effect>(), spentMana = 0, myTurn = true});
// states.Enqueue(new GameState{ me = new Me{hp =10,mana = 250}, boss = new Boss{hp =14, damage= 8}, activeEffects = new List<Effect>(), spentMana = 0, myTurn = true});

while (states.TryDequeue(out var state))
{
    if (state.spentMana > minMana)
    {
        // Console.WriteLine("Already found better answer");
        continue;
    }

    var me = state.me;
    var boss = state.boss;
    var effects = state.activeEffects;

    if (state.myTurn)
    {
        me = me with {hp = me.hp - 1};
    }

    if (me.hp <= 0)
    {
        continue;
    }
    else if (boss.hp <= 0 && state.spentMana < minMana)
    {
        minMana = state.spentMana;
        Console.WriteLine($"I won, and spent {state.spentMana}");
        continue;
    }

    (me, boss, effects) = ApplyEffects(me, boss, effects);

    if (me.hp <=0)
    {
        continue;
    }
    else if (boss.hp <= 0 && state.spentMana < minMana)
    {
        minMana = state.spentMana;
        Console.WriteLine($"I won, and spent {state.spentMana}");
        continue;
    }

    if (state.myTurn)
    {
        // me turn
        var availableSpells = spells.Where(s => s.manaCost <= me.mana && !effects.Contains(s.effect)).ToList();

        if (!availableSpells.Any())
        {
            continue;
        }

        foreach(var availableSpell in availableSpells)
        {
            var effectsCopy = effects.ToList();
            if (availableSpell.effect != null)
            {
                var effect = new Effect
                { 
                    id = availableSpell.effect.id, 
                    armour = availableSpell.effect.armour,
                    damage = availableSpell.effect.damage,
                    manaRegen = availableSpell.effect.manaRegen,
                    remainingTime = availableSpell.effect.initialDuration
                };

                effectsCopy.Add(effect);
            }

            var spellsCast = state.SpellsCast.ToList();
            spellsCast.Add(availableSpell.name);

            states.Enqueue(new GameState
            {
                me = me with { hp = me.hp + availableSpell.heal, mana = me.mana - availableSpell.manaCost, armour = 0 }, 
                boss = boss with { hp = boss.hp - availableSpell.damage }, 
                activeEffects = effectsCopy, 
                spentMana = state.spentMana + availableSpell.manaCost, 
                myTurn = false,
                SpellsCast = spellsCast
            });
        }
    }
    else
    {
        // boss turn
        states.Enqueue(new GameState
        {
            me = me with {hp = me.hp - Math.Max(1, boss.damage - me.armour), armour = 0},
            boss = boss,
            activeEffects = effects.ToList(),
            spentMana = state.spentMana,
            myTurn = true,
            SpellsCast = state.SpellsCast
        });
    }
}

Console.WriteLine(minMana);

(Me me, Boss boss, ICollection<Effect> activeEffects) ApplyEffects(Me me, Boss boss, ICollection<Effect> activeEffects)
{
    var newEffects = new List<Effect>();

    foreach(var effect in activeEffects)
    {
        (me, boss, var newEffect) = effect.Apply(me, boss);
        newEffects.Add(newEffect);
    }
    newEffects = newEffects.Where(a => a.remainingTime > 0).ToList();

    return (me, boss, newEffects);
}




record GameState
{
    public Me me {get;init;}

    public Boss boss { get; init; }

    public ICollection<Effect> activeEffects { get; init; }

    public int spentMana {get;init;}

    public bool myTurn {get;init;}

    public ICollection<string> SpellsCast {get; init;} = new List<string>();

    public override string ToString()
    {
        return string.Join(", ", SpellsCast);
    }
}


class Spell {
    public string name {get;set;}
    public int manaCost { get; set; }
    public int damage { get; set; }
    public int heal { get; set; }
    public Effect effect { get; set; }
}

class Effect {
    public string id { get; set; }
    public int initialDuration{ get; set; }
    public int remainingTime { get; set; }
    public int damage { get; set; }
    public int manaRegen { get; set; }
    public int armour { get; set; }
    public override bool Equals(object? obj)
    {
        // NRE lol
        if ((obj as Effect).id.Equals(this.id) )
        {
            return true;
        }
        return false;
    }

    public (Me me, Boss boss, Effect effect) Apply(Me me, Boss boss)
    {
        var effect = new Effect
        {
            id = this.id,
            armour = this.armour,
            remainingTime = this.remainingTime - 1,
            damage = this.damage,
            manaRegen = this.manaRegen
        };

        return (me with {armour = me.armour + this.armour, mana = me.mana + this.manaRegen}, boss with {hp = boss.hp - this.damage}, effect);
    }
}

record Me 
{
    public int hp {get; init;}
    public int mana{get;init;}
    public int armour {get; init;}
}

record Boss
{
    public int hp {get;init;}
    public int damage {get;init;}
}