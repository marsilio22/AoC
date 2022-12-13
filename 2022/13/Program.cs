var lines = File.ReadAllLines("./input");

var score = 0;
var comparer = new ListComparer();

for (int i = 0; i < lines.Length; i += 3)
{
    var prog = 0;
    var first = Build(lines[i].Substring(1, lines[i].Length - 1), ref prog);

    prog = 0;
    var second = Build(lines[i+1].Substring(1, lines[i+1].Length - 1), ref prog);

    var index = i/3 + 1;

    var res = comparer.Compare(first, second);

    if (res == 1)
    {
        score += index;
    }
}

Console.WriteLine(score);

// p2

var progress = 0;
var divider1 = Build("[[2]]", ref progress);
progress = 0;
var divider2 = Build("[[6]]", ref progress);

var part2 = lines
    .Where(l => !l.Equals(string.Empty))
    .Select(l => {
        var prog = 0;
        return Build(l, ref prog);
    })
    .Append(divider1)
    .Append(divider2)
    .OrderByDescending(d => d, new ListComparer())
    .ToList();

var thing = part2.IndexOf(divider1);
var thing2 = part2.IndexOf(divider2);

Console.WriteLine((thing + 1) * (thing2 + 1));

List<object> Build(string thing, ref int progress)
{
    List<object> test = new List<object>();

    for (int j = progress; j < thing.Length-1;)
    {
        if (thing[j] == '[')
        {
            var prog = j+1;
            test.Add(Build(thing, ref prog));

            j = prog;
        }
        else if (thing[j] == ',')
        {
            j++;
        }
        else if (thing[j] == ']')
        {
            j++;
            progress = j;
            return test;
        }
        else
        {
            var subst = thing.Substring(j);
            var numberEnd = Math.Min(subst.IndexOf(","), subst.IndexOf("]"));

            if (numberEnd == -1)
            {
                numberEnd = Math.Max(subst.IndexOf(","), subst.IndexOf("]"));
            }

            var num = int.Parse(subst.Substring(0, numberEnd));
            test.Add(num);
            j += numberEnd;
        }


        progress = j;
    }

    return test;
}

class ListComparer : IComparer<List<object>>
{
    public int Compare(List<object>? x, List<object>? y)
    {
        if (x == null || y == null)
        {
            throw new Exception();
        }


        for(int i = 0; i < x.Count; i++)
        {
            if (y.Count <= i)
            {
                // second list ran out first
                return -1;
            }
            
            var firstVal = x[i];
            var secondVal = y[i];

            if (firstVal.GetType() == typeof(int) && secondVal.GetType() == typeof(int))
            {
                if ((int)firstVal < (int)secondVal)
                {
                    return 1;
                }
                else if ((int)firstVal > (int) secondVal)
                {
                    return -1;
                }
                else
                {
                    continue;
                }
            }
            else if (firstVal.GetType() == typeof(List<object>) && secondVal.GetType() == typeof(int))
            {
                var res = Compare((List<object>)firstVal, new List<object>{secondVal});

                if (res == 0)
                {
                    continue;
                }
                else
                {
                    return res;
                }
            }
            else if (firstVal.GetType() == typeof(int) && secondVal.GetType() == typeof(List<object>))
            {
                var res = Compare(new List<object>{firstVal}, (List<object>)secondVal);

                if (res == 0)
                {
                    continue;
                }
                else
                {
                    return res;
                }
            }
            else // both lists
            {
                var res = Compare((List<object>)firstVal, (List<object>)secondVal);

                if (res == 0)
                {
                    continue;
                }
                else
                {
                    return res;
                }
            }
        }

        if (x.Count == y.Count)
        {
            return 0;
        }
        else
        {
            return 1;
        }
    }
}