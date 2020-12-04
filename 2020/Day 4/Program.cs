using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day_4
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadLines("./input.txt").ToList();

            var passportStrings = new List<List<string>>();
            passportStrings.Add(new List<string>());
            var passports = new List<Passport>();

            for (var i = 0; i < lines.Count(); i++)
            {
                if (lines[i].Equals(""))
                {
                    passportStrings.Add(new List<string>());
                    i++;
                }
                passportStrings.Last().AddRange(lines[i].Split(' '));
            }

            foreach(var passportString in passportStrings)
            {
                passports.Add(new Passport(passportString));
            }

            Console.WriteLine(passports.Count(p => p.IsValid()));
        }
    }

    public class Passport {

        public Passport(ICollection<string> input){
            foreach(var property in input){
                var split = property.Split(':');


                //var prop = this.GetType().GetProperties().Single(p => p.Name.Equals(split[0])).GetSetMethod().Invoke(this, new [] {split[1]});
                // todo reflection.
                switch(split[0]){
                    case "byr":
                        this.byr = split[1];
                        break;
                    case "iyr":
                        this.iyr = split[1];
                        break;
                    case "eyr":
                        this.eyr = split[1];
                        break;
                    case "hgt":
                        this.hgt = split[1];
                        break;
                    case "hcl":
                        this.hcl = split[1];
                        break;
                    case "ecl":
                        this.ecl = split[1];
                        break;
                    case "pid":
                        this.pid = split[1];
                        break;
                    case "cid":
                        this.cid = split[1];
                        break;
                    default:
                        throw new Exception("Something unknown happened");
                }
            }
        }
    
        public string byr {get; set;}
        public string iyr {get; set;}
        public string eyr {get; set;}
        public string hgt {get; set;}
        public string hcl {get; set;}
        public string ecl {get; set;}
        public string pid {get; set;}
        public string cid {get; set;}

        public bool IsValid(){
            if (byr == null || iyr == null || eyr == null || hgt == null || hcl == null || ecl == null || pid == null){
                return false;
            }

            int.TryParse(this.byr, out int byrVal);
            var byrValid = byrVal <= 2002 && byrVal >= 1920;

            int.TryParse(this.iyr, out int iyrVal);
            var iyrValid = iyrVal <= 2020 && iyrVal >= 2010;

            int.TryParse(this.eyr, out int eyrVal);
            var eyrValid = eyrVal <= 2030 && eyrVal >= 2020;

            bool hgtValid = false;
            int.TryParse(hgt.Substring(0, hgt.Length - 2), out int hgtVal);
            if (hgt.EndsWith("cm")){
                hgtValid = hgtVal >=150 && hgtVal <= 193;
            }
            else if (hgt.EndsWith("in")){
                hgtValid = hgtVal >= 59 && hgtVal <= 76;
            }

            var regex = new Regex("^#[0-9a-f]{6}$");
            var hclValid = regex.Match(hcl).Success;

            var eyeColours = new []{"amb","blu", "brn", "gry", "grn", "hzl", "oth"};
            var eclValid = eyeColours.Contains(ecl);

            regex = new Regex("^[0-9]{9}$");
            var pidValid = regex.Match(pid).Success;

            return 
                byrValid && 
                iyrValid && 
                eyrValid && 
                hgtValid && 
                hclValid && 
                eclValid && 
                pidValid ;
        }
    }
}
