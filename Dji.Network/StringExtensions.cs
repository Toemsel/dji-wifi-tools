using System.IO;

namespace Dji.Network
{
    public static class StringExtensions
    {
        public static string RemoveFileExtension(this string file)
        {
            var fileInfo = new FileInfo(file);
            if (!string.IsNullOrEmpty(fileInfo.Extension))
                file = Path.Combine(fileInfo.DirectoryName, Path.GetFileNameWithoutExtension(fileInfo.Name));

            return file;
        }
    }
}