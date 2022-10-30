using System.Text.RegularExpressions;

var lines = File.ReadAllLines("./input.txt");

int charDiff = 0;
int extraEncodedChars = 0;

// match \x27 etc, but DON'T match \\x27, but DO match \\\x27 :D
Regex hexCharacterLookup = new Regex(@"(?=[^\\]\\x..){1}|\\\\\\x..");

// escaped backslash \\
Regex slashLookup = new Regex(@"\\\\");

// escaped quote, not immediately followed by the end of the line 
// (e.g. if the string ENDS with a backslash, it will be in the file as \\", but we don't want to match that)
Regex quoteLookup = new Regex("\\\\\"(?!$)");

foreach (var line in lines)
{
    charDiff +=
        2 // start and end quotes
        +
        3 * hexCharacterLookup.Matches(line).Count // \x27 -> ' (4 characters to 1, increase diff by 3)
        +
        slashLookup.Matches(line).Count // \\ -> \ (2 characters to 1, increase diff by 1)
        +
        quoteLookup.Matches(line).Count; // \" -> " (2 characters to 1, increase diff by 1)

    extraEncodedChars +=
        2 // new start and end quotes
        +
        2 // new escape on old start and end quotes
        +
        2 * slashLookup.Matches(line).Count // \\ -> \\\\
        +
        2 * quoteLookup.Matches(line).Count // \" -> \\\"
        +
        hexCharacterLookup.Matches(line).Count; // \x26 -> \\x26
}

Console.WriteLine(charDiff);
Console.WriteLine(extraEncodedChars);