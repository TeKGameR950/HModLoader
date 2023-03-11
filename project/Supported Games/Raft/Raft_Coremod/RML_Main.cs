using HarmonyLib;
using HMLLibrary;
using RaftModLoader;
using SocketIO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEngine;
using Debug = UnityEngine.Debug;
using System.Linq;
using Steamworks;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using AssemblyLoader;
using Newtonsoft.Json;
using TMPro;

public class RML_Main : MonoBehaviour
{
    private bool debug = false;

    public static List<string> logs = new List<string>();

    public static InstalledGameData gameData = new InstalledGameData();

    public static KeyCode MenuKey = KeyCode.F9;
    public static KeyCode ConsoleKey = KeyCode.F10;

    [Obsolete("Please use HLib.path_modsFolder instead")]
    public static string path_modsFolder = Path.Combine(Application.dataPath, "..\\mods");

    void OnEnable()
    {
        Application.logMessageReceived += HandleUnityLog;
        if (File.Exists(Path.Combine(HLib.currentGameFolder, "data.json")))
            gameData = JsonConvert.DeserializeObject<InstalledGameData>(File.ReadAllText(Path.Combine(HLib.currentGameFolder, "data.json")));
    }

    void HandleUnityLog(string logString, string stackTrace, LogType type)
    {
        if (type != LogType.Warning && type != LogType.Log)
            File.AppendAllText(HLib.gameLogFile, logString + ":" + stackTrace + "\n");
    }

    [ConsoleCommand("safemode")]
    public static async void EnableSafeMode()
    {
        /*GameObject.Destroy(GameObject.Find("HyTeKModLoader"));
        FindObjectsOfType<RMLObject>().ToList().ForEach(x =>
        {
            GameObject.Destroy(x.gameObject);
        });
        MainMenu.menuPages = new Dictionary<string, MenuPage>();
        harmonyInstance.UnpatchAll("hytekgames.raftmodloader");
        var a = SceneManager.LoadSceneAsync(0);
        while (!a.isDone)
            await Task.Delay(1);
        await Task.Delay(1000);
        HLoader.SAFEMODE = true;
        HLoader.StartModLoader();*/

        //File.WriteAllText(HLoader.safemodeFile,"");
        Process.Start(Process.GetCurrentProcess().MainModule.FileName);
        Process.GetCurrentProcess().Kill();

    }

