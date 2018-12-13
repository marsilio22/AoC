using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_13
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("./input.txt");

            Dictionary<(int x, int y), Coordinate> map = new Dictionary<(int x, int y), Coordinate>();
            List<Cart> carts = new List<Cart>();

            var cartletters = new [] {'v', '^', '<', '>'};
            for(int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                for(int j = 0; j < line.Length; j++){
                    char letter = line[j];
                    bool containsCart = false;

                    if (cartletters.Contains(letter))
                    {
                        containsCart = true;
                        carts.Add(new Cart{
                            X = j,
                            Y = i,
                            DirectionOfTravel = (DirectionOfTravel)letter,
                            NextTurnDirection = TurnDirection.Left
                        });

                        switch(letter){
                            case 'v':
                            case '^':
                                letter = '|';
                                break;
                            case '<':
                            case '>':
                                letter = '-';
                                break;
                            default:
                                throw new Exception("Oops");
                        }
                    }

                    map.Add((j, i), new Coordinate{
                        X = j,
                        Y = i,
                        Track = letter,
                        ContainsCart = containsCart
                    });
                }
            }

            var tick = 0;
            bool crash = false;
            while (!crash)
            {
                Console.WriteLine(tick);
                carts = carts.OrderBy(c => c.Y).ThenBy(c => c.X).ToList();
                foreach (var cart in carts)
                {
                    (int cartX, int cartY) = (cart.X, cart.Y);
                    (int x, int y) nextCoordinate;
                    switch (cart.DirectionOfTravel)
                    {
                        case DirectionOfTravel.North:
                            nextCoordinate = (cartX, cartY - 1);
                            break;
                        case DirectionOfTravel.South:
                            nextCoordinate = (cartX, cartY + 1);
                            break;
                        case DirectionOfTravel.East:
                            nextCoordinate = (cartX + 1, cartY);
                            break;
                        case DirectionOfTravel.West:
                            nextCoordinate = (cartX - 1, cartY);
                            break;
                        default:
                            throw new Exception("Oops");
                    }

                    if (map[nextCoordinate].ContainsCart){
                        crash = true;
                        Console.WriteLine($"{nextCoordinate.x},{nextCoordinate.y}");
                    }
                    else
                    {
                        map[(cartX, cartY)].ContainsCart = false;
                        map[nextCoordinate].ContainsCart = true;
                        cart.X = nextCoordinate.x;
                        cart.Y = nextCoordinate.y;
                        
                        switch(map[nextCoordinate].Track){
                            case '/':
                                switch (cart.DirectionOfTravel)
                                {
                                    case DirectionOfTravel.North:
                                        cart.DirectionOfTravel = DirectionOfTravel.East;
                                        break;
                                    case DirectionOfTravel.East:
                                        cart.DirectionOfTravel = DirectionOfTravel.North;
                                        break;
                                    case DirectionOfTravel.South:
                                        cart.DirectionOfTravel = DirectionOfTravel.West;
                                        break;
                                    case DirectionOfTravel.West:
                                        cart.DirectionOfTravel = DirectionOfTravel.South;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case '\\':
                                switch (cart.DirectionOfTravel)
                                {
                                    case DirectionOfTravel.North:
                                        cart.DirectionOfTravel = DirectionOfTravel.West;
                                        break;
                                    case DirectionOfTravel.East:
                                        cart.DirectionOfTravel = DirectionOfTravel.South;
                                        break;
                                    case DirectionOfTravel.South:
                                        cart.DirectionOfTravel = DirectionOfTravel.East;
                                        break;
                                    case DirectionOfTravel.West:
                                        cart.DirectionOfTravel = DirectionOfTravel.North;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case '+':
                                cart.DirectionOfTravel = GetDirection(cart.DirectionOfTravel, cart.NextTurnDirection);
                                switch(cart.NextTurnDirection){
                                    case TurnDirection.Left:
                                        cart.NextTurnDirection = TurnDirection.Straight;
                                        break;
                                    case TurnDirection.Right:
                                        cart.NextTurnDirection = TurnDirection.Left;
                                        break;
                                    case TurnDirection.Straight:
                                        cart.NextTurnDirection = TurnDirection.Right;
                                        break;
                                }
                                break;
                            case '|':
                            case '-':
                            default:
                                break;
                        }
                    }
                }

                tick++;
            }
        }

        public static DirectionOfTravel GetDirection(DirectionOfTravel current, TurnDirection turn){
            switch (current)
            {
                case DirectionOfTravel.North:
                    switch (turn)
                    {
                        case TurnDirection.Left:
                            return DirectionOfTravel.West;
                        case TurnDirection.Right:
                            return DirectionOfTravel.East;
                        case TurnDirection.Straight:
                            return DirectionOfTravel.North;
                    }
                    break;
                case DirectionOfTravel.South:
                    switch (turn)
                    {
                        case TurnDirection.Left:
                            return DirectionOfTravel.East;
                        case TurnDirection.Right:
                            return DirectionOfTravel.West;
                        case TurnDirection.Straight:
                            return DirectionOfTravel.South;
                    }
                    break;
                case DirectionOfTravel.East:
                    switch (turn)
                    {
                        case TurnDirection.Left:
                            return DirectionOfTravel.North;
                        case TurnDirection.Right:
                            return DirectionOfTravel.South;
                        case TurnDirection.Straight:
                            return DirectionOfTravel.East;
                    }
                    break;
                case DirectionOfTravel.West:
                    switch (turn)
                    {
                        case TurnDirection.Left:
                            return DirectionOfTravel.South;
                        case TurnDirection.Right:
                            return DirectionOfTravel.North;
                        case TurnDirection.Straight:
                            return DirectionOfTravel.West;
                    }
                    break;
                
            }
            throw new Exception ("Oops");
        }
    }

    public class Coordinate{
        public int X { get; set; }
        public int Y { get; set; }
        public char Track { get; set; }
        public bool ContainsCart { get; set; }
    }

    public class Cart{
        public int X { get; set; }
        public int Y { get; set; }
        public DirectionOfTravel DirectionOfTravel { get; set; } 

        public TurnDirection NextTurnDirection { get; set; }
    }

    public enum DirectionOfTravel
    {
        North = '^',
        South = 'v',
        East = '>',
        West = '<'
    }

    public enum TurnDirection
    {
        Left,
        Right,
        Straight
    }
}
