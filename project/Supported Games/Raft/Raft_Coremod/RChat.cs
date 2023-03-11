using HarmonyLib;
using HMLLibrary;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RaftModLoader
{
    public class RChat : MonoBehaviour
    {
        public static bool isCustomChatEnabled = true;

        GameObject ChatCanvas;
        ChatTextFieldController chatTextFieldController;
        Network_Player localPlayer;
        Raft_Network network;
        GameObject inputfield;
        GameObject chatcontent;
        GameObject chatEntry;
        public static bool isInputfieldOpen = false;
        bool hasSent = false;
        public static RChat instance;
        ScrollRect scrollRect;
        RectTransform mainRect;
        RectTransform contentRect;
        TMP_SpriteAsset emoji;

        GameObject emojiList;
        GameObject emojiListContent;
        GameObject emojiListEntry;
        GameObject chatViewport;

        private Coroutine HideCoroutine;

        void Start()
        {
            if (PlayerPrefs.HasKey("rmlSettings_IsCustomChatEnabled"))
            {
                if (PlayerPrefs.GetString("rmlSettings_IsCustomChatEnabled") == "True")
                {
                    isCustomChatEnabled = true;
                }
                else
                {
                    isCustomChatEnabled = false;
                }
            }
            else
            {
                isCustomChatEnabled = true;
                PlayerPrefs.SetString("rmlSettings_IsCustomChatEnabled", "True");
            }

            instance = this;
            chatTextFieldController = ComponentManager<ChatTextFieldController>.Value;
            ChatCanvas = Instantiate(HLib.bundle.LoadAsset<GameObject>("_RMLChatCanvas"), transform).NoteAsRML();
            chatEntry = HLib.bundle.LoadAsset<GameObject>("ChatEntry");
            ChatCanvas.GetComponent<Canvas>().enabled = false;
            chatcontent = ChatCanvas.transform.Find("ScrollRect").Find("Chat").Find("Viewport").Find("Content").gameObject;
            chatViewport = ChatCanvas.transform.Find("ScrollRect").Find("Chat").gameObject;
            inputfield = ChatCanvas.transform.Find("ScrollRect").Find("InputField").gameObject;
            inputfield.GetComponent<CanvasGroup>().alpha = 0;
            inputfield.GetComponent<CanvasGroup>().interactable = false;
            inputfield.GetComponent<CanvasGroup>().blocksRaycasts = false;
            inputfield.GetComponent<TMP_InputField>().onSubmit.AddListener(HandleChatMessage);
            scrollRect = ChatCanvas.transform.Find("ScrollRect").GetComponent<ScrollRect>();
            mainRect = scrollRect.GetComponent<RectTransform>();
            contentRect = chatcontent.GetComponent<RectTransform>();
            emoji = HLib.bundle.LoadAsset<TMP_SpriteAsset>("EmojiData_google");
            emojiListEntry = HLib.bundle.LoadAsset<GameObject>("EmojiBtnEntry");
            emojiList = inputfield.transform.Find("EmojiList").gameObject;
            emojiListContent = emojiList.transform.Find("Viewport").Find("Content").gameObject;

            inputfield.transform.Find("EmojiBtn").GetComponent<Button>().onClick.AddListener(ToggleEmojiList);

            int val = 0;
            foreach (KeyValuePair<string, string> t in HUtils.emoji)
            {
                GameObject a = Instantiate(emojiListEntry, emojiListContent.transform).NoteAsRML();
                a.GetComponentInChildren<TextMeshProUGUI>().text = t.Value;
                a.AddComponent<TooltipHandler>().tooltip = a.transform.Find("Tooltip").gameObject;
                a.transform.Find("Tooltip").GetComponent<TextMeshProUGUI>().text = t.Key;
                a.transform.Find("Tooltip").gameObject.SetActive(false);
                a.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(AddEmoji(t.Key)));
                val++;
            }
            emojiList.gameObject.SetActive(false);
            chatViewport.GetComponent<CanvasGroup>().alpha = 0;
            chatViewport.GetComponent<CanvasGroup>().blocksRaycasts = false;
            chatViewport.GetComponent<CanvasGroup>().interactable = false;
        }

        public void ClearChat()
        {
            foreach (Transform t in chatcontent.transform)
            {
                Destroy(t.gameObject);
            }
        }

        IEnumerator HideChat()
        {
            yield return new WaitForSeconds(5f);
            if (!isInputfieldOpen && isCustomChatEnabled)
            {
                chatViewport.GetComponent<Animation>().Play("ChatClose");
            }
        }

        public static void ToggleCustomChat()
        {
            isCustomChatEnabled = !isCustomChatEnabled;
            PlayerPrefs.SetString("rmlSettings_IsCustomChatEnabled", isCustomChatEnabled.ToString());
        }

        void ToggleEmojiList()
        {
            foreach (Transform t in emojiList.transform)
            {
                Transform a = t.transform.Find("Tooltip");
                if (a != null)
                {
                    a.gameObject.SetActive(false);
                }
            }
            emojiList.SetActive(!emojiList.activeSelf);
            emojiList.GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;
        }

        public IEnumerator AddEmoji(string n)
        {
            inputfield.GetComponent<TMP_InputField>().text += n;
            yield return new WaitForEndOfFrame();
            inputfield.GetComponent<TMP_InputField>().MoveToEndOfLine(false, false);
        }

        IEnumerator ScrollToBottom()
        {
            yield return new WaitForEndOfFrame();
            try
            {
                scrollRect.gameObject.SetActive(true);
                scrollRect.verticalNormalizedPosition = 0f;
                Canvas.ForceUpdateCanvases();
                LayoutRebuilder.ForceRebuildLayoutImmediate(mainRect);
                LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
            }
            catch { }
        }

        public void CreatePlayerJoinText(Network_Player p_player)
        {
            if (!isCustomChatEnabled || p_player.steamID.m_SteamID < 10000) { return; }
            if (network.remoteUsers.ContainsKey(p_player.steamID))
            {
                Network_Player playerFromID = network.GetPlayerFromID(p_player.steamID);
                if (playerFromID != null)
                {
                    AddUITextMessage("<color=#40c965>" + playerFromID.characterSettings.Name + " has joined the raft!</color>");
                }
            }
        }

        public void CreatePlayerLeaveText(CSteamID steamID, DisconnectReason reason)
        {
            if (!isCustomChatEnabled || steamID.m_SteamID < 10000) { return; }
            if (network.remoteUsers.ContainsKey(steamID))
            {
                Network_Player playerFromID = network.GetPlayerFromID(steamID);
                if (playerFromID != null)
                {
                    AddUITextMessage("<color=#c94040>" + playerFromID.characterSettings.Name + " has left the raft!</color>");
                }
            }
        }

        public void AddUITextMessage(string p_ChatMessage)
        {
            // if (!isCustomChatEnabled) { return; }
            AddUITextMessage(p_ChatMessage, new CSteamID());
        }

        public void AddUITextMessage(string p_ChatMessage, CSteamID p_steamID)
        {
            //if (!isCustomChatEnabled) { return; }
            p_ChatMessage = p_ChatMessage.RML_InsertEmoji();
            if (HideCoroutine != null) { StopAllCoroutines(); HideCoroutine = null; }
            string username = "";
            if (p_steamID != new CSteamID())
            {
                if (p_steamID.m_SteamID == 666)
                {
                    username = "<color=red>SERVER</color>";
                }
                else
                {
                    Network_Player playerFromID = network.GetPlayerFromID(p_steamID);
                    if (playerFromID != null)
                    {
                        string name = playerFromID.characterSettings.Name;
                        username = GetProperColorCode(p_steamID) + name + "</color>";
                    }
                }
            }

            GameObject message = Instantiate(chatEntry, chatcontent.transform);
            TextMeshProUGUI text = message.transform.Find("Message").gameObject.AddComponent<TextMeshProUGUI>();
            text.fontStyle = FontStyles.Bold;
            text.fontSize = 15f;
            text.spriteAsset = emoji;
            if (string.IsNullOrWhiteSpace(username))
            {
                text.text = p_ChatMessage;
            }
            else
            {
                text.text = username + ": " + p_ChatMessage;
            }

            StartCoroutine(ScrollToBottom());
            if (chatViewport.GetComponent<CanvasGroup>().alpha < 1)
            {
                chatViewport.GetComponent<Animation>().Play("ChatOpen");
            }
            if (HideCoroutine != null) { StopAllCoroutines(); HideCoroutine = null; }
            HideCoroutine = StartCoroutine(HideChat());
        }

        private string GetProperColorCode(CSteamID p_steamID)
        {
            string result = string.Empty;
            if (network.HostID == p_steamID)
            {
                return "<color=#c94040>";
            }
            else
            {
                return "<color=#40c965>";
            }
        }

        private void HandleChatMessage(string val)
        {
            if (chatTextFieldController == null)
            {
                chatTextFieldController = ComponentManager<ChatTextFieldController>.Value;
            }
            if (!isCustomChatEnabled) { return; }
            if (isInputfieldOpen && !Input.GetKeyDown(KeyCode.Escape))
            {
                try
                {
                    chatTextFieldController.PreHandleChatMessage(val);
                }
                catch
                {
                    RChat.instance.AddUITextMessage("<color=#db2a2a>You are missing some of the server mods! The world might be missing blocks and the chat can't be enabled!", new CSteamID());
                }
                hasSent = true;
                SetChatFieldVisible(false);
            }
        }

        private void SetChatFieldVisible(bool value)
        {
            if (!isCustomChatEnabled) { return; }
            if (value)
            {
                emojiList.SetActive(false);
                inputfield.GetComponent<Animation>().Play("ChatInputfieldOpen");
                isInputfieldOpen = true;
                ComponentManager<CanvasHelper>.Value.OpenMenu(MenuType.ChatField, true);
                PlayerItemManager.IsBusy = true;
                localPlayer.PersonController.IsMovementFree = false;
                inputfield.GetComponent<TMP_InputField>().text = "";
                if (chatViewport.GetComponent<CanvasGroup>().alpha < 1)
                {
                    chatViewport.GetComponent<Animation>().Play("ChatOpen");
                }
                if (HideCoroutine != null) { StopAllCoroutines(); HideCoroutine = null; }
            }
            else
            {
                emojiList.SetActive(false);
                inputfield.GetComponent<TMP_InputField>().text = "";
                inputfield.GetComponent<TMP_InputField>().DeactivateInputField();
                inputfield.GetComponent<Animation>().Play("ChatInputfieldClose");
                isInputfieldOpen = false;
                ComponentManager<CanvasHelper>.Value.CloseMenu(MenuType.ChatField);
                PlayerItemManager.IsBusy = false;
                localPlayer.PersonController.IsMovementFree = true;
                if (HideCoroutine != null) { StopAllCoroutines(); HideCoroutine = null; }
                HideCoroutine = StartCoroutine(HideChat());
            }
        }

        void Update()
        {
            if (!isCustomChatEnabled) { return; }
            ChatCanvas.GetComponent<Canvas>().enabled = RAPI.IsCurrentSceneGame();
            if (RAPI.IsCurrentSceneMainMenu()) { return; }

            if (localPlayer == null || network == null || chatTextFieldController == null)
            {
                network = ComponentManager<Raft_Network>.Value;
                localPlayer = network.GetLocalPlayer();
                chatTextFieldController = ComponentManager<ChatTextFieldController>.Value;
                return;
            }
            chatTextFieldController.GetComponent<Canvas>().enabled = !RChat.isCustomChatEnabled;

            if (hasSent)
            {
                hasSent = false;
                return;
            }

            if (isInputfieldOpen)
            {
                inputfield.GetComponent<TMP_InputField>().Select();
                inputfield.GetComponent<TMP_InputField>().ActivateInputField();
            }

            HandleInput();
        }

        void HandleInput()
        {
            if (isInputfieldOpen && Input.GetKeyDown(KeyCode.Escape))
            {
                inputfield.GetComponent<TMP_InputField>().text = "";
                SetChatFieldVisible(false);
            }
            if (PlayerItemManager.IsBusy || localPlayer.PlayerScript.IsDead || CanvasHelper.ActiveMenu != MenuType.None || localPlayer.BedComponent.Sleeping || localPlayer.RessurectComponent.IsCarrying) { return; }
            if (MyInput.GetButtonDown("Chat") && !isInputfieldOpen)
            {
                SetChatFieldVisible(true);
            }
        }
    }


    [HarmonyPatch(typeof(ChatTextFieldController))]
    [HarmonyPatch("AddUITextMessage")]
    public static class ChatPatch_ChatTextFieldController_AddUITextMessage
    {
        private static bool Prefix(ChatTextFieldController __instance, string p_ChatMessage, CSteamID p_steamID)
        {
            if (RChat.isCustomChatEnabled)
            {
                RChat.instance.AddUITextMessage(p_ChatMessage, p_steamID);
                return false;
            }
            else { return true; }
        }
    }

    [HarmonyPatch(typeof(ChatTextFieldController))]
    [HarmonyPatch("CreatePlayerJoinText")]
    public static class ChatPatch_ChatTextFieldController_CreatePlayerJoinText
    {
        private static bool Prefix(ChatTextFieldController __instance, Network_Player p_player)
        {
            if (RChat.isCustomChatEnabled)
            {
                RChat.instance.CreatePlayerJoinText(p_player);
                return false;
            }
            else { return true; }
        }
    }

    [HarmonyPatch(typeof(ChatTextFieldController))]
    [HarmonyPatch("CreatePlayerLeaveText")]
    public static class ChatPatch_ChatTextFieldController_CreatePlayerLeaveText
    {
        private static bool Prefix(ChatTextFieldController __instance, CSteamID steamID, DisconnectReason reason)
        {
            if (RChat.isCustomChatEnabled)
            {
                if (steamID != ComponentManager<Raft_Network>.Value.HostID && ComponentManager<Raft_Network>.Value.remoteUsers.ContainsKey(steamID))
                {
                    RChat.instance.CreatePlayerLeaveText(steamID, reason);
                }
                return false;
            }
            else { return true; }
        }
    }

    [HarmonyPatch(typeof(ChatTextFieldController))]
    [HarmonyPatch("Start")]
    public static class ChatPatch_ChatTextFieldController_Start
    {
        private static void Prefix(ChatTextFieldController __instance)
        {
            __instance.GetComponent<Canvas>().enabled = !RChat.isCustomChatEnabled;
        }
    }

    [HarmonyPatch(typeof(ChatTextFieldController))]
    [HarmonyPatch("Update")]
    public static class ChatPatch_ChatTextFieldController_Update
    {
        private static bool Prefix(ChatTextFieldController __instance)
        {
            return !RChat.isCustomChatEnabled;
        }
    }
}