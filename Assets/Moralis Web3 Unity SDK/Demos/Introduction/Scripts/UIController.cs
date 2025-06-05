//using MoralisUnity;
//using MoralisUnity.Kits.AuthenticationKit;
//using MoralisUnity.Web3Api.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Networking;

namespace MoralisUnity.Demos.Introduction
{
    [Serializable]
    public class CustomNftMetaData
    {
        public string name;
        public string description;
        public string image;
    }

    public class UIController : MonoBehaviour
    {
        [SerializeField]
        bool godModeOn = true;

        [SerializeField]
        private GameObject authenticationKitObject = null;

        [SerializeField]
        private GameObject congratulationUiObject = null;

        [SerializeField]
        private GameObject fireworksObject = null;

        //private AuthenticationKit authKit = null;

        [SerializeField] private GameObject continueButton = null;

        public TMP_Text nftText;
        public TMP_Text nftTokenText;
        public RawImage nftImage;
        public TMP_Text nftAllDataText;

        [SerializeField] GameObject metaMaskNotInstalledPanel;

        private void Start()
        {
            //authKit = authenticationKitObject.GetComponent<AuthenticationKit>();
            CheckForMetaMAskApp();
            if (godModeOn)
            {
                Invoke(nameof(ChakDE), 2);
            }
        }

        public void CheckForMetaMAskApp()
        {
            metaMaskNotInstalledPanel.SetActive(false);
            if (Application.platform == RuntimePlatform.Android && !IsAppInstalled())
            {
                metaMaskNotInstalledPanel.SetActive(true);
            }
            else
            {
                Invoke(nameof(StartWalletConnection), 1f);
            }
        }

        public void OpenMetaMaskAppInPlaystore()
        {
            Application.OpenURL("https://play.google.com/store/apps/details?id=io.metamask");
        }

        public bool IsAppInstalled()
        {
            if (Application.platform != RuntimePlatform.Android) return true;
            AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");
            Debug.Log(" ********LaunchOtherApp ");
            AndroidJavaObject launchIntent = null;
            //if the app is installed, no errors. Else, doesn't get past next line
            try
            {
                launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", "io.metamask");
                //        
                //ca.Call("startActivity", launchIntent);
            }
            catch
            {
                Debug.Log("exception");
            }
            if (launchIntent == null)
                return false;
            return true;
        }

        void ChakDE()
        {
            SceneManager.LoadScene("MenuScene");
        }

        private void StartWalletConnection()
        {
            MGR.InevitableUI.instance.ShowLoadingScreen();

            //authKit.Connect();
        }

        public void Authentication_OnConnect()
        {
            authenticationKitObject.SetActive(false);
            congratulationUiObject.SetActive(true);
            fireworksObject.SetActive(true);

            continueButton.SetActive(true);


#if UNITY_WEBGL

            //WalletData.instance.id = Web3GL.Account();

#endif

#if UNITY_ANDROID || UNITY_IOS

            //WalletData.instance.id = Moralis.GetClient().ApplicationId;

#endif

            //fetchNFTs();
            Invoke(nameof(OnClick_ContinueButton), 1f);
        }

        //public async void fetchNFTs()
        //{
        //    NftOwnerCollection polygonNFTs = await Moralis.Web3Api.Account.GetNFTs("0x644EA544C25A285f8Ee83cDEa20630BBeE4869A8".ToLower(), ChainList.eth);
        //    Debug.LogError(polygonNFTs);

        //    Debug.Log(polygonNFTs.ToJson());
        //    if (!WalletData.instance.devMode)
        //    {
        //        try
        //        {
        //            WalletData.instance.ownedNfts = new string[polygonNFTs.Result.Count];
        //            for (int i = 0; i < polygonNFTs.Result.Count; i++)
        //            {
        //                WalletData.instance.ownedNfts[i] = polygonNFTs.Result[i].Name;
        //            }
        //        }
        //        catch
        //        {
        //            Master.ColoredLog("red", "No NFT Found");
        //            WalletData.instance.ownedNfts = new string[0];
        //        }
        //    }
        //    Debug.Log(polygonNFTs.Result.Count);
        //    nftText.text = polygonNFTs.Result[0].Name;
        //    nftTokenText.text = polygonNFTs.Result[0].TokenId.ToString();
        //    nftAllDataText.text = polygonNFTs.ToString();

        //    CustomNftMetaData customNftMetaData = JsonUtility.FromJson<CustomNftMetaData>(polygonNFTs.Result[0].Metadata);
        //    Debug.LogError(customNftMetaData);
        //    //StartCoroutine(GetTexture(customNftMetaData.image));
        //    //StartCoroutine(GetTexture(polygonNFTs.Result[0].TokenUri));
        //}

        IEnumerator GetTexture(string imageUrl)
        {
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(imageUrl))
            {
                yield return uwr.SendWebRequest();

                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log("Web Request Failed: " + uwr.error);
                }
                else
                {
                    Debug.Log("Web Request Successfuly");

                    var texture = DownloadHandlerTexture.GetContent(uwr);
                    nftImage.texture = texture;
                }
            }
        }

        public void LogoutButton_OnClicked()
        {
            // Logout the Moralis User.
            //authKit.Disconnect();

            authenticationKitObject.SetActive(true);
            congratulationUiObject.SetActive(false);
            fireworksObject.SetActive(false);
        }

        public void OnClick_ContinueButton()
        {
            Debug.Log("Disable Loading Screen");
            MGR.InevitableUI.instance.HideLoadingScreen();

            SceneManager.LoadScene("MenuScene");
        }
    }
}
