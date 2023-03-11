using FMOD.Studio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UltimateWater;
using RTCPNetImprovements;
using HarmonyLib;
using Steamworks;
using HMLLibrary;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace RaftModLoader
{
    public class RNetworkImprovements : MonoBehaviour
    {
        void Update()
        {
            if (RTCP.RTCPNetworkingLayer)
            {
                NetworkMessage netMessage = RAPI.ListenForNetworkMessagesOnChannel((int)RNetworkMessages.channel);
                if (netMessage != null)
                {
                    OnNetworkMessage(netMessage);
                }
            }
            // In Steam Network Layer(would work but vanilla users wouldn't receive it, with RTCP we are "sure" they are RML users.
        }

        public static void OnNetworkMessage(NetworkMessage netMessage)
        {
            Message message = netMessage.message;
            Messages type = message.Type;
            switch (type)
            {
                case RNetworkMessages.R_DestroyBlock:
                    ((RMessage_DestroyBlock)message).Execute();
                    break;
                case RNetworkMessages.R_CreateBlock:
                    ((RMessage_CreateBlock)message).Execute();
                    break;
                case RNetworkMessages.R_PaintBlock:
                    ((RMessage_PaintBlock)message).Execute();
                    break;
                case RNetworkMessages.R_UpdateBlock:
                    ((RMessage_UpdateBlock)message).Execute();
                    break;
                case RNetworkMessages.R_UpdateCharacterSettings:
                    ((RMessage_UpdateCharacterSettings)message).Execute();
                    break;
                case RNetworkMessages.R_DisconnectReason:
                    RMessage_DisconnectReason reason = message as RMessage_DisconnectReason;
                    ServersPage.OnJoinResult(new CSteamID(1), (InitiateResult)69, reason.reason);
                    break;
                default:
                    Debug.Log("Received an unknown RNetwork message (ID : " + (int)type + ") !");
                    break;
            }
        }
    }
}

