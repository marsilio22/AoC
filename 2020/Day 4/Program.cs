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
                var prop = this.GetType()
                               .GetProperties()
                               .Single(p => p.Name.Equals(split[0]))
                               .GetSetMethod()
                               .Invoke(this, new [] {split[1]});
            }
        }
        private static readonly ICollection<string> eyeColours = new []{"amb","blu", "brn", "gry", "grn", "hzl", "oth"};
    
        private string byr {get; set;}
        private string iyr {get; set;}
        private string eyr {get; set;}
        private string hgt {get; set;}
        private string hcl {get; set;}
        private string ecl {get; set;}
        private string pid {get; set;}
        private string cid {get; set;}

        public int BirthYear { get { int.TryParse(this.byr, out int byrVal); return byrVal; } }
        public int IssueYear { get { int.TryParse(this.iyr, out int iyrVal); return iyrVal; } }
        public int ExpiryYear { get { int.TryParse(this.eyr, out int eyrVal); return eyrVal; }}

        public (int value, string units) Height { get {int.TryParse(hgt.Substring(0, hgt.Length - 2), out int hgtVal); if (hgtVal != 0) return (hgtVal, hgt.Substring(hgt.Length - 2)); else return (0, null); }}
        public string HairColour { get { return hcl; } }
        public string EyeColour { get { return ecl; } }
        public string PassportId { get { return pid; } }
        public string CountryId { get { return cid; } }

        public bool IsValid(){
            if (byr == null || iyr == null || eyr == null || hgt == null || hcl == null || ecl == null || pid == null){
                return false;
            }
            var byrValid = BirthYear <= 2002 && BirthYear >= 1920;
            var iyrValid = IssueYear<= 2020 && IssueYear >= 2010;
            var eyrValid = ExpiryYear <= 2030 && ExpiryYear >= 2020;

            bool hgtValid = false;
            if (hgt.EndsWith("cm")){
                hgtValid = Height.value >=150 && Height.value <= 193;
            }
            else if (hgt.EndsWith("in")){
                hgtValid = Height.value >= 59 && Height.value <= 76;
            }

            var regex = new Regex("^#[0-9a-f]{6}$");
            var hclValid = regex.Match(hcl).Success;

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
