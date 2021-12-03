var lines = File.ReadAllLines("./input.txt");
var parsed = lines.Select(l => l.ToCharArray().Select(c => int.Parse(c.ToString())).ToList()).ToList();
var binLength = parsed.First().Count();
double half = (double)lines.Count() / 2;

string epsilon = string.Empty; 
string gamma = string.Empty;

for (int i = 0; i < binLength; i++)
{
    if (parsed.Sum(p => p[i]) < half)
    {
        gamma += "0";
        epsilon += "1";
    }
    else
    {
        gamma += "1";
        epsilon += "0";
    }
}

Console.WriteLine(gamma + ", " + epsilon);

var gammaDec = Convert.ToInt32(gamma, 2);
var epsilonDec = Convert.ToInt32(epsilon, 2);

Console.WriteLine(gammaDec * epsilonDec);


var oxy = string.Empty;
var co2 = string.Empty;

var parsedCopy = parsed.ToList();

for (int i=0; i < binLength; i++)
{
    if (parsedCopy.Sum(p => p[i]) < (double)parsedCopy.Count / 2)
    {
        parsedCopy = parsedCopy.Where(p => p[i] == 0).ToList();
    }
    else 
    {
        parsedCopy = parsedCopy.Where(p => p[i] == 1).ToList();
    }

    if (parsedCopy.Count() == 1)
    {
        foreach(var digit in parsedCopy.Single())
        {
            oxy += digit;
        }
        i = binLength;
    }
}

// co2 TODO conglomerate the two.
parsedCopy = parsed.ToList();

for (int i=0; i < binLength; i++)
{
    if (parsedCopy.Sum(p => p[i]) < (double)parsedCopy.Count / 2)
    {
        parsedCopy = parsedCopy.Where(p => p[i] == 1).ToList();
    }
    else 
    {
        parsedCopy = parsedCopy.Where(p => p[i] == 0).ToList();
    }

    if (parsedCopy.Count() == 1)
    {
        foreach(var digit in parsedCopy.Single())
        {
            co2 += digit;
        }
        i = binLength;
    }
}

Console.WriteLine(oxy + ", " + co2);

var oxyDec = Convert.ToInt32(oxy, 2);
var co2Dec = Convert.ToInt32(co2, 2);

Console.WriteLine(oxyDec * co2Dec);