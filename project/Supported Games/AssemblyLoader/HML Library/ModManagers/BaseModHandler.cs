using AssemblyLoader;
using HarmonyLib;
using Mono.Cecil;
using HCompiler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;
using RoslynCSharp.Compiler;
using Trivial.Mono.Cecil;

namespace HMLLibrary
{
    public enum ExitCode : int
    {
        None = -1,
        Success = 0,
        CompileFailed = 1,
        ParseError = 2,
        ReferenceError = 4
    }

    public class BaseModHandler : MonoBehaviour
    {
        public static Dictionary<string, ModData> dataCache = new Dictionary<string, ModData>();

        public virtual async Task<ModData> GetModData(FileInfo file) { throw new NotImplementedException(); }

        public virtual List<byte[]> GetModReferences(ModData moddata)
        {
            return moddata.modinfo.modFiles.Where(x => x.Key.ToLower().EndsWith(".dll")).Select(x => x.Value).ToList();
        }

        public virtual async void LoadModReferences(ModData moddata)
        {
            if (HLoader.SAFEMODE) return;
            try
            {
                List<byte[]> refs = GetModReferences(moddata);
                if (refs.Count > 0)
                {
                    refs.ForEach(reference =>
                    {
                        // Rename assembly to support hotloading.
                        Stream stream = new MemoryStream(reference);
                        AssemblyDefinition asmDef = AssemblyDefinition.ReadAssembly(stream);
                        string originalName = asmDef.FullName;
                        asmDef.Name = new AssemblyNameDefinition("modRef[" + DateTime.Now.Ticks + "]" + originalName, Version.Parse("1.0.0.0"));
                        string path = Path.Combine(HLib.path_cacheFolder_temp, "modRef" + DateTime.Now.Ticks + ".dll");
                        asmDef.Write(path);
                        // Load the assembly and add to the mod references.
                        try
                        {
                            Assembly asm = Assembly.Load(File.ReadAllBytes(path));
                            if (ModManagerPage.activeModReferences.ContainsKey(moddata.jsonmodinfo.name))
                                ModManagerPage.activeModReferences[moddata.jsonmodinfo.name].Add(asm);
                            else
                                ModManagerPage.activeModReferences.Add(moddata.jsonmodinfo.name, new List<Assembly> { asm });
                        }
                        catch (Exception ex)
                        {
                            Debug.LogWarning("[ModManager] " + moddata.jsonmodinfo.name + "> The reference \"" + originalName + "\" could not be loaded !\n" + ex.ToString());
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("[ModManager] " + moddata.jsonmodinfo.name + "> An error occured when loading the mod reference dll's ! Stacktrace :\n" + ex.ToString());
            }
        }

        public virtual async Task<Assembly> GetModAssembly(ModData moddata)
        {
            if (HLoader.SAFEMODE) return null;
            Assembly assembly = null;

            if (ModManagerPage.loadedAssemblies.ContainsKey(moddata.modinfo.modFile.Name))
                ModManagerPage.loadedAssemblies.Remove(moddata.modinfo.modFile.Name);

            moddata.modinfo.modState = ModInfo.ModStateEnum.compiling;
            ModManagerPage.RefreshModState(moddata);

            Dictionary<string, string> csFiles = new Dictionary<string, string>();
            foreach (KeyValuePair<string, byte[]> csFile in moddata.modinfo.modFiles.Where(x => x.Key.ToLower().EndsWith(".cs")).ToList())
            {
                csFiles.Add(csFile.Key, Encoding.UTF8.GetString(csFile.Value));
            }
            List<byte[]> dllFiles = moddata.modinfo.modFiles.Where(x => x.Key.ToLower().EndsWith(".dll")).Select(x => x.Value).ToList();
            CompilationResult result = await HCompiler.Main.CompileCode(moddata.jsonmodinfo.name, csFiles, dllFiles, false);
            try
            {
                LoadModReferences(moddata);
                assembly = result.OutputAssembly;
            }
            catch (Exception ex)
            {
#if GAME_RAFT
                Debug.LogError("[ModCompiler] " + Settings.VersionNumberText + " " + moddata.modinfo.modFile.Name + " > The mod failed to load!\n" + ex);
#else
                Debug.LogError("[ModCompiler] " + moddata.modinfo.modFile.Name + " > The mod failed to load!\n" + ex);
#endif
                moddata.modinfo.modState = ModInfo.ModStateEnum.errored;
                ModManagerPage.RefreshModState(moddata);
            }
            return assembly;
        }

#if DISABLED
        public virtual async Task<Assembly> GetModAssembly(ModData moddata)
        {
            ExitCode progress = ExitCode.None;
            Assembly assembly = null;

            if (!moddata.modinfo.isShortcut)
            {
                Debug.Log("Checking for a cached version...");
                string potentialCachedVersion = Path.Combine(HLib.path_cacheFolder_mods, moddata.modinfo.fileHash + "_" + Application.version + ".dll");
                if (File.Exists(potentialCachedVersion))
                {
                    try
                    {
                        LoadModReferences(moddata);
                        assembly = Assembly.Load(File.ReadAllBytes(potentialCachedVersion));
                        return assembly;
                    }
                    catch { }
                }
            }

            if (ModManagerPage.loadedAssemblies.ContainsKey(moddata.modinfo.modFile.Name))
            {
                ModManagerPage.loadedAssemblies.Remove(moddata.modinfo.modFile.Name);
            }
            moddata.modinfo.modState = ModInfo.ModStateEnum.compiling;
            ModManagerPage.RefreshModState(moddata);

#if GAME_GREENHELL
            string TempDirectory = Path.Combine(HLib.path_cacheFolder_temp, "tempfix_mod_"+moddata.modinfo.modFile.Name);
            Directory.CreateDirectory(TempDirectory);

            moddata.modinfo.modFiles.ToList().Where(t => t.Key.ToLower().EndsWith(".cs")).ToList().ForEach((modfile) =>
            {
                File.WriteAllBytes(Path.Combine(TempDirectory, Path.GetFileName(modfile.Key)), modfile.Value);
            });

            if (moddata.modinfo.modFiles.ToList().Where(t => t.Key.ToLower().EndsWith(".cs")).ToList().Count == 1)
            {
                var filename = moddata.modinfo.modFiles.ToList().Where(t => t.Key.ToLower().EndsWith(".cs")).First();
                var file = Path.Combine(TempDirectory, Path.GetFileName(filename.Key));
                if (File.Exists(file))
                {
                    string content = File.ReadAllText(file);
                    if (content.Contains("using Harmony;"))
                    {
                        Debug.LogWarning("[ModCompiler] " + moddata.modinfo.modFile.Name + " > The mod is using a deprecated version of <b>Harmony</b> (Harmony Namespace)! Trying to automatically repair it...");
                        content = content.Replace("using Harmony;", "using HarmonyLib;");
                        File.WriteAllText(file, content);
                        if (HUtils.AddFileToMod(moddata, file, filename.Key))
                        {
                            Debug.LogWarning("[ModCompiler] " + moddata.modinfo.modFile.Name + " > Replaced <b>using Harmony;</b> with <b>using HarmonyLib;</b> to hopefully repair the mod! Trying to compile...");
                            string potentialCachedVersion = Path.Combine(HLib.path_cacheFolder_mods, moddata.modinfo.fileHash + ".dll");
                            if (File.Exists(potentialCachedVersion))
                            {
                                File.Delete(potentialCachedVersion);
                            }
                            moddata.modinfo.modFiles[filename.Key] = File.ReadAllBytes(file);
                        }
                    }

                    if (content.Contains("HarmonyInstance"))
                    {
                        Debug.LogWarning("[ModCompiler] " + moddata.modinfo.modFile.Name + " > The mod is using a deprecated version of <b>Harmony</b> (HarmonyInstance Type)! Trying to automatically repair it...");

                        content = content.Replace("HarmonyInstance.Create", "new Harmony");
                        content = content.Replace("HarmonyInstance", "Harmony");
                        File.WriteAllText(file, content);
                        if (HUtils.AddFileToMod(moddata, file, filename.Key))
                        {
                            Debug.LogWarning("[ModCompiler] " + moddata.modinfo.modFile.Name + " > Replaced <b>HarmonyInstance.Create</b> with <b>new Harmony</b> and <b>HarmonyInstance</b> with <b>Harmony</b> to hopefully repair the mod! Trying to compile...");
                            string potentialCachedVersion = Path.Combine(HLib.path_cacheFolder_mods, moddata.modinfo.fileHash + ".dll");
                            if (File.Exists(potentialCachedVersion))
                            {
                                File.Delete(potentialCachedVersion);
                            }
                            moddata.modinfo.modFiles[filename.Key] = File.ReadAllBytes(file);
                        }
                    }
                }
            }
#endif

#if GAME_RAFT
            string TempDirectory = Path.Combine(HLib.path_cacheFolder_temp, "tempfix_mod_" + moddata.modinfo.modFile.Name);
            Directory.CreateDirectory(TempDirectory);
            moddata.modinfo.modFiles.ToList().Where(t => t.Key.ToLower().EndsWith(".cs")).ToList().ForEach((modfile) =>
            {
                File.WriteAllBytes(Path.Combine(TempDirectory, Path.GetFileName(modfile.Key)), modfile.Value);
            });
            if (moddata.modinfo.modFiles.ToList().Where(t => t.Key.ToLower().EndsWith(".cs")).ToList().Count == 1)
            {
                var filename = moddata.modinfo.modFiles.ToList().Where(t => t.Key.ToLower().EndsWith(".cs")).First();
                var file = Path.Combine(TempDirectory, Path.GetFileName(filename.Key));
                if (File.Exists(file))
                {
                    string content = File.ReadAllText(file);
                    if (content.Contains("Semih_Network"))
                    {
                        Debug.LogWarning("[ModCompiler] " + moddata.modinfo.modFile.Name + " > The mod is using the old Semih_Network class! Trying to automatically repair it...");
                        content = content.Replace("Semih_Network", "Raft_Network");
                        File.WriteAllText(file, content);
                        if (HUtils.AddFileToMod(moddata, file, filename.Key))
                        {
                            Debug.LogWarning("[ModCompiler] " + moddata.modinfo.modFile.Name + " > Replaced <b>Semih_Network</b> with <b>Raft_Network</b> to hopefully repair the mod! Trying to compile...");
                            string potentialCachedVersion = Path.Combine(HLib.path_cacheFolder_mods, moddata.modinfo.fileHash + ".dll");
                            if (File.Exists(potentialCachedVersion))
                            {
                                File.Delete(potentialCachedVersion);
                            }
                            moddata.modinfo.modFiles[filename.Key] = File.ReadAllBytes(file);
                        }
                    }

                    /*if (content.Contains("HarmonyInstance"))
                    {
                        Debug.LogWarning("[ModCompiler] " + moddata.modinfo.modFile.Name + " > The mod is using a deprecated version of <b>Harmony</b> (HarmonyInstance Type)! Trying to automatically repair it...");

                        content = content.Replace("HarmonyInstance.Create", "new Harmony");
                        content = content.Replace("HarmonyInstance", "Harmony");
                        File.WriteAllText(file, content);
                        if (HUtils.AddFileToMod(moddata, file, filename.Key))
                        {
                            Debug.LogWarning("[ModCompiler] " + moddata.modinfo.modFile.Name + " > Replaced <b>HarmonyInstance.Create</b> with <b>new Harmony</b> and <b>HarmonyInstance</b> with <b>Harmony</b> to hopefully repair the mod! Trying to compile...");
                            string potentialCachedVersion = Path.Combine(HLib.path_cacheFolder_mods, moddata.modinfo.fileHash + ".dll");
                            if (File.Exists(potentialCachedVersion))
                            {
                                File.Delete(potentialCachedVersion);
                            }
                            moddata.modinfo.modFiles[filename.Key] = File.ReadAllBytes(file);
                        }
                    }*/
                }
            }
#endif

            /*if (HLoader.csc == null || HLoader.csc.HasExited)
            {
                Debug.LogError("[ModCompiler] The mod compiler process is not running! Trying to start csc.exe...");
                HLoader.StartCSC();
            }*/

            DataReceivedEventHandler eventHandler = null;
            string log = "";
            ExitCode exitCode = ExitCode.None;
            eventHandler = (object sender, DataReceivedEventArgs e) =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    if (e.Data == null)
                    {
#if GAME_RAFT
                        Debug.LogError("[ModCompiler] " + Settings.VersionNumberText + " " + moddata.modinfo.modFile.Name + " > The mod failed to compile!\nReceived empty event data from the compiler!");
#else
                        Debug.LogError("[ModCompiler] " + moddata.modinfo.modFile.Name + " > The mod failed to compile!\nReceived empty event data from the compiler!");
#endif
                    }
                    else
                    {
                        if (e.Data.StartsWith(moddata.modinfo.fileHash))
                        {
                            log = Encoding.UTF8.GetString(Convert.FromBase64String(e.Data.Remove(0, moddata.modinfo.fileHash.Length + 1))); // Removes check code and whitespace or \n next to it
                            if (log.Length == 1)
                            {
                                exitCode = (ExitCode)int.Parse(log);
                                if (exitCode == ExitCode.Success)
                                {
                                    try
                                    {
                                        LoadModReferences(moddata);
                                        assembly = Assembly.Load(File.ReadAllBytes(Path.Combine(HLib.path_cacheFolder_mods, moddata.modinfo.fileHash + "_" + Application.version + ".dll")));
                                    }
                                    catch (Exception ex)
                                    {
#if GAME_RAFT
                                        Debug.LogError("[ModCompiler] " + Settings.VersionNumberText + " " + moddata.modinfo.modFile.Name + " > The mod failed to load!\n" + ex);
#else
                                        Debug.LogError("[ModCompiler] " + moddata.modinfo.modFile.Name + " > The mod failed to load!\n" + ex);
#endif
                                        moddata.modinfo.modState = ModInfo.ModStateEnum.errored;
                                        ModManagerPage.RefreshModState(moddata);
                                    }
                                }
                                HLoader.GetCompiler().OutputDataReceived -= eventHandler;
                                progress = exitCode;
                            }
                            else
                            {
#if GAME_RAFT
                                Debug.LogError("[ModCompiler] " + Settings.VersionNumberText + " " + moddata.modinfo.modFile.Name + " > The mod failed to compile!\n" + log);
#else
                                Debug.LogError("[ModCompiler] " + moddata.modinfo.modFile.Name + " > The mod failed to compile!\n" + log);
#endif

                                moddata.modinfo.modState = ModInfo.ModStateEnum.errored;
                                ModManagerPage.RefreshModState(moddata);
                            }
                        }
                    }
                });
            };

            HLoader.GetCompiler().OutputDataReceived += eventHandler;
            HLoader.GetCompiler().StandardInput.WriteLine(string.Format("-s \"{0}\" -n \"{1}\" -o \"{2}\" -r \"{3}\" \"{4}\" -c \"{5}\"",
                moddata.modinfo.modFile, // Input
                moddata.modinfo.fileHash, // Assembly name
                Path.Combine(HLib.path_cacheFolder_mods, moddata.modinfo.fileHash + "_" + Application.version + ".dll"), // Output
                Application.dataPath + "/Managed", // Referenced assemblies
                HLib.path_binariesFolder, // Referenced assemblies
                moddata.modinfo.fileHash)); // Async check string

            int waitingTime = 0;

            while (progress == ExitCode.None && waitingTime < 15000)
            {
                waitingTime += 5;
                await Task.Delay(5);
            }

            HLoader.GetCompiler().OutputDataReceived -= eventHandler;
            return assembly;
        }
#endif
        public virtual async void LoadMod(ModData moddata)
        {
            if (HLoader.SAFEMODE) return;
            try
            {
                if (moddata == null || moddata.modinfo.modState == ModInfo.ModStateEnum.running || moddata.modinfo.modState == ModInfo.ModStateEnum.compiling) { return; }
                if (!HLib.CanLoadMod(moddata))
                {
                    HNotify.Get().AddNotification(HNotify.NotificationType.normal, moddata.jsonmodinfo.name + " can't be loaded while players are online!", 5, await HLib.bundle.TaskLoadAssetAsync<Sprite>("IconError"));
                    return;
                }
                string hash = moddata.modinfo.isShortcut ? HUtils.CreateSHA512ForFolder(moddata.modinfo.shortcutFolder) : HUtils.GetFileSHA512Hash(moddata.modinfo.modFile.FullName);
                bool modHasUpdated = hash != moddata.modinfo.fileHash;
                bool hashesMismatch = false;
                string fileName = moddata.modinfo.modFile.Name.ToLower();
                if (ModManagerPage.lastLoadedFileHash.ContainsKey(fileName))
                {
                    string oldHash = ModManagerPage.lastLoadedFileHash[fileName];
                    if (oldHash != hash)
                        hashesMismatch = true;
                }
                if (hashesMismatch) modHasUpdated = true;
                if (modHasUpdated)
                {
                    await ModManagerPage.RefreshMod(moddata.modinfo.modFile);
                    moddata = ModManagerPage.modList.Find((m) => m.modinfo.modFile.Name == moddata.modinfo.modFile.Name);
                }
                if (!ModManagerPage.ModsGameObjectParent.transform.Find(moddata.modinfo.modFile.Name.ToLower()))
                {
                    if (modHasUpdated || moddata.modinfo.assembly == null) { moddata.modinfo.assembly = await GetModAssembly(moddata); }
                    if (!ModManagerPage.loadedAssemblies.ContainsKey(moddata.modinfo.modFile.Name))
                    {
                        ModManagerPage.loadedAssemblies.Add(moddata.modinfo.modFile.Name, moddata.modinfo.assembly);
                    }
                    else
                    {
                        ModManagerPage.loadedAssemblies[moddata.modinfo.modFile.Name] = moddata.modinfo.assembly;
                    }
                    if (moddata.modinfo.assembly == null) { moddata.modinfo.modState = ModInfo.ModStateEnum.errored; }
                    else
                    {
                        IEnumerable<Type> types = moddata.modinfo.assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Mod)));
                        if (types.Count() != 1)
                        {
                            Debug.LogError("[ModManager] " + moddata.jsonmodinfo.name + "> The mod codebase doesn't specify a mod class or specifies more than one!");
                            moddata.modinfo.modState = ModInfo.ModStateEnum.errored;
                        }
                        else
                        {
                            GameObject ModObj = new GameObject();
                            ModObj.SetActive(false);
                            ModObj.name = moddata.modinfo.modFile.Name.ToLower();
                            ModObj.transform.parent = ModManagerPage.ModsGameObjectParent.transform;
                            moddata.modinfo.mainClass = ModObj.AddComponent(types.FirstOrDefault()) as Mod;
                            ModManagerPage.activeModInstances.Add(moddata.modinfo.mainClass);
                            moddata.modinfo.mainClass.modlistEntry = moddata;
                            ModObj.SetActive(true);
                            moddata.modinfo.goInstance = ModObj;
                            moddata.modinfo.modState = ModInfo.ModStateEnum.running;
                            if (ModManagerPage.lastLoadedFileHash.ContainsKey(moddata.modinfo.modFile.Name.ToLower()))
                                ModManagerPage.lastLoadedFileHash[moddata.modinfo.modFile.Name.ToLower()] = moddata.modinfo.fileHash;
                            else
                                ModManagerPage.lastLoadedFileHash.Add(moddata.modinfo.modFile.Name.ToLower(), moddata.modinfo.fileHash);
                        }
                    }
                }

