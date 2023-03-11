using HarmonyLib;
using RTCP.Steam.Networking.Signals;
using Steamworks;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SteamPacket = RTCP.Steam.Networking.Packet;

namespace RaftModLoader
{
    public class RTCPHarmonyPatches
    {
        // THE SWITCH TO RTCP/STEAM IS DONE HERE.
        [HarmonyPatch(typeof(Raft_Network))]
        [HarmonyPatch("TryToJoinGame")]
        static class HarmonyPatch_RTCPOverride
        {
            static void Prefix(Raft_Network __instance, CSteamID hostID, string password)
            {
                RTCP.RTCPNetworkingLayer = hostID.GetAccountID().m_AccountID == 1;
                Debug.Log("Joining game with " + (RTCP.RTCPNetworkingLayer ? "RTCP" : "Steam") + " networking layer !");
            }
        }
        [HarmonyPatch(typeof(Raft_Network))]
        [HarmonyPatch("HostGame")]
        static class HarmonyPatch_RTCPOverride1
        {
            static void Prefix(Raft_Network __instance)
            {
                RTCP.RTCPNetworkingLayer = false;
                Debug.Log("Hosting, switching to Steam networking layer !");
            }
        }

        [HarmonyPatch]
        class Patch_Steamnetworking
        {
            [HarmonyPatch(typeof(SteamNetworking), "SendP2PPacket")]
            [HarmonyPrefix]
            static bool SendP2PPacket(CSteamID steamIDRemote, byte[] pubData, uint cubData, EP2PSend eP2PSendType, int nChannel = 0)
            {
                if (!RTCP.RTCPNetworkingLayer)
                {
                    return true;
                }
                if ((bool)(RTCP.client?.GetSocket()?.IsConnected))
                {
                    SteamPacket packet = new SteamPacket(steamIDRemote, eP2PSendType, pubData, nChannel);
                    // Debug.Log(BitConverter.ToInt32(allData, 0));
                    // Debug.Log("SendP2PPacket Called: " + dataFromStream.Length + " == " + dataFromStreamLength + " || " + allData.Length + " || " + nChannel);
                    RTCP.client.Send(packet);

                    //Debug.Log("refore first return");
                    return false;
                }
                else
                {
                    Debug.Log("RDSNetworkOverride.client is null or not connected");
                    RTCP.stringQueue.Enqueue("RDSNetworkOverride.client is null");
                }

                return false;
            }

            [HarmonyPatch(typeof(SteamNetworking), "IsP2PPacketAvailable")]
            [HarmonyPrefix]
            static bool IsP2PPacketAvailable(ref bool __result, out uint pcubMsgSize, int nChannel = 0)
            {
                if (!RTCP.RTCPNetworkingLayer)
                {
                    pcubMsgSize = 0;
                    return true;
                }
                if (!RTCP.packetQueue.ContainsKey(nChannel))
                    RTCP.packetQueue.Add(nChannel, new ConcurrentQueue<SteamPacket>());

                // Debug.Log("nChannel: " + nChannel);
                // Debug.Log("IsP2PPacketAvailable:: " + RTCP.packetQueue[nChannel].Count);

                // Debug.Log("IsP2PPacketAvailable called for "+nChannel+"! => " + RTCP.packetQueue[nChannel].Count);
                __result = !RTCP.packetQueue[nChannel].IsEmpty;

                SteamPacket p;
                if (!RTCP.packetQueue[nChannel].IsEmpty && RTCP.packetQueue[nChannel].TryPeek(out p))
                {
                    // Debug.Log("IsP2PPacketAvailable: " + RTCP.packetQueue[nChannel].Count);
                    pcubMsgSize = Convert.ToUInt32(p.Size);
                }
                else
                {
                    // Debug.Log("IsP2PPacketAvailable: " + RTCP.packetQueue[nChannel].Count);
                    pcubMsgSize = 0;
                }
                return false;
            }

