var lines = File.ReadAllLines("./input.txt");

var matchingSymbols = new Dictionary<char, char>{
    { '(', ')' },
    { '[', ']' },
    { '{', '}' },
    { '<', '>' }
};

// checker and completer score on different things so can use one dict for both B-)
var pointValues = new Dictionary<char, int>
{
    // syntax checker scores
    { ')', 3 },
    { ']', 57 },
    { '}', 1197 },
    { '>', 25137 },
    
    // auto completer scores
    { '(', 1 },
    { '[', 2 },
    { '{', 3 },
    { '<', 4 }
};

var checkerScore = 0;
var completerScores = new List<long>();

foreach(var line in lines)
{
    var lineCorrupt = false;
    var stack = new Stack<char>();
    for(int i = 0; i < line.Length && !lineCorrupt; i++)
    {
        if (matchingSymbols.Keys.Contains(line[i]))
        {
            stack.Push(line[i]);
        }
        else
        {
            if (line[i] != matchingSymbols[stack.Peek()])
            {
                checkerScore += pointValues[line[i]];
                lineCorrupt=true;
            }
            else
            {
                // not corrupt, so pop the match
                stack.Pop();
            }
        }
    }

    if (!lineCorrupt)
    {
        long lineCompleterScore = 0;

        while(stack.Any())
        {
            var character = stack.Pop();
            lineCompleterScore *= 5;
            lineCompleterScore += pointValues[character];
        }
        completerScores.Add(lineCompleterScore);
    }
}

Console.WriteLine(checkerScore);

completerScores = completerScores.OrderByDescending(c => c).ToList();
// assume there's an odd number
Console.WriteLine(completerScores[(completerScores.Count - 1) / 2]);