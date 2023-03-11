using BepInEx.Unity.IL2CPP.UnityEngine;
using Il2CppInterop.Runtime.Injection;
using RoslynCSharp.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Il2CppSystem.Linq.Expressions.Interpreter.InitializeLocalInstruction;

namespace HML
{
    public class HMLInitializer
    {
        public async static void Load()
        {
            AddCompilerReferences();
            HCompiler.Main.Initialize();
            // TODO : Game Specific
            while (SceneManager.GetActiveScene().name != "SonsTitleScene")
                await Task.Delay(1000);

            if (!GameObject.Find("HModLoader"))
            {
                GameObject obj = new GameObject("HModLoader");
                Object.DontDestroyOnLoad(obj);
                // TODO : IL2CPP Specific (add compatibility with Mono)
                ClassInjector.RegisterTypeInIl2Cpp(typeof(MainBehaviour));
                obj.AddComponent<MainBehaviour>();
            }
        }

        public static List<string> blacklistedReferences = new List<string>()
        {
            "System.IO.Compression.Native.dll",
            "msquic.dll",
            "mscorrc.dll",
            "mscordbi.dll",
            "mscordaccore_amd64_amd64_6.0.722.32202.dll",
            "mscordaccore.dll",
            "Microsoft.DiaSymReader.Native.amd64.dll",
            "hostpolicy.dll",
            "dbgshim.dll",
            "coreclr.dll",
            "clrjit.dll",
            "clretwrc.dll",
            "dobby.dll"
        };

        // TODO : Game Specific
        public static void AddCompilerReferences()
        {
            List<string> references = new List<string>();

            string interop = Path.Combine(HMLCore.currentGameFolder, "interop");
            if (Directory.Exists(interop))
                references.AddRange(Directory.GetFiles(interop, "*.dll"));
            string core = Path.Combine(HMLCore.currentGameFolder, "core");
            if (Directory.Exists(core))
                references.AddRange(Directory.GetFiles(core, "*.dll"));
            string dotnet = Path.Combine(HMLCore.currentGameFolder, "dotnet");
            if (Directory.Exists(dotnet))
                references.AddRange(Directory.GetFiles(dotnet, "*.dll"));
            string plugins = Path.Combine(HMLCore.currentGameFolder, "plugins");
            if (Directory.Exists(plugins))
                references.AddRange(Directory.GetFiles(plugins, "*.dll"));
            RoslynCSharp.RoslynCSharp.settings.references = references.Where(r => !blacklistedReferences.Any(b => r.EndsWith(b))).ToList();
        }
    }

    public class TempRawSharp : MonoBehaviour
    {
        public async void Compile()
        {
            HMLCore.Log("Compiling...");
            string raw = File.ReadAllText("raw.cs");
            CompilationResult result = await HCompiler.Main.CompileCode("preload", new Dictionary<string, string>() { { "preload.cs", raw } }, new List<byte[]>(), false);
            if (result.Success)
            {
                Assembly asm = result.OutputAssembly;
                var methodInfo = asm.GetTypes().First().GetMethod("Test");
                methodInfo.Invoke(null, null);
                HMLCore.Log("Evaluation succeeded !");
            }
            else
            {
                HMLCore.LogError("The evaluation failed!");
            }
        }

        public void Update()
        {
            if (Input.GetKeyInt(BepInEx.Unity.IL2CPP.UnityEngine.KeyCode.F5) && Event.current.type == EventType.KeyDown)
            {
                Compile();
            }
        }
    }
}
