using System.Collections.ObjectModel;
using System.Globalization;

var lines = File.ReadAllLines("./input2");

var blueprints = new List<Blueprint>();

foreach (var line in lines)
{
    var split = line.Split(" ");

    blueprints.Add(new Blueprint(
        int.Parse(split[1].Split(":")[0]),
        int.Parse(split[6]),
        int.Parse(split[12]),
        int.Parse(split[18]),
        int.Parse(split[21]),
        int.Parse(split[27]),
        int.Parse(split[30])
    ));
}

var qualities = new List<int>();

foreach(var blueprint in blueprints)
{
    var bestQuality = 0;

    var states = new HashSet<State>();
    states.Add(new State(1, 0, 0, 0, 0, 0, 0, 0, 24));

    var maxOreBots = new [] { blueprint.orebotCost, blueprint.claybotCost, blueprint.geodebotOreCost, blueprint.obsidianbotOreCost }.Max();

    while (states.Any())
    {
        var state = states.Last();
        states.Remove(state);

        if (state.time == 0 && state.geodes > 0)
        {
            if (state.geodes == 17)
            {
                Console.WriteLine();
            }
            bestQuality = Math.Max(state.geodes, bestQuality);
        }
        else if (state.time + state.obsidian < blueprint.geodebotObsidianCost)
        {
            // would never make any geodes so don't bother trying
            continue;
        }
        else 
        {
            if (state.geodebots > 0 && bestQuality < state.time * state.geodebots)
            {
                var waitForGeodesState = state with {
                    time = 0,
                    geodes = state.time * state.geodebots
                };

                states.Add(state);
            }

            int turnsToWait;

            // wait until I have enough ore for an orebot and build one
            if (state.orebots < maxOreBots)
            {
                turnsToWait = (int)Math.Ceiling((double)(blueprint.orebotCost - state.ore) / state.orebots);

                if (state.time > turnsToWait && turnsToWait > 0)
                {
                    var newOrebotState = state with {
                        time = state.time - turnsToWait,
                        ore = state.ore + state.orebots * turnsToWait - blueprint.orebotCost,
                        clay = (state.clay + state.claybots * turnsToWait),
                        obsidian = state.obsidian + state.obsidianbots * turnsToWait,
                        geodes = state.geodes + state.geodebots * turnsToWait,
                        orebots = state.orebots + 1
                    };

                    states.Add(newOrebotState);
                }
                else if (turnsToWait < 0)
                {
                    var newOrebotState = state with {
                        time = state.time - 1,
                        ore = state.ore + state.orebots - blueprint.orebotCost,
                        clay = (state.clay + state.claybots),
                        obsidian = state.obsidian + state.obsidianbots,
                        geodes = state.geodes + state.geodebots,
                        orebots = state.orebots + 1
                    };
                    
                    states.Add(newOrebotState);
                }
            }

            if (state.claybots < blueprint.obsidianbotClayCost)
            {
                // wait until I have enough ore for a claybot and build one
                turnsToWait = (int)Math.Ceiling((double)(blueprint.claybotCost - state.ore) / state.orebots);

                if (state.time > turnsToWait && turnsToWait > 0)
                {
                    var newClaybotState = state with {
                        time = state.time - turnsToWait,
                        ore = state.ore + state.orebots * turnsToWait - blueprint.claybotCost,
                        clay = (state.clay + state.claybots * turnsToWait),
                        obsidian = state.obsidian + state.obsidianbots * turnsToWait,
                        geodes = state.geodes + state.geodebots * turnsToWait,
                        claybots = state.claybots + 1
                    };

                    states.Add(newClaybotState);
                }
                else if (turnsToWait < 0)
                {
                    var newClaybotState = state with {
                        time = state.time - 1,
                        ore = state.ore + state.orebots - blueprint.claybotCost,
                        clay = (state.clay + state.claybots),
                        obsidian = state.obsidian + state.obsidianbots,
                        geodes = state.geodes + state.geodebots,
                        claybots = state.claybots + 1
                    };
                    
                    states.Add(newClaybotState);
                }
            }

            // wait until I have enough ore and clay for an obs bot...
            if (state.claybots > 0)
            {
                turnsToWait = Math.Max(
                    (int)Math.Ceiling((double)(blueprint.obsidianbotOreCost- state.ore) / state.orebots),
                    (int)Math.Ceiling((double)(blueprint.obsidianbotClayCost- state.clay) / state.claybots)
                );

                if (state.time > turnsToWait && turnsToWait > 0)
                {
                    var newObsbotState = state with {
                        time = state.time - turnsToWait,
                        ore = state.ore + state.orebots * turnsToWait - blueprint.obsidianbotOreCost,
                        clay = (state.clay + state.claybots * turnsToWait - blueprint.obsidianbotClayCost),
                        obsidian = state.obsidian + state.obsidianbots * turnsToWait,
                        geodes = state.geodes + state.geodebots * turnsToWait,
                        obsidianbots = state.obsidianbots+ 1
                    };

                    states.Add(newObsbotState);
                }
                else if (turnsToWait < 0)
                {
                    var newObsbotState = state with {
                        time = state.time - 1,
                        ore = state.ore + state.orebots  - blueprint.obsidianbotOreCost,
                        clay = (state.clay + state.claybots  - blueprint.obsidianbotClayCost),
                        obsidian = state.obsidian + state.obsidianbots,
                        geodes = state.geodes + state.geodebots,
                        obsidianbots = state.obsidianbots + 1
                    };
                    
                    states.Add(newObsbotState);
                }
            }
            
            // wait until I have enough ore and obs an geo bot...
            if (state.obsidianbots > 0)
            {
                turnsToWait = Math.Max(
                    (int)Math.Ceiling((double)(blueprint.geodebotOreCost- state.ore) / state.orebots),
                    (int)Math.Ceiling((double)(blueprint.geodebotObsidianCost - state.obsidian) / state.obsidianbots)
                );

                if (state.time > turnsToWait && turnsToWait > 0)
                {
                    var newGeoBotState = state with {
                        time = state.time - turnsToWait,
                        ore = state.ore + state.orebots * turnsToWait - blueprint.geodebotOreCost,
                        clay = (state.clay + state.claybots * turnsToWait),
                        obsidian = state.obsidian + state.obsidianbots * turnsToWait - blueprint.geodebotObsidianCost,
                        geodes = state.geodes + state.geodebots * turnsToWait,
                        geodebots= state.geodebots + 1
                    };

                    states.Add(newGeoBotState);
                }
                else if (turnsToWait < 0)
                {
                    var newGeoBotState = state with {
                        time = state.time - 1,
                        ore = state.ore + state.orebots - blueprint.geodebotOreCost,
                        clay = (state.clay + state.claybots),
                        obsidian = state.obsidian + state.obsidianbots  - blueprint.geodebotObsidianCost,
                        geodes = state.geodes + state.geodebots,
                        geodebots= state.geodebots + 1
                    };
                    
                    states.Add(newGeoBotState);
                }
            }
        }
    }

    qualities.Add(bestQuality * blueprint.id);
}

Console.WriteLine(qualities.Sum());

record State(
    int orebots,
    int claybots,
    int obsidianbots,
    int geodebots,
    int ore,
    int clay,
    int obsidian,
    int geodes,
    int time
);

record Blueprint(
    int id,
    int orebotCost, 
    int claybotCost, 
    int obsidianbotOreCost, 
    int obsidianbotClayCost, 
    int geodebotOreCost,
    int geodebotObsidianCost);