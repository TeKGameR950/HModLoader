using System;
using System.IO;
using System.Reflection;

namespace Launcher
{
    public static class FileManager
    {
        public static string folderMain = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HModLoader");
        public static string folderBinaries = Path.Combine(folderMain, "binaries");
        public static string folderData = Path.Combine(folderMain, "data");
        public static string folderData_games = Path.Combine(folderData, "games");
        public static string folderCache = Path.Combine(folderMain, "cache");
        public static string folderCache_games = Path.Combine(folderCache, "games");
        public static string folderCache_installfiles = Path.Combine(folderCache, "installfiles");
        public static string configFile = Path.Combine(folderMain, "config.ini");
        public static string dataFile = Path.Combine(folderMain, "data.json");

        public static void CreateDirectories()
        {
            Directory.CreateDirectory(folderMain);
            Directory.CreateDirectory(folderBinaries);
            Directory.CreateDirectory(folderCache);
            Directory.CreateDirectory(folderCache_games);
            Directory.CreateDirectory(folderCache_installfiles);
            Directory.CreateDirectory(folderData);
            Directory.CreateDirectory(folderData_games);
        }

        public static byte[] GetEmbeddedFile(string filename)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"Launcher.{filename}"))
            {
                byte[] assemblyData = new byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return assemblyData;
            }
        }
    }
}
