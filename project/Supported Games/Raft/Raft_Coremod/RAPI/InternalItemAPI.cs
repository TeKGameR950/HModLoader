using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RaftModLoader
{
    public class InternalItemAPI
    {
        public static Dictionary<Item_Base, ItemObject> itemObjects = new Dictionary<Item_Base, ItemObject>();

        public static void SetItemObject(Network_Player player)
        {
            foreach (KeyValuePair<Item_Base, ItemObject> itemObject in itemObjects)
            {
                if (itemObject.Key != null && ItemManager.GetItemByName(itemObject.Key.UniqueName) != null && itemObject.Value != null)
                {
                    Transform parent = player.rightHandParent;
                    if (itemObject.Value.parent == RItemHand.leftHand) { parent = player.leftHandParent; }

                    if (!parent.Find("ModdedItem_" + itemObject.Key.UniqueName))
                    {
                        GameObject gameobject = GameObject.Instantiate(itemObject.Value.prefab, parent).NoteAsRML();
                        gameobject.name = "ModdedItem_" + itemObject.Key.UniqueName;
                        gameobject.transform.localPosition = Vector3.zero;
                        gameobject.SetActive(false);
                        ItemConnection itemConnection = new ItemConnection();
                        itemConnection.name = "ModdedItem_" + itemObject.Key.UniqueName;
                        itemConnection.inventoryItem = itemObject.Key;
                        itemConnection.obj = gameobject;
                        (Traverse.Create(player.PlayerItemManager.useItemController).Field("connectionDictionary").GetValue() as Dictionary<string, ItemConnection>).Add(itemObject.Key.UniqueName, itemConnection);
                    }
                }
            }
            player.GetComponentInChildren<PlayerFOVManager>().Initialize();
        }

        [Serializable]
        public class ItemObject
        {
            public GameObject prefab;
            public RItemHand parent = RItemHand.rightHand;

            public ItemObject(GameObject _prefab, RItemHand _parent)
            {
                prefab = _prefab;
                parent = _parent;
            }
        }
    }
}