using System;
using System.Collections.Generic;
using System.Linq;

namespace Day_25
{
    class Program
    {
        static void Main(string[] args)
        {
            char state = 'A';
            long diagnostic = 12208951;
            long cursor = 0;
            long steps = 0;

            IDictionary<long, int> states = new Dictionary<long, int>();

            while(steps < diagnostic)
            {
                if (!states.TryGetValue(cursor, out int value)){
                    value = 0;
                }

                switch(state){
                    case 'A':
                        if (value == 0)
                        {
                            states[cursor] = 1;
                            cursor++;
                            state = 'B';
                        }
                        else
                        {
                            states[cursor] = 0;
                            cursor--;
                            state = 'E';
                        }
                        break;
                    case 'B':
                        if (value == 0)
                        {
                            states[cursor] = 1;
                            cursor--;
                            state = 'C';
                        }
                        else
                        {
                            states[cursor] = 0;
                            cursor++;
                            state = 'A';
                        }
                        break;
                    case 'C':
                        if (value == 0)
                        {
                            states[cursor] = 1;
                            cursor --;
                            state = 'D';
                        }
                        else
                        {
                            states[cursor] = 0;
                            cursor++;
                            state = 'C';
                        }
                        break;
                    case 'D':
                        if (value == 0)
                        {
                            states[cursor] = 1;
                            cursor --;
                            state = 'E';
                        }
                        else
                        {
                            states[cursor] = 0;
                            cursor --;
                            state = 'F';
                        }
                        break;
                    case 'E':
                        if (value == 0)
                        {
                            states[cursor] = 1;
                            cursor --;
                            state = 'A';
                        }
                        else
                        {
                            states[cursor] = 1;
                            cursor --;
                            state = 'C';
                        }
                        break;
                    case 'F':
                        if (value == 0)
                        {
                            states[cursor] = 1;
                            cursor --;
                            state = 'E';
                        }
                        else
                        {
                            states[cursor] = 1;
                            cursor ++;
                            state = 'A';
                        }
                        break;
                    default:
                        throw new Exception("oh noes");
                }
                steps++;
            }

            Console.WriteLine(states.Count(s => s.Value == 1));
        }
    }
}
