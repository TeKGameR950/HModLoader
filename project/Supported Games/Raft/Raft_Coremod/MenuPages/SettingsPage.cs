using HMLLibrary;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace RaftModLoader
{
    public class SettingsPage : MenuPage
    {
        public static SettingsPage instance;
        public static bool EnableMenuInGame;
        public static bool HidePatronsAnimation;
        Toggle ShowMenuOnStart;
        Toggle HidePatrons;
        public static GameObject rmlsetting_consolekeybind;
        public static GameObject rmlsetting_menukeybind;
        public static GameObject rmlsetting_mastervolume;

        public void Awake()
        {
            ShowMenuOnStart = GameObject.Find("RMLSetting_ShowMainMenuOnStartup").GetComponent<Toggle>();
            HidePatrons = GameObject.Find("RMLSetting_HidePatron").GetComponent<Toggle>();
            instance = this;

            rmlsetting_consolekeybind = GameObject.Find("RMLSetting_Consolekeybind");
            rmlsetting_menukeybind = GameObject.Find("RMLSetting_Menukeybind");

            rmlsetting_mastervolume = GameObject.Find("RMLSetting_MasterVolume");
            rmlsetting_mastervolume.GetComponentInChildren<Slider>().onValueChanged.AddListener(MasterVolumeChanged);
            if (PlayerPrefs.HasKey("rmlSettings_MasterVolume"))
            {
                float value = PlayerPrefs.GetFloat("rmlSettings_MasterVolume");
                GameObject.Find("RMLSetting_MasterVolume").GetComponentInChildren<Slider>().value = value;
            }
            else
            {
                PlayerPrefs.SetFloat("rmlSettings_MasterVolume", 1.0f);
                GameObject.Find("RMLSetting_MasterVolume").GetComponentInChildren<Slider>().value = 1.0f;
            }

            if (PlayerPrefs.HasKey("rmlSettings_StartInModManagerTab"))
            {
                if (PlayerPrefs.GetString("rmlSettings_StartInModManagerTab") == "True")
                {
                    GameObject.Find("RMLSetting_StartInModManagerTab").GetComponent<Toggle>().isOn = true;
                }
                else
                {
                    GameObject.Find("RMLSetting_StartInModManagerTab").GetComponent<Toggle>().isOn = false;
                }
            }
            else
            {
                PlayerPrefs.SetString("rmlSettings_StartInModManagerTab", "False");
            }
            if (PlayerPrefs.HasKey("rmlSettings_HidePatronsAnimation"))
            {
                if (PlayerPrefs.GetString("rmlSettings_HidePatronsAnimation") == "True")
                {
                    HidePatrons.isOn = true;
                    HidePatronsAnimation = true;
                }
                else
                {
                    HidePatrons.isOn = false;
                    HidePatronsAnimation = false;
                }
            }
            else
            {
                PlayerPrefs.SetString("rmlSettings_HidePatronsAnimation", "False");
                HidePatronsAnimation = false;
                HidePatrons.isOn = false;
            }
            if (CustomLoadingScreen.loadingscreen_banner != null)
            {
                foreach (Transform t in CustomLoadingScreen.loadingscreen_banner.transform.Find("PatronsBar").Find("PatronsRow"))
                {
                    List<Animator> animations = t.gameObject.GetComponentsInChildren<Animator>().ToList();
                    animations.ForEach(anim =>
                    {
                        anim.enabled = HidePatronsAnimation ? false : anim.GetComponent<PatronData>().ShouldBeOn;
                    });

                }
            }

            if (PlayerPrefs.HasKey("rmlSettings_EnableThisMenuIngame"))
            {
                if (PlayerPrefs.GetString("rmlSettings_EnableThisMenuIngame") == "True")
                {
                    GameObject.Find("RMLSetting_EnableThisMenuIngame").GetComponent<Toggle>().isOn = true;
                    EnableMenuInGame = true;
                }
                else
                {
                    GameObject.Find("RMLSetting_EnableThisMenuIngame").GetComponent<Toggle>().isOn = false;
                    EnableMenuInGame = false;
                }
            }
            else
            {
                PlayerPrefs.SetString("rmlSettings_EnableThisMenuIngame", "True");
                EnableMenuInGame = true;
            }

            if (PlayerPrefs.HasKey("rmlSettings_UserPermanentMods"))
            {
                try
                {
                    ModManagerPage.OnStartUserPermanentMods = JsonConvert.DeserializeObject<PermanentModsList>(PlayerPrefs.GetString("rmlSettings_UserPermanentMods"));
                }
                catch
                {
                    ModManagerPage.OnStartUserPermanentMods = new PermanentModsList();
                    PlayerPrefs.SetString("rmlSettings_UserPermanentMods", JsonConvert.SerializeObject(ModManagerPage.OnStartUserPermanentMods));
                }
            }
            else
            {
                ModManagerPage.OnStartUserPermanentMods = new PermanentModsList();
                PlayerPrefs.SetString("rmlSettings_UserPermanentMods", JsonConvert.SerializeObject(ModManagerPage.OnStartUserPermanentMods));
            }
            ModManagerPage.UserPermanentMods = new PermanentModsList();
            ModManagerPage.UserPermanentMods.list = ModManagerPage.OnStartUserPermanentMods.list.ToArray().ToList();


            GameObject.Find("RMLSetting_StartInModManagerTab").GetComponent<Toggle>().onValueChanged.AddListener(var => PlayerPrefs.SetString("rmlSettings_StartInModManagerTab", var.ToString()));
            GameObject.Find("RMLSetting_EnableThisMenuIngame").GetComponent<Toggle>().onValueChanged.AddListener(var => ToggleEnableThisMenuIngame(var));
            GameObject.Find("RMLSetting_ShowMainMenuOnStartup").GetComponent<Toggle>().onValueChanged.AddListener(var => PlayerPrefs.SetString("rmlSettings_ShowMainMenuOnStart", var.ToString()));

            HidePatrons.onValueChanged.AddListener(var => ToggleHidePatrons(var));

            if (GameObject.Find("RMLSetting_StartInModManagerTab").GetComponent<Toggle>().isOn)
            {
                MainMenu.ChangeMenu("ModManager");
            }
            else
            {
                MainMenu.ChangeMenu("Home");
            }

            if (PlayerPrefs.HasKey("rmlSettings_ShowMainMenuOnStart"))
            {
                if (PlayerPrefs.GetString("rmlSettings_ShowMainMenuOnStart") != "True")
                {
                    ShowMenuOnStart.isOn = false;
                    MainMenu.CloseMenu();
                }
            }
            else
            {
                PlayerPrefs.SetString("rmlSettings_ShowMainMenuOnStart", "True");
            }

            if (PlayerPrefs.HasKey("rmlSettings_ConsoleKeybind"))
            {
                KeyCode keycode = HUtils.KeyCodeFromString(PlayerPrefs.GetString("rmlSettings_ConsoleKeybind"));
                if (keycode != KeyCode.None)
                {
                    RML_Main.ConsoleKey = keycode;
                }
                else
                {
                    PlayerPrefs.SetString("rmlSettings_ConsoleKeybind", "F10");
                    RML_Main.MenuKey = KeyCode.F10;
                }
            }
            else
            {
                PlayerPrefs.SetString("rmlSettings_ConsoleKeybind", "F10");
                RML_Main.MenuKey = KeyCode.F10;
            }

            if (PlayerPrefs.HasKey("rmlSettings_MenuKeybind"))
            {
                KeyCode keycode = HUtils.KeyCodeFromString(PlayerPrefs.GetString("rmlSettings_MenuKeybind"));
                if (keycode != KeyCode.None)
                {
                    RML_Main.MenuKey = keycode;
                }
                else
                {
                    PlayerPrefs.SetString("rmlSettings_MenuKeybind", "F9");
                    RML_Main.MenuKey = KeyCode.F9;
                }
            }
            else
            {
                PlayerPrefs.SetString("rmlSettings_MenuKeybind", "F9");
                RML_Main.MenuKey = KeyCode.F9;
            }

            rmlsetting_consolekeybind.GetComponentInChildren<TMP_Dropdown>().onValueChanged.AddListener(OnConsoleKeybindChange);
            rmlsetting_menukeybind.GetComponentInChildren<TMP_Dropdown>().onValueChanged.AddListener(OnMenuKeybindChange);

            UpdateSettingsKeybinds();
        }

        void MasterVolumeChanged(float val)
        {
            PlayerPrefs.SetFloat("rmlSettings_MasterVolume", val);
            AudioListener.volume = val;
            rmlsetting_mastervolume.transform.Find("ValueShown").GetComponent<TextMeshProUGUI>().text = Math.Round(val * 100) + "%";
        }

        void OnConsoleKeybindChange(int val)
        {
            KeyCode consoleKeycode = KeyCode.F10;
            string keycode = rmlsetting_consolekeybind.GetComponentInChildren<TMP_Dropdown>().options[val].text;
            consoleKeycode = HUtils.KeyCodeFromString(keycode);
            if (consoleKeycode != KeyCode.None)
            {
                if (consoleKeycode != RML_Main.MenuKey)
                {
                    PlayerPrefs.SetString("rmlSettings_ConsoleKeybind", keycode);
                    RML_Main.ConsoleKey = consoleKeycode;
                }
                else
                {
                    PlayerPrefs.SetString("rmlSettings_ConsoleKeybind", "F10");
                    RML_Main.ConsoleKey = KeyCode.F10;
                    PlayerPrefs.SetString("rmlSettings_MenuKeybind", "F9");
                    RML_Main.MenuKey = KeyCode.F9;
                }
            }
            else
            {
                PlayerPrefs.SetString("rmlSettings_ConsoleKeybind", "F10");
                RML_Main.ConsoleKey = KeyCode.F10;
                PlayerPrefs.SetString("rmlSettings_MenuKeybind", "F9");
                RML_Main.MenuKey = KeyCode.F9;
            }

            UpdateSettingsKeybinds();
        }

        void OnMenuKeybindChange(int val)
        {
            KeyCode menuKeycode = KeyCode.F9;
            string keycode = rmlsetting_menukeybind.GetComponentInChildren<TMP_Dropdown>().options[val].text;
            menuKeycode = HUtils.KeyCodeFromString(keycode);
            if (menuKeycode != KeyCode.None)
            {
                if (menuKeycode != RML_Main.ConsoleKey)
                {
                    PlayerPrefs.SetString("rmlSettings_MenuKeybind", keycode);
                    RML_Main.MenuKey = menuKeycode;
                }
                else
                {
                    PlayerPrefs.SetString("rmlSettings_ConsoleKeybind", "F10");
                    RML_Main.ConsoleKey = KeyCode.F10;
                    PlayerPrefs.SetString("rmlSettings_MenuKeybind", "F9");
                    RML_Main.MenuKey = KeyCode.F9;
                }
            }
            else
            {
                PlayerPrefs.SetString("rmlSettings_ConsoleKeybind", "F10");
                RML_Main.ConsoleKey = KeyCode.F10;
                PlayerPrefs.SetString("rmlSettings_MenuKeybind", "F9");
                RML_Main.MenuKey = KeyCode.F9;
            }

            UpdateSettingsKeybinds();
        }

        void UpdateSettingsKeybinds()
        {
            int consoleValue = 0;
            switch (RML_Main.ConsoleKey)
            {
                case KeyCode.F1:
                    consoleValue = 0;
                    break;
                case KeyCode.F2:
                    consoleValue = 1;
                    break;
                case KeyCode.F3:
                    consoleValue = 2;
                    break;
                case KeyCode.F4:
                    consoleValue = 3;
                    break;
                case KeyCode.F5:
                    consoleValue = 4;
                    break;
                case KeyCode.F6:
                    consoleValue = 5;
                    break;
                case KeyCode.F7:
                    consoleValue = 6;
                    break;
                case KeyCode.F8:
                    consoleValue = 7;
                    break;
                case KeyCode.F9:
                    consoleValue = 8;
                    break;
                case KeyCode.F10:
                    consoleValue = 9;
                    break;
                case KeyCode.F11:
                    consoleValue = 10;
                    break;
                case KeyCode.F12:
                    consoleValue = 11;
                    break;
                case KeyCode.Tilde:
                    consoleValue = 12;
                    break;
                default:
                    consoleValue = 9;
                    PlayerPrefs.SetString("rmlSettings_ConsoleKeybind", "F10");
                    RML_Main.MenuKey = KeyCode.F10;
                    break;
            }
            rmlsetting_consolekeybind.GetComponentInChildren<TMP_Dropdown>().value = consoleValue;

            int mainmenuValue = 0;
            switch (RML_Main.MenuKey)
            {
                case KeyCode.F1:
                    mainmenuValue = 0;
                    break;
                case KeyCode.F2:
                    mainmenuValue = 1;
                    break;
                case KeyCode.F3:
                    mainmenuValue = 2;
                    break;
                case KeyCode.F4:
                    mainmenuValue = 3;
                    break;
                case KeyCode.F5:
                    mainmenuValue = 4;
                    break;
                case KeyCode.F6:
                    mainmenuValue = 5;
                    break;
                case KeyCode.F7:
                    mainmenuValue = 6;
                    break;
                case KeyCode.F8:
                    mainmenuValue = 7;
                    break;
                case KeyCode.F9:
                    mainmenuValue = 8;
                    break;
                case KeyCode.F10:
                    mainmenuValue = 9;
                    break;
                case KeyCode.F11:
                    mainmenuValue = 10;
                    break;
                case KeyCode.F12:
                    mainmenuValue = 11;
                    break;
                default:
                    mainmenuValue = 8;
                    PlayerPrefs.SetString("rmlSettings_MenuKeybind", "F9");
                    RML_Main.MenuKey = KeyCode.F9;
                    break;
            }

            rmlsetting_menukeybind.GetComponentInChildren<TMP_Dropdown>().value = mainmenuValue;


            MainMenu.VersionText.GetComponentInChildren<TextMeshProUGUI>().text = @"RaftModLoader <color=#36d1a8>v" + RML_Main.gameData.version + @"</color>
Press <color=#36d1a8>" + RML_Main.MenuKey.ToString() + "</color> to open the main menu\n" +
    "Press <color=#36d1a8>" + RML_Main.ConsoleKey.ToString() + "</color> to open the console";

        }

        void ToggleEnableThisMenuIngame(bool var)
        {
            PlayerPrefs.SetString("rmlSettings_EnableThisMenuIngame", var.ToString());
            EnableMenuInGame = var;
        }

        void ToggleHidePatrons(bool var)
        {
            PlayerPrefs.SetString("rmlSettings_HidePatronsAnimation", var.ToString());
            HidePatronsAnimation = var;

            if (CustomLoadingScreen.loadingscreen_banner != null)
            {
                foreach (Transform t in CustomLoadingScreen.loadingscreen_banner.transform.Find("PatronsBar").Find("PatronsRow"))
                {
                    List<Animator> animations = t.gameObject.GetComponentsInChildren<Animator>().ToList();
                    animations.ForEach(anim =>
                    {
                        anim.enabled = HidePatronsAnimation ? false : anim.GetComponent<PatronData>().ShouldBeOn;
                    });
                }
            }
        }
    }
}