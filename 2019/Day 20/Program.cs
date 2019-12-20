using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_20
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("./input.txt");

            HashSet<Coordinate> coords = new HashSet<Coordinate>();

            var x = 0;
            var y = 0;
            foreach(var line in input){
                foreach(var character in line){
                    if (character == '.'){
                        coords.Add(new Coordinate{X = x, Y = y});
                    }
                    x++;
                }
                y++; x = 0;
            }

            var wallsandemptyspaces = new [] {'.','#',' '};
            Coordinate entrance = new Coordinate(), exit = new Coordinate();
            var portals = new Dictionary<string, Coordinate>();
            for (y = 0; y < input.Length - 1; y ++){
                for (x = 0; x < input[y].Length - 1; x ++){
                    if (!wallsandemptyspaces.Contains(input[y][x])){
                        string portalKey = null;
                        Coordinate adjacentCoordinate = new Coordinate();
                        // Vertical
                        if (!wallsandemptyspaces.Contains(input[y+1][x]))
                        {
                            portalKey = input[y][x].ToString() + input[y+1][x];
                            adjacentCoordinate = coords.First(c => c.X == x && c.Y == y+2 || c.X == x && c.Y == y-1);
                        }
                        // Horizontal
                        else if (!wallsandemptyspaces.Contains(input[y][x+1])){
                            portalKey = input[y][x].ToString() + input[y][x+1];
                            adjacentCoordinate = coords.First(c => c.X == x+2 && c.Y == y || c.X == x-1 && c.Y == y);
                        }
                        else{
                            continue;
                        }

                        if (portalKey.Equals("AA")){
                            entrance = adjacentCoordinate;
                            continue;
                        }
                        else if (portalKey.Equals("ZZ")){
                            exit = adjacentCoordinate;
                            continue;
                        }

                        if (portals.ContainsKey(portalKey)){
                            // adjacentCoordinate.Adjacents.Add(portals[portalKey].Adjacents[0]);
                            portals[portalKey].Adjacents.Add(adjacentCoordinate);
                        }
                        else
                        {
                            portals.Add(portalKey, new Coordinate{Adjacents = new List<Coordinate>{adjacentCoordinate}});
                        }
                    }
                }
            }

            foreach(var coord in coords){
                var adjs = coords.Where(c => 
                    c.X == coord.X && Math.Abs(c.Y - coord.Y) == 1 ||
                    Math.Abs(c.X - coord.X) == 1 && c.Y == coord.Y);
                coord.Adjacents.AddRange(adjs);
            }

            Coordinate currentPosition = entrance;

            distanceToNext(entrance, coords, 0, portals);

            Console.WriteLine(exit.DistanceFromEntrance);



            // TODO method
            coords = new HashSet<Coordinate>();

            x = 0;
            y = 0;
            foreach(var line in input){
                foreach(var character in line){
                    if (character == '.'){
                        for (int i = 0; i < 10; i ++)
                            coords.Add(new Coordinate{X = x, Y = y, Layer = i});
                    }
                    x++;
                }
                y++; x = 0;
            }

            wallsandemptyspaces = new [] {'.','#',' '};
            entrance = new Coordinate();
            exit = new Coordinate();
            portals = new Dictionary<string, Coordinate>();
            for (y = 0; y < input.Length - 1; y ++){
                for (x = 0; x < input[y].Length - 1; x ++){
                    if (!wallsandemptyspaces.Contains(input[y][x])){
                        string portalKey = null;
                        Coordinate adjacentCoordinate = new Coordinate();
                        // Vertical
                        if (!wallsandemptyspaces.Contains(input[y+1][x]))
                        {
                            portalKey = input[y][x].ToString() + input[y+1][x];
                            adjacentCoordinate = coords.First(c => c.X == x && c.Y == y+2 || c.X == x && c.Y == y-1);
                        }
                        // Horizontal
                        else if (!wallsandemptyspaces.Contains(input[y][x+1])){
                            portalKey = input[y][x].ToString() + input[y][x+1];
                            adjacentCoordinate = coords.First(c => c.X == x+2 && c.Y == y || c.X == x-1 && c.Y == y);
                        }
                        else{
                            continue;
                        }

                        if (portalKey.Equals("AA")){
                            entrance = adjacentCoordinate;
                            continue;
                        }
                        else if (portalKey.Equals("ZZ")){
                            exit = adjacentCoordinate;
                            continue;
                        }

                        if (portals.ContainsKey(portalKey)){
                            // adjacentCoordinate.Adjacents.Add(portals[portalKey].Adjacents[0]);
                            portals[portalKey].Adjacents.Add(adjacentCoordinate);
                        }
                        else
                        {
                            portals.Add(portalKey, new Coordinate{Adjacents = new List<Coordinate>{adjacentCoordinate}});
                        }
                    }
                }
            }

        }

        public static void distanceToNext(Coordinate currentPosition, ICollection<Coordinate> map, int distance, Dictionary<string, Coordinate> portals){
            var newShortestPath = currentPosition.DistanceFromEntrance > distance;
            currentPosition.DistanceFromEntrance = newShortestPath ? distance : currentPosition.DistanceFromEntrance;
            if (newShortestPath){
                distance++;
                var adjs = currentPosition.Adjacents;
                var adjPortals = portals.Values.Where(p => p.Adjacents.Contains(currentPosition)).ToList();

                if (adjPortals.Any()){
                    adjs = adjs.Union(adjPortals[0].Adjacents.Where(p => p != currentPosition)).ToList();
                }

                foreach(var adj in adjs){
                    distanceToNext(adj, map, distance, portals);
                }
            }
        }

        public static void distanceToNextWithLayers(Coordinate currentPosition, ICollection<Coordinate> map, int distance, Dictionary<string, Coordinate> portals, int layer){
            var newShortestPath = currentPosition.DistanceFromEntrance > distance;
            currentPosition.DistanceFromEntrance = newShortestPath ? distance : currentPosition.DistanceFromEntrance;
            if (newShortestPath){
                distance++;
                var adjs = currentPosition.Adjacents;
                // need to figure out 
                var adjPortals = portals.Values.Where(p => p.Adjacents.Contains(currentPosition)).ToList();

                if (adjPortals.Any()){
                    adjs = adjs.Union(adjPortals[0].Adjacents.Where(p => p != currentPosition)).ToList();
                }

                foreach(var adj in adjs){
                    distanceToNext(adj, map, distance, portals);
                }
            }

        }
    }

    public class Coordinate{
        public int X { get; set; }
        public int Y { get; set; }
        public int Layer { get; set; }
        public int DistanceFromEntrance {get;set;} = int.MaxValue;
        public List<Coordinate> Adjacents { get; set; } = new List<Coordinate>();
    }
}
