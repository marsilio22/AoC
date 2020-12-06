using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Day_14
{
    class Program
    {
        static ICollection<Square> unmarkedSquares;

        static void Main(string[] args)
        {
            var input = "jxqlasbh";

            var usedSquareCount = 0;

            unmarkedSquares = new List<Square>();

            // var regions = new Dictionary<int, ICollection<int>>();

            for (int i = 0; i < 128; i++)
            {
                var hash = DenseHash(input + $"-{i}");

                var binary = hash.SelectMany(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')).ToList();

                usedSquareCount += binary.Count(c => c == '1');

                for (int j = 0; j < 128; j++)
                {
                    if (binary[j] == '1')
                    unmarkedSquares.Add(new Square{x = j, y = i, marked = false});
                }
            }

            // part 1
            Console.WriteLine(usedSquareCount);

            // part 2
            

            var groupCounter = 0;

            while(unmarkedSquares.Any()){
                var square = unmarkedSquares.First();
                groupCounter++;

               MarkSquareNeighbours(square);
            }

            Console.WriteLine(groupCounter);
        }

        public static string DenseHash(string input){
            
            // from day 10, calculate dense hash
            var lengths = input.ToCharArray().Select(c => (int)c).ToList();
            lengths.AddRange(new int[] {17, 31, 73, 47, 23});

            var list = new LinkedList<int> (Enumerable.Range(0, 256).ToList());

            var skip = 0;
            var pos = list.First;
            var pos2 = list.First;

            for(int i = 0; i < 64; i++){

                foreach(var length in lengths)
                {
                    var listToReverse = new List<int>();

                    for (int j = 0; j < length; j++){
                        listToReverse.Add(pos.Value);
                        pos = pos.Next;
                        if (pos == null)
                        {
                            pos = list.First;
                        }
                    }

                    listToReverse.Reverse();

                    foreach(var reversedValue in listToReverse)
                    {
                        pos2.Value = reversedValue;
                        pos2 = pos2.Next;
                        if (pos2 == null)
                        {
                            pos2 = list.First;
                        }
                    }

                    for(int j = 0; j < skip; j++){
                        pos = pos.Next;
                        if (pos == null)
                        {
                            pos = list.First;
                        }

                        pos2 = pos2.Next;
                        if (pos2 == null)
                        {
                            pos2 = list.First;
                        }
                    }

                    skip ++;
                }
            }

            var denseHash = new List<int>();
            var totalpos = list.First;
            for (int i = 0; i < 16; i++)
            {
                denseHash.Add(totalpos.Value);
                totalpos = totalpos.Next;
                for (int j = 0; j < 15; j++)
                {
                    denseHash[denseHash.Count - 1] ^= totalpos.Value;
                    totalpos = totalpos.Next;
                }
            }

            StringBuilder sb = new StringBuilder();

            foreach(var val in denseHash)
            {
                sb.Append(val.ToString("x2"));
            }

            return sb.ToString();
        }
    
        public static void MarkSquareNeighbours(Square square)
        {
            square.marked = true;
            unmarkedSquares.Remove(square);
            var squares = unmarkedSquares.Where(s => 
                Math.Abs(s.x - square.x) == 1 && Math.Abs(s.y - square.y) == 0 ||
                Math.Abs(s.x - square.x) == 0 && Math.Abs(s.y - square.y) == 1).ToList();

            foreach(var square2 in squares)
            {
                MarkSquareNeighbours(square2);
            }
        }
    }


    public class Square
    {
        public int x { get; set; }
        public int y { get; set; }
        public bool marked { get; set; }
        public int group { get; set; } = 0;
    }
}
