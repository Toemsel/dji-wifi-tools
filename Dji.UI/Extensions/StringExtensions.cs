using System.Linq;

namespace Dji.UI.Extensions
{
    public static class StringExtensions
    {
        public static bool IsValidHexString(this string value)
        {
            string[] hexValues = value.Trim().Split(' ');

            return !(hexValues.Any(h => h.Length != 4) || hexValues.Any(h => h[1] != 'X' && h[1] != 'x') ||
                hexValues.Any(h => h[0] != '0') || hexValues.Any(h => !h[2].IsHex() || !h[3].IsHex()));
        }

    }
}