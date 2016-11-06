using System;
using System.IO;

namespace MyCodeGenerator
{
    public static class Settings
    {
        static Settings()
        {
            ProjectName = "";
            ProjectNamespace = "";
            ConnectionString = "";
        }

        public static DirectoryInfo RootDirectory { get; set; }
        public static string ProjectName { get; set; }
        public static string ProjectNamespace { get; set; }
        public static string ConnectionString { get; set; }

        private static string FilePath => Path.Combine(Environment.CurrentDirectory, ".Settings");

        public static void Save()
        {
            var filePath = FilePath;
            if (File.Exists(filePath))
                File.Delete(filePath);
            File.Create(filePath).Close();
            var settings =
                string.Format(@"{0}|||{1}|||{2}|||{3}", RootDirectory?.FullName ?? "", ProjectName ?? "", ProjectNamespace ?? "", ConnectionString ?? "");
            File.WriteAllText(filePath,settings);
        }

        public static void Read()
        {
            var filePath = FilePath;
            if(!File.Exists(filePath))
                return;
            var content = File.ReadAllText(filePath);
            var splitted = content.Split(new[] {"|||"}, StringSplitOptions.None);
            if(splitted.Length<4)
                return;
            RootDirectory = new DirectoryInfo(splitted[0]);
            ProjectName = splitted[1];
            ProjectNamespace = splitted[2];
            ConnectionString = splitted[3];
        }
    }
}
