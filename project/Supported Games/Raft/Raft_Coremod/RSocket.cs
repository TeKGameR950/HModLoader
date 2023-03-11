using HarmonyLib;
using HMLLibrary;
using RaftModLoader;
using SocketIO;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RaftModLoader
{
    public class RSocket : MonoBehaviour
    {
        public static string serverIp = "ws://164.132.145.11:8081/socket.io/?EIO=4&transport=websocket";
        public static SocketIOComponent socket;
        public Text onlineUsers;
        public static string customServerName = "";

        public async void Start()
        {
            socket = null;
            while (socket == null)
            {
                socket = GetComponent<SocketIOComponent>();
                await Task.Delay(1);
            }
            socket.On("connect", onConnect);
            socket.On("receiveSteamid", receiveSteamid);
            socket.On("playersAmountUpdate", updatePlayersAmount);
            socket.On("receivePatrons", receivePatrons);
            socket.On("disconnect", onDisconnect);
            socket.On("ping", (t) => { socket.Emit("pong"); });
            socket.On("client:GetHomepageInfo", OnHomepageInfoFromServer);
            StartCoroutine(RunningCoroutine());
        }

        public void OnHomepageInfoFromServer(SocketIOEvent e)
        {
            HomePage.instance.OnHomepageInfoFromServer(e);
        }

        public void receivePatrons(SocketIOEvent e)
        {
            CustomLoadingScreen.InitLoadingScreen(e.data["patrons"]);
        }

        public IEnumerator RunningCoroutine()
        {
            MasterAPI.SendHeartbeat();
            yield return new WaitForSeconds(10);
            StartCoroutine(RunningCoroutine());
        }

        public void onDisconnect(SocketIOEvent e)
        {
            Debug.Log("<color=#d64040>You have been disconnected from the RaftModding RuntimeApp.</color>");
        }

        public async void onConnect(SocketIOEvent e)
        {
            Debug.Log("<color=#41f46b>Successfully connected to the RaftModding RuntimeApp!</color>");
            JSONObject data = new JSONObject();
            data.AddField("username", SteamFriends.GetPersonaName());
            data.AddField("accountid", SteamUser.GetSteamID().GetAccountID().ToString());
            data.AddField("steamid", SteamUser.GetSteamID().m_SteamID.ToString());
            if (socket == null) { await Task.Delay(100); }
            socket.Emit("updateData", data);
            socket.Emit("server:GetHomepageInfo");
            socket.Emit("server:GetLoadingscreenInfo");
            while (ServersPage.instance == null)
            {
                await Task.Delay(100);
            }
            await Task.Delay(100);
            ServersPage.instance.RefreshServers();
        }

        public void onError(SocketIOEvent e)
        {
            Debug.Log("<color=#42f4a1>[RSocket]</color> <color=#d64040>" + e.data["error"].str + "</color>");
        }

        public async static void convertSteamid(string steamid, string password)
        {
            if (socket == null) { await Task.Delay(100); }
            ServersPage.lastPassword = password;
            JSONObject data = new JSONObject();
            data.AddField("steamid", steamid);
            socket.Emit("convertSteamid", data);
        }

        public void receiveSteamid(SocketIOEvent e)
        {
            try
            {
                uint steamid = 0;
                uint.TryParse(e.data["accountid"].str, out steamid);
                CSteamID csteamid = new CSteamID(new AccountID_t((uint)steamid), EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeIndividual);
                if (!csteamid.IsValid() || steamid < 1000000 || steamid > uint.MaxValue)
                {
                    Debug.Log("The provided SteamID is invalid, Valid SteamID's are SteamID,SteamID64,SteamID3 and AccountID");
                    return;
                }
                ServersPage.ConnectToServer(csteamid, ServersPage.lastPassword, false);
                ServersPage.lastPassword = "";
                return;
            }
            catch
            {
                Debug.Log("The provided SteamID is invalid, Valid SteamID's are SteamID,SteamID64,SteamID3 and AccountID");
                return;
            }
        }

        public void updatePlayersAmount(SocketIOEvent e)
        {
            onlineUsers.text = e.data["amount"] + " ONLINE USERS";
        }
    }
}