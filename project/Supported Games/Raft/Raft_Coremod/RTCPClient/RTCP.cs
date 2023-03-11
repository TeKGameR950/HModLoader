using HMLLibrary;
using RTCP.Networking;
using RTCP.Networking.Events;
using RTCP.Steam.Networking;
using RTCP.Steam.Networking.Signals;
using RTCP.Utils;
using RTCPNetImprovements;
using Steamworks;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using SteamPacket = RTCP.Steam.Networking.Packet;

namespace RaftModLoader
{
    public class RTCP
    {
        public static string ipUrl = "https://api.ipify.org";
        public static string myIp = "";
        public static bool RTCPNetworkingLayer = false;
        public static Client client;
        public static Dictionary<int, ConcurrentQueue<SteamPacket>> packetQueue = new Dictionary<int, ConcurrentQueue<SteamPacket>>();
        public static ConcurrentQueue<string> stringQueue = new ConcurrentQueue<string>();

        public static async void InitServerIP()
        {
            WebRequestResult result = await DoGetRequest(ipUrl);
            if (result.completed)
            {
                IPAddress ip;
                if (IPAddress.TryParse(result.result, out ip))
                {
                    string sip = ip.MapToIPv4().ToString();
                    myIp = sip;
                    return;
                }
            }
            await Task.Delay(5000);
            InitServerIP();
        }

        private static async void handlePacket(object sender, ReceivedPacketEventArgs args)
        {
            try
            {
                if (args == null || args.packet == null) return;
                if (!(args.packet is SteamPacket))
                    Debug.LogWarning("[RTCP Warning] Received a packet that wasn't a SteamPacket !");
                //stringQueue.Enqueue((new System.Random().Next(1, 100000)) + ": handlePlacket was called!");
                SteamPacket p = (SteamPacket)args.packet;
                if (p is AcceptClientSignal)
                {
                    AcceptClientSignal signal = p as AcceptClientSignal;
                    await Task.Delay(100);
                    CSteamID csteamid = new CSteamID(new AccountID_t(1), EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeIndividual);
                    Raft_Network network = ComponentManager<Raft_Network>.Value;
                    network.CancelTryingToConnect();
                    network.TryToJoinGame(csteamid, ServersPage.lastPassword);
                    return;
                }
                else if (p is PluginInfoSignal)
                {
                    /*PluginInfoSignal signal = p as PluginInfoSignal;
                    Debug.Log("Received Plugin Signal (" + signal.name + ") !");
                    Debug.Log("Plugin Signals are temporarily disabled !");
                    return;
                    switch (signal.command)
                    {
                        case "load":
                            ClientPluginManager.LoadClientPlugin(signal.name, signal.files);
                            break;
                        case "unload":
                            ClientPluginManager.UnloadClientPlugin(signal.name);
                            break;
                    }
                    return;*/
                }
                if (!packetQueue.ContainsKey(p.nChannel))
                    packetQueue.Add(p.nChannel, new ConcurrentQueue<SteamPacket>());
                packetQueue[p.nChannel].Enqueue(p);
            }
            catch (Exception e)
            {
                Debug.LogError("An error occured when receiving an RTCP packet ! Error : \n" + e.ToString());
            }
        }

        public static void StopRTCP()
        {
            client.Disconnect();
        }

        public static bool IsLocalIpAddress(string host)
        {
            try
            {
                // get host IP addresses
                IPAddress[] hostIPs = Dns.GetHostAddresses(host);
                // get local IP addresses
                IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

                // test if any host IP equals to any local IP or to localhost
                foreach (IPAddress hostIP in hostIPs)
                {
                    // is localhost
                    if (IPAddress.IsLoopback(hostIP)) return true;
                    // is local address
                    foreach (IPAddress localIP in localIPs)
                    {
                        if (hostIP.Equals(localIP)) return true;
                    }
                }
            }
            catch { }
            return false;
        }

