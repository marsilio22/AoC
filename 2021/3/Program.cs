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

var oxyCopy = parsed.ToList();
var co2Copy = parsed.ToList();

for (int i=0; i < binLength; i++)
{
    if (oxyCopy.Sum(p => p[i]) < (double)oxyCopy.Count / 2)
    {
        oxyCopy = oxyCopy.Where(p => p[i] == 0).ToList();
    }
    else 
    {
        oxyCopy = oxyCopy.Where(p => p[i] == 1).ToList();
    }

    if (oxyCopy.Count() == 1 && string.IsNullOrEmpty(oxy))
    {
        foreach(var digit in oxyCopy.Single())
        {
            oxy += digit;
        }
    }
    
    // todo method this, or something
    if (co2Copy.Sum(p => p[i]) < (double)co2Copy.Count / 2)
    {
        co2Copy = co2Copy.Where(p => p[i] == 1).ToList();
    }
    else 
    {
        co2Copy = co2Copy.Where(p => p[i] == 0).ToList();
    }

    if (co2Copy.Count() == 1 && string.IsNullOrEmpty(co2))
    {
        foreach(var digit in co2Copy.Single())
        {
            co2 += digit;
        }
    }
}

Console.WriteLine(oxy + ", " + co2);

var oxyDec = Convert.ToInt32(oxy, 2);
var co2Dec = Convert.ToInt32(co2, 2);

Console.WriteLine(oxyDec * co2Dec);