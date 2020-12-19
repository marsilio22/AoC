using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Day_19
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadLines("./input.txt").ToList();

            var rules = new List<Rule>();
            var messages = new List<string>();

            int i = 0;
            while (lines[i].Any())
            {
                var line = lines[i];
                var split = line.Split(": ");

                if (line.Contains('"'))
                {
                    rules.Add(new Rule 
                    {
                        Number = int.Parse(split[0]), 
                        Value = split[1][1].ToString()
                    });
                }
                else if (line.Contains('|'))
                {
                    rules.Add(new Rule 
                    {
                        Number = int.Parse(split[0]), 
                        SubRules = split[1].Split(" | ").Select(s => s.Split(' ').Select(t  => int.Parse(t)).ToList()).ToList()
                    });
                }
                else 
                {
                    rules.Add(new Rule 
                    {
                        Number = int.Parse(split[0]), 
                        SubRules = new List<List<int>>{split[1].Split(' ').Select(t  => int.Parse(t)).ToList()}
                    });

                }
                i++;
            }

            i++;

            while(i < lines.Count)
            {
                var line = lines[i];
                messages.Add(line);
                i++;
            }

            var rule0 = rules.Single(r => r.Number == 0);
            var res = Recurse(rule0, rules);
            res = "^" + res + "$";
            var reg = new Regex(res);

            var count = 0;
            foreach(var message in messages){
                if (reg.Match(message).Success)
                {
                    count++;
                }
            }
            Console.WriteLine(count);

            // NB: the original rules 8 and 11 are:
            // 8: 42
            // 11: 42 31

            // part 2, replace as follows
            // 8: 42 | 42 8
            // 11: 42 31 | 42 11 31


            // so 11 is now like, 42{n}  31{n}, for some n >= 1
            // and 8 is like  42+

            // so we don't need to actually do the replacement, just alter the regex in the special cases
            // of the rule being 8 or 11, to include the above alternative appendages to the regex
            // TODO make it so this doesn't need a weird "append" variable on the below
            
            res = Recurse(rule0, rules, false);
            res = "^" + res + "$";

            var explodedRes = new List<string>();

            // now we need the two ends of 11 to match on number of occurrences.
            // assume there can be no more than 7 (and if it gives the wrong answer increase that lol)
            // NB, it's possible if 11 were to appear in multiple places that we'd need to 
            // increment a variable for each occurrence, because they needn't all match
            // at the same time.
            // TODO if we care about that:
            //    - handle that by changing the n in the recursive method to increment itself starting at e.g. 'c'
            //    - or just change them out here when we have the whole string, but we'd need to find the right pairs

            for(int j = 1; j < 7; j++)
            {
                // create a set of regexes which have different values for the number of repeats of 11
                // we will increment our count if a string matches any of our regexes (we could join them with pipes and have just one regex)
                var resj = res.Replace("n", j.ToString());
                explodedRes.Add(resj);
            }

            reg = new Regex(string.Join('|', explodedRes));

            count = 0;
            foreach(var message in messages){
                if (reg.Match(message).Success)
                {
                    count++;
                }
            }

            Console.WriteLine(count);
        }

        public static string Recurse(Rule rule, ICollection<Rule> rules, bool part1 = true, string append = "")
        {
            if (!string.IsNullOrEmpty(rule.Value))
            {
                return rule.Value;
            }

            var strings = new List<string>();
            foreach(var subRule in rule.SubRules)
            {
                var str = new StringBuilder();
                foreach(var subRuleRule in subRule)
                {
                    var next = rules.Single(r => r.Number == subRuleRule);
                    if (!part1 && subRuleRule == 8)
                    {
                        // the new rule is equivalent to any number of itself in a row, but more than 1
                        append = "+";
                        str.Append(Recurse(next, rules, part1, append));
                    }
                    else if (!part1 && subRuleRule == 11)
                    {
                        // the rule is equivalent to appending and prepending by n copies of itself where
                        // n >= 1. This will need replacing with an actual integer once the recursion is finished
                        append = "{n}";
                        str.Append(Recurse(next, rules, part1, append));
                    }
                    else
                    {
                        str.Append(Recurse(next, rules, part1));
                        
                        if (!string.IsNullOrEmpty(append)){
                            str.Append(append);
                        }
                    }
                }
                strings.Add(str.ToString());
            }

            return "(" + string.Join('|', strings) + ")"; 
        }        
    }

    public class Rule 
    {
        public int Number {get; set;}
        public List<List<int>> SubRules {get; set;} = new List<List<int>>();
        public string Value {get; set;}

        public override string ToString()
        {
            return this.Number.ToString() + ": " + (string.IsNullOrEmpty(this.Value) ? string.Join(" | ", SubRules.Select(s => string.Join(", ", s))) : Value);
        }
    }
}
