using HMLLibrary;
using RaftModLoader;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UltimateWater;
using UnityEngine;
using UnityEngine.AzureSky;
using UnityEngine.SceneManagement;
using RTCP = RaftModLoader.RTCP;

namespace RaftModLoader
{
    public class DefaultConsoleCommands : MonoBehaviour
    {
        public static List<string> safemodeDisabledCommands = new List<string>()
    {
        "getLoadedMods",
        "mod.reload",
        "mod.load",
        "mod.unload",
        "setConsoleMaxLogs",
        "resetConsoleMaxLogs",
        "timeset",
        "settimescale",
        "kill",
        "getcurrentlevel",
        "loadlevel",
        "poopmode",
        "noclip",
        "gotoraft",
        "togglecustomchat",
        "csrun",
        "safemode"
    };

        [ConsoleCommand("unityVersion", "Displays the unity version that the game use.")]
        public static string UnityVersion()
        {
            return "Current Unity Version: " + Application.unityVersion;
        }

        [ConsoleCommand("getLoadedMods", "Displays the loaded mods.")]
        public static string LoadedMods()
        {
            string mods = "";
            if (ModManagerPage.activeModInstances.Count > 0)
            {
                mods = "Mods (" + ModManagerPage.activeModInstances.Count + ") : ";
                foreach (Mod mod in ModManagerPage.activeModInstances.ToArray())
                {
                    mods += mod.name + "@" + mod.version + ",";
                }
                mods = mods.TrimEnd(',');
                mods += ".";
            }
            else
            {
                mods = "Mods (0) : No mods are currently loaded.";
            }
            return mods;
        }

        [ConsoleCommand("mod.reload", "Syntax : 'mod.reload <mod name>' Reload the specified mod.")]
        public static async void ModReload(string[] args)
        {
            if (args.Length > 0)
            {
                string name = string.Join(" ", args);
                ModData md = ModManagerPage.modList.Find(x => x.jsonmodinfo.name.ToLower() == name.ToLower());
                if (md != null)
                {
                    if (md.modinfo.modState == ModInfo.ModStateEnum.running)
                    {
                        md.modinfo.modHandler.UnloadMod(md);
                        await Task.Delay(50);
                        md.modinfo.modHandler.LoadMod(md);
                    }
                    else
                    {
                        Debug.LogWarning("The mod \"" + md.jsonmodinfo.name + "\" wasn't running. Loading it...");
                        md.modinfo.modHandler.LoadMod(md);

                    }
                }
                else
                {
                    Debug.LogWarning("Couldn't find a mod with the name \"" + string.Join(" ", args) + "\"");
                }
            }
            else
            {
                Debug.LogWarning("Syntax error! Usage: <u><i>mod.reload <mod name></i></u>.");
            }
        }

        [ConsoleCommand("mod.load", "Syntax : 'mod.load <mod name>' Load the specified mod.")]
        public static async void ModLoad(string[] args)
        {
            if (args.Length > 0)
            {
                string name = string.Join(" ", args);
                ModData md = ModManagerPage.modList.Find(x => x.jsonmodinfo.name.ToLower() == name.ToLower());
                if (md != null)
                {
                    if (md.modinfo.modState != ModInfo.ModStateEnum.running)
                    {
                        md.modinfo.modHandler.LoadMod(md);
                    }
                    else if (md.modinfo.modState == ModInfo.ModStateEnum.running)
                    {
                        md.modinfo.modHandler.UnloadMod(md);
                        await Task.Delay(50);
                        md.modinfo.modHandler.LoadMod(md);
                    }
                    else
                    {

                        Debug.LogWarning("The mod \"" + md.jsonmodinfo.name + "\" can't be loaded, check its status.");
                    }
                }
                else
                {
                    Debug.LogWarning("Couldn't find a mod with the name \"" + string.Join(" ", args) + "\"");
                }
            }
            else
            {
                Debug.LogWarning("Syntax error! Usage: <u><i>mod.load <mod name></i></u>");
            }
        }

