var lines = File.ReadAllLines("./input.txt");

var numbers = new List<SnailfishNumber>();

foreach (var line in lines)
{
    int i = 1;

    numbers.Add(RecursiveParse(line, ref i));
}

Console.WriteLine();

// var depth = 0;
// foreach(var num in numbers)
// {
//     depth++;
//     Console.WriteLine(num);
//     ExplodeLeftMost(num, ref depth);
//     Console.WriteLine(num + "  -  " + depth);
// }

// Console.WriteLine();

SnailfishNumber runningSum = new SnailfishNumber{Left= numbers[0].Left, Right = numbers[0].Right};

for(int i = 1; i < numbers.Count; i++)
{
    runningSum += numbers[i];

    while(true)
    {
        // explode until no more explosions can occur, then split. 
        // if the result of the split is false, we are done, otherwise we need to 
        // explode again
        var depth = 1;
        SnailfishNumber test = ExplodeLeftMost(runningSum, ref depth).number;

        while(test != null)
        {
            depth = 1;
            test = ExplodeLeftMost(runningSum, ref depth).number;
        }

        if (!SplitLeftMost(runningSum))
        {
            break;
        }
    }
}

// part 1
Console.WriteLine(runningSum);
Console.WriteLine(runningSum.Magnitude);

// part 2
var maxMagnitude = long.MinValue;


for (int i = 0; i < numbers.Count; i++)
{
    for (int j = 0; j < numbers.Count; j++)
    {
        if (j == i)
        {
            continue;
        }

        numbers.Clear();
        // reparse every time because c# is annoying
        foreach (var line in lines)
        {
            int k = 1;

            numbers.Add(RecursiveParse(line, ref k));
        }

        runningSum = numbers[i] + numbers[j];

        while(true)
        {
            // explode until no more explosions can occur, then split. 
            // if the result of the split is false, we are done, otherwise we need to 
            // explode again
            var depth = 1;
            SnailfishNumber test = ExplodeLeftMost(runningSum, ref depth).number;

            while(test != null)
            {
                depth = 1;
                test = ExplodeLeftMost(runningSum, ref depth).number;
            }

            if (!SplitLeftMost(runningSum))
            {
                break;
            }
        }

        if (runningSum.Magnitude > maxMagnitude)
        {
            maxMagnitude = runningSum.Magnitude;
        }
    }
}

Console.WriteLine(maxMagnitude); // 3146 too low

bool SplitLeftMost(SnailfishNumber input)
{
    if (input.Left.Value.HasValue && input.Left.Value.Value >=10)
    {
        var left = (int)Math.Round((double)input.Left.Value / 2, 0, MidpointRounding.ToZero);
        var right = (int)Math.Round((double)input.Left.Value / 2, 0, MidpointRounding.AwayFromZero);

        input.Left = new SnailfishNumber{ Left = new SnailfishNumber{Value = left}, Right = new SnailfishNumber{Value = right}};
        return true;
    }
    else if (input.Left.Value.HasValue && input.Left.Value.Value < 10)
    {
        // try right
        if (input.Right.Value.HasValue && input.Right.Value.Value >= 10)
        {
            var left = (int)Math.Round((double)input.Right.Value / 2, 0, MidpointRounding.ToZero);
            var right = (int)Math.Round((double)input.Right.Value / 2, 0, MidpointRounding.AwayFromZero);
            
            input.Right = new SnailfishNumber{ Left = new SnailfishNumber{Value = left}, Right = new SnailfishNumber{Value = right}};
            return true;
        }
        else if (input.Right.Value.HasValue && input.Right.Value.Value < 10)
        {
            return false;
        }
        else 
        {
            return SplitLeftMost(input.Right);
        }
    }
    else 
    {
        if(SplitLeftMost(input.Left))
        {
            return true;
        }
        else 
        {
            if (input.Right.Value.HasValue && input.Right.Value.Value >= 10)
            {
                var left = (int)Math.Round((double)input.Right.Value / 2, 0, MidpointRounding.ToZero);
                var right = (int)Math.Round((double)input.Right.Value / 2, 0, MidpointRounding.AwayFromZero);
                
                input.Right = new SnailfishNumber{ Left = new SnailfishNumber{Value = left}, Right = new SnailfishNumber{Value = right}};
                return true;
            }
            else if (input.Right.Value.HasValue && input.Right.Value.Value < 10)
            {
                return false;
            }
            else 
            {
                return SplitLeftMost(input.Right);
            }
        }
    }
}

