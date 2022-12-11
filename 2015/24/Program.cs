var packages = File.ReadAllLines("./input").Select(p => long.Parse(p)).ToList();

int target = (int)packages.Sum() / 3;
int target2 = (int)packages.Sum() / 4;

Part(target, packages);

Part(target2, packages);

void Part(int target, List<long> packages)
{
    int shortest = int.MaxValue, longest = 0;

    var groups = Recurse(packages, target, ref shortest, ref longest);
    var smallestGroupCount = groups.Min(s => s.Count());
    var shortestCollections = groups.Where(s => s.Count() == smallestGroupCount);

    long qe = long.MaxValue;

    foreach(var thing in shortestCollections)
    {
        var thingQE = thing.Aggregate((a, b) => a*b);

        if (thingQE < qe)
        {
            qe = thingQE;
        }
    }

    Console.WriteLine(qe);

}

HashSet<HashSet<long>> Recurse(List<long> packages, int target, ref int shortestSolutionFoundSoFar, ref int thisSolutionCount)
{
    var res = new HashSet<HashSet<long>>();

    if (thisSolutionCount > shortestSolutionFoundSoFar)
    {
        return res;
    }

    foreach (var package in packages)
    {
        if (package == target)
        {
            res.Add(new HashSet<long>{package});
            if (thisSolutionCount + 1 < shortestSolutionFoundSoFar)
            {
                shortestSolutionFoundSoFar = thisSolutionCount + 1;
            }
        }
        else 
        {
            var newPackages = packages.Where(p => p < package).ToList();
            int newTarget = target - (int)package;

            var newSolCount = thisSolutionCount + 1;

            var answers = Recurse(newPackages, newTarget, ref shortestSolutionFoundSoFar, ref newSolCount).Select(d => d.Append(package).ToHashSet());

            foreach(var thing in answers)
            {
                res.Add(thing);
            }
        }
    }

    return res;
}