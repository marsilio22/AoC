var input = File.ReadAllLines("./input");

var visibleTrees = 0;

var visibleFromBestTree = 0;

for (int j = 0; j < input.Length; j++)
{
    if (j == 0 || j == input.Length - 1)
    {
        visibleTrees += input[j].Length;
    }
    else
    {
        var row = input[j];

        for (int i = 0; i < row.Length; i++)
        {
            if (i == 0 || i == row.Length - 1)
            {
                visibleTrees++;
            }
            else
            {
                var column = input.Select(l => int.Parse(l[i].ToString())).ToArray();

                var currentHeight = int.Parse(row[i].ToString());
                
                var left = row.Substring(0, i).Select(c => int.Parse(c.ToString())).ToArray();
                var right = row.Substring(i+1).Select(c => int.Parse(c.ToString())).ToArray();
                var above = column.Take(j).ToArray();
                var below = column.TakeLast(input.Length - j - 1).ToArray();

                // reverse these to make part2 a bit easier
                left = left.Reverse().ToArray();
                above = above.Reverse().ToArray();

                if (left.All(t => t < currentHeight) ||
                    right.All(t => t < currentHeight) ||
                    above.All(t => t < currentHeight) ||
                    below.All(t => t < currentHeight))
                {
                    visibleTrees++;
                }

                // part 2
                var visibleFromHere = 1;
                var treeDirections = new [] {left.ToList(), right.ToList(), above.ToList(), below.ToList()};
                foreach(var coll in treeDirections)
                {
                    var factor = 0;

                    for (int k = 0; k < coll.Count; k++)
                    {
                        if (coll[k] < currentHeight)
                        {
                            factor++;
                        }
                        else if (coll[k] >= currentHeight)
                        {
                            factor++;
                            break;
                        }
                    }

                    visibleFromHere *= factor;
                }

                if (visibleFromHere > visibleFromBestTree)
                {
                    visibleFromBestTree = visibleFromHere;
                }
            }
        }
    }
}

Console.WriteLine(visibleTrees);
Console.WriteLine(visibleFromBestTree);