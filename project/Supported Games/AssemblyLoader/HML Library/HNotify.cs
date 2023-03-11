using HMLLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if GAME_GREENHELL
using TextMeshProUGUI = UnityEngine.UI.Text;
#endif

public class HNotify : MonoBehaviour
{
    public static HNotify instance;
    GameObject content;
    GameObject _errorNotificationPrefab;
    GameObject _spinningNotificationPrefab;
    GameObject _scalingNotificationPrefab;
    GameObject _normalNotificationPrefab;

    public static Sprite ErrorSprite;
    public static Sprite LoadingSprite;
    public static Sprite CheckSprite;

    public static List<HNotification> notifications = new List<HNotification>();

    public static HNotification errornotification;

    public static HNotify Get()
    {
        return instance;
    }

    public enum NotificationType
    {
        error,
        spinning,
        scaling,
        normal
    }

    async void Start()
    {
        instance = this;
        content = transform.Find("Content").gameObject;
        _errorNotificationPrefab = HLib.bundle.LoadAsset<GameObject>("ErrorNotification");
        _spinningNotificationPrefab = HLib.bundle.LoadAsset<GameObject>("SpinningNotification");
        _scalingNotificationPrefab = HLib.bundle.LoadAsset<GameObject>("ScalingNotification");
        _normalNotificationPrefab = HLib.bundle.LoadAsset<GameObject>("NormalNotification");
        ErrorSprite = HLib.bundle.LoadAsset<Sprite>("IconError");
#if GAME_GREENHELL
        LoadingSprite = HLib.bundle.LoadAsset<Sprite>("RefreshIcon");
        CheckSprite = HLib.bundle.LoadAsset<Sprite>("ToggleIcon");
#elif GAME_RAFT
        LoadingSprite = HLib.bundle.LoadAsset<Sprite>("IconRefresh");
        CheckSprite = HLib.bundle.LoadAsset<Sprite>("Check");
#endif
        errornotification = AddNotification(NotificationType.error, "");
    }

    public HNotification AddNotification(NotificationType type, int closeDelay = 0, Sprite icon = null)
    {
        return AddNotification(type, "", closeDelay, icon);
    }

    public HNotification AddNotification(NotificationType type, string text = "", int closeDelay = 0, Sprite icon = null)
    {
        GameObject prefab = null;
        if (type == NotificationType.error)
        {
            if (errornotification != null) { return errornotification; }
            prefab = _errorNotificationPrefab;
        }
        else if (type == NotificationType.normal)
        {
            prefab = _normalNotificationPrefab;
        }
        else if (type == NotificationType.scaling)
        {
            prefab = _scalingNotificationPrefab;
        }
        else if (type == NotificationType.spinning)
        {
            prefab = _spinningNotificationPrefab;
        }
        GameObject nobj = Instantiate(prefab, content.transform);
        HNotification n = nobj.AddComponent<HNotification>();

        n.closeDelay = closeDelay;
        if (closeDelay > 0)
        {
            n.StartCoroutine(n.AutoClose());
        }

        n.text = n.transform.Find("Message").GetComponent<TextMeshProUGUI>();
        n.icon = n.transform.Find("Icon").GetComponent<Image>();
        n.Background = n.GetComponent<RawImage>();

        if (text != "")
            n.text.text = text;
        if (icon != null)
            n.icon.sprite = icon;

        if (type == NotificationType.error)
        {
            n.StartCoroutine(n.ErrorChecker());
            n.SetText("").Hide();
        }
        else
        {
            notifications.Add(n);
        }
        return n;
    }

    public static void ClearNotifications()
    {
        foreach (HNotification rn in notifications)
        {
            if (rn != null)
                rn.Close();
        }
        notifications.Clear();
    }
}

public class HNotification : MonoBehaviour
{
    public int closeDelay = 0;

    public bool isError;
    public int errorAmount;
    public DateTime lastError = DateTime.Now;
    public TextMeshProUGUI text;
    public Image icon;
    public RawImage Background;

    public async void AddNewError()
    {
        lastError = DateTime.Now;
        errorAmount++;
        SetText("An error occured! (x" + errorAmount + ")");
        Show();
        await Task.Delay(0);
    }

    public IEnumerator ErrorChecker()
    {
        yield return new WaitForSeconds(2);
        if (lastError.AddSeconds(2) < DateTime.Now)
        {
            if (errorAmount > 0)
            {
                errorAmount = 0;
                Hide();
            }
        }
        StartCoroutine(ErrorChecker());
    }

    public void Hide()
    {
        GetComponent<Animation>().Play("NotifyClose");
    }

    public void Show()
    {
        if (errorAmount == 1)
        {
            GetComponent<Animation>().Play("NotifyOpen");
        }
    }

    public IEnumerator AutoClose()
    {
        if (HNotify.errornotification == this) { yield break; }
        yield return new WaitForSeconds(closeDelay);
        GetComponent<Animation>().Play("NotifyClose");
        yield return new WaitForSeconds(0.20f);
        Destroy(gameObject);
        HNotify.notifications.Remove(this);
        yield break;
    }

    public void Close()
    {
        if (HNotify.errornotification == this) { return; }
        closeDelay = 0;
        StartCoroutine(AutoClose());
    }

    public HNotification SetColor(Color color)
    {
        Background.color = color;
        return this;
    }

    public HNotification SetText(string val)
    {
        text.text = val;
        return this;
    }

    public HNotification SetIcon(Sprite val)
    {
        icon.sprite = val;
        return this;
    }

    public HNotification SetNormal()
    {
        if (icon.GetComponent<Animation>())
        {
            Destroy(icon.GetComponent<Animation>());
            icon.GetComponent<RectTransform>().rotation = Quaternion.identity;
            icon.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        }
        return this;
    }

    public HNotification SetFontSize(int size = 10)
    {
        if (size != 10)
            text.fontSize = size;
        return this;
    }

    public HNotification SetTextColor(Color color = default)
    {
        if (color != default)
            text.color = color;
        return this;
    }

    public HNotification SetCloseDelay(int delay)
    {
        if (HNotify.errornotification == this) { return this; }
        StopAllCoroutines();
        if (delay > 0)
        {
            closeDelay = delay;
            StartCoroutine(AutoClose());
        }
        return this;
    }
}