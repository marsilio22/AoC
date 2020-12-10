using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_16
{
    class Program
    {
        static void Main(string[] args)
        {
            var chars = "abcdefghijklmnop".ToList();
            // var chars = "abcde".ToList();

            var moves = File.ReadLines("./input.txt").Single().Split(',').ToList();
            // var moves = "s1,x3/4,pe/b".Split(',').ToList();

            chars = Dance(chars, moves);
            
            foreach (var character in chars){
                Console.Write(character);
            }

            Console.WriteLine();

            var originalOrder = "abcdefghijklmnop".ToList();
            // var originalOrder = "abcde".ToList();

            var loops = 1;
            Func<bool> charsInOrder = () => {
                for(int i = 0; i < chars.Count; i++)
                {
                    if(chars[i] != originalOrder[i])
                    {
                        return false;
                    }
                }
                return true;
            };

            while (!charsInOrder())
            {
                chars = Dance(chars, moves);
                loops++;
            }

            var maxLoops = 1000000000 % loops;

            chars = originalOrder.ToList();

            for(int i = 0; i < maxLoops; i++){
                chars = Dance(chars, moves);
            }

            foreach(var character in chars){
                Console.Write(character);
            }

            Console.WriteLine();
        }

        public static List<char> Dance(List<char> chars, List<string> moves){
            var newChars = chars;

            foreach(var move in moves)
            {
                switch(move[0]){
                    case 's':
                        var size = int.Parse(move.Substring(1));
                        var toFront = newChars.TakeLast(size).ToList();
                        toFront.Reverse();
                        
                        newChars.RemoveRange(newChars.Count - size, size);

                        foreach(var character in toFront){
                            newChars = newChars.Prepend(character).ToList();
                        }
                        break;
                    
                    case 'x':
                        var indices = move.Split('/').Select(c => int.Parse(c.Split('x').Last())).ToList();
                        var store = newChars[indices[0]];
                        newChars[indices[0]] = newChars[indices[1]];
                        newChars[indices[1]] = store;
                        break;

                    case 'p':
                        var characters = (move[1], move[3]);
                        var store2 = newChars.IndexOf(characters.Item2);
                        newChars[newChars.IndexOf(characters.Item1)] = characters.Item2;
                        newChars[store2] = characters.Item1;
                        break;
                }
            }

            return newChars;
        }
    }
}
