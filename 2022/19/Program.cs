using System.Collections.ObjectModel;

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

    var states = new Queue<State>();
    states.Enqueue(new State(1, 0, 0, 0, 0, 0, 0, 0, 24));

    while (states.TryDequeue(out var state))
    {
        if (state.time == 0)
        {
            bestQuality = Math.Max(state.geodes, bestQuality);
        }
        else if (state.time + state.obsidian < blueprint.geodebotObsidianCost)
        {
            // would never make any geodes so don't bother trying
            continue;
        }
        else
        {
            // var doNothingState = state with {
            //     time = state.time - 1,
            //     ore = state.ore + state.orebots,
            //     clay = state.clay + state.claybots,
            //     obsidian = state.obsidian + state.obsidianbots,
            //     geodes = state.geodes + state.geodebots
            // };
            
            // states.Enqueue(doNothingState);

            if (state.geodebots > 0)
            {
                var waitForGeodesState = state with {
                    time = 0,
                    geodes = state.time * state.geodebots
                };

                states.Enqueue(state);
            }

            // do nothing
            // if (state.ore < blueprint.orebotCost &&
            //     state.ore < blueprint.claybotCost &&
            //     (state.ore < blueprint.obsidianbotOreCost || state.clay < blueprint.obsidianbotClayCost) &&
            //     (state.ore < blueprint.geodebotOreCost || state.obsidian < blueprint.geodebotObsidianCost))
            // {
                // only do nothing if we can't do anything else
            // }

            // // tryBuild ore robot
            // if (state.ore >= blueprint.orebotCost)
            // {
            //     states.Enqueue(doNothingState with {
            //         ore = doNothingState.ore - blueprint.orebotCost, 
            //         orebots = doNothingState.orebots + 1
            //     });
            // }

            // // tryBuild clay robot
            // if (state.ore >= blueprint.claybotCost)
            // {
            //     states.Enqueue(doNothingState with {
            //         ore = doNothingState.ore - blueprint.claybotCost, 
            //         claybots= doNothingState.claybots + 1
            //     });
            // }
            
            // // tryBuild obs robot
            // if (state.ore >= blueprint.obsidianbotOreCost && 
            //     state.clay >= blueprint.obsidianbotClayCost)
            // {
            //     states.Enqueue(doNothingState with {
            //         ore = doNothingState.ore - blueprint.obsidianbotOreCost, 
            //         clay = doNothingState.clay - blueprint.obsidianbotClayCost,
            //         obsidianbots = doNothingState.obsidianbots + 1
            //     });
            // }
            
            // // tryBuild geode robot
            // if (state.ore >= blueprint.geodebotOreCost && 
            //     state.obsidian >= blueprint.geodebotObsidianCost)
            // {
            //     states.Enqueue(doNothingState with {
            //         ore = doNothingState.ore - blueprint.geodebotOreCost, 
            //         obsidian = doNothingState.obsidian - blueprint.geodebotObsidianCost, 
            //         geodebots = doNothingState.geodebots + 1
            //     });
            // }


            // wait until I have enough ore for an orebot and build one
            var turnsToWait = (int)Math.Ceiling((double)(blueprint.orebotCost - state.ore) / state.orebots);

            if (state.time > turnsToWait && turnsToWait > 0)
            {
                var newOrebotState = state with {
                    time = state.time - turnsToWait,
                    ore = state.ore + state.orebots * turnsToWait - blueprint.orebotCost,
                    clay = state.clay + state.claybots * turnsToWait,
                    obsidian = state.obsidian + state.obsidianbots * turnsToWait,
                    geodes = state.geodes + state.geodebots * turnsToWait,
                    orebots = state.orebots + 1
                };

                states.Enqueue(newOrebotState);
            }
            else if (turnsToWait < 0)
            {
                var newOrebotState = state with {
                    time = state.time - 1,
                    ore = state.ore + state.orebots - blueprint.orebotCost,
                    clay = state.clay + state.claybots,
                    obsidian = state.obsidian + state.obsidianbots,
                    geodes = state.geodes + state.geodebots,
                    orebots = state.orebots + 1
                };
                
                states.Enqueue(newOrebotState);
            }

            // wait until I have enough ore for a claybod and build one
            turnsToWait = (int)Math.Ceiling((double)(blueprint.claybotCost - state.ore) / state.orebots);

            if (state.time > turnsToWait && turnsToWait > 0)
            {
                var newClaybotState = state with {
                    time = state.time - turnsToWait,
                    ore = state.ore + state.orebots * turnsToWait - blueprint.claybotCost,
                    clay = state.clay + state.claybots * turnsToWait,
                    obsidian = state.obsidian + state.obsidianbots * turnsToWait,
                    geodes = state.geodes + state.geodebots * turnsToWait,
                    claybots = state.claybots + 1
                };

                states.Enqueue(newClaybotState);
            }
            else if (turnsToWait < 0)
            {
                var newClaybotState = state with {
                    time = state.time - 1,
                    ore = state.ore + state.orebots - blueprint.claybotCost,
                    clay = state.clay + state.claybots,
                    obsidian = state.obsidian + state.obsidianbots,
                    geodes = state.geodes + state.geodebots,
                    claybots = state.claybots + 1
                };
                
                states.Enqueue(newClaybotState);
            }

            // wait until I have enough ore and clayfor an obs bot...
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
                        clay = state.clay + state.claybots * turnsToWait - blueprint.obsidianbotClayCost,
                        obsidian = state.obsidian + state.obsidianbots * turnsToWait,
                        geodes = state.geodes + state.geodebots * turnsToWait,
                        obsidianbots = state.obsidianbots+ 1
                    };

                    states.Enqueue(newObsbotState);
                }
                else if (turnsToWait < 0)
                {
                    var newObsbotState = state with {
                        time = state.time - 1,
                        ore = state.ore + state.orebots  - blueprint.obsidianbotOreCost,
                        clay = state.clay + state.claybots  - blueprint.obsidianbotClayCost,
                        obsidian = state.obsidian + state.obsidianbots,
                        geodes = state.geodes + state.geodebots,
                        obsidianbots = state.obsidianbots + 1
                    };
                    
                    states.Enqueue(newObsbotState);
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
                        clay = state.clay + state.claybots * turnsToWait,
                        obsidian = state.obsidian + state.obsidianbots * turnsToWait - blueprint.geodebotObsidianCost,
                        geodes = state.geodes + state.geodebots * turnsToWait,
                        geodebots= state.geodebots + 1
                    };

                    states.Enqueue(newGeoBotState);
                }
                else if (turnsToWait < 0)
                {
                    var newGeoBotState = state with {
                        time = state.time - 1,
                        ore = state.ore + state.orebots - blueprint.geodebotOreCost,
                        clay = state.clay + state.claybots,
                        obsidian = state.obsidian + state.obsidianbots  - blueprint.geodebotObsidianCost,
                        geodes = state.geodes + state.geodebots,
                        geodebots= state.geodebots + 1
                    };
                    
                    states.Enqueue(newGeoBotState);
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