        public static async Task<bool> JoinServer(IPAddress address, int port)
        {
            try
            {
                string ip = address.ToString();
                if (!address.IsLocalNetwork())
                {
                    bool validServer = await MasterAPI.VerifyServer(address.ToString() + ":" + port);
                    if (!validServer)
                    {
                        Debug.LogWarning("This server is not registered on the RDS Master ! Please try again in a few seconds or contact the server owner !");
                        return false;
                    }
                }
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                await socket.ConnectAsync(address, port);
                if (!socket.Connected)
                {
                    Debug.LogWarning("Could not connect to " + address.ToString() + ":" + port + " ! Reason : Timed Out !");
                    return false;
                }
                AsyncSocketWrapper clientSocket = new AsyncSocketWrapper(socket);
                clientSocket.OnPacketReceived += handlePacket;
                clientSocket.OnDisconnect += (object sender, DisconnectEventArgs arguments) =>
                {
                    arguments.GetSocket().Close();
                    /*UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        if (!Raft_Network.InMenuScene)
                            RNetwork.network.LeaveGame(DisconnectReason.SelfDisconnected, SceneName.Exit, false, true);
                    });*/

                };
                clientSocket.StartReceive();
                client = new Client();
                client.AttachSocket(clientSocket);
                string ticket = await GetSteamAuthTicket();
                if (ticket.StartsWith("ERR:"))
                {
                    string error = ticket.Split(':')[1];
                    Debug.LogError("[RTCP Authentication] An error occured when retrieving the steam authentication ticket ! Error : " + error);
                    return false;
                }
                else
                {
                    AcceptClientSignal signal = new AcceptClientSignal(ComponentManager<Raft_Network>.Value.LocalSteamID, ticket, EP2PSend.k_EP2PSendReliable, new byte[] { }, -999);
                    signal.clientmods = string.Join(";", ModManagerPage.modList.Where(x => x.modinfo.modState == ModInfo.ModStateEnum.running).Select(x => x.jsonmodinfo.name + "@" + x.jsonmodinfo.version).ToList());
                    client.Send(signal);
                    //Debug.Log("Sent authentication data to the server...");
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Could not connect to " + address.ToString() + ":" + port + " ! Reason : " + ex.Message);
                return false;
            }
        }


        public static Callback<GetAuthSessionTicketResponse_t> m_GetAuthSessionTicketResponse = null;

        public static HAuthTicket? lastTicket;
        public static EResult? lastError;
        public static void OnGetAuthSessionTicketResponse(GetAuthSessionTicketResponse_t pCallback)
        {
            if (pCallback.m_eResult == EResult.k_EResultOK || pCallback.m_eResult == EResult.k_EResultAdministratorOK)
            {
                lastTicket = pCallback.m_hAuthTicket;
            }
            else
            {
                lastError = pCallback.m_eResult;
            }
        }

        public static async Task<string> GetSteamAuthTicket()
        {
            if (m_GetAuthSessionTicketResponse == null)
                m_GetAuthSessionTicketResponse = Callback<GetAuthSessionTicketResponse_t>.Create(OnGetAuthSessionTicketResponse);
            if (lastTicket != null)
            {
                SteamUser.CancelAuthTicket((HAuthTicket)lastTicket);
                lastTicket = null;
            }
            byte[] ticketByteArray = new byte[1024];
            uint ticketSize;
            SteamUser.GetAuthSessionTicket(ticketByteArray, ticketByteArray.Length, out ticketSize);
            DateTime now = DateTime.Now;
            bool cancelTimeout = false;
            string ticket = "";
            while (lastTicket == null && !cancelTimeout && lastError == null)
            {
                if (now.AddSeconds(5) <= DateTime.Now)
                {
                    cancelTimeout = true;
                }
                await Task.Delay(1);
            }
            if (lastError != null)
            {
                ticket = "ERR:" + lastError.ToString();
            }
            else if (cancelTimeout)
            {
                ticket = "ERR:TimedOut";
            }
            else
            {
                Array.Resize(ref ticketByteArray, (int)ticketSize);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < ticketSize; i++)
                {
                    sb.AppendFormat("{0:x2}", ticketByteArray[i]);
                }
                ticket = sb.ToString();
            }
            return ticket;
        }

        public static async Task<WebRequestResult> DoGetRequest(string url, int timeout = 5)
        {
            try
            {
                using (UnityWebRequest www = UnityWebRequest.Get(url))
                {
                    www.certificateHandler = new HttpCertHandler();
                    www.SendWebRequest();
                    bool cancelTimeOut = false;
                    DateTime now = DateTime.Now;
                    while (!cancelTimeOut && !www.isDone && !www.isNetworkError && !www.isHttpError)
                    {
                        if (now.AddSeconds(5) <= DateTime.Now)
                            cancelTimeOut = true;
                        await Task.Delay(1);

                        if (!www.isNetworkError && !www.isHttpError && string.IsNullOrEmpty(www.error))
                        {
                            string response = www.downloadHandler.text;
                            return new WebRequestResult(true, response, response);
                        }
                        return new WebRequestResult(false, "Timed Out");
                    }
                }
            }
            catch (Exception ex)
            {
                return new WebRequestResult(false, GetInnerException(ex).Message);
            }
            return new WebRequestResult(false, "An error occured");
        }

        public static Exception GetInnerException(Exception ex)
        {
            Exception result = ex;
            while (result.InnerException != null)
                result = result.InnerException;
            return result;
        }
    }

    public class WebRequestResult
    {
        public bool completed;
        public string error;
        public string result;

        public WebRequestResult(bool c = false, string e = "", string r = "")
        {
            completed = c;
            error = e;
            result = r;
        }
    }


    public static class RTCPExtensions
    {
        private static Tuple<IPAddress, IPAddress>[] localNetworks = new Tuple<IPAddress, IPAddress>[] {
        new Tuple<IPAddress, IPAddress>(IPAddress.Parse("0.0.0.0"), IPAddress.Parse("255.255.255.255")),
        new Tuple<IPAddress, IPAddress>(IPAddress.Parse("10.0.0.0"), IPAddress.Parse("255.0.0.0")),
        new Tuple<IPAddress, IPAddress>(IPAddress.Parse("127.0.0.0"), IPAddress.Parse("255.0.0.0")),
        new Tuple<IPAddress, IPAddress>(IPAddress.Parse("172.16.0.0"), IPAddress.Parse("255.255.240.0")),
        new Tuple<IPAddress, IPAddress>(IPAddress.Parse("192.168.0.0"), IPAddress.Parse("255.255.0.0"))
    };
        public static IPAddress GetHostAddressOfNetwork(this IPAddress ipAddress, IPAddress mask)
        {
            byte[] addressBytes = ipAddress.GetAddressBytes();
            byte[] maskBytes = mask.GetAddressBytes();
            byte[] hostAddressBytes = new byte[addressBytes.Length];
            for (int i = 0; i < addressBytes.Length; i++)
            {
                hostAddressBytes[i] = (byte)(addressBytes[i] & maskBytes[i]);
            }
            return new IPAddress(hostAddressBytes);
        }
        public static bool IsLocalNetwork(this IPAddress ipAddress)
        {
            return localNetworks.Any(t => ipAddress.GetHostAddressOfNetwork(t.Item2).Equals(t.Item1));
        }
    }
}
