using ICSharpCode.SharpZipLib.Zip;
using Launcher.Forms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Timer = System.Windows.Forms.Timer;
using System.Security.Policy;

namespace Launcher
{
    public static class GameManager
    {
        public static List<Game> SupportedGames = null;

        public static async Task RetrieveSupportedGamesFromAPI()
        {
            Notification notif = NotificationManager.CreateNotification("Retrieving games...", NotificationType.Spinner);
            string cached = Path.Combine(FileManager.folderCache, "api_games.json");
            WebResult r = await HUtils.DoWebRequest(MainForm.APIROUTE_GAMES);
            if (r.success)
            {
                try
                {
                    SupportedGames = JsonConvert.DeserializeObject<List<Game>>(r.result);
                    File.WriteAllText(cached, JsonConvert.SerializeObject(SupportedGames, Formatting.Indented));
                }
                catch { }
            }

            // API Failed, Try to use cached version.
            if (SupportedGames == null && File.Exists(cached))
            {
                try
                {
                    SupportedGames = JsonConvert.DeserializeObject<List<Game>>(File.ReadAllText(cached));
                    Notification warning = NotificationManager.CreateNotification("The supported games could not be retrieved ! The launcher will use the cached version.", NotificationType.Warning).AutoClose();
                }
                catch { }
            }

            // API & Cache failed, Nothing can be done.
            if (SupportedGames == null)
            {
                MainForm.Get().DisplayFatalError(Localization.NOGAMES);
                Notification error = NotificationManager.CreateNotification("Error: Supported games list not available.\nTry again later or contact support.\nClick here for more info.", NotificationType.Error);
                error.SetClickAction(() =>
                {
                    // TODO : Proper contact form.
                });
                return;
            }

            LoadGamesData(SupportedGames);


            notif.Close();
            DisplayGames();
        }

        public static void ToggleFavorite(this GameTileForm form, string uniquename)
        {
            if (ConfigManager.data.favoritedGames.Contains(uniquename))
                ConfigManager.data.favoritedGames.Remove(uniquename);
            else
                ConfigManager.data.favoritedGames.Add(uniquename);

            bool favorited = ConfigManager.data.favoritedGames.Contains(uniquename);
            form._favbtnicon.BackgroundImage = favorited ? Properties.Resources.star2 : Properties.Resources.star1;
            form._favbtnicon.InitControlUtils();
            if (!favorited)
            {
                if (MainForm.currentTab == "fav")
                    form.Visible = form.Enabled = false;

            }

            ConfigManager.SaveData();
        }

        public static void LoadGamesData(List<Game> games)
        {
            games.ForEach(game =>
            {
                try
                {
                    string dataFile = Path.Combine(FileManager.folderData_games, game.uniquename, "data.json");
                    game.data = JsonConvert.DeserializeObject<InstalledGameData>(File.ReadAllText(dataFile));
                }
                catch { game.data = new InstalledGameData(); }
            });
        }

        public static async void DisplayGames()
        {
            FlowLayoutPanel gameTiles = MainForm.Get()._gametiles;

            // First refresh.
            if (gameTiles.Controls.Count == 0)
            {
                GameManager.SupportedGames.OrderByDescending(o => o.data.lastPlay).ToList().ForEach(async game =>
                {
                    // MOVE SOMEWHERE ELSE
                    string gameCacheFolder = Path.Combine(FileManager.folderCache_games, game.uniquename);
                    string gameDataFolder = Path.Combine(FileManager.folderData_games, game.uniquename);
                    if (!Directory.Exists(gameCacheFolder))
                        Directory.CreateDirectory(gameCacheFolder);
                    if (!Directory.Exists(gameDataFolder))
                        Directory.CreateDirectory(gameDataFolder);


                    GameTileForm tile = new GameTileForm(game);
                    game.variables.tile = tile;
                    tile.RoundCorners();
                    tile._playbtn.MouseDown += (a, b) => OnPlayPressed(game.uniquename);
                    tile._playbtnicon.MouseDown += (a, b) => OnPlayPressed(game.uniquename);
                    tile._favbtn.MouseDown += (a, b) => tile.ToggleFavorite(game.uniquename);
                    tile._favbtnicon.MouseDown += (a, b) => tile.ToggleFavorite(game.uniquename);

                    tile.TopLevel = false;
                    tile.AutoScroll = true;

                    gameTiles.Controls.Add(tile);
                    tile.Show();
                    if (game.data.installed && game.versions.ContainsKey(ConfigManager.config.UpdateBranch))
                    {
                        GameVersion latestVersion = game.versions[ConfigManager.config.UpdateBranch];
                        if (latestVersion.version != game.data.version)
                            NotificationManager.CreateNotification($"A new mod loader update for {game.name} is available !", NotificationType.Info).AutoClose();
                    }

                    game.variables.runningTimer = new Timer();
                    game.variables.runningTimer.Interval = 1000;
                    game.variables.runningTimer.Tick += (a, b) =>
                    {
                        OnGameRunningTick(game);
                    };
                    game.variables.runningTimer.Start();
                    tile.PlayBtnVisualUpdate();
                });
            }

            // Refresh.

            foreach (Control ctrl in gameTiles.Controls)
            {
                GameTileForm gameTile = ctrl as GameTileForm;
                if (gameTile != null)
                    gameTile.VisualUpdate();
            }
        }

