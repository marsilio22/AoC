using System;
using System.Collections.Generic;

namespace Day_17
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = 344;
            
            var list = new List<int>{0};
            int j = 0;
            
            for (var i = 1; i <= 2017; i++)
            {
                j = (input + j) % list.Count;

                list.Insert(j+1, i);

                j = j+1;
            }

            Console.WriteLine(list[j+1]);
        

            // things get inserted after 0 when the j is 0

            j = 0;
            int ans = 1;
            for (var i = 1; i <= 50_000_000; i++)
            {
                j = (input + j) % i;
                if (j == 0)
                {
                    ans = i;
                }

                j ++;
            }

            Console.WriteLine(ans);
        }
    }
}
