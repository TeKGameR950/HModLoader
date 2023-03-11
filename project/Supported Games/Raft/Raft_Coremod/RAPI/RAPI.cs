using HarmonyLib;
using RaftModLoader;
using Sirenix.Serialization;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RAPI
{
    public static bool IsDedicatedServer()
    {
        return false;
    }

    public static bool IsCurrentSceneMainMenu()
    {
        return SceneManager.GetActiveScene().name == "MainMenuScene";
    }

    public static bool IsCurrentSceneGame()
    {
        return SceneManager.GetActiveScene().name == "MainScene";
    }

    public static string GetUsernameFromSteamID(CSteamID steamid)
    {
        string username = SteamFriends.GetFriendPersonaName(steamid);
        if (username.ToLower() != "[unknown]")
        {
            return username;
        }
        Network_Player player = ComponentManager<Raft_Network>.Value.GetPlayerFromID(steamid);
        if (player != null)
        {
            return player.playerNameTextMesh.text;
        }
        return username;
    }

    public static void TogglePriorityCursor(bool var)
    {
        if (RAPI.IsCurrentSceneMainMenu())
            return;
        try
        {
            if (var)
            {
                if (CanvasHelper.ActiveMenu == MenuType.None)
                {
                    Helper.SetCursorVisibleAndLockState(true, CursorLockMode.None);
                    CanvasHelper.ActiveMenu = (MenuType)666;
                    var ch = ComponentManager<CanvasHelper>.Value;
                    ch.SetUIState(true);
                }
                if (CanvasHelper.ActiveMenu == (MenuType)666 && !Cursor.visible)
                {
                    Helper.SetCursorVisibleAndLockState(true, CursorLockMode.None);
                    CanvasHelper.ActiveMenu = (MenuType)666;
                    var ch = ComponentManager<CanvasHelper>.Value;
                    ch.SetUIState(true);
                }
            }
            else
            {
                if (RConsole.isOpen || MainMenu.IsOpen) { return; }
                if (CanvasHelper.ActiveMenu == (MenuType)666)
                {
                    Helper.SetCursorVisibleAndLockState(false, CursorLockMode.Locked);
                    CanvasHelper.ActiveMenu = MenuType.None;
                    var ch = ComponentManager<CanvasHelper>.Value;
                    ch.SetUIState(true);
                }
            }
        }
        catch { }
    }

    public static void ToggleCursor(bool var)
    {
        if (RAPI.IsCurrentSceneMainMenu())
            return;
        try
        {
            if (var)
            {
                Helper.SetCursorVisibleAndLockState(true, CursorLockMode.None);
                CanvasHelper.ActiveMenu = MenuType.PauseMenu;
                var ch = ComponentManager<CanvasHelper>.Value;
                ch.SetUIState(true);
            }
            else
            {
                Helper.SetCursorVisibleAndLockState(false, CursorLockMode.Locked);
                CanvasHelper.ActiveMenu = MenuType.None;
                var ch = ComponentManager<CanvasHelper>.Value;
                ch.SetUIState(true);
            }
        }
        catch { }
    }

    public static Network_Player GetLocalPlayer()
    {
        return ComponentManager<Raft_Network>.Value.GetLocalPlayer();
    }

    public static void BroadcastChatMessage(string message)
    {
        ChatManager chatManager = ComponentManager<ChatManager>.Value;
        Raft_Network network = ComponentManager<Raft_Network>.Value;
        CSteamID csteamid = new CSteamID();
        Message_IngameChat nmessage = new Message_IngameChat(Messages.Ingame_Chat_Message, chatManager, csteamid, message);
        network.RPC(nmessage, Target.All, EP2PSend.k_EP2PSendReliable, NetworkChannel.Channel_Game);
    }

    public static void GiveItem(Item_Base item, int amount)
    {
        ComponentManager<Raft_Network>.Value.GetLocalPlayer().Inventory.AddItem(item.UniqueName, amount);
    }

    public static void AddItemToBlockQuadType(Item_Base item, RBlockQuadType quadtype)
    {
        string quadtypestring = "blockquadtype/" + quadtype.ToString();
        List<Item_Base> customquadtype = Traverse.Create(Resources.Load<ScriptableObject>(quadtypestring)).Field("acceptableBlockTypes").GetValue<List<Item_Base>>();
        customquadtype.Add(item);
        Traverse.Create(Resources.Load<ScriptableObject>(quadtypestring)).Field("acceptableBlockTypes").SetValue(customquadtype);
    }

    public static void RemoveItemFromBlockQuadType(string itemUniqueName, RBlockQuadType quadtype)
    {
        if (string.IsNullOrWhiteSpace(itemUniqueName))
        {
            throw new ArgumentNullException("itemUniqueName");
        }

        string quadtypestring = "blockquadtype/" + quadtype.ToString();
        List<Item_Base> customquadtype = Traverse.Create(Resources.Load<ScriptableObject>(quadtypestring)).Field("acceptableBlockTypes").GetValue<List<Item_Base>>();

        customquadtype.RemoveAll(_o => _o.UniqueName == itemUniqueName);

        Traverse.Create(Resources.Load<ScriptableObject>(quadtypestring)).Field("acceptableBlockTypes").SetValue(customquadtype);
    }

    public static void RegisterItem(Item_Base item)
    {
        if (item != null)
        {
            if (item.UniqueIndex >= short.MaxValue)
            {
                Debug.LogError("[RAPI.RegisterNewItem()] Failed! > The item \"" + item.UniqueName + "\" has an invalid UniqueIndex (Needs to be less than " + short.MaxValue + ")!");
                return;
            }
            else
            {
                if (item.MaxUses < 1)
                {
                    Debug.LogError("[RAPI.RegisterNewItem()] Failed! The MaxUses value for item \"" + item.UniqueName + "\" is lower than 1!");
                    return;
                }

                if (!ItemManager.GetItemByIndex(item.UniqueIndex))
                {
                    try
                    {
                        Block block = item.settings_buildable.GetBlockPrefab(0);
                        if (block is Storage_Small)
                        {
                            Traverse t = Traverse.Create(block).Field("animatorMessageForwarder");
                            if (t.GetValue() == null)
                            {
                                AnimatorMessageForwarder anim = block.gameObject.AddComponent<AnimatorMessageForwarder>();
                                t.SetValue(anim);
                            }
                        }
                    }
                    catch { }
                    List<Item_Base> list = Traverse.Create(typeof(ItemManager)).Field("allAvailableItems").GetValue<List<Item_Base>>();
                    list.Add(item);
                    Traverse.Create(typeof(ItemManager)).Field("allAvailableItems").SetValue(list);
                }
                else
                {
                    Debug.LogError("[RAPI.RegisterNewItem()] Failed! > The item \"" + item.UniqueName + "\" can't be registered because the item \"" + ItemManager.GetItemByIndex(item.UniqueIndex).UniqueName + "\" already use that UniqueIndex!");
                    return;
                }
            }
        }
        else
        {
            Debug.LogError("[RAPI.RegisterNewItem()] Failed! > The method has been invoked with a null argument!");
            return;
        }
    }

    public static void SetItemObject(Item_Base item, GameObject prefab, RItemHand parent = RItemHand.rightHand)
    {
        if (item != null)
        {
            if (prefab != null)
            {
                if (!InternalItemAPI.itemObjects.ContainsKey(item))
                {
                    InternalItemAPI.itemObjects.Add(item, new InternalItemAPI.ItemObject(prefab, parent));
                }
                else
                {
                    Debug.LogError("[RAPI.SetItemObject()] Failed! > The item \"" + item.UniqueName + "\" already has an object!");
                    return;
                }
            }
            else
            {
                Debug.LogError("[RAPI.SetItemObject()] Failed! > The method has been invoked with a null argument!");
                return;
            }
        }
        else
        {
            Debug.LogError("[RAPI.SetItemObject()] Failed! > The method has been invoked with a null argument!");
            return;
        }
    }

    public static void SendNetworkMessage(Message message, int channel = 0, EP2PSend ep2psend = EP2PSend.k_EP2PSendReliable, Target target = Target.Other, CSteamID fallbackSteamID = new CSteamID())
    {
        if (message == null)
        {
            throw new NullReferenceException("Message was null in RAPI.SendNetworkMessage()");
        }
        if (GetLocalPlayer() == null)
        {
            Raft_Network network = ComponentManager<Raft_Network>.Value;
            network.SendP2P(fallbackSteamID, message, ep2psend, (NetworkChannel)channel);
            return;
        }
        if (Raft_Network.IsHost)
        {
            GetLocalPlayer().Network.RPC(message, target, ep2psend, (NetworkChannel)channel);
        }
        else
        {
            GetLocalPlayer().SendP2P(message, ep2psend, (NetworkChannel)channel);
        }
    }

    public static NetworkMessage ListenForNetworkMessagesOnChannel(int channel = 0)
    {
        if (channel <= 1)
        {
            Debug.LogError("RAPI.ListenForNetworkMessagesOnChannel() can't be used to listen for messages on the channel 0 and 1! Please choose a unique number for your mod.");
            return null;
        }

        uint pcubMsgSize;
        while (SteamNetworking.IsP2PPacketAvailable(out pcubMsgSize, channel))
        {
            byte[] array = new byte[pcubMsgSize];
            uint num2;
            CSteamID csteamID;
            if (SteamNetworking.ReadP2PPacket(array, pcubMsgSize, out num2, out csteamID, channel))
            {
                Packet packet = SerializationUtility.DeserializeValue<Packet>(array, DataFormat.Binary, new DeserializationContext());
                Packet_Multiple packet_Multiple;
                if (packet.PacketType == PacketType.Single)
                {
                    Packet_Single packet_Single = packet as Packet_Single;
                    packet_Multiple = new Packet_Multiple(packet_Single.SendType);
                    packet_Multiple.messages = new Message[]
                    {
                        packet_Single.message
                    };
                }
                else
                {
                    packet_Multiple = (packet as Packet_Multiple);
                }
                return new NetworkMessage(packet_Multiple.messages[0], csteamID, packet_Multiple.messages.ToList());
            }
        }
        return null;
    }
}

