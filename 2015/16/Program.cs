var lines = File.ReadAllLines("./input.txt");

var sues = new List<Sue>();

foreach(var line in lines)
{
    var newLine = line.Replace("Sue ", "");

    var split = newLine.Split(":", 2);

    var sueNumber = int.Parse(split[0]);
    var stuff = split[1].Split(", ");

    var sue = new Sue{number = sueNumber};

    foreach(var thing in stuff)
    {
        var splitThing = thing.Split(": ", StringSplitOptions.TrimEntries);
        int? value = int.Parse(splitThing[1]);

        switch(splitThing[0])
        {
            case "children":
                sue.children = value;
                break;
            case "cats":
                sue.cats = value;
                break;
            case "samoyeds":
                sue.samoyeds = value;
                break;
            case "pomeranians":
                sue.pomeranians = value;
                break;
            case "akitas":
                sue.akitas = value;
                break;
            case "vizslas":
                sue.vizslas = value;
                break;
            case "goldfish":
                sue.goldfish = value;
                break;
            case "trees":
                sue.trees = value;
                break;
            case "cars":
                sue.cars = value;
                break;
            case "perfumes":
                sue.perfumes = value;
                break;
            default: throw new Exception("oh noes");
        }
    }

    sues.Add(sue);
}

var answerP1 = sues.First(s => 
    (s.children == null || s.children == 3) &&
    (s.cats == null || s.cats == 7) &&
    (s.samoyeds == null || s.samoyeds == 2) &&
    (s.pomeranians == null || s.pomeranians == 3) &&
    (s.akitas == null || s.akitas == 0) &&
    (s.vizslas == null || s.vizslas == 0) &&
    (s.goldfish == null || s.goldfish == 5) &&
    (s.trees == null || s.trees == 3) &&
    (s.cars == null || s.cars == 2) &&
    (s.perfumes == null || s.perfumes == 1)
);

Console.WriteLine($"Part 1: {answerP1.number}");

var answerP2 = sues.First(s => 
    (s.children == null || s.children == 3) &&
    (s.cats == null || s.cats > 7) &&
    (s.samoyeds == null || s.samoyeds == 2) &&
    (s.pomeranians == null || s.pomeranians < 3) &&
    (s.akitas == null || s.akitas == 0) &&
    (s.vizslas == null || s.vizslas == 0) &&
    (s.goldfish == null || s.goldfish < 5) &&
    (s.trees == null || s.trees > 3) &&
    (s.cars == null || s.cars == 2) &&
    (s.perfumes == null || s.perfumes == 1)
);

Console.WriteLine($"Part 2: {answerP2.number}");

class Sue
{
    public int number { get; set; }
    public int? children { get; set; }
    public int? cats { get; set; }
    public int? samoyeds { get; set; }
    public int? pomeranians { get; set; }
    public int? akitas { get; set; }
    public int? vizslas { get; set; }
    public int? goldfish { get; set; }
    public int? trees { get; set; }
    public int? cars { get; set; }
    public int? perfumes { get; set; }
}