        public static void OnGameRunningTick(Game game)
        {
            game.variables.tile.PlayBtnVisualUpdate();
        }

        public static async void OnPlayPressed(string gamename)
        {
            Notification notif = null;
            Game game = SupportedGames.Find(x => x.uniquename == gamename);
            if (game != null && game.data != null)
            {
                if (game.variables.installing) return;
                if (!game.data.installed)
                {
                    try
                    {
                        game.variables.installing = true;
                        string gamePath = HUtils.FindGameFolder(game);
                        if (gamePath == null)
                        {
                            NotificationManager.CreateNotification($"You need to select your {game.name} folder in order to install {game.prefix}ML !", NotificationType.Warning).AutoClose();
                            return;
                        }
                        game.data.gamepath = gamePath;
                        SaveGameData(game);
                        BeginGameInstallation_Step1(game);

                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        notif?.Close();
                        game.variables.installing = false;
                    }
                }
                else
                {
                    // Check for game updates
                    if (game.versions.ContainsKey(ConfigManager.config.UpdateBranch))
                    {
                        GameVersion latestVersion = game.versions[ConfigManager.config.UpdateBranch];
                        if (latestVersion.version != game.data.version)
                        {
                            // TODO : Show popup for install instead of starting straight away.
                            BeginGameInstallation_Step1(game);
                            return;
                        }
                    }

                    // Launch Game
                    // TODO : Add steam support to game settings.

                    bool firstRunWarning = false;
                    string gameDataFolder = Path.Combine(FileManager.folderData_games, game.uniquename);
                    File.WriteAllText(Path.Combine(gameDataFolder, "pid.txt"), Process.GetCurrentProcess().Id.ToString());
                    if (game.runtime == "IL2CPP")
                    {
                        if (!Directory.Exists(Path.Combine(gameDataFolder, "interop")) || !Directory.Exists(Path.Combine(gameDataFolder, "unity-libs")))
                            firstRunWarning = true;
                        if (Directory.Exists(Path.Combine(gameDataFolder, "interop")) && Directory.GetFiles(Path.Combine(gameDataFolder, "interop")).Length < 20)
                            firstRunWarning = true;
                    }
                    string path = Path.Combine(game.data.gamepath, game.executable);
                    Process gameProcess = null;
                    if (game.data.startingMethod == "executable" || game.appid == "0")
                    {
                        gameProcess = Process.Start(path);
                    }
                    else if (game.data.startingMethod == "steam")
                    {
                        gameProcess = Process.Start($"steam://run/{game.appid}");
                    }

                    MessageBox.Show(gameProcess.HasExited.ToString());
                    //game.data.gameProcesses.Add(gameProcess);
                    Notification aa = NotificationManager.CreateNotification($"process alive", NotificationType.Spinner).SetIcon(Properties.Resources.folder_solid);
                    while (!gameProcess.HasExited)
                    {
                        await Task.Delay(500);
                        MessageBox.Show(gameProcess.HasExited.ToString());
                    }
                    aa?.Close();
                    if (firstRunWarning)
                    {
                        Notification gen_notif = NotificationManager.CreateNotification($"{game.prefix}ML is generating files for the first launch. Please be patient.", NotificationType.Spinner).SetIcon(Properties.Resources.folder_solid);
                        HUtils.ShowPopup("Files Generation In Progress", $"{game.prefix}ML is generating files for the first launch.\nThis process may take a few seconds or minutes to complete, depending on the speed of your computer.\n\nPlease wait while the necessary files are being created. The game will launch automatically once the process is finished.\nThank you for your patience.", MessageBoxButtons.OK);
                        bool done = false;
                        string interopPath = Path.Combine(gameDataFolder, "interop");
                        while (gameProcess != null && !gameProcess.HasExited && !done && gen_notif != null && !gen_notif.closed)
                        {
                            int count = Directory.Exists(interopPath) ? Directory.GetFiles(interopPath).Length : 0;
                            if (count > 20)
                                done = true;
                            await Task.Delay(2000);
                        }
                        gen_notif?.Close();
                        if ((Directory.Exists(interopPath) ? Directory.GetFiles(interopPath).Length : 0) > 20)
                        {
                            NotificationManager.CreateNotification($"{game.prefix}ML has successfully generated all the necessary files!", NotificationType.Success).AutoClose();
                        }
                    }
                }
            }
        }

