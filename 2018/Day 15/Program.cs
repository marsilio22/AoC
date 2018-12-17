// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;

// namespace Day_15
// {
//     class Program
//     {
//         static void Main(string[] args)
//         {
//             var lines = File.ReadAllLines("./input.txt");

//             var map = new List<Coordinate>() ;
//             var characters = new List<Character>();

//             for (int i = 0; i < lines.Length; i++)
//             {
//                 var line = lines[i];
//                 for (int j = 0; j < line.Length; j++)
//                 {
//                     var symbol = line[j];
//                     var containsCharacter = false;
//                     Character character = null;
//                     if (symbol == 'G' || symbol == 'E')
//                     {
//                         character = new Character{
//                             HP = 200,
//                             Race = symbol,
//                             AttackPower = 3
//                         };
//                         characters.Add(character);

//                         symbol = '.';
//                         containsCharacter = true;
//                     }

//                     var coord = new Coordinate{
//                         X = j,
//                         Y = i,
//                         Symbol = symbol,
//                         ContainsCharacter = containsCharacter
//                     };
//                     map.Add(coord);

//                     if (character != null)
//                     {
//                         character.Coordinate = coord;
//                     }
//                 }
//             }

//             map = map.Where(m => m.Symbol != '#').ToList();

//             foreach(var coord in map)
//             {
//                 coord.AdjacentCoordinates = 
//                     map.Where(m => Math.Abs(m.X - coord.X) == 1 && Math.Abs(m.Y - coord.Y) == 0 ||
//                                    Math.Abs(m.X - coord.X) == 0 && Math.Abs(m.Y - coord.Y) == 1).ToList();    
//             }


//             var actualMap = new Map(map);
//             bool finished = false;
//             var combatRound = 0;

//             while (!finished)
//             {
//                 combatRound++;

//                 // On a combatant's turn:
//                 // Identify all possible targets (if none then end)
//                 // Identify all open squares adjacent to targets
//                 //     If unit is not in range of a target, and there are no available targets, then it ends its turn
//                 //     If a unit is in range of a target it attacks
//                 //     Otherwise it moves 1 step along the shortest path to that target
//                 // Notes
//                 // Units move in reading order, and one at a time, not all at once

//                 characters = characters.OrderBy(c => c.Coordinate.Y).ThenBy(c => c.Coordinate.X).ToList();

//                 foreach (var character in characters)
//                 {
//                     var characterTargets = characters.Where(c => c.Race != character.Race && !c.IsDead).ToList();

//                     if (!characterTargets.Any()){
//                         // end the loop;
//                         finished = true;
//                         break;
//                     }

//                     var adjCoords = characterTargets.SelectMany(ct => ct.Coordinate.AdjacentCoordinates.Where(c => !c.ContainsCharacter)).Distinct().ToList();

//                     if (!adjCoords.Any()){
//                         // end turn
//                         continue;
//                     }
//                     else if (adjCoords.Contains(character.Coordinate)){
//                         // attack
//                         var target = 
//                             characters
//                                 .Where(c => character.Coordinate.AdjacentCoordinates.Contains(c.Coordinate) && !c.IsDead)
//                                 .OrderBy(c => c.Coordinate.Y)
//                                 .ThenBy(c => c.Coordinate.X)
//                                 .First();

//                         character.Attack(target);
//                     }
//                     else 
//                     {
//                         List<Coordinate> shortestPath = null;
//                         // order them in increasing manhattan distance, 
//                         // then only bother working out the shortest path if their manhattan distance 
//                         // is less than the current shortest path
//                         adjCoords = adjCoords.OrderBy(c => Math.Abs(c.X - character.Coordinate.X) + Math.Abs(c.Y - character.Coordinate.Y)).ToList();
//                         var maxDistance = 47; // by eye this is the greatest distance between any two combatants...
//                         foreach(var adjCoord in adjCoords)
//                         {
//                             if (shortestPath == null || Math.Abs(adjCoord.X - character.Coordinate.X) + Math.Abs(adjCoord.Y - character.Coordinate.Y) < shortestPath.Count)
//                             {   
//                                 var next = actualMap.ShortestPathBetween(character.Coordinate, adjCoord, maxDistance);
//                                 if (next != null && shortestPath == null){
//                                     shortestPath = next;
//                                 }
//                                 else if (next != null && shortestPath != null)
//                                 {
//                                     shortestPath = next.Count < shortestPath.Count ? next : shortestPath;
//                                 }
//                                 else if (next != null && shortestPath != null && next.Count == shortestPath.Count)
//                                 {
//                                     shortestPath = new List<List<Coordinate>>{shortestPath, next}.OrderBy(p => p[1].Y).ThenBy(p => p[1].X).First();
//                                 }
//                             }

//                             if (shortestPath != null){
//                                 maxDistance = shortestPath.Count;
//                             }
//                         }

//                         if (shortestPath == null)
//                         {
//                             // end turn, can't reach anywhere useful
//                             continue;
//                         }

