using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;

namespace Launcher.Pages
{
    public partial class GameSettingsForm : PageForm
    {
        public Game currentGame;

        public GameSettingsForm()
        {
            InitializeComponent();
            _executablebtn.MouseDown += (a, b) => ToggleStartMethod("executable");
            _executablelabel.MouseDown += (a, b) => ToggleStartMethod("executable");
            _steambtn.MouseDown += (a, b) => ToggleStartMethod("steam");
            _steamlabel.MouseDown += (a, b) => ToggleStartMethod("steam");
            _uninstallbtn.MouseDown += (a, b) => UninstallGame();
            _uninstallbtnlabel.MouseDown += (a, b) => UninstallGame();
            _modsfolderbtn.MouseDown += (a, b) => OpenModsFolder();
            _modsfolderlabel.MouseDown += (a, b) => OpenModsFolder();
            _modcreatorbtn.MouseDown += (a, b) => OpenModCreator();
            _modcreatorlabel.MouseDown += (a, b) => OpenModCreator();
        }

        public void OpenModCreator()
        {

        }

        public void OpenModsFolder()
        {
            if (HUtils.IsGameFolderValid(currentGame.data.gamepath, currentGame))
            {
                string modsPath = Path.Combine(currentGame.data.gamepath, "mods");
                if (!Directory.Exists(modsPath))
                    Directory.CreateDirectory(modsPath);
                Process.Start(modsPath);
            }
        }

        public void OnSettingsOpen()
        {
            // Here disable settings according to current game.
            _uninstallbtnlabel.Text = $"Uninstall {currentGame.prefix}ML";
            _version.Text = $"Installed version : {currentGame.name.Replace(" ", "")}ModLoader {currentGame.data.version}";
            UpdateGameSettings();
        }

        public void ToggleStartMethod(string name)
        {
            currentGame.data.startingMethod = name;
            GameManager.SaveGameData(currentGame);
            UpdateGameSettings();
        }

        public void UpdateGameSettings()
        {
            switch (currentGame.data.startingMethod)
            {
                case "executable":
                    _executablebtn.InitControlUtilsRecursive(HColors.BTN_BLUE);
                    _steambtn.InitControlUtilsRecursive(HColors.BTN_GREY);
                    break;
                case "steam":
                    _executablebtn.InitControlUtilsRecursive(HColors.BTN_GREY);
                    _steambtn.InitControlUtilsRecursive(HColors.BTN_BLUE);
                    break;
            }
        }

        public void UninstallGame()
        {
            if (HUtils.ShowPopup($"Confirm {currentGame.prefix}ML Uninstallation", $"Are you sure you want to uninstall the Mod Loader for {currentGame.name}?\nThis will disable all mods and remove the Mod Loader from your game.\n\nClick \"Yes\" to uninstall or \"No\" to cancel.", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                ProceedGameUninstallation();
            }
        }

        public void ProceedGameUninstallation()
        {
            foreach (string directory in currentGame.data.installedFolders)
            {
                try
                {
                    if (Directory.Exists(directory))
                    {
                        Directory.Delete(directory, true);
                    }
                }
                catch { }
            }
            foreach (string file in currentGame.data.installedFiles)
            {
                try
                {
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                    }
                }
                catch { }
            }
            currentGame.data = new InstalledGameData();
            GameManager.SaveGameData(currentGame);
            MainForm.Get().ToggleGameSettings(null);
            GameManager.DisplayGames();
        }
    }
}
