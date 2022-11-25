var input = File.ReadAllLines("./input.txt");

var lines = new List<string[]>();

foreach(var line in input)
{
    var newline = line.Replace("would lose ", "-");
    newline = newline.Replace("would gain ", "");
    newline = newline.Replace("happiness units by sitting next to ", "");
    newline = newline.Replace(".", "");
    
    lines.Add(newline.Split(" ").ToArray());
}

var names = lines.Select(l => l[0]).Distinct();
var perms = RecursePermutations(names.ToArray());
var maxScore = 0;

// p1

Console.WriteLine(FindMaxScore(perms, lines));

// p2

names = names.Append("me");
perms = RecursePermutations(names.ToArray());
Console.WriteLine(FindMaxScore(perms, lines));

int FindMaxScore(ICollection<string[]> perms, ICollection<string[]> lines){

    maxScore = 0;

    foreach(var perm in perms)
    {
        var score = 0;

        for(int i = 0; i < perm.Length; i++)
        {
            var j = i + 1 == perm.Length ? 0 : i+1;

            // if one of the people is "me" then it's fine, one of these won't exist which is handily equivalent to a score of 0 :)
            var relevantLines = lines.Where(l => 
                (l[0].Equals(perm[i]) && l[2].Equals(perm[j])) ||
                (l[0].Equals(perm[j]) && l[2].Equals(perm[i])));

            score += relevantLines.Sum(r => int.Parse(r[1]));
        }

        if (score > maxScore)
        {
            maxScore = score;
        }
    }

    return maxScore;
}

ICollection<string[]> RecursePermutations(string[] arr)
{
    if (arr.Length == 1)
    {
        return new List<string[]> { arr };
    }

    ICollection<string[]> ans = new List<string[]>();

    foreach(var str in arr)
    {
        var arr2 = arr.Where(a => a != str).ToArray();
        
        var perms = RecursePermutations(arr2);

        foreach(var perm in perms)
        {
            ans.Add(perm.Prepend(str).ToArray());
        }
    }

    return ans;
}