//                         // move 1 step along the path. Note the first entry of the path is the start coord.
//                         shortestPath[0].ContainsCharacter = false;
//                         character.Coordinate = shortestPath[1];
//                         shortestPath[1].ContainsCharacter = true;

//                         var targets = 
//                             characters
//                                 .Where(c => character.Coordinate.AdjacentCoordinates.Contains(c.Coordinate) && !c.IsDead);
//                         var target = targets.Any() ? targets
//                                 .OrderBy(c => c.Coordinate.Y)
//                                 .ThenBy(c => c.Coordinate.X)
//                                 .First() : null;

//                         if (target != null){
//                             character.Attack(target);
//                             if(target.IsDead){
//                                 target.Coordinate.ContainsCharacter = false;
//                             }
//                         }
//                     }
//                 }
//             }
        
//         Console.WriteLine(combatRound);
//         Console.WriteLine(characters.Where(c => !c.IsDead).Sum(c => c.HP));
//         }
//     }

//     public class Map
//     {
//         private List<Coordinate> map;

//         public Map(List<Coordinate> map){
//             this.map = map;
//         }

//         public List<Coordinate> ShortestPathBetween(Coordinate start, Coordinate end, int maxDistance)
//         {
//             var step = 0;
//             var ans = this.ShortestPathBetween(start, end, ref step, maxDistance);
//             this.ResetMap();
//             return ans;
//         }

//         private void ResetMap()
//         {
//             foreach(var coord in this.map)
//             {
//                 coord.Visited = false;
//                 coord.CurrentMinimumDistanceToThisCoordinate = 0;
//             }
//         }

//             // Code to draw the map (sort of)
//         public void DrawMap(){    
//             for (int i = 0; i < 32; i ++){
//                 for (int j = 0; j < 32; j ++){
//                     var coord = map.SingleOrDefault(m => m.X == j && m.Y == i);
//                     if (coord != null)
//                     {
//                         if (coord.ContainsCharacter){
//                             Console.Write('C');
//                         }
//                         else
//                         {
//                             Console.Write('.');
//                         }
//                     }
//                     else
//                     {
//                         Console.Write(' ');
//                     }
//                 }
//                 Console.WriteLine();
//             }
//         }
//         private List<Coordinate> ShortestPathBetween(Coordinate start, Coordinate end, ref int step, int maxDistance)
//         {
//             if (step >= maxDistance){
//                 // we've travelled maxDistance steps, which is far enough to have crossed the whole map. 
//                 // If we haven't found our target yet then we must be travelling away from them...
//                 return null; 
//             }

//             if (start.X == end.X && start.Y == end.Y)
//             {
//                 return new List<Coordinate>{end};
//             }

//             var shortestPaths = new List<List<Coordinate>>();
//             var step2 = step; // why the fuck is this necessary... fuck refs...

//             var validAdjacentCoords = end.AdjacentCoordinates.Where(c =>
//                 (!c.ContainsCharacter || c.X == start.X && c.Y == start.Y) && (!c.Visited || c.CurrentMinimumDistanceToThisCoordinate >= step2)
//             ).ToList();

//             foreach (var adjacentCoord in validAdjacentCoords)
//             {
//                 adjacentCoord.Visited = true;
//                 var currentStep = step + 1;
//                 adjacentCoord.CurrentMinimumDistanceToThisCoordinate = currentStep;

//                 shortestPaths.Add(ShortestPathBetween(start, adjacentCoord, ref currentStep, maxDistance));
//             }
            
//             if (!shortestPaths.Any(p => p != null))
//             {
//                 return null;
//             }

//             var minLength = shortestPaths.Where(p => p != null).Min(l => l.Count);
//             shortestPaths = shortestPaths.Where(p => p != null).OrderBy(p => p.First().Y).ThenBy(p => p.First().X).ToList(); // reading order

//             var shortest = shortestPaths.First(p => p.Count == minLength);
//             shortest.Add(end);

//             return shortest;
//         }
//     }

//     public class Coordinate
//     {
//         public int X { get; set; }
//         public int Y { get; set; }
//         public char Symbol { get; set; }
//         public bool ContainsCharacter { get; set; }
//         public List<Coordinate> AdjacentCoordinates {get; set;}
//         public bool Visited { get; set; }
//         public int CurrentMinimumDistanceToThisCoordinate { get; set; }
//     }

//     public class Character
//     {
//         public Coordinate Coordinate { get; set; }
//         public char Race {get; set;}
//         public int HP { get; set; }
//         public int AttackPower { get; set; }
//         public bool IsDead => HP <= 0;

//         public void Attack(Character other){
//             other.HP -= this.AttackPower;
//         }
//     }
// }


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Program
{
    public static void Main()
    {
        string[] map = File.ReadAllLines("./input.txt");

        Console.WriteLine(new Game(map, 3).RunGame(false));

        for (int attackPower = 4; ; attackPower++)
        {
            int? outcome = new Game(map, attackPower).RunGame(true);
            if (outcome.HasValue)
            {
                Console.WriteLine(outcome);
                break;
            }
        }
    }
}

