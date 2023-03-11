using HarmonyLib;
using HMLLibrary;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RaftModLoader
{
    public class NetworkModManager : MonoBehaviour
    {
        public ServerModsInfo currentServerMods = new ServerModsInfo();
        public static Dictionary<CSteamID, ServerModsInfo> remoteUsersMods = new Dictionary<CSteamID, ServerModsInfo>();
        public static ServerModsInfo missingMods;

        public static GameObject missingModsMenu;
        public static GameObject missingModEntry;
        public static GameObject modList;

        public static Raft_Network network;

        void Start()
        {
            missingModsMenu = Instantiate(HLib.bundle.LoadAsset<GameObject>("MissingModsMenu"), transform);
            missingModEntry = HLib.bundle.LoadAsset<GameObject>("MissingModEntry");
            modList = missingModsMenu.transform.Find("BG").Find("View").Find("ModScrollView").Find("Viewport").Find("Modlist").gameObject;
            missingModsMenu.transform.Find("BG").Find("TopBar").Find("MissingModsCloseBtn").GetComponent<Button>().onClick.AddListener(() =>
            {
                missingModsMenu.SetActive(false);
            });
            HLib.CanUnloadMod += CanUnloadMod;
            HLib.CanLoadMod += CanLoadMod;
            missingModsMenu.SetActive(false);
            network = ComponentManager<Raft_Network>.Value;
        }

        public bool CanUnloadMod(string name, string version)
        {
            if (!Raft_Network.IsHost && network.IsConnectedToHost && currentServerMods.mods.Find(x => x.modName == name && x.modVersion == version) != null)
            {
                return false;
            }
            if (!network.IsConnectedToHost)
            {
                currentServerMods = new ServerModsInfo();
            }
            return true;
        }

        public bool CanLoadMod(ModData data)
        {
            if (data.jsonmodinfo.requiredByAllPlayers && network.remoteUsers.Count > 1)
            {
                return false;
            }
            return true;
        }

        public static void OnJoinResult(CSteamID remoteID, InitiateResult result)
        {
            if (result == InitiateResult.Success)
            {
                ServerModsInfo mods = new ServerModsInfo();
                ModManagerPage.modList.Where(x => x.modinfo.modState == ModInfo.ModStateEnum.running).ToList().ForEach(x =>
                {
                    mods.mods.Add(new Mods() { modName = x.jsonmodinfo.name, modVersion = x.jsonmodinfo.version, modState = x.modinfo.modState });
                });
                RMessage_Modlist message = new RMessage_Modlist((Messages)1000, mods);
                RAPI.SendNetworkMessage(message, 2, EP2PSend.k_EP2PSendReliable, Target.Other, remoteID);
            }
        }
        void Update()
        {
            NetworkMessage netMessage = RAPI.ListenForNetworkMessagesOnChannel(2);
            if (netMessage != null)
            {
                CSteamID id = netMessage.steamid;
                Message message = netMessage.message;
                if (message.Type == (Messages)1000)
                {
                    RMessage_Modlist m = message as RMessage_Modlist;
                    if (remoteUsersMods.ContainsKey(id))
                    {
                        remoteUsersMods[id] = m.mods;
                    }
                    else
                    {
                        remoteUsersMods.Add(id, m.mods);
                    }
                }
                else if (message.Type == (Messages)1001)
                {
                    RMessage_Modlist m = message as RMessage_Modlist;
                    missingMods = m.mods;
                    DisplayMissingMods();
                }
                else if (message.Type == (Messages)1002)
                {
                    RMessage_Modlist m = message as RMessage_Modlist;
                    currentServerMods = m.mods;
                }
                else if (message.Type == (Messages)1003)
                {
                    RMessage_Modlist m = message as RMessage_Modlist;
                    missingMods = new ServerModsInfo() { mods = m._mods.Split(';').Select(x => new Mods() { modName = x.Split('@')[0], modVersion = x.Split('@')[1] }).ToList() };
                    DisplayMissingMods();
                }
            }
        }

        void DisplayMissingMods()
        {
            network.LeaveGame(DisconnectReason.HostDisconnected, SceneName.Lobby, false);
            foreach (Transform t in modList.transform)
            {
                Destroy(t.gameObject);
            }
            Debug.LogWarning("The server you tried to join requires the following mods :\n" + string.Join("\n", missingMods.mods.Select(x => " - " + x.modName + "@" + x.modVersion)));
            missingMods.mods.ForEach(x =>
            {
                GameObject o = Instantiate(missingModEntry, modList.transform);
                o.transform.Find("ModName").GetComponent<Text>().text = x.modName;
                o.transform.Find("ModVersion").GetComponent<Text>().text = x.modVersion;
                o.transform.Find("Buttons").Find("ModInfoBtn").gameObject.AddComponent<TooltipHandler>().tooltip = o.transform.Find("Buttons").Find("ModInfoBtn").Find("Tooltip").gameObject;
                o.transform.Find("Buttons").Find("ModInfoBtn").Find("Tooltip").gameObject.GetComponent<Canvas>().sortingOrder = 32001;
                o.transform.Find("Buttons").Find("ModInfoBtn").GetComponent<Button>().onClick.AddListener(() =>
                {
                    Application.OpenURL("https://www.raftmodding.com/mods?q=" + x.modName);
                });
            });
            missingModsMenu.SetActive(true);
            if (!missingModsMenu.GetComponent<Canvas>())
                missingModsMenu.AddComponent<Canvas>();
            missingModsMenu.GetComponent<Canvas>().overrideSorting = true;
            missingModsMenu.GetComponent<Canvas>().sortingOrder = 32000;
            missingMods = new ServerModsInfo();
        }


        public async static void KickUser(CSteamID id)
        {
            await Task.Delay(3000);
            SteamNetworking.CloseP2PSessionWithUser(id);
            SteamNetworking.CloseP2PChannelWithUser(id, 0);
            SteamNetworking.CloseP2PChannelWithUser(id, 1);
        }
    }

    [Serializable]
    public class ServerModsInfo
    {
        [SerializeField]
        public List<Mods> mods = new List<Mods>();
    }

    [Serializable]
    public class Mods
    {
        public string modName;
        public string modVersion;
        public ModInfo.ModStateEnum modState;
    }

    [Serializable]
    public class RMessage_Modlist : Message
    {
        public ServerModsInfo mods;
        public string _mods;

        public RMessage_Modlist(Messages type, ServerModsInfo mods) : base(type)
        {
            this.mods = mods;
        }

        public RMessage_Modlist(Messages type, string mods) : base(type)
        {
            this._mods = mods;
        }
    }

    [HarmonyPatch(typeof(Raft_Network))]
    [HarmonyPatch("AddPlayer")]
    [HarmonyPatch(new System.Type[] { typeof(CSteamID), typeof(RGD_Settings_Character) })]
    public static class NetworkModManager_AddPlayer_Patch
    {
        private static bool Prefix(Raft_Network __instance, CSteamID steamID)
        {
            if (steamID != __instance.HostID)
            {
                ServerModsInfo requiredMods = new ServerModsInfo();
                ModManagerPage.modList.Where(x => x.modinfo.modState == ModInfo.ModStateEnum.running && x.jsonmodinfo.requiredByAllPlayers == true).ToList().ForEach(x =>
                {
                    requiredMods.mods.Add(new Mods() { modName = x.jsonmodinfo.name, modVersion = x.jsonmodinfo.version });
                });
                if (requiredMods.mods.Count == 0)
                {
                    return true;
                }
                if (NetworkModManager.remoteUsersMods.ContainsKey(steamID))
                {
                    ServerModsInfo userMods = NetworkModManager.remoteUsersMods[steamID];
                    ServerModsInfo missingMods = new ServerModsInfo();
                    foreach (Mods mod in requiredMods.mods)
                    {
                        if (userMods.mods.Find(x => x.modName == mod.modName && x.modVersion == mod.modVersion) == null)
                        {
                            missingMods.mods.Add(mod);
                        }
                    }
                    if (missingMods.mods.Count > 0)
                    {
                        __instance.SendP2P(steamID, new RMessage_Modlist((Messages)1001, missingMods), EP2PSend.k_EP2PSendReliable, (NetworkChannel)2);
                        NetworkModManager.remoteUsersMods.Remove(steamID);
                        //NetworkModManager.KickUser(steamID);
                        return true;
                    }
                }
                else
                {
                    __instance.SendP2P(steamID, new RMessage_Modlist((Messages)1001, new ServerModsInfo()), EP2PSend.k_EP2PSendReliable, (NetworkChannel)2);
                    //NetworkModManager.KickUser(steamID);
                    return true;
                }
                __instance.SendP2P(steamID, new RMessage_Modlist((Messages)1002, requiredMods), EP2PSend.k_EP2PSendReliable, (NetworkChannel)2);
                return true;
            }
            return true;
        }
    }
}