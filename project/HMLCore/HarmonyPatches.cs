using HarmonyLib;
using Il2CppInterop.Common.Attributes;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace HML
{
    public class HarmonyPatches
    {

        [HarmonyPatch("Log",typeof(Debug))]
        [HarmonyPatch(new System.Type[] { typeof(Il2CppSystem.Object) })]
        static class HarmonyDebugLog
        {
            static bool Prefix(Il2CppSystem.Object message)
            {
                HConsole.HandleUnityLog(message.ToString(),"",LogType.Log,false);
                return false;
            }
        }
    }
}
