var input = File.ReadAllLines("input.txt").First();

var map = new HashSet<(int, int)>();

(int x, int y) santa = (0, 0);

map.Add(santa);

foreach (var character in input)
{
    switch(character)
    {
        case '^':
            santa = (santa.x, santa.y + 1);
            break;
        case '<':
            santa = (santa.x - 1, santa.y);
            break;
        case '>':
            santa = (santa.x + 1, santa.y);
            break;
        case 'v':
            santa = (santa.x, santa.y - 1);
            break;
    }
    map.Add(santa);
}

Console.WriteLine(map.Count());

santa = (0, 0);
(int x, int y) robo = (0, 0);

map = new HashSet<(int, int)>();
map.Add(santa);

for (int i = 0; i < input.Length; i++)
{
    var character = input[i];
    var mover = i % 2;
    switch((character, mover))
    {
        case ('^', 0):
            santa = (santa.x, santa.y + 1);
            break;
        case ('<', 0):
            santa = (santa.x - 1, santa.y);
            break;
        case ('>', 0):
            santa = (santa.x + 1, santa.y);
            break;
        case ('v', 0):
            santa = (santa.x, santa.y - 1);
            break;
        case ('^', 1):
            robo = (robo.x, robo.y + 1);
            break;
        case ('<', 1):
            robo = (robo.x - 1, robo.y);
            break;
        case ('>', 1):
            robo = (robo.x + 1, robo.y);
            break;
        case ('v', 1):
            robo = (robo.x, robo.y - 1);
            break;
    }
    map.Add(santa);
    map.Add(robo);
}

Console.WriteLine(map.Count());