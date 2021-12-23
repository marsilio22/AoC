using System.Data;

var col1 = new [] { 'D', 'D' };
var col2 = new [] { 'C', 'C' };
var col3 = new [] { 'A', 'B' };
var col4 = new [] { 'B', 'A' };

var state = new State{
    Room1State = col1.ToList(),
    Room2State = col2.ToList(),
    Room3State = col3.ToList(),
    Room4State = col4.ToList()
};

// want to get the A's to column 1 and the D's to column 4, etc.
// a column's FIRST entry is the thing closest to the top spaces

// valid first moves are col1[0] -> 0, 1, 3, 5, 7, 9, or 10
//                       col2[0] -> ...
//                       etc.

// cost of a move is 1 to get out of the column, 



class State
{
    public List<char> Room1State {get;set;} = new List<char>();
    public List<char> Room2State {get;set;} = new List<char>();
    public List<char> Room3State {get;set;} = new List<char>();
    public List<char> Room4State {get;set;} = new List<char>();

    public IDictionary<int, char> HallwayState {get;set;}

    public State()
    {
        this.HallwayState = new Dictionary<int, char>
        { 
            { 0, '.' }, // far left
            { 1, '.' },
            { 2, '.' }, // above Room1
            { 3, '.' },
            { 4, '.' }, // above Room2
            { 5, '.' },
            { 6, '.' }, // above Room3
            { 7, '.' },
            { 8, '.' }, // above Room4
            { 9, '.' },
            { 10, '.' } // far right
        };
    }

    private bool isUnblocked(int start, int end)
    {
        if (start > end)
        {
            var temp = start;
            start = end;
            end = temp;
        }

        for (int i = start+1; i < end; i++)
        {
            if (HallwayState[i] != '.')
            {
                return false;
            }
        }

        return true;
    }

    public ICollection<(State, int)> ValidNextStatesAndCosts()
    {
        var result = new List<(State, int)>();

        // first check whether anything can go home now from the hallway, because that's going to cost the same no matter what
        var hallwayCharacters = HallwayState.Where(c => !c.Equals('.'));
        foreach(var hallChar in hallwayCharacters)
        {
            int cost;
            bool canMove;

            switch(hallChar.Value)
            {
                case 'A':
                    // check whether Room1 contains no or only A's, and whether this A is blocked from moving to Room1
                    canMove = Room1State.All(r => r.Equals('A') || r.Equals('.')) && isUnblocked(hallChar.Key, 2);
                    
                    if (!canMove)
                    {
                    
                        continue;
                    }

                    // work out cost = abs(hallchar.key - 2) + (Room1.IsEmpty ? 2 : 1) ((this might be off by one))
                    cost = Math.Abs(hallChar.Key - 2) + (Room1State.All(c => c.Equals('.')) ? 2 : 1);

                    var newHallwayState = this.HallwayState.ToDictionary(k => k.Key, v => v.Value);
                    newHallwayState[hallChar.Key] = '.';

                    List<char> newRoom1State;

                    if (Room1State.Contains('A'))
                    {
                        newRoom1State = new List<char>{'A', 'A'};
                    }
                    else
                    {
                        newRoom1State = new List<char>{'.', 'A'};
                    }

                    var state = new State{
                        // moving to room1
                        HallwayState = newHallwayState,
                        Room1State = newRoom1State,
                        Room2State = this.Room2State,
                        Room3State = this.Room3State,
                        Room4State = this.Room4State
                    };

                    result.Add((state, cost));
                    break;
                case 'B':
                    canMove = Room2State.All(r => r.Equals('B') || r.Equals('.')) && isUnblocked(hallChar.Key, 4);
                    
                    if (!canMove)
                    {
                        continue;
                    }

                    cost = (Math.Abs(hallChar.Key - 4)+ (Room2State.All(c => c.Equals('.')) ? 2 : 1)) * 10;

                    newHallwayState = this.HallwayState.ToDictionary(k => k.Key, v => v.Value);
                    newHallwayState[hallChar.Key] = '.';

                    List<char> newRoom2State;

                    if (Room2State.Contains('B'))
                    {
                        newRoom2State = new List<char>{'B', 'B'};
                    }
                    else
                    {
                        newRoom2State = new List<char>{'.', 'B'};
                    }

                    state = new State{
                        // moving to room2
                        HallwayState = newHallwayState,
                        Room1State = this.Room1State,
                        Room2State = newRoom2State,
                        Room3State = this.Room3State,
                        Room4State = this.Room4State
                    };

                    result.Add((state, cost));
                    break;
                case 'C':
                    canMove = Room3State.All(r => r.Equals('C') || r.Equals('.')) && isUnblocked(hallChar.Key, 6);
                    
                    if (!canMove)
                    {
                        continue;
                    }

                    cost = (Math.Abs(hallChar.Key - 6) + (Room3State.All(c => c.Equals('.')) ? 2 : 1)) * 100;
                    
                    newHallwayState = this.HallwayState.ToDictionary(k => k.Key, v => v.Value);
                    newHallwayState[hallChar.Key] = '.';

                    List<char> newRoom3State;

                    if (Room3State.Contains('C'))
                    {
                        newRoom3State = new List<char>{'C', 'C'};
                    }
                    else
                    {
                        newRoom3State = new List<char>{'.', 'C'};
                    }

                    state = new State{
                        // moving to room1
                        HallwayState = newHallwayState,
                        Room1State = this.Room1State,
                        Room2State = this.Room2State,
                        Room3State = newRoom3State,
                        Room4State = this.Room4State
                    };

                    result.Add((state, cost));
                    break;
                case 'D':
                    canMove = Room4State.All(r => r.Equals('D') || r.Equals('.')) && isUnblocked(hallChar.Key, 8);
                    
                    if (!canMove)
                    {
                        continue;
                    }

                    cost = (Math.Abs(hallChar.Key - 8) + (Room4State.All(c => c.Equals('.')) ? 2 : 1)) * 1000;
                    
                    newHallwayState = this.HallwayState.ToDictionary(k => k.Key, v => v.Value);
                    newHallwayState[hallChar.Key] = '.';

                    List<char> newRoom4State;

                    if (Room1State.Contains('D'))
                    {
                        newRoom4State = new List<char>{'D', 'D'};
                    }
                    else
                    {
                        newRoom4State = new List<char>{'.', 'D'};
                    }

                    state = new State{
                        // moving to room1
                        HallwayState = newHallwayState,
                        Room1State = this.Room1State,
                        Room2State = this.Room2State,
                        Room3State = this.Room3State,
                        Room4State = newRoom4State
                    };

                    result.Add((state, cost));
                    break;
            }
        }

        // now for each top character in a room, consider moving it into the hallway

        // Room1
        char top = Room1State.FirstOrDefault(r => !r.Equals('.'));
        if (top is not default(char))
        {
            switch(top)
            {
                case 'A':
                    // can move from here to any of 0, 1, 3, 5, 7, 9, 10 that aren't blocked
                    break;
                case 'B':
                    break;
                case 'C':
                    break;
                case 'D':
                    break;
            }
        }

        // Room2

        // Room3

        // Room4
    }
}