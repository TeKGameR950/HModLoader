using HMLLibrary;
using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace AssemblyLoader
{
    public class HLoader
    {
        public static bool SAFEMODE = false;
        public static string SAFEMODE_ARGS = "";
#if GAME_RAFT
        public static string mainClass = "RML_Main";
        public static string website = "https://www.raftmodding.com/";
        public static string modFormat = ".rmod";
        public static string settingsPrefix = "rmlSettings_";
#else
        public static string folderName = "GreenHellModLoader";
        public static string mainClass = "GHML_Main";
        public static string website = "https://www.greenhellmodding.com/";
        public static string modFormat = ".ghmod";
        public static string settingsPrefix = "ghmlSettings_";
#endif

        public static string appdataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HModLoader");
        public static string dataFolder = Path.Combine(appdataFolder, "data");
        public static string gamesFolder = Path.Combine(dataFolder, "games");
        // TODO : Use ScriptingSymbols to support various games (or get name from a known file).

        public static string currentGameFolder = Path.Combine(gamesFolder, "raft");
        public static string binariesFolder = Path.Combine(currentGameFolder, "binaries");
        public static string logsFolder = Path.Combine(currentGameFolder, "logs");

        public static string cacheFolder = Path.Combine(currentGameFolder, "cache");
        public static string tempCacheFolder = Path.Combine(cacheFolder, "temp");
        public static string modsCacheFolder = Path.Combine(cacheFolder, "mods");
        public static string logFile = Path.Combine(logsFolder, "hloader.log");

        public static string pidFile = Path.Combine(currentGameFolder, "pid.txt");
        //public static LauncherConfiguration config;

        public static List<string> embeddedAssemblies = new List<string>()
        {
            "0Harmony.dll",
            "SharpZipLib.dll",
#if GAME_RAFT
            "RTCP.dll",
            "RTCPNetImprovements.dll",
#endif
            "System.ValueTuple.dll",
            "Mono.Cecil.dll"
        };


        public async static void Load()
        {
            await CreateDirectories();
            HLogger.Clear();
            try
            {

                await LoadEmbeddedAssemblies();
                AddCompilerReferences();
                HCompiler.Main.Initialize();
                // Add refs from binaries.

                //config = GetLauncherConfiguration();
                StartModLoader();
            }
            catch (Exception ex)
            {
                HLogger.LogError(ex.ToString());
            }
        }

        public static void AddCompilerReferences()
        {
            List<string> references = new List<string>();

            if (Directory.Exists(Path.Combine(Application.dataPath, "Managed")))
                references.AddRange(Directory.GetFiles(Path.Combine(Application.dataPath, "Managed"), "*.dll"));
            if (Directory.Exists(HLib.currentGameFolder))
                references.AddRange(Directory.GetFiles(HLib.currentGameFolder, "*.dll"));
            if (Directory.Exists(Path.Combine(HLib.currentGameFolder, "binaries")))
                references.AddRange(Directory.GetFiles(Path.Combine(HLib.currentGameFolder, "binaries"), "*.dll"));
            RoslynCSharp.RoslynCSharp.settings.references = references;
        }

        public static bool StartedWithLauncher()
        {
            try
            {
                Directory.CreateDirectory(appdataFolder);
                HLogger.Log("Verifying if game has been started with HML...");
                string potentialPid = File.ReadAllText(pidFile);
                HLogger.Log("Found PID : " + potentialPid);
                File.WriteAllText(pidFile, "");
                if (string.IsNullOrWhiteSpace(potentialPid)) return false;
                HLogger.Log("Searching process by PID...");
                Process launcher = Process.GetProcessById(int.Parse(potentialPid));
                HLogger.Log(launcher != null ? ("Found process " + launcher + " matching PID !") : "Could not find process with PID !");
                if (launcher != null && !launcher.HasExited)
                {
                    HLogger.Log("Valid process ! Initializing HML...");
                    return true;
                }
                else
                {
                    HLogger.LogWarning("Invalid process ! (Process : " + launcher + ", HasExited : " + launcher.HasExited + ")");
                }
            }
            catch (Exception ex)
            {
                HLogger.LogError("An exception occured in the PID checking method ! Stacktrace : " + ex.ToString());
            }
            return false;
        }

        public static async Task CreateDirectories()
        {
            Directory.CreateDirectory(binariesFolder);
            Directory.CreateDirectory(cacheFolder);
            Directory.CreateDirectory(tempCacheFolder);
            Directory.CreateDirectory(modsCacheFolder);
            await Task.Delay(1);
        }

        public static async void StartModLoader()
        {
            try
            {
#if GAME_RAFT
                while (!ComponentManager<Settings>.Value)
                    await Task.Delay(1);
#else
                // Find a static field to check for
                await Task.Delay(1000);
#endif
                string dllFile = Path.Combine(currentGameFolder, "HMLCore_Raft.dll");
                if (!GameObject.Find("HyTeKModLoader"))
                {
                    Assembly assembly = Assembly.Load(File.ReadAllBytes(dllFile));
                    HLogger.Log("Loaded assembly \"" + assembly + "\" !");
                    GameObject gameObject = new GameObject("HyTeKModLoader");
                    UnityEngine.Object.DontDestroyOnLoad(gameObject);
                    gameObject.AddComponent(assembly.GetType(mainClass));
                    HLogger.Log("Successfully loaded HML !");
                }
            }
            catch (Exception e)
            {
                HLogger.LogError("An error occured while loading HML ! ( " + e.Message + " ) Stacktrace : " + e.StackTrace.ToString());
            }
        }

        public static async Task LoadEmbeddedAssemblies()
        {
            embeddedAssemblies.ForEach(async assemblyName =>
            {
                try
                {
                    byte[] ba = GetAssemblyBytes("AssemblyLoader.Dependencies." + assemblyName);
                    Assembly asm = Assembly.Load(ba);
                    File.WriteAllBytes(Path.Combine(binariesFolder, assemblyName), ba);
                    //HLogger.Log("Loaded assembly "+asm.FullName);
                }
                catch (Exception e)
                {
                    HLogger.LogError("An error occured while extracting the embedded assembly \"" + assemblyName + "\" ( " + e.Message + " ) Stacktrace : " + e.StackTrace.ToString());
                }
                await Task.Delay(1);
            });
        }

        public static byte[] GetAssemblyBytes(string embeddedResource)
        {
            try
            {
                byte[] ba = null;
                Assembly curAsm = Assembly.GetExecutingAssembly();

                using (Stream stm = curAsm.GetManifestResourceStream(embeddedResource))
                {
                    if (stm == null)
                        throw new Exception(embeddedResource + " could not be found in embedded resources !");

                    ba = new byte[(int)stm.Length];
                    stm.Read(ba, 0, (int)stm.Length);
                    if (ba.Length > 1000)
                    {
                        return ba;
                    }
                }
            }
            catch { }
            return null;
        }
        /*
        public static LauncherConfiguration GetLauncherConfiguration()
        {
            LauncherConfiguration config = new LauncherConfiguration();
            try
            {
                var parser = new FileIniDataParser();

                IniData data = parser.ReadFile(configFile);
                config.gamePath = data["launcher"]["gamePath"];
                config.coreVersion = data["launcher"]["coreVersion"];
                config.agreeWithTOS = int.Parse(data["launcher"]["agreeWithTOS"]);
                config.skipSplashScreen = bool.Parse(data["launcher"]["skipSplashScreen"]);
                config.startGameFromSteam = bool.Parse(data["launcher"]["startGameFromSteam"]);
                config.branch = data["launcher"]["branch"];
            }
            catch { }
            return config;
        }*/
    }

    public class HLogger
    {
        public static void Clear()
        {
            if (File.Exists(HLoader.logFile))
                File.WriteAllText(HLoader.logFile, "");
        }
        public static void Log(object o) => File.AppendAllText(HLoader.logFile, o.ToString() + "\n");
        public static void LogWarning(object o) => File.AppendAllText(HLoader.logFile, "[Warning] " + o.ToString() + "\n");
        public static void LogError(object o) => File.AppendAllText(HLoader.logFile, "[Error] " + o.ToString() + "\n");
    }
}