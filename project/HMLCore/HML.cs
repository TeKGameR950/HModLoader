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
    public static class HML
    {
        public static void Log(object o)
        {
            HMLCore.log.LogInfo("[HMLCore] " + o);
        }

        public static void LogError(object o)
        {
            HMLCore.log.LogError("[HMLCore] " + o);
        }

        public static void LogWarning(object o)
        {
            HMLCore.log.LogWarning("[HMLCore] " + o);
        }
    }
}
