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
            var depth = 0;

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

                    coord.CharactersFromHere = curr.CharactersFromHere;
                    coord.Index = index;
                    coord.Depth = depth;

                    map.AddCoordinate(coord);

                    curr = coord;
                    character = curr.CharactersFromHere.Dequeue();
                    index ++;
                }

                if (character == '(')
                {
                    openBracketLocations.Push(curr);
                    character = curr.CharactersFromHere.Dequeue();
                    index++;
                    depth++;
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
                    if (openBracketLocations.Count != 0)
                    {
                        openBracketLocations.Pop();
                        depth--;
                    }
                    else
                    {
                        var otherCurr = curr;

                        if (openBracketLocations.TryPeek(out curr))
                        {
                            character = curr.CharactersFromHere.Dequeue();
                            index++;
                        }
                        else
                        {
                            character = otherCurr.CharactersFromHere.Dequeue();
                            curr = otherCurr;
                            index++;
                        }

                        depth--;
                    }
                }
                
                if (character == '$')
                {
                    currentStartingLocations = new Queue<Coordinate>(currentStartingLocations.ToList().Distinct());
                    if(currentStartingLocations.TryDequeue(out curr))
                    {
                        curr.CharactersFromHere = new Queue<char>(line.ToList().GetRange(curr.Index, line.Count - curr.Index));
                        index = curr.Index;

                        while (true)
                        {
                            character = curr.CharactersFromHere.Dequeue();
                            index++;
                            int count = 0;
                            if (character == '(')
                            {
                                count++;
                            }

                            if (character == ')')
                            {
                                if (count == 0)
                                {
                                    curr.CharactersFromHere.Dequeue();
                                    index++;
                                    depth = curr.Depth;
                                    break;
                                }
                                else if (count != 0)
                                {
                                    count--;
                                }
                            }
                            
                        }

                        character = curr.CharactersFromHere.Dequeue();
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

    public class Coordinate : IEquatable<Coordinate>
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Queue<char> CharactersFromHere { get; set; }
        public int Index { get; set; }
        public List<Coordinate> Adjacents { get; set; } = new List<Coordinate>();
        public int Depth { get; set; }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
        public bool Equals(Coordinate other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return X == other.X && Y == other.Y;
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Coordinate) obj);
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }

        /// <summary>Returns a value that indicates whether the values of two <see cref="T:Day_20.Coordinate" /> objects are equal.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        public static bool operator ==(Coordinate left, Coordinate right)
        {
            return Equals(left, right);
        }

        /// <summary>Returns a value that indicates whether two <see cref="T:Day_20.Coordinate" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        public static bool operator !=(Coordinate left, Coordinate right)
        {
            return !Equals(left, right);
        }
    }
}