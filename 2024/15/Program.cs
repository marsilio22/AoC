using System.Text;

var lines = File.ReadAllLines("./input.txt");

var map = new Dictionary<(int x, int y), char>();
var j = 0;
(int x, int y) start = (0, 0);

while (lines[j].Contains("#"))
{
    int i = 0;
    foreach(var character in lines[j])
    {
        if (character == '.')
        {
            //
        }
        else if (character == '@')
        {
            start = (i, j);
        }
        else
        {
            map.Add((i, j), character);
        }

        i++;
    }
    j++;
}

var instr = new StringBuilder();

for (;j < lines.Length; j++)
{
    instr.Append(lines[j]);
}

var instructions = instr.ToString();