public class Game
{
    private readonly string[] _map;
    private List<Unit> _units = new List<Unit>();
    public Game(string[] initialMap, int elfAttackPower)
    {
        for (int y = 0; y < initialMap.Length; y++)
        {
            for (int x = 0; x < initialMap[y].Length; x++)
            {
                if (initialMap[y][x] == 'G')
                    _units.Add(new Unit { X = x, Y = y, IsGoblin = true, Health = 200, AttackPower = 3 });
                else if (initialMap[y][x] == 'E')
                    _units.Add(new Unit { X = x, Y = y, IsGoblin = false, Health = 200, AttackPower = elfAttackPower });
            }
        }

        _map = initialMap.Select(l => l.Replace('G', '.').Replace('E', '.')).ToArray();
    }

    // Returns outcome of game.
    public int? RunGame(bool failOnElfDeath)
    {
        for (int rounds = 0; ; rounds++)
        {
            _units = _units.OrderBy(u => u.Y).ThenBy(u => u.X).ToList();
            for (int i = 0; i < _units.Count; i++)
            {
                Unit u = _units[i];
                List<Unit> targets = _units.Where(t => t.IsGoblin != u.IsGoblin).ToList();
                if (targets.Count == 0)
                    return rounds * _units.Sum(ru => ru.Health);

                if (!targets.Any(t => IsAdjacent(u, t)))
                    TryMove(u, targets);

                Unit bestAdjacent =
                    targets
                    .Where(t => IsAdjacent(u, t))
                    .OrderBy(t => t.Health)
                    .ThenBy(t => t.Y)
                    .ThenBy(t => t.X)
                    .FirstOrDefault();

                if (bestAdjacent == null)
                    continue;

                bestAdjacent.Health -= u.AttackPower;
                if (bestAdjacent.Health > 0)
                    continue;

                if (failOnElfDeath && !bestAdjacent.IsGoblin)
                    return null;

                int index = _units.IndexOf(bestAdjacent);
                _units.RemoveAt(index);
                if (index < i)
                    i--;
            }
        }
    }

    // Important: ordered in reading order
    private static readonly (int dx, int dy)[] s_neis = { (0, -1), (-1, 0), (1, 0), (0, 1) };
    private void TryMove(Unit u, List<Unit> targets)
    {
        HashSet<(int x, int y)> inRange = new HashSet<(int x, int y)>();
        foreach (Unit target in targets)
        {
            foreach ((int dx, int dy) in s_neis)
            {
                (int nx, int ny) = (target.X + dx, target.Y + dy);
                if (IsOpen(nx, ny))
                    inRange.Add((nx, ny));
            }
        }

        Queue<(int x, int y)> queue = new Queue<(int x, int y)>();
        Dictionary<(int x, int y), (int px, int py)> prevs = new Dictionary<(int x, int y), (int px, int py)>();
        queue.Enqueue((u.X, u.Y));
        prevs.Add((u.X, u.Y), (-1, -1));
        while (queue.Count > 0)
        {
            (int x, int y) = queue.Dequeue();
            foreach ((int dx, int dy) in s_neis)
            {
                (int x, int y) nei = (x + dx, y + dy);
                if (prevs.ContainsKey(nei) || !IsOpen(nei.x, nei.y))
                    continue;

                queue.Enqueue(nei);
                prevs.Add(nei, (x, y));
            }
        }

        List<(int x, int y)> getPath(int destX, int destY)
        {
            if (!prevs.ContainsKey((destX, destY)))
                return null;
            List<(int x, int y)> path = new List<(int x, int y)>();
            (int x, int y) = (destX, destY);
            while (x != u.X || y != u.Y)
            {
                path.Add((x, y));
                (x, y) = prevs[(x, y)];
            }

            path.Reverse();
            return path;
        }

        List<(int tx, int ty, List<(int x, int y)> path)> paths =
            inRange
            .Select(t => (t.x, t.y, path: getPath(t.x, t.y)))
            .Where(t => t.path != null)
            .OrderBy(t => t.path.Count)
            .ThenBy(t => t.Item2)
            .ThenBy(t => t.Item1)
            .ToList();

        List<(int x, int y)> bestPath = paths.FirstOrDefault().path;
        if (bestPath != null)
            (u.X, u.Y) = bestPath[0];
    }

    private bool IsOpen(int x, int y) => _map[y][x] == '.' && _units.All(u => u.X != x || u.Y != y);
    private bool IsAdjacent(Unit u1, Unit u2) => Math.Abs(u1.X - u2.X) + Math.Abs(u1.Y - u2.Y) == 1;

    private class Unit
    {
        public int X, Y;
        public bool IsGoblin;
        public int Health = 200;
        public int AttackPower;
    }
}
