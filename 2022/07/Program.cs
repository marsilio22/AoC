var lines = File.ReadAllLines("./input.txt");

var currentDirectory = "";

var sizes = new Dictionary<string, long>{[currentDirectory] = 0};
var structure = new Dictionary<string, long>{[currentDirectory] = 0};

foreach(var line in lines)
{
    if (line.StartsWith("$ cd"))
    {
        if (line.EndsWith(".."))
        {
            var split = currentDirectory.Split("/").ToList();
            split.RemoveAt(split.Count - 1);

            currentDirectory = string.Join("/", split);
        }
        else if (line.EndsWith("/")) 
        {
            currentDirectory = "";
        }
        else {
            var newFolder = line.Split(" ").Last();

            currentDirectory += $"/{newFolder}";
        }
    }
    else if (!line.StartsWith("$"))
    {
        var split = line.Split(" ");

        if (split[0] == "dir")
        {
            if (!sizes.ContainsKey($"{currentDirectory}/{split[1]}"))
            {
                sizes[$"{currentDirectory}/{split[1]}"] = 0;
            }
        }
        else if (!structure.ContainsKey($"{currentDirectory}/{split[1]}"))
        {
            structure.Add($"{currentDirectory}/{split[1]}", int.Parse(split[0]));

            foreach(var file in sizes)
            {
                if (currentDirectory.StartsWith(file.Key))
                {
                    sizes[file.Key] = file.Value + int.Parse(split[0]);
                }
            }
        }
    }
}

Console.WriteLine(sizes.Values.Where(s => s < 100_000).Sum());
Console.WriteLine();

var totalSize = 70000000;
var remaining = totalSize - structure.Sum(s => s.Value);

Console.WriteLine(sizes.Where(s => s.Value >= (30000000 - remaining)).OrderBy(s => s.Value).First().Value);