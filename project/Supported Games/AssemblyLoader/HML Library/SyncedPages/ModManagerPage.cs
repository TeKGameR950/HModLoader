using AssemblyLoader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

#if GAME_RAFT
using Text = TMPro.TextMeshProUGUI;
#endif

namespace HMLLibrary
{
    public class ModManagerPage : MenuPage
    {
        public static ModManagerPage instance;
        public static List<ModData> modList = new List<ModData>();
        public static List<Mod> activeModInstances = new List<Mod>();
        public static Dictionary<string, List<Assembly>> activeModReferences = new Dictionary<string, List<Assembly>>();
        public static Dictionary<string, string> lastLoadedFileHash = new Dictionary<string, string>();
        public static Dictionary<string, Assembly> loadedAssemblies = new Dictionary<string, Assembly>();
        public static Transform ModListContent;
        public static GameObject ModEntryPrefab;
        public static Button LoadModsBtn;
        public static Button UnloadModsBtn;
        public static Button ModListRefreshBtn;
        public static GameObject ModsGameObjectParent;
        public static bool bypassValueChangeCheckAll;
        public static GameObject modInfoObj;
        public static GameObject noModsText;

#if GAME_RAFT
        public static Color greenColor = new Color(136f / 255, 216f / 255, 176f / 255);
        public static Color orangeColor = new Color(255f / 255, 204f / 255, 92f / 255);
        public static Color redColor = new Color(255f / 255, 111f / 255, 105f / 255);
        public static Color blueColor = new Color(150f / 255, 185f / 255, 226f / 255);
        public static Color yellowColor = new Color(255f / 255, 238f / 255, 173f / 255);
#else
        public static Color greenColor = new Color(67.0f / 255, 181.0f / 255, 129.0f / 255);
        public static Color orangeColor = new Color(181.0f / 255, 135.0f / 255, 67.0f / 255);
        public static Color redColor = new Color(181.0f / 255, 67.0f / 255, 67.0f / 255);
        public static Color blueColor = new Color(67.0f / 255, 129.0f / 255, 181.0f / 255);
        public static Color yellowColor = new Color(232.0f / 255, 226.0f / 255, 58.0f / 255);
#endif

        public static bool canRefreshModlist = true;

        static int listRefreshAmount = 0;
        public static PermanentModsList UserPermanentMods;
        public static PermanentModsList OnStartUserPermanentMods;

        public static Assembly ModAssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly modAssembly = args.RequestingAssembly;
            string modName = GetModnameFromAssembly(modAssembly);
            string dllName = args.Name.Split(',')[0];
            try
            {
                byte[] assembly = HLoader.GetAssemblyBytes("AssemblyLoader.Dependencies.HCompiler." + dllName + ".dll");
                if (assembly != null && assembly.Length > 0)
                {
                    return Assembly.Load(assembly);
                }
            }
            catch { }
            if (dllName == "Microsoft.CodeAnalysis.CSharp.resources")
                return AppDomain.CurrentDomain.GetAssemblies().ToList().Find(x => x.FullName.Split(',')[0] == "Microsoft.CodeAnalysis.CSharp");
            if (modName != "" && modName != null)
            {
                //Debug.Log("[ModManager] " + modName + " > Retrieving hotloaded reference \"" + dllName + "\"...");
                if (activeModReferences.ContainsKey(modName))
                    foreach (Assembly reference in activeModReferences[modName])
                        if (reference.GetName().Name.EndsWith(args.Name))
                        {
                            //Debug.Log("[ModManager] " + modName + " > Successfully bound \"" + dllName + "\" to \"" + GetCleanAssemblyName(reference) + "\" !");
                            return reference;
                        }
            }
            foreach (Assembly reference in AppDomain.CurrentDomain.GetAssemblies())
            {
                string refName = reference.FullName.Split(',')[0];
                if (refName == dllName)
                {
                    return reference;
                }
                if (reference.GetName().Name.EndsWith(args.Name))
                {
                    //Debug.Log("[ModManager] Successfully bound hotloaded reference \"" + dllName + "\" to \"" + GetCleanAssemblyName(reference) + "\" !");
                    return reference;
                }
            }