(SnailfishNumber number, bool leftHandled, bool rightHandled, bool numberHasBeenTurnedInto0) ExplodeLeftMost(SnailfishNumber input, ref int depth)
{
    var checkedLeft = false;
    while (true)
    {
        // recurse down the left most thing until hit a value
        if (input.Left != null && !checkedLeft)
        {
            depth++;
            var recursionResult = ExplodeLeftMost(input.Left, ref depth);

            if (recursionResult.number == null)
            {
                checkedLeft = true;
                continue;
            }

            if (recursionResult.number.Value.HasValue)
            {
                // it was a single value, but certainly deep enough
                // if this number has both left and right, then this is what we should explode
                // otherwise we should continue to the right
                if (input.Right.Value.HasValue)
                {
                    // take a copy of input to return, otherwise there's pointer issues later

                    depth --;
                    return (new SnailfishNumber { Left = input.Left, Right = input.Right }, false, false, false);
                }
                else
                {
                    checkedLeft = true;
                    continue;
                }
            }

            // if test has returned a number, we need to add it's left and right to the immediate left and right
            // VALUES in the tree.
            if (!recursionResult.numberHasBeenTurnedInto0)
            {
                // this must be the immediate parent of the exploded value, so replace it by 0
                input.Left.Left = null;
                input.Left.Right = null;
                input.Left.Value = 0;
                recursionResult.numberHasBeenTurnedInto0 = true;
            }

            if (!recursionResult.rightHandled)
            {
                if (input.Right.Value.HasValue)
                {
                    input.Right.Value += recursionResult.number.Right.Value.Value;
                }
                else
                {
                    input.Right.AddToLeftMostNumber(recursionResult.number.Right.Value.Value);
                }
                recursionResult.rightHandled = true;
            }

            depth--;
            return recursionResult;
        }
        else if (input.Right != null && checkedLeft)
        {
            depth++;
            var recursionResult = ExplodeLeftMost(input.Right, ref depth);

            if (recursionResult.number == null)
            {
                depth--;
                return recursionResult;
            }
            
            // NB, here don't need to worry about test.number.Value being non null, as the left case should handle that

            // if test has returned a number, we need to add its left and right to the immediate left and right
            // VALUES in the tree.
            if (!recursionResult.numberHasBeenTurnedInto0)
            {
                // this must be the immediate parent of the exploded value, so replace it by 0
                input.Right.Left = null;
                input.Right.Right = null;
                input.Right.Value = 0;
                recursionResult.numberHasBeenTurnedInto0 = true;
            }

            if (!recursionResult.leftHandled)
            {                
                if (input.Left.Value.HasValue)
                {
                    input.Left.Value += recursionResult.number.Left.Value.Value;
                }
                else
                {
                    input.Left.AddToRightMostNumber(recursionResult.number.Left.Value.Value);
                }
                recursionResult.leftHandled = true;
            }

            depth --;
            return recursionResult;
        }
        else
        {
            if (depth >= 6) // off by 2 because this is a single number, not a pair
            {
                depth --;
                return (input, false, false, false);
            }
            else
            {
                depth --;
                return (null, false, false, false);
            }
        }
    }
}

SnailfishNumber RecursiveParse(string number, ref int i)
{
    SnailfishNumber result = new SnailfishNumber();
    var expectedClosingBrackets = 1;
    var targettingLeft = true;
    while (expectedClosingBrackets > 0)
    {
        if (number[i] == ']')
        {
            i++;
            expectedClosingBrackets--;
        }
        else if (number[i] == '[')
        {
            i++;
            var num = RecursiveParse(number, ref i);
            if (targettingLeft)
            {
                result.Left = num;
            }
            else
            {
                result.Right = num;
            }
        }
        else if (number[i] == ',')
        {
            i++;
            targettingLeft = false;
        }
        else
        {
            var num = new SnailfishNumber { Value = int.Parse(number[i].ToString()) };
            i++;
            if (targettingLeft)
            {
                result.Left = num;
            }
            else
            {
                result.Right = num;
            }
        }
    }

    return result;
}

class SnailfishNumber
{
    public SnailfishNumber Left { get; set; }
    public SnailfishNumber Right { get; set; }

    public int? Value { get; set; }

    public long Magnitude
    {
        get
        {
            if (Value.HasValue)
            {
                return Value.Value;
            }

            return 3 * Left.Magnitude + 2 * Right.Magnitude;
        }
    }

    public void AddToLeftMostNumber(int value)
    {
        if (this.Value.HasValue)
        {
            this.Value += value;
        }
        else
        {
            this.Left.AddToLeftMostNumber(value);
        }
    }

    public void AddToRightMostNumber(int value)
    {
        if (this.Value.HasValue)
        {
            this.Value += value;
        }
        else
        {
            this.Right.AddToRightMostNumber(value);
        }
    }

    public static SnailfishNumber operator +(SnailfishNumber left, SnailfishNumber right)
    {
        return new SnailfishNumber { Left = left, Right = right };
    }

    public override string ToString()
    {
        if (this.Value.HasValue)
        {
            return Value.Value.ToString();
        }

        return $"[{Left.ToString()},{Right.ToString()}]";
    }
}