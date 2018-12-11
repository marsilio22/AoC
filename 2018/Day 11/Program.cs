using System;
using System.Collections.Generic;
using System.Linq;

namespace Day_11
{
    class Program
    {
        static void Main(string[] args)
        {
            int gridSerial = 3628;
            Dictionary<(int x, int y), int> grid = new Dictionary<(int x, int y), int>();

            for (int i = 0; i < 300; i++)
            {
                for (int j = 0; j < 300; j++)
                {
                    int rackId = i + 10;
                    int powerLevel = (rackId * j);
                    powerLevel += gridSerial;
                    powerLevel *= (rackId);
                    powerLevel = (int) Math.Abs(powerLevel / 100 % 10);
                    powerLevel -= 5;

                    grid[(i, j)] = powerLevel;
                }
            }

            Dictionary<int, (int x, int y, int value)> largestCoordsBySquareSize = new Dictionary<int, (int x, int y, int value)>();

            for (int i = 0; i < 300; i++)
            {
                Console.WriteLine($"Calculating for {i}");
                Dictionary<(int x, int y), int> result = new Dictionary<(int x, int y), int>();

                for (int j = 0; j < 300 - i; j++){
                    for (int k = 0; k < 300 - i; k++){
                        result[(j, k)] = ComputeSquare(j, k, i, grid);
                    }
                }

                var largestValue = result.OrderByDescending(v => v.Value).First();

                largestCoordsBySquareSize[i] = (largestValue.Key.x, largestValue.Key.y, largestValue.Value);
            }

            var largestLargestValue = largestCoordsBySquareSize.OrderByDescending(v => v.Value.value).First();

            Console.WriteLine(largestLargestValue.Value.x+ "," + largestLargestValue.Value.y + "," + largestLargestValue.Key);
        }

        public static int ComputeSquare(int x, int y, int squareSize, Dictionary<(int x, int y), int> data)
        {
            var total = 0;

            for (int i = 0; i < squareSize; i++)
            {
                for (int j = 0; j < squareSize; j++)
                {
                    total += data[(x+i, y+j)];
                }
            }
            return total;
        }
    }
}
