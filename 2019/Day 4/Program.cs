using System;
using System.Collections.Generic;
using System.Linq;

namespace Day_4
{
    class Program
    {
        static void Main(string[] args)
        {
            // var start = "256310";
            // var end = "732736";

            var validPasswords = new List<string>();

            for (int i = 256310; i <= 732736; i++){

                var password = i.ToString();

                for (int j = 0; j < 10; j ++)
                {
                    var jchar = j.ToString();
                    var indexesOfJ = AllIndexesOf(password, jchar);
                    bool passwordContainsRepeatedCharacter = false;

                    for (int k = 0; k < indexesOfJ.Count - 1; k++){
                        if (indexesOfJ[k] == indexesOfJ[k+1]-1){
                            passwordContainsRepeatedCharacter = true;
                            break;
                        }
                    }

                    bool passwordIsIncreasing = true;
                    for( int k=0; k < password.Length - 1;  k++)
                    {
                        if (int.Parse(password[k].ToString()) > int.Parse(password[k+1].ToString()))
                        {
                            passwordIsIncreasing = false;
                            break;
                        }
                    }

                    if (passwordContainsRepeatedCharacter && passwordIsIncreasing && indexesOfJ.Count == 2){
                        validPasswords.Add(password);
                    }
                }
            }

            Console.WriteLine(validPasswords.Distinct().Count()); // part 1


        }

        public static List<int> AllIndexesOf(string str, string value) {
            if (String.IsNullOrEmpty(value))
{                throw new ArgumentException("the string to find may not be empty", "value");}
            List<int> indexes = new List<int>();

            for (int index = 0;; index += value.Length) {
                index = str.IndexOf(value, index);
                if (index == -1)
                    {
                        return indexes;
                    }
                indexes.Add(index);
            }
}


    }
}
