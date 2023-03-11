using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HMLLibrary
{
    public class HLib
    {
#if GAME_RAFT
        public static string path_dataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HModLoader");
        public static string datafoldername = "Raft_Data";
        public static CanUnloadModDelegate CanUnloadMod;
        public static CanLoadModDelegate CanLoadMod;
        public delegate bool CanUnloadModDelegate(string modName, string modVersion);
        public delegate bool CanLoadModDelegate(ModData data);
#elif GAME_GREENHELL
        public static string datafoldername = "GH_Data";
        public static string path_dataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GreenHellModLoader");
        public static CanUnloadModDelegate CanUnloadMod;
        public static CanLoadModDelegate CanLoadMod;
        public delegate bool CanUnloadModDelegate(string modName, string modVersion);
        public delegate bool CanLoadModDelegate(ModData data);
#endif

        public static string dataFolder = Path.Combine(path_dataFolder, "data");
        public static string gamesFolder = Path.Combine(dataFolder, "games");
        // TODO : Use ScriptingSymbols to support various games (or get name from a known file).

        public static string currentGameFolder = Path.Combine(gamesFolder, "raft");

        public static string path_binariesFolder = Path.Combine(currentGameFolder, "binaries");
        public static string path_logsFolder = Path.Combine(currentGameFolder, "logs");
        public static string path_cacheFolder = Path.Combine(currentGameFolder, "cache");
        public static string path_cacheFolder_mods = Path.Combine(path_cacheFolder, "mods");
        public static string path_cacheFolder_textures = Path.Combine(path_cacheFolder, "textures");
        public static string path_cacheFolder_temp = Path.Combine(path_cacheFolder, "temp");
        public static string path_modsFolder = Path.Combine(Application.dataPath, "..\\mods");
        public static string gameLogFile = Path.Combine(path_logsFolder, "coremod.log");
        public static Texture2D missingTexture;
        public static AssetBundle bundle;
    }

}