            Debug.Log("[ModManager] Could not find an hotloaded reference matching \"" + dllName + "\" for mod \"" + modName + "\"...");
            return null;
        }

        public static string GetCleanAssemblyName(Assembly asm)
        {
            return asm.FullName.Substring(0, asm.FullName.IndexOf(','));
        }

        public static string GetModnameFromAssembly(Assembly asm)
        {
            return modList.Find(x => x.modinfo.assembly == asm)?.jsonmodinfo.name;
        }

        public override async void Initialize()
        {
            instance = this;
            ModsGameObjectParent = new GameObject();
            ModsGameObjectParent.name = "ModsPrefabs";
            DontDestroyOnLoad(ModsGameObjectParent);
            modInfoObj = transform.Find("ModScrollView").Find("Viewport").Find("ModInfo").gameObject;
            modInfoObj.SetActive(false);
            noModsText = transform.Find("ModScrollView").Find("Viewport").Find("HML_FindMods").gameObject;
            noModsText.GetComponent<Button>().onClick.AddListener(() => Application.OpenURL(AssemblyLoader.HLoader.website + "mods"));
            noModsText.SetActive(false);
            ModEntryPrefab = await HLib.bundle.TaskLoadAssetAsync<GameObject>("ModEntry");
            ModListContent = transform.Find("ModScrollView").Find("Viewport").Find("HMLModManager_ModListContent");
            transform.Find("InfoBar").Find("HML_CheckAllToggle").GetComponent<Toggle>().onValueChanged.AddListener(var => SelectAllMods(var));
            LoadModsBtn = transform.Find("HML_LoadModsBtn").GetComponent<Button>();
            LoadModsBtn.onClick.AddListener(LoadSelectedMods);
            UnloadModsBtn = transform.Find("HML_UnloadModsBtn").GetComponent<Button>();
            UnloadModsBtn.onClick.AddListener(UnloadSelectedMods);
            ModListRefreshBtn = transform.Find("Buttons").Find("HML_RefreshModsBtn").GetComponent<Button>();
            ModListRefreshBtn.onClick.AddListener(() => RefreshMods());
            transform.Find("Buttons").Find("HML_OpenModsFolderBtn").GetComponent<Button>().onClick.AddListener(OpenModsFolder);
            RefreshMods();
        }

        public static void OpenModsFolder()
        {
            Process.Start("explorer.exe", Path.GetFullPath(HLib.path_modsFolder));
        }

        public static void RefreshModsStates()
        {
            modList.ForEach(m => RefreshModState(m));
        }

