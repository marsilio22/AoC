using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_19
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("./input.txt");
            var map = new Dictionary<(int x, int y), char>();

            string answer = string.Empty;
            int distance = 0;
            var lineCharacters = new [] { '-', '|', '+' };

            int x = 0, y = 0;
            foreach(var line in lines)
            {
                foreach(char c in line)
                {
                    if (c != ' '){
                        map.Add((x, y), c);
                    }
                    x++;
                }
                y++;
                x = 0;
            }

            var currentPosition = map.Single(c => c.Key.y == 0).Key;
            var facing = Facings.S;

            while (true)
            {
                List<KeyValuePair<(int x, int y), char>> line = null;
                switch(facing)
                {
                    case Facings.N:
                        line = map.Where(kvp => kvp.Key.x == currentPosition.x && kvp.Key.y < currentPosition.y).ToList();
                        break;
                    case Facings.S:
                        line = map.Where(kvp => kvp.Key.x == currentPosition.x && kvp.Key.y > currentPosition.y).ToList();
                        break;
                    case Facings.E:
                        line = map.Where(kvp => kvp.Key.x > currentPosition.x && kvp.Key.y == currentPosition.y).ToList();
                        break;
                    case Facings.W:
                        line = map.Where(kvp => kvp.Key.x < currentPosition.x && kvp.Key.y == currentPosition.y).ToList();
                        break;
                    default:
                        throw new Exception("oh noes");
                }

                var adjacentsToMe = new List<KeyValuePair<(int x, int y), char>>();

                for (var i = 1; i <= line.Count; i++)
                {
                    var next = line.SingleOrDefault(l => Math.Abs(l.Key.x - currentPosition.x) + Math.Abs(l.Key.y - currentPosition.y) == i);

                    if (next.Key != (0, 0))
                    {
                        adjacentsToMe.Add(next);
                        if (!lineCharacters.Contains(next.Value))
                        {
                            answer += next.Value;
                        }
                        distance++;
                    }
                    else
                    {
                        break;
                    }
                }

                currentPosition = adjacentsToMe.Last().Key;
                // work out new facing
                var newNextTravelCoord = map.SingleOrDefault(m => Math.Abs(currentPosition.x - m.Key.x) + Math.Abs(currentPosition.y - m.Key.y) == 1 && !adjacentsToMe.Select(a => a.Key).Contains(m.Key));
                if (newNextTravelCoord.Key == (0, 0))
                    break;
                else if (newNextTravelCoord.Key.x > currentPosition.x)
                    facing = Facings.E;
                else if (newNextTravelCoord.Key.x < currentPosition.x)
                    facing = Facings.W;
                else if (newNextTravelCoord.Key.y > currentPosition.y)
                    facing = Facings.S;
                else
                    facing = Facings.N;
            }

            Console.WriteLine(answer);
            Console.WriteLine(distance);
        }

        public enum Facings 
        {
            N,
            E,
            S,
            W
        }
    }
}
