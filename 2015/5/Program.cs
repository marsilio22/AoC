using System.Text.RegularExpressions;

var input = File.ReadAllLines("input.txt");

var vowelRegex = new Regex("[aeiou]");
var doubleLetterRegex = new Regex("(.)\\1");
var badSubstringsRegex = new Regex("ab|cd|pq|xy");

var nice = 0;

foreach(var inp in input)
{
    if (vowelRegex.Matches(inp).Count >= 3 && doubleLetterRegex.Matches(inp).Count > 0 && badSubstringsRegex.Matches(inp).Count == 0)
    {
        nice++;
    }
}

Console.WriteLine(nice);

var doublePairLetterRegex = new Regex("(..).*\\1");
var singleLetterTwiceWithGapRegex = new Regex("(.).\\1");

nice = 0;

foreach(var inp in input)
{
    if (doublePairLetterRegex.Matches(inp).Any() && singleLetterTwiceWithGapRegex.Matches(inp).Any())
    {
        nice++;
    }
}

Console.WriteLine(nice);