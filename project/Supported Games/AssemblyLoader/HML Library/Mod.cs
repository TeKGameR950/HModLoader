using UnityEngine;
using HMLLibrary;
using System.Linq;
using System;
using System.Reflection;
using System.Collections.Generic;
#if GAME_RAFT
using Steamworks;
#endif

namespace HMLLibrary
{
    public class Mod : MonoBehaviour
    {
        public ModData modlistEntry;
        public static Mod modInstance;
        public Dictionary<string, MethodInfo> methods = new Dictionary<string, MethodInfo>();
        public bool delayWorldLoading = false;

        public Mod()
        {
            modInstance = this;
            /*List<MethodInfo> m = Assembly.GetCallingAssembly().GetTypes().SelectMany(x => x.GetMethods((BindingFlags)62)).ToList();
            m.ForEach(x =>
            {
                ModMethodAttribute attr = (ModMethodAttribute)x.GetCustomAttribute(typeof(ModMethodAttribute));
                if (attr != null)
                {
                    string name = string.IsNullOrWhiteSpace(attr.Name) ? x.Name : attr.Name;
                    methods.Add(name, x);
                    Debug.Log(Call(name));
                }
            });*/
        }

        /*public object Call(string name, params object[] args)
        {
            if (methods.ContainsKey(name))
            {
                MethodInfo m = methods[name];
                Debug.Log(m);
            }
            return null;
        }

        public T Call<T>(string name, params object[] args)
        {
            return (T)((object)Convert.ChangeType(Call(name, args), typeof(T)));
        }*/

        public virtual void UnloadMod()
        {
            modlistEntry.modinfo.modHandler.UnloadMod(modlistEntry);
        }

        public virtual byte[] GetEmbeddedFileBytes(string path)
        {
            byte[] value = null;
            modlistEntry.modinfo.modFiles.TryGetValue(path, out value);
            if (value == null)
                Debug.LogError("[ModManager] " + modlistEntry.jsonmodinfo.name + " > The file " + path + " doesn't exist!");
            return value;
        }

        public JsonModInfo GetModInfo() { return modlistEntry.jsonmodinfo; }

        public void Log(object message)
        {
            Debug.Log("[" + GetModInfo().name + "] : " + message.ToString());
        }

#if GAME_RAFT
        public virtual void WorldEvent_WorldLoaded()
        {
        }

        public virtual void WorldEvent_WorldSaved()
        {
        }

        public virtual void LocalPlayerEvent_Hurt(float damage, Vector3 hitPoint, Vector3 hitNormal, EntityType damageInflictorEntityType)
        {
        }

        public virtual void LocalPlayerEvent_Death(Vector3 deathPosition)
        {
        }

        public virtual void LocalPlayerEvent_Respawn()
        {
        }

        public virtual void LocalPlayerEvent_ItemCrafted(Item_Base item)
        {
        }

        public virtual void LocalPlayerEvent_PickupItem(PickupItem item)
        {
        }

        public virtual void LocalPlayerEvent_DropItem(ItemInstance item, Vector3 position, Vector3 direction, bool parentedToRaft)
        {
        }

        public virtual void LocalPlayerEvent_OnRespawn()
        {
        }

        public virtual void WorldEvent_OnPlayerConnected(CSteamID steamid, RGD_Settings_Character characterSettings)
        {
        }

        public virtual void WorldEvent_OnPlayerDisconnected(CSteamID steamid, DisconnectReason disconnectReason)
        {
        }

        public virtual void WorldEvent_WorldUnloaded()
        {
        }
#endif

        // JsonModInfo Exposed Fields
        public new string name
        {
            get
            {
                return GetModInfo().name;
            }
        }
        public string author
        {
            get
            {
                return GetModInfo().author;
            }
        }
        public string description
        {
            get
            {
                return GetModInfo().description;
            }
        }
        public string version
        {
            get
            {
                return GetModInfo().version;
            }
        }
        public string license
        {
            get
            {
                return GetModInfo().license;
            }
        }
        public string icon
        {
            get
            {
                return GetModInfo().icon;
            }
        }
        public string banner
        {
            get
            {
                return GetModInfo().banner;
            }
        }
        public string gameVersion
        {
            get
            {
                return GetModInfo().gameVersion;
            }
        }
        public string updateUrl
        {
            get
            {
                return GetModInfo().updateUrl;
            }
        }
        public bool isModPermanent
        {
            get
            {
                return GetModInfo().isModPermanent;
            }
        }
        public bool requiredByAllPlayers
        {
            get
            {
                return GetModInfo().requiredByAllPlayers;
            }
        }
        public List<string> excludedFiles
        {
            get
            {
                return GetModInfo().excludedFiles;
            }
        }
    }
}