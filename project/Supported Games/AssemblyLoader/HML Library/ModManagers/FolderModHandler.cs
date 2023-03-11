using Newtonsoft.Json;
using ShellLink;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;

namespace HMLLibrary
{
    public class FolderModHandler : BaseModHandler
    {
        public override async Task<ModData> GetModData(FileInfo file)
        {
            ModData moddata = new ModData();
            string path = Shortcut.ReadFromFile(file.FullName).LinkTargetIDList.Path;
            if (!Directory.Exists(path)) { return null; }
            moddata = GetModDataFromFolder(file, new DirectoryInfo(path));
            ModManagerPage.loadedAssemblies.TryGetValue(file.Name, out moddata.modinfo.assembly);
            return moddata;
        }

        public ModData GetModDataFromFolder(FileInfo file, DirectoryInfo directory)
        {
            ModData moddata = new ModData();
            moddata.jsonmodinfo = null;
            moddata.modinfo.isShortcut = true;
            moddata.modinfo.shortcutFolder = directory.FullName;
            moddata.modinfo.modHandler = this;
            moddata.modinfo.modFile = file;
            moddata.modinfo.fileHash = HUtils.CreateSHA512ForFolder(directory.FullName);
            FileInfo[] files = GetFolderFiles(directory);

            foreach (FileInfo entry in files)
            {
                string entryname = entry.FullName.Replace(directory.FullName + "\\", "");
                if (!moddata.modinfo.modFiles.ContainsKey(entryname))
                {
                    try
                    {
                        byte[] bytes = File.ReadAllBytes(entry.FullName);

                        moddata.modinfo.modFiles.Add(entryname, bytes);
                        if (entryname.ToLower().EndsWith("modinfo.json"))
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
                        Debug.LogError("[ModManager] " + file.Name + " > An error occured while loading the file " + entryname + " !\nStacktrace : " + e.ToString());
                        return null;
                    }
                }
            }

            // excludedFiles, should be improved to support wildcards etc
            foreach (string s in moddata.jsonmodinfo.excludedFiles)
            {
                foreach (KeyValuePair<string, byte[]> entry in moddata.modinfo.modFiles.ToArray())
                {
                    if (s.EndsWith(entry.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        moddata.modinfo.modFiles.Remove(entry.Key);
                    }
                }
            }
            return moddata;
        }

        public FileInfo[] GetFolderFiles(DirectoryInfo dir)
        {
            List<FileInfo> files = new List<FileInfo>();
            files.AddRange(dir.GetFiles("*", SearchOption.TopDirectoryOnly));
            DirectoryInfo[] directories = dir.GetDirectories("*", SearchOption.AllDirectories);
            directories.ToList().ForEach(d =>
            {
                string parentname = d.FullName.Replace(dir.FullName + "\\", "");
                if (!parentname.StartsWith("bin") && !parentname.StartsWith("obj"))
                {
                    files.AddRange(d.GetFiles("*", SearchOption.AllDirectories));
                }
            });
            return files.ToArray();
        }
    }

}