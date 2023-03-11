using AssemblyLoader;
using FMODUnity;
using HarmonyLib;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RaftModLoader
{
    [HarmonyPatch(typeof(LoadGameBox), "Start")]
    static class Patch_LoadGameBox_Start
    {
        static void Postfix(LoadGameBox __instance) => ComponentManager<LoadGameBox>.Value = __instance;
    }

    [HarmonyPatch(typeof(NewGameBox), "Start")]
    static class Patch_NewGameBox_Start
    {
        static void Postfix(NewGameBox __instance) => ComponentManager<NewGameBox>.Value = __instance;
    }

    [HarmonyPatch(typeof(Harmony))]
    [HarmonyPatch("UnpatchAll")]
    static class HarmonyUnpatchAllFix
    {
        static bool Prefix(Harmony __instance, string harmonyID)
        {
            if (harmonyID == null)
            {
                string id = __instance.Id;
                if (id != null && id != "" && !string.IsNullOrWhiteSpace(id))
                {
                    Debug.LogWarning("[HarmonyFix] Warning! UnpatchAll() has been called with no id! Trying to call it on id \"" + id + "\"");
                    __instance.UnpatchAll(id);
                    return false;
                }
                else
                {
                    Debug.LogError("[HarmonyFix] Error! UnpatchAll() has been called with no id and no id could be found! Cancelled invoke.");
                    return false;
                }
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(UseItemController))]
    [HarmonyPatch("StartUsing")]
    static class UseItemController_StartUsing_ModdedItemFix
    {
        static bool Prefix(UseItemController __instance, Item_Base item, Dictionary<string, ItemConnection> ___connectionDictionary, ref Item_Base ___usableItem, ref PlayerAnimator ___playerAnimator, ref Network_Player ___playerNetwork, ref StudioEventEmitter ___SoundEmitter_Equip, ref ItemConnection ___activeObject)
        {
            string itemNameFromUsable = __instance.GetItemNameFromUsable(item);
            if (itemNameFromUsable != string.Empty && !___connectionDictionary.ContainsKey(itemNameFromUsable))
            {
                UnityEngine.Debug.LogWarning("[RAPI] The item " + item.UniqueName + " is not correctly configured. Please use RAPI.SetItemObject() to fix it.");
                ___usableItem = item;
                if (item.settings_usable.AnimationOnSelect != PlayerAnimation.None)
                {
                    ___playerAnimator.SetAnimation(item.settings_usable.AnimationOnSelect, item.settings_usable.ForceAnimationIndex, item.settings_usable.SetTriggering);
                }
                if (___playerNetwork.IsLocalPlayer && item.settings_recipe.CraftingCategory == CraftingCategory.Tools)
                {
                    ___SoundEmitter_Equip.Play();
                }
                ___activeObject = null;
                __instance.OnSelectItem?.Invoke(item);
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Raft_Network))]
    [HarmonyPatch("AddPlayer")]
    [HarmonyPatch(new Type[] { typeof(CSteamID), typeof(RGD_Settings_Character) })]
    class Patch_AddPlayer1
    {
        static void Postfix(Raft_Network __instance, CSteamID steamID)
        {
            Network_Player player = __instance.GetPlayerFromID(steamID);
            if (player != null)
            {
                InternalItemAPI.SetItemObject(player);
            }
        }
    }

    [HarmonyPatch(typeof(Raft_Network))]
    [HarmonyPatch("AddPlayer")]
    [HarmonyPatch(new Type[] { typeof(Message_Player_Create) })]
    class Patch_AddPlayer2
    {
        static void Postfix(Raft_Network __instance, Message_Player_Create msg)
        {
            Network_Player player = __instance.GetPlayerFromID(msg.SteamID);
            if (player != null)
            {
                InternalItemAPI.SetItemObject(player);
            }
        }
    }

    [HarmonyPatch(typeof(MyInput))]
    [HarmonyPatch("CanGetInput", MethodType.Getter)]
    class Patch_RequestWorld
    {
        static bool Prefix(ref bool __result)
        {
            if (RConsole.isOpen || MainMenu.IsOpen)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(StartMenuScreen))]
    [HarmonyPatch("ShowBox")]
    class Patch_JoinGetBox
    {
        static bool Prefix(MenuBox box)
        {
            if (HLoader.SAFEMODE && box is JoinGameBox)
            {
                MainMenu.instance.CurrentPage = "Servers";
                if (!MainMenu.IsOpen)
                    MainMenu.instance.OpenMenu();
                return false;
            }
            return true;
        }
    }


    [HarmonyPatch(typeof(Storage_Small))]
    [HarmonyPatch("UpdateStorageFillRenderers")]
    class Patch_Storages
    {
        static bool Prefix(Storage_Small __instance, Inventory ___inventoryReference, Storage_Small.StorageFill[] ___storageFillers)
        {
            if (___inventoryReference == null || ___storageFillers == null)
            {
                return false;
            }
            return true;
        }
    }
}