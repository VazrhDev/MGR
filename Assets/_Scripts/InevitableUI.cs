using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace MGR
{
    public class InevitableUI : MonoBehaviour
    {
        public static InevitableUI instance;

        [SerializeField] GameObject splashScreen;

        [Header("Pop Up")]
        [SerializeField] TextMeshProUGUI popUpText;
        [SerializeField] GameObject popUpObject;

        [Header("Loading")]
        [SerializeField] GameObject loadingScreen;
        [SerializeField] TextMeshProUGUI loadingScreenLog;
        [SerializeField] GameObject loadingProgressBarParent;
        //[SerializeField] GameObject Logos;
        public Image loadingProgressBar;

        [SerializeField] string abc = "255,0,0,255";

        private void Awake()
        {
            string[] abc1 = abc.Split(',');
            Color temp = new Color32((byte)int.Parse(abc1[0]), (byte)int.Parse(abc1[1]), (byte)int.Parse(abc1[2]), (byte)int.Parse(abc1[3]));
            popUpObject.GetComponent<Image>().color = temp;

            if (instance == null) instance = this;
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }

        public void ShowPopUp(string msg, float timeOfPopup)
        {
            ShowPopUpInternal(msg, timeOfPopup);
        }
        public void ShowPopUp(string msg)
        {
            ShowPopUpInternal(msg, 2f);
        }

        void ShowPopUpInternal(string message, float time)
        {
            popUpText.text = message;
            popUpObject.SetActive(true);
            if (IsInvoking(nameof(HidePopUp))) CancelInvoke(nameof(HidePopUp));
            Invoke(nameof(HidePopUp), time);
        }

        void HidePopUp()
        {
            popUpObject.SetActive(false);
        }

        public void ShowLoadingScreen()
        {
            loadingScreen.SetActive(true);
        }

        public void LogOnLoadingScreen(string logMsg)
        {
            loadingScreenLog.text = logMsg;
        }

        public void ProgressOnLoadingScreen(float percentage)
        {
            if (!loadingProgressBarParent.activeInHierarchy) loadingProgressBarParent.SetActive(true);
            loadingProgressBar.fillAmount = (float)(percentage / 100);
        }

        public void AddInProgressOnLoadingScreen(float percentage)
        {
            if (!loadingProgressBarParent.activeInHierarchy) loadingProgressBarParent.SetActive(true);
            loadingProgressBar.fillAmount += (float)(percentage / 100);
        }

        public void HideLoadingScreen()
        {
            loadingScreen.SetActive(false);
            loadingScreenLog.text = "";
            loadingProgressBarParent.SetActive(false);
            loadingProgressBar.fillAmount = 0;
        }

        public void HideLoadingScreenWithPopup(string popMsg, float popTime)
        {
            HideLoadingScreen();
            ShowPopUp(popMsg, popTime);
        }

        public void ShowSplashScreen()
        {
            splashScreen.SetActive(true);
        }

        public void HideSplashScreen()
        {
            splashScreen.SetActive(false);
        }

    }
}