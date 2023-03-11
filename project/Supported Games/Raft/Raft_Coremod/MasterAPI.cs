using HMLLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace RaftModLoader
{
    public static class MasterAPI
    {
        public static async void SendHeartbeat()
        {
            try
            {
                if (Raft_Network.IsHost && Raft_Network.CurrentRequestJoinAuthSetting == RequestJoinAuthSetting.ALLOW_ALL && Raft_Network.WorldHasBeenRecieved && ComponentManager<Raft_Network>.Value.remoteUsers.Count > 0)
                {
                    SendHeartbeatRequest(new HeartbeatData()
                    {
                        privatekey = HUtils.GetMLAssemblyUniqueKey(typeof(RSocket)),
                        ip = SteamUser.GetSteamID().GetAccountID().ToString(),
                        players = Mathf.Clamp(ComponentManager<Raft_Network>.Value.remoteUsers.Count, 1, 20),
                        maxplayers = 0,
                        name = "",
                        gamemode = GameManager.GameMode.ToString(),
                        friendlyfire = GameManager.FriendlyFire,
                        plugins = string.Join(";", ModManagerPage.modList.Where(x => x.modinfo.modState == ModInfo.ModStateEnum.running).Select(x => x.jsonmodinfo.name).ToArray()),
                        haspassword = GameManager.HasPassword,
                        iconurl = "",
                        bannerurl = "",
                        listed = true
                    });
                }
            }
            catch { }
        }

        private static async Task<bool> SendHeartbeatRequest(HeartbeatData hd)
        {
            try
            {
                WWWForm form = new WWWForm();
                form.AddField("data", JsonConvert.SerializeObject(hd));

                DateTime now = DateTime.Now;
                using (UnityWebRequest www = UnityWebRequest.Post("https://master.raftmodding.com/api/heartbeat", form))
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
                        string result = www.downloadHandler.text;
                        JToken r = JsonConvert.DeserializeObject<JToken>(result);
                        if (r["result"].ToString() == "OK")
                            return true;

                    }
                }
            }
            catch { }
            return false;
        }

        public static async Task<bool> VerifyServer(string ip)
        {
            try
            {
                WWWForm form = new WWWForm();
                form.AddField("ip", ip);

                DateTime now = DateTime.Now;
                using (UnityWebRequest www = UnityWebRequest.Post("https://master.raftmodding.com/api/verifyserver", form))
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
                        string result = www.downloadHandler.text;
                        JToken r = JsonConvert.DeserializeObject<JToken>(result);
                        if (r["result"].ToString() == "OK")
                            return true;

                    }
                }
            }
            catch { }
            return false;
        }
    }

    public class HeartbeatData
    {
        public string privatekey;
        public string ip;
        public int players;
        public int maxplayers;
        public string name;
        public string gamemode;
        public bool friendlyfire;
        public string plugins;
        public bool haspassword;
        public string iconurl;
        public string bannerurl;
        public bool listed;
    }
}
