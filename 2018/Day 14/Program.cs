using System;
using System.Collections.Generic;

namespace Day_14
{
    class Program
    {
        static void Main(string[] args)
        {
            // var input = new List<int>{0, 4, 7, 8, 0, 1};
            
            var input = new List<int> {0, 4, 7, 8, 0, 1};

            var state = new List<int> {3, 7};
            var firstElfIndex = 0;
            var secondElfIndex = 1;
            var count = state.Count;

            while(!ShouldIFinish(input, state))
            {
                var newScore = state[firstElfIndex] + state[secondElfIndex];

                if (newScore >= 10)
                {
                    state.Add(newScore / 10);
                }
                state.Add(newScore % 10);

                count = state.Count;
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
        }

        public static bool ShouldIFinish(List<int> input, List<int> state){
            bool result = true;

            if (state.Count < input.Count + 1){
                result = false;
                return result;
            }

            for(int i = 1; i <= input.Count; i++){
                if (state[state.Count - i] != input[input.Count - i] && state[state.Count - 1 - i] != input[input.Count - i])
                {
                    result = false;
                }
            }

            return result;
        }
    }
}
