using System.IO;

namespace Codestellation.DarkFlow.Tests
{
    public static class Utils
    {
        public static void SafeDeleteDirectory(string directory)
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }
        }
    }
}