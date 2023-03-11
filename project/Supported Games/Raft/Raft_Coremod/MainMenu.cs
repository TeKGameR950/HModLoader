using AssemblyLoader;
using HarmonyLib;
using HMLLibrary;
using I2.Loc;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AzureSky;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace RaftModLoader
{
    public class MainMenu : MonoBehaviour
    {
        public static MainMenu instance;
        public static bool IsOpen = true;
        public string CurrentPage;
        public static GameObject pages;
        public static Dictionary<string, MenuPage> menuPages = new Dictionary<string, MenuPage>();
        public static GameObject LeftPagesButtons;
        public static GameObject VersionText;
        public static GameObject SafeMode;

        GameObject introductionCanvas;
        void Awake()
        {
            if (!HLoader.SAFEMODE)
            {
                if (!PlayerPrefs.HasKey("rmlSettings_ShownIntro") || PlayerPrefs.GetInt("rmlSettings_ShownIntro") != 1)
                {
                    introductionCanvas = Instantiate(HLib.bundle.LoadAsset<GameObject>("Introduction")).NoteAsRML();
                    introductionCanvas.transform.Find("Background").Find("Text").GetComponent<TextMeshProUGUI>().text = introductionCanvas.transform.Find("Background").Find("Text").GetComponent<TextMeshProUGUI>().text.Replace("TeKGameR", SteamFriends.GetPersonaName());
                    introductionCanvas.transform.Find("Buttons").Find("discord").GetComponent<Button>().onClick.AddListener(() =>
                    {
                        Application.OpenURL("https://www.raftmodding.com/discord");
                    });
                    introductionCanvas.transform.Find("Buttons").Find("patreon").GetComponent<Button>().onClick.AddListener(() =>
                    {
                        Application.OpenURL("https://www.patreon.com/hytekgames");
                    });
                    introductionCanvas.transform.Find("Buttons").Find("continue").GetComponent<Button>().onClick.AddListener(() =>
                    {
                        PlayerPrefs.SetInt("rmlSettings_ShownIntro", 1);
                        introductionCanvas.GetComponent<Animation>().Play();
                        Destroy(introductionCanvas, 5f);
                    });
                }
            }
        }

        void Start()
        {
            instance = this;
            ComponentManager<RSocket>.Value.onlineUsers = GameObject.Find("RMLOnlineUsersCounter").GetComponent<Text>();
            pages = GameObject.Find("RMLMainMenu_PageZone");
            foreach (Transform p in pages.transform)
            {
                p.gameObject.SetActive(true);
            }

            LeftPagesButtons = GameObject.Find("RMLMainMenu_Slots");

            if (HLoader.SAFEMODE)
            {
                ClientPluginManager.OnMainMenu();
                try
                {
                    Destroy(pages.transform.Find("ModManager").gameObject);
                    Destroy(GameObject.Find("RMLMainMenuLButton_ModManager"));
                    Destroy(pages.transform.Find("Settings").gameObject);
                    Destroy(GameObject.Find("RMLMainMenuLButton_Settings"));
                }
                catch (Exception ex)
                {
                    HLogger.Log(ex.ToString());
                }
            }

            VersionText = Instantiate(HLib.bundle.LoadAsset<GameObject>("RMLVersionText"), Vector3.zero, Quaternion.identity).NoteAsRML();
            VersionText.GetComponentInChildren<TextMeshProUGUI>().text = @"RaftModLoader <color=#36d1a8>v" + RML_Main.gameData.version + @"</color>
Press <color=#36d1a8>F9</color> to open the main menu
Press <color=#36d1a8>F10</color> to open the console";
            DontDestroyOnLoad(VersionText);

            GameObject.Find("RMLMainMenuLButton_Home").GetComponent<Button>().onClick.AddListener(() => { ChangeMenu("Home"); });
            if (!HLoader.SAFEMODE)
                GameObject.Find("RMLMainMenuLButton_ModManager").GetComponent<Button>().onClick.AddListener(() => { ChangeMenu("ModManager"); });
            GameObject.Find("RMLMainMenuLButton_Servers").GetComponent<Button>().onClick.AddListener(() => { ChangeMenu("Servers"); });
            if (!HLoader.SAFEMODE)
                GameObject.Find("RMLMainMenuLButton_Settings").GetComponent<Button>().onClick.AddListener(() => { ChangeMenu("Settings"); });
            GameObject.Find("RMLMainMenuLButton_Credits").GetComponent<Button>().onClick.AddListener(() => { ChangeMenu("Credits"); });
            GameObject.Find("RMLMainMenuCloseBtn").GetComponent<Button>().onClick.AddListener(CloseMenu);

            menuPages.Add("Home", pages.transform.Find("Home").gameObject.AddComponent<HomePage>());
            if (!HLoader.SAFEMODE)
                menuPages.Add("ModManager", pages.transform.Find("ModManager").gameObject.AddComponent<ModManagerPage>());
            else
                pages.transform.Find("ModManager").gameObject.SetActive(false);
            menuPages.Add("Servers", pages.transform.Find("Servers").gameObject.AddComponent<ServersPage>());
            if (!HLoader.SAFEMODE)
                menuPages.Add("Settings", pages.transform.Find("Settings").gameObject.AddComponent<SettingsPage>());
            else
                pages.transform.Find("Settings").gameObject.SetActive(false);

            menuPages.Add("Credits", pages.transform.Find("Credits").gameObject.AddComponent<CreditsPage>());

            menuPages.ToList().ForEach(p => p.Value.Initialize());

            if (HLoader.SAFEMODE)
            {
                SafeMode = Instantiate(HLib.bundle.LoadAsset<GameObject>("SafeModeWarning"), Vector3.zero, Quaternion.identity).NoteAsRML();
                DontDestroyOnLoad(SafeMode);

                ChangeMenu("Servers");
            }

        }

        Dropdown load_authSettingDropdown;
        Dropdown new_authSettingDropdown;
        void Update()
        {
            if (Camera.current != null && Camera.current.GetComponent<AudioListener>() == null)
            {
                Camera.current.gameObject.AddComponent<AudioListener>();
            }
            if (!RAPI.IsCurrentSceneMainMenu()) { return; }
            if (!HLoader.SAFEMODE)
            {
                if (!load_authSettingDropdown)
                {
                    load_authSettingDropdown = Traverse.Create(ComponentManager<LoadGameBox>.Value).Field("authSettingDropdown").GetValue() as Dropdown;
                }
                else
                {
                    if (load_authSettingDropdown.options.Count < 3)
                    {
                        load_authSettingDropdown.AddOptions(new List<string> { "Everyone Can Join" });
                    }
                }

                if (!new_authSettingDropdown)
                {
                    new_authSettingDropdown = Traverse.Create(ComponentManager<NewGameBox>.Value).Field("authSettingDropdown").GetValue() as Dropdown;
                }
                else
                {
                    if (new_authSettingDropdown.options.Count < 3)
                    {
                        new_authSettingDropdown.AddOptions(new List<string> { "Everyone Can Join" });
                    }
                }
            }
        }

        public static void CloseMenu()
        {
            ModManagerPage.RefreshModsStates();
            IsOpen = false;
            RAPI.TogglePriorityCursor(false);
        }

        public void OpenMenu()
        {
            ChangeMenu(CurrentPage);
            IsOpen = true;
            RAPI.TogglePriorityCursor(true);
            ModManagerPage.RefreshModsStates();
        }

        void LateUpdate()
        {
            if (RAPI.IsCurrentSceneMainMenu())
            {
                VersionText.GetComponent<Canvas>().enabled = true;
                if (HLoader.SAFEMODE)
                    SafeMode.GetComponent<Canvas>().enabled = true;
            }
            else
            {
                VersionText.GetComponent<Canvas>().enabled = false;
                if (HLoader.SAFEMODE)
                    SafeMode.GetComponent<Canvas>().enabled = false;
            }

            if (!SettingsPage.EnableMenuInGame && !RAPI.IsCurrentSceneMainMenu())
            {
                GetComponent<Canvas>().enabled = false;
                IsOpen = false;
                return;
            }

            if (IsOpen)
            {
                if (!GetComponent<Canvas>().enabled)
                {
                    GetComponent<Canvas>().enabled = true;
                    GetComponent<Animation>().Play("MenuOpen");
                }
            }
            else
            {
                if (GetComponent<Canvas>().enabled)
                {
                    StartCoroutine(CloseMenuLate());
                }
            }

            if (Input.GetKeyDown(RML_Main.MenuKey))
            {
                LoadSceneManager lsm = ComponentManager<LoadSceneManager>.Value;
                if (!lsm) lsm = FindObjectOfType<LoadSceneManager>();
                if ((Traverse.Create(lsm).Field("fadePanel").GetValue() as FadePanel).gameObject.activeSelf)
                {
                    return;
                }
                IsOpen = !IsOpen;
                if (IsOpen)
                {
                    ChangeMenu(CurrentPage);
                }
                if (!RAPI.IsCurrentSceneMainMenu())
                {
                    RAPI.TogglePriorityCursor(IsOpen);
                }

            }
            else if (Input.GetKeyDown(KeyCode.Escape) && IsOpen)
            {
                LoadSceneManager lsm = ComponentManager<LoadSceneManager>.Value;
                if (!lsm) lsm = FindObjectOfType<LoadSceneManager>();
                if ((Traverse.Create(lsm).Field("fadePanel").GetValue() as FadePanel).gameObject.activeSelf)
                {
                    return;
                }
                IsOpen = false;
                if (!RAPI.IsCurrentSceneMainMenu())
                {
                    RAPI.TogglePriorityCursor(false);
                }
            }
        }

        public IEnumerator CloseMenuLate()
        {
            GetComponent<Animation>().Play("MenuClose");
            yield return new WaitForSeconds(0.10f);
            GetComponent<Canvas>().enabled = false;
        }

        public static void ChangeMenu(string menuname)
        {
            foreach (Transform p in pages.transform)
            {
                if (p.gameObject.activeSelf)
                {
                    if (menuPages.ContainsKey(p.name))
                    {
                        menuPages[p.name].OnPageOpen();
                    }
                }
                p.gameObject.SetActive(false);
            }
            if (menuPages.ContainsKey(menuname))
            {
                menuPages[menuname].OnPageOpen();
            }
            instance.StartCoroutine(instance.ChangePage(menuname));
        }

        public IEnumerator ChangePage(string menuname)
        {
            yield return new WaitForEndOfFrame();
            foreach (Transform b in LeftPagesButtons.transform)
            {
                b.GetComponent<Button>().interactable = true;
            }
            pages.GetComponent<CanvasGroup>().alpha = 0;
            GameObject page = pages.transform.Find(menuname).gameObject;
            Button button = LeftPagesButtons.transform.Find("RMLMainMenuLButton_" + menuname).gameObject.GetComponent<Button>();

            if (page != null)
            {
                page.SetActive(true);
                pages.GetComponent<Animation>().Play();
                button.interactable = false;
                CurrentPage = menuname;
            }
        }
    }

    public class TooltipHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public GameObject tooltip;

        public void Start()
        {
            tooltip.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            tooltip.SetActive(true);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            if (gameObject.GetComponent<Button>() != null)
            {
                gameObject.GetComponent<Button>().OnDeselect(null);
            }
            tooltip.SetActive(false);
        }
    }
}