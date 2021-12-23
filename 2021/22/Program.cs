using System.Diagnostics;

var lines = File.ReadAllLines("./input.txt");

var outerInstructions = new Dictionary<Cuboid, int>();

foreach (var line in lines)
{
    var split = line.Split(" ");

    int onOff = 0;

    if (split[0].Equals("on"))
    {
        onOff = 1;
    }

    var coords = split[1].Split(",").Select(s => s.Split("=")[1].Split("..")).ToList();

    
    var x1 = int.Parse(coords[0][0]); 
    var x2 = int.Parse(coords[0][1]);

    var y1 = int.Parse(coords[1][0]); 
    var y2 = int.Parse(coords[1][1]); 
    
    var z1 = int.Parse(coords[2][0]); 
    var z2 = int.Parse(coords[2][1]);

    var boid = new Cuboid(Math.Min(x1, x2), Math.Max(x1, x2), Math.Min(y1, y2), Math.Max(y1, y2), Math.Min(z1, z2), Math.Max(z1, z2));

    outerInstructions[boid] = onOff;
}

// for (int p = 1; p < outerInstructions.Count(); p++)
// {
    var instructions = outerInstructions/*.Take(p)*/;

    // brute force for now
    IDictionary < (int x, int y, int z), int > states = new Dictionary < (int, int, int), int > ();

    foreach (var instr in instructions)
    {
        var boid = instr.Key;

        for (int k = Math.Max(-50, boid.minZ); k <= Math.Min(50, boid.maxZ); k++)
        {
            for (int j = Math.Max(-50, boid.minY); j <= Math.Min(50, boid.maxY); j++)
            {
                for (int i = Math.Max(-50, boid.minX); i <= Math.Min(50, boid.maxX); i++)
                {
                    states[(i, j, k)] = instr.Value;
                }
            }
        }
    }

    var p1 = states.Sum(s => s.Value);

    Console.WriteLine(states.Sum(s => s.Value));




    // part 2
    ICollection<Cuboid> onCubes = new List<Cuboid>();
    var loopCount = 0;

    var sw = new Stopwatch();
    sw.Start();

    foreach (var instruction in instructions)
    {
        var intersectors = onCubes.Where(o => o.Intersects(instruction.Key)).ToList();
    
        // Console.WriteLine($"{intersectors.Count} intersectors");
        if (instruction.Value == 1)
        {
            if (intersectors.Count == 0)
            {
                onCubes.Add(instruction.Key);
            }
            else
            {
                // something which is *on* intersects the cube we are about to turn *on*
                foreach(var onCube in intersectors)
                {
                    onCubes.Remove(onCube);

                    var newCubes = Cuboid.SplitIntersection(onCube, instruction.Key, true);
                    
                    foreach(var newCube in newCubes)
                    {
                        var gluable = onCubes.FirstOrDefault(o => o.IsGluable(newCube));
                        if (gluable != null)
                        {
                            onCubes.Remove(gluable);
                            onCubes.Add(Cuboid.Glue(gluable, newCube));
                        }
                        else
                        {                            
                            onCubes.Add(newCube);
                        }
                    }
                }
            }
        }
        else
        {
            // figure out what we're turning off
            foreach(var onCube in intersectors)
            {
                onCubes.Remove(onCube);

                var newCubes = Cuboid.SplitIntersection(onCube, instruction.Key, false);
                
                foreach(var newCube in newCubes)
                {
                    var gluable = onCubes.FirstOrDefault(o => o.IsGluable(newCube));
                    if (gluable != null)
                    {
                        onCubes.Remove(gluable);
                        onCubes.Add(Cuboid.Glue(gluable, newCube));
                    }
                    else
                    {
                        onCubes.Add(newCube);
                    }
                }
            }
        }

        if (instruction.Value == 1)
        {
            onCubes = onCubes.Distinct().ToList();

            // holy hell i hate this.
            while(true)
            {
                bool breakTime = true;

                for (int g = 0; g < onCubes.Count; g++)
                {
                    var thing = onCubes.Skip(g+1).Where(o => o.Intersects(onCubes.ToList()[g])).ToList();
                    if (thing.Any())
                    {
                        var gCube = onCubes.ToList()[g];
                        onCubes.Remove(gCube);
                        onCubes.Remove(thing.First());

                        var intersect = Cuboid.SplitIntersection(gCube, thing.First(), true);

                        foreach(var thing2 in intersect)
                        {
                            onCubes.Add(thing2);
                        }
                        breakTime = breakTime && false;
                    }
                }

                if (breakTime)
                {
                    break;
                }
            }
        }


        onCubes = onCubes.Distinct().ToList();
        loopCount++;
        Console.WriteLine($"loop {loopCount}, {sw.ElapsedMilliseconds}ms, {onCubes.Count()} on cubes" );
    }

    Console.WriteLine(onCubes.Sum(o => o.Volume()));
    var p2 = onCubes.Sum(o => o.Volume());

    if (p1 != p2)
    {
        Console.WriteLine();
    }