            [HarmonyPatch(typeof(SteamNetworking), "ReadP2PPacket")]
            [HarmonyPrefix]
            static bool ReadP2PPacket(ref bool __result, ref byte[] pubDest, uint cubDest, out uint pcubMsgSize, out CSteamID psteamIDRemote, int nChannel = 0)
            {
                if (!RTCP.RTCPNetworkingLayer)
                {
                    pcubMsgSize = 0;                        // Set to anything because it is required. Will be set correctly later in the original method
                    psteamIDRemote = default(CSteamID);     // Set to anything because it is required. Will be set correctly later in the original method
                    return true;
                }
                SteamPacket p;
                if (!RTCP.packetQueue[nChannel].IsEmpty && RTCP.packetQueue[nChannel].TryDequeue(out p))
                {
                    __result = true;
                    Array.Copy(p.Data, pubDest, p.Size);
                    pcubMsgSize = Convert.ToUInt32(p.Size);
                    psteamIDRemote = p.steamIDRemote;
                }
                else
                {
                    __result = false;
                    // cubDest = 0;
                    pcubMsgSize = 0;
                    psteamIDRemote = default(CSteamID);
                }
                return false;
            }

            [HarmonyPatch(typeof(SteamNetworking), "AcceptP2PSessionWithUser")]
            [HarmonyPrefix]
            static bool AcceptP2PSessionWithUser(ref bool __result, CSteamID steamIDRemote)
            {
                if (!RTCP.RTCPNetworkingLayer)
                {
                    return true;
                }
                Debug.Log("AcceptP2PSessionWithUser");
                __result = true;
                //RTCP.client.Send(new AcceptClientSignal(steamIDRemote, EP2PSend.k_EP2PSendReliable, new byte[] { }, -999));
                return false;
            }

            [HarmonyPatch(typeof(SteamNetworking), "CloseP2PSessionWithUser")]
            [HarmonyPrefix]
            static bool CloseP2PSessionWithUser(ref bool __result, CSteamID steamIDRemote)
            {
                if (!RTCP.RTCPNetworkingLayer)
                {
                    return true;
                }
                Debug.Log("CloseP2PSessionWithUser");
                __result = true;
                //RTCP.client.Send(new DisconnectClientSignal(steamIDRemote, EP2PSend.k_EP2PSendReliable, new byte[] { }, -999));
                return false;
            }

            [HarmonyPatch(typeof(SteamNetworking), "CloseP2PChannelWithUser")]
            [HarmonyPrefix]
            static bool CloseP2PChannelWithUser(ref bool __result, CSteamID steamIDRemote, int nChannel)
            {
                if (!RTCP.RTCPNetworkingLayer)
                {
                    return true;
                }
                // We don't care. A channel is not it's own socket connection. It's just a layer inside one socket that does not need any resources that could be freed
                return false;
            }

            [HarmonyPatch(typeof(SteamNetworking), "GetP2PSessionState")]
            [HarmonyPrefix]
            static bool GetP2PSessionState(ref bool __result, CSteamID steamIDRemote, out P2PSessionState_t pConnectionState)
            {
                if (!RTCP.RTCPNetworkingLayer)
                {
                    pConnectionState = default(P2PSessionState_t);  
                    return true;
                }
                __result = true;
                P2PSessionState_t result = new P2PSessionState_t();
                /*
                 * TODO: really fill out that data. But Raft does not check the result, so no need for now.
                 * result.m_bConnectionActive = ...
                 * result.m_bConnecting = ...
                 * ...
                 */
                pConnectionState = result;
                return false;
            }

            [HarmonyPatch(typeof(SteamNetworking), "AllowP2PPacketRelay")]
            [HarmonyPrefix]
            static bool AllowP2PPacketRelay(ref bool __result, bool bAllow)
            {
                if (!RTCP.RTCPNetworkingLayer)
                {
                    return true;
                }
                // We don't care. We send everything to RDS instance
                return false;
            }
        }

        [HarmonyPatch]
        class Patch_Raft_Network
        {
            [HarmonyPatch(typeof(Raft_Network), "IsUserWhitelisted", new Type[] { typeof(CSteamID) })]
            [HarmonyPrefix]
            static bool IsUserWhitelisted(ref bool __result, CSteamID steamID)
            {
                if (!RTCP.RTCPNetworkingLayer)
                {
                    return true;
                }
                __result = true;
                return false;
            }
        }

        [HarmonyPatch]
        class Patch_CSteamID
        {
            [HarmonyPatch(typeof(CSteamID), "IsValid")]
            [HarmonyPrefix]
            static bool IsValid(ref bool __result, CSteamID __instance)
            {
                if (!RTCP.RTCPNetworkingLayer)
                {
                    return true;
                }
                if ((__instance.m_SteamID) > 0 && (__instance.m_SteamID) < 10000)
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }
    }
}
