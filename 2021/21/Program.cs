// i'm choosing to confuse myself by zero indexing the board
var board = new Dictionary<int, int>
    { 
        { 0, 1 },
        { 1, 2 },
        { 2, 3 },
        { 3, 4 },
        { 4, 5 },
        { 5, 6 },
        { 6, 7 },
        { 7, 8 },
        { 8, 9 },
        { 9, 10 }
    };

var p1Space = 9;
var p2Space = 8;

var possibleRollsInOrder = new List<int>();

for (int i = 1; possibleRollsInOrder.Count < 100; i = (i + 3) > 100 ? i - 97 : i + 3)
{
    var firstTerm = i;
    var secondTerm = i + 1 > 100 ? i - 99 : i + 1;
    var thirdTerm = i + 2 > 100 ? i - 98 : i + 2;
    possibleRollsInOrder.Add(firstTerm + secondTerm + thirdTerm);
}

var p1Score = 0;
var p2Score = 0;

var nextDiceRoll = 0;
var diceRollCounter = 0;
while (true)
{
    p1Space = (p1Space + possibleRollsInOrder[nextDiceRoll]) % 10;
    p1Score += board[p1Space];

    nextDiceRoll++;
    nextDiceRoll %= 100;

    diceRollCounter += 3;

    if (p1Score >= 1000)
    {
        break;
    }

    p2Space = (p2Space + possibleRollsInOrder[nextDiceRoll]) % 10;
    p2Score += board[p2Space];

    nextDiceRoll++;
    nextDiceRoll %= 100;

    diceRollCounter += 3;

    if (p2Score >= 1000)
    {
        break;
    }
}

Console.WriteLine((long)Math.Min(p2Score, p1Score) * diceRollCounter); //306027 too low

// part 2

// var test = Recurse(0, 3);
// var test2 = Recurse(0, 7);
// Console.WriteLine();

// long p1wins = 0;
// foreach(var res in test)
// {
//     var p2loseUniverses = test2.Where(t => t.Key >= res.Key).ToList();
//     var p2winUniverses = test2.Where(t => t.Key < res.Key).ToList();
//     p1wins += res.Value * (p2loseUniverses.Sum(v => v.Value));
// }
// Console.WriteLine(p1wins);


// long p2wins = 0;
// foreach(var res in test2)
// {
//     var p1loseUniverses = test.Where(t => t.Key >= res.Key).ToList();
//     var p1winUniverses = test.Where(t => t.Key < res.Key).ToList();
//     p2wins += res.Value * (p1loseUniverses.Sum(v => v.Value));
// }

// Console.WriteLine(p2wins);

// Console.WriteLine(p1wins + p2wins);

var test = CountWins(9, 0, 8, 0, true, new Dictionary<(int, int, int, int, bool), (long, long)>());

Console.WriteLine(Math.Max(test.p1wins, test.p2wins));

// calculate the full tree of (waysTo21, turnsToGetThere) for both start positions
// work out who wins more etc, based on that.

(long p1wins, long p2wins) CountWins(int p1Pos, int p1Score, int p2Pos, int p2Score, bool player1ToGo, IDictionary<(int p1pos, int p1Score, int p2Pos, int p2Score, bool p12go), (long p1, long p2)> cache)
{
    var board = new Dictionary<int, int>
    { 
        { 0, 1 },
        { 1, 2 },
        { 2, 3 },
        { 3, 4 },
        { 4, 5 },
        { 5, 6 },
        { 6, 7 },
        { 7, 8 },
        { 8, 9 },
        { 9, 10 }
    };

    var ways = new Dictionary<int, long>
    {
        {3, 1},
        {4, 3},
        {5, 6},
        {6, 7},
        {7, 6},
        {8, 3},
        {9, 1}
    };
    
    if (p1Score >= 21)
    {
        return (1, 0);
    }
    else if (p2Score >= 21)
    {
        return (0, 1);
    }
    else
    {
        long p1wins = 0;
        long p2wins = 0;

        int currentPosition = -1;
        int currentScore = -1;

        if (player1ToGo)
        {
            currentPosition = p1Pos;
            currentScore = p1Score;
        }
        else
        {
            currentPosition = p2Pos;
            currentScore = p2Score;
        }

        foreach(var way in ways)
        {

            (long p1, long p2) winnersFromHere;

            if (player1ToGo)
            {
                var newPos = (p1Pos + way.Key) % 10;
                var newScore = (p1Score + board[newPos]);
                
                if (cache.TryGetValue((newPos, newScore, p2Pos, p2Score, false), out var val))
                {
                    winnersFromHere = val;
                }
                else
                {
                    val = CountWins(newPos, newScore, p2Pos, p2Score, false, cache);
                    cache.Add((newPos, newScore, p2Pos, p2Score, false), val);
                    winnersFromHere = val;
                }
            }
            else
            {
                
                var newPos = (p2Pos + way.Key) % 10;
                var newScore = (p2Score + board[newPos]);

                if (cache.TryGetValue((p1Pos, p1Score, newPos, newScore, true), out var val))
                {
                    winnersFromHere = val;
                }
                else
                {
                    val = CountWins(p1Pos, p1Score, newPos, newScore, true, cache);
                    cache.Add((p1Pos, p1Score, newPos, newScore, true), val);
                    winnersFromHere = val;
                }
            }
            
            p1wins += winnersFromHere.p1 * way.Value;
            p2wins += winnersFromHere.p2 * way.Value;
        }

        return (p1wins, p2wins);
    }

}

