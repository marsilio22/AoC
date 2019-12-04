using System;
using System.Collections.Generic;
using System.Linq;

namespace Day_4 {
    class Program {
        static void Main (string[] args) {
            var validPasswords = new List<string> ();
            // input of a range of 256310-732736
            for (int integerPassword = 256310; integerPassword <= 732736; integerPassword++) {

                var stringPassword = integerPassword.ToString ();

                for (int i = 0; i < 10; i++) {
                    var ichar = i.ToString ();
                    var indexesOfI = AllIndexesOf (stringPassword, ichar);

                    // Password must contain a repeated digit
                    bool passwordContainsRepeatedCharacter = false;
                    for (int j = 0; j < indexesOfI.Count - 1; j++) {
                        if (indexesOfI[j] == indexesOfI[j + 1] - 1) {
                            passwordContainsRepeatedCharacter = true;
                            break;
                        }
                    }

                    // Password must be non-strictly increasing
                    bool passwordIsIncreasing = true;
                    for (int j = 0; j < stringPassword.Length - 1; j++) {
                        if (int.Parse (stringPassword[j].ToString ()) > int.Parse (stringPassword[j + 1].ToString ())) {
                            passwordIsIncreasing = false;
                            break;
                        }
                    }

                    // Password must contain only a pair of the repeated character. (part 2)
                    bool exactly2RepeatedCharacters = indexesOfI.Count == 2;

                    // Determine whether password is valid, and add it to the valid passwords
                    // NB. This is inside the loop for i, so if a password has multiple repeated characters and is valid,
                    // e.g. 112233, then it will be added multiple times.
                    if (passwordContainsRepeatedCharacter && passwordIsIncreasing && exactly2RepeatedCharacters) {
                        validPasswords.Add (stringPassword);
                    }
                }
            }

            Console.WriteLine (validPasswords.Distinct ().Count ());
        }

        public static List<int> AllIndexesOf (string str, string value) {
            if (String.IsNullOrEmpty (value)) { throw new ArgumentException ("the string to find may not be empty", "value"); }
            List<int> indexes = new List<int> ();

            for (int index = 0;; index += value.Length) {
                index = str.IndexOf (value, index);
                if (index == -1) {
                    return indexes;
                }
                indexes.Add (index);
            }
        }
    }
}