    void OnGUI()
    {
        if (debug)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.red;
            string text = "";
            foreach (string s in logs)
            {
                text += "\n" + s;
            }
            GUI.Label(new Rect(0, 0, Screen.width, Screen.height), text, style);
        }
    }

    private void OnSceneChanged(Scene scene1, Scene scene2)
    {
        Raft_Network.OnJoinResult += OnJoinResult;
    }

    public static async void OnJoinResult(CSteamID remoteID, InitiateResult result)
    {
        ServersPage.OnJoinResult(remoteID, result);
        NetworkModManager.OnJoinResult(remoteID, result);
    }

    private void Awake()
    {

        SceneManager.activeSceneChanged += OnSceneChanged;
        Application.backgroundLoadingPriority = ThreadPriority.High;
        Directory.CreateDirectory(HLib.path_dataFolder);
        Directory.CreateDirectory(HLib.path_cacheFolder);
        Directory.CreateDirectory(HLib.path_cacheFolder_mods);
        Directory.CreateDirectory(HLib.path_cacheFolder_textures);
        Directory.CreateDirectory(HLib.path_cacheFolder_temp);
        Directory.SetCurrentDirectory(Path.Combine(Application.dataPath, "..\\"));
        try
        {
            // Free up space in cache.
            try
            {
                foreach (string f in Directory.GetFiles(HLib.path_cacheFolder_mods))
                {
                    if (!f.EndsWith("_" + Application.version + ".dll"))
                    {
                        try
                        {
                            File.Delete(f);
                        }
                        catch { }
                    }
                }

                foreach (string f in Directory.GetFiles(HLib.path_cacheFolder_temp))
                {
                    if (f.EndsWith("csc_preloadsample.cs")) continue;
                    try
                    {
                        File.Delete(f);
                    }
                    catch { }
                }
            }
            catch { }

            StartCoroutine(LoadRML());
        }
        catch (Exception ex)
        {
            File.AppendAllText(HLib.gameLogFile, ex.ToString() + "\n");
        }
    }

    [Serializable]
    private class ServerVersionInfo
    {
        public string clientVersion = "";
    }

    static Harmony harmonyInstance;
    IEnumerator LoadRML()
    {
        Directory.GetFiles(HLib.path_cacheFolder_temp).Where(s => s.ToLower().EndsWith(".dll")).ToList().ForEach((s) => File.Delete(s));

        AssetBundle.GetAllLoadedAssetBundles().ToList().ForEach(x =>
        {
            if (x.name == "rml.assets")
                x.Unload(true);
        });

        ComponentManager<RML_Main>.Value = this;
        var request = AssetBundle.LoadFromFileAsync(Path.Combine(HLib.currentGameFolder, "rml.assets"));
        yield return request;
        HLib.bundle = request.assetBundle;

        ComponentManager<RSocket>.Value = gameObject.AddComponent<RSocket>();

        HLib.missingTexture = HLib.bundle.LoadAsset<Sprite>("missing").texture;
        gameObject.AddComponent<RConsole>();
        while (!GameObject.Find("RConsole")) { yield return new WaitForEndOfFrame(); }

        try
        {
            harmonyInstance = new Harmony("hytekgames.raftmodloader");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }

        ComponentManager<RawSharp>.Value = gameObject.AddComponent<RawSharp>();
        ComponentManager<SocketIOComponent>.Value = gameObject.AddComponent<SocketIOComponent>();
        GameObject mainThread = new GameObject("HMainThreadDispatcher");
        mainThread.transform.SetParent(gameObject.transform);
        ComponentManager<UnityMainThreadDispatcher>.Value = mainThread.AddComponent<UnityMainThreadDispatcher>();
        ComponentManager<RNetworkImprovements>.Value = gameObject.AddComponent<RNetworkImprovements>();
        gameObject.AddComponent<CustomLoadingScreen>();

        var MenuAssetLoadRequest = HLib.bundle.LoadAssetAsync<GameObject>("RMLMainMenu");
        yield return MenuAssetLoadRequest;

        GameObject tempmenuobj = MenuAssetLoadRequest.asset as GameObject;
        GameObject mainmenu = Instantiate(tempmenuobj, gameObject.transform);
        ComponentManager<MainMenu>.Value = mainmenu.AddComponent<MainMenu>();

        var HNotifyLoadRequest = HLib.bundle.LoadAssetAsync<GameObject>("RMLNotificationSystem");
        yield return HNotifyLoadRequest;

        GameObject hnotifytempobj = HNotifyLoadRequest.asset as GameObject;
        GameObject hnotify = Instantiate(hnotifytempobj, gameObject.transform);
        ComponentManager<HNotify>.Value = hnotify.AddComponent<HNotify>();
        gameObject.AddComponent<RChat>();

        gameObject.AddComponent<NetworkModManager>();

        RaftModLoader.RTCP.InitServerIP();
    }

    public class RMLObject : MonoBehaviour
    {

    }
}

public static class RMLObjectExtension
{
    public static GameObject NoteAsRML(this GameObject obj)
    {
        obj.AddComponent<RML_Main.RMLObject>();
        return obj;
    }
}

public class InstalledGameData
{
    public bool installed;
    [JsonIgnore]
    public bool installing;
    public DateTime lastPlay;
    public string version;
    public string gamepath;
    public string startingMethod = "executable";
    public List<string> installedFiles = new List<string>();
    public List<string> installedFolders = new List<string>();
}

