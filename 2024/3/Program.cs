using System.Text.RegularExpressions;

var lines = File.ReadAllLines("input.txt");

var matcher = new Regex(@"mul\(\d+,\d+\)");

var tot = 0;

foreach(var line in lines)
{
    var matches = matcher.Matches(line);

    foreach(var match in matches)
    {
        var str = match.ToString();
        tot += str.Substring(4, str.Length - 5).Split(",").Select(m => int.Parse(m)).Aggregate((a, b) => a * b);
    }
}

Console.WriteLine(tot);

matcher = new Regex(@"mul\(\d+,\d+\)|do\(\)|don't\(\)");

tot = 0;
var on = true;

foreach(var line in lines)
{
    var matches = matcher.Matches(line);

    foreach(var match in matches)
    {
        var str = match.ToString();

        if (str.Contains("mul") && on)
        {        
            tot += str.Substring(4, str.Length - 5).Split(",").Select(m => int.Parse(m)).Aggregate((a, b) => a * b);
        }
        else if (str.Contains("don't()") && on)
        {
            on = false;
        }
        else if (str.Contains("do()") && !on)
        {
            on = true;
        }
    }
}

Console.WriteLine(tot);
