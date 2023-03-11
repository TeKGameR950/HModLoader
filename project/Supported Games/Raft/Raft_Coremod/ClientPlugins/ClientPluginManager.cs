using AssemblyLoader;
using HarmonyLib;
using HMLLibrary;
using I2.Loc;
using RoslynCSharp.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AzureSky;
using UnityEngine.UI;
using CPlugin = RaftModLoader.ClientPlugin.ClientPlugin;

namespace RaftModLoader
{
    public class ClientPluginManager
    {
        public static GameObject clientPlugins;

        public static Dictionary<string, ClientPluginData> loadedClientPlugins = new Dictionary<string, ClientPluginData>();

        public static async void OnMainMenu()
        {
            if (!HLoader.SAFEMODE) return;
            GameObject menuCanvas = GameObject.Find("MainMenuCanvas");
            Transform btns = menuCanvas.transform.Find("MenuButtons");
            btns.Find("New Game").gameObject.SetActive(false);
            btns.Find("Load game").gameObject.SetActive(false);
            if (btns.Find("Join Game").GetComponentInChildren<Localize>())
                GameObject.Destroy(btns.Find("Join Game").GetComponentInChildren<Localize>());
            btns.Find("Join Game").GetComponentInChildren<Text>().resizeTextForBestFit = false;
            btns.Find("Join Game").GetComponentInChildren<Text>().fontSize = 45;
            btns.Find("Join Game").GetComponentInChildren<Text>().text = "View Servers";
            btns.Find("Join Game").GetComponent<LayoutElement>().preferredHeight = 60;
            Image img = GameObject.Find("Raft Logo").GetComponent<Image>();
            img.color = Color.white;
            img.sprite = HLib.bundle.LoadAsset<Sprite>("Raft_Safe");
            GameObject.FindObjectOfType<AzureSkyController>().timeOfDay.GotoTime(24);

            // This method goal is to make the game as close as possible to original (without modifications)
            loadedClientPlugins.ToList().ForEach(x =>
            {
                UnloadClientPlugin(x.Key);
            });
            GameObject.Destroy(clientPlugins);

            // Unregister every items.
            ItemManager.Initialize();
        }

        public static async void LoadClientPlugin(string name, Dictionary<string, byte[]> files)
        {
            if (!HLoader.SAFEMODE) return;
            Debug.Log("LOADING PLUGIN " + name);
            if (clientPlugins == null)
            {
                clientPlugins = new GameObject("ClientPlugins");
                GameObject.DontDestroyOnLoad(clientPlugins);
            }
            if (loadedClientPlugins.ContainsKey(name))
            {
                ClientPluginData data = loadedClientPlugins[name];
                if (data.instance != null)
                {
                    UnloadClientPlugin(name);
                    await Task.Delay(200);
                }
            }

            Assembly asm = null;
            CompilationResult result = await HCompiler.Main.CompileCode(name, files.Where(x => x.Key.ToLower().EndsWith(".cs")).Select(x => new KeyValuePair<string, string>(x.Key, Encoding.UTF8.GetString(x.Value))).ToDictionary(x => x.Key, x => x.Value), new List<byte[]>(), true);
            try
            {
                asm = result.OutputAssembly;
                IEnumerable<Type> types = asm.GetTypes().Where(t => t.IsSubclassOf(typeof(CPlugin)));
                if (types.Count() != 1)
                {
                    Debug.LogError("[ClientPluginManager] " + name + "> The client plugin codebase doesn't specify a ClientPlugin class or specifies more than one!");
                    return;
                }
                else
                {
                    ClientPluginData cpd = new ClientPluginData();
                    cpd.name = name;
                    cpd.files = files;
                    cpd.assembly = asm;
                    GameObject PluginObj = new GameObject();
                    PluginObj.SetActive(false);
                    PluginObj.name = "ClientPlugin-" + name;
                    PluginObj.transform.parent = clientPlugins.transform;
                    cpd.mainClass = PluginObj.AddComponent(types.FirstOrDefault()) as CPlugin;
                    loadedClientPlugins.Add(name, cpd);
                    PluginObj.SetActive(true);
                    cpd.instance = PluginObj;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("[ClientPluginManager] " + name + " > The client plugin failed to load!\n" + ex);
            }
        }

        public static void UnloadClientPlugin(string name)
        {
            if (!HLoader.SAFEMODE) return;
            Debug.Log("UNLOADING PLUGIN " + name);
            if (clientPlugins != null)
            {
                Transform instance = clientPlugins.transform.Find("ClientPlugin-" + name);
                if (loadedClientPlugins.ContainsKey(name))
                {
                    ClientPluginData cpd = loadedClientPlugins[name];
                    loadedClientPlugins.Remove(name);
                    try
                    {
                        if (cpd.instance != null && cpd.assembly != null)
                        {
                            IEnumerable<Type> types = cpd.assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(CPlugin)));
                            if (cpd.instance.GetComponent(types.First()))
                            {
                                MethodInfo methodInfo = AccessTools.Method(types.First(), "OnPluginUnload");
                                if (methodInfo != null)
                                    if (methodInfo.IsStatic) { methodInfo.Invoke(null, null); } else { methodInfo.Invoke(cpd.instance.GetComponent(types.First()), null); }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("[ClientPluginManager] " + name + " > An error occured while running the client plugin OnPluginUnload() method!\nStacktrace : " + e.ToString());
                    }
                }
                if (instance != null)
                {
                    GameObject.Destroy(instance.gameObject);
                }
                //HConsole.instance.RefreshCommands();
            }
        }
    }

    public class ClientPluginData
    {
        public string name;
        public GameObject instance;
        public Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();
        public Assembly assembly;
        public CPlugin mainClass;
    }
}