public class NetworkMessage
{
    public Message message;
    public CSteamID steamid;

    public List<Message> messages = new List<Message>();

    public NetworkMessage(Message m, CSteamID s, List<Message> messages)
    {
        message = m;
        steamid = s;
        this.messages = messages;
    }
}

sealed class TwoWayPreMergeToMergedDeserializationBinder : TwoWaySerializationBinder
{
    public Assembly assembly;

    public TwoWayPreMergeToMergedDeserializationBinder(Assembly assembly)
    {
        this.assembly = assembly;
    }

    public override string BindToName(Type type, DebugContext debugContext = null)
    {
        return type.Name;
    }

    public override Type BindToType(string typeName, DebugContext debugContext = null)
    {
        // OOF BUT FCK
        if (typeName.Contains("RDSModWrapperNamespace"))
        {
            string name = "RaftModLoader." + typeName.Split(',')[0].Split('.').ToList().Last();
            Type type = assembly.GetType(name);
            return type;
        }
        Type typeToDeserialize = null;
        String exeAssembly = Assembly.GetCallingAssembly().FullName;
        typeToDeserialize = Type.GetType(String.Format("{0}, {1}", typeName, exeAssembly));
        if (typeToDeserialize == null && typeName.Contains(","))
        {
            typeName = typeName.Split(',')[0];
            typeToDeserialize = Type.GetType(String.Format("{0}, {1}", typeName, assembly.FullName));
        }

        return typeToDeserialize;
    }

