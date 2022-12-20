var input = File.ReadAllLines("./input").Select(d => int.Parse(d)).ToList();

long decrypt = 811_589_153;
// long decrypt = 1;

var linkedList = new LinkedList<Box>();

foreach(var inp in input)
{
    var val = decrypt * inp;
    // when jumping, the list is one less long because we are "holding" the current number
    // so `% (n-1)`
    var jump = val % (input.Count - 1); 
    
    linkedList.AddLast(new Box(val, jump));
}

var orig = linkedList.ToList();

// Console.WriteLine(string.Join(", ", linkedList.Select(l => l.Value)));

for (int i = 0; i < 10; i++)
{
    foreach(var o in orig)
    {
        var current = linkedList.Find(o);

        var next = current.Next?? linkedList.First;
        var previous = current.Previous?? linkedList.Last;

        linkedList.Remove(current);

        if (current.Value.JumpValue > 0)
        {
            for (int j = 0; j < current.Value.JumpValue - 1; j++)
            {
                next = next.Next ?? linkedList.First;
            }

            linkedList.AddAfter(next, current);
        }
        else if (current.Value.JumpValue < 0)
        {
            for (int j = 0; j > current.Value.JumpValue + 1; j--)
            {
                previous = previous.Previous ?? linkedList.Last;
            }

            linkedList.AddBefore(previous, current);
        }
        else 
        {
            linkedList.AddBefore(next, current);
        }
    }
}

var curr = linkedList.First;
long count = 0L;
while (true)
{
    if (curr.Value.Value != 0)
    {
        curr = curr.Next;
    }
    else
    {
        curr = curr.Next ?? linkedList.First;
        for (int i = 1; i <= 3000; i++)
        {
            if (i % 1000 == 0)
            {
                count += curr.Value.Value;
            }
            curr = curr.Next ?? linkedList.First;
        }

        break;
    }
}

Console.WriteLine(count);




class Box {
    public long Value { get; set; }
    public long JumpValue { get; set; }

    public Box(long value, long jump)
    {
        this.Value = value;
        this.JumpValue = jump;
    }
}
