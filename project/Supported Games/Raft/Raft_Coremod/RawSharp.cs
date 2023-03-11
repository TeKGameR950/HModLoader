using AssemblyLoader;
using HMLLibrary;
using RoslynCSharp;
using RoslynCSharp.Compiler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RaftModLoader
{
    public class RawSharp : MonoBehaviour
    {
        public static string evalPrefix = "<color=#189ad3>[Evaluation]</color> ";

        [ConsoleCommand("csrun", "Syntax : 'csrun <code...>' Allows you to run CSharp code at runtime for testing and debugging.")]
        public static async void CSRUN(string[] args)
        {
            if (HLoader.SAFEMODE)
            {
                Debug.LogWarning("This command has been disabled for security reasons! Create a file named <b>disable.sandbox</b> in Raft folder to enable it. IT IS HIGHLY DISCOURAGED!");
                return;
            }
            DateTime start = DateTime.Now;
            if (args.Length < 1)
            {
                Debug.LogWarning("You must specify code to evaluate with 'csrun'.");
                return;
            }
            string evalCode = string.Join(" ", args);
            if (evalCode.Length < 2)
            {
                Debug.LogWarning("You must specify code to evaluate with 'csrun'.");
                return;
            }
            Debug.Log(evalPrefix + "Evaluation is in progress...");

            CompilationResult result = await HCompiler.Main.CompileCode("Evaluation", new Dictionary<string, string>() { { "eval.cs", evalFileContent.Replace("EVALCODE_STRING", evalCode) } }, new List<byte[]>(), false);
            if (result.Success)
            {
                Assembly asm = result.OutputAssembly;
                var methodInfo = asm.GetTypes().First().GetMethod("EvalMethod");
                methodInfo.Invoke(null, null);
                Debug.Log(evalPrefix + "Evaluation succeeded in " + (DateTime.Now - start).Milliseconds + "ms!");
            }
            else
            {
                Debug.LogError(evalPrefix + " The evaluation failed!");
            }
        }

        public static string evalFileContent = @"using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using HarmonyLib;
using Steamworks;
using HMLLibrary;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Text;
using System.Collections;
using FMOD;
using FMODUnity;
using UltimateWater;
using UnityEngine.AzureSky;
using RaftModLoader;
using TMPro;
using EZCameraShake;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;
using ShellLink;

public class Class1 : MonoBehaviour
{
    public static void EvalMethod()
    {
        EVALCODE_STRING
    }
}";
    }
}