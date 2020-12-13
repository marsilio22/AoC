using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_12
{
    class Program
    {
        static void Main(string[] args)
        {
            ICollection<(char instruction, int val)> lines = File.ReadLines("./input.txt").Select(l => (l[0], int.Parse(l.Substring(1)))).ToList();

            // var test = lines.Where(l => l.Item1 == 'L' || l.Item1 == 'R').Select(l => l.Item2).Distinct().ToList();

            var currentFacing = 'E';
            (int x, int y) shipCoord = (0, 0);

            foreach(var line in lines)
            {
                switch(line.instruction){
                    case 'N':
                    case 'S':
                    case 'E':
                    case 'W':
                        shipCoord = Move(shipCoord, line.instruction, line.val);
                        break;
                    case 'F':
                        shipCoord = Move(shipCoord, currentFacing, line.val);
                        break;
                    case 'L':
                    case 'R':
                        var dir = line.instruction;
                        var amount = line.val;
                        if (dir == 'L')
                        {
                            amount *= -1;
                            amount += 360;
                        }
                        currentFacing = Turn(currentFacing, amount);

                        break;
                }
            }

            Console.WriteLine(Math.Abs(shipCoord.x) + Math.Abs(shipCoord.y));





            currentFacing = 'E';
            shipCoord = (0, 0);
            var wayPointCoord = (10, 1);

            foreach(var line in lines)
            {
                switch(line.instruction){
                    case 'N':
                    case 'S':
                    case 'E':
                    case 'W':
                        wayPointCoord = Move(wayPointCoord, line.instruction, line.val);
                        break;
                    case 'F':
                        shipCoord = MoveTowards(shipCoord, wayPointCoord, line.val);
                        break;
                    case 'L':
                    case 'R':
                        var dir = line.instruction;
                        var amount = line.val;
                        if (dir == 'L')
                        {
                            amount *= -1;
                            amount += 360;
                        }
                        wayPointCoord = TurnAround(wayPointCoord, amount);

                        break;
                }
            }

            // 447323 too high
            Console.WriteLine(Math.Abs(shipCoord.x) + Math.Abs(shipCoord.y));

        }

        public static (int x, int y) TurnAround((int x, int y) turnCoord, int degreesClockwise){
            (int x, int y) ans = (turnCoord.x, turnCoord.y);

            for (int i = 0; i < degreesClockwise; i += 90)
            {
                ans = (ans.y, -1 * ans.x);
            }

            return ans;
        }

        public static char Turn(char facing, int degreesClockwise){
            var dirs = new List<char> {'N', 'E', 'S', 'W'};

            var currentDir = dirs.IndexOf(facing);
            var newDir = dirs[(currentDir + (degreesClockwise / 90)) % 4];
            return newDir;
        }

        public static (int x, int y) Move((int x, int y) coordinate, char direction, int distance){
            (int x, int y) ans = coordinate;

            switch(direction){
                case 'N':
                    ans.y = coordinate.y + distance;
                    break;
                case 'S':
                    ans.y = coordinate.y - distance;
                    break;
                case 'E':
                    ans.x = coordinate.x + distance;
                    break;
                case 'W':
                    ans.x = coordinate.x - distance;
                    break;
            }
            return ans;
        }

        public static (int x, int y) MoveTowards((int x, int y) coordinate, (int x, int y) target, int reps){
            var ans = (coordinate.x + reps * target.x, coordinate.y + reps * target.y);
            return ans;
        }
    }
}