        [ConsoleCommand("mod.unload", "Syntax : 'mod.unload <mod name>' Unload the specified mod.")]
        public static async void ModUnload(string[] args)
        {
            if (args.Length > 0)
            {
                string name = string.Join(" ", args);
                ModData md = ModManagerPage.modList.Find(x => x.jsonmodinfo.name.ToLower() == name.ToLower());
                if (md != null)
                {
                    if (md.modinfo.modState == ModInfo.ModStateEnum.running)
                    {
                        md.modinfo.modHandler.UnloadMod(md);
                    }
                    else
                    {
                        Debug.LogWarning("The mod \"" + md.jsonmodinfo.name + "\" needs to be running to be unloaded.");
                    }
                }
                else
                {
                    Debug.LogWarning("Couldn't find a mod with the name \"" + string.Join(" ", args) + "\"");
                }
            }
            else
            {
                Debug.LogWarning("Syntax error! Usage: <u><i>mod.unload <mod name></i></u>");
            }
        }

        [ConsoleCommand("raftVersion", "Displays the current Raft version.")]
        public static string RaftVersion()
        {
            return "Current Raft Version: " + Settings.VersionNumberText;
        }

        [ConsoleCommand("dotnetVersion", "Displays the current .NET version that the environment use.")]
        static string DotNetVersion()
        {
            return "Current .NET Version: " + Environment.Version.ToString();
        }

        [ConsoleCommand("clear", "Clear the current console output.")]
        public static void clear()
        {
            RConsole.ClearConsole();
        }

        [ConsoleCommand("listRafters", "Displays all your friends currently playing Raft.")]
        private static void ListRafters()
        {
            int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
            List<string> rafters = new List<string>();
            for (int i = 0; i < friendCount; ++i)
            {
                CSteamID friendSteamId = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
                string friendName = SteamFriends.GetFriendPersonaName(friendSteamId);
                FriendGameInfo_t gameInfo;
                SteamFriends.GetFriendGamePlayed(friendSteamId, out gameInfo);
                if (gameInfo.m_gameID.ToString() == "648800")
                {
                    rafters.Add(friendName + " is playing Raft! its SteamID is " + friendSteamId.GetAccountID().ToString());
                }
            }
            if (rafters.Count > 0)
            {
                Debug.Log("You have " + rafters.Count + " friend(s) currently playing Raft!");
                foreach (string s in rafters)
                {
                    Debug.Log(s);
                }
            }
            else
            {
                Debug.Log("None of your friends are currently playing Raft!");
            }
        }
        // CSteamID csteamid = new CSteamID(new AccountID_t(1), EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeIndividual);
        //ServersPage.ConnectToServer(csteamid, "", false);
        [ConsoleCommand("connect", "Syntax : 'connect <IP or Steam ID> <password>' Join the specified server.")]
        public static async void TryJoinServer(string[] args)
        {
            if (args.Length < 1)
            {
                Debug.LogWarning("Syntax error! Usage: <u><i>connect <IP or Steam ID> <password></i></u>");
                return;
            }
            string steamid = args[0].Replace("localhost", "127.0.0.1");
            string password = "";
            if (args.Length >= 2)
            {
                args[0] = null;
                password = String.Join(" ", args).TrimStart(' ');
            }
            if (steamid.Contains("."))
            {
                string ip = steamid.Split(':')[0];
                string sport = steamid.Contains(":") ? steamid.Split(':')[1] : "6969";
                int port = 6969;
                bool validPort = int.TryParse(sport, out port);
                if (!validPort)
                {
                    Debug.LogWarning("The specified port is invalid !");
                    return;
                }
                IPAddress address = null;
                bool valid = IPAddress.TryParse(ip, out address);
                if (valid)
                {
                    Debug.Log("Connecting to <b>" + ip + ":" + port + "</b>" + (password == "" ? "" : " with the specified password") + "...");
                    ServersPage.lastPassword = password;
                    RaftModLoader.RTCP.JoinServer(address, port);
                    return;
                }
                else
                {
                    Debug.LogWarning("The specified IP is invalid !");
                    return;
                }
            }
            try
            {
                if (steamid.Length > 5 && steamid.Length < 12)
                {
                    uint accountid = uint.Parse(steamid);
                    CSteamID csteamid = new CSteamID(new AccountID_t(accountid), EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeIndividual);
                    if (!csteamid.IsValid())
                    {
                        Debug.LogWarning("The provided SteamID is invalid, Valid SteamID's are SteamID,SteamID64,SteamID3 and AccountID");
                        return;
                    }
                    ServersPage.ConnectToServer(csteamid, password, false);
                    return;
                }
            }
            catch { }
            RSocket.convertSteamid(steamid, password);
            return;
        }

