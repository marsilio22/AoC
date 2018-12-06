using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_7
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("./input.txt");

            // Part 1
            var progs = new List<Prog>();

            foreach(var line in lines){
                progs.Add(new Prog(line));
            }

            foreach(var prog in progs){
                prog.Progs = progs.Where(p => prog.SupportedProgs?.Contains(p.Name) ?? false)?.ToList();
            }

            var ans = progs.Single(p => !progs.SelectMany(prog => prog.SupportedProgs ?? new List<string>()).ToList().Contains(p.Name));

            Console.WriteLine(ans.Name);

            // Part 2

            var currentProg = ans;
            while(true){
                var weights = currentProg.Select(p => p.)
                if()
            }
        }
    }

    public class Prog
    {
        public string Name { get; set; }

        public int Weight { get; set; }

        public List<string> SupportedProgs { get; set; }

        public List<Prog> Progs {get;set;} = new List<Prog>();

        public int SubWeight => Progs.Any() ? Progs.Sum(p => p.SubWeight) : Weight;

        public Prog(string line){
            var splitOnSupports = line.Split("->");
            if (splitOnSupports.Length > 1){
                this.SupportedProgs = splitOnSupports[1].Trim().Split(", ").ToList();
            }
            this.Name = splitOnSupports[0].Split('(')[0].Trim();
            this.Weight = int.Parse(splitOnSupports[0].Split('(')[1].Trim().Split(')')[0]);
        }
    }
}
