var sizes = File.ReadAllLines("./input.txt").Select(l => int.Parse(l)).ToList();

// recursion was fun here lol don't at me
Console.WriteLine(Recurse(sizes, 150)); 

int Recurse(List<int> sizes, int total)
{
    var numberOfWays = 0;

    if ( total < 0 || !sizes.Any())
    {
        return 0;
    }
    else if (sizes.Sum() == total)
    {
        return 1;
    }
    else if (sizes.Any(s => s == total))
    {
        var count = sizes.Count(s => s == total);
        var newSizes = sizes.Where(s => s < total).ToList();
        return count + Recurse(newSizes,total);
    }

    for (int i = 0; i < sizes.Count(); i++)
    {
        var newSizes = new List<int>();

        // only need to consider the things in the array *after* sizes[i]
        // otherwise we end up double (or more) counting answers
        for (int j = i+1; j < sizes.Count(); j++)
        {
            newSizes.Add(sizes[j]);
        }

        numberOfWays += Recurse(newSizes, total - sizes[i]);
    }

    return numberOfWays;
}

// part 2, recursion is entirely inappropriate now I think...

var ordered = sizes.OrderByDescending(d=>d).ToList();

// by observation, need at least 4 containers to make 150

var waysWith4 = 0;

for (int first = 0; first < ordered.Count; first++)
{
    for (int second = first + 1; second < ordered.Count; second++)
    {
        for (int third = second + 1; third < ordered.Count; third++)
        {
            for (int fourth = third + 1; fourth < ordered.Count; fourth++)
            {
                if (sizes[first] + sizes[second] + sizes[third] + sizes[fourth] == 150)
                {
                    waysWith4++;
                }
            }
        }
    }
}

Console.WriteLine(waysWith4);
