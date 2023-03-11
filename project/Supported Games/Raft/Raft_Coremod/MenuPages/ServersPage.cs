using FMOD;
using HarmonyLib;
using HMLLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RaftModLoader;
using ShellLink.Structures;
using SocketIO;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace RaftModLoader
{
    public class ServersPage : MenuPage
    {
        public static ServersPage instance;
        public GameObject serverInfoObj;
        public GameObject serverInfoPluginEntry;
        public static GameObject serverlistWaitWindow;
        public static Button serverlistRefreshBtn;
        public static GameObject serverlistContent;
        public static GameObject ServerEntryPrefab;
        public static GameObject DediServerEntryPrefab;
        public static Text serverlistStatus;
        public static TextMeshProUGUI serversAmount;
        public static TextMeshProUGUI serversAmountTooltip;
        public static string lastPassword = "";
        public static string lastServerInfoBackgroundUrl = "";
        public static string searchServersValue = "";
        public static Sprite defaultIcon;
        public static Sprite defaultBanner;

        public void Awake()
        {
            instance = this;
            ServerEntryPrefab = HLib.bundle.LoadAsset<GameObject>("ServerEntry");
            DediServerEntryPrefab = HLib.bundle.LoadAsset<GameObject>("DediServerEntry");
            serverlistContent = GameObject.Find("RMLServerListContent").gameObject;
            serverlistWaitWindow = transform.Find("ServerScrollView").Find("Viewport").Find("ConnectWindow").gameObject;
            serverlistWaitWindow.transform.Find("CloseBtn").GetComponent<Button>().onClick.AddListener(() =>
            {
                ComponentManager<Raft_Network>.Value.CancelTryingToConnect();
                serverlistWaitWindow.SetActive(false);
            });

            serverlistStatus = transform.Find("ServerScrollView").Find("Viewport").Find("StatusText").GetComponent<Text>();

            transform.Find("KnowMoreRDS").GetComponent<Button>().onClick.AddListener(() => Application.OpenURL("https://www.raftmodding.com"));
            serverlistWaitWindow.SetActive(false);
            serverlistRefreshBtn = transform.Find("RML_RefreshServerList").GetComponent<Button>();
            serverlistRefreshBtn.onClick.AddListener(RefreshServers);
            serverInfoObj = transform.Find("ServerScrollView").Find("Viewport").Find("ServerInfo").gameObject;
            transform.Find("SearchBar").GetComponent<TMP_InputField>().onValueChanged.AddListener((val) => OnSearchServersInputChanged(val));
            transform.Find("SearchBar").GetComponent<TMP_InputField>().onEndEdit.AddListener((val) => OnSearchServersInputChanged(val));
            serverInfoObj.SetActive(false);
            serverInfoPluginEntry = HLib.bundle.LoadAsset<GameObject>("ServerInfoPluginEntry");
            serversAmount = GameObject.Find("RMLMainMenuLButton_Servers").transform.Find("Amount").Find("AmountValue").GetComponent<TextMeshProUGUI>();
            serversAmountTooltip = GameObject.Find("RMLMainMenuLButton_Servers").transform.Find("Amount").Find("Tooltip").GetComponentInChildren<TextMeshProUGUI>();
            serversAmount.gameObject.AddComponent<TooltipHandler>().tooltip = GameObject.Find("RMLMainMenuLButton_Servers").transform.Find("Amount").Find("Tooltip").gameObject;
            defaultIcon = HLib.bundle.LoadAsset<Sprite>("defaultIcon");
            defaultBanner = HLib.bundle.LoadAsset<Sprite>("defaultBanner");
        }

        public static void OnSearchServersInputChanged(string val)
        {
            searchServersValue = val;
            foreach (Transform t in serverlistContent.transform)
            {
                t.gameObject.SetActive(HUtils.StripRichText(t.Find("Name").GetComponent<TextMeshProUGUI>().text).ToLower().Contains(val.ToLower()));
            }
        }

        public static async Task<JToken> RetrieveServers()
        {
            try
            {
                string url = "https://master.raftmodding.com/api/servers";
                DateTime now = DateTime.Now;
                using (UnityWebRequest www = UnityWebRequest.Get(url))
                {
                    www.SendWebRequest();
                    bool cancelTimeOut = false;
                    while (!cancelTimeOut && !www.isDone && !www.isNetworkError && !www.isHttpError)
                    {
                        if (now.AddSeconds(5) <= DateTime.Now)
                            cancelTimeOut = true;
                        await Task.Delay(1);
                    }

                    if (!www.isNetworkError && !www.isHttpError && string.IsNullOrEmpty(www.error))
                    {
                        string json = www.downloadHandler.text;
                        JToken servers = JsonConvert.DeserializeObject<JToken>(json);
                        return servers;
                    }
                    return null;
                }
            }
            catch { return null; }
        }

        async Task<long> GetPingTimeAsync(string host, int port)
        {
            try
            {
                var stopwatch = new Stopwatch();
                using (var client = new TcpClient())
                {
                    stopwatch.Start();
                    await client.ConnectAsync(host, port);
                    stopwatch.Stop();
                    client.Close();
                    return stopwatch.ElapsedMilliseconds;
                }
            }
            catch
            {
                return -1;
            }
        }

        async Task<long> GetSteamPingTimeAsync(CSteamID steamid)
        {
            await Task.Delay(UnityEngine.Random.Range(50, 500));
            return -1;
        }

        public async void RefreshServers()
        {
            foreach (Transform t in serverlistContent.transform)
            {
                Destroy(t.gameObject);
            }

            serverlistStatus.text = "Retrieving servers...";
            serverlistStatus.gameObject.SetActive(true);
            try
            {
                transform.Find("SearchBar").GetComponent<TMP_InputField>().text = "";
                JToken result = await RetrieveServers();
                if (result == null || result["servers"] == null)
                {
                    serverlistStatus.text = "The Master server is currently offline.\nPlease try again later!";
                    serverlistStatus.gameObject.SetActive(true);
                    return;
                }

                JArray servers = result["servers"] as JArray;
                serverlistStatus.text = "";
                serverlistStatus.gameObject.SetActive(false);
                int serversAmount = servers.Count;
                if (serversAmount <= 0)
                {
                    serverlistStatus.text = "No servers found!";
                    serverlistStatus.gameObject.SetActive(true);
                }

                for (int i = 0; i < servers.Count; i++)
                {
                    try
                    {
                        bool dedicated = (bool)servers[i]["dedicated"];
                        string name = servers[i]["name"].ToString();
                        string ip = servers[i]["ip"].ToString();
                        uint accountid = 0;
                        CSteamID steamid = new CSteamID();
                        bool rtcp = ip.Contains(".") && ip.Contains(":");
                        if (!rtcp)
                        {
                            uint.TryParse(ip, out accountid);
                            steamid = new CSteamID(new AccountID_t(accountid), EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeIndividual);
                        }
                        GameObject item = Instantiate(dedicated ? DediServerEntryPrefab : ServerEntryPrefab, Vector3.zero, Quaternion.identity, serverlistContent.transform);
                        if (dedicated)
                        {
                            item.transform.Find("Official").gameObject.SetActive(servers[i]["tag"].ToString().Contains("official"));
                            item.transform.Find("Partner").gameObject.SetActive(servers[i]["tag"].ToString().Contains("partner"));
                        }
                        string serverName = dedicated ? name.RML_InsertEmoji() : name;
                        string serverPlayers = "Players : " + servers[i]["players"].ToString() + " / " + servers[i]["maxplayers"].ToString();
                        string serverGamemode = "Gamemode : " + servers[i]["gamemode"].ToString();
                        string serverFriendlyFire = "Friendly Fire : " + (((bool)servers[i]["friendlyfire"]) ? "Yes" : "No");
                        string dedicatedServer = "Dedicated : " + (dedicated ? "Yes" : "No");
                        string ping = "Ping : ... ms";
                        string pluginslist = servers[i]["plugins"].ToString();
                        bool hasPassword = (bool)servers[i]["haspassword"];
                        string[] ipParsed = ip.Split(':');
                        Task<long> pingTask = rtcp ? GetPingTimeAsync(ipParsed[0], int.Parse(ipParsed[1])) : GetSteamPingTimeAsync(steamid);
                        pingTask.ContinueWith(p =>
                        {
                            try
                            {
                                ping = $"Ping : {p.Result} ms";
                                if (item != null)
                                {
                                    item.transform.Find("Ping").GetComponent<Text>().text = p.Result + "ms";
                                }
                            }
                            catch (Exception e)
                            {
                                Debug.LogWarning("Could not retrieve ping for " + ip);
                            }
                        });

                        item.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = serverName;
                        item.transform.Find("Players").GetComponent<Text>().text = servers[i]["players"].ToString() + "/" + servers[i]["maxplayers"].ToString();
                        item.transform.Find("Ping").GetComponent<Text>().text = "...";

                        string iconUrl = servers[i]["iconurl"].ToString();
                        string bannerUrl = servers[i]["bannerurl"].ToString();
                        //string backgroundUrl = e.data["servers"][i]["serverCustomLoadingScreenUrl"].str;
                        //lastServerInfoBackgroundUrl = backgroundUrl;
                        item.transform.Find("Buttons").Find("InfoBtn").gameObject.AddComponent<TooltipHandler>().tooltip = item.transform.Find("Buttons").Find("InfoBtn").Find("Tooltip").gameObject;
                        item.transform.Find("Buttons").Find("ConnectBtn").gameObject.AddComponent<TooltipHandler>().tooltip = item.transform.Find("Buttons").Find("ConnectBtn").Find("Tooltip").gameObject;
                        if (!string.IsNullOrWhiteSpace(iconUrl) && iconUrl.Length > 5)
                        {
                            HUtils.DownloadUncachedTexture(iconUrl).ContinueWith((t) =>
                            {
                                item.transform.Find("ServerIconMask").Find("ServerIcon").GetComponent<RawImage>().texture = t.Result;
                                item.transform.Find("_icon").GetComponent<RawImage>().texture = t.Result;
                            });
                        }
                        else
                        {
                            item.transform.Find("_icon").GetComponent<RawImage>().texture = defaultIcon.texture;
                        }

                        if (!string.IsNullOrWhiteSpace(bannerUrl) && bannerUrl.Length > 5)
                        {
                            HUtils.DownloadUncachedTexture(bannerUrl).ContinueWith((t) =>
                            {
                                item.transform.Find("_banner").GetComponent<RawImage>().texture = t.Result;
                            });
                        }
                        else
                        {
                            item.transform.Find("_banner").GetComponent<RawImage>().texture = defaultBanner.texture;
                        }

                        item.transform.Find("Buttons").Find("InfoBtn").GetComponent<Button>().onClick.AddListener(() =>
                        {
                            if (serverlistWaitWindow.activeSelf) { return; }
                            instance.serverInfoObj.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = serverName;
                            instance.serverInfoObj.transform.Find("Players").GetComponent<TextMeshProUGUI>().text = serverPlayers;
                            instance.serverInfoObj.transform.Find("Gamemode").GetComponent<TextMeshProUGUI>().text = serverGamemode;
                            instance.serverInfoObj.transform.Find("Friendlyfire").GetComponent<TextMeshProUGUI>().text = serverFriendlyFire;
                            instance.serverInfoObj.transform.Find("Dedicated").GetComponent<TextMeshProUGUI>().text = dedicatedServer;
                            instance.serverInfoObj.transform.Find("Ping").GetComponent<TextMeshProUGUI>().text = ping;
                            instance.serverInfoObj.transform.Find("BannerMask").Find("Banner").GetComponent<RawImage>().texture = item.transform.Find("_banner").GetComponent<RawImage>().texture;
                            instance.serverInfoObj.transform.Find("IconMask").Find("Icon").GetComponent<RawImage>().texture = item.transform.Find("_icon").GetComponent<RawImage>().texture;

                            string[] plugins = pluginslist.Split(new string[] { ";" }, StringSplitOptions.None);
                            foreach (Transform t in instance.serverInfoObj.transform.Find("PluginsList").Find("Scroll View").Find("Viewport").Find("Content"))
                            {
                                Destroy(t.gameObject);
                            }
                            if (!string.IsNullOrWhiteSpace(pluginslist) && plugins.Length > 0)
                            {
                                instance.serverInfoObj.transform.Find("PluginsList").Find("Scroll View").Find("Viewport").Find("NoPlugins").GetComponent<TextMeshProUGUI>().text = "";
                                foreach (string plugin in plugins)
                                {
                                    GameObject plobj = Instantiate(instance.serverInfoPluginEntry, Vector3.zero, Quaternion.identity, instance.serverInfoObj.transform.Find("PluginsList").Find("Scroll View").Find("Viewport").Find("Content"));
                                    TextMeshProUGUI text = plobj.transform.Find("Text").GetComponent<TextMeshProUGUI>();
                                    if (dedicated)
                                    {
                                        if (plugin.StartsWith("P:"))
                                        {
                                            text.text = $"<color=#2b81c2>{plugin.Substring(2)}</color>";
                                        }
                                        else if (plugin.StartsWith("M:"))
                                        {
                                            text.text = $"<color=#2bc235>{plugin.Substring(2)}</color>";
                                        }
                                        else
                                        {
                                            // Old servers
                                            text.text = $"<color=#2b81c2>{plugin}</color>";
                                        }
                                    }
                                    else
                                    {
                                        text.text = plugin;
                                    }
                                }
                            }
                            else
                            {
                                if (dedicated)
                                    instance.serverInfoObj.transform.Find("PluginsList").Find("Scroll View").Find("Viewport").Find("NoPlugins").GetComponent<TextMeshProUGUI>().text = "This server isn't running any resources.";
                                else
                                    instance.serverInfoObj.transform.Find("PluginsList").Find("Scroll View").Find("Viewport").Find("NoPlugins").GetComponent<TextMeshProUGUI>().text = "This party isn't running any mods.";
                            }
                            if (dedicated)
                                instance.serverInfoObj.transform.Find("PluginsList").Find("Plugins").GetComponent<TextMeshProUGUI>().text = "Server Resources : (<color=#2b81c2>Plugins</color>, <color=#2bc235>Mods</color>)";
                            else
                                instance.serverInfoObj.transform.Find("PluginsList").Find("Plugins").GetComponent<TextMeshProUGUI>().text = "Party Mods : ";

                            instance.serverInfoObj.SetActive(true);
                        });


                        item.transform.Find("Buttons").Find("ConnectBtn").GetComponent<Button>().onClick.AddListener(() =>
                        {
                            instance.serverInfoObj.SetActive(false);
                            serverlistWaitWindow.transform.Find("Password").Find("TextMeshPro - InputField").GetComponent<TMP_InputField>().text = "";
                            serverlistWaitWindow.transform.Find("BannerMask").Find("Banner").GetComponent<RawImage>().texture = item.transform.Find("_banner").GetComponent<RawImage>().texture; ;
                            serverlistWaitWindow.transform.Find("IconMask").Find("Icon").GetComponent<RawImage>().texture = item.transform.Find("_icon").GetComponent<RawImage>().texture;
                            if (hasPassword)
                            {
                                serverlistWaitWindow.transform.Find("Connecting").Find("ServerName").GetComponent<TextMeshProUGUI>().text = "Connecting to " + serverName;
                                serverlistWaitWindow.transform.Find("Password").Find("ServerName").GetComponent<TextMeshProUGUI>().text = "Connecting to " + serverName;
                                serverlistWaitWindow.transform.Find("Connecting").gameObject.SetActive(false);
                                serverlistWaitWindow.transform.Find("Error").gameObject.SetActive(false);
                                serverlistWaitWindow.transform.Find("Password").gameObject.SetActive(true);
                                serverlistWaitWindow.transform.Find("Password").Find("Button").GetComponent<Button>().onClick.RemoveAllListeners();
                                serverlistWaitWindow.transform.Find("Password").Find("Button").GetComponent<Button>().onClick.AddListener(() =>
                                {
                                    serverlistWaitWindow.transform.Find("Connecting").gameObject.SetActive(true);
                                    serverlistWaitWindow.transform.Find("Error").gameObject.SetActive(false);
                                    serverlistWaitWindow.transform.Find("Password").gameObject.SetActive(false);
                                    serverlistWaitWindow.transform.Find("Connecting").Find("ServerName").GetComponent<TextMeshProUGUI>().text = "Connecting to " + serverName;
                                    serverlistWaitWindow.transform.Find("Password").Find("ServerName").GetComponent<TextMeshProUGUI>().text = "Connecting to " + serverName;
                                    if (rtcp)
                                    {
                                        if (!string.IsNullOrWhiteSpace(ip) && !string.IsNullOrWhiteSpace(RTCP.myIp) && ip.StartsWith(RTCP.myIp))
                                            ip = ip.Replace(RTCP.myIp, "127.0.0.1");
                                        ConnectToServer(ip, serverlistWaitWindow.transform.Find("Password").Find("TextMeshPro - InputField").GetComponent<TMP_InputField>().text, true, serverName);
                                    }
                                    else
                                    {
                                        ConnectToServer(steamid, serverlistWaitWindow.transform.Find("Password").Find("TextMeshPro - InputField").GetComponent<TMP_InputField>().text, true);
                                    }
                                });
                                serverlistWaitWindow.SetActive(true);
                            }
                            else
                            {
                                serverlistWaitWindow.transform.Find("Connecting").gameObject.SetActive(true);
                                serverlistWaitWindow.transform.Find("Error").gameObject.SetActive(false);
                                serverlistWaitWindow.transform.Find("Password").gameObject.SetActive(false);
                                serverlistWaitWindow.transform.Find("Connecting").Find("ServerName").GetComponent<TextMeshProUGUI>().text = "Connecting to " + serverName;
                                serverlistWaitWindow.transform.Find("Password").Find("ServerName").GetComponent<TextMeshProUGUI>().text = "Connecting to " + serverName;
                                if (rtcp)
                                {
                                    if (!string.IsNullOrWhiteSpace(ip) && !string.IsNullOrWhiteSpace(RTCP.myIp) && ip.StartsWith(RTCP.myIp))
                                        ip = ip.Replace(RTCP.myIp, "127.0.0.1");
                                    ConnectToServer(ip, "", true, serverName);
                                }
                                else
                                {
                                    ConnectToServer(steamid, "", true);
                                }
                                serverlistWaitWindow.SetActive(true);
                            }
                        });
                        if (!hasPassword)
                        {
                            item.transform.Find("HasPassword").gameObject.SetActive(false);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning("An error occured while retrieving a server in the serverlist!\n" + ex.ToString());
                    }
                }
            }
            catch
            {
                foreach (Transform t in serverlistContent.transform)
                {
                    Destroy(t.gameObject);
                }
                serverlistStatus.text = "An error occured while fetching servers";
                serverlistStatus.gameObject.SetActive(true);
            }

            foreach (Transform t in serverlistContent.transform)
            {
                if (t.name.StartsWith("DediServerEntry"))
                    t.SetAsFirstSibling();
            }
            serversAmount.text = serverlistContent.transform.childCount.ToString();
            serversAmountTooltip.text = serversAmount.text + " Servers";
        }

        public static void ConnectToServer(CSteamID sid, string password, bool showUI)
        {
            if (!RAPI.IsCurrentSceneMainMenu())
            {
                Debug.LogWarning("You are already playing on a server! Please disconnect before joining another server!");
                serverlistWaitWindow.SetActive(showUI);
                serverlistWaitWindow.transform.Find("Connecting").gameObject.SetActive(false);
                serverlistWaitWindow.transform.Find("Password").gameObject.SetActive(false);
                serverlistWaitWindow.transform.Find("Error").gameObject.SetActive(true);
                serverlistWaitWindow.transform.Find("Error").Find("Status").GetComponent<TextMeshProUGUI>().text = "You are already playing on a server!";
                return;
            }
            ComponentManager<Raft_Network>.Value.CancelTryingToConnect();
            string username = SteamFriends.GetFriendPersonaName(sid);
            if (password != "")
            {
                Debug.Log("Connecting to " + username + " with the specified password...");
            }
            else
            {
                Debug.Log("Connecting to " + username + "...");
            }

            ComponentManager<Raft_Network>.Value.TryToJoinGame(sid, password);
            if (showUI)
            {
                serverlistWaitWindow.SetActive(true);
            }
        }

        public static async void ConnectToServer(string ipport, string password, bool showUI, string name)
        {
            lastPassword = password;
            string ip = ipport.Split(':')[0];
            int port = int.Parse(ipport.Split(':')[1]);
            if (!RAPI.IsCurrentSceneMainMenu())
            {
                Debug.LogWarning("You are already playing on a server! Please disconnect before joining another server!");
                serverlistWaitWindow.SetActive(showUI);
                serverlistWaitWindow.transform.Find("Connecting").gameObject.SetActive(false);
                serverlistWaitWindow.transform.Find("Password").gameObject.SetActive(false);
                serverlistWaitWindow.transform.Find("Error").gameObject.SetActive(true);
                serverlistWaitWindow.transform.Find("Error").Find("Status").GetComponent<TextMeshProUGUI>().text = "You are already playing on a server!";
                return;
            }
            ComponentManager<Raft_Network>.Value.CancelTryingToConnect();
            if (password != "")
            {
                Debug.Log("Connecting to " + name + " with the specified password...");
            }
            else
            {
                Debug.Log("Connecting to " + name + "...");
            }
            bool connected = await RTCP.JoinServer(IPAddress.Parse(ip), port);
            if (connected)
            {
                CSteamID csteamid = new CSteamID(new AccountID_t(1), EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeIndividual);
                Raft_Network network = ComponentManager<Raft_Network>.Value;
                network.CancelTryingToConnect();
                network.TryToJoinGame(csteamid, password);
            }
            else
            {
                serverlistWaitWindow.transform.Find("Connecting").gameObject.SetActive(false);
                serverlistWaitWindow.transform.Find("Password").gameObject.SetActive(false);
                TextMeshProUGUI TextMeshProUGUI = serverlistWaitWindow.transform.Find("Error").Find("Status").GetComponent<TextMeshProUGUI>();
                serverlistWaitWindow.transform.Find("Password").Find("TextMeshPro - InputField").GetComponent<TMP_InputField>().text = "";
                serverlistWaitWindow.transform.Find("Error").gameObject.SetActive(true);
                TextMeshProUGUI.text = "Server Timed Out!";
            }
            if (showUI)
            {
                serverlistWaitWindow.SetActive(true);
            }
        }

        private void OnSceneChanged(Scene scene1, Scene scene2)
        {
            Raft_Network.OnJoinResult += OnRaftJoinResult;
        }
        public static async void OnRaftJoinResult(CSteamID remoteID, InitiateResult result) => OnJoinResult(remoteID, result);
        public static async void OnJoinResult(CSteamID remoteID, InitiateResult result, string message = "")
        {
            if ((int)result == 70) return;
            if ((int)result == 69 && !Raft_Network.InMenuScene)
            {
                while (!Raft_Network.InMenuScene)
                {
                    await Task.Delay(100);
                }
                await Task.Delay(500);
                MainMenu.instance.CurrentPage = "Servers";
                if (!MainMenu.IsOpen)
                    MainMenu.instance.OpenMenu();
                if (!serverlistWaitWindow.activeSelf)
                    serverlistWaitWindow.SetActive(true);
            }
            if (!string.IsNullOrWhiteSpace(message))
            {
                Debug.Log("The server stopped the connection ! Reason : " + message);
            };
            if (!serverlistWaitWindow.activeSelf)
            {
                return;
            }
            serverlistWaitWindow.transform.Find("Connecting").gameObject.SetActive(false);
            serverlistWaitWindow.transform.Find("Password").gameObject.SetActive(false);
            serverlistWaitWindow.transform.Find("Error").gameObject.SetActive(false);
            TextMeshProUGUI TextMeshProUGUI = serverlistWaitWindow.transform.Find("Error").Find("Status").GetComponent<TextMeshProUGUI>();
            serverlistWaitWindow.transform.Find("Password").Find("TextMeshPro - InputField").GetComponent<TMP_InputField>().text = "";
            switch (result)
            {
                case InitiateResult.Fail:
                    serverlistWaitWindow.transform.Find("Error").gameObject.SetActive(true);
                    TextMeshProUGUI.text = "The server doesn't accept players!";
                    break;
                case InitiateResult.Fail_TimeOut:
                    serverlistWaitWindow.transform.Find("Error").gameObject.SetActive(true);
                    TextMeshProUGUI.text = "The Server Timed Out!";
                    break;
                case InitiateResult.Fail_NotHost:
                    serverlistWaitWindow.transform.Find("Error").gameObject.SetActive(true);
                    TextMeshProUGUI.text = "The server is full!";
                    break;
                case InitiateResult.Fail_AlreadyOnServer:
                    serverlistWaitWindow.transform.Find("Error").gameObject.SetActive(true);
                    TextMeshProUGUI.text = "You are already playing on the server!";
                    break;
                case InitiateResult.Fail_AlreadyConnecting:
                    serverlistWaitWindow.transform.Find("Error").gameObject.SetActive(true);
                    TextMeshProUGUI.text = "You are already connecting to this server!";
                    break;
                case InitiateResult.Fail_MissmatchAppBuildID:
                    serverlistWaitWindow.transform.Find("Error").gameObject.SetActive(true);
                    TextMeshProUGUI.text = "You don't have the same Raft Version as the server!";
                    break;
                case InitiateResult.Fail_WrongPassword:
                    serverlistWaitWindow.transform.Find("Password").gameObject.SetActive(true);
                    serverlistWaitWindow.transform.Find("Password").Find("Button").GetComponent<Button>().onClick.RemoveAllListeners();
                    serverlistWaitWindow.transform.Find("Password").Find("Button").GetComponent<Button>().onClick.AddListener(() =>
                    {
                        serverlistWaitWindow.transform.Find("Connecting").gameObject.SetActive(true);
                        serverlistWaitWindow.transform.Find("Password").gameObject.SetActive(false);
                        ConnectToServer(remoteID, serverlistWaitWindow.transform.Find("Password").Find("TextMeshPro - InputField").GetComponent<TMP_InputField>().text, true);
                        serverlistWaitWindow.SetActive(true);
                    });
                    break;
                case InitiateResult.Fail_NotFriendWithHost:
                    serverlistWaitWindow.transform.Find("Error").gameObject.SetActive(true);
                    TextMeshProUGUI.text = "The server is whitelisted!";
                    break;
                case InitiateResult.Success:
                    Traverse.Create(typeof(Raft_Network)).Field("isHost").SetValue(false);
                    Traverse.Create(ComponentManager<SaveAndLoad>.Value).Field("autoSaveTimer").SetValue(0);
                    TextMeshProUGUI.text = "Downloading World...";
                    serverlistWaitWindow.SetActive(false);
                    MainMenu.CloseMenu();
                    break;
                case (InitiateResult)69:
                    serverlistWaitWindow.transform.Find("Error").gameObject.SetActive(true);
                    TextMeshProUGUI.text = message;
                    break;
                default:
                    TextMeshProUGUI.text = result.ToString();
                    break;
            }
        }
    }

    [HarmonyPatch(typeof(Raft_Network))]
    [HarmonyPatch("OnP2PSessionConnectFail")]
    public class ServerSystem_OnP2PSessionConnectFail
    {
        public static void Prefix(P2PSessionConnectFail_t callback)
        {
            if (ServersPage.serverlistWaitWindow.activeSelf)
            {
                ServersPage.OnJoinResult(new CSteamID(), InitiateResult.Fail_TimeOut);
            }
        }
    }
}