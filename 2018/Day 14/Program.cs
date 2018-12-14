using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Day_14
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            
            var input = new List<int> {0, 4, 7, 8, 0, 1};

            var state = new List<int> {3, 7};
            var firstElfIndex = 0;
            var secondElfIndex = 1;

            while(!ShouldIFinish(input, state))
            {
                var newScore = state[firstElfIndex] + state[secondElfIndex];

                if (newScore >= 10)
                {
                    state.Add(newScore / 10);
                }
                state.Add(newScore % 10);

                var count = state.Count;
                firstElfIndex = (firstElfIndex + state[firstElfIndex] + 1) % count;
                secondElfIndex = (secondElfIndex + state[secondElfIndex] + 1) % count;
            }

            Console.WriteLine("Last 10 digits");
            for (int i = -10; i < 0; i ++)
            {
                Console.Write(state[state.Count + i]);
            }
            Console.WriteLine();

            Console.WriteLine("Number of Recipes");
            Console.WriteLine(state.Count - input.Count - 1);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            Console.ReadLine();
        }

        public static bool ShouldIFinish(List<int> input, List<int> state){
            bool result = true;

            if (state.Count < input.Count + 1){
                return false;
            }

            for(int i = 1; i <= input.Count; i++)
            {
                if (state[state.Count - i] != input[input.Count - i] && state[state.Count - 1 - i] != input[input.Count - i])
                {
                    result = false;
                }
            }

            if (result)
            {
                string stringState = "";
                for (int i = -7; i < 0; i++)
                {
                    stringState += state[state.Count + i];
                }

                var inputString = input[0].ToString() + input[1] + input[2] + input[3] + input[4] + input[5];

                result = stringState.Contains(inputString);
            }

            return result;
        }
    }
}
