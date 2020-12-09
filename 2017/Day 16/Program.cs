using System;
using System.IO;
using System.Linq;

namespace Day_16
{
    class Program
    {
        static void Main(string[] args)
        {
            var chars = "abcdefghijklmnop".ToList();

            var moves = File.ReadLines("./input.txt").Single().Split(',').ToList();

            foreach(var move in moves)
            {
                switch(move[0]){
                    case 's':
                        var size = int.Parse(move.Substring(1));
                        var toFront = chars.TakeLast(size).ToList();
                        toFront.Reverse();
                        
                        chars.RemoveRange(chars.Count - size, size);

                        foreach(var character in toFront){
                            chars = chars.Prepend(character).ToList();
                        }
                        break;
                    
                    case 'x':
                        var indices = move.Split('/').Select(c => int.Parse(c.Split('x').Last())).ToList();
                        //(int.Parse(move[1].ToString()), int.Parse(move[3].ToString()));
                        var store = chars[indices[0]];
                        chars[indices[0]] = chars[indices[1]];
                        chars[indices[1]] = store;
                        break;

                    case 'p':
                        var characters = (move[1], move[3]);
                        var store2 = chars.IndexOf(characters.Item2);
                        chars[chars.IndexOf(characters.Item1)] = characters.Item2;
                        chars[store2] = characters.Item1;
                        break;
                }

            }
            
            foreach (var character in chars){
                Console.Write(character);
            }

            Console.WriteLine();

            

        }
    }
}
