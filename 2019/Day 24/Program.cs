using System;
using System.Collections.Generic;
using System.IO;

namespace Day_24
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("./input.txt");

            var previousStates = new HashSet<long>();

            var state = new List<int>(); // 0 or 1 for life or no life

            foreach(var line in input){
                foreach(var character in line){
                    int life = character == '#' ? 1 : 0;
                    state.Add(life);
                }
            }

            while (previousStates.Add(GetBiodiversity(state))){
                Draw(state);

                // a thing is adjacent to +1, -1, +5 and -5
                var newState = new List<int>();
                for (int i = 0; i < state.Count; i ++){
                    var currentStateValue = state[i];

                    var adjacentsCount = 
                        (i-1 < 0 || i % 5 == 0 ? 0 : state[i-1]) +
                        (i-5 < 0 ? 0 : state[i-5]) +
                        (i+1 >= state.Count || (i+1) % 5 == 0 ? 0 : state[i+1]) +
                        (i+5 >= state.Count ? 0 : state[i+5]);

                    if (currentStateValue == 1 && adjacentsCount != 1){
                        newState.Add(0);    
                    }
                    else if (currentStateValue == 1)
                    {
                        newState.Add(1);
                    }

                    else if (currentStateValue == 0 && (adjacentsCount == 1 || adjacentsCount == 2)) {
                        newState.Add(1);
                    }
                    else if (currentStateValue == 0){
                        newState.Add(0);
                    }

                }
                state = newState;
            }

            Draw(state);

            var answer = GetBiodiversity(state);
            Console.WriteLine(answer);
            // 63181321 too high


        }

        static void Draw(List<int> map){
            for (int i = 0; i < 5; i ++){
                for (int j = 0; j < 5; j ++){
                    char character = map[i*5 + j] == 1 ? '#' : ' ';
                    Console.Write(character);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        static long GetBiodiversity (List<int> map){
            long total = 0;
            for (int i = 0; i < map.Count; i++){
                total += (long)Math.Pow(2, i) * map[i];
            }

            return total;
        }
    }
}
