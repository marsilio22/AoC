using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Day_14
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadLines("./input.txt");

            var mask = "";
            var mems = new Dictionary<long, long>();

            foreach(var line in lines)
            {
                if (line.StartsWith("mask"))
                {
                    mask = line.Split(' ')[2];
                }
                else
                {
                    var split = line.Split(' ');
                    int addr = int.Parse(split[0].Split('[')[1].Split(']')[0]);
                    long num = long.Parse(split[2]);

                    var bin = Convert.ToString(num, 2).PadLeft(36, '0');
                    var sb = new StringBuilder();

                    int i = 0;
                    foreach(var character in mask){
                        if (character != 'X')
                        {
                            sb.Append(character);
                        }
                        else{
                            sb.Append(bin[i]);
                        }
                        i++;
                    }
                    num = Convert.ToInt64(sb.ToString(), 2);

                    mems[addr] = num;
                }
            }

            var ans = mems.Values.Sum();
            Console.WriteLine(ans);



            // reset part 2
            mask = "";
            mems = new Dictionary<long, long>();

            foreach(var line in lines)
            {
                if (line.StartsWith("mask"))
                {
                    mask = line.Split(' ')[2];
                }
                else
                {
                    var split = line.Split(' ');
                    int addr = int.Parse(split[0].Split('[')[1].Split(']')[0]);
                    long num = long.Parse(split[2]);
                    
                    // this time we want to binary-ise the address
                    var bin = Convert.ToString(addr, 2).PadLeft(36, '0');
                    var sb = new StringBuilder();

                    int i = 0;
                    foreach(var character in mask){
                        if (character == '0')
                        {
                            sb.Append(bin[i]);
                        }
                        else if (character == '1')
                        {
                            sb.Append('1');
                        }
                        else {
                            sb.Append('X');
                        }
                        i++;
                    }

                    var BinaryWithFloats = sb.ToString();

                    // total number of value's we'll end up with will be 2^(X's count)
                    var combos = Math.Pow(2, BinaryWithFloats.Count(c => c == 'X'));
                    var addresses = new List<string>{BinaryWithFloats};

                    while (addresses.Count() < combos)
                    {
                        // each time, double the size of the list, and replace the next X with 0 and 1 in each half
                        var nextX = addresses.First().IndexOf('X');
                         
                        var docCombos2 = addresses.Select(c => c.Substring(0, nextX) + '0' + c.Substring(nextX + 1)).ToList();
                        docCombos2.AddRange(addresses.Select(c => c.Substring(0, nextX) + '1' + c.Substring(nextX + 1)).ToList());
                        addresses = docCombos2;
                    }

                    // set the value in each address in memory
                    foreach(var mem in addresses){
                        var addr2 = Convert.ToInt64(mem, 2);
                        mems[addr2] = num;
                    }
                }
            }

            ans = mems.Values.Sum();
            Console.WriteLine(ans);
        }
    }
}