        // INSTALLATION STEP 1
        public async static void BeginGameInstallation_Step1(Game requestedGame)
        {

            Notification notif = NotificationManager.CreateNotification($"Beginning {requestedGame.prefix}ML installation...", NotificationType.Spinner);
            try
            {
                WebResult r = await HUtils.DoWebRequest(MainForm.APIROUTE_GAMES);
                string cached = Path.Combine(FileManager.folderCache, "api_games.json");
                if (r.success)
                {
                    try
                    {
                        List<Game> games = JsonConvert.DeserializeObject<List<Game>>(r.result);
                        LoadGamesData(games);
                        File.WriteAllText(cached, JsonConvert.SerializeObject(games, Formatting.Indented));
                        Game game = games.Find(x => x.uniquename == requestedGame.uniquename);
                        if (game != null)
                        {
                            if (!game.versions.ContainsKey(ConfigManager.config.UpdateBranch))
                            {
                                AbortGameInstallation(requestedGame.uniquename);
                                notif?.Close();
                                game.variables.installing = false;
                                notif = NotificationManager.CreateNotification("Could not find an available version for the specified game !\nInstallation aborted at step 1 !", NotificationType.Warning).AutoClose();
                                return;
                            }
                            GameVersion version = game.versions[ConfigManager.config.UpdateBranch];
                            notif?.Close();
                            DownloadGameVersion_Step2(game, version);
                            return;
                        }
                    }
                    catch { }
                }
                AbortGameInstallation(requestedGame.uniquename);
                notif?.Close();
                notif = NotificationManager.CreateNotification("Could not retrieve the specified game data !\nInstallation aborted at step 1 !", NotificationType.Warning).AutoClose();
                return;
            }
            catch
            {
                AbortGameInstallation(requestedGame.uniquename);
                notif?.Close();
                notif = NotificationManager.CreateNotification($"Could not begin the ${requestedGame.prefix}ML installation !\nInstallation aborted at step 1 !", NotificationType.Warning).AutoClose();
                return;
            }
        }

        // INSTALLATION STEP 2
        public async static void DownloadGameVersion_Step2(Game game, GameVersion version)
        {
            Notification notif = NotificationManager.CreateNotification($"Downloading {game.prefix}ML...", NotificationType.Download);
            notif.ToggleCloseBtn(false);
            try
            {
                string downloadPath = version.downloadpath;
                string diskPath = Path.Combine(FileManager.folderCache_installfiles, game.uniquename + ".hmlcore");

                // Proceed to download of the .hmlcore file, read install script and process.
                WebClient coreClient = new WebClient();
                coreClient.DownloadProgressChanged += (a, b) =>
                {
                    double bytesIn = double.Parse(b.BytesReceived.ToString());
                    double totalBytes = double.Parse(b.TotalBytesToReceive.ToString());
                    double percentage = bytesIn / totalBytes * 100;

                    notif?.SetProgress((int)percentage);
                };
                coreClient.DownloadFileCompleted += async (a, b) =>
                {
                    notif?.Close();
                    InstallGameVersion_Step3(game, version, diskPath);
                };
                coreClient.DownloadFileAsync(new Uri(downloadPath), diskPath);
            }
            catch
            {
                notif?.Close();
                AbortGameInstallation(game.uniquename);
                notif = NotificationManager.CreateNotification($"The latest {game.prefix}ML version could not be downloaded !\nInstallation aborted at step 2 !", NotificationType.Error).AutoClose(15);
                return;
            }
        }

        public static void MainThreadProgressUpdate(Notification notif, int totalFiles, int processedFiles)
        {
            int remainingFiles = totalFiles - processedFiles;
            notif.SetProgress((int)(((float)processedFiles / totalFiles * 50)));
        }

