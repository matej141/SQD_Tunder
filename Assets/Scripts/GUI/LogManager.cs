using System;
using UnityEngine;
using UnityEngine.UI;

public class LogManager : MonoBehaviour
{
    public static LogManager Singleton { get; private set; } = null;

    public Text Logs = null;

    public ScrollRect ScrollRect = null;

    public void Reset()
    {
        Logs = GetComponent<Text>();
        ScrollRect = transform.parent.parent.parent.GetComponent<ScrollRect>();
    }

    void Start()
    {
        if (Singleton == null) Singleton = this;
    }

    public void AddMyLog(string log)
    {
        float beforeSilderPosition = ScrollRect.verticalNormalizedPosition;
        Logs.text += DateTime.Now.ToString() + "\t" + log + "\n";
        LayoutRebuilder.ForceRebuildLayoutImmediate(ScrollRect.GetComponent<RectTransform>());
        if (beforeSilderPosition <= 0f)
        {
            // Scroll down again when it was scrolled down before adding log
            ScrollRect.verticalNormalizedPosition = 0f;
        }
    }

    public static void AddGlobalLog(string log)
    {
        Singleton.AddMyLog(log);
    }
}
