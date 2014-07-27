using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace Crystal2.Utilities
{
    public class ColorHelper
    {
        public static Color ParseHex(string hexCode)
        {
            //http://stackoverflow.com/a/16815300/2263199
            //modified by Amrykid

            int index = hexCode.StartsWith("#") ? -2 : -3;

            var color = new Color();
            if (hexCode.Length >= 9)
            {
                color.A = byte.Parse(hexCode.Substring(1, 2), NumberStyles.AllowHexSpecifier);
                index = hexCode.StartsWith("#") ? -1 : 0;
            }
            else
                color.A = 255;

            color.R = byte.Parse(hexCode.Substring(index + 3, 2), NumberStyles.AllowHexSpecifier);
            color.G = byte.Parse(hexCode.Substring(index + 5, 2), NumberStyles.AllowHexSpecifier);
            color.B = byte.Parse(hexCode.Substring(index + 7, 2), NumberStyles.AllowHexSpecifier);

            return color;
        }
    }
}