//}


class Cuboid
{
    public int minX { get; set; }
    public int maxX { get; set; }
    public int minY { get; set; }
    public int maxY { get; set; }
    public int minZ { get; set; }
    public int maxZ { get; set; }

    public Cuboid(int x1, int x2, int y1, int y2, int z1, int z2)
    {
        this.minX = x1;
        this.minY = y1;
        this.minZ = z1;
        this.maxX = x2;
        this.maxY = y2;
        this.maxZ = z2;
    }

    public long Volume()
    {
        return (long)(maxX - minX + 1) * (long)(maxY - minY + 1) * (long)(maxZ - minZ + 1);
    }

    public bool Intersects(Cuboid other)
    {
        if ((this.minX >= other.minX && this.minX <= other.maxX || this.maxX >= other.minX && this.maxX <= other.maxX ||
                other.minX >= this.minX && other.minX <= this.maxX || other.maxX >= this.minX && other.maxX <= this.maxX) &&
            (this.minY >= other.minY && this.minY <= other.maxY || this.maxY >= other.minY && this.maxY <= other.maxY ||
                other.minY >= this.minY && other.minY <= this.maxY || other.maxY >= this.minY && other.maxY <= this.maxY) &&
            (this.minZ >= other.minZ && this.minZ <= other.maxZ || this.maxZ >= other.minZ && this.maxZ <= other.maxZ ||
                other.minZ >= this.minZ && other.minZ <= this.maxZ || other.maxZ >= this.minZ && other.maxZ <= this.maxZ))
        {
            return true;
        }

        return false;
    }

