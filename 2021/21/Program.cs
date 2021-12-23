// i'm choosing to confuse myself by zero indexing the board
var board = new Dictionary<int, int>
{
    { 0, 1 },
    { 1, 2},
    { 2, 3},
    { 3, 4},
    { 4, 5},
    { 5, 6},
    { 6, 7},
    { 7, 8},
    { 8, 9},
    { 9, 10}
};

var p1Space = 9;
var p2Space = 8;

var possibleRollsInOrder = new List<int>();

for (int i = 1;possibleRollsInOrder.Count < 100; i = (i + 3) > 100 ? i-97 : i+3)
{
    var firstTerm = i;
    var secondTerm = i+1 > 100? i -99 : i+1;
    var thirdTerm = i+2 > 100 ? i - 98 : i+2;
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
    
    nextDiceRoll ++;
    nextDiceRoll %= 100;

    diceRollCounter += 3;
    
    if (p1Score >= 1000)
    {
        break;
    }

    p2Space = (p2Space + possibleRollsInOrder[nextDiceRoll]) % 10;
    p2Score += board[p2Space];
    
    nextDiceRoll ++;
    nextDiceRoll %= 100;

    diceRollCounter += 3;

    if (p2Score >= 1000)
    {
        break;
    }
}

Console.WriteLine((long)Math.Min(p2Score, p1Score) * diceRollCounter); //306027 too low


// part 2

// all of these happen on each player's turn
var possibilities = new List<int>
{
    1 + 1 + 1,
    1 + 1 + 2,
    1 + 1 + 3,
    1 + 2 + 1,
    1 + 2 + 2,
    1 + 2 + 3,
    1 + 3 + 1,
    1 + 3 + 2,
    1 + 3 + 3,
    2 + 1 + 1,
    2 + 1 + 2,
    2 + 1 + 3,
    2 + 2 + 1,
    2 + 2 + 2,
    2 + 2 + 3,
    2 + 3 + 1,
    2 + 3 + 2,
    2 + 3 + 3,
    3 + 1 + 1,
    3 + 1 + 2,
    3 + 1 + 3,
    3 + 2 + 1,
    3 + 2 + 2,
    3 + 2 + 3,
    3 + 3 + 1,
    3 + 3 + 2,
    3 + 3 + 3
};

var groups = possibilities.GroupBy(p => p).Select(g => (g.Key, g.Count())).ToList();

Console.WriteLine();

// recursively
// - check the next set of dice rolls (see groups above)
// - figure out the number of ways to get to 21 for each respective dice roll recursively
// - multiply the ways together on the way back up.

// calculate the full tree of (waysTo21, turnsToGetThere) for both start positions
// work out who wins more etc, based on that.