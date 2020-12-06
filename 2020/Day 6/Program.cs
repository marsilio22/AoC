using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_6
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadLines("./input.txt").ToList();

            var part1 = CalculateSurveyGroupCount(lines, (x, y) => x.Union(y).ToList());
            var part2 = CalculateSurveyGroupCount(lines, (x, y) => x.Intersect(y).ToList());

            Console.WriteLine(part1);
            Console.WriteLine(part2);
        }

        public static int CalculateSurveyGroupCount(ICollection<string> lines, Func<ICollection<char>, ICollection<char>, ICollection<char>> groupMethod)
        {
            var surveys = new Dictionary<int, List<char>>();
            surveys.Add(0, new List<char>());
            var firstLine = true;

            foreach(var line in lines){
                if (line.Equals(string.Empty)){
                    surveys.Add(surveys.Count, new List<char>());
                    firstLine = true;
                    continue;
                }

                // We can't just assume an empty entry means that we're starting a 
                // new group, as the groupmethod on the previous loop may have resulted 
                // in an empty collection
                if (firstLine){
                    surveys[surveys.Count - 1] = line.ToCharArray().ToList();
                    firstLine = false;
                }

                var newAnswers = line.ToCharArray();
                surveys[surveys.Count - 1] = groupMethod(surveys[surveys.Count - 1], newAnswers).ToList();
            }

            var answer = surveys.Sum(s => s.Value.Count);
            return answer;
        }
    }
}
