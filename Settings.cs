using System.IO;

namespace MyCodeGenerator
{
    public static class Settings
    {
        public static DirectoryInfo RootDirectory { get; set; }
        public static string ProjectName { get; set; }
        public static string ProjectNamespace { get; set; }
    }
}