        // INSTALLATION STEP 3
        public async static void InstallGameVersion_Step3(Game game, GameVersion version, string corefileDiskPath)
        {
            Notification notif = NotificationManager.CreateNotification($"Installing {game.prefix}ML...", NotificationType.Download).SetIcon(Properties.Resources.folder_solid);
            notif.ToggleCloseBtn(false);
            try
            {
                Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();
                int totalFiles = GetNumberOfFilesInZip(corefileDiskPath);
                try
                {
                    InstallScriptData script = null;
                    bool completed = false;
                    int processedFiles = 0;
                    Thread zipThread = new Thread(new ThreadStart(async () =>
                    {
                        Thread.CurrentThread.IsBackground = true;
                        using (Stream stream = File.OpenRead(corefileDiskPath))
                        {
                            using (var zipInputStream = new ZipInputStream(stream))
                            {
                                while (zipInputStream.GetNextEntry() is ZipEntry v)
                                {
                                    var zipentry = v.Name;
                                    StreamReader reader = new StreamReader(zipInputStream);

                                    var bytes = default(byte[]);
                                    using (var memstream = new MemoryStream())
                                    {
                                        reader.BaseStream.CopyTo(memstream);
                                        bytes = memstream.ToArray();
                                    }
                                    files.Add(v.Name, bytes);
                                    processedFiles++;
                                    MainForm.Get().Invoke(new Action(() => GameManager.MainThreadProgressUpdate(notif, totalFiles, processedFiles)));
                                }
                            }
                            stream.Close();
                        }
                        completed = true;
                    }));
                    zipThread.Start();
                    while (!completed)
                        await Task.Delay(1);
                    zipThread.Abort();
                    try
                    {
                        if (!files.ContainsKey("install.json")) throw new FileNotFoundException();
                        script = JsonConvert.DeserializeObject<InstallScriptData>(Encoding.UTF8.GetString(files["install.json"]));
                    }
                    catch
                    {
                        notif?.Close();
                        AbortGameInstallation(game.uniquename);
                        notif = NotificationManager.CreateNotification($"Could not read the {game.prefix}ML install script !\nInstallation aborted at step 3 !", NotificationType.Warning).AutoClose();
                        return;
                    }
                    List<string> failedActions = new List<string>();
                    int extractedFiles = 0;
                    game.data.installedFiles = new List<string>();
                    game.data.installedFolders = new List<string>();
                    // Deleting files.
                    foreach (string dummyFile in script.fileDeletions)
                    {
                        try
                        {
                            string validFile = ReplaceInstallVariables(dummyFile, game);
                            if (File.Exists(validFile))
                                File.Delete(validFile);
                        }
                        catch { failedActions.Add($"Deleting file \"{dummyFile}\""); }
                    }
                    // Editing files.
                    foreach (KeyValuePair<string, Dictionary<string, string>> dummyKvp in script.fileEdits)
                    {
                        try
                        {
                            if (files.ContainsKey(dummyKvp.Key))
                            {
                                byte[] fileBytes = files[dummyKvp.Key];
                                string fileContent = Encoding.UTF8.GetString(fileBytes);
                                foreach (KeyValuePair<string, string> s in dummyKvp.Value)
                                {
                                    fileContent = fileContent.Replace(s.Key, ReplaceInstallVariables(s.Value, game));
                                }

                                files[dummyKvp.Key] = Encoding.UTF8.GetBytes(fileContent);
                            }
                        }
                        catch { failedActions.Add($"Editing file \"{dummyKvp.Key}\""); }
                    }
                    // Extracting folders.
                    foreach (KeyValuePair<string, string> dummyKvp in script.dirExtract)
                    {
                        try
                        {
                            string zipFolderName = dummyKvp.Key;
                            string validFolderPath = ReplaceInstallVariables(dummyKvp.Value, game);
                            validFolderPath = validFolderPath.Substring(0, validFolderPath.Length - zipFolderName.Length);
                            game.data.installedFolders.Add(validFolderPath);
                            foreach (KeyValuePair<string, byte[]> file in files)
                            {
                                if (file.Key.StartsWith($"{dummyKvp.Key}/"))
                                {
                                    string dir = Path.GetDirectoryName(file.Key);
                                    Directory.CreateDirectory(Path.Combine(validFolderPath, dir));
                                    File.WriteAllBytes(Path.Combine(Path.Combine(validFolderPath, file.Key)), file.Value);
                                    extractedFiles++;
                                    notif.SetProgress(50 + (int)(((float)extractedFiles / totalFiles * 50)));
                                }
                            }
                        }
                        catch { failedActions.Add($"Extracting folder \"{dummyKvp.Value}\""); }
                    }
                    // Extracting files.
                    foreach (KeyValuePair<string, string> dummyKvp in script.fileExtract)
                    {
                        try
                        {
                            string zipFileName = dummyKvp.Key;
                            if (files.ContainsKey(zipFileName))
                            {
                                byte[] fileBytes = files[zipFileName];
                                string validFilePath = ReplaceInstallVariables(dummyKvp.Value, game);
                                game.data.installedFiles.Add(validFilePath);

                                string dir = Path.GetDirectoryName(validFilePath);
                                if (!Directory.Exists(dir))
                                    Directory.CreateDirectory(dir);
                                File.WriteAllBytes(validFilePath, fileBytes);
                                extractedFiles++;
                                notif.SetProgress(50 + (int)(((float)extractedFiles / totalFiles * 50)));
                            }
                        }
                        catch { failedActions.Add($"Extracting file \"{dummyKvp.Value}\""); }
                    }
                    notif?.Close();
                    if (failedActions.Count > 0)
                    {
                        HUtils.ShowPopup($"{game.prefix}ML Installation Failed", $"The following actions failed during the installation of {game.prefix}ML :\n - " + string.Join("\n - ", failedActions), MessageBoxButtons.OK);
                    }
                    else
                    {
                        notif = NotificationManager.CreateNotification($"{game.prefix}ML has been successfully installed !", NotificationType.Success).AutoClose(5);
                        game.data.version = version.version;
                        game.data.installed = true;
                        game.variables.installing = false;
                        SaveGameData(game);
                        LoadGamesData(SupportedGames);
                        DisplayGames();
                    }
                }
                catch { }
            }
            catch
            {
                notif = NotificationManager.CreateNotification($"The latest {game.prefix}ML version could not be installed !\nInstallation aborted at step 3 !", NotificationType.Error).AutoClose(15);
                return;
            }
            finally
            {
                //notif?.Close();
            }
        }

