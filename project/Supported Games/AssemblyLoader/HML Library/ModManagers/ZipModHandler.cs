using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;

namespace HMLLibrary
{
    public class ZipModHandler : BaseModHandler
    {
        public override async Task<ModData> GetModData(FileInfo file)
        {
            ModData moddata = new ModData();
            moddata.modinfo.fileHash = HUtils.GetFileSHA512Hash(file.FullName);
            moddata = dataCache.TryGetValue(moddata.modinfo.fileHash, out moddata) ? moddata : await GetModDataFromZipStream(file);
            if (moddata == null)
            {
                Debug.LogWarning("[ModManager] " + file.Name + " > An error occured while opening the file.");
                return null;
            }
            ModManagerPage.loadedAssemblies.TryGetValue(file.Name, out moddata.modinfo.assembly);
            if (!dataCache.ContainsKey(moddata.modinfo.fileHash))
            {
                dataCache.Add(moddata.modinfo.fileHash, moddata);
            }
            return moddata;
        }

        public async Task<ModData> GetModDataFromZipStream(FileInfo file)
        {
            ModData moddata = new ModData();
            moddata.jsonmodinfo = null;
            moddata.modinfo.modHandler = this;
            moddata.modinfo.modFile = file;
            moddata.modinfo.fileHash = HUtils.GetFileSHA512Hash(file.FullName);
            try
            {
                using (Stream stream = File.OpenRead(file.FullName))
                {
                    using (var zipInputStream = new ZipInputStream(stream))
                    {
                        while (zipInputStream.GetNextEntry() is ZipEntry v)
                        {
                            var zipentry = v.Name;
                            StreamReader reader = new StreamReader(zipInputStream);

                            if (!moddata.modinfo.modFiles.ContainsKey(zipentry))
                            {
                                try
                                {
                                    var bytes = default(byte[]);
                                    using (var memstream = new MemoryStream())
                                    {
                                        reader.BaseStream.CopyTo(memstream);
                                        bytes = memstream.ToArray();
                                    }
                                    moddata.modinfo.modFiles.Add(zipentry, bytes);
                                    if (zipentry.ToLower().EndsWith("modinfo.json"))
                                    {
                                        try
                                        {
                                            moddata.jsonmodinfo = JsonConvert.DeserializeObject<JsonModInfo>(Encoding.UTF8.GetString(bytes));
                                        }
                                        catch (Exception e)
                                        {
                                            Debug.LogError("[ModManager] " + file.Name + " > An error occured while deserializing the modinfo.json file!\nStacktrace : " + e.Message);
                                            return null;
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    Debug.LogError("[ModManager] " + file.Name + " > An error occured while loading the file " + zipentry + " !\nStacktrace : " + e.ToString());
                                    return null;
                                }
                            }
                        }

                        return moddata;
                    }
                }
            }
            catch { return null; }
        }
    }
}