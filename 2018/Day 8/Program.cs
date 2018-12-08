using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_8
{
    class Program
    {
        static void Main(string[] args)
        {
            // DataStructure:
            //  A header, which is always exactly two numbers:
            //      The quantity of child nodes.
            //      The quantity of metadata entries.
            //  Zero or more child nodes (as specified in the header).
            //  One or more metadata entries (as specified in the header).

            var line = File.ReadAllLines("./input.txt")[0].Split(' ').Select(c => int.Parse(c)).ToList();
            
// var line = new [] {3, 3, 0, 3, 10, 11, 12, 1, 1, 0, 1, 99, 2, 2, 1, 2, 1, 0, 1, 3, 0, 2, 4, 3, 7, 0, 1, 5, 1, 1, 1, 2}.ToList();
//var line = new [] {2, 1, 2, 1, 0, 1, 3, 0, 2, 4, 3, 7, 0, 1, 5, 1}.ToList();
            var indices = new List<int>();
            GetLengthOfChunk(0, line, indices);

            int total = 0;
            for(int i = 0; i < line.Count; i++){
                if (!indices.Contains(i) && !indices.Contains(i-1))
                {
                    total += line[i];
                }
            }
            
            Console.WriteLine(total);
        }

        public static int GetLengthOfChunk(int currentIndex, List<int> list, List<int> indices)
        {
            indices.Add(currentIndex);

            var alreadyChopped = 0;
            var children = list[0];
            var initialChildren = list[0];
            var numberOfMetaData = list[1];

            int length = 2; 
            if (children == 0)
            {
                return length + list[1]; 
            }

            while (children > 0)
            {
                if (children == initialChildren){
                    length = length + GetLengthOfChunk( length + currentIndex, list.GetRange(2, list.Count - 2), indices);
                }
                else{
                    length = length + GetLengthOfChunk( length + currentIndex, list, indices);
                }
                children -= 1;
                
                if (children > 0)
                {
                    list = list.GetRange(length - alreadyChopped, list.Count - (length - alreadyChopped));
                    alreadyChopped = length;
                }
            }

            length = length + numberOfMetaData;
            return length;
        }

        public class Node
        {
            public List<int> Metadata { get; set; }
            public List<Node> Children{ get; set; }
        }
    }
}
