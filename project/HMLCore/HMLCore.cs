using System;
using BepInEx;
using UnityEngine;
using BepInEx.Unity.IL2CPP;
using BepInEx.Unity.IL2CPP.UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Il2CppSystem.Runtime.Remoting.Messaging;
using Il2CppInterop.Runtime.Injection;
using UnityEngine.SceneManagement;
using System.Text;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppSystem.Linq;

namespace HML
{
    [BepInPlugin("net.hmodloader", "HMLCore", "0.0.1")]
    public class HMLCore : BasePlugin
    {
        public static BepInEx.Logging.ManualLogSource log;
        public HMLCore() { log = base.Log; }

        public static string appdataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HModLoader");
        public static string dataFolder = Path.Combine(appdataFolder, "data");
        public static string gamesFolder = Path.Combine(dataFolder, "games");
        // TODO : Use ScriptingSymbols to support various games (or get name from a known file).
        public static string currentGameFolder = Path.Combine(gamesFolder, "sotf");
        public static string binariesFolder = Path.Combine(currentGameFolder, "binaries");

        // Maybe ?
        public static string pidFile = Path.Combine(appdataFolder, "pid.txt");
        public static string configFile = Path.Combine(appdataFolder, "config.ini");

        public static Harmony harmonyInstance;
        public static Il2CppAssetBundle hmlBundle;
        public static List<string> embeddedAssemblies = new List<string>()
        {
            "SharpZipLib.dll",
            "System.ValueTuple.dll"
        };

        public async override void Load()
        {
            // HERE DO THE PID CHECK
            Log("Loading HMLCore...");
            Il2CppTypeSupport.Initialize();
            AssetBundle.GetAllLoadedAssetBundles().ToList().ForEach((Action<AssetBundle>)(x =>
            {
                if (x.name == "hml.assets")
                {
                    HMLCore.Log("Unloaded hml.assets as it was already loaded !");
                    x.Unload(true);
                }
            }));
            hmlBundle = Il2CppAssetBundleManager.LoadFromMemory(File.ReadAllBytes(Path.Combine(currentGameFolder, "hml.assets")));
            CreateDirectories();
            LoadEmbeddedAssemblies();

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(HUtils.ModAssemblyResolve);
            Assembly compilerAsm = Assembly.Load(HUtils.GetAssemblyBytes("HML.Dependencies.HCompiler.HCompiler.IL2CPP.dll"));

            try
            {
                harmonyInstance = new Harmony("net.hmodloader");
                harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }

            HMLInitializer.Load();
        }

        static void LoadEmbeddedAssemblies()
        {
            embeddedAssemblies.ForEach(assemblyName =>
            {
                try
                {
                    byte[] ba = HUtils.GetAssemblyBytes("HML.Dependencies." + assemblyName);
                    Assembly asm = Assembly.Load(ba);
                    File.WriteAllBytes(Path.Combine(binariesFolder, assemblyName), ba);
                }
                catch (Exception e)
                {
                    LogError("An error occured while extracting the embedded assembly \"" + assemblyName + "\" ( " + e.Message + " ) Stacktrace : " + e.StackTrace.ToString());
                }
            });
        }


        public static void CreateDirectories()
        {
            Directory.CreateDirectory(appdataFolder);
            Directory.CreateDirectory(dataFolder);
            Directory.CreateDirectory(gamesFolder);
            Directory.CreateDirectory(currentGameFolder);
            Directory.CreateDirectory(binariesFolder);
        }

        public static void Log(object o)
        {
            log.LogInfo("[HMLCore] " + o);
        }

        public static void LogError(object o)
        {
            log.LogError("[HMLCore] " + o);
        }

        public static void LogWarning(object o)
        {
            log.LogWarning("[HMLCore] " + o);
        }
    }

    public class TestBehaviour : MonoBehaviour
    {
        static bool noclip = false;
        static float normal = 0.1f;
        static float fast = 0.5f;
        static float speed = 0.1f;

        void Update()
        {
            if (Input.GetKeyInt(BepInEx.Unity.IL2CPP.UnityEngine.KeyCode.F1) && Event.current.type == EventType.KeyDown)
            {
                noclip = !noclip;
                HMLCore.Log("Toggled Noclip !");
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                Rigidbody rb = player.GetComponent<Rigidbody>();
                rb.isKinematic = noclip;
            }

            if (Input.GetKeyInt(BepInEx.Unity.IL2CPP.UnityEngine.KeyCode.F2) && Event.current.type == EventType.KeyDown)
            {

            }

            if (noclip && Input.GetKeyInt(BepInEx.Unity.IL2CPP.UnityEngine.KeyCode.LeftShift))
                speed = fast;
            else
                speed = normal;

            if (noclip && Input.GetKeyInt(BepInEx.Unity.IL2CPP.UnityEngine.KeyCode.Space))
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                player.transform.position += Vector3.up * speed;
            }

            if (noclip && Input.GetKeyInt(BepInEx.Unity.IL2CPP.UnityEngine.KeyCode.LeftControl))
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                player.transform.position += Vector3.down * speed;
            }

            if (noclip && Input.GetKeyInt(BepInEx.Unity.IL2CPP.UnityEngine.KeyCode.Z))
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                player.transform.position += Camera.main.transform.forward * speed;
            }
            if (noclip && Input.GetKeyInt(BepInEx.Unity.IL2CPP.UnityEngine.KeyCode.S))
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                player.transform.position += -Camera.main.transform.forward * speed;
            }
            if (noclip && Input.GetKeyInt(BepInEx.Unity.IL2CPP.UnityEngine.KeyCode.D))
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                player.transform.position += Camera.main.transform.right * speed;
            }
            if (noclip && Input.GetKeyInt(BepInEx.Unity.IL2CPP.UnityEngine.KeyCode.Q))
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                player.transform.position += -Camera.main.transform.right * speed;
            }
        }
    }

}
