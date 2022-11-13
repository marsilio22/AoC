using System.Text;

var number = "1113122113";

var curr = '1';
var count = 0;

for(int i = 1; i <= 50; i++)
{

    var sb = new StringBuilder();

    foreach (var character in number)
    {
        if (character == curr)
        {
            count++;
        }
        else
        {
            sb.Append(count);
            sb.Append(curr);

            curr = character;
            count = 1;
        }
    }

    // handle reaching the end of the string
    sb.Append(count);
    sb.Append(curr);

    // prepare for next loop
    number = sb.ToString();
    curr = number.First();
    count = 0;
    
    if (i % 10 == 0)
    {
        Console.WriteLine($"at step {i}, length is {number.Length}");
    }
}
