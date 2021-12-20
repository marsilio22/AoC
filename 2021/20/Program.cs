using System.Text;

var lines = File.ReadAllLines("./input.txt").ToList();

var key = lines[0];

lines = lines.Skip(2).ToList();

var map = new Dictionary<(int x, int y), char>();

int j = 0;
foreach(var line in lines)
{
    int i = 0;
    foreach(var character in line)
    {        
        map[(i, j)] = character;    

        i++;
    }

    j++;
}
var turn = 0;

while (true)
{
    var newMap = new Dictionary<(int x, int y), char>();

    foreach(var node in map)
    {
        for (j = -1; j <= 1; j ++)
        {
            for (int i = -1; i <= 1; i++)
            {
                if (!newMap.ContainsKey((node.Key.x + i, node.Key.y + j)))
                {
                    var str = new StringBuilder();

                    // check whether this node should be lit
                    for(int k = -1; k <= 1; k++)
                    {
                        for(int s = -1; s <= 1; s++)
                        {
                            if (!map.TryGetValue((node.Key.x + i + s, node.Key.y + j + k), out var value))
                            {
                                if (turn % 2 == 0 || key[0] == '.')
                                {
                                    str.Append("0");
                                }
                                else
                                {
                                    str.Append("1");
                                }
                            }
                            else if (value == '#')
                            {
                                str.Append("1");
                            }
                            else
                            {
                                str.Append("0");
                            }
                        }
                    }

                    var character = key[Convert.ToInt32(str.ToString(), 2)];

                    newMap[(node.Key.x + i, node.Key.y + j)] = character;
                    
                }
            }
        }
    }

    map = newMap;
    turn++;
    Console.WriteLine(map.Count(c => c.Value == '#')); // 24009 too high

    if (turn == 50)
    {
        break;
    }
}

class Node 
{
    public char Value { get; set; } = '.';

    public (int x, int y) Position { get; set; }
}