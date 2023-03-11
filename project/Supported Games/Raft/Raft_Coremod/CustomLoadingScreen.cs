using HarmonyLib;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using HMLLibrary;
using System.Collections.Generic;
using System.Linq;

namespace RaftModLoader
{
    public class CustomLoadingScreen : MonoBehaviour
    {
        public static CustomLoadingScreen instance;
        public static LoadSceneManager loadscenemanager;
        public static GameObject loadPanel;
        public static GameObject loadingscreen_banner;
        public static GameObject patronPrefab;

        void Awake()
        {
            instance = this;
            GameObject prefab2 = HLib.bundle.LoadAsset<GameObject>("LoadingScreenBanner");
            patronPrefab = HLib.bundle.LoadAsset<GameObject>("PatronEntry");
            loadscenemanager = ComponentManager<LoadSceneManager>.Value ?? FindObjectOfType<LoadSceneManager>();
            loadPanel = (Traverse.Create(loadscenemanager).Field("loadPanel").GetValue() as GameObject);
            loadingscreen_banner = Instantiate(prefab2, loadPanel.transform).NoteAsRML();
            loadingscreen_banner.transform.SetAsLastSibling();
        }

        public static async void InitLoadingScreen(JSONObject patronsdata)
        {
            while (loadingscreen_banner == null)
            {
                await Task.Delay(1);
            }
            try
            {
                foreach (Transform t in loadingscreen_banner.transform.Find("PatronsBar").Find("PatronsRow"))
                {
                    Destroy(t.gameObject);
                }
                for (int i = 0; i < patronsdata.Count; i++)
                {
                    string name = patronsdata[i]["name"].str;
                    string amount = patronsdata[i]["amount"].str;
                    string imageUrl = patronsdata[i]["imageUrl"].str;
                    bool isNitroBooster = patronsdata[i]["isNitroBooster"].b;
                    bool avatarAnimated = patronsdata[i]["avatarAnimated"].b;
                    bool usernameAnimated = patronsdata[i]["usernameAnimated"].b;
                    GameObject item = Instantiate(patronPrefab, Vector3.zero, Quaternion.identity, loadingscreen_banner.transform.Find("PatronsBar").Find("PatronsRow")).NoteAsRML();
                    item.transform.Find("username").GetComponent<TextMeshProUGUI>().text = name;
                    if (amount != "")
                    {
                        item.transform.Find("username").GetComponent<TextMeshProUGUI>().text += " : <color=#4287f5>" + amount + "</color>";
                    }
                    item.transform.Find("IsNitroUser").gameObject.SetActive(isNitroBooster);
                    PatronData pd = item.transform.Find("AvatarMask").gameObject.AddComponent<PatronData>();
                    pd.ShouldBeOn = avatarAnimated;
                    item.transform.Find("AvatarMask").GetComponent<Animator>().enabled = avatarAnimated;
                    pd = item.transform.Find("username").gameObject.AddComponent<PatronData>();
                    pd.ShouldBeOn = usernameAnimated;
                    item.transform.Find("username").GetComponent<Animator>().enabled = usernameAnimated;

                    if (imageUrl != "" && imageUrl.Length > 5)
                    {
                        RawImage image = item.transform.Find("AvatarMask").Find("avatar").GetComponent<RawImage>();
                        HUtils.DownloadCachedTexture(imageUrl).ContinueWith((t) =>
                        {
                            image.texture = t.Result;
                        });
                    }
                }
                foreach (Transform t in loadingscreen_banner.transform.Find("PatronsBar").Find("PatronsRow"))
                {
                    List<Animator> animations = t.gameObject.GetComponentsInChildren<Animator>().ToList();
                    animations.ForEach(anim =>
                    {
                        anim.enabled = SettingsPage.HidePatronsAnimation ? false : anim.GetComponent<PatronData>().ShouldBeOn;
                    });
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
    }

    public class PatronData : MonoBehaviour
    {
        public bool ShouldBeOn;
    }
}