    public static ICollection<Cuboid> SplitIntersection(Cuboid first, Cuboid second, bool secondIsOn) // assume first is always on
    {
        // there are 27 possible new cuboids, one for each section if one cuboid is completely contained in the other.
        // most likely some will have zero height/width/depth, so can discard those before returning.
        // need to be careful of off by one errors though, because a 1 high cuboid will probably have minX == maxX or sim.

        if (!first.Intersects(second))
        {
            if (secondIsOn)
            {
                return new [] { first, second};
            }
            else
            {
                return new [] {first};
            }
        }
        else
        {
            var newCubeMinX = 0;
            var newCubeMaxX = 0;
            var newCubeMinY = 0;
            var newCubeMaxY = 0;
            var newCubeMinZ = 0;
            var newCubeMaxZ = 0;

            ICollection<Cuboid> result = new List<Cuboid>();

            for (int k = 0; k < 3; k++)
            {
                switch (k)
                {
                    case 0:
                        newCubeMinZ = Math.Min(first.minZ, second.minZ);
                        newCubeMaxZ = Math.Max(first.minZ, second.minZ) - 1;
                        break;
                    case 1:
                        newCubeMinZ = Math.Max(first.minZ, second.minZ);
                        newCubeMaxZ = Math.Min(first.maxZ, second.maxZ);
                        break;
                    case 2:
                        newCubeMinZ = Math.Min(first.maxZ, second.maxZ) + 1;
                        newCubeMaxZ = Math.Max(first.maxZ, second.maxZ);
                        break;
                    default:
                        throw new Exception("oh noes");
                }

                if (newCubeMinZ > newCubeMaxZ)
                {
                    continue;
                }

                for (int j = 0; j < 3; j++)
                {
                    switch (j)
                    {
                        case 0:
                            newCubeMinY = Math.Min(first.minY, second.minY);
                            newCubeMaxY = Math.Max(first.minY, second.minY) - 1;
                            break;
                        case 1:
                            newCubeMinY = Math.Max(first.minY, second.minY);
                            newCubeMaxY = Math.Min(first.maxY, second.maxY);
                            break;
                        case 2:
                            newCubeMinY = Math.Min(first.maxY, second.maxY) + 1;
                            newCubeMaxY = Math.Max(first.maxY, second.maxY);
                            break;
                        default:
                            throw new Exception("oh noes");
                    }
                    
                    if (newCubeMinY > newCubeMaxY)
                    {
                        continue;
                    }

                    for (int i = 0; i < 3; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                newCubeMinX = Math.Min(first.minX, second.minX);
                                newCubeMaxX = Math.Max(first.minX, second.minX ) - 1;
                                break;
                            case 1:
                                newCubeMinX = Math.Max(first.minX, second.minX);
                                newCubeMaxX = Math.Min(first.maxX, second.maxX);
                                break;
                            case 2:
                                newCubeMinX = Math.Min(first.maxX, second.maxX) + 1;
                                newCubeMaxX = Math.Max(first.maxX, second.maxX);
                                break;
                            default:
                                throw new Exception("oh noes");
                        }

                        if (newCubeMinX > newCubeMaxX)
                        {
                            continue;
                        }
                        else
                        {
                            // the cube is on if
                            // - it was inside the left cube ONLY
                            // - it is inside the right cube and the right cube is on

                            var firstContainsPoint = first.ContainsPoint(newCubeMinX, newCubeMinY, newCubeMinZ) ;
                            var secondContainsPoint = second.ContainsPoint(newCubeMinX, newCubeMinY, newCubeMinZ);

                            bool isOn = firstContainsPoint && !secondContainsPoint
                                        || (secondIsOn && secondContainsPoint);

                            if (!isOn)
                            {
                                continue;
                            }

                            result.Add(new Cuboid(newCubeMinX, newCubeMaxX, newCubeMinY, newCubeMaxY, newCubeMinZ, newCubeMaxZ));
                        }
                    }

                }
            }

            var oldOnCubes = result.ToList();

            // glue cubes together to prevent the list getting too big.
            int q = 0;
            while (q < oldOnCubes.Count)
            {
                var gluableCube = oldOnCubes.FirstOrDefault(o => !o.Equals(oldOnCubes[q]) && o.IsGluable(oldOnCubes[q]));
                if (gluableCube != null)
                {
                    // glue
                    var newCube = Cuboid.Glue(gluableCube, oldOnCubes[q]);

                    // replace in OnCubes
                    oldOnCubes.Remove(oldOnCubes[q]);
                    oldOnCubes.Remove(gluableCube);

                    oldOnCubes.Add(newCube);

                    // reset
                    q = 0;
                    oldOnCubes = oldOnCubes.ToList();
                }
                
                q++;
            }

            return oldOnCubes;
        }
    }

    public bool ContainsPoint(int x, int y, int z)
    {
        return this.minX <= x && this.maxX >= x && this.minY <= y && this.maxY >= y && this.minZ <= z && this.maxZ >= z;
    }

    public bool IsGluable(Cuboid other)
    {
        // 2 dimensions must match
        // the remaining dimension will differ by 1 at one end
        var xMatch = this.minX == other.minX && this.maxX == other.maxX;
        var yMatch = this.minY == other.minY && this.maxY == other.maxY;
        var zMatch = this.minZ == other.minZ && this.maxZ == other.maxZ;

        var xDifferBy1 = Math.Abs(this.minX - other.maxX) == 1 || Math.Abs(this.maxX - other.minX) == 1;
        var yDifferBy1 = Math.Abs(this.minY - other.maxY) == 1 || Math.Abs(this.maxY - other.minY) == 1;
        var zDifferBy1 = Math.Abs(this.minZ - other.maxZ) == 1 || Math.Abs(this.maxZ - other.minZ) == 1;

        return (xMatch && yMatch && zDifferBy1) || (yMatch && zMatch && xDifferBy1) || (xMatch && zMatch && yDifferBy1);
    }

    public static Cuboid Glue(Cuboid first, Cuboid second)
    {
        var xMatch = first.minX == second.minX && first.maxX == second.maxX;
        var yMatch = first.minY == second.minY && first.maxY == second.maxY;
        var zMatch = first.minZ == second.minZ && first.maxZ == second.maxZ;

        
        var newCubeMinX = int.MinValue;
        var newCubeMaxX = int.MinValue;
        var newCubeMinY = int.MinValue;
        var newCubeMaxY = int.MinValue;
        var newCubeMinZ = int.MinValue;
        var newCubeMaxZ = int.MinValue;


        if (xMatch)
        {
            newCubeMinX = first.minX;
            newCubeMaxX = first.maxX;
        }

        if (yMatch)
        {
            newCubeMinY = first.minY;
            newCubeMaxY = first.maxY;
        }

        if (zMatch)
        {
            newCubeMinZ = first.minZ;
            newCubeMaxZ = first.maxZ;
        }

        if (newCubeMinX == int.MinValue)
        {
            newCubeMinX = Math.Min(first.minX, second.minX);
            newCubeMaxX = Math.Max(first.maxX, second.maxX);
        }

        if (newCubeMinY == int.MinValue)
        {
            newCubeMinY = Math.Min(first.minY, second.minY);
            newCubeMaxY = Math.Max(first.maxY, second.maxY);
        }

        if (newCubeMinZ == int.MinValue)
        {
            newCubeMinZ = Math.Min(first.minZ, second.minZ);
            newCubeMaxZ = Math.Max(first.maxZ, second.maxZ);
        }

        var cube = new Cuboid(newCubeMinX, newCubeMaxX, newCubeMinY, newCubeMaxY, newCubeMinZ, newCubeMaxZ);

        return cube;
    }

    public override string ToString()
    {
        return $"[({minX}..{maxX}),({minY}..{maxY}),({minZ}..{maxZ})]";
    }
    
    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (obj.GetType() == this.GetType())
        {
            var typed = ((Cuboid) obj);

            return typed.minX == this.minX &&
                    typed.maxX == this.maxX &&
                    typed.minY == this.minY &&
                    typed.maxY == this.maxY &&
                    typed.minZ == this.minZ &&
                    typed.maxZ == this.maxZ;
        }

        return false;
    }

    public override int GetHashCode()
    {
        unchecked // Allow arithmetic overflow, numbers will just "wrap around"
        {
            int hashcode = 1430287;
            hashcode = hashcode * 7302013 ^ minX.GetHashCode();
            hashcode = hashcode * 7302013 ^ maxX.GetHashCode();
            hashcode = hashcode * 7302013 ^ minY.GetHashCode();
            hashcode = hashcode * 7302013 ^ maxY.GetHashCode();
            hashcode = hashcode * 7302013 ^ minZ.GetHashCode();
            hashcode = hashcode * 7302013 ^ maxZ.GetHashCode();
            return hashcode;
        }
    }
}