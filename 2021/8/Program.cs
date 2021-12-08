var lines = File.ReadAllLines("./input.txt");

var simpleInputsOutputs = lines.Select(l => l.Split(" | ")).ToList();

var outputs = simpleInputsOutputs.Select(io => io[1].Split(" ")).ToList();

var count = outputs.SelectMany(o => o).Count(c => c.Length == 2 || c.Length == 3 || c.Length == 4 || c.Length == 7);

Console.WriteLine(count);

List<(List<string> input, List<string> output)> tupleIO = simpleInputsOutputs.Select(j => (j[0].Split(" ").ToList(), j[1].Split(" ").ToList())).ToList();
var sum = 0;
foreach(var io in tupleIO)
{

    var test = new SevenSegment{Input = io.input, Output = io.output};
    sum += test.Solve();
}

Console.WriteLine(sum);

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
            var arr = candidate.ToCharArray();
            
            // zero is the only length 6 string that shares 3 entries with seven
            if (arr.ToList().Intersect(sevenArr).Count() == 3)
            {
                map[candidate] = 0;
            }
            // nine is the only length 6 string that shares 4 entries with four
            else if (arr.ToList().Intersect(fourArr).Count() == 4)
            {
                map[candidate] = 9;
            }
        }

        // the remaining entry in the list must be six
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
            // the keys aren't necessarily strings in the same order as the original observations
            var correctKey = map.Keys.Single(
                k => k.Length == key.Length && 
                k.ToCharArray().Intersect(key.ToCharArray()).Count() == k.Length);

            int value = map[correctKey];

            ans += value;
        }

        return int.Parse(ans);
    }
}