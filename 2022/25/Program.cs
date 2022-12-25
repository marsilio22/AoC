using System.Text;

var lines = File.ReadAllLines("./input");


ICollection<long> nums = new List<long>();

long sum = 0;
var powersOf5 = Enumerable.Range(0, 30).Select(m => (long)Math.Pow(5, m)).ToArray();

foreach(var line in lines)
{
    var pow = 0;
    for (int i = line.Length - 1; i >= 0; i--)
    {
        var constant = powersOf5[pow];

        sum += line[i] switch {
            '2' => constant * 2,
            '1' => constant,
            '0' => 0,
            '-' => constant * -1,
            '=' => constant * -2,
            _ => throw new Exception()
        };

        pow++;
    }
}

Console.WriteLine(sum);

var str = string.Empty;

for (int i = 1; i< powersOf5.Length; i++)
{
    if (sum <= 0)
    {
        break;
    }

    var count = 2;
    var test = sum - count * powersOf5[i-1];

    while(true)
    {
        if( test % powersOf5[i] == 0)
        {
            str = count switch {
                -2 => '=',
                -1 => '-',
                0 => '0',
                1 => '1',
                2 => '2'
            } + str;

            sum = test;
            break;
        }
        else
        {
            test += powersOf5[i-1];
            count--;
        }
    }
}

Console.WriteLine(sum);
Console.WriteLine(str);