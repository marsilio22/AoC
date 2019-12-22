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
            var input = File.ReadAllLines("./input.txt");
            var testInput = File.ReadAllLines("./inputA.txt");

            long deckSize = 10;
            long shuffles = 10;
            for(int j = 1; j <= shuffles; j ++)
            {
                Console.WriteLine($"{j} shuffles");
                Console.Write("Slow  :");
                for(long i = 0; i < deckSize; i ++){
                    var test1 = SlowShuffle((int)i, deckSize, j, testInput);
                    Console.Write(test1 + ",");
                }
                Console.WriteLine();

                // the cards themselves can be used as indices for the array. e.g. the last card in a 10007 list is 2517, the next
                // one will be the card in position 2517 after the first shuffle (454) and so on.
                // Console.WriteLine(deck.ToList()[2517]);
                Console.Write("Fast  :");
                for(int i = 0; i < deckSize; i ++){
                    var test1 = FastShuffle(i, deckSize, j, testInput);
                    Console.Write(test1 + ",");
                }
                Console.WriteLine();

                Console.WriteLine();
            }


            var answer = FastShuffle(2020, 119315717514047, 101741582076661, input);
            //       2406044462912 is too low
            //      65304365993455 is too high
            // 9223372036854775808 long overflow
        }

        public static long SlowShuffle(int position, long cards, long shuffles, ICollection<string> input){
            ICollection<int> deck = Enumerable.Range(0, (int)cards).ToList();
            for (long i = 0; i < shuffles; i ++)
            {
                foreach(var line in input){
                    if (line.Contains("stack")){
                        deck = DealIntoNewStack(deck);
                    }
                    else if (line.Contains("cut")){
                        var N = int.Parse(line.Split(' ').Last());

                        deck = CutN(deck, N);
                    }
                    else if (line.Contains("increment")){
                        var N = int.Parse(line.Split(' ').Last());

                        deck = DealWithIncrementN(deck, N);
                    }
                }
            }

            return deck.ToList()[position];
        }

        public static (long x, long y, long gcd) euclid(long a, long b)
        {
            long x0 = 1, xn = 1, y0 = 0, yn = 0, x1 = 0, y1 = 1, f, r = a % b;
 
            while (r > 0)
            {
                f = a / b;
                xn = x0 - f * x1;
                yn = y0 - f * y1;
 
                x0 = x1;
                y0 = y1;
                x1 = xn;
                y1 = yn;
                a = b;
                b = r;
                r = a % b;
            }
 
            return (xn, yn, b);
        }

        public static long FastShuffle(int position, long cards, long shuffles, ICollection<string> input){
            var values = new HashSet<long>();
            List<long> indexableValues = new List<long>();

            long answer = position;
            while(true){
                foreach(var line in input){
                    if (line.Contains("stack")){
                        answer = (cards - 1 - answer) % cards;
                    // Console.WriteLine($"{cards}, {answer}");
                    }
                    else if (line.Contains("cut")){
                        var N = long.Parse(line.Split(' ').Last());
                        answer = (answer + N) % cards;
                    }
                    else if (line.Contains("increment")){
                        var N = long.Parse(line.Split(' ').Last());
                        var euclidAns = euclid(N, cards);
                        answer = (answer * euclidAns.x) % cards;

                    }
                    if (answer < 0){
                        answer += cards;
                    }
                }
                if (Math.Abs(values.Count() - 630717) < 10){
                    Console.WriteLine($"shuffle: {values.Count()} = {answer}");
                }

                if (!values.Add(answer)){
                    break;
                }
                indexableValues.Add(answer);
            }

            // Now we have done some shuffles and reached a cyclical value
            // Need to figure out which element of the cycle we care about.

            var cycleLength = values.Count();
            
            var indexOfLastThing = (long)((((double)shuffles/(double)cycleLength) % 1) * cycleLength);

            answer = indexableValues[(int)indexOfLastThing];

            return answer;
        }

        // extended Euclidean Algorithm 
        public static (int gcd, int x1, int y1) gcdExtended(int a, int b,  
                                    int x, int y) 
        { 
            // Base Case 
            if (a == 0) 
            { 
                x = 0; 
                y = 1; 
                return (b, x, y); 
            } 
    
            // To store results of 
            // recursive call 
            int x1 = 1, y1 = 1;  
            int gcd = gcdExtended(b % a, a, x1, y1).gcd; 
    
            // Update x and y using  
            // results of recursive call 
            x = y1 - (b / a) * x1; 
            y = x1; 
    
            return (gcd, x1, y1); 
        } 


        public static long HowManyCardsHaveBeenDealtBeforeThisOne(long n, long length, int k){
            long value = 0;
            
            for(int j = 0; j < k; j++)
            {
                value += 1 + (length - j - 1)/n;
            }
            
            return value;
            
        }

        public static long CardsDealtInRound(long n, long length, int k){
            return 1 + (length - k - 1)/n;
        }
        

        public static ICollection<int> DealIntoNewStack(ICollection<int> deck){
            return deck.Reverse().ToList();
        }

        public static ICollection<int> CutN(ICollection<int> deck, int N){
            List<int> cutSectionA = new List<int>(); // 
            List<int> cutSectionB = new List<int>(); // 
            if (N > 0){
                cutSectionA = deck.ToList().GetRange(0, N);
                cutSectionB = deck.ToList().GetRange(N, deck.Count - N);
            }
            else if (N < 0){
                cutSectionA = deck.ToList().GetRange(0, deck.Count + N);
                cutSectionB = deck.ToList().GetRange(deck.Count + N, Math.Abs(N));
            }

            return cutSectionB.Concat(cutSectionA).ToList();
        }

        public static ICollection<int> DealWithIncrementN(ICollection<int> deck, int N){
            var newDeck = (from x in Enumerable.Range(0, deck.Count) select -1).ToList();

            var dealIndex = 0;
            while (deck.Any()){
                var top = deck.First();

                newDeck[dealIndex] = top;
                deck.Remove(top);
                dealIndex = (dealIndex + N) % newDeck.Count;
            }

            if (newDeck.Any(n => n == -1)){
                throw new Exception();
            }

            return newDeck;
        }
    }
}
