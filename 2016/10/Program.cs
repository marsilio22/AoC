var lines = File.ReadAllLines("./input.txt");

IDictionary<int, Bot> bots = new Dictionary<int, Bot>();

foreach (var line in lines)
{
    var split = line.Split(' ');

    int bot;
    int value;

    if (split[0].Equals("value"))
    {
        bot = int.Parse(split[5]);
        value = int.Parse(split[1]);

        if (!bots.ContainsKey(bot))
        {
            bots[bot] = new Bot();
        }
        
        bots[bot].values.Add(value);
    }
    else
    {
        var fromBot = int.Parse(split[1]);
        var lowToBot = int.Parse(split[6]);
        var highToBot = int.Parse(split[11]);

        foreach (var num in new []{fromBot, lowToBot, highToBot})
        {
            if (!bots.ContainsKey(num))
            {
                bots[num] = new Bot();
            }
        }

        bots[fromBot].highTarget = highToBot;
        bots[fromBot].lowTarget = lowToBot;
    }
}

var count = -1;

while (bots.Count(b => b.Value.values.Count() != 2) != count)
{
    count = bots.Count(b => b.Value.values.Count() != 2);
    foreach(var bot in bots)
    {
        if (bot.Value.values.Count() == 2)
        {
var highBot = bots[bot.Value.highTarget];
var lowBot = bots[bot.Value.lowTarget];


            if (!highBot.values.Contains(bot.Value.values.Max()) && highBot.values.Count() != 2)
            {
                highBot.values.Add(bot.Value.values.Max());
            }

            if (!lowBot.values.Contains(bot.Value.values.Min()) && lowBot.values.Count() != 2)
            {
                lowBot.values.Add(bot.Value.values.Min());
            }
        }
    }
}

Console.WriteLine(bots.Single(b => b.Value.values.Intersect(new [] {61, 17}).Count() == 2).Key);

class Bot
{
    public List<int> values {get; set;} = new List<int>();

    public int highTarget {get;set;}
    public int lowTarget {get;set;}

    public override string ToString()
    {
        if (values.Count == 2)
        {
            return "{" + values.Min() + ", " + values.Max() + "}";
        }
        else 
        {
            return "{" + values.FirstOrDefault() + "}";
        }
    }
}