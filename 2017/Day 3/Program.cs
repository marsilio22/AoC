using System;
using System.Collections.Generic;

namespace Day_3
{
    class Program
    {
        static void Main(string[] args)
        {
            
            //Part1(325489);
            Part2(325489);
        }

        public static void Part1(int dataPointOfInterest)
        {                        
            // The number of data points in square number n is:
            // n = 0: 1;
            // n > 0: 8(n); 
            // (sequence 1, 8, 16, 24, ...)

            // To work out which square our point of interest is at, start at 1, add the items from the sequence until that 
            // number exceeds our point of interest, then stop. That square contains our number
            int n=0, squareNumber=0;

            if (dataPointOfInterest != 1)
            {
                while(n < dataPointOfInterest)
                {
                    n += squareNumber == 0 ? 1 : 8 * squareNumber; // shorthand for if (squareNumber == 0) {add 1} else {add 8*n}
                    if (n < dataPointOfInterest) squareNumber++;
                }
            }

            Console.WriteLine("Sqar:" + squareNumber); // This is the number of square that the data point of interest is in
            Console.WriteLine("Maxx:" + n); // This is the maximum number in the square of interest

            // Now work out which direction the point of interest is in. 
            // This will let us figure out how far it is from a centrepoint on its square and thus let us figure out how far away it is
            int additionVariable = 0;
            int currentValue = 1;
            int previousValue = 0;
            while(currentValue < dataPointOfInterest)
            {
                for (var j = 0; currentValue < dataPointOfInterest && j < 3; j++)
                {
                    previousValue = currentValue;
                    currentValue += additionVariable;
                }

                if (currentValue >= dataPointOfInterest) break;

                additionVariable ++;
                previousValue = currentValue;
                currentValue += additionVariable;
                additionVariable ++;
            }

            Console.WriteLine("Prev:" + previousValue);
            Console.WriteLine("Curr:" + currentValue);

            var distance = Math.Min(Math.Abs(previousValue - dataPointOfInterest), Math.Abs(currentValue - dataPointOfInterest));

            var totalDistance = distance + squareNumber;

            Console.WriteLine("--------");
            Console.WriteLine("Total Distance: " + totalDistance);
        }

        public static void Part2(int dataPointOfInterest)
        {
            // Store of values
            var values = new Dictionary<(int x, int y), int>();
            
            // Initial values
            values[(0, 0)] = 1;
            (int x, int y) currentPosition = (1, 0);
            (int x, int y) directionOfTravel = (0, 1);
            (int x, int y) nextDirectionChangeCoordinate = (1, 1);

            // The result, reset every loop
            var n = 0;
            while (n < dataPointOfInterest)
            {
                n = 0;
                Console.WriteLine(currentPosition);
                var coords = GetSurroundingCoordinates(currentPosition.x, currentPosition.y);
                foreach(var coord in coords){
                    if (values.ContainsKey(coord)){
                        n += values[coord];
                    }
                }
                values[currentPosition] = n;

                // move
                currentPosition = AddCoords(currentPosition, directionOfTravel);

                // if it's time to change direction, do so.
                if (currentPosition.x == nextDirectionChangeCoordinate.x && currentPosition.y == nextDirectionChangeCoordinate.y) {
                    // these do just about what they say.
                    directionOfTravel = ChangeDirection(directionOfTravel);
                    nextDirectionChangeCoordinate = DetermineNextChangeCoordinate(nextDirectionChangeCoordinate);
                }
            }

            Console.WriteLine(n);
        }

        public static (int x, int y) AddCoords((int x, int y)firstCoordinate, (int x, int y)secondCoordinate)
        {
            return (firstCoordinate.x + secondCoordinate.x, firstCoordinate.y + secondCoordinate.y);
        }

        public static (int x, int y) ChangeDirection((int x, int y) currentDirection){
            if (currentDirection.x == 0 && currentDirection.y == 1){
                return (-1, 0);
            }
            else if ( currentDirection.x == -1 && currentDirection.y == 0){
                return (0, -1);
            }
            else if (currentDirection.x == 0 && currentDirection.y == -1){
                return (1, 0);
            }
            else if (currentDirection.x == 1 && currentDirection.y == 0){
                return (0, 1);
            }
            else {
                throw new InvalidOperationException("oops");
            }
        }

        public static (int x, int y) DetermineNextChangeCoordinate((int x, int y) currentChangeCoordinate){
            var thing = Math.Max(Math.Abs(currentChangeCoordinate.x), Math.Abs(currentChangeCoordinate.y));

            if (currentChangeCoordinate.x == currentChangeCoordinate.y && currentChangeCoordinate.x < 0){
                return (thing + 1, thing * -1);
            }
            else if (currentChangeCoordinate.x == currentChangeCoordinate.y)
            {
                return (thing * -1, thing);
            }
            else if (currentChangeCoordinate.x == currentChangeCoordinate.y * -1){
                return (thing * -1, thing * -1);
            }
            else return (thing, thing);
        }

        public static List<(int, int)> GetSurroundingCoordinates(int x, int y)
        {
            var result = new List<(int, int)>();
            for (var i = -1; i <= 1; i++){
                for (var j = -1; j <= 1; j++){
                    result.Add((x + i, y + j));
                }
            }

            result.Remove((x, y));
            return result;
        }
    }
}
