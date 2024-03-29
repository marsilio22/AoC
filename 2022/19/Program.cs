﻿var lines = File.ReadAllLines("./input");

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

var bestGeodes = Part(blueprints, 24).ToList();

for(int i = 0; i < blueprints.Count; i++)
{
    bestGeodes[i] = bestGeodes[i] * blueprints[i].id;
}

Console.WriteLine(bestGeodes.Sum());

bestGeodes = Part(blueprints.Take(3).ToList(), 32).ToList();

Console.WriteLine(bestGeodes.Aggregate((a, b) => a*b));

ICollection<int> Part(ICollection<Blueprint> blueprints, int time)
{
    var result = new List<int>();

    foreach(var blueprint in blueprints)
    {
        var bestQuality = 0;

        var states = new Queue<State>();
        states.Enqueue(new State(1, 0, 0, 0, 0, 0, 0, 0, time));

        var maxOre = new [] { blueprint.orebotCost, blueprint.claybotCost, blueprint.obsidianbotOreCost, blueprint.geodebotOreCost}.Max();

        while (states.TryDequeue(out var state))
        {
            if (state.time == 1)
            {
                bestQuality = Math.Max(state.geodes + state.geodebots, bestQuality);
            }
            else
            {
                var doNothingState = state with {
                    time = state.time - 1,
                    ore = state.ore + state.orebots,
                    clay = state.clay + state.claybots,
                    obsidian = state.obsidian + state.obsidianbots,
                    geodes = state.geodes + state.geodebots
                };

                // tryBuild geode robot
                if (state.ore >= blueprint.geodebotOreCost && 
                    state.obsidian >= blueprint.geodebotObsidianCost) // suspect this is now giving me the wrong answer for part 1
                {
                    states.Enqueue(doNothingState with {
                        ore = doNothingState.ore - blueprint.geodebotOreCost, 
                        obsidian = doNothingState.obsidian - blueprint.geodebotObsidianCost, 
                        geodebots = doNothingState.geodebots + 1
                    });
                }
                else 
                {
                    if (state.ore < maxOre)
                    {
                        states.Enqueue(doNothingState);
                    }

                    // tryBuild ore robot
                    if (state.ore >= blueprint.orebotCost && state.orebots < maxOre)
                    {
                        states.Enqueue(doNothingState with {
                            ore = doNothingState.ore - blueprint.orebotCost, 
                            orebots = doNothingState.orebots + 1
                        });
                    }

                    // tryBuild clay robot
                    if (state.ore >= blueprint.claybotCost && state.claybots < blueprint.obsidianbotClayCost)
                    {
                        states.Enqueue(doNothingState with {
                            ore = doNothingState.ore - blueprint.claybotCost, 
                            claybots= doNothingState.claybots + 1
                        });
                    }
                    
                    // tryBuild obs robot
                    if (state.ore >= blueprint.obsidianbotOreCost && 
                        state.clay >= blueprint.obsidianbotClayCost && 
                        state.obsidianbots < blueprint.geodebotObsidianCost)
                    {
                        states.Enqueue(doNothingState with {
                            ore = doNothingState.ore - blueprint.obsidianbotOreCost, 
                            clay = doNothingState.clay - blueprint.obsidianbotClayCost,
                            obsidianbots = doNothingState.obsidianbots + 1
                        });
                    }
                }
            }
        }

        result.Add(bestQuality);
    }

    return result;
}

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