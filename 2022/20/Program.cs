var input = File.ReadAllLines("./input").Select(d => int.Parse(d)).ToList();

var linkedList = new LinkedList<(int value, bool touched)>();

foreach(var inp in input)
{
    linkedList.AddLast((inp, false));
}

for (int i = 0; i < linkedList.Count; i++)
{
    var current = linkedList.First;
    while(current.Value.touched)
    {
        current = current.Next;
    }

    var next = current.Next?? linkedList.First;
    var previous = current.Previous?? linkedList.Last;

    linkedList.Remove(current);

    current.Value = (current.Value.value, true);

    if (current.Value.value > 0)
    {
        for (int j = 0; j < current.Value.value - 1; j++)
        {
            next = next.Next ?? linkedList.First;
        }

        linkedList.AddAfter(next, current);
    }
    else if (current.Value.value < 0)
    {
        for (int j = 0; j > current.Value.value + 1; j--)
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


var curr = linkedList.First;
var count = 0;
while (true)
{
    if (curr.Value.value != 0)
    {
        curr = curr.Next;
    }
    else
    {
        curr = curr.Next;
        for (int i = 1; i <= 3000; i++)
        {
            if (i % 1000 == 0)
            {
                count += curr.Value.value;
                Console.WriteLine(curr.Value.value);
            }
            curr = curr.Next ?? linkedList.First;
        }

        break;
    }
}

Console.WriteLine(count);

