using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Day4
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> lines = File.ReadAllLines("./input.txt").ToList();

            List<Room> rooms = new List<Room>();
            foreach(var line in lines)
            {
                rooms.Add(new Room(line));
            }

            var validRooms = rooms.Where(r => r.IsValid()).ToList();

            Console.WriteLine(validRooms.Sum(r => r.SectorId));

            foreach(var validRoom in validRooms)
            {
                var str = new StringBuilder();

                foreach (char character in validRoom.Name)
                {
                    if (character == '-')
                    {
                        str.Append(' ');
                    }
                    else 
                    {
                        str.Append((char)((character - 'a' + (validRoom.SectorId % 26)) % 26 + 'a'));
                    }
                }

                var roomname = str.ToString();
                if (roomname.Contains("north", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine(roomname);
                    Console.WriteLine(validRoom.SectorId);
                }
            }
        }
    }

    public class Room 
    {
        public string Name { get; set; }
        
        public int SectorId { get; set; }
        
        public string Checksum { get; set; }
        
        public Room(string line)
        {
            ICollection<string> splitLine = line.Split('-').ToList();

            var sumAndId = splitLine.Last();

            splitLine.Remove(sumAndId);

            var sumAndIdSplit = sumAndId.Split('[');

            Name = string.Join('-', splitLine);
            SectorId = int.Parse(sumAndIdSplit[0]);
            Checksum = sumAndIdSplit[1].Substring(0, sumAndIdSplit[1].Length - 1);
        }

        public bool IsValid()
        {
            var LetterCounts = new Dictionary<char, int>();

            LetterCounts = this.Name.Where(n => n != '-').Select(n => n).Distinct().ToDictionary(c => c, c => Name.Count(n => n == c));

            var LetterCountsOrdered = LetterCounts.OrderByDescending(d => d.Value).ThenBy(d => d.Key).ToList();

            var fullChecksum = string.Join(string.Empty, LetterCountsOrdered.Select(l => l.Key));

            if (fullChecksum.StartsWith(Checksum))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
