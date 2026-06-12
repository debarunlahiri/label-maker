// Author: Debarun Lahiri
// GitHub: https://github.com/debarunlahiri/

using Microsoft.Maui.Graphics;

namespace LabelMaker;

internal static class ColorExtensions
{
    public static string ToRgbHex(this Color color)
    {
        var red = ToByte(color.Red);
        var green = ToByte(color.Green);
        var blue = ToByte(color.Blue);

        return $"#{red:X2}{green:X2}{blue:X2}";
    }

    private static int ToByte(float channel)
    {
        return Math.Clamp((int)Math.Round(channel * 255), 0, 255);
    }
}