IDictionary<int, long> Recurse(int currentScore, int currentPosition)
{
    var board = new Dictionary<int, int>
    { 
        { 0, 1 },
        { 1, 2 },
        { 2, 3 },
        { 3, 4 },
        { 4, 5 },
        { 5, 6 },
        { 6, 7 },
        { 7, 8 },
        { 8, 9 },
        { 9, 10 }
    };

    var result = new Dictionary<int, long>();

    // one way of rolling 3
    var newPos = (currentPosition + 3) % 10;
    var newScore = currentScore + board[newPos];
    if (newScore >= 21)
    {
        result.TryGetValue(1, out var val);
        result[1] = val + 1;
    }
    else
    {
        var intermediateResult = Recurse(newScore, newPos);
        foreach(var res in intermediateResult)
        {
            result.TryGetValue(res.Key + 1, out var val);
            result[res.Key + 1] = val + res.Value;
        }
    }

    // three ways of rolling 4
    newPos = (currentPosition + 4) % 10;
    newScore = currentScore + board[newPos];
    if (newScore >= 21)
    {
        result.TryGetValue(1, out var val);
        result[1] = val + 3;
    }
    else
    {
        var intermediateResult = Recurse(newScore, newPos);
        foreach(var res in intermediateResult)
        {
            result.TryGetValue(res.Key + 1, out var val);
            result[res.Key + 1] = val + res.Value * 3;
        }
    }
    
    // six ways of rolling 5
    newPos = (currentPosition + 5) % 10;
    newScore = currentScore + board[newPos];
    if (newScore >= 21)
    {
        result.TryGetValue(1, out var val);
        result[1] = val + 6;
    }
    else
    {
        var intermediateResult = Recurse(newScore, newPos);
        foreach(var res in intermediateResult)
        {
            result.TryGetValue(res.Key + 1, out var val);
            result[res.Key + 1] = val + res.Value * 6;
        }
    }
    
    // seven ways of rolling 6
    newPos = (currentPosition + 6) % 10;
    newScore = currentScore + board[newPos];
    if (newScore >= 21)
    {
        result.TryGetValue(1, out var val);
        result[1] = val + 7;
    }
    else
    {
        var intermediateResult = Recurse(newScore, newPos);
        foreach(var res in intermediateResult)
        {
            result.TryGetValue(res.Key + 1, out var val);
            result[res.Key + 1] = val + res.Value * 7;
        }
    }

    // six ways of rolling 7
    newPos = (currentPosition + 7) % 10;
    newScore = currentScore + board[newPos];
    if (newScore >= 21)
    {
        result.TryGetValue(1, out var val);
        result[1] = val + 6;
    }
    else
    {
        var intermediateResult = Recurse(newScore, newPos);
        foreach(var res in intermediateResult)
        {
            result.TryGetValue(res.Key + 1, out var val);
            result[res.Key + 1] = val + res.Value * 6;
        }
    }

    // three ways of rolling 8
    newPos = (currentPosition + 8) % 10;
    newScore = currentScore + board[newPos];
    if (newScore >= 21)
    {
        result.TryGetValue(1, out var val);
        result[1] = val + 3;
    }
    else
    {
        var intermediateResult = Recurse(newScore, newPos);
        foreach(var res in intermediateResult)
        {
            result.TryGetValue(res.Key + 1, out var val);
            result[res.Key + 1] = val + res.Value * 3;
        }
    }

    // one way of rolling 9
    newPos = (currentPosition + 9) % 10;
    newScore = currentScore + board[newPos];
    if (newScore >= 21)
    {
        result.TryGetValue(1, out var val);
        result[1] = val + 1;
    }
    else
    {
        var intermediateResult = Recurse(newScore, newPos);
        foreach(var res in intermediateResult)
        {
            result.TryGetValue(res.Key + 1, out var val);
            result[res.Key + 1] = val + res.Value;
        }
    }

    return result;
}