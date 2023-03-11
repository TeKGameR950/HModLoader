using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Launcher.Forms
{
    public partial class GameTileForm : Form
    {
        public Game game;
        public GameTileForm(Game game)
        {
            this.game = game;
            InitializeComponent();
            _settingsbtn.MouseDown += (a, b) => OpenSettings();
            _settingsbtnicon.MouseDown += (a, b) => OpenSettings();
            _banner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
        }

        public void OpenSettings()
        {
            MainForm.Get().ToggleGameSettings(game);
        }

        public void PlayBtnVisualUpdate()
        {
            if (!game.data.installed)
            {
                _playbtn.BackColor = HColors.BTN_BLUE;
                _playbtnicon.BackgroundImage = Properties.Resources.download_solid;
                _playbtn.InitControlUtils();
                return;
            }

            bool newVersionAvailable = false;
            if (game.versions.ContainsKey(ConfigManager.config.UpdateBranch))
            {
                GameVersion latestVersion = game.versions[ConfigManager.config.UpdateBranch];
                if (latestVersion.version != game.data.version)
                {
                    newVersionAvailable = true;
                    _playbtnicon.BackgroundImage = Properties.Resources.download_solid;
                    _playbtn.BackColor = HColors.BTN_PURPLE;
                    _playbtn.InitControlUtils();
                }
            }
            bool running = game.variables.gameProcesses.Any(x => x != null && !x.HasExited);
            if (!newVersionAvailable)
            {
                _playbtnicon.BackgroundImage = Properties.Resources.play_solid;
                _playbtn.BackColor = HColors.BTN_GREEN;
                _playbtn.InitControlUtils();

                if (ConfigManager.config.AllowMultipleGameInstances) { return; }
                if (running)
                {
                    _playbtn.BackColor = HColors.BTN_RED;
                    _playbtnicon.BackColor = HColors.BTN_RED;
                    _playbtnicon.BackgroundImage = Properties.Resources.stop_solid;
                    _playbtn.InitControlUtils();
                }
            }
            
        }

        public void VisualUpdate()
        {
            Visible = (MainForm.currentTab != "fav") || ConfigManager.data.favoritedGames.Contains(game.uniquename);
            _errormsg.Visible = false;
            HUtils.LoadGameBanner(game, _banner, "banner_vertical.png", _errormsg);

            if (game.data.installed)
            {
                _versionbg.Size = new Size(60, _versionbg.Height);
                _versionbg.Location = new Point(Width - 65, _versionbg.Location.Y);
                _versionlabel.Text = game.data.version;

                _playbtnicon.BackgroundImage = Properties.Resources.play_solid;
                _playbtn.BackColor = HColors.BTN_GREEN;
                GameManager.OnGameRunningTick(game);
            }

            _settingsbtn.Visible = _settingsbtn.Enabled = _versionbg.Visible = game.data.installed;

            if (ConfigManager.data.favoritedGames.Contains(game.uniquename))
            {
                _favbtnicon.BackgroundImage = Properties.Resources.star2;
                _favbtnicon.InitControlUtils();
            }

            PlayBtnVisualUpdate();
            this.InitFormUtils();
        }
    }
}
