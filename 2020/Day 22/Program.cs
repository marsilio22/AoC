using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_22
{
    class Program
    {
        static void Main(string[] args)
        {
            (Queue<int> p1Deck, Queue<int> p2Deck) decks = Parse();

            var p1Deck= decks.p1Deck;
            var p2Deck= decks.p2Deck;
            
            while (p1Deck.Any() && p2Deck.Any())
            {
                Console.WriteLine(p1Deck.GetHashCode());
                Console.WriteLine(p2Deck.GetHashCode());

                var p1Card = p1Deck.Dequeue();
                var p2Card = p2Deck.Dequeue();

                if (p1Card > p2Card)
                {
                    p1Deck.Enqueue(p1Card);
                    p1Deck.Enqueue(p2Card);
                }
                else
                {
                    p2Deck.Enqueue(p2Card);
                    p2Deck.Enqueue(p1Card);
                }
            }

            var score = 0;

            while(p1Deck.Any())
            {
                score += p1Deck.Count * p1Deck.Dequeue();
            }

            while(p2Deck.Any())
            {
                score += p2Deck.Count * p2Deck.Dequeue();
            }

            Console.WriteLine(score);

            // reset for recursive version.
            decks = Parse();

            p1Deck= decks.p1Deck;
            p2Deck= decks.p2Deck;

            Console.WriteLine($"Player {RecursiveRound(p1Deck, p2Deck)} wins");
        }

        public static int RecursiveRound(Queue<int> p1Deck, Queue<int> p2Deck, int depth = 0)
        {
            var previousRounds = new List<(long p1Score, long p2Score)>();

            while (p1Deck.Any() && p2Deck.Any())
            {
                var currentScores = (GetScoreForDeck(new Queue<int>(p1Deck.ToList())),GetScoreForDeck(new Queue<int>(p2Deck.ToList())));
                if (previousRounds.Contains(currentScores))
                {
                    // player 1 wins immediately
                    Console.WriteLine($"Player 1 wins because of same deck order at depth {depth}");
                    return 1;
                }

                previousRounds.Add(currentScores);

                var p1Card = p1Deck.Dequeue();
                var p2Card = p2Deck.Dequeue();
                var roundWinner = 0;

                if (p1Deck.Count() >= p1Card && p2Deck.Count >= p2Card)
                {
                    var p1NewDeck = new Queue<int>(p1Deck.ToList().Take(p1Card));
                    var p2NewDeck = new Queue<int>(p2Deck.ToList().Take(p2Card));
                    roundWinner = RecursiveRound(p1NewDeck, p2NewDeck, depth+1);
                }
                else
                {
                    roundWinner = p1Card > p2Card? 1 : 2;
                }

                if (roundWinner == 1)
                {
                    p1Deck.Enqueue(p1Card);
                    p1Deck.Enqueue(p2Card);
                }
                else
                {
                    p2Deck.Enqueue(p2Card);
                    p2Deck.Enqueue(p1Card);
                }
            }

            if (p1Deck.Any())
            {
                Console.WriteLine($"Player 1 wins the round at depth {depth} with score: {GetScoreForDeck(p1Deck)}");
                return 1;
            }
            else
            {
                Console.WriteLine($"Player 2 wins the round at depth {depth} with score: {GetScoreForDeck(p2Deck)}");
                return 2;
            }
        }

        public static long GetScoreForDeck(Queue<int> deck)
        {
            int score = 0;
            while(deck.Any())
            {
                score += deck.Count * deck.Dequeue();
            }

            return score;
        }

        public static (Queue<int>, Queue<int>) Parse()
        {
            var lines = File.ReadLines("./input.txt");
            int player = 0;

            var p1Deck = new Queue<int>();
            var p2Deck = new Queue<int>();

            foreach(var line in lines)
            {
                if (line.Contains("Player "))
                {
                    player = int.Parse(line.Split(':')[0].Split("Player ")[1]);
                }
                else
                {
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    if (player == 1)
                    {
                        p1Deck.Enqueue (int.Parse(line));
                    }
                    else if (player == 2)
                    {
                        p2Deck.Enqueue (int.Parse(line));
                    }
                }
            }
            
            return (p1Deck, p2Deck);
        }
    }
}
