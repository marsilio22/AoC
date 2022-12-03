var input = File.ReadAllLines("./input.txt");

var prioP1 = 0;
var prioP2 = 0;

for (int i = 0; i < input.Length; i++)
{
    var first = input[i].Substring(0, input[i].Length / 2);
    var second = input[i].Substring(input[i].Length / 2);

    var misplaced = first.Where(f => second.Contains(f)).Distinct().Single();

    var misplacedItemValue = (int)misplaced - 96 <= 0 ? (int)misplaced - 38: (int)misplaced - 96 ;

    prioP1 += misplacedItemValue;

    if (i%3 == 0)
    {
        var group = new List<string> {input[i], input[i+1], input[i+2]};

        var badge = group.SelectMany(g => g).Where(c => group[0].Contains(c) && group[1].Contains(c) && group[2].Contains(c)).Distinct().Single();

        var badgeValue = (int)badge - 96 <= 0 ? (int)badge - 38: (int)badge - 96 ;

        prioP2 += badgeValue;

    }
}

Console.WriteLine(prioP1);
Console.WriteLine(prioP2);