        public static void RefreshModState(ModData md)
        {
            if (md.modinfo.ModlistEntry == null) { return; }
            if (md.modinfo.modState == ModInfo.ModStateEnum.running)
            {
                if (md.modinfo.goInstance != null && md.modinfo.mainClass != null)
                {
                    md.modinfo.mainClass.modlistEntry = md;
                }

                if (!md.jsonmodinfo.isModPermanent)
                {
                    md.modinfo.permanentModWarning?.SetActive(false);
                    md.modinfo.unloadBtn?.SetActive(true);
                    md.modinfo.loadBtn?.SetActive(false);
                    if (OnStartUserPermanentMods.list.Contains(md.modinfo.modFile.Name.ToLower()))
                    {
                        md.modinfo.permanentModWarning.SetActive(true);
#if GAME_GREENHELL
                        ColorBlock block = md.modinfo.permanentModWarning.transform.GetComponent<Button>().colors;
                        block.disabledColor = new Color(84.0f / 255.0f, 166.0f / 255.0f, 87.0f / 255.0f, 200.0f / 255.0f);
                        md.modinfo.permanentModWarning.transform.GetComponent<Button>().colors = block;
#else
                        md.modinfo.permanentModWarning.transform.GetComponent<Image>().sprite = HLib.bundle.LoadAsset<Sprite>("info_bg");
#endif
                        md.modinfo.permanentModWarning.transform.Find("Tooltip").Find("Text").GetComponent<Text>().text = "Loaded at startup";
                    }
                }
                else
                {
                    md.modinfo.unloadBtn?.SetActive(false);
                    md.modinfo.loadBtn?.SetActive(false);
                    md.modinfo.permanentModWarning?.SetActive(true);
                }
            }
            else
            {
                if (!md.jsonmodinfo.isModPermanent)
                {
                    md.modinfo.permanentModWarning.SetActive(false);
                    md.modinfo.unloadBtn.SetActive(false);
                    md.modinfo.loadBtn.SetActive(true);
                    if (OnStartUserPermanentMods.list.Contains(md.modinfo.modFile.Name.ToLower()))
                    {
                        md.modinfo.permanentModWarning.SetActive(true);
#if GAME_GREENHELL
                        ColorBlock block = md.modinfo.permanentModWarning.transform.GetComponent<Button>().colors;
                        block.disabledColor = new Color(84.0f / 255.0f, 166.0f / 255.0f, 87.0f / 255.0f, 200.0f / 255.0f);
                        md.modinfo.permanentModWarning.transform.GetComponent<Button>().colors = block;
#else
                        md.modinfo.permanentModWarning.transform.GetComponent<Image>().sprite = HLib.bundle.LoadAsset<Sprite>("info_bg");
#endif
                        md.modinfo.permanentModWarning.transform.Find("Tooltip").Find("Text").GetComponent<Text>().text = "Loaded at startup";
                    }
                }
                else
                {
                    md.modinfo.unloadBtn?.SetActive(false);
                    md.modinfo.loadBtn?.SetActive(false);
                    if (md.modinfo.modState != ModInfo.ModStateEnum.needrestart)
                        md.modinfo.permanentModWarning.SetActive(true);
                }
            }
            UnityEngine.UI.Text Statustext = md.modinfo.ModlistEntry.transform.Find("ModStatusText").GetComponent<UnityEngine.UI.Text>();
            switch (md.modinfo.modState)
            {
                case ModInfo.ModStateEnum.running:
                    Statustext.text = "Running...";
                    Statustext.color = greenColor;
                    break;
                case ModInfo.ModStateEnum.errored:
                    Statustext.text = "Errored";
                    Statustext.color = redColor;
                    break;
                case ModInfo.ModStateEnum.idle:
                    Statustext.text = "Not loaded";
                    Statustext.color = blueColor;
                    break;
                case ModInfo.ModStateEnum.compiling:
                    Statustext.text = "Compiling...";
                    Statustext.color = yellowColor;
                    break;
                case ModInfo.ModStateEnum.needrestart:
                    Statustext.text = "Needs Game Restart";
                    Statustext.color = redColor;
                    break;
            }
        }

        public async static void LoadSelectedMods()
        {
            foreach (ModData md in modList.ToArray())
            {
                if (md.jsonmodinfo.isModPermanent) { continue; }
                if (md.modinfo.ModlistEntry.GetComponentInChildren<Toggle>().isOn)
                {
                    md.modinfo.modHandler.LoadMod(md);
                }
                await Task.Delay(1);
            }
        }

        public async static void UnloadSelectedMods()
        {
            foreach (ModData md in modList.ToArray())
            {
                if (md.jsonmodinfo.isModPermanent) { continue; }
                if (md.modinfo.ModlistEntry.GetComponentInChildren<Toggle>().isOn)
                {
                    md.modinfo.modHandler.UnloadMod(md);
                }
                await Task.Delay(1);
            }
        }

        public static void SelectAllMods(bool var)
        {
            if (!bypassValueChangeCheckAll)
            {
                foreach (ModData md in modList.ToArray())
                {
                    md.modinfo.ModlistEntry.GetComponentInChildren<Toggle>().isOn = var;
                }
            }
            else
            {
                bypassValueChangeCheckAll = false;
            }
        }

        public static async void RefreshMods()
        {
            if (!canRefreshModlist) { return; }
            canRefreshModlist = false;
            noModsText.SetActive(true);
            foreach (Transform t in ModsGameObjectParent.transform)
            {
                bool found = false;
                foreach (ModData moddata in modList.ToArray())
                {
                    if (moddata.modinfo.modFile.Name.ToLower() == t.gameObject.name)
                    {
                        found = true;
                    }
                }
                if (!found)
                {
                    Destroy(t.gameObject);
                }
            }
            modList.Clear();
            LoadModsBtn.interactable = false;
            UnloadModsBtn.interactable = false;
            ModListRefreshBtn.interactable = false;
            foreach (Transform t in ModListContent.transform)
            {
                Destroy(t.gameObject);
            }
            DirectoryInfo d = new DirectoryInfo(Path.Combine(Application.dataPath, "..\\mods"));
            //List<FileInfo> mods = d.GetFiles("*", SearchOption.AllDirectories).ToList();
            List<FileInfo> mods = d.GetFiles("*", SearchOption.TopDirectoryOnly).ToList();
            foreach (FileInfo file in mods)
            {
                await RefreshMod(file);
            }
            LoadModsBtn.interactable = true;
            UnloadModsBtn.interactable = true;
            ModListRefreshBtn.interactable = true;
            RefreshModsStates();
            listRefreshAmount++;
            canRefreshModlist = true;
        }

