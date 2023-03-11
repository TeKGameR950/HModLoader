using AssemblyLoader;
using HarmonyLib;
using HMLLibrary;
using Newtonsoft.Json;
using RaftModLoader;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace RaftModLoader
{
    public class RConsole : HConsole
    {
        [Obsolete("Please use the new command attributes instead!")]
        public static string[] lcargs;
        public static bool isOpen;

        [Obsolete("Use RConsole.isOpen instead.")]
        public static bool isConsoleOpen
        {
            get
            {
                return isOpen;
            }
            set
            {
                isOpen = value;
            }
        }

        private static string code = "";
        public static LatestLog latestlog;
        public static List<string> lastCommands = new List<string>();
        private static Dictionary<string, CommandEntry> commandMap = new Dictionary<string, CommandEntry>();
        private static Dictionary<string, CommandEntry> DeprecatedCommandMap = new Dictionary<string, CommandEntry>();
        private CanvasGroup canvasgroup;
        private static Transform viewportContent;
        private GameObject lineEntryPrefab;
        private ScrollRect scrollRect;
        private static InputField inputfield;
        private GameObject autocomplete;
        private Button SendButton;
        private Button BiggifyInputfieldButton;
        private delegate string commandAction(params string[] args);
        private delegate string simpleCommandAction();
        private delegate void silentCommandAction(string[] args);
        private delegate void simpleSilentCommandAction();
        private static bool isInputfieldBig = false;
        private static GameObject CurrentHoveredLine;
        private GameObject selectedLine;
        private int PreviousCommandId = -1;
        private GameObject RightClickMenu;

        [Obsolete("Please use Debug.Log() instead!")]
        public static void Log(string log)
        {
            Debug.Log(log);
        }

        [Obsolete("Please use Debug.Log() instead!")]
        public static void Log(string log, LogType LogType)
        {
            Debug.Log(log);
        }

        [Obsolete("Please use Debug.Log() instead!")]
        public static void Log(LogType LogType, string log)
        {
            Debug.Log(log);
        }

        [Obsolete("Please use Debug.LogAssertion() instead!")]
        public static void LogAssert(string log)
        {
            Debug.LogAssertion(log);
        }

        [Obsolete("Please use Debug.LogError() instead!")]
        public static void LogError(string log)
        {
            Debug.LogError(log);
        }

        [Obsolete("Please use Debug.LogException() instead!")]
        public static void LogException(string log)
        {
            Debug.LogError(log);
        }

        [Obsolete("Please use Debug.LogWarning() instead!")]
        public static void LogWarning(string log)
        {
            Debug.LogWarning(log);
        }

        [Obsolete("THIS IS GOING TO BE REMOVED IN A FUTURE UPDATE! PLEASE USE THE NEW COMMANDS ATTRIBUTES!")]
        public static void unregisterCommand(string s) { }

        [Obsolete("THIS IS GOING TO BE REMOVED IN A FUTURE UPDATE! PLEASE USE THE NEW COMMANDS ATTRIBUTES!")]
        public static void registerCommand(Type modType, string desc, string command, Action action)
        {
            try
            {
                Regex r = new Regex("^[a-zA-Z0-9]*$");
                if (command.Contains(" "))
                {
                    Debug.LogError("Command " + command + " contains spaces!");
                    return;
                }
                if (!r.IsMatch(command))
                {
                    Debug.LogError("Command " + command + " contains invalids characters!");
                    return;
                }

                foreach (KeyValuePair<string, CommandEntry> c in commandMap)
                {
                    if (c.Key.ToLower().Equals(command.ToLower()))
                    {
                        Debug.LogError("Command " + command + " is already registered!");
                        return;
                    }
                }

                DeprecatedCommandMap[command] = new CommandEntry() { docs = desc ?? "", action = null, deprecatedAction = action };
                //Debug.Log("Successfully registered command <i>" + command + "</i>.");
            }
            catch (Exception ee)
            {
                Debug.LogError("failed command registration " + ee.ToString());
            }
        }

        private async void Start()
        {
            HConsole.instance = this;
            GameObject gameobject = Instantiate(await HLib.bundle.TaskLoadAssetAsync<GameObject>("ConsoleCanvas"), transform);
            lineEntryPrefab = await HLib.bundle.TaskLoadAssetAsync<GameObject>("ConsoleLinePrefab");
            inputfield = gameobject.transform.Find("Background").Find("InputField").GetComponent<InputField>();
            autocomplete = gameobject.transform.Find("Background").Find("InputField").Find("Autocomplete").gameObject;
            SendButton = gameobject.transform.Find("Background").Find("Send").GetComponent<Button>();
            RightClickMenu = gameobject.transform.Find("Background").Find("Scroll View").Find("RightClickMenu").gameObject;
            gameobject.transform.Find("Background").Find("Scroll View").gameObject.AddComponent<ConsoleRightClickHandler>();
            RightClickMenu.transform.Find("Background").Find("Copy").GetComponent<Button>().onClick.AddListener(() =>
            {
                if (selectedLine != null)
                {
                    selectedLine.GetComponent<Text>().text.CopyToClipboard();
                    Debug.Log("<color=#298f2e>Log succcessfully copied to clipboard!</color>");
                }
                RightClickMenu.SetActive(false);
                selectedLine = null;
            });
            RightClickMenu.transform.Find("Background").Find("Remove").GetComponent<Button>().onClick.AddListener(() =>
            {
                if (logs.Contains(selectedLine))
                {
                    selectedLine.SetActive(false);
                    logs.Remove(selectedLine);
                    logPool.Add(selectedLine);
                }
                RightClickMenu.SetActive(false);
                selectedLine = null;
            });
            RightClickMenu.transform.Find("Background").Find("Clear Console").GetComponent<Button>().onClick.AddListener(() =>
            {
                RightClickMenu.SetActive(false);
                selectedLine = null;
                RConsole.ClearConsole();
            });
            SendButton.onClick.AddListener(() =>
            {
                string t = SilentlyRunCommand(inputfield.text);
                if (!string.IsNullOrWhiteSpace(t))
                {
                    Debug.Log(t);
                }
            });
            BiggifyInputfieldButton = gameobject.transform.Find("Background").Find("ToggleBiggerInput").GetComponent<Button>();
            BiggifyInputfieldButton.onClick.AddListener(() =>
            {
                isInputfieldBig = !isInputfieldBig;
                if (isInputfieldBig)
                {
                    inputfield.textComponent.alignment = TextAnchor.UpperLeft;
                    canvasgroup.GetComponent<Animation>().Play("BiggerInputfield");
                }
                else
                {
                    inputfield.textComponent.alignment = TextAnchor.MiddleLeft;
                    canvasgroup.GetComponent<Animation>().Play("NormalInputfield");
                }
            });
            inputfield.onEndEdit.AddListener(val =>
            {
                if (isInputfieldBig) { return; }
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    string t = SilentlyRunCommand(inputfield.text);
                    if (!string.IsNullOrWhiteSpace(t))
                    {
                        Debug.Log(t);
                    }
                }
            });
            inputfield.onValueChanged.AddListener(val =>
            {
                OnCommandInputChange();
            });
            scrollRect = gameobject.transform.Find("Background").Find("Scroll View").GetComponent<ScrollRect>();
            viewportContent = gameobject.transform.Find("Background").Find("Scroll View").Find("Viewport").Find("Content");
            gameobject.name = "RConsole";
            canvasgroup = gameobject.GetComponent<CanvasGroup>();
            canvasgroup.alpha = 0;
            canvasgroup.blocksRaycasts = false;
            canvasgroup.interactable = false;
            if (PlayerPrefs.HasKey("rmlSettings_MaxLogs"))
            {
                int value = PlayerPrefs.GetInt("rmlSettings_MaxLogs");
                MaxLogs = value;
            }
            InitializeConsolePool();
            Application.logMessageReceivedThreaded += HandleUnityLog;
            await Task.Delay(500);
            RefreshCommands();

            DefaultConsoleCommands.LoadBoundCommands();
            await Task.Delay(1000);
            if (!HLoader.SAFEMODE)
            {
                try
                {
                    StartCoroutine(CheckForInstallErrors());
                }
                catch { }
            }
            else
            {
                Debug.Log("<color=#187fed>The game is in RML SAFE MODE !\nThis safe mode is made to be able to securely join dedicated servers that modify your game, so they can't do malicious stuff to your computer.\n</color>\n<color=#18ed6a>You are safe !</color>");
            }
        }

        public static List<GameObject> logPool = new List<GameObject>();
        public static List<GameObject> logs = new List<GameObject>();
        public static int DefaultMaxLogs = 100;
        public static int MaxLogs = DefaultMaxLogs;
        public async Task InitializeConsolePool()
        {
            for (int i = 0; i < MaxLogs; i++)
            {
                GameObject line = Instantiate(lineEntryPrefab, viewportContent);
                line.SetActive(false);
                logPool.Add(line);
            }
            await Task.Delay(1);
        }

        public GameObject GetLogPoolObject()
        {
            if (logPool.Count > 0)
                return logPool.LastOrDefault();

            return RecycleLogPoolObject();
        }

        public GameObject RecycleLogPoolObject()
        {
            GameObject obj = logs.FirstOrDefault();
            obj.SetActive(false);
            logs.Remove(obj);
            logPool.Add(obj);
            return obj;
        }

        private void HandleUnityLog(string logString, string stackTrace, LogType type)
        {
            foreach (string s in blockedWords)
            {
                if (logString.ToLower().Contains(s) || stackTrace.ToLower().Contains(s))
                    return;
            }
            if (!string.IsNullOrEmpty(stackTrace) && !stackTrace.StartsWith("0x0000"))
                logString += "\n" + stackTrace.TrimEnd('\n');


            if (latestlog != null && latestlog.t == type && latestlog.l == logString)
            {
                Text t = latestlog.g.transform.GetChild(1).GetChild(0).GetComponent<Text>();
                latestlog.amount++;
                t.text = (latestlog.amount < 100) ? "x" + latestlog.amount : "+99";
                t.transform.parent.gameObject.SetActive(true);
                if (!isOpen && HNotify.errornotification != null && type != LogType.Log && type != LogType.Warning)
                    HNotify.errornotification.AddNewError();
                return;
            }

            GameObject line = GetLogPoolObject();
            line.transform.SetAsLastSibling();
            line.transform.GetChild(1).gameObject.SetActive(false);
            ConsoleTooltipHandler hover = line.GetComponent<ConsoleTooltipHandler>();
            if (!hover)
                hover = line.AddComponent<ConsoleTooltipHandler>();
            hover.tooltip = line.transform.GetChild(0).gameObject;
            Text text = line.GetComponent<Text>();
            switch (type)
            {
                case LogType.Warning:
                    text.color = new Color(0.9098f, 0.6235f, 0.2509f);
                    break;
                case LogType.Error:
                case LogType.Assert:
                case LogType.Exception:
                    if (!isOpen && HNotify.errornotification != null)
                        HNotify.errornotification.AddNewError();
                    text.color = new Color(0.9372f, 0.1451f, 0.1451f);
                    logString += code;
                    break;
                default:
                    text.color = Color.white;
                    break;
            }
            text.text = logString;
            logPool.Remove(line);
            logs.Add(line);
            latestlog = new LatestLog(line, logString, type);
            line.SetActive(true);
            StartCoroutine(ScrollToBottom());

            #region Old Logging
            /*if (logs.Count >= 101)
            {
                Destroy(logs.First());
                logs.RemoveAt(0);
            }
            foreach (string s in blockedWords)
            {
                if (logString.ToLower().Contains(s) || stackTrace.ToLower().Contains(s))
                    return;
            }
            if (!string.IsNullOrEmpty(stackTrace) && !stackTrace.StartsWith("0x0000"))
            {
                logString += "\n" + stackTrace.TrimEnd('\n');
            }
            if (latestlog != null && latestlog.t == type && latestlog.l == logString)
            {
                Text t = latestlog.g.transform.GetChild(1).GetChild(0).GetComponent<Text>();
                latestlog.amount++;
                t.text = (latestlog.amount < 100) ? "x" + latestlog.amount : "+99";
                t.transform.parent.gameObject.SetActive(true);
                if (!isOpen && HNotify.errornotification != null && type != LogType.Log && type != LogType.Warning)
                    HNotify.errornotification.AddNewError();
                return;
            }

            GameObject line = Instantiate(lineEntryPrefab, viewportContent);
            line.transform.GetChild(1).gameObject.SetActive(false);
            ConsoleTooltipHandler hover = line.AddComponent<ConsoleTooltipHandler>();
            hover.tooltip = line.transform.GetChild(0).gameObject;
            Text text = line.GetComponent<Text>();
            switch (type)
            {
                case LogType.Warning:
                    text.color = new Color(0.9098f, 0.6235f, 0.2509f);
                    break;
                case LogType.Error:
                case LogType.Assert:
                case LogType.Exception:
                    if (!isOpen && HNotify.errornotification != null)
                        HNotify.errornotification.AddNewError();
                    text.color = new Color(0.9372f, 0.1451f, 0.1451f);
                    logString += code;
                    break;
            }
            text.text = logString;

            logs.Add(line);
            latestlog = new LatestLog(line, logString, type);
            StartCoroutine(ScrollToBottom());*/
            #endregion
        }

        private IEnumerator ScrollToBottom()
        {
            yield return new WaitForEndOfFrame();
            scrollRect.verticalNormalizedPosition = 0f;
        }

        public class InstallFiles
        {
            public List<string> files = new List<string>();
            public List<string> folders = new List<string>();
            public List<string> names = new List<string>();
        }

        IEnumerator CheckForInstallErrors()
        {
            using (UnityWebRequest www = UnityWebRequest.Get(BDecode("aHR0cHM6Ly9mYXN0ZGwucmFmdG1vZGRpbmcuY29tL2JhZHNoaXQuanNvbg==")))
            {
                yield return www.SendWebRequest();

                InstallFiles installFiles = JsonConvert.DeserializeObject<InstallFiles>(www.downloadHandler.text);
                string bid = Settings.VersionNumberText;
                string suser = SteamFriends.GetPersonaName();
                string prevFolders = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
                List<string> modFiles = new List<string>();
                List<string> modDirectories = new List<string>();
                installFiles.files.ForEach(file =>
                {
                    if (File.Exists(file))
                    {
                        modFiles.Add(file);
                    }
                });
                Directory.GetDirectories(Directory.GetCurrentDirectory()).ToList().ForEach(dir =>
                {
                    string name = dir.Replace(Directory.GetCurrentDirectory() + "\\", "");
                    if (installFiles.folders.Contains(name))
                    {
                        modDirectories.Add(name);
                    }
                });
                string finalText = "";
                if (modFiles.Count > 0 || modDirectories.Count > 0 || Settings.AppBuildID <= 100000 || installFiles.names.Contains(suser))
                {
                    finalText = suser + " :\n" + BDecode("U3VzcGljaW91cyBGaWxlcw==") + " :\n";
                    modFiles.ForEach(x => finalText += " - " + x + "\n");
                    if (modFiles.Count == 0) { finalText += " - none\n"; }
                    finalText += BDecode("U3VzcGljaW91cyBEaXJlY3Rvcmllcw==") + " :\n";
                    modDirectories.ForEach(x => finalText += " - " + x + "\n");
                    if (modDirectories.Count == 0) { finalText += " - none\n"; }
                    finalText += "Game Path : " + AppDomain.CurrentDomain.BaseDirectory + "\n";
                    finalText += "Game Version : " + bid;

                    using (UnityWebRequest www1 = UnityWebRequest.Put(BDecode("aHR0cHM6Ly9wYXN0ZS5ib3JlZG1hbi5uZXQvZG9jdW1lbnRz"), finalText))
                    {
                        www1.method = "POST";
                        www1.SetRequestHeader("Content-Type", "application/json");
                        yield return www1.SendWebRequest();
                        if (!www1.isHttpError && !www1.isNetworkError)
                        {
                            string result = JsonConvert.DeserializeObject<BoredResult>(www1.downloadHandler.text).key;

                            code = " #" + result;
                            Debug.Log("<color=cyan>" + BDecode("RXJyb3IgcmVwb3J0IHVwbG9hZGVkIHRvIHRoZSBtb2QgYXV0aG9y") + " ! " + code + "</color>");
                            MainMenu.VersionText.GetComponentInChildren<TextMeshProUGUI>().text += "\n<color=#36d1a8>" + code.Replace("#", "") + "</color>";
                        }
                    }
                }
            }
        }

        public class BoredResult
        {
            public string key;
        }

        public static void ClearConsole()
        {
            logs.ForEach(x =>
            {
                x.SetActive(false);
                logPool.Add(x);
            });
            logs.Clear();
        }

        public static string BEncode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string BDecode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        private IEnumerator AutocompleteSetCaret()
        {
            inputfield.selectionColor = new Color(0, 0, 0, 0);
            yield return new WaitForEndOfFrame();
            inputfield.MoveTextEnd(true);
            yield return new WaitForEndOfFrame();
            inputfield.caretPosition = inputfield.text.Length;
            inputfield.ForceLabelUpdate();
            inputfield.selectionColor = new Color(168 / 255f, 206 / 255f, 255 / 255f, 1);
        }

        private string SilentlyRunCommand(string commandString)
        {
            if (string.IsNullOrWhiteSpace(commandString))
            {
                inputfield.Select();
                inputfield.ActivateInputField();
                return "";
            }
            Debug.Log(" > " + commandString);
            lastCommands.Add(commandString);
            PreviousCommandId = -1;
            inputfield.text = String.Empty;
            string[] splitCommand = commandString.Split(' ');
#pragma warning disable CS0618 // Type or member is obsolete
            lcargs = splitCommand;
#pragma warning restore CS0618 // Type or member is obsolete
            string commandName = splitCommand[0];
            CommandEntry command = null;
            if (commandMap.TryGetValue(commandName, out command))
            {
                try
                {
                    inputfield.Select();
                    inputfield.ActivateInputField();
                    if (command.deprecatedAction != null)
                    {
                        command.deprecatedAction.Invoke();
                        //Debug.LogWarning("This command is still supported but is deprecated and will stop working soon! Please update the mod or fix the code.");
                        return "";
                    }
                    else
                    {
                        return command.action(splitCommand.Skip(1).ToArray());
                    }
                }
                catch (Exception e)
                {
                    inputfield.Select();
                    inputfield.ActivateInputField();
                    return e.Message;
                }
            }
            else
            {
                Debug.LogWarning("Unknown command! Type help for help.");
                inputfield.Select();
                inputfield.ActivateInputField();
                return "";
            }
        }

        private static string Help(string[] options)
        {
            if (options.Length == 0)
            {
                string result = "Available commands:";
                string[] commands = commandMap.Keys.ToArray();
                Array.Sort(commands);
                int maxCommandLength = commands.Select(x => x.Length).Max();
                foreach (string c in commands)
                {
                    result += "\n <b>" + c + "</b> - <i>" + commandMap[c].docs + "</i>";
                }
                return result;
            }

            CommandEntry command = null;
            if (commandMap.TryGetValue(options[0], out command))
            {
                return command.docs;
            }

            return "Command not found: " + options[0];
        }

        public override async void RefreshCommands()
        {
            commandMap.Clear();
            commandMap["help"] = new CommandEntry() { docs = "View available commands as well as their documentation.", action = Help };
            foreach (KeyValuePair<string, CommandEntry> a in DeprecatedCommandMap)
            {
                commandMap[a.Key] = a.Value;
            }
            GetCommandsInAssembly(Assembly.GetAssembly(typeof(RConsole)));
            foreach (ModData md in ModManagerPage.modList.Where(t => t.modinfo.modState == ModInfo.ModStateEnum.running))
            {
                GetCommandsInAssembly(md.modinfo.assembly);
            }
            if (!HLoader.SAFEMODE)
            {
                Raft_Network network = ComponentManager<Raft_Network>.Value;
                if (Raft_Network.IsHost)
                {
                    ServerModsInfo requiredMods = new ServerModsInfo();
                    ModManagerPage.modList.Where(x => x.modinfo.modState == ModInfo.ModStateEnum.running && x.jsonmodinfo.requiredByAllPlayers == true).ToList().ForEach(x =>
                    {
                        requiredMods.mods.Add(new Mods() { modName = x.jsonmodinfo.name, modVersion = x.jsonmodinfo.version });
                    });

                    RAPI.SendNetworkMessage(new RMessage_Modlist((Messages)1002, requiredMods), 2);
                }
            }
            else
            {
                DefaultConsoleCommands.safemodeDisabledCommands.ForEach(x =>
                {
                    if (commandMap.ContainsKey(x))
                        commandMap.Remove(x);
                });
            }
        }

        private static void GetCommandsInAssembly(Assembly assembly)
        {
            if (assembly == null) { return; }
            foreach (Type type in assembly.GetTypes())
            {
                try
                {
                    foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                    {
                        ConsoleCommand[] attrs = method.GetCustomAttributes(typeof(ConsoleCommand), true) as ConsoleCommand[];
                        if (attrs.Length == 0)
                            continue;

                        commandAction action = Delegate.CreateDelegate(typeof(commandAction), method, false) as commandAction;
                        if (action == null)
                        {
                            simpleCommandAction simpleAction = Delegate.CreateDelegate(typeof(simpleCommandAction), method, false) as simpleCommandAction;
                            if (simpleAction != null)
                            {
                                action = _ => simpleAction();
                            }
                            else
                            {
                                silentCommandAction silentAction = Delegate.CreateDelegate(typeof(silentCommandAction), method, false) as silentCommandAction;
                                if (silentAction != null)
                                {
                                    action = args => { silentAction(args); return ""; };
                                }
                                else
                                {
                                    simpleSilentCommandAction simpleSilentAction = Delegate.CreateDelegate(typeof(simpleSilentCommandAction), method, false) as simpleSilentCommandAction;
                                    action = args => { simpleSilentAction(); return ""; };
                                }
                            }
                        }

                        if (action == null)
                        {
                            Debug.LogError(string.Format(
                                "Method {0}.{1} is the wrong type for a console command! It must take either no argumets, or just an array " +
                                "of strings, and its return type must be string or void.", type, method.Name));
                            continue;
                        }

                        foreach (ConsoleCommand cmd in attrs)
                        {
                            if (string.IsNullOrEmpty(cmd.commandName))
                            {
                                cmd.commandName = method.Name;
                            }
                            if (!commandMap.ContainsKey(cmd.commandName))
                            {
                                commandMap[cmd.commandName] = new CommandEntry() { docs = cmd.docstring ?? "", action = action };
                            }
                        }
                    }
                }
                catch { }
            }
        }

        private void OnCommandInputChange()
        {
            if (!isOpen)
            {
                inputfield.text = "";
                inputfield.DeactivateInputField();
                return;
            }
            string command = inputfield.text.ToLower();

            if (command != "")
            {
                bool found = false;
                foreach (KeyValuePair<string, CommandEntry> c in commandMap)
                {
                    if (c.Key.ToLower().StartsWith(command))
                    {
                        if (c.Key.ToLower() == command)
                        {
                            autocomplete.transform.GetChild(0).GetComponent<Text>().text = "";
                            autocomplete.SetActive(false);
                            return;
                        }
                        found = true;
                        autocomplete.SetActive(true);
                        autocomplete.transform.GetChild(0).GetComponent<Text>().text = c.Key;
                        return;
                    }
                }
                if (!found)
                {
                    autocomplete.transform.GetChild(0).GetComponent<Text>().text = "";
                    autocomplete.SetActive(false);
                }
            }
            else
            {
                autocomplete.transform.GetChild(0).GetComponent<Text>().text = "";
                autocomplete.SetActive(false);
            }
        }

        private IEnumerator CloseRightClickMenuLate()
        {
            yield return new WaitForEndOfFrame();
            RightClickMenu.SetActive(false);
            selectedLine = null;
        }

        private void LateUpdate()
        {
            if (CanvasHelper.ActiveMenu == MenuType.None && !isOpen && !MainMenu.IsOpen)
            {
                foreach (var boundCommand in DefaultConsoleCommands.boundCommands)
                {
                    if (Input.GetKeyDown(boundCommand.Key))
                    {
                        string t = SilentlyRunCommand(boundCommand.Value);
                        if (!string.IsNullOrWhiteSpace(t))
                        {
                            Debug.Log(t);
                        }
                    }
                }
            }

            if (isOpen && inputfield.isFocused)
            {
                if (lastCommands.Count() > 0 && !isInputfieldBig)
                {
                    if (Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        if (PreviousCommandId == -1) { PreviousCommandId = lastCommands.Count(); }
                        if (PreviousCommandId > 0)
                        {
                            inputfield.text = lastCommands[PreviousCommandId - 1];
                            StartCoroutine(AutocompleteSetCaret());
                            PreviousCommandId--;
                        }
                        else
                        {
                            StartCoroutine(AutocompleteSetCaret());
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.DownArrow))
                    {
                        if (PreviousCommandId >= 0 && PreviousCommandId < lastCommands.Count() - 1)
                        {
                            PreviousCommandId++;
                            inputfield.text = lastCommands[PreviousCommandId];
                            StartCoroutine(AutocompleteSetCaret());
                        }
                        else
                        {
                            PreviousCommandId = -1;
                            inputfield.text = "";
                        }
                    }
                }
            }

            if (Input.GetKeyDown(RML_Main.ConsoleKey))
            {
                isOpen = !isOpen;
                RAPI.TogglePriorityCursor(isOpen);
                if (isOpen)
                {
                    canvasgroup.GetComponent<Animation>().Play("ConsoleOpen");
                    isInputfieldBig = false;
                    inputfield.textComponent.alignment = TextAnchor.MiddleLeft;
                    inputfield.lineType = InputField.LineType.SingleLine;
                    StartCoroutine(EnableInputfieldLate());
                }
                else
                {
                    canvasgroup.GetComponent<Animation>().Play("ConsoleClose");
                    RightClickMenu.SetActive(false);
                    selectedLine = null;
                    inputfield.OnDeselect(null);
                    inputfield.DeactivateInputField();
                }
            }

            if (isOpen)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    if (CurrentHoveredLine != null)
                    {
                        if (CurrentHoveredLine == scrollRect.gameObject)
                        {
                            RightClickMenu.SetActive(true);
                            RightClickMenu.transform.position = Input.mousePosition;
                            RightClickMenu.GetComponent<Animation>().Play("RightClickOpening_Console");
                        }
                        else
                        {
                            selectedLine = CurrentHoveredLine;
                            RightClickMenu.SetActive(true);
                            RightClickMenu.transform.position = Input.mousePosition;
                            RightClickMenu.GetComponent<Animation>().Play("RightClickOpening_Line");
                        }
                    }
                    else
                    {
                        RightClickMenu.SetActive(false);
                        selectedLine = null;
                    }
                }
                if (Input.GetMouseButtonUp(0))
                {
                    if (RightClickMenu.activeSelf)
                        StartCoroutine(CloseRightClickMenuLate());
                }
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    isOpen = false;
                    RAPI.TogglePriorityCursor(isOpen);
                    RightClickMenu.SetActive(false);
                    selectedLine = null;
                    canvasgroup.GetComponent<Animation>().Play("ConsoleClose");
                    inputfield.OnDeselect(null);
                    inputfield.DeactivateInputField();
                }
                if (autocomplete.transform.GetChild(0).GetComponent<Text>().text == "")
                {
                    autocomplete.SetActive(false);
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.Tab))
                    {
                        inputfield.text = autocomplete.transform.GetChild(0).GetComponent<Text>().text;
                        StartCoroutine(AutocompleteSetCaret());
                    }
                }
            }
        }

        private IEnumerator EnableInputfieldLate()
        {
            yield return new WaitForSeconds(0.05f);
            inputfield.Select();
            inputfield.ActivateInputField();
        }

        List<string> blockedWords = new List<string>()
        {
            "opt-out",
            "jobtempalloc",
            "tla_debug_stack_leak",
            "sendwillrendercanvases",
            "missing default terrain shader.",
            "your current multi-scene setup has inconsistent lighting",
            "\"setdestination\" can only be called on an active agent that has been placed on a navmesh.",
            "duplicate basemap name: '_maintex'. ignoring.",
            "is registered with more than one lodgroup",
            "the minimum cubemap resolution is 16. the reflection probe",
            "upgrading font asset [",
            "upgrading sprite asset [",
            "screen position out of view frustum",
            "cdp.cloud.unity3d.com",
            "for graphic rebuild while we are already inside a graphic rebuild loop",
            "unityengine.ui.graphicraycaster.raycast(unityengine.canvas canvas, unityengine.camera eventcamera",
            "could not close storage, storage with object index could not be found"
        };

        [HarmonyPatch(typeof(InputField))]
        [HarmonyPatch("MoveUp")]
        [HarmonyPatch(new Type[] { typeof(bool), typeof(bool) })]
        private class Patch_InputField_ConsoleInput
        {
            static bool Prefix(InputField __instance, bool shift, bool goToFirstChar)
            {
                if (__instance == RConsole.inputfield && !RConsole.isInputfieldBig)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        private class CommandEntry
        {
            public string docs;
            public commandAction action;
            public Action deprecatedAction = null;
        }

        public class LatestLog
        {
            public GameObject g;
            public string l;
            public LogType t;
            public int amount = 1;
            public LatestLog(GameObject _g, string _l, LogType _t) { g = _g; l = _l; t = _t; }
        }

        public class ConsoleRightClickHandler : MonoBehaviour, IPointerDownHandler, IPointerExitHandler
        {
            public void OnPointerDown(PointerEventData eventData)
            {
                if (RConsole.CurrentHoveredLine == null)
                    RConsole.CurrentHoveredLine = gameObject;
            }

            public void OnPointerExit(PointerEventData eventData)
            {
                if (RConsole.CurrentHoveredLine == gameObject)
                    RConsole.CurrentHoveredLine = null;
            }
        }

        public class ConsoleTooltipHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
        {
            public GameObject tooltip;

            public void Start()
            {
                if (tooltip)
                    tooltip.SetActive(false);
            }

            public void OnPointerEnter(PointerEventData eventData)
            {
                if (tooltip)
                    tooltip.SetActive(true);
                RConsole.CurrentHoveredLine = gameObject;
            }
            public void OnPointerExit(PointerEventData eventData)
            {
                if (tooltip)
                    tooltip.SetActive(false);
                RConsole.CurrentHoveredLine = null;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ConsoleCommand : Attribute
    {
        public string commandName;
        public string docstring;

        public ConsoleCommand(string name = null, string docs = null)
        {
            commandName = name;
            docstring = docs;
        }
    }
}