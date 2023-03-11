using HMLLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace HMLLibrary
{
    public class ModManager
    {
        public static string cscPath = "";

        public static bool DoesModNeedsToBeRepaired(string content)
        {
            if (content.Contains("using Harmony;") || content.Contains("HarmonyInstance") || content.Contains("HarmonyInstance.Create") || content.Contains("RNotify"))
            {
                return true;
            }
            return false;
        }

        public static string RepairMod(ModData moddata, string content)
        {

            if (content.Contains("using Harmony;"))
            {
                Debug.LogWarning("[ModCompiler] " + moddata.modinfo.modFile.Name + " > The mod is using a deprecated version of <b>Harmony</b> (Harmony Namespace)! Trying to automatically repair it...");
                content = content.Replace("using Harmony;", "using HarmonyLib;");
                Debug.LogWarning("[ModCompiler] " + moddata.modinfo.modFile.Name + " > Replaced <b>using Harmony;</b> with <b>using HarmonyLib;</b> to hopefully repair the mod!");
            }

            if (content.Contains("HarmonyInstance"))
            {
                Debug.LogWarning("[ModCompiler] " + moddata.modinfo.modFile.Name + " > The mod is using a deprecated version of <b>Harmony</b> (HarmonyInstance Type)! Trying to automatically repair it...");
                content = content.Replace("HarmonyInstance.Create", "new Harmony");
                content = content.Replace("HarmonyInstance", "Harmony");
                Debug.LogWarning("[ModCompiler] " + moddata.modinfo.modFile.Name + " > Replaced <b>HarmonyInstance.Create</b> with <b>new Harmony</b> and <b>HarmonyInstance</b> with <b>Harmony</b> to hopefully repair the mod!");
            }

            if (content.Contains("RNotify") || content.Contains("RNotification"))
            {
                Debug.LogWarning("[ModCompiler] " + moddata.modinfo.modFile.Name + " > The mod is using a deprecated version of <b>RNotify</b>! Trying to automatically repair it...");
                content = content.Replace("RNotify", "HNotify");
                content = content.Replace("RNotification", "HNotification");
                Debug.LogWarning("[ModCompiler] " + moddata.modinfo.modFile.Name + " > Replaced <b>RNotify</b> with <b>HNotify</b> and <b>RNotification</b> with <b>HNotification</b> to hopefully repair the mod!");
            }

            Debug.LogWarning("[ModCompiler] " + moddata.modinfo.modFile.Name + " > Repairing done! Compiling...");

            return content;
        }
    }
}

[Serializable]
public class ModData
{
    public JsonModInfo jsonmodinfo = new JsonModInfo();
    public ModInfo modinfo = new ModInfo();
    public long registrationTick;
}

[Serializable]
public class JsonModInfo
{
    public string name = "Default Mod Name";
    public string author = "Unknown";
    public string description = "";
    public string version = "1.0";
    public string license = "GNU AGPLv3";
    public string icon = "";
    public string banner = "";
    public string gameVersion = "1.0";
    public string updateUrl = "";
    public bool isModPermanent = false;
    public bool requiredByAllPlayers = false;
    public List<string> excludedFiles = new List<string>() { "*.csproj", "*.cache", "*.sln" };

    public string GetTemporaryHorribleModUniqueIdentifier()
    {
        return author + ":" + name + "@" + version;
    }
}

[Serializable]
public class ModInfo
{
    public BaseModHandler modHandler;
    public FileInfo modFile;
    public string fileHash;
    public string shortcutFolder;
    public GameObject ModlistEntry;
    public Assembly assembly;
    public string assemblyFile;
    public Dictionary<string, byte[]> modFiles = new Dictionary<string, byte[]>();

    public GameObject goInstance;
    public Mod mainClass;

    public ModStateEnum modState;
    public enum ModStateEnum
    {
        idle,
        running,
        errored,
        compiling,
        needrestart
    }

    public bool isShortcut;

    public Texture2D ModIcon;
    public Texture2D ModBanner;
    public GameObject buttons;
    public GameObject infoBtn;
    public GameObject loadBtn;
    public GameObject unloadBtn;
    public GameObject permanentModWarning;
    public GameObject permanentTooltip;
    public GameObject infoBtnTooltip;
    public GameObject loadBtnTooltip;
    public GameObject unloadBtnTooltip;
    public GameObject versionTooltip;
}

[Serializable]
public class PermanentModsList
{
    public List<string> list = new List<string>();
}
