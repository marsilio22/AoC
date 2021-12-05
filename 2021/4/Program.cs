var lines = File.ReadAllLines("./input.txt").ToList();

var numbers = lines.First().Split(",").Select(k => int.Parse(k));

// get rid of the first line and the newline afterwards
lines.RemoveRange(0, 2);

ICollection<Board> boards = new List<Board>();

for(int i = 0; i < lines.Count; i+=6)
{
    boards.Add(new Board(lines.Take(new Range(i, i+5)).ToList()));
}

var answers = new List<int>();

// squiddy bingo time
foreach(var num in numbers)
{
    foreach(var board in boards)
    {
        board.CallNumber(num);
    }
    
    var newWinningBoards = boards.Where(b => b.HasWon());

    foreach(var board in newWinningBoards)
    {
        answers.Add(board.Sum() * num);
    }

    boards = boards.Where(b => !b.HasWon()).ToList();
}

Console.WriteLine(boards.Count());
Console.WriteLine(answers.First());
Console.WriteLine(answers.Last());

public class Board 
{
    public IDictionary<(int x, int y), (int value, bool called)> State {get; set;}

    public Board(ICollection<string> unparsedState)
    {
        State = new Dictionary<(int, int), (int, bool)>();

        var x = 0; 
        var y = 0;

        foreach(var line in unparsedState)
        {
            var splitLine = line.Split(" ", options: StringSplitOptions.RemoveEmptyEntries).Select(j => int.Parse(j)).ToList();

            foreach(var num in splitLine)
            {
                State[(x, y)] = (num, false);
                x++;
            }
            y++;
            x=0;
        }
    }

    public void CallNumber(int num)
    {
        // this is mega slow in debug because it obviously throws loads of exceptions
        // but cba comparing default KVP's today...
        try
        {
            var thing = State.First(v => v.Value.value == num);
            State[thing.Key] = (thing.Value.value, true);
        }
        catch (Exception){}
    }

    public bool HasWon()
    {
        // take advantage of the state being square
        var maxX = State.Max(s => s.Key.x);

        for(int i = 0; i <= maxX; i++)
        {
            if(State.Where(s => s.Key.x == i).All(s => s.Value.called)
                || State.Where(s => s.Key.y == i).All(s => s.Value.called))
            {
                return true;
            }
        }

        return false;
    }

    public int Sum()
    {
        return State.Values.Where(s => !s.called).Sum(s => s.value);
    }
}