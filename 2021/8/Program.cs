var lines = File.ReadAllLines("./input.txt");

var inputOutput = lines.Select(l => l.Split(" | ")).ToList();

var outputs = inputOutput.Select(io => io[1].Split(" ")).ToList();

var count = outputs.SelectMany(o => o).Count(c => c.Length == 2 || c.Length == 3 || c.Length == 4 || c.Length == 7);

Console.WriteLine(count);

List<(List<string> input, List<string> output)> inputOutput2 = inputOutput.Select(j => (j[0].Split(" ").ToList(), j[1].Split(" ").ToList())).ToList();
var sum = 0;
foreach(var io in inputOutput2)
{

    var test = new SevenSegment{Input = io.input, Output = io.output};
    sum += test.Solve();
}

Console.WriteLine(sum);

// 0 = 6 shares only with 7
// 6 = 6 remainder
// 9 = 6 shares only with 4

// 2 = 5 three off sharing with 6
// 3 = 5 shares only with 1 --------
// 5 = 5 one off sharing with 6

// 1 = 2
// 4 = 4
// 7 = 3
// 8 = 7

public class SevenSegment
{
    public ICollection<string> Input {get;set;}
    public ICollection<string> Output {get;set;}

    private IDictionary<string, int> map = new Dictionary<string, int>();

    public int Solve()
    {
        
        var one = Input.Single(i => i.Length == 2);
        var seven = Input.Single(i => i.Length == 3);
        var four = Input.Single(i => i.Length == 4);
        var eight = Input.Single(i => i.Length == 7);

        var oneArr = one.ToCharArray();
        var sevenArr = seven.ToCharArray();
        var fourArr = four.ToCharArray();
        var eightArr = eight.ToCharArray();

        map[one] = 1;
        map[four] = 4;
        map[seven] = 7;
        map[eight] = 8;

        var zeroSixNine = Input.Where(i => i.Length == 6).ToList();
        foreach(var candidate in zeroSixNine)
        {
            // zero is the only length 6 string that shares 3 entries with seven
            var arr = candidate.ToCharArray();

            if (arr.ToList().Intersect(sevenArr).Count() == 3)
            {
                map[candidate] = 0;
            }

            if (arr.ToList().Intersect(fourArr).Count() == 4)
            {
                map[candidate] = 9;
            }
        }

        var six = zeroSixNine.Single(z => !map.Keys.Contains(z));
        map[six] = 6;
        var sixArr = six.ToCharArray();

        var twoThreeFive = Input.Where(i => i.Length == 5).ToList();

        foreach(var candidate in twoThreeFive)
        {
            // three is the only length 5 string that shares 2 entries with one
            var arr = candidate.ToCharArray();
            if (arr.ToList().Intersect(oneArr).Count() == 2)
            {
                map[candidate] = 3;
            }
            // six and five share 4 entries
            else if (arr.ToList().Intersect(sixArr).Count() == 5)
            {
                map[candidate] = 5;
            }
            // six and two share 2 entries
            else if (arr.ToList().Intersect(sixArr).Count() == 4)
            {
                map[candidate] = 2;
            }
        }

        var ans = string.Empty;
        foreach(var key in Output)
        {
            var test = map.Keys.Single(k => k.Length == key.Length && k.ToCharArray().Intersect(key.ToCharArray()).Count() == k.Length);

            int value = map[test];

            ans += value;
        }

        return int.Parse(ans);
    }
}