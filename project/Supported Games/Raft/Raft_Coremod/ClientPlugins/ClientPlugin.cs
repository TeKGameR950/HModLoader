using UnityEngine;

namespace RaftModLoader.ClientPlugin
{
    public class ClientPlugin : MonoBehaviour
    {
        public ClientPluginData pluginData;

        public virtual byte[] GetEmbeddedFileBytes(string path)
        {
            byte[] value = null;
            pluginData.files.TryGetValue(path, out value);
            if (value == null)
                Debug.LogError("[ClientPluginManager] " + pluginData.name + " > The file " + path + " doesn't exist!");
            return value;
        }
    }
}