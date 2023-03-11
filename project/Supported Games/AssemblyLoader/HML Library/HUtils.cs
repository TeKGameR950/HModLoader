using AssemblyLoader;
using ICSharpCode.SharpZipLib.Zip;
using ShellLink;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace HMLLibrary
{
    public static class HUtils
    {
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static void CopyToClipboard(this string s)
        {
            TextEditor te = new TextEditor();
            te.text = s;
            te.SelectAll();
            te.Copy();
        }

        public static bool IsFileLocked(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                return true;
            }
            return false;
        }

        public static bool AddFileToMod(ModData moddata, string filepath, string name)
        {
            try
            {
                ZipFile zipFile = new ZipFile(moddata.modinfo.modFile.FullName);
                zipFile.BeginUpdate();
                zipFile.Add(filepath, name);
                zipFile.CommitUpdate();
                zipFile.Close();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("[ModCompiler] " + moddata.modinfo.modFile.Name + " > Adding file to mod failed!\n" + e.ToString());
                return false;
            }
        }

        public async static Task<AssetBundle> TaskLoadAssetBundleFromMemoryAsync(byte[] bytes)
        {
            AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(bytes);
            while (!request.isDone) { await Task.Delay(1); }
            return request.assetBundle;
        }

        public async static Task<T> TaskLoadAssetAsync<T>(this AssetBundle bundle, string name)
        {
            AssetBundleRequest request = bundle.LoadAssetAsync<T>(name);
            while (!request.isDone) { await Task.Delay(1); }
            return (T)Convert.ChangeType(request.asset, typeof(T));
        }

        public static string CreateSHA512ForFolder(string path)
        {
            // assuming you want to include nested folders
            var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                                 .OrderBy(p => p).ToList();

            SHA512 sha512 = SHA512.Create();

            for (int i = 0; i < files.Count; i++)
            {
                string file = files[i];

                // hash path
                string relativePath = file.Substring(path.Length + 1);
                byte[] pathBytes = Encoding.UTF8.GetBytes(relativePath.ToLower());
                sha512.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);

                // hash contents
                byte[] contentBytes = File.ReadAllBytes(file);
                if (i == files.Count - 1)
                    sha512.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
                else
                    sha512.TransformBlock(contentBytes, 0, contentBytes.Length, contentBytes, 0);
            }

            return BitConverter.ToString(sha512.Hash).Replace("-", "").ToLower();
        }

        public static string ReplaceCaseInsensitive(this string input, string search, string replacement)
        {
            string result = Regex.Replace(
                input,
                Regex.Escape(search),
                replacement.Replace("$", "$$"),
                RegexOptions.IgnoreCase
            );
            return result;
        }

        public static string StripRichText(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        public static string GetStringSHA512Hash(string strToEncrypt)
        {
            UTF8Encoding ue = new UTF8Encoding();
            byte[] bytes = ue.GetBytes(strToEncrypt);

            SHA512CryptoServiceProvider sha512 = new SHA512CryptoServiceProvider();
            byte[] hashBytes = sha512.ComputeHash(bytes);

            string hashString = "";

            for (int i = 0; i < hashBytes.Length; i++)
            {
                hashString += Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
            }

            return hashString.PadLeft(32, '0');
        }

        public static string GetFileSHA512Hash(string filename)
        {
            using (var sha512 = SHA512.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = sha512.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "");
                }
            }
        }

        public static string CalculateSHA512FromBytes(byte[] bytes)
        {
            using (var sha512 = SHA512.Create())
            {
                using (var stream = new MemoryStream(bytes))
                {
                    var hash = sha512.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToUpperInvariant();
                }
            }
        }

        public static string GetShortcutTarget(string file)
        {
            try
            {
                string link = Shortcut.ReadFromFile(file).LinkTargetIDList.Path;
                if (File.Exists(link)) { return link; }
            }
            catch { }
            return "";
        }

        public static async Task<int> MakeAsync()
        {
            await Task.Delay(1);
            return 0;
        }

        public static string GetMLAssemblyUniqueKey(Type p)
        {
            return GetStringSHA512Hash(p.Assembly.FullName + typeof(HUtils).FullName + typeof(HLoader).GetFields().Length);
        }

        public static async Task<string> GetModVersion(string url)
        {
            if (url != "Unknown" && !string.IsNullOrWhiteSpace(url))
            {
                DateTime now = DateTime.Now;
                using (UnityWebRequest www = UnityWebRequest.Get(url))
                {
                    www.certificateHandler = new HttpCertHandler();
                    www.SendWebRequest();
                    bool cancelTimeOut = false;
                    while (!cancelTimeOut && !www.isDone && !www.isNetworkError && !www.isHttpError)
                    {
                        if (now.AddSeconds(5) <= DateTime.Now)
                            cancelTimeOut = true;
                        await Task.Delay(1);
                    }

                    if (!www.isNetworkError && !www.isHttpError && string.IsNullOrEmpty(www.error))
                    {
                        string latestVersion = www.downloadHandler.text;
                        return www.downloadHandler.text;
                    }
                }
                return "";
            }
            else
            {
                return "";
            }
        }

        public static async Task<Texture> DownloadCachedTexture(string url)
        {
            string cachedFile = Path.Combine(HLib.path_cacheFolder_textures, GetStringSHA512Hash(url) + ".png");
            if (File.Exists(cachedFile))
            {
                try
                {
                    Texture2D texture = new Texture2D(2, 2, TextureFormat.RGB24, false);
                    texture.LoadImage(File.ReadAllBytes(cachedFile));
                    if (texture.height > 8 && texture.width > 8)
                    {
                        return texture;
                    }
                    else
                    {
                        File.Delete(cachedFile);
                    }
                }
                catch { File.Delete(cachedFile); }
            }
            DateTime now = DateTime.Now;
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            www.certificateHandler = new HttpCertHandler();
            www.SendWebRequest();
            bool cancelTimeOut = false;
            while (!cancelTimeOut && !www.isDone && !www.isNetworkError && !www.isHttpError)
            {
                if (now.AddSeconds(5) > DateTime.Now)
                {
                    await Task.Delay(1);
                }
                else
                {
                    await Task.Delay(1);
                    cancelTimeOut = true;
                }
            }

            if (!www.isNetworkError && !www.isHttpError && !cancelTimeOut)
            {
                Texture texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                if (texture.width > 8 && texture.height > 8)
                {
                    File.WriteAllBytes(cachedFile, www.downloadHandler.data);
                    return texture;
                }
                else
                {
                    return HLib.missingTexture;
                }
            }
            else
            {
                return HLib.missingTexture;
            }
        }

        public static string GetDateFormatted(string datetimestamp)
        {
            DateTime date = DateTime.Parse(datetimestamp);
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            var ts = new TimeSpan(DateTime.Now.Ticks - date.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
                return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";

            if (delta < 2 * MINUTE)
                return "a minute and " + ts.Seconds + " seconds ago";

            if (delta < 45 * MINUTE)
                return ts.Minutes + " minutes and " + ts.Seconds + " seconds ago";

            if (delta < 90 * MINUTE)
                return "an hour and " + ts.Minutes + " minutes ago";

            if (delta < 24 * HOUR)
                return ts.Hours + " hours and " + ts.Minutes + " minutes ago";

            if (delta < 48 * HOUR)
                return "yesterday";

            if (delta < 30 * DAY)
                return ts.Days + " days and " + ts.Hours + " hours ago";

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months and " + ts.Days + " days ago";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "one year ago" : years + " years ago";
            }
        }

        public static async Task<Texture> DownloadUncachedTexture(string url)
        {
            DateTime now = DateTime.Now;
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            www.certificateHandler = new HttpCertHandler();
            www.SendWebRequest();
            bool cancelTimeOut = false;
            while (!cancelTimeOut && !www.isDone && !www.isNetworkError && !www.isHttpError)
            {
                if (now.AddSeconds(5) > DateTime.Now)
                {
                    await Task.Delay(1);
                }
                else
                {
                    await Task.Delay(1);
                    cancelTimeOut = true;
                }
            }

            if (!www.isNetworkError && !www.isHttpError && !cancelTimeOut)
            {
                Texture texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                if (texture.width > 8 && texture.height > 8)
                {
                    return texture;
                }
                else
                {
                    return HLib.missingTexture;
                }
            }
            else
            {
                return HLib.missingTexture;
            }
        }

        public static string RML_InsertEmoji(this string val)
        {
            string t = val;
            foreach (KeyValuePair<string, string> e in emoji)
            {
                t = t.Replace(e.Key, e.Value);
            }
            return t;
        }

        public static KeyCode KeyCodeFromString(string keyString)
        {
            if (keyString == "~")
            {
                return KeyCode.Tilde;
            }
            if (keyString.Length == 1)
            {
                keyString = keyString.ToUpper();
            }

            KeyCode key = KeyCode.None;
            try
            {
                key = (KeyCode)Enum.Parse(typeof(KeyCode), keyString);
            }
            catch (ArgumentException)
            {
                Debug.LogError("Key '" + keyString + "' does not specify a key code.");
            }
            return key;
        }

        /*public static List<ModData> DependencySort(IEnumerable<ModData> source, Func<ModData, IEnumerable<ModData>> getDependencies)
        {

            var sorted = new List<ModData>();
            var visited = new Dictionary<ModData, bool>();

            foreach (var item in source)
            {
                DependencyVisit(item, getDependencies, sorted, visited);
            }

            return sorted;
        }

        public static void DependencyVisit(ModData item, Func<ModData, IEnumerable<ModData>> getDependencies, List<ModData> sorted, Dictionary<ModData, bool> visited)
        {
            bool inProcess;
            var alreadyVisited = visited.TryGetValue(item, out inProcess);

            if (alreadyVisited)
            {
                if (inProcess)
                {
                    foreach (string p in item.jsonmodinfo.ModDependencies)
                    {
                        if (p == item.jsonmodinfo.ModPackage)
                        {
                            return;
                        }
                    }
                    Debug.LogError("[ModManager] " + item.jsonmodinfo.ModName + " > The mod has a cyclic dependency!");
                    return;
                }
            }
            else
            {
                visited[item] = true;

                var dependencies = getDependencies(item);
                if (dependencies != null)
                {
                    foreach (var dependency in dependencies)
                    {
                        DependencyVisit(dependency, getDependencies, sorted, visited);
                    }
                }

                visited[item] = false;
                sorted.Add(item);
            }
        }*/

        public static Dictionary<string, string> emoji = new Dictionary<string, string>() {{":hash:","<sprite name=\"0023-fe0f-20e3\">" },
{":keycap_star:","<sprite name=\"002a-fe0f-20e3\">" },
{":zero:","<sprite name=\"0030-fe0f-20e3\">" },
{":one:","<sprite name=\"0031-fe0f-20e3\">" },
{":two:","<sprite name=\"0032-fe0f-20e3\">" },
{":three:","<sprite name=\"0033-fe0f-20e3\">" },
{":four:","<sprite name=\"0034-fe0f-20e3\">" },
{":five:","<sprite name=\"0035-fe0f-20e3\">" },
{":six:","<sprite name=\"0036-fe0f-20e3\">" },
{":seven:","<sprite name=\"0037-fe0f-20e3\">" },
{":eight:","<sprite name=\"0038-fe0f-20e3\">" },
{":nine:","<sprite name=\"0039-fe0f-20e3\">" },
{":copyright:","<sprite name=\"00a9-fe0f\">" },
{":registered:","<sprite name=\"00ae-fe0f\">" },
{":mahjong:","<sprite name=\"1f004\">" },
{":black_joker:","<sprite name=\"1f0cf\">" },
{":a:","<sprite name=\"1f170-fe0f\">" },
{":b:","<sprite name=\"1f171-fe0f\">" },
{":o2:","<sprite name=\"1f17e-fe0f\">" },
{":parking:","<sprite name=\"1f17f-fe0f\">" },
{":ab:","<sprite name=\"1f18e\">" },
{":cl:","<sprite name=\"1f191\">" },
{":cool:","<sprite name=\"1f192\">" },
{":free:","<sprite name=\"1f193\">" },
{":id:","<sprite name=\"1f194\">" },
{":new:","<sprite name=\"1f195\">" },
{":ng:","<sprite name=\"1f196\">" },
{":ok:","<sprite name=\"1f197\">" },
{":sos:","<sprite name=\"1f198\">" },
{":up:","<sprite name=\"1f199\">" },
{":vs:","<sprite name=\"1f19a\">" },
{":flag-ac:","<sprite name=\"1f1e6-1f1e8\">" },
{":flag-ad:","<sprite name=\"1f1e6-1f1e9\">" },
{":flag-ae:","<sprite name=\"1f1e6-1f1ea\">" },
{":flag-af:","<sprite name=\"1f1e6-1f1eb\">" },
{":flag-ag:","<sprite name=\"1f1e6-1f1ec\">" },
{":flag-ai:","<sprite name=\"1f1e6-1f1ee\">" },
{":flag-al:","<sprite name=\"1f1e6-1f1f1\">" },
{":flag-am:","<sprite name=\"1f1e6-1f1f2\">" },
{":flag-ao:","<sprite name=\"1f1e6-1f1f4\">" },
{":flag-aq:","<sprite name=\"1f1e6-1f1f6\">" },
{":flag-ar:","<sprite name=\"1f1e6-1f1f7\">" },
{":flag-as:","<sprite name=\"1f1e6-1f1f8\">" },
{":flag-at:","<sprite name=\"1f1e6-1f1f9\">" },
{":flag-au:","<sprite name=\"1f1e6-1f1fa\">" },
{":flag-aw:","<sprite name=\"1f1e6-1f1fc\">" },
{":flag-ax:","<sprite name=\"1f1e6-1f1fd\">" },
{":flag-az:","<sprite name=\"1f1e6-1f1ff\">" },
{":flag-ba:","<sprite name=\"1f1e7-1f1e6\">" },
{":flag-bb:","<sprite name=\"1f1e7-1f1e7\">" },
{":flag-bd:","<sprite name=\"1f1e7-1f1e9\">" },
{":flag-be:","<sprite name=\"1f1e7-1f1ea\">" },
{":flag-bf:","<sprite name=\"1f1e7-1f1eb\">" },
{":flag-bg:","<sprite name=\"1f1e7-1f1ec\">" },
{":flag-bh:","<sprite name=\"1f1e7-1f1ed\">" },
{":flag-bi:","<sprite name=\"1f1e7-1f1ee\">" },
{":flag-bj:","<sprite name=\"1f1e7-1f1ef\">" },
{":flag-bl:","<sprite name=\"1f1e7-1f1f1\">" },
{":flag-bm:","<sprite name=\"1f1e7-1f1f2\">" },
{":flag-bn:","<sprite name=\"1f1e7-1f1f3\">" },
{":flag-bo:","<sprite name=\"1f1e7-1f1f4\">" },
{":flag-bq:","<sprite name=\"1f1e7-1f1f6\">" },
{":flag-br:","<sprite name=\"1f1e7-1f1f7\">" },
{":flag-bs:","<sprite name=\"1f1e7-1f1f8\">" },
{":flag-bt:","<sprite name=\"1f1e7-1f1f9\">" },
{":flag-bv:","<sprite name=\"1f1e7-1f1fb\">" },
{":flag-bw:","<sprite name=\"1f1e7-1f1fc\">" },
{":flag-by:","<sprite name=\"1f1e7-1f1fe\">" },
{":flag-bz:","<sprite name=\"1f1e7-1f1ff\">" },
{":flag-ca:","<sprite name=\"1f1e8-1f1e6\">" },
{":flag-cc:","<sprite name=\"1f1e8-1f1e8\">" },
{":flag-cd:","<sprite name=\"1f1e8-1f1e9\">" },
{":flag-cf:","<sprite name=\"1f1e8-1f1eb\">" },
{":flag-cg:","<sprite name=\"1f1e8-1f1ec\">" },
{":flag-ch:","<sprite name=\"1f1e8-1f1ed\">" },
{":flag-ci:","<sprite name=\"1f1e8-1f1ee\">" },
{":flag-ck:","<sprite name=\"1f1e8-1f1f0\">" },
{":flag-cl:","<sprite name=\"1f1e8-1f1f1\">" },
{":flag-cm:","<sprite name=\"1f1e8-1f1f2\">" },
{":cn:","<sprite name=\"1f1e8-1f1f3\">" },
{":flag-co:","<sprite name=\"1f1e8-1f1f4\">" },
{":flag-cp:","<sprite name=\"1f1e8-1f1f5\">" },
{":flag-cr:","<sprite name=\"1f1e8-1f1f7\">" },
{":flag-cu:","<sprite name=\"1f1e8-1f1fa\">" },
{":flag-cv:","<sprite name=\"1f1e8-1f1fb\">" },
{":flag-cw:","<sprite name=\"1f1e8-1f1fc\">" },
{":flag-cx:","<sprite name=\"1f1e8-1f1fd\">" },
{":flag-cy:","<sprite name=\"1f1e8-1f1fe\">" },
{":flag-cz:","<sprite name=\"1f1e8-1f1ff\">" },
{":de:","<sprite name=\"1f1e9-1f1ea\">" },
{":flag-dg:","<sprite name=\"1f1e9-1f1ec\">" },
{":flag-dj:","<sprite name=\"1f1e9-1f1ef\">" },
{":flag-dk:","<sprite name=\"1f1e9-1f1f0\">" },
{":flag-dm:","<sprite name=\"1f1e9-1f1f2\">" },
{":flag-do:","<sprite name=\"1f1e9-1f1f4\">" },
{":flag-dz:","<sprite name=\"1f1e9-1f1ff\">" },
{":flag-ea:","<sprite name=\"1f1ea-1f1e6\">" },
{":flag-ec:","<sprite name=\"1f1ea-1f1e8\">" },
{":flag-ee:","<sprite name=\"1f1ea-1f1ea\">" },
{":flag-eg:","<sprite name=\"1f1ea-1f1ec\">" },
{":flag-eh:","<sprite name=\"1f1ea-1f1ed\">" },
{":flag-er:","<sprite name=\"1f1ea-1f1f7\">" },
{":es:","<sprite name=\"1f1ea-1f1f8\">" },
{":flag-et:","<sprite name=\"1f1ea-1f1f9\">" },
{":flag-eu:","<sprite name=\"1f1ea-1f1fa\">" },
{":flag-fi:","<sprite name=\"1f1eb-1f1ee\">" },
{":flag-fj:","<sprite name=\"1f1eb-1f1ef\">" },
{":flag-fk:","<sprite name=\"1f1eb-1f1f0\">" },
{":flag-fm:","<sprite name=\"1f1eb-1f1f2\">" },
{":flag-fo:","<sprite name=\"1f1eb-1f1f4\">" },
{":fr:","<sprite name=\"1f1eb-1f1f7\">" },
{":flag-ga:","<sprite name=\"1f1ec-1f1e6\">" },
{":gb:","<sprite name=\"1f1ec-1f1e7\">" },
{":flag-gd:","<sprite name=\"1f1ec-1f1e9\">" },
{":flag-ge:","<sprite name=\"1f1ec-1f1ea\">" },
{":flag-gf:","<sprite name=\"1f1ec-1f1eb\">" },
{":flag-gg:","<sprite name=\"1f1ec-1f1ec\">" },
{":flag-gh:","<sprite name=\"1f1ec-1f1ed\">" },
{":flag-gi:","<sprite name=\"1f1ec-1f1ee\">" },
{":flag-gl:","<sprite name=\"1f1ec-1f1f1\">" },
{":flag-gm:","<sprite name=\"1f1ec-1f1f2\">" },
{":flag-gn:","<sprite name=\"1f1ec-1f1f3\">" },
{":flag-gp:","<sprite name=\"1f1ec-1f1f5\">" },
{":flag-gq:","<sprite name=\"1f1ec-1f1f6\">" },
{":flag-gr:","<sprite name=\"1f1ec-1f1f7\">" },
{":flag-gs:","<sprite name=\"1f1ec-1f1f8\">" },
{":flag-gt:","<sprite name=\"1f1ec-1f1f9\">" },
{":flag-gu:","<sprite name=\"1f1ec-1f1fa\">" },
{":flag-gw:","<sprite name=\"1f1ec-1f1fc\">" },
{":flag-gy:","<sprite name=\"1f1ec-1f1fe\">" },
{":flag-hk:","<sprite name=\"1f1ed-1f1f0\">" },
{":flag-hm:","<sprite name=\"1f1ed-1f1f2\">" },
{":flag-hn:","<sprite name=\"1f1ed-1f1f3\">" },
{":flag-hr:","<sprite name=\"1f1ed-1f1f7\">" },
{":flag-ht:","<sprite name=\"1f1ed-1f1f9\">" },
{":flag-hu:","<sprite name=\"1f1ed-1f1fa\">" },
{":flag-ic:","<sprite name=\"1f1ee-1f1e8\">" },
{":flag-id:","<sprite name=\"1f1ee-1f1e9\">" },
{":flag-ie:","<sprite name=\"1f1ee-1f1ea\">" },
{":flag-il:","<sprite name=\"1f1ee-1f1f1\">" },
{":flag-im:","<sprite name=\"1f1ee-1f1f2\">" },
{":flag-in:","<sprite name=\"1f1ee-1f1f3\">" },
{":flag-io:","<sprite name=\"1f1ee-1f1f4\">" },
{":flag-iq:","<sprite name=\"1f1ee-1f1f6\">" },
{":flag-ir:","<sprite name=\"1f1ee-1f1f7\">" },
{":flag-is:","<sprite name=\"1f1ee-1f1f8\">" },
{":it:","<sprite name=\"1f1ee-1f1f9\">" },
{":flag-je:","<sprite name=\"1f1ef-1f1ea\">" },
{":flag-jm:","<sprite name=\"1f1ef-1f1f2\">" },
{":flag-jo:","<sprite name=\"1f1ef-1f1f4\">" },
{":jp:","<sprite name=\"1f1ef-1f1f5\">" },
{":flag-ke:","<sprite name=\"1f1f0-1f1ea\">" },
{":flag-kg:","<sprite name=\"1f1f0-1f1ec\">" },
{":flag-kh:","<sprite name=\"1f1f0-1f1ed\">" },
{":flag-ki:","<sprite name=\"1f1f0-1f1ee\">" },
{":flag-km:","<sprite name=\"1f1f0-1f1f2\">" },
{":flag-kn:","<sprite name=\"1f1f0-1f1f3\">" },
{":flag-kp:","<sprite name=\"1f1f0-1f1f5\">" },
{":kr:","<sprite name=\"1f1f0-1f1f7\">" },
{":flag-kw:","<sprite name=\"1f1f0-1f1fc\">" },
{":flag-ky:","<sprite name=\"1f1f0-1f1fe\">" },
{":flag-kz:","<sprite name=\"1f1f0-1f1ff\">" },
{":flag-la:","<sprite name=\"1f1f1-1f1e6\">" },
{":flag-lb:","<sprite name=\"1f1f1-1f1e7\">" },
{":flag-lc:","<sprite name=\"1f1f1-1f1e8\">" },
{":flag-li:","<sprite name=\"1f1f1-1f1ee\">" },
{":flag-lk:","<sprite name=\"1f1f1-1f1f0\">" },
{":flag-lr:","<sprite name=\"1f1f1-1f1f7\">" },
{":flag-ls:","<sprite name=\"1f1f1-1f1f8\">" },
{":flag-lt:","<sprite name=\"1f1f1-1f1f9\">" },
{":flag-lu:","<sprite name=\"1f1f1-1f1fa\">" },
{":flag-lv:","<sprite name=\"1f1f1-1f1fb\">" },
{":flag-ly:","<sprite name=\"1f1f1-1f1fe\">" },
{":flag-ma:","<sprite name=\"1f1f2-1f1e6\">" },
{":flag-mc:","<sprite name=\"1f1f2-1f1e8\">" },
{":flag-md:","<sprite name=\"1f1f2-1f1e9\">" },
{":flag-me:","<sprite name=\"1f1f2-1f1ea\">" },
{":flag-mf:","<sprite name=\"1f1f2-1f1eb\">" },
{":flag-mg:","<sprite name=\"1f1f2-1f1ec\">" },
{":flag-mh:","<sprite name=\"1f1f2-1f1ed\">" },
{":flag-mk:","<sprite name=\"1f1f2-1f1f0\">" },
{":flag-ml:","<sprite name=\"1f1f2-1f1f1\">" },
{":flag-mm:","<sprite name=\"1f1f2-1f1f2\">" },
{":flag-mn:","<sprite name=\"1f1f2-1f1f3\">" },
{":flag-mo:","<sprite name=\"1f1f2-1f1f4\">" },
{":flag-mp:","<sprite name=\"1f1f2-1f1f5\">" },
{":flag-mq:","<sprite name=\"1f1f2-1f1f6\">" },
{":flag-mr:","<sprite name=\"1f1f2-1f1f7\">" },
{":flag-ms:","<sprite name=\"1f1f2-1f1f8\">" },
{":flag-mt:","<sprite name=\"1f1f2-1f1f9\">" },
{":flag-mu:","<sprite name=\"1f1f2-1f1fa\">" },
{":flag-mv:","<sprite name=\"1f1f2-1f1fb\">" },
{":flag-mw:","<sprite name=\"1f1f2-1f1fc\">" },
{":flag-mx:","<sprite name=\"1f1f2-1f1fd\">" },
{":flag-my:","<sprite name=\"1f1f2-1f1fe\">" },
{":flag-mz:","<sprite name=\"1f1f2-1f1ff\">" },
{":flag-na:","<sprite name=\"1f1f3-1f1e6\">" },
{":flag-nc:","<sprite name=\"1f1f3-1f1e8\">" },
{":flag-ne:","<sprite name=\"1f1f3-1f1ea\">" },
{":flag-nf:","<sprite name=\"1f1f3-1f1eb\">" },
{":flag-ng:","<sprite name=\"1f1f3-1f1ec\">" },
{":flag-ni:","<sprite name=\"1f1f3-1f1ee\">" },
{":flag-nl:","<sprite name=\"1f1f3-1f1f1\">" },
{":flag-no:","<sprite name=\"1f1f3-1f1f4\">" },
{":flag-np:","<sprite name=\"1f1f3-1f1f5\">" },
{":flag-nr:","<sprite name=\"1f1f3-1f1f7\">" },
{":flag-nu:","<sprite name=\"1f1f3-1f1fa\">" },
{":flag-nz:","<sprite name=\"1f1f3-1f1ff\">" },
{":flag-om:","<sprite name=\"1f1f4-1f1f2\">" },
{":flag-pa:","<sprite name=\"1f1f5-1f1e6\">" },
{":flag-pe:","<sprite name=\"1f1f5-1f1ea\">" },
{":flag-pf:","<sprite name=\"1f1f5-1f1eb\">" },
{":flag-pg:","<sprite name=\"1f1f5-1f1ec\">" },
{":flag-ph:","<sprite name=\"1f1f5-1f1ed\">" },
{":flag-pk:","<sprite name=\"1f1f5-1f1f0\">" },
{":flag-pl:","<sprite name=\"1f1f5-1f1f1\">" },
{":flag-pm:","<sprite name=\"1f1f5-1f1f2\">" },
{":flag-pn:","<sprite name=\"1f1f5-1f1f3\">" },
{":flag-pr:","<sprite name=\"1f1f5-1f1f7\">" },
{":flag-ps:","<sprite name=\"1f1f5-1f1f8\">" },
{":flag-pt:","<sprite name=\"1f1f5-1f1f9\">" },
{":flag-pw:","<sprite name=\"1f1f5-1f1fc\">" },
{":flag-py:","<sprite name=\"1f1f5-1f1fe\">" },
{":flag-qa:","<sprite name=\"1f1f6-1f1e6\">" },
{":flag-re:","<sprite name=\"1f1f7-1f1ea\">" },
{":flag-ro:","<sprite name=\"1f1f7-1f1f4\">" },
{":flag-rs:","<sprite name=\"1f1f7-1f1f8\">" },
{":ru:","<sprite name=\"1f1f7-1f1fa\">" },
{":flag-rw:","<sprite name=\"1f1f7-1f1fc\">" },
{":flag-sa:","<sprite name=\"1f1f8-1f1e6\">" },
{":flag-sb:","<sprite name=\"1f1f8-1f1e7\">" },
{":flag-sc:","<sprite name=\"1f1f8-1f1e8\">" },
{":flag-sd:","<sprite name=\"1f1f8-1f1e9\">" },
{":flag-se:","<sprite name=\"1f1f8-1f1ea\">" },
{":flag-sg:","<sprite name=\"1f1f8-1f1ec\">" },
{":flag-sh:","<sprite name=\"1f1f8-1f1ed\">" },
{":flag-si:","<sprite name=\"1f1f8-1f1ee\">" },
{":flag-sj:","<sprite name=\"1f1f8-1f1ef\">" },
{":flag-sk:","<sprite name=\"1f1f8-1f1f0\">" },
{":flag-sl:","<sprite name=\"1f1f8-1f1f1\">" },
{":flag-sm:","<sprite name=\"1f1f8-1f1f2\">" },
{":flag-sn:","<sprite name=\"1f1f8-1f1f3\">" },
{":flag-so:","<sprite name=\"1f1f8-1f1f4\">" },
{":flag-sr:","<sprite name=\"1f1f8-1f1f7\">" },
{":flag-ss:","<sprite name=\"1f1f8-1f1f8\">" },
{":flag-st:","<sprite name=\"1f1f8-1f1f9\">" },
{":flag-sv:","<sprite name=\"1f1f8-1f1fb\">" },
{":flag-sx:","<sprite name=\"1f1f8-1f1fd\">" },
{":flag-sy:","<sprite name=\"1f1f8-1f1fe\">" },
{":flag-sz:","<sprite name=\"1f1f8-1f1ff\">" },
{":flag-ta:","<sprite name=\"1f1f9-1f1e6\">" },
{":flag-tc:","<sprite name=\"1f1f9-1f1e8\">" },
{":flag-td:","<sprite name=\"1f1f9-1f1e9\">" },
{":flag-tf:","<sprite name=\"1f1f9-1f1eb\">" },
{":flag-tg:","<sprite name=\"1f1f9-1f1ec\">" },
{":flag-th:","<sprite name=\"1f1f9-1f1ed\">" },
{":flag-tj:","<sprite name=\"1f1f9-1f1ef\">" },
{":flag-tk:","<sprite name=\"1f1f9-1f1f0\">" },
{":flag-tl:","<sprite name=\"1f1f9-1f1f1\">" },
{":flag-tm:","<sprite name=\"1f1f9-1f1f2\">" },
{":flag-tn:","<sprite name=\"1f1f9-1f1f3\">" },
{":flag-to:","<sprite name=\"1f1f9-1f1f4\">" },
{":flag-tr:","<sprite name=\"1f1f9-1f1f7\">" },
{":flag-tt:","<sprite name=\"1f1f9-1f1f9\">" },
{":flag-tv:","<sprite name=\"1f1f9-1f1fb\">" },
{":flag-tw:","<sprite name=\"1f1f9-1f1fc\">" },
{":flag-tz:","<sprite name=\"1f1f9-1f1ff\">" },
{":flag-ua:","<sprite name=\"1f1fa-1f1e6\">" },
{":flag-ug:","<sprite name=\"1f1fa-1f1ec\">" },
{":flag-um:","<sprite name=\"1f1fa-1f1f2\">" },
{":flag-un:","<sprite name=\"1f1fa-1f1f3\">" },
{":us:","<sprite name=\"1f1fa-1f1f8\">" },
{":flag-uy:","<sprite name=\"1f1fa-1f1fe\">" },
{":flag-uz:","<sprite name=\"1f1fa-1f1ff\">" },
{":flag-va:","<sprite name=\"1f1fb-1f1e6\">" },
{":flag-vc:","<sprite name=\"1f1fb-1f1e8\">" },
{":flag-ve:","<sprite name=\"1f1fb-1f1ea\">" },
{":flag-vg:","<sprite name=\"1f1fb-1f1ec\">" },
{":flag-vi:","<sprite name=\"1f1fb-1f1ee\">" },
{":flag-vn:","<sprite name=\"1f1fb-1f1f3\">" },
{":flag-vu:","<sprite name=\"1f1fb-1f1fa\">" },
{":flag-wf:","<sprite name=\"1f1fc-1f1eb\">" },
{":flag-ws:","<sprite name=\"1f1fc-1f1f8\">" },
{":flag-xk:","<sprite name=\"1f1fd-1f1f0\">" },
{":flag-ye:","<sprite name=\"1f1fe-1f1ea\">" },
{":flag-yt:","<sprite name=\"1f1fe-1f1f9\">" },
{":flag-za:","<sprite name=\"1f1ff-1f1e6\">" },
{":flag-zm:","<sprite name=\"1f1ff-1f1f2\">" },
{":flag-zw:","<sprite name=\"1f1ff-1f1fc\">" },
{":koko:","<sprite name=\"1f201\">" },
{":sa:","<sprite name=\"1f202-fe0f\">" },
{":u7121:","<sprite name=\"1f21a\">" },
{":u6307:","<sprite name=\"1f22f\">" },
{":u7981:","<sprite name=\"1f232\">" },
{":u7a7a:","<sprite name=\"1f233\">" },
{":u5408:","<sprite name=\"1f234\">" },
{":u6e80:","<sprite name=\"1f235\">" },
{":u6709:","<sprite name=\"1f236\">" },
{":u6708:","<sprite name=\"1f237-fe0f\">" },
{":u7533:","<sprite name=\"1f238\">" },
{":u5272:","<sprite name=\"1f239\">" },
{":u55b6:","<sprite name=\"1f23a\">" },
{":ideograph_advantage:","<sprite name=\"1f250\">" },
{":accept:","<sprite name=\"1f251\">" },
{":cyclone:","<sprite name=\"1f300\">" },
{":foggy:","<sprite name=\"1f301\">" },
{":closed_umbrella:","<sprite name=\"1f302\">" },
{":night_with_stars:","<sprite name=\"1f303\">" },
{":sunrise_over_mountains:","<sprite name=\"1f304\">" },
{":sunrise:","<sprite name=\"1f305\">" },
{":city_sunset:","<sprite name=\"1f306\">" },
{":city_sunrise:","<sprite name=\"1f307\">" },
{":rainbow:","<sprite name=\"1f308\">" },
{":bridge_at_night:","<sprite name=\"1f309\">" },
{":ocean:","<sprite name=\"1f30a\">" },
{":volcano:","<sprite name=\"1f30b\">" },
{":milky_way:","<sprite name=\"1f30c\">" },
{":earth_africa:","<sprite name=\"1f30d\">" },
{":earth_americas:","<sprite name=\"1f30e\">" },
{":earth_asia:","<sprite name=\"1f30f\">" },
{":globe_with_meridians:","<sprite name=\"1f310\">" },
{":new_moon:","<sprite name=\"1f311\">" },
{":waxing_crescent_moon:","<sprite name=\"1f312\">" },
{":first_quarter_moon:","<sprite name=\"1f313\">" },
{":moon:","<sprite name=\"1f314\">" },
{":full_moon:","<sprite name=\"1f315\">" },
{":waning_gibbous_moon:","<sprite name=\"1f316\">" },
{":last_quarter_moon:","<sprite name=\"1f317\">" },
{":waning_crescent_moon:","<sprite name=\"1f318\">" },
{":crescent_moon:","<sprite name=\"1f319\">" },
{":new_moon_with_face:","<sprite name=\"1f31a\">" },
{":first_quarter_moon_with_face:","<sprite name=\"1f31b\">" },
{":last_quarter_moon_with_face:","<sprite name=\"1f31c\">" },
{":full_moon_with_face:","<sprite name=\"1f31d\">" },
{":sun_with_face:","<sprite name=\"1f31e\">" },
{":star2:","<sprite name=\"1f31f\">" },
{":stars:","<sprite name=\"1f320\">" },
{":thermometer:","<sprite name=\"1f321-fe0f\">" },
{":mostly_sunny:","<sprite name=\"1f324-fe0f\">" },
{":barely_sunny:","<sprite name=\"1f325-fe0f\">" },
{":partly_sunny_rain:","<sprite name=\"1f326-fe0f\">" },
{":rain_cloud:","<sprite name=\"1f327-fe0f\">" },
{":snow_cloud:","<sprite name=\"1f328-fe0f\">" },
{":lightning:","<sprite name=\"1f329-fe0f\">" },
{":tornado:","<sprite name=\"1f32a-fe0f\">" },
{":fog:","<sprite name=\"1f32b-fe0f\">" },
{":wind_blowing_face:","<sprite name=\"1f32c-fe0f\">" },
{":hotdog:","<sprite name=\"1f32d\">" },
{":taco:","<sprite name=\"1f32e\">" },
{":burrito:","<sprite name=\"1f32f\">" },
{":chestnut:","<sprite name=\"1f330\">" },
{":seedling:","<sprite name=\"1f331\">" },
{":evergreen_tree:","<sprite name=\"1f332\">" },
{":deciduous_tree:","<sprite name=\"1f333\">" },
{":palm_tree:","<sprite name=\"1f334\">" },
{":cactus:","<sprite name=\"1f335\">" },
{":hot_pepper:","<sprite name=\"1f336-fe0f\">" },
{":tulip:","<sprite name=\"1f337\">" },
{":cherry_blossom:","<sprite name=\"1f338\">" },
{":rose:","<sprite name=\"1f339\">" },
{":hibiscus:","<sprite name=\"1f33a\">" },
{":sunflower:","<sprite name=\"1f33b\">" },
{":blossom:","<sprite name=\"1f33c\">" },
{":corn:","<sprite name=\"1f33d\">" },
{":ear_of_rice:","<sprite name=\"1f33e\">" },
{":herb:","<sprite name=\"1f33f\">" },
{":four_leaf_clover:","<sprite name=\"1f340\">" },
{":maple_leaf:","<sprite name=\"1f341\">" },
{":fallen_leaf:","<sprite name=\"1f342\">" },
{":leaves:","<sprite name=\"1f343\">" },
{":mushroom:","<sprite name=\"1f344\">" },
{":tomato:","<sprite name=\"1f345\">" },
{":eggplant:","<sprite name=\"1f346\">" },
{":grapes:","<sprite name=\"1f347\">" },
{":melon:","<sprite name=\"1f348\">" },
{":watermelon:","<sprite name=\"1f349\">" },
{":tangerine:","<sprite name=\"1f34a\">" },
{":lemon:","<sprite name=\"1f34b\">" },
{":banana:","<sprite name=\"1f34c\">" },
{":pineapple:","<sprite name=\"1f34d\">" },
{":apple:","<sprite name=\"1f34e\">" },
{":green_apple:","<sprite name=\"1f34f\">" },
{":pear:","<sprite name=\"1f350\">" },
{":peach:","<sprite name=\"1f351\">" },
{":cherries:","<sprite name=\"1f352\">" },
{":strawberry:","<sprite name=\"1f353\">" },
{":hamburger:","<sprite name=\"1f354\">" },
{":pizza:","<sprite name=\"1f355\">" },
{":meat_on_bone:","<sprite name=\"1f356\">" },
{":poultry_leg:","<sprite name=\"1f357\">" },
{":rice_cracker:","<sprite name=\"1f358\">" },
{":rice_ball:","<sprite name=\"1f359\">" },
{":rice:","<sprite name=\"1f35a\">" },
{":curry:","<sprite name=\"1f35b\">" },
{":ramen:","<sprite name=\"1f35c\">" },
{":spaghetti:","<sprite name=\"1f35d\">" },
{":bread:","<sprite name=\"1f35e\">" },
{":fries:","<sprite name=\"1f35f\">" },
{":sweet_potato:","<sprite name=\"1f360\">" },
{":dango:","<sprite name=\"1f361\">" },
{":oden:","<sprite name=\"1f362\">" },
{":sushi:","<sprite name=\"1f363\">" },
{":fried_shrimp:","<sprite name=\"1f364\">" },
{":fish_cake:","<sprite name=\"1f365\">" },
{":icecream:","<sprite name=\"1f366\">" },
{":shaved_ice:","<sprite name=\"1f367\">" },
{":ice_cream:","<sprite name=\"1f368\">" },
{":doughnut:","<sprite name=\"1f369\">" },
{":cookie:","<sprite name=\"1f36a\">" },
{":chocolate_bar:","<sprite name=\"1f36b\">" },
{":candy:","<sprite name=\"1f36c\">" },
{":lollipop:","<sprite name=\"1f36d\">" },
{":custard:","<sprite name=\"1f36e\">" },
{":honey_pot:","<sprite name=\"1f36f\">" },
{":cake:","<sprite name=\"1f370\">" },
{":bento:","<sprite name=\"1f371\">" },
{":stew:","<sprite name=\"1f372\">" },
{":fried_egg:","<sprite name=\"1f373\">" },
{":fork_and_knife:","<sprite name=\"1f374\">" },
{":tea:","<sprite name=\"1f375\">" },
{":sake:","<sprite name=\"1f376\">" },
{":wine_glass:","<sprite name=\"1f377\">" },
{":cocktail:","<sprite name=\"1f378\">" },
{":tropical_drink:","<sprite name=\"1f379\">" },
{":beer:","<sprite name=\"1f37a\">" },
{":beers:","<sprite name=\"1f37b\">" },
{":baby_bottle:","<sprite name=\"1f37c\">" },
{":knife_fork_plate:","<sprite name=\"1f37d-fe0f\">" },
{":champagne:","<sprite name=\"1f37e\">" },
{":popcorn:","<sprite name=\"1f37f\">" },
{":ribbon:","<sprite name=\"1f380\">" },
{":gift:","<sprite name=\"1f381\">" },
{":birthday:","<sprite name=\"1f382\">" },
{":jack_o_lantern:","<sprite name=\"1f383\">" },
{":christmas_tree:","<sprite name=\"1f384\">" },
{":santa:","<sprite name=\"1f385\">" },
{":fireworks:","<sprite name=\"1f386\">" },
{":sparkler:","<sprite name=\"1f387\">" },
{":balloon:","<sprite name=\"1f388\">" },
{":tada:","<sprite name=\"1f389\">" },
{":confetti_ball:","<sprite name=\"1f38a\">" },
{":tanabata_tree:","<sprite name=\"1f38b\">" },
{":crossed_flags:","<sprite name=\"1f38c\">" },
{":bamboo:","<sprite name=\"1f38d\">" },
{":dolls:","<sprite name=\"1f38e\">" },
{":flags:","<sprite name=\"1f38f\">" },
{":wind_chime:","<sprite name=\"1f390\">" },
{":rice_scene:","<sprite name=\"1f391\">" },
{":school_satchel:","<sprite name=\"1f392\">" },
{":mortar_board:","<sprite name=\"1f393\">" },
{":medal:","<sprite name=\"1f396-fe0f\">" },
{":reminder_ribbon:","<sprite name=\"1f397-fe0f\">" },
{":studio_microphone:","<sprite name=\"1f399-fe0f\">" },
{":level_slider:","<sprite name=\"1f39a-fe0f\">" },
{":control_knobs:","<sprite name=\"1f39b-fe0f\">" },
{":film_frames:","<sprite name=\"1f39e-fe0f\">" },
{":admission_tickets:","<sprite name=\"1f39f-fe0f\">" },
{":carousel_horse:","<sprite name=\"1f3a0\">" },
{":ferris_wheel:","<sprite name=\"1f3a1\">" },
{":roller_coaster:","<sprite name=\"1f3a2\">" },
{":fishing_pole_and_fish:","<sprite name=\"1f3a3\">" },
{":microphone:","<sprite name=\"1f3a4\">" },
{":movie_camera:","<sprite name=\"1f3a5\">" },
{":cinema:","<sprite name=\"1f3a6\">" },
{":headphones:","<sprite name=\"1f3a7\">" },
{":art:","<sprite name=\"1f3a8\">" },
{":tophat:","<sprite name=\"1f3a9\">" },
{":circus_tent:","<sprite name=\"1f3aa\">" },
{":ticket:","<sprite name=\"1f3ab\">" },
{":clapper:","<sprite name=\"1f3ac\">" },
{":performing_arts:","<sprite name=\"1f3ad\">" },
{":video_game:","<sprite name=\"1f3ae\">" },
{":dart:","<sprite name=\"1f3af\">" },
{":slot_machine:","<sprite name=\"1f3b0\">" },
{":8ball:","<sprite name=\"1f3b1\">" },
{":game_die:","<sprite name=\"1f3b2\">" },
{":bowling:","<sprite name=\"1f3b3\">" },
{":flower_playing_cards:","<sprite name=\"1f3b4\">" },
{":musical_note:","<sprite name=\"1f3b5\">" },
{":notes:","<sprite name=\"1f3b6\">" },
{":saxophone:","<sprite name=\"1f3b7\">" },
{":guitar:","<sprite name=\"1f3b8\">" },
{":musical_keyboard:","<sprite name=\"1f3b9\">" },
{":trumpet:","<sprite name=\"1f3ba\">" },
{":violin:","<sprite name=\"1f3bb\">" },
{":musical_score:","<sprite name=\"1f3bc\">" },
{":running_shirt_with_sash:","<sprite name=\"1f3bd\">" },
{":tennis:","<sprite name=\"1f3be\">" },
{":ski:","<sprite name=\"1f3bf\">" },
{":basketball:","<sprite name=\"1f3c0\">" },
{":checkered_flag:","<sprite name=\"1f3c1\">" },
{":snowboarder:","<sprite name=\"1f3c2\">" },
{":woman-running:","<sprite name=\"1f3c3-200d-2640-fe0f\">" },
{":man-running:","<sprite name=\"1f3c3-200d-2642-fe0f\">" },
{":runner:","<sprite name=\"1f3c3\">" },
{":woman-surfing:","<sprite name=\"1f3c4-200d-2640-fe0f\">" },
{":man-surfing:","<sprite name=\"1f3c4-200d-2642-fe0f\">" },
{":surfer:","<sprite name=\"1f3c4\">" },
{":sports_medal:","<sprite name=\"1f3c5\">" },
{":trophy:","<sprite name=\"1f3c6\">" },
{":horse_racing:","<sprite name=\"1f3c7\">" },
{":football:","<sprite name=\"1f3c8\">" },
{":rugby_football:","<sprite name=\"1f3c9\">" },
{":woman-swimming:","<sprite name=\"1f3ca-200d-2640-fe0f\">" },
{":man-swimming:","<sprite name=\"1f3ca-200d-2642-fe0f\">" },
{":swimmer:","<sprite name=\"1f3ca\">" },
{":woman-lifting-weights:","<sprite name=\"1f3cb-fe0f-200d-2640-fe0f\">" },
{":man-lifting-weights:","<sprite name=\"1f3cb-fe0f-200d-2642-fe0f\">" },
{":weight_lifter:","<sprite name=\"1f3cb-fe0f\">" },
{":woman-golfing:","<sprite name=\"1f3cc-fe0f-200d-2640-fe0f\">" },
{":man-golfing:","<sprite name=\"1f3cc-fe0f-200d-2642-fe0f\">" },
{":golfer:","<sprite name=\"1f3cc-fe0f\">" },
{":racing_motorcycle:","<sprite name=\"1f3cd-fe0f\">" },
{":racing_car:","<sprite name=\"1f3ce-fe0f\">" },
{":cricket_bat_and_ball:","<sprite name=\"1f3cf\">" },
{":volleyball:","<sprite name=\"1f3d0\">" },
{":field_hockey_stick_and_ball:","<sprite name=\"1f3d1\">" },
{":ice_hockey_stick_and_puck:","<sprite name=\"1f3d2\">" },
{":table_tennis_paddle_and_ball:","<sprite name=\"1f3d3\">" },
{":snow_capped_mountain:","<sprite name=\"1f3d4-fe0f\">" },
{":camping:","<sprite name=\"1f3d5-fe0f\">" },
{":beach_with_umbrella:","<sprite name=\"1f3d6-fe0f\">" },
{":building_construction:","<sprite name=\"1f3d7-fe0f\">" },
{":house_buildings:","<sprite name=\"1f3d8-fe0f\">" },
{":cityscape:","<sprite name=\"1f3d9-fe0f\">" },
{":derelict_house_building:","<sprite name=\"1f3da-fe0f\">" },
{":classical_building:","<sprite name=\"1f3db-fe0f\">" },
{":desert:","<sprite name=\"1f3dc-fe0f\">" },
{":desert_island:","<sprite name=\"1f3dd-fe0f\">" },
{":national_park:","<sprite name=\"1f3de-fe0f\">" },
{":stadium:","<sprite name=\"1f3df-fe0f\">" },
{":house:","<sprite name=\"1f3e0\">" },
{":house_with_garden:","<sprite name=\"1f3e1\">" },
{":office:","<sprite name=\"1f3e2\">" },
{":post_office:","<sprite name=\"1f3e3\">" },
{":european_post_office:","<sprite name=\"1f3e4\">" },
{":hospital:","<sprite name=\"1f3e5\">" },
{":bank:","<sprite name=\"1f3e6\">" },
{":atm:","<sprite name=\"1f3e7\">" },
{":hotel:","<sprite name=\"1f3e8\">" },
{":love_hotel:","<sprite name=\"1f3e9\">" },
{":convenience_store:","<sprite name=\"1f3ea\">" },
{":school:","<sprite name=\"1f3eb\">" },
{":department_store:","<sprite name=\"1f3ec\">" },
{":factory:","<sprite name=\"1f3ed\">" },
{":izakaya_lantern:","<sprite name=\"1f3ee\">" },
{":japanese_castle:","<sprite name=\"1f3ef\">" },
{":european_castle:","<sprite name=\"1f3f0\">" },
{":rainbow-flag:","<sprite name=\"1f3f3-fe0f-200d-1f308\">" },
{":waving_white_flag:","<sprite name=\"1f3f3-fe0f\">" },
{":flag-england:","<sprite name=\"1f3f4-e0067-e0062-e0065-e006e-e0067-e007f\">" },
{":flag-scotland:","<sprite name=\"1f3f4-e0067-e0062-e0073-e0063-e0074-e007f\">" },
{":flag-wales:","<sprite name=\"1f3f4-e0067-e0062-e0077-e006c-e0073-e007f\">" },
{":waving_black_flag:","<sprite name=\"1f3f4\">" },
{":rosette:","<sprite name=\"1f3f5-fe0f\">" },
{":label:","<sprite name=\"1f3f7-fe0f\">" },
{":badminton_racquet_and_shuttlecock:","<sprite name=\"1f3f8\">" },
{":bow_and_arrow:","<sprite name=\"1f3f9\">" },
{":amphora:","<sprite name=\"1f3fa\">" },
{":skin-tone-2:","<sprite name=\"1f3fb\">" },
{":skin-tone-3:","<sprite name=\"1f3fc\">" },
{":skin-tone-4:","<sprite name=\"1f3fd\">" },
{":skin-tone-5:","<sprite name=\"1f3fe\">" },
{":skin-tone-6:","<sprite name=\"1f3ff\">" },
{":rat:","<sprite name=\"1f400\">" },
{":mouse2:","<sprite name=\"1f401\">" },
{":ox:","<sprite name=\"1f402\">" },
{":water_buffalo:","<sprite name=\"1f403\">" },
{":cow2:","<sprite name=\"1f404\">" },
{":tiger2:","<sprite name=\"1f405\">" },
{":leopard:","<sprite name=\"1f406\">" },
{":rabbit2:","<sprite name=\"1f407\">" },
{":cat2:","<sprite name=\"1f408\">" },
{":dragon:","<sprite name=\"1f409\">" },
{":crocodile:","<sprite name=\"1f40a\">" },
{":whale2:","<sprite name=\"1f40b\">" },
{":snail:","<sprite name=\"1f40c\">" },
{":snake:","<sprite name=\"1f40d\">" },
{":racehorse:","<sprite name=\"1f40e\">" },
{":ram:","<sprite name=\"1f40f\">" },
{":goat:","<sprite name=\"1f410\">" },
{":sheep:","<sprite name=\"1f411\">" },
{":monkey:","<sprite name=\"1f412\">" },
{":rooster:","<sprite name=\"1f413\">" },
{":chicken:","<sprite name=\"1f414\">" },
{":dog2:","<sprite name=\"1f415\">" },
{":pig2:","<sprite name=\"1f416\">" },
{":boar:","<sprite name=\"1f417\">" },
{":elephant:","<sprite name=\"1f418\">" },
{":octopus:","<sprite name=\"1f419\">" },
{":shell:","<sprite name=\"1f41a\">" },
{":bug:","<sprite name=\"1f41b\">" },
{":ant:","<sprite name=\"1f41c\">" },
{":bee:","<sprite name=\"1f41d\">" },
{":beetle:","<sprite name=\"1f41e\">" },
{":fish:","<sprite name=\"1f41f\">" },
{":tropical_fish:","<sprite name=\"1f420\">" },
{":blowfish:","<sprite name=\"1f421\">" },
{":turtle:","<sprite name=\"1f422\">" },
{":hatching_chick:","<sprite name=\"1f423\">" },
{":baby_chick:","<sprite name=\"1f424\">" },
{":hatched_chick:","<sprite name=\"1f425\">" },
{":bird:","<sprite name=\"1f426\">" },
{":penguin:","<sprite name=\"1f427\">" },
{":koala:","<sprite name=\"1f428\">" },
{":poodle:","<sprite name=\"1f429\">" },
{":dromedary_camel:","<sprite name=\"1f42a\">" },
{":camel:","<sprite name=\"1f42b\">" },
{":dolphin:","<sprite name=\"1f42c\">" },
{":mouse:","<sprite name=\"1f42d\">" },
{":cow:","<sprite name=\"1f42e\">" },
{":tiger:","<sprite name=\"1f42f\">" },
{":rabbit:","<sprite name=\"1f430\">" },
{":cat:","<sprite name=\"1f431\">" },
{":dragon_face:","<sprite name=\"1f432\">" },
{":whale:","<sprite name=\"1f433\">" },
{":horse:","<sprite name=\"1f434\">" },
{":monkey_face:","<sprite name=\"1f435\">" },
{":dog:","<sprite name=\"1f436\">" },
{":pig:","<sprite name=\"1f437\">" },
{":frog:","<sprite name=\"1f438\">" },
{":hamster:","<sprite name=\"1f439\">" },
{":wolf:","<sprite name=\"1f43a\">" },
{":bear:","<sprite name=\"1f43b\">" },
{":panda_face:","<sprite name=\"1f43c\">" },
{":pig_nose:","<sprite name=\"1f43d\">" },
{":feet:","<sprite name=\"1f43e\">" },
{":chipmunk:","<sprite name=\"1f43f-fe0f\">" },
{":eyes:","<sprite name=\"1f440\">" },
{":eye-in-speech-bubble:","<sprite name=\"1f441-fe0f-200d-1f5e8-fe0f\">" },
{":eye:","<sprite name=\"1f441-fe0f\">" },
{":ear:","<sprite name=\"1f442\">" },
{":nose:","<sprite name=\"1f443\">" },
{":lips:","<sprite name=\"1f444\">" },
{":tongue:","<sprite name=\"1f445\">" },
{":point_up_2:","<sprite name=\"1f446\">" },
{":point_down:","<sprite name=\"1f447\">" },
{":point_left:","<sprite name=\"1f448\">" },
{":point_right:","<sprite name=\"1f449\">" },
{":facepunch:","<sprite name=\"1f44a\">" },
{":wave:","<sprite name=\"1f44b\">" },
{":ok_hand:","<sprite name=\"1f44c\">" },
{":+1:","<sprite name=\"1f44d\">" },
{":-1:","<sprite name=\"1f44e\">" },
{":clap:","<sprite name=\"1f44f\">" },
{":open_hands:","<sprite name=\"1f450\">" },
{":crown:","<sprite name=\"1f451\">" },
{":womans_hat:","<sprite name=\"1f452\">" },
{":eyeglasses:","<sprite name=\"1f453\">" },
{":necktie:","<sprite name=\"1f454\">" },
{":shirt:","<sprite name=\"1f455\">" },
{":jeans:","<sprite name=\"1f456\">" },
{":dress:","<sprite name=\"1f457\">" },
{":kimono:","<sprite name=\"1f458\">" },
{":bikini:","<sprite name=\"1f459\">" },
{":womans_clothes:","<sprite name=\"1f45a\">" },
{":purse:","<sprite name=\"1f45b\">" },
{":handbag:","<sprite name=\"1f45c\">" },
{":pouch:","<sprite name=\"1f45d\">" },
{":mans_shoe:","<sprite name=\"1f45e\">" },
{":athletic_shoe:","<sprite name=\"1f45f\">" },
{":high_heel:","<sprite name=\"1f460\">" },
{":sandal:","<sprite name=\"1f461\">" },
{":boot:","<sprite name=\"1f462\">" },
{":footprints:","<sprite name=\"1f463\">" },
{":bust_in_silhouette:","<sprite name=\"1f464\">" },
{":busts_in_silhouette:","<sprite name=\"1f465\">" },
{":boy:","<sprite name=\"1f466\">" },
{":girl:","<sprite name=\"1f467\">" },
{":male-farmer:","<sprite name=\"1f468-200d-1f33e\">" },
{":male-cook:","<sprite name=\"1f468-200d-1f373\">" },
{":male-student:","<sprite name=\"1f468-200d-1f393\">" },
{":male-singer:","<sprite name=\"1f468-200d-1f3a4\">" },
{":male-artist:","<sprite name=\"1f468-200d-1f3a8\">" },
{":male-teacher:","<sprite name=\"1f468-200d-1f3eb\">" },
{":male-factory-worker:","<sprite name=\"1f468-200d-1f3ed\">" },
{":man-boy-boy:","<sprite name=\"1f468-200d-1f466-200d-1f466\">" },
{":man-boy:","<sprite name=\"1f468-200d-1f466\">" },
{":man-girl-boy:","<sprite name=\"1f468-200d-1f467-200d-1f466\">" },
{":man-girl-girl:","<sprite name=\"1f468-200d-1f467-200d-1f467\">" },
{":man-girl:","<sprite name=\"1f468-200d-1f467\">" },
{":man-man-boy:","<sprite name=\"1f468-200d-1f468-200d-1f466\">" },
{":man-man-boy-boy:","<sprite name=\"1f468-200d-1f468-200d-1f466-200d-1f466\">" },
{":man-man-girl:","<sprite name=\"1f468-200d-1f468-200d-1f467\">" },
{":man-man-girl-boy:","<sprite name=\"1f468-200d-1f468-200d-1f467-200d-1f466\">" },
{":man-man-girl-girl:","<sprite name=\"1f468-200d-1f468-200d-1f467-200d-1f467\">" },
{":man-woman-boy:","<sprite name=\"1f468-200d-1f469-200d-1f466\">" },
{":man-woman-boy-boy:","<sprite name=\"1f468-200d-1f469-200d-1f466-200d-1f466\">" },
{":man-woman-girl:","<sprite name=\"1f468-200d-1f469-200d-1f467\">" },
{":man-woman-girl-boy:","<sprite name=\"1f468-200d-1f469-200d-1f467-200d-1f466\">" },
{":man-woman-girl-girl:","<sprite name=\"1f468-200d-1f469-200d-1f467-200d-1f467\">" },
{":male-technologist:","<sprite name=\"1f468-200d-1f4bb\">" },
{":male-office-worker:","<sprite name=\"1f468-200d-1f4bc\">" },
{":male-mechanic:","<sprite name=\"1f468-200d-1f527\">" },
{":male-scientist:","<sprite name=\"1f468-200d-1f52c\">" },
{":male-astronaut:","<sprite name=\"1f468-200d-1f680\">" },
{":male-firefighter:","<sprite name=\"1f468-200d-1f692\">" },
{":male-doctor:","<sprite name=\"1f468-200d-2695-fe0f\">" },
{":male-judge:","<sprite name=\"1f468-200d-2696-fe0f\">" },
{":male-pilot:","<sprite name=\"1f468-200d-2708-fe0f\">" },
{":man-heart-man:","<sprite name=\"1f468-200d-2764-fe0f-200d-1f468\">" },
{":man-kiss-man:","<sprite name=\"1f468-200d-2764-fe0f-200d-1f48b-200d-1f468\">" },
{":man:","<sprite name=\"1f468\">" },
{":female-farmer:","<sprite name=\"1f469-200d-1f33e\">" },
{":female-cook:","<sprite name=\"1f469-200d-1f373\">" },
{":female-student:","<sprite name=\"1f469-200d-1f393\">" },
{":female-singer:","<sprite name=\"1f469-200d-1f3a4\">" },
{":female-artist:","<sprite name=\"1f469-200d-1f3a8\">" },
{":female-teacher:","<sprite name=\"1f469-200d-1f3eb\">" },
{":female-factory-worker:","<sprite name=\"1f469-200d-1f3ed\">" },
{":woman-boy-boy:","<sprite name=\"1f469-200d-1f466-200d-1f466\">" },
{":woman-boy:","<sprite name=\"1f469-200d-1f466\">" },
{":woman-girl-boy:","<sprite name=\"1f469-200d-1f467-200d-1f466\">" },
{":woman-girl-girl:","<sprite name=\"1f469-200d-1f467-200d-1f467\">" },
{":woman-girl:","<sprite name=\"1f469-200d-1f467\">" },
{":woman-woman-boy:","<sprite name=\"1f469-200d-1f469-200d-1f466\">" },
{":woman-woman-boy-boy:","<sprite name=\"1f469-200d-1f469-200d-1f466-200d-1f466\">" },
{":woman-woman-girl:","<sprite name=\"1f469-200d-1f469-200d-1f467\">" },
{":woman-woman-girl-boy:","<sprite name=\"1f469-200d-1f469-200d-1f467-200d-1f466\">" },
{":woman-woman-girl-girl:","<sprite name=\"1f469-200d-1f469-200d-1f467-200d-1f467\">" },
{":female-technologist:","<sprite name=\"1f469-200d-1f4bb\">" },
{":female-office-worker:","<sprite name=\"1f469-200d-1f4bc\">" },
{":female-mechanic:","<sprite name=\"1f469-200d-1f527\">" },
{":female-scientist:","<sprite name=\"1f469-200d-1f52c\">" },
{":female-astronaut:","<sprite name=\"1f469-200d-1f680\">" },
{":female-firefighter:","<sprite name=\"1f469-200d-1f692\">" },
{":female-doctor:","<sprite name=\"1f469-200d-2695-fe0f\">" },
{":female-judge:","<sprite name=\"1f469-200d-2696-fe0f\">" },
{":female-pilot:","<sprite name=\"1f469-200d-2708-fe0f\">" },
{":woman-heart-man:","<sprite name=\"1f469-200d-2764-fe0f-200d-1f468\">" },
{":woman-heart-woman:","<sprite name=\"1f469-200d-2764-fe0f-200d-1f469\">" },
{":woman-kiss-man:","<sprite name=\"1f469-200d-2764-fe0f-200d-1f48b-200d-1f468\">" },
{":woman-kiss-woman:","<sprite name=\"1f469-200d-2764-fe0f-200d-1f48b-200d-1f469\">" },
{":woman:","<sprite name=\"1f469\">" },
{":family:","<sprite name=\"1f46a\">" },
{":couple:","<sprite name=\"1f46b\">" },
{":two_men_holding_hands:","<sprite name=\"1f46c\">" },
{":two_women_holding_hands:","<sprite name=\"1f46d\">" },
{":female-police-officer:","<sprite name=\"1f46e-200d-2640-fe0f\">" },
{":male-police-officer:","<sprite name=\"1f46e-200d-2642-fe0f\">" },
{":cop:","<sprite name=\"1f46e\">" },
{":woman-with-bunny-ears-partying:","<sprite name=\"1f46f-200d-2640-fe0f\">" },
{":man-with-bunny-ears-partying:","<sprite name=\"1f46f-200d-2642-fe0f\">" },
{":dancers:","<sprite name=\"1f46f\">" },
{":bride_with_veil:","<sprite name=\"1f470\">" },
{":blond-haired-woman:","<sprite name=\"1f471-200d-2640-fe0f\">" },
{":blond-haired-man:","<sprite name=\"1f471-200d-2642-fe0f\">" },
{":person_with_blond_hair:","<sprite name=\"1f471\">" },
{":man_with_gua_pi_mao:","<sprite name=\"1f472\">" },
{":woman-wearing-turban:","<sprite name=\"1f473-200d-2640-fe0f\">" },
{":man-wearing-turban:","<sprite name=\"1f473-200d-2642-fe0f\">" },
{":man_with_turban:","<sprite name=\"1f473\">" },
{":older_man:","<sprite name=\"1f474\">" },
{":older_woman:","<sprite name=\"1f475\">" },
{":baby:","<sprite name=\"1f476\">" },
{":female-construction-worker:","<sprite name=\"1f477-200d-2640-fe0f\">" },
{":male-construction-worker:","<sprite name=\"1f477-200d-2642-fe0f\">" },
{":construction_worker:","<sprite name=\"1f477\">" },
{":princess:","<sprite name=\"1f478\">" },
{":japanese_ogre:","<sprite name=\"1f479\">" },
{":japanese_goblin:","<sprite name=\"1f47a\">" },
{":ghost:","<sprite name=\"1f47b\">" },
{":angel:","<sprite name=\"1f47c\">" },
{":alien:","<sprite name=\"1f47d\">" },
{":space_invader:","<sprite name=\"1f47e\">" },
{":imp:","<sprite name=\"1f47f\">" },
{":skull:","<sprite name=\"1f480\">" },
{":woman-tipping-hand:","<sprite name=\"1f481-200d-2640-fe0f\">" },
{":man-tipping-hand:","<sprite name=\"1f481-200d-2642-fe0f\">" },
{":information_desk_person:","<sprite name=\"1f481\">" },
{":female-guard:","<sprite name=\"1f482-200d-2640-fe0f\">" },
{":male-guard:","<sprite name=\"1f482-200d-2642-fe0f\">" },
{":guardsman:","<sprite name=\"1f482\">" },
{":dancer:","<sprite name=\"1f483\">" },
{":lipstick:","<sprite name=\"1f484\">" },
{":nail_care:","<sprite name=\"1f485\">" },
{":woman-getting-massage:","<sprite name=\"1f486-200d-2640-fe0f\">" },
{":man-getting-massage:","<sprite name=\"1f486-200d-2642-fe0f\">" },
{":massage:","<sprite name=\"1f486\">" },
{":woman-getting-haircut:","<sprite name=\"1f487-200d-2640-fe0f\">" },
{":man-getting-haircut:","<sprite name=\"1f487-200d-2642-fe0f\">" },
{":haircut:","<sprite name=\"1f487\">" },
{":barber:","<sprite name=\"1f488\">" },
{":syringe:","<sprite name=\"1f489\">" },
{":pill:","<sprite name=\"1f48a\">" },
{":kiss:","<sprite name=\"1f48b\">" },
{":love_letter:","<sprite name=\"1f48c\">" },
{":ring:","<sprite name=\"1f48d\">" },
{":gem:","<sprite name=\"1f48e\">" },
{":couplekiss:","<sprite name=\"1f48f\">" },
{":bouquet:","<sprite name=\"1f490\">" },
{":couple_with_heart:","<sprite name=\"1f491\">" },
{":wedding:","<sprite name=\"1f492\">" },
{":heartbeat:","<sprite name=\"1f493\">" },
{":broken_heart:","<sprite name=\"1f494\">" },
{":two_hearts:","<sprite name=\"1f495\">" },
{":sparkling_heart:","<sprite name=\"1f496\">" },
{":heartpulse:","<sprite name=\"1f497\">" },
{":cupid:","<sprite name=\"1f498\">" },
{":blue_heart:","<sprite name=\"1f499\">" },
{":green_heart:","<sprite name=\"1f49a\">" },
{":yellow_heart:","<sprite name=\"1f49b\">" },
{":purple_heart:","<sprite name=\"1f49c\">" },
{":gift_heart:","<sprite name=\"1f49d\">" },
{":revolving_hearts:","<sprite name=\"1f49e\">" },
{":heart_decoration:","<sprite name=\"1f49f\">" },
{":diamond_shape_with_a_dot_inside:","<sprite name=\"1f4a0\">" },
{":bulb:","<sprite name=\"1f4a1\">" },
{":anger:","<sprite name=\"1f4a2\">" },
{":bomb:","<sprite name=\"1f4a3\">" },
{":zzz:","<sprite name=\"1f4a4\">" },
{":boom:","<sprite name=\"1f4a5\">" },
{":sweat_drops:","<sprite name=\"1f4a6\">" },
{":droplet:","<sprite name=\"1f4a7\">" },
{":dash:","<sprite name=\"1f4a8\">" },
{":hankey:","<sprite name=\"1f4a9\">" },
{":muscle:","<sprite name=\"1f4aa\">" },
{":dizzy:","<sprite name=\"1f4ab\">" },
{":speech_balloon:","<sprite name=\"1f4ac\">" },
{":thought_balloon:","<sprite name=\"1f4ad\">" },
{":white_flower:","<sprite name=\"1f4ae\">" },
{":100:","<sprite name=\"1f4af\">" },
{":moneybag:","<sprite name=\"1f4b0\">" },
{":currency_exchange:","<sprite name=\"1f4b1\">" },
{":heavy_dollar_sign:","<sprite name=\"1f4b2\">" },
{":credit_card:","<sprite name=\"1f4b3\">" },
{":yen:","<sprite name=\"1f4b4\">" },
{":dollar:","<sprite name=\"1f4b5\">" },
{":euro:","<sprite name=\"1f4b6\">" },
{":pound:","<sprite name=\"1f4b7\">" },
{":money_with_wings:","<sprite name=\"1f4b8\">" },
{":chart:","<sprite name=\"1f4b9\">" },
{":seat:","<sprite name=\"1f4ba\">" },
{":computer:","<sprite name=\"1f4bb\">" },
{":briefcase:","<sprite name=\"1f4bc\">" },
{":minidisc:","<sprite name=\"1f4bd\">" },
{":floppy_disk:","<sprite name=\"1f4be\">" },
{":cd:","<sprite name=\"1f4bf\">" },
{":dvd:","<sprite name=\"1f4c0\">" },
{":file_folder:","<sprite name=\"1f4c1\">" },
{":open_file_folder:","<sprite name=\"1f4c2\">" },
{":page_with_curl:","<sprite name=\"1f4c3\">" },
{":page_facing_up:","<sprite name=\"1f4c4\">" },
{":date:","<sprite name=\"1f4c5\">" },
{":calendar:","<sprite name=\"1f4c6\">" },
{":card_index:","<sprite name=\"1f4c7\">" },
{":chart_with_upwards_trend:","<sprite name=\"1f4c8\">" },
{":chart_with_downwards_trend:","<sprite name=\"1f4c9\">" },
{":bar_chart:","<sprite name=\"1f4ca\">" },
{":clipboard:","<sprite name=\"1f4cb\">" },
{":pushpin:","<sprite name=\"1f4cc\">" },
{":round_pushpin:","<sprite name=\"1f4cd\">" },
{":paperclip:","<sprite name=\"1f4ce\">" },
{":straight_ruler:","<sprite name=\"1f4cf\">" },
{":triangular_ruler:","<sprite name=\"1f4d0\">" },
{":bookmark_tabs:","<sprite name=\"1f4d1\">" },
{":ledger:","<sprite name=\"1f4d2\">" },
{":notebook:","<sprite name=\"1f4d3\">" },
{":notebook_with_decorative_cover:","<sprite name=\"1f4d4\">" },
{":closed_book:","<sprite name=\"1f4d5\">" },
{":book:","<sprite name=\"1f4d6\">" },
{":green_book:","<sprite name=\"1f4d7\">" },
{":blue_book:","<sprite name=\"1f4d8\">" },
{":orange_book:","<sprite name=\"1f4d9\">" },
{":books:","<sprite name=\"1f4da\">" },
{":name_badge:","<sprite name=\"1f4db\">" },
{":scroll:","<sprite name=\"1f4dc\">" },
{":memo:","<sprite name=\"1f4dd\">" },
{":telephone_receiver:","<sprite name=\"1f4de\">" },
{":pager:","<sprite name=\"1f4df\">" },
{":fax:","<sprite name=\"1f4e0\">" },
{":satellite_antenna:","<sprite name=\"1f4e1\">" },
{":loudspeaker:","<sprite name=\"1f4e2\">" },
{":mega:","<sprite name=\"1f4e3\">" },
{":outbox_tray:","<sprite name=\"1f4e4\">" },
{":inbox_tray:","<sprite name=\"1f4e5\">" },
{":package:","<sprite name=\"1f4e6\">" },
{":e-mail:","<sprite name=\"1f4e7\">" },
{":incoming_envelope:","<sprite name=\"1f4e8\">" },
{":envelope_with_arrow:","<sprite name=\"1f4e9\">" },
{":mailbox_closed:","<sprite name=\"1f4ea\">" },
{":mailbox:","<sprite name=\"1f4eb\">" },
{":mailbox_with_mail:","<sprite name=\"1f4ec\">" },
{":mailbox_with_no_mail:","<sprite name=\"1f4ed\">" },
{":postbox:","<sprite name=\"1f4ee\">" },
{":postal_horn:","<sprite name=\"1f4ef\">" },
{":newspaper:","<sprite name=\"1f4f0\">" },
{":iphone:","<sprite name=\"1f4f1\">" },
{":calling:","<sprite name=\"1f4f2\">" },
{":vibration_mode:","<sprite name=\"1f4f3\">" },
{":mobile_phone_off:","<sprite name=\"1f4f4\">" },
{":no_mobile_phones:","<sprite name=\"1f4f5\">" },
{":signal_strength:","<sprite name=\"1f4f6\">" },
{":camera:","<sprite name=\"1f4f7\">" },
{":camera_with_flash:","<sprite name=\"1f4f8\">" },
{":video_camera:","<sprite name=\"1f4f9\">" },
{":tv:","<sprite name=\"1f4fa\">" },
{":radio:","<sprite name=\"1f4fb\">" },
{":vhs:","<sprite name=\"1f4fc\">" },
{":film_projector:","<sprite name=\"1f4fd-fe0f\">" },
{":prayer_beads:","<sprite name=\"1f4ff\">" },
{":twisted_rightwards_arrows:","<sprite name=\"1f500\">" },
{":repeat:","<sprite name=\"1f501\">" },
{":repeat_one:","<sprite name=\"1f502\">" },
{":arrows_clockwise:","<sprite name=\"1f503\">" },
{":arrows_counterclockwise:","<sprite name=\"1f504\">" },
{":low_brightness:","<sprite name=\"1f505\">" },
{":high_brightness:","<sprite name=\"1f506\">" },
{":mute:","<sprite name=\"1f507\">" },
{":speaker:","<sprite name=\"1f508\">" },
{":sound:","<sprite name=\"1f509\">" },
{":loud_sound:","<sprite name=\"1f50a\">" },
{":battery:","<sprite name=\"1f50b\">" },
{":electric_plug:","<sprite name=\"1f50c\">" },
{":mag:","<sprite name=\"1f50d\">" },
{":mag_right:","<sprite name=\"1f50e\">" },
{":lock_with_ink_pen:","<sprite name=\"1f50f\">" },
{":closed_lock_with_key:","<sprite name=\"1f510\">" },
{":key:","<sprite name=\"1f511\">" },
{":lock:","<sprite name=\"1f512\">" },
{":unlock:","<sprite name=\"1f513\">" },
{":bell:","<sprite name=\"1f514\">" },
{":no_bell:","<sprite name=\"1f515\">" },
{":bookmark:","<sprite name=\"1f516\">" },
{":link:","<sprite name=\"1f517\">" },
{":radio_button:","<sprite name=\"1f518\">" },
{":back:","<sprite name=\"1f519\">" },
{":end:","<sprite name=\"1f51a\">" },
{":on:","<sprite name=\"1f51b\">" },
{":soon:","<sprite name=\"1f51c\">" },
{":top:","<sprite name=\"1f51d\">" },
{":underage:","<sprite name=\"1f51e\">" },
{":keycap_ten:","<sprite name=\"1f51f\">" },
{":capital_abcd:","<sprite name=\"1f520\">" },
{":abcd:","<sprite name=\"1f521\">" },
{":1234:","<sprite name=\"1f522\">" },
{":symbols:","<sprite name=\"1f523\">" },
{":abc:","<sprite name=\"1f524\">" },
{":fire:","<sprite name=\"1f525\">" },
{":flashlight:","<sprite name=\"1f526\">" },
{":wrench:","<sprite name=\"1f527\">" },
{":hammer:","<sprite name=\"1f528\">" },
{":nut_and_bolt:","<sprite name=\"1f529\">" },
{":hocho:","<sprite name=\"1f52a\">" },
{":gun:","<sprite name=\"1f52b\">" },
{":microscope:","<sprite name=\"1f52c\">" },
{":telescope:","<sprite name=\"1f52d\">" },
{":crystal_ball:","<sprite name=\"1f52e\">" },
{":six_pointed_star:","<sprite name=\"1f52f\">" },
{":beginner:","<sprite name=\"1f530\">" },
{":trident:","<sprite name=\"1f531\">" },
{":black_square_button:","<sprite name=\"1f532\">" },
{":white_square_button:","<sprite name=\"1f533\">" },
{":red_circle:","<sprite name=\"1f534\">" },
{":large_blue_circle:","<sprite name=\"1f535\">" },
{":large_orange_diamond:","<sprite name=\"1f536\">" },
{":large_blue_diamond:","<sprite name=\"1f537\">" },
{":small_orange_diamond:","<sprite name=\"1f538\">" },
{":small_blue_diamond:","<sprite name=\"1f539\">" },
{":small_red_triangle:","<sprite name=\"1f53a\">" },
{":small_red_triangle_down:","<sprite name=\"1f53b\">" },
{":arrow_up_small:","<sprite name=\"1f53c\">" },
{":arrow_down_small:","<sprite name=\"1f53d\">" },
{":om_symbol:","<sprite name=\"1f549-fe0f\">" },
{":dove_of_peace:","<sprite name=\"1f54a-fe0f\">" },
{":kaaba:","<sprite name=\"1f54b\">" },
{":mosque:","<sprite name=\"1f54c\">" },
{":synagogue:","<sprite name=\"1f54d\">" },
{":menorah_with_nine_branches:","<sprite name=\"1f54e\">" },
{":clock1:","<sprite name=\"1f550\">" },
{":clock2:","<sprite name=\"1f551\">" },
{":clock3:","<sprite name=\"1f552\">" },
{":clock4:","<sprite name=\"1f553\">" },
{":clock5:","<sprite name=\"1f554\">" },
{":clock6:","<sprite name=\"1f555\">" },
{":clock7:","<sprite name=\"1f556\">" },
{":clock8:","<sprite name=\"1f557\">" },
{":clock9:","<sprite name=\"1f558\">" },
{":clock10:","<sprite name=\"1f559\">" },
{":clock11:","<sprite name=\"1f55a\">" },
{":clock12:","<sprite name=\"1f55b\">" },
{":clock130:","<sprite name=\"1f55c\">" },
{":clock230:","<sprite name=\"1f55d\">" },
{":clock330:","<sprite name=\"1f55e\">" },
{":clock430:","<sprite name=\"1f55f\">" },
{":clock530:","<sprite name=\"1f560\">" },
{":clock630:","<sprite name=\"1f561\">" },
{":clock730:","<sprite name=\"1f562\">" },
{":clock830:","<sprite name=\"1f563\">" },
{":clock930:","<sprite name=\"1f564\">" },
{":clock1030:","<sprite name=\"1f565\">" },
{":clock1130:","<sprite name=\"1f566\">" },
{":clock1230:","<sprite name=\"1f567\">" },
{":candle:","<sprite name=\"1f56f-fe0f\">" },
{":mantelpiece_clock:","<sprite name=\"1f570-fe0f\">" },
{":hole:","<sprite name=\"1f573-fe0f\">" },
{":man_in_business_suit_levitating:","<sprite name=\"1f574-fe0f\">" },
{":female-detective:","<sprite name=\"1f575-fe0f-200d-2640-fe0f\">" },
{":male-detective:","<sprite name=\"1f575-fe0f-200d-2642-fe0f\">" },
{":sleuth_or_spy:","<sprite name=\"1f575-fe0f\">" },
{":dark_sunglasses:","<sprite name=\"1f576-fe0f\">" },
{":spider:","<sprite name=\"1f577-fe0f\">" },
{":spider_web:","<sprite name=\"1f578-fe0f\">" },
{":joystick:","<sprite name=\"1f579-fe0f\">" },
{":man_dancing:","<sprite name=\"1f57a\">" },
{":linked_paperclips:","<sprite name=\"1f587-fe0f\">" },
{":lower_left_ballpoint_pen:","<sprite name=\"1f58a-fe0f\">" },
{":lower_left_fountain_pen:","<sprite name=\"1f58b-fe0f\">" },
{":lower_left_paintbrush:","<sprite name=\"1f58c-fe0f\">" },
{":lower_left_crayon:","<sprite name=\"1f58d-fe0f\">" },
{":raised_hand_with_fingers_splayed:","<sprite name=\"1f590-fe0f\">" },
{":middle_finger:","<sprite name=\"1f595\">" },
{":spock-hand:","<sprite name=\"1f596\">" },
{":black_heart:","<sprite name=\"1f5a4\">" },
{":desktop_computer:","<sprite name=\"1f5a5-fe0f\">" },
{":printer:","<sprite name=\"1f5a8-fe0f\">" },
{":three_button_mouse:","<sprite name=\"1f5b1-fe0f\">" },
{":trackball:","<sprite name=\"1f5b2-fe0f\">" },
{":frame_with_picture:","<sprite name=\"1f5bc-fe0f\">" },
{":card_index_dividers:","<sprite name=\"1f5c2-fe0f\">" },
{":card_file_box:","<sprite name=\"1f5c3-fe0f\">" },
{":file_cabinet:","<sprite name=\"1f5c4-fe0f\">" },
{":wastebasket:","<sprite name=\"1f5d1-fe0f\">" },
{":spiral_note_pad:","<sprite name=\"1f5d2-fe0f\">" },
{":spiral_calendar_pad:","<sprite name=\"1f5d3-fe0f\">" },
{":compression:","<sprite name=\"1f5dc-fe0f\">" },
{":old_key:","<sprite name=\"1f5dd-fe0f\">" },
{":rolled_up_newspaper:","<sprite name=\"1f5de-fe0f\">" },
{":dagger_knife:","<sprite name=\"1f5e1-fe0f\">" },
{":speaking_head_in_silhouette:","<sprite name=\"1f5e3-fe0f\">" },
{":left_speech_bubble:","<sprite name=\"1f5e8-fe0f\">" },
{":right_anger_bubble:","<sprite name=\"1f5ef-fe0f\">" },
{":ballot_box_with_ballot:","<sprite name=\"1f5f3-fe0f\">" },
{":world_map:","<sprite name=\"1f5fa-fe0f\">" },
{":mount_fuji:","<sprite name=\"1f5fb\">" },
{":tokyo_tower:","<sprite name=\"1f5fc\">" },
{":statue_of_liberty:","<sprite name=\"1f5fd\">" },
{":japan:","<sprite name=\"1f5fe\">" },
{":moyai:","<sprite name=\"1f5ff\">" },
{":grinning:","<sprite name=\"1f600\">" },
{":grin:","<sprite name=\"1f601\">" },
{":joy:","<sprite name=\"1f602\">" },
{":smiley:","<sprite name=\"1f603\">" },
{":smile:","<sprite name=\"1f604\">" },
{":sweat_smile:","<sprite name=\"1f605\">" },
{":laughing:","<sprite name=\"1f606\">" },
{":innocent:","<sprite name=\"1f607\">" },
{":smiling_imp:","<sprite name=\"1f608\">" },
{":wink:","<sprite name=\"1f609\">" },
{":blush:","<sprite name=\"1f60a\">" },
{":yum:","<sprite name=\"1f60b\">" },
{":relieved:","<sprite name=\"1f60c\">" },
{":heart_eyes:","<sprite name=\"1f60d\">" },
{":sunglasses:","<sprite name=\"1f60e\">" },
{":smirk:","<sprite name=\"1f60f\">" },
{":neutral_face:","<sprite name=\"1f610\">" },
{":expressionless:","<sprite name=\"1f611\">" },
{":unamused:","<sprite name=\"1f612\">" },
{":sweat:","<sprite name=\"1f613\">" },
{":pensive:","<sprite name=\"1f614\">" },
{":confused:","<sprite name=\"1f615\">" },
{":confounded:","<sprite name=\"1f616\">" },
{":kissing:","<sprite name=\"1f617\">" },
{":kissing_heart:","<sprite name=\"1f618\">" },
{":kissing_smiling_eyes:","<sprite name=\"1f619\">" },
{":kissing_closed_eyes:","<sprite name=\"1f61a\">" },
{":stuck_out_tongue:","<sprite name=\"1f61b\">" },
{":stuck_out_tongue_winking_eye:","<sprite name=\"1f61c\">" },
{":stuck_out_tongue_closed_eyes:","<sprite name=\"1f61d\">" },
{":disappointed:","<sprite name=\"1f61e\">" },
{":worried:","<sprite name=\"1f61f\">" },
{":angry:","<sprite name=\"1f620\">" },
{":rage:","<sprite name=\"1f621\">" },
{":cry:","<sprite name=\"1f622\">" },
{":persevere:","<sprite name=\"1f623\">" },
{":triumph:","<sprite name=\"1f624\">" },
{":disappointed_relieved:","<sprite name=\"1f625\">" },
{":frowning:","<sprite name=\"1f626\">" },
{":anguished:","<sprite name=\"1f627\">" },
{":fearful:","<sprite name=\"1f628\">" },
{":weary:","<sprite name=\"1f629\">" },
{":sleepy:","<sprite name=\"1f62a\">" },
{":tired_face:","<sprite name=\"1f62b\">" },
{":grimacing:","<sprite name=\"1f62c\">" },
{":sob:","<sprite name=\"1f62d\">" },
{":open_mouth:","<sprite name=\"1f62e\">" },
{":hushed:","<sprite name=\"1f62f\">" },
{":cold_sweat:","<sprite name=\"1f630\">" },
{":scream:","<sprite name=\"1f631\">" },
{":astonished:","<sprite name=\"1f632\">" },
{":flushed:","<sprite name=\"1f633\">" },
{":sleeping:","<sprite name=\"1f634\">" },
{":dizzy_face:","<sprite name=\"1f635\">" },
{":no_mouth:","<sprite name=\"1f636\">" },
{":mask:","<sprite name=\"1f637\">" },
{":smile_cat:","<sprite name=\"1f638\">" },
{":joy_cat:","<sprite name=\"1f639\">" },
{":smiley_cat:","<sprite name=\"1f63a\">" },
{":heart_eyes_cat:","<sprite name=\"1f63b\">" },
{":smirk_cat:","<sprite name=\"1f63c\">" },
{":kissing_cat:","<sprite name=\"1f63d\">" },
{":pouting_cat:","<sprite name=\"1f63e\">" },
{":crying_cat_face:","<sprite name=\"1f63f\">" },
{":scream_cat:","<sprite name=\"1f640\">" },
{":slightly_frowning_face:","<sprite name=\"1f641\">" },
{":slightly_smiling_face:","<sprite name=\"1f642\">" },
{":upside_down_face:","<sprite name=\"1f643\">" },
{":face_with_rolling_eyes:","<sprite name=\"1f644\">" },
{":woman-gesturing-no:","<sprite name=\"1f645-200d-2640-fe0f\">" },
{":man-gesturing-no:","<sprite name=\"1f645-200d-2642-fe0f\">" },
{":no_good:","<sprite name=\"1f645\">" },
{":woman-gesturing-ok:","<sprite name=\"1f646-200d-2640-fe0f\">" },
{":man-gesturing-ok:","<sprite name=\"1f646-200d-2642-fe0f\">" },
{":ok_woman:","<sprite name=\"1f646\">" },
{":woman-bowing:","<sprite name=\"1f647-200d-2640-fe0f\">" },
{":man-bowing:","<sprite name=\"1f647-200d-2642-fe0f\">" },
{":bow:","<sprite name=\"1f647\">" },
{":see_no_evil:","<sprite name=\"1f648\">" },
{":hear_no_evil:","<sprite name=\"1f649\">" },
{":speak_no_evil:","<sprite name=\"1f64a\">" },
{":woman-raising-hand:","<sprite name=\"1f64b-200d-2640-fe0f\">" },
{":man-raising-hand:","<sprite name=\"1f64b-200d-2642-fe0f\">" },
{":raising_hand:","<sprite name=\"1f64b\">" },
{":raised_hands:","<sprite name=\"1f64c\">" },
{":woman-frowning:","<sprite name=\"1f64d-200d-2640-fe0f\">" },
{":man-frowning:","<sprite name=\"1f64d-200d-2642-fe0f\">" },
{":person_frowning:","<sprite name=\"1f64d\">" },
{":woman-pouting:","<sprite name=\"1f64e-200d-2640-fe0f\">" },
{":man-pouting:","<sprite name=\"1f64e-200d-2642-fe0f\">" },
{":person_with_pouting_face:","<sprite name=\"1f64e\">" },
{":pray:","<sprite name=\"1f64f\">" },
{":rocket:","<sprite name=\"1f680\">" },
{":helicopter:","<sprite name=\"1f681\">" },
{":steam_locomotive:","<sprite name=\"1f682\">" },
{":railway_car:","<sprite name=\"1f683\">" },
{":bullettrain_side:","<sprite name=\"1f684\">" },
{":bullettrain_front:","<sprite name=\"1f685\">" },
{":train2:","<sprite name=\"1f686\">" },
{":metro:","<sprite name=\"1f687\">" },
{":light_rail:","<sprite name=\"1f688\">" },
{":station:","<sprite name=\"1f689\">" },
{":tram:","<sprite name=\"1f68a\">" },
{":train:","<sprite name=\"1f68b\">" },
{":bus:","<sprite name=\"1f68c\">" },
{":oncoming_bus:","<sprite name=\"1f68d\">" },
{":trolleybus:","<sprite name=\"1f68e\">" },
{":busstop:","<sprite name=\"1f68f\">" },
{":minibus:","<sprite name=\"1f690\">" },
{":ambulance:","<sprite name=\"1f691\">" },
{":fire_engine:","<sprite name=\"1f692\">" },
{":police_car:","<sprite name=\"1f693\">" },
{":oncoming_police_car:","<sprite name=\"1f694\">" },
{":taxi:","<sprite name=\"1f695\">" },
{":oncoming_taxi:","<sprite name=\"1f696\">" },
{":car:","<sprite name=\"1f697\">" },
{":oncoming_automobile:","<sprite name=\"1f698\">" },
{":blue_car:","<sprite name=\"1f699\">" },
{":truck:","<sprite name=\"1f69a\">" },
{":articulated_lorry:","<sprite name=\"1f69b\">" },
{":tractor:","<sprite name=\"1f69c\">" },
{":monorail:","<sprite name=\"1f69d\">" },
{":mountain_railway:","<sprite name=\"1f69e\">" },
{":suspension_railway:","<sprite name=\"1f69f\">" },
{":mountain_cableway:","<sprite name=\"1f6a0\">" },
{":aerial_tramway:","<sprite name=\"1f6a1\">" },
{":ship:","<sprite name=\"1f6a2\">" },
{":woman-rowing-boat:","<sprite name=\"1f6a3-200d-2640-fe0f\">" },
{":man-rowing-boat:","<sprite name=\"1f6a3-200d-2642-fe0f\">" },
{":rowboat:","<sprite name=\"1f6a3\">" },
{":speedboat:","<sprite name=\"1f6a4\">" },
{":traffic_light:","<sprite name=\"1f6a5\">" },
{":vertical_traffic_light:","<sprite name=\"1f6a6\">" },
{":construction:","<sprite name=\"1f6a7\">" },
{":rotating_light:","<sprite name=\"1f6a8\">" },
{":triangular_flag_on_post:","<sprite name=\"1f6a9\">" },
{":door:","<sprite name=\"1f6aa\">" },
{":no_entry_sign:","<sprite name=\"1f6ab\">" },
{":smoking:","<sprite name=\"1f6ac\">" },
{":no_smoking:","<sprite name=\"1f6ad\">" },
{":put_litter_in_its_place:","<sprite name=\"1f6ae\">" },
{":do_not_litter:","<sprite name=\"1f6af\">" },
{":potable_water:","<sprite name=\"1f6b0\">" },
{":non-potable_water:","<sprite name=\"1f6b1\">" },
{":bike:","<sprite name=\"1f6b2\">" },
{":no_bicycles:","<sprite name=\"1f6b3\">" },
{":woman-biking:","<sprite name=\"1f6b4-200d-2640-fe0f\">" },
{":man-biking:","<sprite name=\"1f6b4-200d-2642-fe0f\">" },
{":bicyclist:","<sprite name=\"1f6b4\">" },
{":woman-mountain-biking:","<sprite name=\"1f6b5-200d-2640-fe0f\">" },
{":man-mountain-biking:","<sprite name=\"1f6b5-200d-2642-fe0f\">" },
{":mountain_bicyclist:","<sprite name=\"1f6b5\">" },
{":woman-walking:","<sprite name=\"1f6b6-200d-2640-fe0f\">" },
{":man-walking:","<sprite name=\"1f6b6-200d-2642-fe0f\">" },
{":walking:","<sprite name=\"1f6b6\">" },
{":no_pedestrians:","<sprite name=\"1f6b7\">" },
{":children_crossing:","<sprite name=\"1f6b8\">" },
{":mens:","<sprite name=\"1f6b9\">" },
{":womens:","<sprite name=\"1f6ba\">" },
{":restroom:","<sprite name=\"1f6bb\">" },
{":baby_symbol:","<sprite name=\"1f6bc\">" },
{":toilet:","<sprite name=\"1f6bd\">" },
{":wc:","<sprite name=\"1f6be\">" },
{":shower:","<sprite name=\"1f6bf\">" },
{":bath:","<sprite name=\"1f6c0\">" },
{":bathtub:","<sprite name=\"1f6c1\">" },
{":passport_control:","<sprite name=\"1f6c2\">" },
{":customs:","<sprite name=\"1f6c3\">" },
{":baggage_claim:","<sprite name=\"1f6c4\">" },
{":left_luggage:","<sprite name=\"1f6c5\">" },
{":couch_and_lamp:","<sprite name=\"1f6cb-fe0f\">" },
{":sleeping_accommodation:","<sprite name=\"1f6cc\">" },
{":shopping_bags:","<sprite name=\"1f6cd-fe0f\">" },
{":bellhop_bell:","<sprite name=\"1f6ce-fe0f\">" },
{":bed:","<sprite name=\"1f6cf-fe0f\">" },
{":place_of_worship:","<sprite name=\"1f6d0\">" },
{":octagonal_sign:","<sprite name=\"1f6d1\">" },
{":shopping_trolley:","<sprite name=\"1f6d2\">" },
{":hammer_and_wrench:","<sprite name=\"1f6e0-fe0f\">" },
{":shield:","<sprite name=\"1f6e1-fe0f\">" },
{":oil_drum:","<sprite name=\"1f6e2-fe0f\">" },
{":motorway:","<sprite name=\"1f6e3-fe0f\">" },
{":railway_track:","<sprite name=\"1f6e4-fe0f\">" },
{":motor_boat:","<sprite name=\"1f6e5-fe0f\">" },
{":small_airplane:","<sprite name=\"1f6e9-fe0f\">" },
{":airplane_departure:","<sprite name=\"1f6eb\">" },
{":airplane_arriving:","<sprite name=\"1f6ec\">" },
{":satellite:","<sprite name=\"1f6f0-fe0f\">" },
{":passenger_ship:","<sprite name=\"1f6f3-fe0f\">" },
{":scooter:","<sprite name=\"1f6f4\">" },
{":motor_scooter:","<sprite name=\"1f6f5\">" },
{":canoe:","<sprite name=\"1f6f6\">" },
{":sled:","<sprite name=\"1f6f7\">" },
{":flying_saucer:","<sprite name=\"1f6f8\">" },
{":zipper_mouth_face:","<sprite name=\"1f910\">" },
{":money_mouth_face:","<sprite name=\"1f911\">" },
{":face_with_thermometer:","<sprite name=\"1f912\">" },
{":nerd_face:","<sprite name=\"1f913\">" },
{":thinking_face:","<sprite name=\"1f914\">" },
{":face_with_head_bandage:","<sprite name=\"1f915\">" },
{":robot_face:","<sprite name=\"1f916\">" },
{":hugging_face:","<sprite name=\"1f917\">" },
{":the_horns:","<sprite name=\"1f918\">" },
{":call_me_hand:","<sprite name=\"1f919\">" },
{":raised_back_of_hand:","<sprite name=\"1f91a\">" },
{":left-facing_fist:","<sprite name=\"1f91b\">" },
{":right-facing_fist:","<sprite name=\"1f91c\">" },
{":handshake:","<sprite name=\"1f91d\">" },
{":crossed_fingers:","<sprite name=\"1f91e\">" },
{":i_love_you_hand_sign:","<sprite name=\"1f91f\">" },
{":face_with_cowboy_hat:","<sprite name=\"1f920\">" },
{":clown_face:","<sprite name=\"1f921\">" },
{":nauseated_face:","<sprite name=\"1f922\">" },
{":rolling_on_the_floor_laughing:","<sprite name=\"1f923\">" },
{":drooling_face:","<sprite name=\"1f924\">" },
{":lying_face:","<sprite name=\"1f925\">" },
{":woman-facepalming:","<sprite name=\"1f926-200d-2640-fe0f\">" },
{":man-facepalming:","<sprite name=\"1f926-200d-2642-fe0f\">" },
{":face_palm:","<sprite name=\"1f926\">" },
{":sneezing_face:","<sprite name=\"1f927\">" },
{":face_with_raised_eyebrow:","<sprite name=\"1f928\">" },
{":star-struck:","<sprite name=\"1f929\">" },
{":zany_face:","<sprite name=\"1f92a\">" },
{":shushing_face:","<sprite name=\"1f92b\">" },
{":face_with_symbols_on_mouth:","<sprite name=\"1f92c\">" },
{":face_with_hand_over_mouth:","<sprite name=\"1f92d\">" },
{":face_vomiting:","<sprite name=\"1f92e\">" },
{":exploding_head:","<sprite name=\"1f92f\">" },
{":pregnant_woman:","<sprite name=\"1f930\">" },
{":breast-feeding:","<sprite name=\"1f931\">" },
{":palms_up_together:","<sprite name=\"1f932\">" },
{":selfie:","<sprite name=\"1f933\">" },
{":prince:","<sprite name=\"1f934\">" },
{":man_in_tuxedo:","<sprite name=\"1f935\">" },
{":mrs_claus:","<sprite name=\"1f936\">" },
{":woman-shrugging:","<sprite name=\"1f937-200d-2640-fe0f\">" },
{":man-shrugging:","<sprite name=\"1f937-200d-2642-fe0f\">" },
{":shrug:","<sprite name=\"1f937\">" },
{":woman-cartwheeling:","<sprite name=\"1f938-200d-2640-fe0f\">" },
{":man-cartwheeling:","<sprite name=\"1f938-200d-2642-fe0f\">" },
{":person_doing_cartwheel:","<sprite name=\"1f938\">" },
{":woman-juggling:","<sprite name=\"1f939-200d-2640-fe0f\">" },
{":man-juggling:","<sprite name=\"1f939-200d-2642-fe0f\">" },
{":juggling:","<sprite name=\"1f939\">" },
{":fencer:","<sprite name=\"1f93a\">" },
{":woman-wrestling:","<sprite name=\"1f93c-200d-2640-fe0f\">" },
{":man-wrestling:","<sprite name=\"1f93c-200d-2642-fe0f\">" },
{":wrestlers:","<sprite name=\"1f93c\">" },
{":woman-playing-water-polo:","<sprite name=\"1f93d-200d-2640-fe0f\">" },
{":man-playing-water-polo:","<sprite name=\"1f93d-200d-2642-fe0f\">" },
{":water_polo:","<sprite name=\"1f93d\">" },
{":woman-playing-handball:","<sprite name=\"1f93e-200d-2640-fe0f\">" },
{":man-playing-handball:","<sprite name=\"1f93e-200d-2642-fe0f\">" },
{":handball:","<sprite name=\"1f93e\">" },
{":wilted_flower:","<sprite name=\"1f940\">" },
{":drum_with_drumsticks:","<sprite name=\"1f941\">" },
{":clinking_glasses:","<sprite name=\"1f942\">" },
{":tumbler_glass:","<sprite name=\"1f943\">" },
{":spoon:","<sprite name=\"1f944\">" },
{":goal_net:","<sprite name=\"1f945\">" },
{":first_place_medal:","<sprite name=\"1f947\">" },
{":second_place_medal:","<sprite name=\"1f948\">" },
{":third_place_medal:","<sprite name=\"1f949\">" },
{":boxing_glove:","<sprite name=\"1f94a\">" },
{":martial_arts_uniform:","<sprite name=\"1f94b\">" },
{":curling_stone:","<sprite name=\"1f94c\">" },
{":croissant:","<sprite name=\"1f950\">" },
{":avocado:","<sprite name=\"1f951\">" },
{":cucumber:","<sprite name=\"1f952\">" },
{":bacon:","<sprite name=\"1f953\">" },
{":potato:","<sprite name=\"1f954\">" },
{":carrot:","<sprite name=\"1f955\">" },
{":baguette_bread:","<sprite name=\"1f956\">" },
{":green_salad:","<sprite name=\"1f957\">" },
{":shallow_pan_of_food:","<sprite name=\"1f958\">" },
{":stuffed_flatbread:","<sprite name=\"1f959\">" },
{":egg:","<sprite name=\"1f95a\">" },
{":glass_of_milk:","<sprite name=\"1f95b\">" },
{":peanuts:","<sprite name=\"1f95c\">" },
{":kiwifruit:","<sprite name=\"1f95d\">" },
{":pancakes:","<sprite name=\"1f95e\">" },
{":dumpling:","<sprite name=\"1f95f\">" },
{":fortune_cookie:","<sprite name=\"1f960\">" },
{":takeout_box:","<sprite name=\"1f961\">" },
{":chopsticks:","<sprite name=\"1f962\">" },
{":bowl_with_spoon:","<sprite name=\"1f963\">" },
{":cup_with_straw:","<sprite name=\"1f964\">" },
{":coconut:","<sprite name=\"1f965\">" },
{":broccoli:","<sprite name=\"1f966\">" },
{":pie:","<sprite name=\"1f967\">" },
{":pretzel:","<sprite name=\"1f968\">" },
{":cut_of_meat:","<sprite name=\"1f969\">" },
{":sandwich:","<sprite name=\"1f96a\">" },
{":canned_food:","<sprite name=\"1f96b\">" },
{":crab:","<sprite name=\"1f980\">" },
{":lion_face:","<sprite name=\"1f981\">" },
{":scorpion:","<sprite name=\"1f982\">" },
{":turkey:","<sprite name=\"1f983\">" },
{":unicorn_face:","<sprite name=\"1f984\">" },
{":eagle:","<sprite name=\"1f985\">" },
{":duck:","<sprite name=\"1f986\">" },
{":bat:","<sprite name=\"1f987\">" },
{":shark:","<sprite name=\"1f988\">" },
{":owl:","<sprite name=\"1f989\">" },
{":fox_face:","<sprite name=\"1f98a\">" },
{":butterfly:","<sprite name=\"1f98b\">" },
{":deer:","<sprite name=\"1f98c\">" },
{":gorilla:","<sprite name=\"1f98d\">" },
{":lizard:","<sprite name=\"1f98e\">" },
{":rhinoceros:","<sprite name=\"1f98f\">" },
{":shrimp:","<sprite name=\"1f990\">" },
{":squid:","<sprite name=\"1f991\">" },
{":giraffe_face:","<sprite name=\"1f992\">" },
{":zebra_face:","<sprite name=\"1f993\">" },
{":hedgehog:","<sprite name=\"1f994\">" },
{":sauropod:","<sprite name=\"1f995\">" },
{":t-rex:","<sprite name=\"1f996\">" },
{":cricket:","<sprite name=\"1f997\">" },
{":cheese_wedge:","<sprite name=\"1f9c0\">" },
{":face_with_monocle:","<sprite name=\"1f9d0\">" },
{":adult:","<sprite name=\"1f9d1\">" },
{":child:","<sprite name=\"1f9d2\">" },
{":older_adult:","<sprite name=\"1f9d3\">" },
{":bearded_person:","<sprite name=\"1f9d4\">" },
{":person_with_headscarf:","<sprite name=\"1f9d5\">" },
{":woman_in_steamy_room:","<sprite name=\"1f9d6-200d-2640-fe0f\">" },
{":man_in_steamy_room:","<sprite name=\"1f9d6-200d-2642-fe0f\">" },
{":person_in_steamy_room:","<sprite name=\"1f9d6\">" },
{":woman_climbing:","<sprite name=\"1f9d7-200d-2640-fe0f\">" },
{":man_climbing:","<sprite name=\"1f9d7-200d-2642-fe0f\">" },
{":person_climbing:","<sprite name=\"1f9d7\">" },
{":woman_in_lotus_position:","<sprite name=\"1f9d8-200d-2640-fe0f\">" },
{":man_in_lotus_position:","<sprite name=\"1f9d8-200d-2642-fe0f\">" },
{":person_in_lotus_position:","<sprite name=\"1f9d8\">" },
{":female_mage:","<sprite name=\"1f9d9-200d-2640-fe0f\">" },
{":male_mage:","<sprite name=\"1f9d9-200d-2642-fe0f\">" },
{":mage:","<sprite name=\"1f9d9\">" },
{":female_fairy:","<sprite name=\"1f9da-200d-2640-fe0f\">" },
{":male_fairy:","<sprite name=\"1f9da-200d-2642-fe0f\">" },
{":fairy:","<sprite name=\"1f9da\">" },
{":female_vampire:","<sprite name=\"1f9db-200d-2640-fe0f\">" },
{":male_vampire:","<sprite name=\"1f9db-200d-2642-fe0f\">" },
{":vampire:","<sprite name=\"1f9db\">" },
{":mermaid:","<sprite name=\"1f9dc-200d-2640-fe0f\">" },
{":merman:","<sprite name=\"1f9dc-200d-2642-fe0f\">" },
{":merperson:","<sprite name=\"1f9dc\">" },
{":female_elf:","<sprite name=\"1f9dd-200d-2640-fe0f\">" },
{":male_elf:","<sprite name=\"1f9dd-200d-2642-fe0f\">" },
{":elf:","<sprite name=\"1f9dd\">" },
{":female_genie:","<sprite name=\"1f9de-200d-2640-fe0f\">" },
{":male_genie:","<sprite name=\"1f9de-200d-2642-fe0f\">" },
{":genie:","<sprite name=\"1f9de\">" },
{":female_zombie:","<sprite name=\"1f9df-200d-2640-fe0f\">" },
{":male_zombie:","<sprite name=\"1f9df-200d-2642-fe0f\">" },
{":zombie:","<sprite name=\"1f9df\">" },
{":brain:","<sprite name=\"1f9e0\">" },
{":orange_heart:","<sprite name=\"1f9e1\">" },
{":billed_cap:","<sprite name=\"1f9e2\">" },
{":scarf:","<sprite name=\"1f9e3\">" },
{":gloves:","<sprite name=\"1f9e4\">" },
{":coat:","<sprite name=\"1f9e5\">" },
{":socks:","<sprite name=\"1f9e6\">" },
{":bangbang:","<sprite name=\"203c-fe0f\">" },
{":interrobang:","<sprite name=\"2049-fe0f\">" },
{":tm:","<sprite name=\"2122-fe0f\">" },
{":information_source:","<sprite name=\"2139-fe0f\">" },
{":left_right_arrow:","<sprite name=\"2194-fe0f\">" },
{":arrow_up_down:","<sprite name=\"2195-fe0f\">" },
{":arrow_upper_left:","<sprite name=\"2196-fe0f\">" },
{":arrow_upper_right:","<sprite name=\"2197-fe0f\">" },
{":arrow_lower_right:","<sprite name=\"2198-fe0f\">" },
{":arrow_lower_left:","<sprite name=\"2199-fe0f\">" },
{":leftwards_arrow_with_hook:","<sprite name=\"21a9-fe0f\">" },
{":arrow_right_hook:","<sprite name=\"21aa-fe0f\">" },
{":watch:","<sprite name=\"231a\">" },
{":hourglass:","<sprite name=\"231b\">" },
{":keyboard:","<sprite name=\"2328-fe0f\">" },
{":eject:","<sprite name=\"23cf-fe0f\">" },
{":fast_forward:","<sprite name=\"23e9\">" },
{":rewind:","<sprite name=\"23ea\">" },
{":arrow_double_up:","<sprite name=\"23eb\">" },
{":arrow_double_down:","<sprite name=\"23ec\">" },
{":black_right_pointing_double_triangle_with_vertical_bar:","<sprite name=\"23ed-fe0f\">" },
{":black_left_pointing_double_triangle_with_vertical_bar:","<sprite name=\"23ee-fe0f\">" },
{":black_right_pointing_triangle_with_double_vertical_bar:","<sprite name=\"23ef-fe0f\">" },
{":alarm_clock:","<sprite name=\"23f0\">" },
{":stopwatch:","<sprite name=\"23f1-fe0f\">" },
{":timer_clock:","<sprite name=\"23f2-fe0f\">" },
{":hourglass_flowing_sand:","<sprite name=\"23f3\">" },
{":double_vertical_bar:","<sprite name=\"23f8-fe0f\">" },
{":black_square_for_stop:","<sprite name=\"23f9-fe0f\">" },
{":black_circle_for_record:","<sprite name=\"23fa-fe0f\">" },
{":m:","<sprite name=\"24c2-fe0f\">" },
{":black_small_square:","<sprite name=\"25aa-fe0f\">" },
{":white_small_square:","<sprite name=\"25ab-fe0f\">" },
{":arrow_forward:","<sprite name=\"25b6-fe0f\">" },
{":arrow_backward:","<sprite name=\"25c0-fe0f\">" },
{":white_medium_square:","<sprite name=\"25fb-fe0f\">" },
{":black_medium_square:","<sprite name=\"25fc-fe0f\">" },
{":white_medium_small_square:","<sprite name=\"25fd\">" },
{":black_medium_small_square:","<sprite name=\"25fe\">" },
{":sunny:","<sprite name=\"2600-fe0f\">" },
{":cloud:","<sprite name=\"2601-fe0f\">" },
{":umbrella:","<sprite name=\"2602-fe0f\">" },
{":snowman:","<sprite name=\"2603-fe0f\">" },
{":comet:","<sprite name=\"2604-fe0f\">" },
{":phone:","<sprite name=\"260e-fe0f\">" },
{":ballot_box_with_check:","<sprite name=\"2611-fe0f\">" },
{":umbrella_with_rain_drops:","<sprite name=\"2614\">" },
{":coffee:","<sprite name=\"2615\">" },
{":shamrock:","<sprite name=\"2618-fe0f\">" },
{":point_up:","<sprite name=\"261d-fe0f\">" },
{":skull_and_crossbones:","<sprite name=\"2620-fe0f\">" },
{":radioactive_sign:","<sprite name=\"2622-fe0f\">" },
{":biohazard_sign:","<sprite name=\"2623-fe0f\">" },
{":orthodox_cross:","<sprite name=\"2626-fe0f\">" },
{":star_and_crescent:","<sprite name=\"262a-fe0f\">" },
{":peace_symbol:","<sprite name=\"262e-fe0f\">" },
{":yin_yang:","<sprite name=\"262f-fe0f\">" },
{":wheel_of_dharma:","<sprite name=\"2638-fe0f\">" },
{":white_frowning_face:","<sprite name=\"2639-fe0f\">" },
{":relaxed:","<sprite name=\"263a-fe0f\">" },
{":female_sign:","<sprite name=\"2640-fe0f\">" },
{":male_sign:","<sprite name=\"2642-fe0f\">" },
{":aries:","<sprite name=\"2648\">" },
{":taurus:","<sprite name=\"2649\">" },
{":gemini:","<sprite name=\"264a\">" },
{":cancer:","<sprite name=\"264b\">" },
{":leo:","<sprite name=\"264c\">" },
{":virgo:","<sprite name=\"264d\">" },
{":libra:","<sprite name=\"264e\">" },
{":scorpius:","<sprite name=\"264f\">" },
{":sagittarius:","<sprite name=\"2650\">" },
{":capricorn:","<sprite name=\"2651\">" },
{":aquarius:","<sprite name=\"2652\">" },
{":pisces:","<sprite name=\"2653\">" },
{":spades:","<sprite name=\"2660-fe0f\">" },
{":clubs:","<sprite name=\"2663-fe0f\">" },
{":hearts:","<sprite name=\"2665-fe0f\">" },
{":diamonds:","<sprite name=\"2666-fe0f\">" },
{":hotsprings:","<sprite name=\"2668-fe0f\">" },
{":recycle:","<sprite name=\"267b-fe0f\">" },
{":wheelchair:","<sprite name=\"267f\">" },
{":hammer_and_pick:","<sprite name=\"2692-fe0f\">" },
{":anchor:","<sprite name=\"2693\">" },
{":crossed_swords:","<sprite name=\"2694-fe0f\">" },
{":medical_symbol:","<sprite name=\"2695-fe0f\">" },
{":scales:","<sprite name=\"2696-fe0f\">" },
{":alembic:","<sprite name=\"2697-fe0f\">" },
{":gear:","<sprite name=\"2699-fe0f\">" },
{":atom_symbol:","<sprite name=\"269b-fe0f\">" },
{":fleur_de_lis:","<sprite name=\"269c-fe0f\">" },
{":warning:","<sprite name=\"26a0-fe0f\">" },
{":zap:","<sprite name=\"26a1\">" },
{":white_circle:","<sprite name=\"26aa\">" },
{":black_circle:","<sprite name=\"26ab\">" },
{":coffin:","<sprite name=\"26b0-fe0f\">" },
{":funeral_urn:","<sprite name=\"26b1-fe0f\">" },
{":soccer:","<sprite name=\"26bd\">" },
{":baseball:","<sprite name=\"26be\">" },
{":snowman_without_snow:","<sprite name=\"26c4\">" },
{":partly_sunny:","<sprite name=\"26c5\">" },
{":thunder_cloud_and_rain:","<sprite name=\"26c8-fe0f\">" },
{":ophiuchus:","<sprite name=\"26ce\">" },
{":pick:","<sprite name=\"26cf-fe0f\">" },
{":helmet_with_white_cross:","<sprite name=\"26d1-fe0f\">" },
{":chains:","<sprite name=\"26d3-fe0f\">" },
{":no_entry:","<sprite name=\"26d4\">" },
{":shinto_shrine:","<sprite name=\"26e9-fe0f\">" },
{":church:","<sprite name=\"26ea\">" },
{":mountain:","<sprite name=\"26f0-fe0f\">" },
{":umbrella_on_ground:","<sprite name=\"26f1-fe0f\">" },
{":fountain:","<sprite name=\"26f2\">" },
{":golf:","<sprite name=\"26f3\">" },
{":ferry:","<sprite name=\"26f4-fe0f\">" },
{":boat:","<sprite name=\"26f5\">" },
{":skier:","<sprite name=\"26f7-fe0f\">" },
{":ice_skate:","<sprite name=\"26f8-fe0f\">" },
{":woman-bouncing-ball:","<sprite name=\"26f9-fe0f-200d-2640-fe0f\">" },
{":man-bouncing-ball:","<sprite name=\"26f9-fe0f-200d-2642-fe0f\">" },
{":person_with_ball:","<sprite name=\"26f9-fe0f\">" },
{":tent:","<sprite name=\"26fa\">" },
{":fuelpump:","<sprite name=\"26fd\">" },
{":scissors:","<sprite name=\"2702-fe0f\">" },
{":white_check_mark:","<sprite name=\"2705\">" },
{":airplane:","<sprite name=\"2708-fe0f\">" },
{":email:","<sprite name=\"2709-fe0f\">" },
{":fist:","<sprite name=\"270a\">" },
{":hand:","<sprite name=\"270b\">" },
{":v:","<sprite name=\"270c-fe0f\">" },
{":writing_hand:","<sprite name=\"270d-fe0f\">" },
{":pencil2:","<sprite name=\"270f-fe0f\">" },
{":black_nib:","<sprite name=\"2712-fe0f\">" },
{":heavy_check_mark:","<sprite name=\"2714-fe0f\">" },
{":heavy_multiplication_x:","<sprite name=\"2716-fe0f\">" },
{":latin_cross:","<sprite name=\"271d-fe0f\">" },
{":star_of_david:","<sprite name=\"2721-fe0f\">" },
{":sparkles:","<sprite name=\"2728\">" },
{":eight_spoked_asterisk:","<sprite name=\"2733-fe0f\">" },
{":eight_pointed_black_star:","<sprite name=\"2734-fe0f\">" },
{":snowflake:","<sprite name=\"2744-fe0f\">" },
{":sparkle:","<sprite name=\"2747-fe0f\">" },
{":x:","<sprite name=\"274c\">" },
{":negative_squared_cross_mark:","<sprite name=\"274e\">" },
{":question:","<sprite name=\"2753\">" },
{":grey_question:","<sprite name=\"2754\">" },
{":grey_exclamation:","<sprite name=\"2755\">" },
{":exclamation:","<sprite name=\"2757\">" },
{":heavy_heart_exclamation_mark_ornament:","<sprite name=\"2763-fe0f\">" },
{":heart:","<sprite name=\"2764-fe0f\">" },
{":heavy_plus_sign:","<sprite name=\"2795\">" },
{":heavy_minus_sign:","<sprite name=\"2796\">" },
{":heavy_division_sign:","<sprite name=\"2797\">" },
{":arrow_right:","<sprite name=\"27a1-fe0f\">" },
{":curly_loop:","<sprite name=\"27b0\">" },
{":loop:","<sprite name=\"27bf\">" },
{":arrow_heading_up:","<sprite name=\"2934-fe0f\">" },
{":arrow_heading_down:","<sprite name=\"2935-fe0f\">" },
{":arrow_left:","<sprite name=\"2b05-fe0f\">" },
{":arrow_up:","<sprite name=\"2b06-fe0f\">" },
{":arrow_down:","<sprite name=\"2b07-fe0f\">" },
{":black_large_square:","<sprite name=\"2b1b\">" },
{":white_large_square:","<sprite name=\"2b1c\">" },
{":star:","<sprite name=\"2b50\">" },
{":o:","<sprite name=\"2b55\">" },
{":wavy_dash:","<sprite name=\"3030-fe0f\">" },
{":part_alternation_mark:","<sprite name=\"303d-fe0f\">" },
{":congratulations:","<sprite name=\"3297-fe0f\">" },
{":secret:","<sprite name=\"3299-fe0f\">" },
};

    }
}