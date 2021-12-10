var lines = File.ReadAllLines("./input.txt");

var startChars = new [] {'(', '{', '[', '<'};

var checkerScore = 0;
var completerScores = new List<long>();

foreach(var line in lines)
{
    var lineCorrupt = false;
    var stack = new Stack<char>();
    for(int i = 0; i < line.Length; i++)
    {
        if (lineCorrupt)
        {
            break;
        }

        if (startChars.Contains(line[i]))
        {
            stack.Push(line[i]);
        }
        else
        {
            // todo tidy this mess...
            switch (line[i]) {
                case '}':
                    if (stack.Peek() != '{')
                    {
                        checkerScore += 1197;
                        lineCorrupt = true;
                        continue;
                    }
                    else
                    {
                        stack.Pop();
                    }
                    break;
                case ']':
                    if (stack.Peek() != '[')
                    {
                        checkerScore += 57;
                        lineCorrupt = true;
                        continue;
                    }
                    else
                    {
                        stack.Pop();
                    }
                    break;
                case '>':
                    if (stack.Peek() != '<')
                    {
                        checkerScore += 25137;
                        lineCorrupt = true;
                        continue;
                    }
                    else
                    {
                        stack.Pop();
                    }
                    break;
                case ')':
                    if (stack.Peek() != '(')
                    {
                        checkerScore += 3;
                        lineCorrupt = true;
                        continue;
                    }
                    else
                    {
                        stack.Pop();
                    }
                    break;
                default:
                    throw new Exception("oh noes");
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
            switch (character)
            {
                case '{':
                    lineCompleterScore += 3;
                    break;
                case '(':
                    lineCompleterScore += 1;
                    break;
                case '[':
                    lineCompleterScore += 2;
                    break;
                case '<':
                    lineCompleterScore += 4;
                    break;
                default:
                    throw new Exception ("oh noes");
            }
        }
        completerScores.Add(lineCompleterScore);
    }
}

Console.WriteLine(checkerScore);

completerScores = completerScores.OrderByDescending(c => c).ToList();
Console.WriteLine(completerScores[(completerScores.Count - 1) / 2]);