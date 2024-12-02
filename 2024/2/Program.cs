var lines = File.ReadAllLines("./input.txt");

var tot = 0;
var tot2 = 0;

foreach (var line in lines)
{
    var parsed = line.Split(" ").Select(int.Parse).ToArray();

    if (IsSafe(parsed))
    {
        tot++;
        tot2++;
        continue;
    }
    
    for (int j = 0; j < parsed.Length; j++)
    {
        var reducedParsed = parsed.ToList();
        reducedParsed.RemoveAt(j);

        if (IsSafe(reducedParsed.ToArray()))
        {
            tot2++;
            break;
        }
    }
}

Console.WriteLine(tot);
Console.WriteLine(tot2);

bool IsSafe(int[] report)
{
    var asc = report.First() < report.Last();
    var safe = true;

    for (int i = 0; i < report.Length - 1; i++)
    {
        var diff = Math.Abs(report[i] - report[i + 1]);

        // it's still safe if:
        safe = safe && // it's safe thus far AND
            ((asc && report[i] < report[i+1]) || (!asc && report[i] > report[i+1])) // if it should be ASCENDING AND this jump is ASCENDING OR it should be DESCENDING AND this jump is DESCENDING
            && diff >= 1 && diff <= 3;  // if the difference between the two numbers is GREATER THAN 1 AND LESS THAN 3 

    }

    return safe;
}