        [ConsoleCommand("fullscreenMode", "Syntax : 'fullscreenMode windowed/fullscreen/borderless' Set the current fullscreen mode.")]
        public static void fullscreen(string[] args)
        {
            if (args.Length != 1)
            {
                Debug.LogWarning("Syntax error! Usage: <u><i>fullscreenMode windowed/fullscreen/borderless</i></u>");
                return;
            }
            string mode = args[0].ToLower();
            switch (mode)
            {
                case "windowed":
                    Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.Windowed);
                    break;
                case "fullscreen":
                    Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.ExclusiveFullScreen);
                    break;
                case "borderless":
                    Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.FullScreenWindow);
                    break;
                default:
                    Debug.LogWarning("Invalid fullscreen mode, Valid modes are <i>Windowed</i>, <i>Fullscreen</i> or <i>Borderless</i>");
                    break;
            }
        }

        [ConsoleCommand("setConsoleMaxLogs", "Syntax: 'setConsoleMaxLogs <amount>' Change the maximum amount of logs in the console.")]
        public static async void SetMaxLogs(string[] args)
        {
            if (args.Length != 1)
            {
                Debug.LogWarning("You must specify a value. Syntax: 'setConsoleMaxLogs <amount>'");
            }
            else
            {
                int newValue = -1;
                int.TryParse(args[0], out newValue);

                if (newValue < 1 || newValue > 10000)
                {
                    Debug.LogWarning("The value that you specified is invalid, It should be between 1 and 10000!");
                    return;
                }
                else
                {
                    PlayerPrefs.SetInt("rmlSettings_MaxLogs", newValue);
                    RConsole console = (RConsole.instance as RConsole);
                    RConsole.logPool.ForEach(x =>
                    {
                        GameObject.Destroy(x);
                    });
                    RConsole.logPool.Clear();
                    RConsole.logs.ForEach(x =>
                    {
                        GameObject.Destroy(x);
                    });
                    RConsole.logs.Clear();
                    RConsole.latestlog = null;
                    RConsole.MaxLogs = newValue;
                    await console.InitializeConsolePool();
                    Debug.Log("The maximum logs amount has been set to " + newValue + " !");
                }

            }
        }

        [ConsoleCommand("resetConsoleMaxLogs", "Resets the maximum amount of logs in the console.")]
        public static async void ResetMaxLogs(string[] args)
        {
            PlayerPrefs.DeleteKey("rmlSettings_MaxLogs");
            RConsole console = (RConsole.instance as RConsole);
            RConsole.logPool.ForEach(x =>
            {
                GameObject.Destroy(x);
            });
            RConsole.logPool.Clear();
            RConsole.logs.ForEach(x =>
            {
                GameObject.Destroy(x);
            });
            RConsole.logs.Clear();
            RConsole.latestlog = null;
            RConsole.MaxLogs = RConsole.DefaultMaxLogs;
            await console.InitializeConsolePool();
            Debug.Log("The maximum logs amount has been reseted to its default value (" + RConsole.DefaultMaxLogs + ")!");
        }

        [ConsoleCommand("timeset", "Syntax: 'timeset <hour>' Change the game time.")]
        public static void Timeset(string[] args)
        {
            if (args.Length != 1)
            {
                Debug.LogWarning("You must specify a value to change the game time. Syntax: 'timeset <hour>'");
            }
            else
            {
                int newValue = -1;
                int.TryParse(args[0], out newValue);

                if (newValue < 0 || newValue > 24)
                {
                    Debug.LogWarning("The value that you specified is invalid, It should be between 0 and 24!");
                    return;
                }
                else
                {
                    FindObjectOfType<AzureSkyController>().timeOfDay.GotoTime(newValue);
                    Debug.Log("The game time has been changed to " + newValue);
                }

            }
        }

        [ConsoleCommand("settimescale", "Syntax: 'settimescale <speed>' Change the scale at which time passes.")]
        public static void Settimescale(string[] args)
        {
            if (args.Length != 1)
            {
                Debug.LogWarning("You must specify a value to change the game speed.");
                return;
            }

            int newValue = 1;
            int.TryParse(args[0], out newValue);

            if (newValue < 0 || newValue > 100)
            {
                Debug.LogWarning("The value that you specified is invalid, It should be between 0 and 100!");
                return;
            }
            else
            {
                Time.timeScale = newValue;
                Debug.Log("The game timescale has been changed to " + newValue);
            }
        }

        [ConsoleCommand("kill", "Kill yourself.")]
        public static void Kill()
        {
            Network_Player ply = RAPI.GetLocalPlayer();
            if (ply != null && !ply.IsKilled && SceneManager.GetActiveScene().name == Raft_Network.GameSceneName)
            {
                Message_NetworkBehaviour message = new Message_NetworkBehaviour(Messages.PlayerKilled, ply);
                if (Raft_Network.IsHost)
                {
                    ply.Network.RPC(message, Target.Other, EP2PSend.k_EP2PSendReliable, NetworkChannel.Channel_Game);
                    ply.PlayerScript.Kill(false);
                    return;
                }
                ply.SendP2P(message, EP2PSend.k_EP2PSendReliable, NetworkChannel.Channel_Game);

                Debug.Log("The suicide note was written in blue ink.");
                return;
            }
            Debug.LogWarning("You can't suicide right now.");
        }

        [ConsoleCommand("exit", "Exit the game.")]
        public static void exit()
        {
            Application.Quit();
        }

        [ConsoleCommand("getcurrentlevel", "Tells you the current game level.")]
        public static string getcurrentlevel()
        {
            return "Current Level : " + SceneManager.GetActiveScene().name;
        }

        [ConsoleCommand("loadlevel", "Syntax: 'loadlevel <name>' Load the specified level.")]
        public static void loadlevel(string[] args)
        {
            if (args.Length != 1)
            {
                Debug.LogWarning("Invalid Arguments! Syntax: 'loadlevel <value>'");
                return;
            }
            else
            {
                SceneManager.LoadScene(args[0]);
            }
        }


        static PoopModeVariables pmv = new PoopModeVariables();
        [ConsoleCommand("poopmode", "Makes the game looks like minecraft to help on low-end computers.")]
        public static void PoopMode()
        {
            if (!pmv.IsEnabled)
            {
                pmv.IsEnabled = true;
                pmv.SaveCurrentVariables();
                QualitySettings.pixelLightCount = 0;
                QualitySettings.masterTextureLimit = 5;
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
                QualitySettings.antiAliasing = 0;
                QualitySettings.softParticles = false;
                QualitySettings.realtimeReflectionProbes = false;
                QualitySettings.billboardsFaceCameraPosition = false;
                QualitySettings.shadowCascades = 0;
                QualitySettings.shadows = ShadowQuality.Disable;
                QualitySettings.softVegetation = false;
                QualitySettings.vSyncCount = 0;
                QualitySettings.lodBias = 0;
                QualitySettings.maximumLODLevel = 0;
                WaterQualitySettings.Instance.SetQualityLevel(0);
                QualitySettings.shadowDistance = 0;
                QualitySettings.maxQueuedFrames = 1000;
                Settings settings = FindObjectOfType<Settings>();
                settings.graphicsBox.postEffects.ambientOcclusion.enabled = false;
                settings.graphicsBox.postEffects.antialiasing.enabled = false;
                settings.graphicsBox.postEffects.bloom.enabled = false;
                settings.graphicsBox.postEffects.chromaticAberration.enabled = false;
                settings.graphicsBox.postEffects.colorGrading.enabled = false;
                settings.graphicsBox.postEffects.debugViews.enabled = false;
                settings.graphicsBox.postEffects.depthOfField.enabled = false;
                settings.graphicsBox.postEffects.dithering.enabled = false;
                settings.graphicsBox.postEffects.eyeAdaptation.enabled = false;
                settings.graphicsBox.postEffects.fog.enabled = false;
                settings.graphicsBox.postEffects.grain.enabled = false;
                settings.graphicsBox.postEffects.motionBlur.enabled = false;
                settings.graphicsBox.postEffects.screenSpaceReflection.enabled = false;
                settings.graphicsBox.postEffects.userLut.enabled = false;
                settings.graphicsBox.postEffects.vignette.enabled = false;
                FindObjectOfType<VolumetricLightRenderer>().Resolution = VolumetricLightRenderer.VolumtericResolution.Quarter;
                FindObjectOfType<VolumetricLightRenderer>().enabled = false;
                Application.backgroundLoadingPriority = ThreadPriority.Low;
                Application.runInBackground = true;
            }
            else
            {
                pmv.IsEnabled = false;
                QualitySettings.pixelLightCount = pmv.pixelLightCount;
                QualitySettings.masterTextureLimit = pmv.masterTextureLimit;
                QualitySettings.anisotropicFiltering = pmv.anisotropicFiltering;
                QualitySettings.antiAliasing = pmv.antiAliasing;
                QualitySettings.softParticles = pmv.softParticles;
                QualitySettings.realtimeReflectionProbes = pmv.realtimeReflectionProbes;
                QualitySettings.billboardsFaceCameraPosition = pmv.billboardsFaceCameraPosition;
                QualitySettings.shadowCascades = pmv.shadowCascades;
                QualitySettings.shadows = pmv.shadows;
                QualitySettings.softVegetation = pmv.softVegetation;
                QualitySettings.vSyncCount = pmv.vSyncCount;
                QualitySettings.lodBias = pmv.lodBias;
                QualitySettings.maximumLODLevel = pmv.maximumLODLevel;
                WaterQualitySettings.Instance.SetQualityLevel(pmv.WaterQualityLevel);
                QualitySettings.shadowDistance = pmv.shadowDistance;
                QualitySettings.maxQueuedFrames = pmv.maxQueuedFrames;
                Settings settings = FindObjectOfType<Settings>();
                settings.graphicsBox.postEffects.ambientOcclusion.enabled = pmv.ambientOcclusion;
                settings.graphicsBox.postEffects.antialiasing.enabled = pmv.antialiasing;
                settings.graphicsBox.postEffects.bloom.enabled = pmv.bloom;
                settings.graphicsBox.postEffects.chromaticAberration.enabled = pmv.chromaticAberration;
                settings.graphicsBox.postEffects.colorGrading.enabled = pmv.colorGrading;
                settings.graphicsBox.postEffects.debugViews.enabled = pmv.debugViews;
                settings.graphicsBox.postEffects.depthOfField.enabled = pmv.depthOfField;
                settings.graphicsBox.postEffects.dithering.enabled = pmv.dithering;
                settings.graphicsBox.postEffects.eyeAdaptation.enabled = pmv.eyeAdaptation;
                settings.graphicsBox.postEffects.fog.enabled = pmv.fog;
                settings.graphicsBox.postEffects.grain.enabled = pmv.grain;
                settings.graphicsBox.postEffects.motionBlur.enabled = pmv.motionBlur;
                settings.graphicsBox.postEffects.screenSpaceReflection.enabled = pmv.screenSpaceReflection;
                settings.graphicsBox.postEffects.userLut.enabled = pmv.userLut;
                settings.graphicsBox.postEffects.vignette.enabled = pmv.vignette;
                FindObjectOfType<VolumetricLightRenderer>().Resolution = pmv.VolumtericResolution;
                FindObjectOfType<VolumetricLightRenderer>().enabled = pmv.VolumetricLightRenderer_Enabled;
                Application.backgroundLoadingPriority = pmv.backgroundLoadingPriority;
                Application.runInBackground = pmv.runInBackground;
            }
        }

        [ConsoleCommand("noclip", "Toggle the flight camera.")]
        public static void noclip()
        {
            Network_Player ply = RAPI.GetLocalPlayer();
            if (ply != null && !ply.IsKilled && SceneManager.GetActiveScene().name == Raft_Network.GameSceneName)
            {
                ply.flightCamera.Toggle(true);
                return;
            }
            Debug.LogWarning("You can't use noclip right now.");
        }

        [ConsoleCommand("gotoraft", "Teleports you to your raft.")]
        static void gotoraft()
        {
            Network_Player ply = RAPI.GetLocalPlayer();
            if (ply != null && !ply.IsKilled && SceneManager.GetActiveScene().name == Raft_Network.GameSceneName)
            {
                ply.PersonController.CameraSubmersionChanged(UltimateWater.SubmersionState.None, true);
                ply.PersonController.SwitchControllerType(ControllerType.Ground);
                ply.SetToWalkableBlockPosition();
                ply.PlayerScript.MakeUnStuck();
                return;
            }
            Debug.LogWarning("You can't teleport to your raft right now.");
        }

        [ConsoleCommand("togglecustomchat", "Toggle the custom chat. <color=red>(Misbehaving & Deprecated!)</color>")]
        public static void togglecustomchat()
        {
            RChat.ToggleCustomChat();
        }

        public static Dictionary<KeyCode, string> boundCommands = new Dictionary<KeyCode, string>();

        [ConsoleCommand("bind", "Syntax: 'bind <key> <command...>' Bind a console command to a key.")]
        public static void BindCommand(string[] args)
        {
            if (args.Length < 2)
            {
                Debug.LogWarning("You must specify a key and a command as arguments to 'bind'.");
                return;
            }

            KeyCode key = HUtils.KeyCodeFromString(args[0]);
            if (key == KeyCode.None) { return; }
            string command = string.Join(" ", args.Skip(1).ToArray());
            boundCommands[key] = command;
            UpdateBoundCommandsFile();
        }

        [ConsoleCommand("unbind", "Syntax: 'unbind <key>' Unbind a console command from a key.")]
        public static void UnbindCommand(string[] args)
        {
            if (args.Length < 1)
            {
                Debug.LogWarning("Command 'unbind' only takes 1 argument.");
                return;
            }

            KeyCode key = HUtils.KeyCodeFromString(args[0]);
            if (key == KeyCode.None) { return; }
            boundCommands.Remove(key);
            UpdateBoundCommandsFile();
        }

        [ConsoleCommand("unbindall", "Unbind all keys.")]
        public static void Unbindall()
        {
            boundCommands.Clear();
            UpdateBoundCommandsFile();
        }

        public static void LoadBoundCommands()
        {
            if (PlayerPrefs.HasKey("rml4.boundcommands"))
            {
                JSONObject j = new JSONObject(PlayerPrefs.GetString("rml4.boundcommands"));
                if (j.IsArray && j.list.Count >= 1)
                {
                    foreach (JSONObject obj in j.list)
                    {
                        try
                        {
                            string[] args = obj.str.Split(new[] { "•◆•◆•" }, StringSplitOptions.None);
                            if (args.Length < 2)
                            {
                                Debug.LogWarning("You must specify a key and a command as arguments to 'bind'.");
                                return;
                            }

                            KeyCode key = HUtils.KeyCodeFromString(args[0]);
                            if (key == KeyCode.None) { return; }
                            string command = string.Join(" ", args.Skip(1).ToArray());
                            boundCommands[key] = command;
                            UpdateBoundCommandsFile();
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                    Debug.Log("Successfully loaded bound commands.");
                }
            }
        }

        public static void UpdateBoundCommandsFile()
        {
            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RaftModLoader");
            JSONObject j = new JSONObject(JSONObject.Type.ARRAY);
            foreach (KeyValuePair<KeyCode, string> v in boundCommands)
            {
                j.Add(v.Key.ToString() + "•◆•◆•" + v.Value);
            }
            PlayerPrefs.SetString("rml4.boundcommands", j.Print());
        }
    }

    class PoopModeVariables
    {
        public bool IsEnabled;

        public int pixelLightCount;
        public int masterTextureLimit;
        public AnisotropicFiltering anisotropicFiltering;
        public int antiAliasing;
        public bool softParticles;
        public bool realtimeReflectionProbes;
        public bool billboardsFaceCameraPosition;
        public int shadowCascades;
        public ShadowQuality shadows;
        public bool softVegetation;
        public int vSyncCount;
        public float lodBias;
        public int maximumLODLevel;
        public int WaterQualityLevel;
        public float shadowDistance;
        public int maxQueuedFrames;
        public bool ambientOcclusion;
        public bool antialiasing;
        public bool bloom;
        public bool chromaticAberration;
        public bool colorGrading;
        public bool debugViews;
        public bool depthOfField;
        public bool dithering;
        public bool eyeAdaptation;
        public bool fog;
        public bool grain;
        public bool motionBlur;
        public bool screenSpaceReflection;
        public bool userLut;
        public bool vignette;
        public VolumetricLightRenderer.VolumtericResolution VolumtericResolution;
        public bool VolumetricLightRenderer_Enabled;
        public ThreadPriority backgroundLoadingPriority;
        public bool runInBackground;

        public void SaveCurrentVariables()
        {
            pixelLightCount = QualitySettings.pixelLightCount;
            masterTextureLimit = QualitySettings.masterTextureLimit;
            anisotropicFiltering = QualitySettings.anisotropicFiltering;
            antiAliasing = QualitySettings.antiAliasing;
            softParticles = QualitySettings.softParticles;
            realtimeReflectionProbes = QualitySettings.realtimeReflectionProbes;
            billboardsFaceCameraPosition = QualitySettings.billboardsFaceCameraPosition;
            shadowCascades = QualitySettings.shadowCascades;
            shadows = QualitySettings.shadows;
            softVegetation = QualitySettings.softVegetation;
            vSyncCount = QualitySettings.vSyncCount;
            lodBias = QualitySettings.lodBias;
            maximumLODLevel = QualitySettings.maximumLODLevel;
            WaterQualityLevel = WaterQualitySettings.Instance.GetQualityLevel();
            shadowDistance = QualitySettings.shadowDistance;
            maxQueuedFrames = QualitySettings.maxQueuedFrames;
            Settings settings = UnityEngine.GameObject.FindObjectOfType<Settings>();
            ambientOcclusion = settings.graphicsBox.postEffects.ambientOcclusion.enabled;
            antialiasing = settings.graphicsBox.postEffects.antialiasing.enabled;
            bloom = settings.graphicsBox.postEffects.bloom.enabled;
            chromaticAberration = settings.graphicsBox.postEffects.chromaticAberration.enabled;
            colorGrading = settings.graphicsBox.postEffects.colorGrading.enabled;
            debugViews = settings.graphicsBox.postEffects.debugViews.enabled;
            depthOfField = settings.graphicsBox.postEffects.depthOfField.enabled;
            dithering = settings.graphicsBox.postEffects.dithering.enabled;
            eyeAdaptation = settings.graphicsBox.postEffects.eyeAdaptation.enabled;
            fog = settings.graphicsBox.postEffects.fog.enabled;
            grain = settings.graphicsBox.postEffects.grain.enabled;
            motionBlur = settings.graphicsBox.postEffects.motionBlur.enabled;
            screenSpaceReflection = settings.graphicsBox.postEffects.screenSpaceReflection.enabled;
            userLut = settings.graphicsBox.postEffects.userLut.enabled;
            vignette = settings.graphicsBox.postEffects.vignette.enabled;
            VolumtericResolution = UnityEngine.GameObject.FindObjectOfType<VolumetricLightRenderer>().Resolution;
            VolumetricLightRenderer_Enabled = UnityEngine.GameObject.FindObjectOfType<VolumetricLightRenderer>().enabled;
            backgroundLoadingPriority = Application.backgroundLoadingPriority;
            runInBackground = Application.runInBackground;
        }
    }
}