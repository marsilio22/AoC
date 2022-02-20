var lines = File.ReadAllLines("./input.txt");

IDictionary<int, Bot> bots = new Dictionary<int, Bot>();

IDictionary<int, int> outputs = new Dictionary<int, int>();

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
        var lowTo = int.Parse(split[6]);
        var highTo = int.Parse(split[11]);

        var lowToIsBot = split[5].Equals("bot", StringComparison.OrdinalIgnoreCase);
        var highToIsBot = split[10].Equals("bot", StringComparison.OrdinalIgnoreCase);

        foreach (var num in new []{fromBot, lowTo, highTo})
        {
            if (!bots.ContainsKey(num))
            {
                bots[num] = new Bot();
            }

            if (!outputs.ContainsKey(num))
            {
                outputs[num] = -1;
            }
        }

        if (highToIsBot)
        {
            bots[fromBot].highBotTarget = highTo;
        }
        else
        {
            bots[fromBot].highOutputTarget = highTo;
        }

        if (lowToIsBot)
        {
            bots[fromBot].lowBotTarget = lowTo;
        }
        else
        {
            bots[fromBot].lowOutputTarget = lowTo;
        }
    }
}

var count = -1;

while (outputs[0] == -1 || outputs[1] == -1 || outputs[2] == -1)
{
    count = bots.Count(b => b.Value.values.Count() != 2);
    foreach(var bot in bots)
    {
        if (bot.Value.values.Count() == 2)
        {
            var highBot = bot.Value.highBotTarget.HasValue ? bots[bot.Value.highBotTarget.Value] : null;
            var lowBot = bot.Value.lowBotTarget.HasValue ? bots[bot.Value.lowBotTarget.Value] : null;
            var highOutput = bot.Value.highOutputTarget.HasValue ? bot.Value.highOutputTarget.Value : -2;
            var lowOutput = bot.Value.lowOutputTarget.HasValue ? bot.Value.lowOutputTarget.Value : -2;

            if (highBot != null && !highBot.values.Contains(bot.Value.values.Max()))
            {
                highBot.values.Add(bot.Value.values.Max());
            }
            else if (highOutput != -2)
            {
                outputs[highOutput] = bot.Value.values.Max();
            }

            if (lowBot != null && !lowBot.values.Contains(bot.Value.values.Min()))
            {
                lowBot.values.Add(bot.Value.values.Min());
            }
            else if (lowOutput != -2)
            {
                outputs[lowOutput] = bot.Value.values.Min();
            }
        }
    }
}

Console.WriteLine(bots.Single(b => b.Value.values.Intersect(new [] {61, 17}).Count() == 2).Key);

Console.WriteLine(outputs[0] * outputs[1] * outputs[2]);

class Bot
{
    public List<int> values {get; set;} = new List<int>();


    public int? highBotTarget {get; set;}
    public int? lowBotTarget {get; set;}
    public int? highOutputTarget {get; set;}
    public int? lowOutputTarget {get; set;}

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