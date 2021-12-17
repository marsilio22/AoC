var input = "target area: x=70..96, y=-179..-124";
//input = "target area: x=20..30, y=-10..-5";

// lol what a mess
var xs = input.Split(" ")[2].Split("..").Select(d => int.Parse(d.Split("x=", StringSplitOptions.RemoveEmptyEntries)[0].Split(",", StringSplitOptions.RemoveEmptyEntries)[0])).ToList();
var ys = input.Split(" ")[3].Split("..").Select(d => int.Parse(d.Split("y=", StringSplitOptions.RemoveEmptyEntries)[0])).ToList();

(int x, int y) topLeft = (xs.Min(), ys.Max());
(int x, int y) bottomRight = (xs.Max(), ys.Min());

ICollection<(int v, int turn)> validXVelocities = new List<(int, int)>();
var yDiff = Math.Abs(Math.Abs(ys.Min()) - Math.Abs(ys.Max()));

for (int x = 0; x <= xs.Max(); x++)
{
    var sum = 0;
    var step = 0;

    // need to consider as many extra steps as the square is deep in the y direction
    // because it would take at most that many steps to fall through the square
    for (int j = x; j > -1 * yDiff; j--)
    {
        sum+= Math.Max(0, j);
        step++;
        if (sum <= xs.Max() && sum >= xs.Min())
        {
            validXVelocities.Add((x, step));
        }
    }
}

ICollection<(int v, int turn)> validYVelocities = new List<(int, int)>();

var xDiff = Math.Abs(Math.Abs(xs.Min()) - Math.Abs(xs.Max()));

for (int y = ys.Min(); y < ys.Max(d => Math.Abs(d)); y++)
{
    var pos = 0;

    // don't want to miss 0 y valued stuff, so need to take the max here.
    // Also want to catch when y hits the square late on, so take 20* the y value as arbitrarily long enough
    // bit random but wevs
    var count = Math.Max(20 * Math.Abs(y), xDiff); 

    for (int j = 0; j < count ; j++)
    {
        pos += (y-j);

        if (pos <= ys.Max() && pos >= ys.Min())
        {
            validYVelocities.Add((y, j+1));
        }
    }
}

ICollection<(int x, int y, int turn)> xyVelocitiesThatEndUpInTheSquare = new List<(int, int, int)>();

xyVelocitiesThatEndUpInTheSquare = validXVelocities.SelectMany(x => validYVelocities.Where(y => y.turn == x.turn).Select(y => (x.v, y.v, x.turn))).ToList();

var ans = xyVelocitiesThatEndUpInTheSquare.MaxBy(v => v.y);

Console.WriteLine((ans.y * (ans.y+1)) / 2); 
var distinctStartingVelicities = xyVelocitiesThatEndUpInTheSquare.Select(c => (c.x, c.y)).Distinct().ToList();
Console.WriteLine(distinctStartingVelicities.Count());
Console.WriteLine();