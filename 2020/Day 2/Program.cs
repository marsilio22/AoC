using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_2
{
    class Program
    {
        static void Main(string[] args)
        {
            // 15-19 k: kkkkkkkkkkkkzkkkkkkk
            var lines = File.ReadLines("./input.txt");

            ICollection<(int a, int b, char letter, string password)> splitlines = lines.Select(l => l.Split(' ')).Select(
                l => {
                    var ab = l[0].Split('-').Select(m => int.Parse(m)).ToList();

                    var letter = l[1].Split(':')[0][0];

                    var password = l[2];

                    return (a: ab[0], b: ab[1], letter: letter, password: password);
                }
            ).ToList();

            var count = 0;

            foreach(var line in splitlines){
                var lettercount = line.password.Count(s => s == line.letter);

                if (lettercount >= line.a && lettercount <= line.b){
                    count++;
                }
            }

            Console.WriteLine(count);

            count = 0;
            foreach(var line in splitlines){
                if (
                    line.password[line.a - 1] == line.letter && line.password[line.b - 1] != line.letter
                    || line.password[line.a - 1] != line.letter && line.password[line.b - 1] == line.letter){
                    count++;
                }
            }

            Console.WriteLine(count);
        }
    }
}
