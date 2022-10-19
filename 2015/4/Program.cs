using System.Security.Cryptography;
using System.Text;

var input = "iwrupvqb";

var post = 1;

var md5 = MD5.Create();
while (true)
{
    byte[] bytes = Encoding.ASCII.GetBytes(input + post.ToString());
    byte[] hash = md5.ComputeHash(bytes);

    StringBuilder sb = new StringBuilder();
    for (int i = 0; i < hash.Length; i++)
    {
        sb.Append(hash[i].ToString("X2"));
    }

    var str = sb.ToString();

    if (str.StartsWith("00000"))
    {
        Console.WriteLine(post);
        // break;
    }
    if (str.StartsWith("000000")){
        Console.WriteLine(post);
        break;
    }
    post++;
}
