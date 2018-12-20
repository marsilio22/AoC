using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_20
{
    class Program
    {
        static void Main(string[] args)
        {
            var line = new Queue<char>(File.ReadAllLines("./input.txt")[0]);

            Queue<Coordinate> currentStartingLocations = new Queue<Coordinate>();
            Map map = new Map();
            var start = new Coordinate
            {
                X = 0,
                Y = 0, 
                CharactersFromHere = new Queue<char>(line.ToList())
            };

            map.AddCoordinate(start);

            currentStartingLocations.Enqueue(start);
            Stack<Coordinate> openBracketLocations = new Stack<Coordinate>();

            // Logically:
            //      '(' means "remember this coordinate as a 'start', because we want to start from here again in a minute"
            //      '|' means "remember this coordinate as an 'end',  jump back to the last 'start' and start again"
            //      ')' means "start again from all the last remembered 'end's

            // while (currentStartingLocations.TryDequeue(out var curr))
            var curr = currentStartingLocations.Dequeue();
            var character  = curr.CharactersFromHere.Dequeue();
            var index = 0;
            
            while (character != '$')
            {
                while(character != '(' && character != '|' && character != ')' && character != '$')
                {
                    var coord = new Coordinate();
                    switch (character)
                    {
                        case 'N':
                            coord.X = curr.X;
                            coord.Y = curr.Y + 1;
                            break;
                        case 'S':
                            coord.X = curr.X;
                            coord.Y = curr.Y - 1;
                            break;
                        case 'E':
                            coord.X = curr.X + 1;
                            coord.Y = curr.Y;
                            break;
                        case 'W':
                            coord.X = curr.X - 1;
                            coord.Y = curr.Y;
                            break;
                        case '^':
                            character = curr.CharactersFromHere.Dequeue();
                            index ++;
                            continue;
                    }

                    coord.Adjacents.Add(curr);
                    curr.Adjacents.Add(coord);

                    coord.Index = index;

                    map.AddCoordinate(coord);

                    curr = coord;
                    character = curr.CharactersFromHere.Dequeue();
                    index ++;
                }

                if (character == '(')
                {
                    openBracketLocations.Push(curr);
                    character = curr.CharactersFromHere.Dequeue();
                    index ++;
                    continue;
                }
                else if (character == '|')
                {
                    currentStartingLocations.Enqueue(curr);
                    curr = openBracketLocations.Peek();
                    character = curr.CharactersFromHere.Dequeue();
                    index ++;
                    continue;
                }
                else if (character == ')')
                {
                    openBracketLocations.Pop();
                    var otherCurr = curr;
                    if(openBracketLocations.TryPeek(out curr)){
                        character = curr.CharactersFromHere.Dequeue();
                        index ++;
                    }
                    else
                    {
                        character = otherCurr.CharactersFromHere.Dequeue();
                        index ++;
                    }
                }
                
                if (character == '$')
                {
                    if(currentStartingLocations.TryDequeue(out curr))
                    {
                        character = curr.CharactersFromHere.Dequeue();
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }

    public class Map{
        Dictionary<(int x, int y), Coordinate> map = new Dictionary<(int x, int y), Coordinate>();

        public void AddCoordinate(Coordinate coordinate){
            map[(coordinate.X, coordinate.Y)] = coordinate;
        }

        public void FindShortestRoute((int x, int y) start, (int x, int y) end){

        }
    }

    public class Coordinate
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Queue<char> CharactersFromHere { get; set; }
        public int Index { get; set; }
        public List<Coordinate> Adjacents { get; set; } = new List<Coordinate>();
    }
}
