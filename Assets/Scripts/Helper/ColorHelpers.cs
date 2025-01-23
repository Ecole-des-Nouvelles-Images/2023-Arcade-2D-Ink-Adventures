using System.Collections.Generic;
using UnityEngine;

namespace Elias.Scripts.Helper
{
    public static class ColorHelpers
    {
        public static bool Match(Color color, Color otherColor)
        {
            // Set alpha to 1 for different alpha comparison
            color.a = 1;
            otherColor.a = 1;
            
            // Color match dictionary
            var colorsDictionary = new Dictionary<(Color, Color), bool>
            {
                { (Color.red, Color.red), true },
                { (Color.red, Color.magenta), true },
                { (Color.red, Color.yellow), true },
                { (Color.blue, Color.blue), true },
                { (Color.blue, Color.cyan), true },
                { (Color.blue, Color.magenta), true },
                { (Color.green, Color.green), true },
                { (Color.green, Color.yellow), true },
                { (Color.green, Color.cyan), true },
                { (Color.yellow, Color.yellow), true },
                { (Color.magenta, Color.magenta), true },
                { (Color.cyan, Color.cyan), true }
                
            };

            return colorsDictionary.ContainsKey((color, otherColor)) ||
                   colorsDictionary.ContainsKey((otherColor, color));
        }
    }
}