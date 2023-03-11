using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HML
{
    public static class HConsole
    {
        public static bool initialized = false;
        public static GameObject gameObject;
        public static GameObject logEntryPrefab;
        public static Transform logViewport;
        public static ScrollRect scrollRect;

        

        public static void Initialize(GameObject console)
        {
            /*gameObject = console;
            logEntryPrefab = HMLCore.hmlBundle.Load<GameObject>("ConsoleLogEntry");
            logViewport = gameObject.transform.Find("BG").Find("Scroll View").Find("Viewport").Find("Content");
            scrollRect = gameObject.transform.Find("BG").Find("Scroll View").GetComponent<ScrollRect>();
            initialized = true;*/
        }

        public static async void HandleUnityLog(string logString, string stackTrace, LogType type, bool invokedOnMainThread)
        {
            if (!initialized) return;
            /*try
            {
                logEntryPrefab = HMLCore.hmlBundle.Load<GameObject>("ConsoleLogEntry");
                logViewport = gameObject.transform.Find("BG").Find("Scroll View").Find("Viewport").Find("Content");

                GameObject logEntry = GameObject.Instantiate(logEntryPrefab, logViewport);
                
                if (logEntry != null)
                {
                    logEntry.gameObject.GetComponent<Text>().text = logString;
                    logViewport.GetComponent<RectTransform>().ForceUpdateRectTransforms();
                    scrollRect.verticalNormalizedPosition = 0f;
                }
            }
            catch(Exception e) { HML.Log("Exception!!!! " + e); }*/
        }
    }
}
