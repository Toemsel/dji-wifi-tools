namespace Dji.UI.Extensions
{
    public static class CharExtensions
    {
        public static bool IsHex(this char c) => (c >= 48 && c <= 57) || (c >= 65 && c <= 70) || (c >= 97 && c <= 102);
    }
}
