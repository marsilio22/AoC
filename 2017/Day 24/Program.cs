using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_24
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("./input.txt");

            ICollection<(int l, int r)> plugs = new List<(int, int)>();

            foreach(var line in lines){
                var splitLine = line.Split('/');
                var l = int.Parse(splitLine[0]);
                var r = int.Parse(splitLine[1]);
                plugs.Add((l, r));
            }

            var validStartingPlugs = plugs.Where(p => p.l == 0 || p.r == 0).ToList();

            foreach(var plug in validStartingPlugs)
            {
                var startingNumber = plug.l == 0 ? plug.r : plug.l;
                var remainingPlugs = plugs.ToList(); // copy
                remainingPlugs.Remove(plug);
                ICollection<(int l, int r)> bridge = ConstructBestChainForStartingPlug(startingNumber, remainingPlugs, false);
                var score = bridge.Sum(b => b.l + b.r) + startingNumber;
                Console.WriteLine($"plug ({plug.l}, {plug.r}) produced a bridge of strength {score}");
            }
            
            foreach(var plug in validStartingPlugs)
            {
                var startingNumber = plug.l == 0 ? plug.r : plug.l;
                var remainingPlugs = plugs.ToList(); // copy
                remainingPlugs.Remove(plug);
                ICollection<(int l, int r)> bridge = ConstructBestChainForStartingPlug(startingNumber, remainingPlugs, true);
                var score = bridge.Sum(b => b.l + b.r) + startingNumber;
                Console.WriteLine($"plug ({plug.l}, {plug.r}) produced a bridge of length {bridge.Count() + 1} strength {score}");
            }

        }

        public static ICollection<(int, int)> ConstructBestChainForStartingPlug(int startingNumber, ICollection<(int l, int r)> remainingPlugs, bool useLengthNotStrength)
        {
            var validNextPlugs = remainingPlugs.Where(r => r.l == startingNumber || r.r == startingNumber).ToList();

            var bridges = new Dictionary<(int l, int r), ICollection<(int l, int r)>>();

            if (!validNextPlugs.Any())
            {
                return new List<(int, int)>(); // we finish the bridge from the layer above us
            }
            else
            {
                foreach(var plug in validNextPlugs)
                {
                    var nextNumber = plug.l == startingNumber ? plug.r : plug.l;
                    var newRemainingPlugs = remainingPlugs.ToList(); // copy
                    var test = newRemainingPlugs.Remove(plug);

                    bridges.Add(plug, new List<(int l, int r)>{plug}.Concat(ConstructBestChainForStartingPlug(nextNumber, newRemainingPlugs, useLengthNotStrength)).ToList());
                }
            }

            ICollection<(int, int)> winningBridge = new List<(int, int)>();
            var winningBridgeScore = 0;
            var winningBridgeLength = bridges.Values.Select(b => b.Count).Max();

            foreach(var bridge in bridges){
                if (useLengthNotStrength && bridge.Value.Count() < winningBridgeLength)
                {
                    continue;
                }

                var score = bridge.Value.Sum(b => b.l + b.r);

                if (score > winningBridgeScore)
                {
                    winningBridge = bridge.Value;
                    winningBridgeScore = score;
                }
            }

            return winningBridge;
        }
    }
}
