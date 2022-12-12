using System.Text;

var input = "01000100010010111";

Part(input, 272);
Part(input, 35_651_584);

void Part(string input, int length)
{
    while (input.Length < length)
    {
        var sb = new StringBuilder();

        sb.Append(input);
        sb.Append("0");
        sb.Append(string.Join(string.Empty, input.Reverse().Select(s => s == '0' ? '1' : '0')));

        input = sb.ToString();
    }

    while(Math.Min(length, input.Length) % 2 != 1)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < Math.Min(length, input.Length); i+=2)
        {
            if (input[i] == input[i+1])
            {
                sb.Append("1");
            }
            else
            {
                sb.Append("0");
            }
        }
        input = sb.ToString();
    }

    Console.WriteLine(input);
}