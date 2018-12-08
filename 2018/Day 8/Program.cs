using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_8
{
    class Program
    {
        private static int treeDataIndex = 0;

        static void Main(string[] args)
        {
            var line = File.ReadAllLines("./input.txt")[0].Split(' ').Select(c => int.Parse(c)).ToList();
            // var line = new [] {2, 3, 0, 3, 10, 11, 12, 1, 1, 0, 1, 99, 2, 1, 1, 2}.ToList();
            //var line = new [] {3, 3, 0, 3, 10, 11, 12, 1, 1, 0, 1, 99, 2, 2, 1, 2, 1, 0, 1, 3, 0, 2, 4, 3, 2, 0, 1, 5, 1, 1, 1, 2}.ToList();
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

            var root = CreateTree(indices, line);

            Console.WriteLine(root.Value);
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

        public static Node CreateTree(List<int> nodeStartIndices, List<int> data)
        {
            Node root = new Node
            {
                Metadata = new List<int>(), 
                Children = new List<Node>()
            };

            var nodeStartIndex = nodeStartIndices.First();
            var numberOfChildren = data[nodeStartIndex];
            var amountOfMetadata = data[nodeStartIndex + 1];
            nodeStartIndices.RemoveAt(0);

            while(numberOfChildren > 0)
            {
                root.Children.Add(CreateTree(nodeStartIndices, data));
                numberOfChildren--;
            }

/*          if(nodeStartIndices.Any())
            {
                root.Metadata = data.GetRange(nodeStartIndices.First() - amountOfMetadata, amountOfMetadata);
                treeDataIndex = nodeStartIndices.First();
            }
            else*/ if (data[nodeStartIndex] == 0) // initial number of children was 0, this is a leaf node
            {
                root.Metadata = data.GetRange(nodeStartIndex + 2, amountOfMetadata);
                treeDataIndex = nodeStartIndex + 2 + amountOfMetadata;
            }
            else // edge case, this is the last node in the list, but not necessarily the top node
            {
                root.Metadata = data.GetRange(treeDataIndex, amountOfMetadata);
                treeDataIndex += amountOfMetadata;
            }

            return root;
        }

        public class Node
        {
            public List<int> Metadata { get; set; }
            public List<Node> Children{ get; set; }

            public int Value => Children.Any() ? Metadata.Select(m => m > Children.Count() ? 0 : Children[m-1].Value).Sum() : Metadata.Sum();
        }
    }
}
