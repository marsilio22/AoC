var input = File.ReadAllLines("./input.txt");

var paper = 0;
var ribbon = 0;

foreach (var box in input)
{
    var split = box.Split("x").Select(c => int.Parse(c)).ToList();

    var lw = split[0] * split[1];
    var wh = split[1] * split[2];
    var hl = split[2] * split[0];

    var lwSide = 2 * (split[0] + split[1]);
    var whSide = 2 * (split[1] + split[2]);
    var hlSide = 2 * (split[0] + split[2]);

    var area = 2 * lw + 2 * wh + 2 * hl + Math.Min(lw, Math.Min(wh, hl));

    var ribbonLength = Math.Min(lwSide, Math.Min(whSide, hlSide)) + split[0] * split[1] * split[2];

    paper += area;
    ribbon += ribbonLength;
}

Console.WriteLine(paper);
Console.WriteLine(ribbon);