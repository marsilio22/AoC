/** 
    Sprinkles: capacity 5, durability -1, flavor 0, texture 0, calories 5
    PeanutButter: capacity -1, durability 3, flavor 0, texture 0, calories 1
    Frosting: capacity 0, durability -1, flavor 4, texture 0, calories 6
    Sugar: capacity -1, durability 0, flavor 0, texture 2, calories 8
*/

IDictionary<string ,(int capacity, int durability, int flavour, int texture, int calories)> ingredients = new Dictionary<string, (int, int, int, int, int)>{
    ["Sprinkles"]    = ( 5, -1, 0, 0, 5),
    ["PeanutButter"] = (-1,  3, 0, 0, 1),
    ["Frosting"]     = ( 0, -1, 4, 0, 6),
    ["Sugar"]        = (-1,  0, 0, 2, 8)
};


// 5 Sprinkles > PeanutButter + Sugar 
// 3 PeanutButter > Sprinkles + Frosting 
// Frosting > 0
// Sugar > 0

long max = 0;

for (int sug = 1; sug <= 100; sug++)
{
    for (int fro = 1; fro <= 100 - sug; fro++)
    {
        for (int pea = 1; pea <= 100 - sug - fro; pea++)
        {
            for (int spr = 1; spr <= 100 - sug - fro - pea; spr++)
            {
                if (5* spr > pea + sug && 3*pea > spr + fro)
                {
                    var cal = ingredients["Sprinkles"].calories * spr + ingredients["Frosting"].calories * fro + ingredients["PeanutButter"].calories *pea + ingredients["Sugar"].calories * sug;

                    if (cal != 500)
                    {
                        continue;
                    }

                    var cap = ingredients["Sprinkles"].capacity * spr + ingredients["Frosting"].capacity * fro + ingredients["PeanutButter"].capacity * pea + ingredients["Sugar"].capacity * sug;
                    var dur= ingredients["Sprinkles"].durability * spr + ingredients["Frosting"].durability * fro + ingredients["PeanutButter"].durability * pea + ingredients["Sugar"].durability * sug;
                    var fla= ingredients["Sprinkles"].flavour * spr + ingredients["Frosting"].flavour * fro + ingredients["PeanutButter"].flavour * pea + ingredients["Sugar"].flavour * sug;
                    var tex = ingredients["Sprinkles"].texture * spr + ingredients["Frosting"].texture * fro + ingredients["PeanutButter"].texture * pea + ingredients["Sugar"].texture * sug;

                    long value = (long) cap * dur * fla * tex;

                    if (value > max)
                    {
                        max = value;
                        Console.WriteLine($"spr{spr} fro{fro} pea{pea} sug{sug}, val{value}");
                    }
                }
            }
        }
    }
}

// 13333440