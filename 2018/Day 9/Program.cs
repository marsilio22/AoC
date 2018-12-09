using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_9
{
    class Program
    {
        static void Main(string[] args)
        {
            var line = File.ReadAllLines("./input.txt")[0];
            
            (long players, long lastMarble) = (long.Parse(line.Split(' ')[0]), long.Parse(line.Split(' ')[6]));

            // Part1
            CalculateWinningScore(players, lastMarble);

            // Part2
            CalculateWinningScore(players, lastMarble * 100);
        }

        public static void CalculateWinningScore(long players, long lastMarble){

            Dictionary<long, long> scores = new Dictionary<long, long>(); // indexed by player number.
            List<long> circle = new List<long>{0}; // Marble circle
            int currentMarbleIndex = 0;

            for(long i = 1; i <= lastMarble; i++)
            {
                if (i % 23 != 0)
                {
                    currentMarbleIndex = (currentMarbleIndex + 2) % circle.Count;
                    circle.Insert(currentMarbleIndex + 1, i);
                }
                else if (i % 23 == 0)
                {
                    long playerWhoScores = i % players;
                    var marble7Back = currentMarbleIndex - 6 % circle.Count;
                    marble7Back = marble7Back < 0 ? marble7Back + circle.Count : marble7Back;
                    if(!scores.ContainsKey(playerWhoScores))
                    {
                        scores[playerWhoScores] = 0;
                    }

                    scores[playerWhoScores] += i + circle[marble7Back];

                    circle.RemoveAt(marble7Back);
                    currentMarbleIndex = marble7Back - 1 % circle.Count;
                }
            }

            Console.WriteLine($"{players} players; last marble is worth {lastMarble}: high score is {scores.Values.Max()}");
        }
    }
}
