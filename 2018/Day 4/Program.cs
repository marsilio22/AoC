using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_4
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("./input.txt");
            
            var lines2 = lines.Select(l => l.Split(']'));
            List<(DateTime messageTime, string message)> lines3 = lines2.Select(l => (DateTime.Parse(l[0].Split('[')[1]), l[1].Trim())).OrderBy(l => l.Item1).ToList();

            var guards = new List<Guard>();
            var firstMinuteAsleep = 0;
            Guard currentGuard = new Guard("0");
            foreach(var line in lines3){
                if (line.message.EndsWith("begins shift")){
                    var id = line.message.Split(' ')[1];
                    currentGuard = guards.FirstOrDefault(g => g.Id.Equals(id));
                    if (currentGuard == null){
                        currentGuard = new Guard(id);
                        guards.Add(currentGuard);
                    }
                }
                else if (line.message.Equals("falls asleep"))
                {
                    firstMinuteAsleep = line.messageTime.Minute;
                }
                else if (line.message.Equals("wakes up"))
                {
                    for (int i = firstMinuteAsleep; i< line.messageTime.Minute; i++)
                    currentGuard.MinuteAsleepFrequency[i]++;
                }
            }

            Guard winner = guards.OrderByDescending(g => g.TotalTimeAsleep).First();
            Console.WriteLine("Part 1");
            Console.WriteLine(winner.Id);

            foreach(var thing in winner.MinuteAsleepFrequency.OrderByDescending(m => m.Value)){
                Console.WriteLine($"Key {thing.Key}, Value {thing.Value}");
            }

            Console.WriteLine("Part 2");

            var result = new Dictionary<string, (int minute, int frequency)>();
            foreach(var guard in guards){
                result[guard.Id] = 
                    (guard.MinuteAsleepFrequency.Aggregate((x, y) => x.Value > y.Value ? x : y).Key, 
                     guard.MinuteAsleepFrequency.Aggregate((x, y) => x.Value > y.Value ? x : y).Value);
            }
            var ordered = result.OrderBy(r => r.Value.frequency);

            foreach(var thing in ordered){
                Console.WriteLine($"Id {thing.Key}, Most Asleep Minute {thing.Value.minute}, Frequency {thing.Value.frequency}");
            }
        }
    }

    public class Guard{
        public string Id {get;}
        public int TotalTimeAsleep => this.MinuteAsleepFrequency.Values.Sum();
        public Dictionary<int, int> MinuteAsleepFrequency { get; set; }

        public Guard(string id){
            this.Id = id;
            this.MinuteAsleepFrequency = new Dictionary<int, int>();
            for (int i = 0; i < 60; i++)
            {
                this.MinuteAsleepFrequency[i] = 0;
            }
        }
    }
}