                if (modHasUpdated)
                {
                    ModManagerPage.RefreshMod(moddata.modinfo.modFile);
                }
                else
                {
                    ModManagerPage.RefreshModState(moddata);
                }
            }
            catch (Exception e)
            {
                moddata.modinfo.modState = ModInfo.ModStateEnum.errored;
#if GAME_RAFT
                Debug.LogError("[ModManager] " + Settings.VersionNumberText + " " + moddata.jsonmodinfo.name + " > A fatal error occured while loading the mod!\nStacktrace : " + e.ToString());
#else
                Debug.LogError("[ModManager] " + moddata.jsonmodinfo.name + " > A fatal error occured while loading the mod!\nStacktrace : " + e.ToString());
#endif
            }
            HConsole.instance.RefreshCommands();
        }

        public virtual async void UnloadMod(ModData moddata)
        {
            if (HLoader.SAFEMODE) return;
            try
            {
                if (moddata == null || moddata.modinfo.modState == ModInfo.ModStateEnum.compiling) { return; }
                if (!HLib.CanUnloadMod(moddata.jsonmodinfo.name, moddata.jsonmodinfo.version))
                {
                    HNotify.Get().AddNotification(HNotify.NotificationType.normal, moddata.jsonmodinfo.name + " is required by the server and can't be unloaded!", 5, await HLib.bundle.TaskLoadAssetAsync<Sprite>("IconError"));
                    return;
                }

                if (moddata.modinfo.modState == ModInfo.ModStateEnum.running)
                {
                    if (ModManagerPage.activeModReferences.ContainsKey(moddata.jsonmodinfo.name))
                        ModManagerPage.activeModReferences.Remove(moddata.jsonmodinfo.name);
                    Transform mod = ModManagerPage.ModsGameObjectParent.transform.Find(moddata.modinfo.modFile.Name.ToLower());
                    try
                    {
                        if (mod != null && moddata.modinfo.assembly != null)
                        {
                            IEnumerable<Type> types = moddata.modinfo.assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Mod)));
                            if (mod.GetComponent(types.First()))
                            {
                                MethodInfo methodInfo = AccessTools.Method(types.First(), "OnModUnload");
                                if (methodInfo != null)
                                    if (methodInfo.IsStatic) { methodInfo.Invoke(null, null); } else { methodInfo.Invoke(mod.GetComponent(types.First()), null); }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("[ModManager] " + moddata.jsonmodinfo.name + " > An error occured while running the mod OnModUnload() method!\nStacktrace : " + e.ToString());
                    }
                    if (mod != null)
                    {
                        Destroy(mod.gameObject);
                    }
                    if (ModManagerPage.loadedAssemblies.ContainsKey(moddata.modinfo.modFile.Name)) { ModManagerPage.loadedAssemblies.Remove(moddata.modinfo.modFile.Name); }
                    moddata.modinfo.modState = ModInfo.ModStateEnum.idle;
                    try
                    {
                        ModManagerPage.activeModInstances = ModManagerPage.activeModInstances.Where(x => x.modlistEntry.modinfo.modFile != moddata.modinfo.modFile).ToList();
                    }
                    catch { }
                    ModManagerPage.RefreshModState(moddata);
                    HConsole.instance.RefreshCommands();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[ModManager] " + moddata.jsonmodinfo.name + " > An error occured while unloading the mod!\nStacktrace : " + e.ToString());
            }
        }
    }
}