        public static string ReplaceInstallVariables(string dummy, Game game)
        {
            dummy = dummy.Replace("$GAME_APPDATA$", Path.Combine(FileManager.folderData_games, game.uniquename));
            dummy = dummy.Replace("$GAME_ROOTFOLDER$", game.data.gamepath);
            return dummy;
        }

        public static void SaveGameData(Game game)
        {
            string dataPath = Path.Combine(FileManager.folderData_games, game.uniquename);
            if (!Directory.Exists(dataPath))
                Directory.CreateDirectory(dataPath);
            File.WriteAllText(Path.Combine(dataPath, "data.json"), JsonConvert.SerializeObject(game.data, Formatting.Indented));
        }

        public static int GetNumberOfFilesInZip(string zipFilePath)
        {
            int count = 0;
            using (ZipFile zip = new ZipFile(zipFilePath))
            {
                count = (int)zip.Count;
            }
            return count;
        }

        public static void AbortGameInstallation(string name) => SupportedGames.Find(x => x.uniquename == name).variables.installing = false;
    }

    public class InstallScriptData
    {
        [JsonProperty("Files to delete")]
        public List<string> fileDeletions = new List<string>();
        [JsonProperty("Files to extract")]
        public Dictionary<string, string> fileExtract = new Dictionary<string, string>();
        [JsonProperty("Directories to extract")]
        public Dictionary<string, string> dirExtract = new Dictionary<string, string>();
        [JsonProperty("Files to edit")]
        public Dictionary<string, Dictionary<string, string>> fileEdits = new Dictionary<string, Dictionary<string, string>>();
    }

    public class InstalledGameData
    {
        public bool installed;

        public DateTime lastPlay;
        public string version;
        public string gamepath;
        public string startingMethod = "executable";
        public List<string> installedFiles = new List<string>();
        public List<string> installedFolders = new List<string>();
    }

    public class Game
    {
        public string name;
        public string shortname;
        public string prefix;
        public string appid;
        public string runtime;
        public string uniquename;
        public string executable;
        public bool maintenance;
        public Dictionary<string, GameVersion> versions = new Dictionary<string, GameVersion>();
        public string images;

        [JsonIgnore]
        public InstalledGameData data;

        [JsonIgnore]
        public GameVariables variables = new GameVariables();
    }

    public class GameVariables
    {
        public bool installing;
        public Timer runningTimer;
        public List<Process> gameProcesses = new List<Process>();
        public GameTileForm tile;
    }

    public class GameVersion
    {
        public string version;
        public string downloadpath;
        public string date;
        public string changelogpath;
    }

    public enum GameRuntime
    {
        IL2CPP,
        Mono
    }
}
