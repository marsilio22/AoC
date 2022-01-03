List<string> lines = File.ReadAllLines("./input.txt").ToList();

var TLSSearchCollection = (from a in Enumerable.Range('a', 26)
from b in Enumerable.Range('a', 26)
where a != b
select(((char)a).ToString() + (char)b + (char)b + (char)a)).ToList();



var ABAs = (from a in Enumerable.Range('a', 26)
    from b in Enumerable.Range('a', 26)
    where a != b
    select (((char)a).ToString() + (char)b + (char)a)).ToList();

List<(string first, string second)> SSLSearchCollection = ABAs.Select(s => (s, s[1].ToString() + s[0] + s[1])).ToList();


var countTLS = 0;
var countSSL = 0;

foreach(var line in lines)
{
    string lineOutsideBrackets = string.Empty;
    string lineInsideBrackets = string.Empty;

    char lastBracket = ']';

    var line2 = line;

    while (line2.Contains('[') || line2.Contains(']'))
    {
        var nextBracketIndex = line2.IndexOfAny(new []{'[', ']'});

        var nextBracket = line2[nextBracketIndex];

        if (nextBracket == lastBracket)
        {
            // two of the same bracket in a row
            continue;
        }

        if (lastBracket == '[')
        {
            lineInsideBrackets += line2.Substring(0, nextBracketIndex) + '|';
        }
        else if (lastBracket == ']')
        {
            lineOutsideBrackets += line2.Substring(0, nextBracketIndex) + '|';
        }

        lastBracket = line2[nextBracketIndex];
        line2 = line2.Substring(nextBracketIndex + 1);
    }

    lineOutsideBrackets += line2;


    if (!TLSSearchCollection.Any(a => lineInsideBrackets.Contains(a)) && TLSSearchCollection.Any(a => lineOutsideBrackets.Contains(a)))
    {
        countTLS++;
    }

    if (SSLSearchCollection.Any(a => lineInsideBrackets.Contains(a.first) && lineOutsideBrackets.Contains(a.second)))
    {
        countSSL++;
    }
}

Console.WriteLine(countTLS);
Console.WriteLine(countSSL);