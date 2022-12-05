using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

var salt = "yjdafjpo";
// var salt = "abc";

var regex3 = new Regex("(.)\\1{2,}");

var part2 = true;

using var md5hash = MD5.Create();

var hashes = new List<string>();

for (int i = 0; i < 35_000; i++)
{
    var sourceBytes = Encoding.UTF8.GetBytes(salt + i);

    var hashBytes = md5hash.ComputeHash(sourceBytes);

    if (part2)
    {
        for (int k = 0; k < 2016; k++)
        {
            sourceBytes = Encoding.UTF8.GetBytes(BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLower());
            hashBytes = md5hash.ComputeHash(sourceBytes);
        }
    }

    var hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLower();

    hashes.Add(hash);
}

Console.WriteLine();

var keys = new List<string>();

for (int i = 0; i < hashes.Count; i++)
{
    var match = regex3.Match(hashes[i]);
    if (match.Success)
    {
        var character = match.Value[0];
        var regex5 = new Regex($"({character})\\1{{4,}}");

        for (int j = 1; j < 1000; j++)
        {
            var match2 = regex5.Match(hashes[i+j]);

            if (match2.Success)
            {
                keys.Add(hashes[i]);
                if (keys.Count == 64)
                {
                    Console.WriteLine($"index: {i}");
                }
                break;
            }
        }
    }

    if (keys.Count == 64)
    {
        break;
    }
}