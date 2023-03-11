using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HML
{
    public static class HML
    {

        public static string appdataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HModLoader");
        public static string dataFolder = Path.Combine(appdataFolder, "data");
        public static string gamesFolder = Path.Combine(dataFolder, "games");
        // TODO : Use ScriptingSymbols to support various games (or get name from a known file).
        public static string currentGameFolder = Path.Combine(gamesFolder, "sotf");
        public static string binariesFolder = Path.Combine(currentGameFolder, "binaries");

        public static void CreateDirectories()
        {
            Directory.CreateDirectory(appdataFolder);
            Directory.CreateDirectory(dataFolder);
            Directory.CreateDirectory(gamesFolder);
            Directory.CreateDirectory(currentGameFolder);
            Directory.CreateDirectory(binariesFolder);
        }
    }
}
