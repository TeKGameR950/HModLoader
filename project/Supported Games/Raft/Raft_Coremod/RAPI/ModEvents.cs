#if GAME_IS_RAFT
using AssemblyLoader;
using HarmonyLib;
using HMLLibrary;
using Steamworks;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RaftModLoader
{
    [HarmonyPatch(typeof(Raft_Network))]
    [HarmonyPatch("OnSceneLoaded")]
    class Patch_WorldEvent_WorldLoaded
    {
        static async void Prefix(Raft_Network __instance, Scene scene)
        {
            if (scene.name == Raft_Network.GameSceneName)
            {
                while (!LoadSceneManager.IsGameSceneLoaded)
                {
                    await Task.Delay(100);
                }
                foreach (Mod mod in ModManagerPage.activeModInstances)
                {
                    if (mod)
                        mod.WorldEvent_WorldLoaded();
                }
            }
            if(scene.name == Raft_Network.MenuSceneName && HLoader.SAFEMODE)
            {
                ClientPluginManager.OnMainMenu();
            }
        }
    }

    [HarmonyPatch(typeof(SaveAndLoad))]
    [HarmonyPatch("SaveWorld")]
    class Patch_WorldEvent_WorldSaved
    {
        static void Postfix(SaveAndLoad __instance)
        {
            foreach (Mod mod in ModManagerPage.activeModInstances)
            {
                if (mod)
                    mod.WorldEvent_WorldSaved();
            }
        }
    }

    [HarmonyPatch(typeof(PlayerStats))]
    [HarmonyPatch("Damage")]
    class Patch_LocalPlayerEvent_Hurt
    {
        static void Prefix(PlayerStats __instance, float damage, Vector3 hitPoint, Vector3 hitNormal, EntityType damageInflictorEntityType, SO_Buff buffAsset = null)
        {
            if (__instance != RAPI.GetLocalPlayer().Stats) { return; }

            foreach (Mod mod in ModManagerPage.activeModInstances)
            {
                if (mod)
                    mod.LocalPlayerEvent_Hurt(damage, hitPoint, hitNormal, damageInflictorEntityType);
            }
        }
    }

    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("Kill")]
    class Patch_LocalPlayerEvent_Death
    {
        static void Postfix(Player __instance, ref Network_Player ___playerNetwork)
        {
            if (___playerNetwork.IsLocalPlayer)
            {
                foreach (Mod mod in ModManagerPage.activeModInstances)
                {
                    if (mod)
                        mod.LocalPlayerEvent_Death(__instance.transform.position);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("OnRespawnComplete")]
    class Patch_LocalPlayerEvent_Respawn
    {
        static void Postfix(Player __instance, ref Network_Player ___playerNetwork)
        {
            if (___playerNetwork.IsLocalPlayer)
            {
                foreach (Mod mod in ModManagerPage.activeModInstances)
                {
                    if (mod)
                        mod.LocalPlayerEvent_Respawn();
                }
            }
        }
    }

    [HarmonyPatch(typeof(CraftingMenu))]
    [HarmonyPatch("CraftItem")]
    class Patch_LocalPlayerEvent_ItemCrafted
    {
        static void Postfix(CraftingMenu __instance, ref Network_Player ___localPlayer, ref SelectedRecipeBox ___selectedRecipeBox)
        {
            if (___localPlayer.IsLocalPlayer)
            {
                foreach (Mod mod in ModManagerPage.activeModInstances)
                {
                    if (mod)
                        mod.LocalPlayerEvent_ItemCrafted(___selectedRecipeBox.selectedRecipeItem);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Pickup))]
    [HarmonyPatch("AddItemToInventory")]
    class Patch_LocalPlayerEvent_PickupItem
    {
        static void Postfix(CraftingMenu __instance, ref Network_Player ___playerNetwork, PickupItem item)
        {
            if (___playerNetwork.IsLocalPlayer)
            {
                foreach (Mod mod in ModManagerPage.activeModInstances)
                {
                    if (mod)
                        mod.LocalPlayerEvent_PickupItem(item);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Helper))]
    [HarmonyPatch("DropItem")]
    class Patch_LocalPlayerEvent_DropItem
    {
        static void Postfix(Helper __instance, ItemInstance item, Vector3 position, Vector3 direction, bool parentedToRaft)
        {
            foreach (Mod mod in ModManagerPage.activeModInstances)
            {
                if (mod)
                    mod.LocalPlayerEvent_DropItem(item, position, direction, parentedToRaft);
            }
        }
    }

    [HarmonyPatch(typeof(Raft_Network))]
    [HarmonyPatch("AddPlayer")]
    [HarmonyPatch(new System.Type[] { typeof(CSteamID), typeof(RGD_Settings_Character) })]
    class Patch_WorldEvent_OnPlayerConnected_Host
    {
        static async void Postfix(Raft_Network __instance, CSteamID steamID, RGD_Settings_Character characterSettings)
        {
            if (Raft_Network.IsHost)
            {
                foreach (Mod mod in ModManagerPage.activeModInstances)
                {
                    if (mod)
                        mod.WorldEvent_OnPlayerConnected(steamID, characterSettings);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Raft_Network))]
    [HarmonyPatch("AddPlayer")]
    [HarmonyPatch(new System.Type[] { typeof(Message_Player_Create) })]
    class Patch_WorldEvent_OnPlayerConnected_NotHost
    {
        static async void Postfix(Raft_Network __instance, Message_Player_Create msg)
        {
            if (!Raft_Network.IsHost)
            {
                CSteamID id = msg.SteamID;
                RGD_Settings_Character characterSettings = ComponentManager<Raft_Network>.Value.GetPlayerFromID(id)?.characterSettings;
                if (characterSettings != null)
                {
                    foreach (Mod mod in ModManagerPage.activeModInstances)
                    {
                        if (mod)
                            mod.WorldEvent_OnPlayerConnected(id, characterSettings);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Raft_Network))]
    [HarmonyPatch("OnDisconnect")]
    class Patch_WorldEvent_OnPlayerDisconnected
    {
        static async void Postfix(Raft_Network __instance, CSteamID remoteID, DisconnectReason disconnectReason)
        {
            foreach (Mod mod in ModManagerPage.activeModInstances)
            {
                if (mod)
                    mod.WorldEvent_OnPlayerDisconnected(remoteID, disconnectReason);
            }
        }
    }

    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("OnRespawnComplete")]
    class Patch_LocalPlayerEvent_OnRespawn
    {
        static async void Postfix(Player __instance)
        {
            foreach (Mod mod in ModManagerPage.activeModInstances)
            {
                if (mod)
                    mod.LocalPlayerEvent_OnRespawn();
            }
        }
    }

    [HarmonyPatch(typeof(AchievementHandler))]
    [HarmonyPatch("OnLeaveGame")]
    class Patch_WorldEvent_WorldUnloaded
    {
        static async void Postfix(Raft_Network __instance)
        {
            foreach (Mod mod in ModManagerPage.activeModInstances)
            {
                if (mod)
                    mod.WorldEvent_WorldUnloaded();
            }
        }
    }
}
#endif