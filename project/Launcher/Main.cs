using Launcher.Forms;
using Launcher.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;

namespace Launcher
{
    public partial class MainForm : Form
    {
        public static string APIROUTE_MAIN = "https://fastdl.hmodding.com/api";
        public static string APIROUTE_GAMES = APIROUTE_MAIN + "/games";

        public static string currentTab = "";
        public static Dictionary<string, Control> tabs;
        public static Dictionary<string, PageForm> pages = new Dictionary<string, PageForm>();
        static MainForm instance;
        public static MainForm Get() => instance;
        public SettingsForm SettingsPage;
        public StatusForm StatusPage;
        public GameSettingsForm GameSettingsPage;

        public MainForm() => Init();

        public async void Init()
        {
            instance = this;
            FileManager.CreateDirectories();
            ConfigManager.Load();
            Opacity = 0;
            HUtils.LoadFont();
            InitializeComponent();
            tabs = new Dictionary<string, Control>()
            {
                { "games", _tabgamesselect },
                { "fav", _tabfavselect }
            };
            SwitchTab(ConfigManager.data.lastTab);
            _errormsg.Visible = false;
            _errormsg.Enabled = false;
            this.RoundCorners();
            this.InitFormUtils();

            LoadPages();
            GameManager.RetrieveSupportedGamesFromAPI();
            this.FadeIn(10);
            //DisplayFatalError("Ah !");
            /*
            NotificationManager.CreateNotification("This is a test", NotificationType.Spinner);
            NotificationManager.CreateNotification("This is a test", NotificationType.Info);
            NotificationManager.CreateNotification("This is a test", NotificationType.Info).ToggleCloseBtn(false);
            NotificationManager.CreateNotification("This is a test with some huge text............... Blabla.... Blabla.... Blabla.... Blabla.... Blabla.... Blabla", NotificationType.Info);*/

            _tabfavselect.Width = 0;
            _tabfavselect.Location = new Point(_tabfavselect.Parent.Width / 2, _tabfavselect.Location.Y);
            _tabgamesselect.Width = 0;
            _tabgamesselect.Location = new Point(_tabgamesselect.Parent.Width / 2, _tabgamesselect.Location.Y);
            _tabgames.MouseDown += (a, b) => SwitchTab("games");
            _tabgamesicon.MouseDown += (a, b) => SwitchTab("games");
            _tabgameslabel.MouseDown += (a, b) => SwitchTab("games");
            _tabfav.MouseDown += (a, b) => SwitchTab("fav");
            _tabfavicon.MouseDown += (a, b) => SwitchTab("fav");
            _tabfavlabel.MouseDown += (a, b) => SwitchTab("fav");

            // TODO : Remember last page by writing into data.
        }

        public void SwitchTab(string name)
        {
            if (currentTab == name) return;
            tabs.ToList().ForEach(x =>
            {
                HUtils.SetTabSelected(x.Value, x.Key, name == x.Key);
                if (name == x.Key)
                    currentTab = name;

                if (GameManager.SupportedGames != null)
                    GameManager.DisplayGames();
            });
            ConfigManager.data.lastTab = name;
            ConfigManager.SaveData();
            // Handle tab redraw
        }

        public void DisplayFatalError(string error)
        {
            Controls.SetChildIndex(_errormsg, 0);
            _errormsg.Dock = DockStyle.Fill;
            _errormsg.Visible = true;
            _errormsg.Text = error;
            _errormsg.Enabled = true;
        }

        public void LoadPages()
        {
            // Settings
            SettingsPage = new SettingsForm();

            SettingsPage.TopLevel = false;
            SettingsPage.AutoScroll = true;
            Controls.Add(SettingsPage);
            SettingsPage.Location = new Point(880, 30);
            SettingsPage.Show();
            pages.Add("settings", SettingsPage);
            _btnsettings.MouseDown += (a, b) => TogglePage("settings");
            _btnsettingsicon.MouseDown += (a, b) => TogglePage("settings");
            SettingsPage.UpdateSettings();
            SettingsPage.InitFormUtils();
            // Status
            StatusPage = new StatusForm();
            StatusPage.InitFormUtils();
            StatusPage.TopLevel = false;
            StatusPage.AutoScroll = true;
            Controls.Add(StatusPage);
            StatusPage.Location = new Point(880, 30);
            StatusPage.Show();
            pages.Add("status", StatusPage);
            _btnstatus.MouseDown += (a, b) => TogglePage("status");
            _btnstatusicon.MouseDown += (a, b) => TogglePage("status");

            // Game Settings
            GameSettingsPage = new GameSettingsForm();
            GameSettingsPage.InitFormUtils();
            GameSettingsPage.TopLevel = false;
            GameSettingsPage.AutoScroll = true;
            Controls.Add(GameSettingsPage);
            GameSettingsPage.Location = new Point(880, 30);
            GameSettingsPage.Show();
            pages.Add("gamesettings", GameSettingsPage);

            _galleryBlocker.Size = new Size(735, 520);
            _galleryBlocker.BackColor = Color.Black;
            _galleryBlocker.Enabled = false;

            _galleryBlocker.MouseDown += (a, b) => TogglePage("");

        }

        public void TogglePage(string name, bool force = false, bool forcedValue = false)
        {
            pages.ToList().ForEach(pageKvp =>
            {
                if (pageKvp.Key == name)
                {
                    bool willShow = !pageKvp.Value.PageOpened;
                    if (force)
                        willShow = forcedValue;
                    if(willShow)
                        pageKvp.Value.OnPageOpened();
                    Controls.SetChildIndex(pageKvp.Value, 0);
                    HUtils.SlideRightPage(pageKvp.Value, willShow, 20);
                }
                else
                {
                    Controls.SetChildIndex(pageKvp.Value, 0);
                    HUtils.SlideRightPage(pageKvp.Value, false, 20);
                }
            });

            _galleryBlocker.Enabled = pages.Any(x => x.Value.PageOpened);
        }

        public void ToggleGameSettings(Game game)
        {
            GameSettingsPage.currentGame = game;
            if (GameSettingsPage.currentGame != null)
            {
                GameSettingsPage.OnSettingsOpen();
                TogglePage("gamesettings");
            }
            else
            {
                TogglePage("gamesettings", true, false);
            }
        }


        void _closebtn_Click(object s, EventArgs e) => this.CloseForm(10);
        void _minimizebtn_Click(object s, EventArgs e) => WindowState = FormWindowState.Minimized;
    }
}
