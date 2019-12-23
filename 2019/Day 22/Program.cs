using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Day_22
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("./input.txt");
            var testInput = File.ReadAllLines("./inputA.txt");

            BigInteger deckSize = 101;
            BigInteger shuffles = 1;
            for(int j = 1; j <= shuffles; j ++)
            {
                Console.WriteLine($"{j} shuffles");
                Console.Write("Slow  :");
                for(BigInteger i = 0; i < deckSize; i ++){
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

            BigInteger cards = 119315717514047;
            shuffles = 101741582076661;

            // test....
            BigInteger x = 2020;
            BigInteger y = FastShuffle(x, cards, shuffles, input);
            BigInteger z = FastShuffle(y, cards, shuffles, input);

            BigInteger a = (y - z) * BigInteger.ModPow(x - y + cards, cards - 2, cards) % cards;
            BigInteger b = (y - a * x) % cards;

            var result = (BigInteger.ModPow(a, shuffles, cards) * x + (BigInteger.ModPow(a, shuffles, cards) - 1) * BigInteger.ModPow(a - 1, cards - 2, cards) * b) % cards;

            Console.WriteLine(result);
            //       2406044462912 is too low
            //      65304365993455 is too high
            //      6821410630991
            // 9223372036854775808 long overflow
            //Console.WriteLine(answer);
        }

        public static BigInteger SlowShuffle(int position, BigInteger cards, BigInteger shuffles, ICollection<string> input){
            ICollection<int> deck = Enumerable.Range(0, (int)cards).ToList();
            for (BigInteger i = 0; i < shuffles; i ++)
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

        public static (BigInteger x, BigInteger y, BigInteger gcd) euclid(BigInteger a, BigInteger b)
        {
            BigInteger x0 = 1, xn = 1, y0 = 0, yn = 0, x1 = 0, y1 = 1, f, r = a % b;
 
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

        public static BigInteger FastShuffle(BigInteger position, BigInteger cards, BigInteger shuffles, ICollection<string> input){
            var values = new HashSet<BigInteger>();
            List<BigInteger> indexableValues = new List<BigInteger>();
                        input = input.Reverse().ToList();

            BigInteger answer = position;
            values.Add(answer);
            indexableValues.Add(answer);
           // while(true){
            foreach(var line in input){
                if (line.Contains("stack")){
                    answer = (cards - 1 - answer) % cards;
                // Console.WriteLine($"{cards}, {answer}");
                }
                else if (line.Contains("cut")){
                    var N = BigInteger.Parse(line.Split(' ').Last());
                    answer = (answer + N) % cards;
                }
                else if (line.Contains("increment")){
                    var N = BigInteger.Parse(line.Split(' ').Last());
                    var euclidAns = euclid(N, cards);
                    answer = (answer * euclidAns.x) % cards;

                }
                if (answer < 0){
                    answer += cards;
                }
            }

                //indexableValues.Add(answer);
                // if (!values.Add(answer)){
                //     break;
                // }
            //}

            // Now we have done some shuffles and reached a cyclical value
            // Need to figure out which element of the cycle we care about.

            //answer = BigInteger.ModPow(answer % cards, shuffles, cards);




            // var cycleLength = indexableValues.IndexOf(answer, indexableValues.IndexOf(answer) + 1) - indexableValues.IndexOf(answer);
            
            // var isLeadInTime = indexableValues.IndexOf(answer) != 0;
            // var itemsInCycle = shuffles - indexableValues.IndexOf(answer);
            // var cyclesToCompletion = (double)itemsInCycle / (double)cycleLength;
            // var finalCycleProportion = cyclesToCompletion % 1;
            // var finalCycleLastIndex = (int)(finalCycleProportion * cycleLength);
            
            // var indexOfLastThing = (int)((((double)(shuffles-indexableValues.IndexOf(answer))/(double)cycleLength) % 1) * cycleLength);

            //answer = indexableValues[indexableValues.IndexOf(answer) + finalCycleLastIndex];

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


        public static BigInteger HowManyCardsHaveBeenDealtBeforeThisOne(BigInteger n, BigInteger length, int k){
            BigInteger value = 0;
            
            for(int j = 0; j < k; j++)
            {
                value += 1 + (length - j - 1)/n;
            }
            
            return value;
            
        }

        public static BigInteger CardsDealtInRound(BigInteger n, BigInteger length, int k){
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
