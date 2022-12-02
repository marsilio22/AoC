using System.IO.Compression;

var lines = File.ReadAllLines("./input.txt");

var points = 0;

foreach (var line in lines)
{
    var them = line[0];
    var me = line[2];

    points += me switch
    {
        'X' => 1 + them == 'C' ? 6 : them == 'A' ? 3 : 0,
        'Y' => 2 + them == 'A' ? 6 : them == 'B' ? 3 : 0,
        _ => 3 + them == 'B' ? 6 : them == 'C' ? 3 : 0
    };
}

Console.WriteLine(points);

points = 0;
foreach (var line in lines)
{
    var them = line[0];
    var me = line[2];

    points += me
    switch
    {
        'X' => them switch 
        {
            'A' => 3,
            'B' => 1,
            _ => 2
        },
        'Y' => 3 + them switch
        {
            'A' => 1,
            'B' => 2,
            _ => 3
        },
        _ => 6 + them switch
        {
            'A' => 2,
            'B' => 3,
            _ => 1
        }
    };
}

Console.WriteLine(points);