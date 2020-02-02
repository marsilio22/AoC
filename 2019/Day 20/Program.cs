using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Day_20
{
    class Program
    {
        public static int MazeSize;

        static void Main(string[] args)
        {
            Thread T = new Thread(DoThing, 16000000);
            T.Start();
        }

        public static void DoThing(){
            var input = File.ReadAllLines("./input.txt");
            MazeSize = input[0].Length;

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

            distanceToNext(entrance, coords, 0, portals);

            Console.WriteLine(exit.DistanceFromEntrance);








            // TODO method
            coords = new HashSet<Coordinate>();
            var maxLayer = 25;

            x = 0;
            y = 0;
            foreach(var line in input){
                foreach(var character in line){
                    if (character == '.'){
                        for (int i = 0; i < maxLayer; i ++)
                            {coords.Add(new Coordinate{X = x, Y = y, Layer = i});}
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
                        List<Coordinate> adjacentCoordinates = new List<Coordinate>();
                        // Vertical
                        if (!wallsandemptyspaces.Contains(input[y+1][x]))
                        {
                            portalKey = input[y][x].ToString() + input[y+1][x];
                            adjacentCoordinates = coords.Where(c => (c.X == x && c.Y == y+2 || c.X == x && c.Y == y-1)).ToList();
                        }
                        // Horizontal
                        else if (!wallsandemptyspaces.Contains(input[y][x+1])){
                            portalKey = input[y][x].ToString() + input[y][x+1];
                            adjacentCoordinates = coords.Where(c => (c.X == x+2 && c.Y == y || c.X == x-1 && c.Y == y)).ToList();
                        }
                        else {
                            continue;
                        }

                        if (portalKey.Equals("AA")){
                            entrance = adjacentCoordinates.Single(a => a.Layer == 0);
                            continue;
                        }
                        else if (portalKey.Equals("ZZ")){
                            exit = adjacentCoordinates.Single(a => a.Layer == 0);
                            continue;
                        }
                        else if (portalKey.Equals("AA") || portalKey.Equals("ZZ")){
                            continue;
                        }

                        if (portals.ContainsKey(portalKey)){
                            portals[portalKey].Adjacents.AddRange(adjacentCoordinates);
                        }
                        else
                        {
                            portals.Add(portalKey, new Coordinate{Adjacents = new List<Coordinate>(adjacentCoordinates)});
                        }
                    }
                }
            }

            foreach(var coord in coords){
                var adjs = coords.Where(c => 
                    coord.Layer == c.Layer ).Where(c => (c.X == coord.X && Math.Abs(c.Y - coord.Y) == 1 ||
                    Math.Abs(c.X - coord.X) == 1 && c.Y == coord.Y));
                coord.Adjacents.AddRange(adjs);
            }

            distanceToNextWithLayers(entrance, coords, 0, portals, maxLayer);
//646toolow
// 1874 too low
// 1875 too low
            Console.WriteLine(exit.DistanceFromEntrance);
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

        public static void distanceToNextWithLayers(Coordinate currentPosition, ICollection<Coordinate> map, int distance, Dictionary<string, Coordinate> portals, int maxLayer){
            var newShortestPath = currentPosition.DistanceFromEntrance > distance;
            currentPosition.DistanceFromEntrance = newShortestPath ? distance : currentPosition.DistanceFromEntrance;
            if (newShortestPath){
                distance++;
                var adjs = currentPosition.Adjacents;
                var adjPortals = portals.Values.Where(p => p.Adjacents.Contains(currentPosition)).ToList();

                var layer = currentPosition.Layer;
                if (adjPortals.Any()){
                    if (currentPosition.X == 2 || currentPosition.X == MazeSize - 3 || currentPosition.Y == 2 || currentPosition.Y == MazeSize - 3)
                    {
                        // external edge
                        if (layer != 0){
                            adjs = adjs.Union(adjPortals[0].Adjacents.Where(p => !(p.X == currentPosition.X && p.Y == currentPosition.Y) && p.Layer == layer - 1)).ToList();
                        }
                    }
                    else
                    {
                        // internal edge 
                        if (layer != maxLayer){
                            adjs = adjs.Union(adjPortals[0].Adjacents.Where(p => !(p.X == currentPosition.X  && p.Y == currentPosition.Y) && p.Layer == layer + 1 )).ToList();
                        }
                    }
                }
                // Console.WriteLine(distance);
                foreach(var adj in adjs){
                    distanceToNextWithLayers(adj, map, distance, portals, maxLayer);
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
