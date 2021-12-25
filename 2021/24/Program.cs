using System.Text;

var lines = File.ReadAllLines("./input.txt");

var inputs = new HashSet<string>();
var r = new Random();

// start with 10000 random inputs
for (int i = 0; i < 10000; i++)
{
    var s = string.Empty;
    for (int j = 0; j < 14; j++)
    {
        s += r.Next(1, 10);
    }

    inputs.Add(s);
}

// this takes a while, but does work. Though **maybe** it only works sometimes if you're particularly lucky on the above random bit.
// TODO wonder if there is a sensible input set that will get you to the right answer always, but not just from knowing what that is to start with...
inputs = RunPart(1, lines, inputs).Keys.ToHashSet();

RunPart(2, lines, inputs);

/// <summary>
/// Runs the machine with a starting set of inputs
/// </summary>
/// <param name="part">part 1 or part 2?</param>
/// <param name="lines">The "assembly" program</param>
/// <remarks>
/// I thought of this as a game of Mastermind, where the output score is an indication of how close you are.
/// it kept getting stuck on "7", or sometimes it would get to "0" but the answer was wrong
/// I figured if you viewed it as a 14 dimensional problem, then maybe changing one dimension at once wasn't enough
/// so I tried changing 2 and it suddenly worked.
/// </remarks>
IDictionary<string, long> RunPart(int part, ICollection<string> lines, ICollection<string> inputs)
{
    var output = RunProgram(lines, inputs, part);
    var currentBest = output.Keys.First();

    while (true)
    {
        inputs = new HashSet<string>();    
        
        // take the best 100 outputs, and change one number in each
        foreach (var outs in output.Take(100))
        {
            for (char i = '1'; i <= '9'; i++)
            {
                for (int j = 0; j < 14; j++)
                {
                    var arr = outs.Key.ToCharArray();
                    if (arr[j] != i)
                    {
                        arr[j] = i;
                        inputs.Add(string.Join("",arr));
                    }
                }
            }
        }

        // copy those new inputs (to avoid iteration errors) and then change another number in each
        var copy = inputs.ToList();

        foreach(var inp in copy){
            
            for(char i = '1'; i <= '9'; i++)
            {
                for (int j = 0; j < 14; j++)
                {
                    var arr = inp.ToCharArray();
                    if (arr[j] != i)
                    {
                        arr[j] = i;
                        inputs.Add(string.Join("",arr));
                    }
                }
            }
        }

        var currentBestLong = long.Parse(currentBest);

        // if (part == 1)
        //     inputs = inputs.Where(i => long.Parse(i) >= currentBestLong).ToList();
        // else
        //     inputs = inputs.Where(i => long.Parse(i) <= currentBestLong).ToList();

        // run the program against the new input, and record the new best answer
        output = RunProgram(lines, inputs, part);

        if (currentBest == output.Keys.First() && output.Values.First() == 0)
        {        
            // if we get the same best answer twice in a row, we're probably done
            break;
        }
        else if (currentBest == output.Keys.First() && output.Values.First() == 7) // we're stuck!!
        {
            // restart because it's gone wrong >:(
            inputs = new HashSet<string>();

            for (int i = 0; i < 10000; i++)
            {
                var s = string.Empty;
                for (int j = 0; j < 14; j++)
                {
                    s += r.Next(1, 10);
                }

                inputs.Add(s);
            }
        }

        currentBest = output.Keys.First();
    }

    Console.WriteLine($"Best answer: {currentBest}");
    return output;
}


IDictionary<string, long> RunProgram(ICollection<string> lines, ICollection<string> inputs, int part)
{
    var results = new Dictionary<string, long>();

    foreach(var input in inputs)
    {
        var index = 0;
        long w = 0;
        long x = 0;
        long y = 0;
        long z = 0;

        var count = 0;
        foreach(var line in lines)
        {
            count++;

            var split = line.Split(" ");
            long second = long.MinValue;

            if (split.Count() == 3)
            {
                if (!long.TryParse(split[2], out second))
                {
                    switch(split[2])
                    {
                        case "w":
                            second = w;
                            break;
                        case "x":
                            second = x;
                            break;
                        case "y":
                            second = y;
                            break;
                        case "z":
                            second = z;
                            break;
                        default: throw new Exception("oh noes");
                    }
                }
            }

            switch(split[0])
            {
                case "inp": // they're always "w"
                    w = int.Parse(input[index].ToString());
                    index++;
                    break;
                case "add":
                    switch(split[1])
                    {
                        case "w":
                            w += second;
                            break;
                        case "x":
                            x += second;
                            break;
                        case "y":
                            y += second;
                            break;
                        case "z":
                            z += second;
                            break;
                    }
                    break;
                case "mul":            
                    switch(split[1])
                    {
                        case "w":
                            w *= second;
                            break;
                        case "x":
                            x *= second;
                            break;
                        case "y":
                            y *= second;
                            break;
                        case "z":
                            z *= second;
                            break;
                    }
                    break;
                case "div":
                    switch(split[1])
                    {
                        case "w":
                            w /= second;
                            break;
                        case "x":
                            x /= second;
                            break;
                        case "y":
                            y /= second;
                            break;
                        case "z":
                            z /= second;
                            break;
                    }
                    break;
                case "mod":
                    switch(split[1])
                    {
                        case "w":
                            w %= second;
                            break;
                        case "x":
                            x %= second;
                            break;
                        case "y":
                            y %= second;
                            break;
                        case "z":
                            z %= second;
                            break;
                    }
                    break;
                case "eql":
                    switch(split[1])
                    {
                        case "w":
                            w = w == second ? 1 : 0;
                            break;
                        case "x":
                            x = x == second ? 1 : 0;
                            break;
                        case "y":
                            y = y == second ? 1 : 0; 
                            break;
                        case "z":
                            z = z == second ? 1 : 0;
                            break;
                    }
                    break;
            }

            // Console.WriteLine("| " + count.ToString().PadLeft(5) + " | " + line.PadLeft(10) +
            //                 " | " + w.ToString().PadLeft(30) + 
            //                 " | " + x.ToString().PadLeft(30) +
            //                 " | " + y.ToString().PadLeft(30) +
            //                 " | " + z.ToString().PadLeft(30) + " |");
        }

        results.Add(input, z);
    }

    // take the best results
    if (part == 1)
    {
        results = results.OrderBy(d => d.Value).ThenByDescending(d => d.Key).ToDictionary(k => k.Key, v => v.Value);
    }
    else
    {
        results = results.OrderBy(d => d.Value).ThenBy(d => d.Key).ToDictionary(k => k.Key, v => v.Value);
    }

    foreach(var res in results.Take(10))
    {
        Console.WriteLine($"{res.Key}, {res.Value.ToString().PadLeft(30)}");
    }
    Console.WriteLine("------");

    return results;
}
