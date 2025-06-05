using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Debugger : MonoBehaviour
{
    public static Debugger instance;
    [SerializeField] TextMeshProUGUI currentFps;
    [SerializeField] TextMeshProUGUI avgFps;
    [SerializeField] List<float> fpss;
    bool KuchBhi = true;

    [SerializeField] string bundle = "io.metamask";
    [SerializeField] TMP_InputField bundleText;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Screen.SetResolution(1280, 720, true);
        if (instance == null) instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        //bundleText.text = bundle;
    }

    public void Shit()
    {
        if (!IsAppInstalled())
        {
            Application.OpenURL("https://play.google.com/store/apps/details?id=" + bundleText.text);
        }
    }

    private void Update()
    {
        if (KuchBhi)
        {
            float thisFrameFps = float.Parse((1 / Time.deltaTime).ToString("f2"));
            currentFps.text = "Current: " + thisFrameFps.ToString();
            CalculateAvgFps(thisFrameFps);
            KuchBhi = false;
            Invoke(nameof(kuchbhiTrue), 0.1f);
        }
    }

    void kuchbhiTrue()
    {
        KuchBhi = true;
    }

    void CalculateAvgFps(float currentFps)
    {
        if (currentFps > 60) currentFps = 60;
        fpss.Add(currentFps);
        if (fpss.Count > 50)
        {
            fpss.RemoveAt(0);
        }
        float total = 0;
        foreach (var item in fpss)
        {
            total += item;
        }
        float avg = total / 50;
        avgFps.text = "AVG: " + avg.ToString("f2");
    }

    void IsAppInstalled1()
    {
        AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");

        
    }

    public bool IsAppInstalled()
    {
        AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");
        
        Debug.Log(" ********LaunchOtherApp ");
        AndroidJavaObject launchIntent = null;
        //if the app is installed, no errors. Else, doesn't get past next line
        try
        {
            Debug.Log("1 ********LaunchOtherApp ");
            launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", bundleText.text);
            Debug.Log("2 ********LaunchOtherApp ");
            //Debug.Log(launchIntent.ToString());
            //        
            //ca.Call("startActivity",launchIntent);
        }
        catch(Exception ex) 
        {
            Debug.Log("3 ********LaunchOtherApp " + ex.Message);
            Debug.Log("exception");
        }
        if (launchIntent == null)
        {
            Debug.Log("4 ********LaunchOtherApp ");
            return false;
        }
            Debug.Log("5 ********LaunchOtherApp ");
        return true;
    }
}