using System;
using System.Drawing;
using System.Text.RegularExpressions;
using RT.Serialization;
using RT.Util.ExtensionMethods;

namespace ZiimHelper
{
    sealed class ColorClassifyOptions : IClassifySubstitute<Color, string>
    {
        public string ToSubstitute(Color instance)
        {
            return "#{0:X2}{1:X2}{2:X2}{3}".Fmt(instance.R, instance.G, instance.B, instance.A == 255 ? null : ":{0:X2}".Fmt(instance.A));
        }

        public Color FromSubstitute(string instance)
        {
            if (instance == null)
                return Color.Black;
            var match = Regex.Match(instance, @"^#([0-9a-f]{2})([0-9a-f]{2})([0-9a-f]{2})(:([0-9a-f]{2}))?$", RegexOptions.IgnoreCase);
            if (!match.Success)
                return Color.Black;
            return Color.FromArgb(
                match.Groups[4].Success ? Convert.ToInt32(match.Groups[5].Value, 16) : 255,
                Convert.ToInt32(match.Groups[1].Value, 16),
                Convert.ToInt32(match.Groups[2].Value, 16),
                Convert.ToInt32(match.Groups[3].Value, 16)
            );
        }
    }
}