        public static List<ModData> TestList = new List<ModData>();
        public static async Task RefreshMod(FileInfo file)
        {
            try
            {
                ModData _md = modList.Where(m => m.modinfo.modFile == file).FirstOrDefault();
                if (_md != null && _md.modinfo.ModlistEntry != null)
                {
                    Destroy(_md.modinfo.ModlistEntry);
                    modList.Remove(_md);
                }
                ModData md = null;
                string filename = file.Name.ToLower();
                string folderName = "mods";
                if (folderName != "mods") { return; }
                if (filename.EndsWith(AssemblyLoader.HLoader.modFormat))
                {
                    md = await new ZipModHandler().GetModData(file);
                }
                else if (filename.EndsWith(".lnk"))
                {
                    md = await new FolderModHandler().GetModData(file);
                }
                else
                {
                    return;
                }

                if (md == null)
                {
                    Debug.LogError("[ModManager] " + file.Name + " > Couldn't load the mod! Invalid ModData!");
                    return;
                }
                md.registrationTick = DateTime.Now.Ticks;
                if (md.jsonmodinfo == null)
                {
                    Debug.LogError("[ModManager] " + file.Name + " > The mod is missing a modinfo.json file!");
                    return;
                }
                md.modinfo.ModlistEntry = Instantiate(ModEntryPrefab, ModListContent.transform.position, ModListContent.transform.rotation, ModListContent.transform);
                md.modinfo.ModlistEntry.GetComponent<RectTransform>().sizeDelta = new Vector2(580, 25);
                md.modinfo.ModlistEntry.GetComponent<RectTransform>().ForceUpdateRectTransforms();

                if (ModsGameObjectParent.transform.Find(md.modinfo.modFile.Name.ToLower()))
                {
                    md.modinfo.modState = ModInfo.ModStateEnum.running;
                }

                md.modinfo.ModlistEntry.transform.Find("ModName").GetComponent<UnityEngine.UI.Text>().text = (md.modinfo.isShortcut ? "{DEV} " : "") + HUtils.StripRichText(md.jsonmodinfo.name);
#if GAME_RAFT
                md.modinfo.ModlistEntry.transform.Find("ModName").GetComponent<UnityEngine.UI.Text>().color = md.modinfo.isShortcut ? new Color(40f / 255f, 199f / 255f, 69f / 255f) : new Color(187f / 255f, 161f / 255f, 106f / 255f);
#else
                md.modinfo.ModlistEntry.transform.Find("ModName").GetComponent<UnityEngine.UI.Text>().color = md.modinfo.isShortcut ? new Color(40f / 255f, 199f / 255f, 69f / 255f) : Color.white;
#endif
                md.modinfo.ModlistEntry.transform.Find("ModAuthor").GetComponent<UnityEngine.UI.Text>().text = HUtils.StripRichText(md.jsonmodinfo.author);
                md.modinfo.ModlistEntry.transform.Find("ModVersionText").GetComponent<UnityEngine.UI.Text>().text = HUtils.StripRichText(md.jsonmodinfo.version);
                md.modinfo.buttons = md.modinfo.ModlistEntry.transform.Find("Buttons").gameObject;
                md.modinfo.infoBtn = md.modinfo.buttons.transform.Find("ModInfoBtn").gameObject;
                md.modinfo.infoBtnTooltip = md.modinfo.buttons.transform.Find("ModInfoBtn").Find("Tooltip").gameObject;
                md.modinfo.infoBtn.AddComponent<TooltipHandler>().tooltip = md.modinfo.infoBtnTooltip;
                md.modinfo.permanentModWarning = md.modinfo.buttons.transform.Find("PermanentModWarning").gameObject;
                md.modinfo.permanentTooltip = md.modinfo.buttons.transform.Find("PermanentModWarning").Find("Tooltip").gameObject;
                md.modinfo.permanentModWarning.AddComponent<TooltipHandler>().tooltip = md.modinfo.permanentTooltip;
                md.modinfo.loadBtn = md.modinfo.buttons.transform.Find("LoadModBtn").gameObject;
                md.modinfo.loadBtnTooltip = md.modinfo.buttons.transform.Find("LoadModBtn").Find("Tooltip").gameObject;
                md.modinfo.loadBtn.AddComponent<TooltipHandler>().tooltip = md.modinfo.loadBtnTooltip;
                md.modinfo.unloadBtn = md.modinfo.buttons.transform.Find("UnloadModBtn").gameObject;
                md.modinfo.unloadBtnTooltip = md.modinfo.buttons.transform.Find("UnloadModBtn").Find("Tooltip").gameObject;
                md.modinfo.unloadBtn.AddComponent<TooltipHandler>().tooltip = md.modinfo.unloadBtnTooltip;
                md.modinfo.permanentModWarning?.SetActive(false);
                md.modinfo.loadBtn?.SetActive(false);
                md.modinfo.unloadBtn?.SetActive(false);
                md.modinfo.versionTooltip = md.modinfo.ModlistEntry.transform.Find("ModVersionText").Find("VersionTooltip").gameObject;
                md.modinfo.ModlistEntry.transform.Find("ModVersionText").gameObject.AddComponent<TooltipHandler>().tooltip = md.modinfo.versionTooltip;
                if (md.modinfo.modFiles.ContainsKey(md.jsonmodinfo.icon))
                {
                    Texture2D potentialIcon = new Texture2D(2, 2);
                    potentialIcon.LoadImage(md.modinfo.modFiles[md.jsonmodinfo.icon]);
                    md.modinfo.ModIcon = potentialIcon;
                    md.modinfo.ModlistEntry.transform.Find("ModIconMask").Find("ModIcon").GetComponent<RawImage>().texture = potentialIcon;
                }
                else
                {
                    md.modinfo.ModIcon = HLib.missingTexture;
                    md.modinfo.ModlistEntry.transform.Find("ModIconMask").Find("ModIcon").GetComponent<RawImage>().texture = HLib.missingTexture;
                }
#if GAME_RAFT
                if (md.jsonmodinfo.author == "Update Me!")
                {
                    md.modinfo.ModIcon = HLib.bundle.LoadAsset<Texture2D>("deprecatedMod");
                    md.modinfo.ModlistEntry.transform.Find("ModIconMask").Find("ModIcon").GetComponent<RawImage>().texture = HLib.bundle.LoadAsset<Texture2D>("deprecatedMod");
                }
#endif
                if (md.modinfo.modFiles.ContainsKey(md.jsonmodinfo.banner))
                {
                    Texture2D potentialBanner = new Texture2D(2, 2);
                    potentialBanner.LoadImage(md.modinfo.modFiles[md.jsonmodinfo.banner]);
                    md.modinfo.ModBanner = potentialBanner;
                }
                else
                {
                    md.modinfo.ModBanner = HLib.missingTexture;
                }
                HUtils.GetModVersion(md.jsonmodinfo.updateUrl).ContinueWith((t) =>
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(async () =>
                    {
                        try
                        {
                            if (t.Result == "Unknown" || t.Result == "")
                            {
                                md.modinfo.versionTooltip.GetComponentInChildren<Text>().text = "Unknown";
                                md.modinfo.ModlistEntry.transform.Find("ModVersionText").GetComponent<UnityEngine.UI.Text>().color = orangeColor;
                            }
                            else
                            {
                                if (t.Result != md.jsonmodinfo.version)
                                {
                                    md.modinfo.versionTooltip.GetComponentInChildren<Text>().text = "New version available!";
                                    Debug.LogWarning("[ModManager] " + md.jsonmodinfo.name + " > The current installed version is outdated! A new version is available! (" + t.Result + ")");
                                    HNotify.Get().AddNotification(HNotify.NotificationType.scaling, md.jsonmodinfo.name + " has a new version available!", 5, await HLib.bundle.TaskLoadAssetAsync<Sprite>("DownloadIcon"));
                                    md.modinfo.ModlistEntry.transform.Find("ModVersionText").GetComponent<UnityEngine.UI.Text>().color = redColor;
                                }
                                else
                                {
                                    md.modinfo.versionTooltip.GetComponentInChildren<Text>().text = "Up to date!";
                                    md.modinfo.ModlistEntry.transform.Find("ModVersionText").GetComponent<UnityEngine.UI.Text>().color = greenColor;
                                }
                            }
                        }
                        catch { }
                    });
                });
                md.modinfo.unloadBtn.GetComponent<Button>().onClick.AddListener(() => md.modinfo.modHandler.UnloadMod(md));
                md.modinfo.loadBtn.GetComponent<Button>().onClick.AddListener(() => md.modinfo.modHandler.LoadMod(md));
                md.modinfo.infoBtn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    ShowModInfo(md);
                });

                if ((md.jsonmodinfo.isModPermanent || (OnStartUserPermanentMods.list.Contains(md.modinfo.modFile.Name.ToLower()))) && listRefreshAmount == 0)
                {
                    md.modinfo.modHandler.LoadMod(md);
                }
                else if (md.jsonmodinfo.isModPermanent && listRefreshAmount > 0 && md.modinfo.modState == ModInfo.ModStateEnum.idle)
                {
                    md.modinfo.modState = ModInfo.ModStateEnum.needrestart;
                }

                RefreshModState(md);
                modList.Add(md);
                TestList.Add(md);
                noModsText.SetActive(false);
            }
            catch (Exception e)
            {
                Debug.LogError("[ModManager] " + file.Name + " > A fatal error occured while loading the mod!\nStacktrace : " + e.ToString());
            }
            await Task.Delay(1);
        }

        public static void ShowModInfo(ModData md)
        {
            modInfoObj.transform.Find("ModName").GetComponent<Text>().text = md.jsonmodinfo.name;
            modInfoObj.transform.Find("Author").GetComponent<Text>().text = "Author : " + md.jsonmodinfo.author;
            modInfoObj.transform.Find("ModVersion").GetComponent<Text>().text = "Version : " + md.jsonmodinfo.version;
            modInfoObj.transform.Find("GameVersion").GetComponent<Text>().text = "Game Version : " + md.jsonmodinfo.gameVersion;
            modInfoObj.transform.Find("BannerMask").Find("Banner").GetComponent<RawImage>().texture = md.modinfo.ModBanner;
            modInfoObj.transform.Find("BannerMask").Find("IconMask").Find("Icon").GetComponent<RawImage>().texture = md.modinfo.ModIcon;
            modInfoObj.transform.Find("Description").GetComponent<Text>().text = "Description : \n\n" + md.jsonmodinfo.description;

            modInfoObj.transform.Find("MakePermanent").gameObject.SetActive(!md.jsonmodinfo.isModPermanent);
            modInfoObj.transform.Find("MakePermanent").GetComponentInChildren<Text>().text = "LOAD THIS MOD AT STARTUP";
            modInfoObj.transform.Find("MakePermanent").GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
            modInfoObj.transform.Find("MakePermanent").GetComponent<Toggle>().isOn = UserPermanentMods.list.Contains(md.modinfo.modFile.Name.ToLower());
            modInfoObj.transform.Find("MakePermanent").GetComponent<Toggle>().onValueChanged.AddListener((val) =>
            {
                SetModPermanent(val, md.modinfo.modFile.Name.ToLower());
            });
            modInfoObj.SetActive(true);
        }

        public static void SetModPermanent(bool val, string fileName)
        {
            if (val)
            {
                if (!UserPermanentMods.list.Contains(fileName))
                {
                    UserPermanentMods.list.Add(fileName);
                }
            }
            else
            {
                if (UserPermanentMods.list.Contains(fileName))
                {
                    UserPermanentMods.list.Remove(fileName);
                }
            }

            PlayerPrefs.SetString(AssemblyLoader.HLoader.settingsPrefix + "UserPermanentMods", JsonConvert.SerializeObject(UserPermanentMods));
        }
    }
}
