using System.Text;

var pwd = "cqjxjnds";

int a = 'a';
int z = 'z';

var straights = new List<string>();

// only need consider a -> x inclusive == 25 options
for (int i = 0; i <= 24; i++)
{
    var sb = new StringBuilder();

    sb.Append((char)(a + i));
    sb.Append((char)(a + i + 1));
    sb.Append((char)(a + i + 2));

    straights.Add(sb.ToString());
}

var banned = new [] { 'i', 'o', 'l'};

var pairs = new List<string>();

for (int i = 0; i <= 25; i++)
{
    pairs.Add(((char)(a + i)).ToString() + (char)(a + i));
}

int counter = 0;

while(counter != 2)
{
    var containsBanned = banned.Any(b => pwd.Contains(b));
    var containsTwoPairs = pairs.Count(p => pwd.Contains(p)) >= 2;
    var containsStraight = straights.Any(s => pwd.Contains(s));

    if (containsStraight && containsTwoPairs && !containsBanned)
    {
        counter++;
        Console.WriteLine(pwd);
        pwd = Increment(pwd);
    }
    else
    {
        pwd = Increment(pwd);
    }
}

string Increment(string curr)
{
    var arr = curr.ToCharArray();

    var ints = arr.Select(x => (int)x - a).ToArray();

    for (int i = arr.Length - 1; i >= 0; i--)
    {
        if((ints[i] + 1) % 26 == 0)
        {
            ints[i] = 0;
        }
        else
        {
            ints[i] = ints[i] + 1;
            break;
        }
    }

    var val = string.Join(string.Empty, ints.Select(i => (char)(i + a)));

    return val;
}