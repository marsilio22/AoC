var input = File.ReadAllText("./input.txt");

Console.WriteLine(FindConsecutiveChars(input, 4));
Console.WriteLine(FindConsecutiveChars(input, 14));

int FindConsecutiveChars(string input, int count)
{
    for (int i = count; i < input.Length; i ++)
    {
        var h = new HashSet<char>();

        for (int j = 1; j <= count; j++)
        {
            h.Add(input[i-j]);
        }

        if (h.Count == count)
        {
            return i;
        }
    }

    throw new Exception();
}