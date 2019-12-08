using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_8 {
    class Program {
        static void Main (string[] args) {
            // puzzle input
            var input = File.ReadAllLines ("./input.txt") [0];
            var width = 25;
            var height = 6;

            // Separate the input into layers
            List<string> layers = new List<string> ();
            var layerSize = height * width;
            for (int i = 0; i < input.Length; i += layerSize) {
                layers.Add (input.Substring (i, layerSize));
            }

            // Part 1, Calculate what the product of the (number of 1's) and (number of 2's)
            // is on the layer with the fewest 0's
            var fewestZeros = int.MaxValue;
            var currentAnswer = 0;

            foreach (var layer in layers) {
                var array = layer.ToCharArray ();
                if (array.Count (a => a.Equals ('0')) < fewestZeros) {
                    currentAnswer = array.Count (a => a.Equals ('1')) * array.Count (a => a.Equals ('2'));
                    fewestZeros = array.Count (a => a.Equals ('0'));
                }
            }
            Console.WriteLine (currentAnswer);

            // Part 2, Calculate what colour the topmost non-transparent layer is. Print this and read the password
            // Initialise this to the first layer. Slight repeat of work, but probably easiest thing to do.
            List<Colours> image = layers[0].ToCharArray ().Select (l => (Colours) Enum.Parse (typeof (Colours), l.ToString ())).ToList ();

            foreach (var layer in layers) {
                var array = layer.ToCharArray ().Select (l => (Colours) Enum.Parse (typeof (Colours), l.ToString ())).ToList ();

                for (int i = 0; i < layer.Length; i++) {
                    if (image[i] == Colours.Transparent && array[i] != Colours.Transparent) {
                        image[i] = array[i];
                    }
                }
            }

            // Print. Note that the words appear on the White squares. (from observation)
            var index = 0;
            for (int j = 0; j < height; j++) {
                for (int i = 0; i < width; i++) {
                    // Use double width characters to ease readability.
                    Console.Write (image[index] == Colours.White ? "■■" : "  ");
                    index++;
                }
                Console.WriteLine ();
            }
        }

    }
    public enum Colours {
        Black = 0,
        White = 1,
        Transparent = 2
    }
}