    public override bool ContainsType(string typeName)
    {
        Type typeToDeserialize = null;
        String exeAssembly = Assembly.GetExecutingAssembly().FullName;
        typeToDeserialize = Type.GetType(String.Format("{0}, {1}", typeName, exeAssembly));
        return typeToDeserialize != null;
    }
}

public static class RPlayerExtentions
{
    // Update 12 fixing some ass mods
    [Obsolete("This is obsolete, please use .FindChildRecursively(childName) instead.")]
    public static Transform FindChildRecursivly(this Transform parent, string childName)
    {
        return parent.FindChildRecursively(childName);
    }

    public static void SendChatMessage(this Network_Player player, string message)
    {
        ChatManager chatManager = ComponentManager<ChatManager>.Value;
        Raft_Network network = ComponentManager<Raft_Network>.Value;
        CSteamID csteamid = new CSteamID();
        Message_IngameChat nmessage = new Message_IngameChat(Messages.Ingame_Chat_Message, chatManager, csteamid, message);
        network.SendP2P(player.steamID, nmessage, EP2PSend.k_EP2PSendReliable, NetworkChannel.Channel_Game);
    }
}

public enum RBlockQuadType
{
    quad_corner_all_empty,
    quad_corner_inv_empty,
    quad_corner_normal_empty,
    quad_floor,
    quad_floor_empty,
    quad_foundation,
    quad_foundation_empty,
    quad_itemnet_empty,
    quad_pillar_empty,
    quad_pipe_empty,
    quad_roof_straight_45_inv,
    quad_table,
    quad_tikipole,
    quad_wall,
    quad_walltop_empty,
};

public enum RItemHand
{
    leftHand,
    rightHand
};