using System.Text;

var line = File.ReadAllLines("./input.txt")[0];

var i = line.IndexOf('(');

while (i < line.Length && i != -1)
{
    if (line[i] == '(')
    {
        var aStr = new StringBuilder();
        var bStr = new StringBuilder();

        line = line.Remove(i, 1); // opening bracket

        while(int.TryParse(line[i].ToString(), out _))
        {
            aStr.Append(line[i]);
            line = line.Remove(i, 1);
        }

        line = line.Remove(i, 1); // x

        while(int.TryParse(line[i].ToString(), out _))
        {
            bStr.Append(line[i]);
            line = line.Remove(i, 1);
        }

        line = line.Remove(i, 1); // closing bracket

        var a = int.Parse(aStr.ToString());
        var b = int.Parse(bStr.ToString());

        // a is the number of characters
        // b is the number of repeats

        var repeatedString = line.Substring(i, a);

        var fullString = new StringBuilder();

        for (int j = 0; j < b - 1; j++) // only need b-1 copies as there's already one in the string
        {
            fullString.Append(repeatedString);
        }

        line = line.Insert(i, fullString.ToString());

        i+= a*b;

        i = line.IndexOf('(', i);
    }
}

Console.WriteLine(line.Length);


// now do it the smart way

line = File.ReadAllLines("./input.txt")[0];

var sections = new List<Section>();

sections.Add(new Section{Value = line, Repetitions = 1});

while (sections.Any(v => v.Value.Contains('(')))
{
    var newSections = new List<Section>();

    foreach(var section in sections)
    {
        i = 0;
        while (i < section.Value.Length && i != -1)
        {
            if (section.Value[i] == '(')
            {
                var aStr = new StringBuilder();
                var bStr = new StringBuilder();

                i++; // opening bracket

                while(int.TryParse(section.Value[i].ToString(), out _))
                {
                    aStr.Append(section.Value[i]);
                    i++;
                }

                i++; // x

                while(int.TryParse(section.Value[i].ToString(), out _))
                {
                    bStr.Append(section.Value[i]);
                    i++;
                }

                i++; // closing bracket

                var a = int.Parse(aStr.ToString());
                var b = int.Parse(bStr.ToString());

                // a is the number of characters
                // b is the number of repeats

                var repeatedString = section.Value.Substring(i, a);

                i+= a;
                newSections.Add(new Section{Value = repeatedString, Repetitions = b * section.Repetitions});
            }
            else
            {

                var indexNextBracketWRTi = section.Value.IndexOf('(', i) - i;

                if (indexNextBracketWRTi > 0)
                {
                    newSections.Add(new Section{Value = section.Value.Substring(i, indexNextBracketWRTi), Repetitions = 1* section.Repetitions});
                    i = section.Value.IndexOf('(', i);
                }
                else
                {
                    newSections.Add(new Section{Value = section.Value.Substring(i), Repetitions = 1 * section.Repetitions});
                    i = -1;
                }
            }
        }

        sections = newSections;
    }

    Console.WriteLine(sections.Sum(s => (long)s.Repetitions * s.Value.Length));
}

class Section
{
    public string Value{get;set;}

    public int Repetitions {get;set;}

    public override string ToString()
    {
        return $"{this.Repetitions} * {this.Value}";
    }
}