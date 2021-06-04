using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Day5
{
    class Program
    {
        static void Main(string[] args)
        {
            // part 1
            var input = "cxdnnyjw";
            int i = 0;
            // string password = "";

            // while(password.Length < 8)
            // {
            //     var md5 = CreateMD5(input + i);
            //     if (md5.StartsWith("00000"))
            //     {
            //         password += md5[5];
            //     }
            //     i++;
            // }

            // Console.WriteLine(password);

            // part 2
            i = 0;
            var passwordPart2 = new Dictionary<int, char>();

            while(passwordPart2.Count < 8)
            {
                var md5 = CreateMD5(input + i);
                if (md5.StartsWith("00000"))
                {
                    if (int.TryParse(md5[5].ToString(), out int pos) && pos >= 0 && pos < 8)
                    {
                        Console.WriteLine("Filling pos " + pos);
                        if (!passwordPart2.ContainsKey(pos))
                        {
                            passwordPart2[pos] = md5[6];
                        }
                    }
                }
                i++;
            }
            var p = string.Join(string.Empty, passwordPart2.OrderBy(p => p.Key).Select(p => p.Value));
            Console.WriteLine(p);
        }

        // from msdn
        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
