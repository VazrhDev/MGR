using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using Newtonsoft.Json;
using System;
using Photon.Pun;

namespace MGR
{

    [Serializable]
    public struct IgnoreList
    {
        public string carName;
        public bool shouldIgnore;
    }

    public enum CheckDateTimeType
    {
        BeforeStartingDRM,
        IncreaseDRMPlays
    }

    public class PlayfabManager : MonoBehaviour
    {
        public bool automaticLogin = false;
        public bool usingWalletConnection = false;

        public static PlayfabManager instance;

        [SerializeField] private GameObject LoginPanel;
        [SerializeField] private TMP_InputField emailInput;
        [SerializeField] private TMP_InputField usernameInput;
        [SerializeField] private TMP_InputField passwordInput;
        [SerializeField] private TMP_Text messageText;

        [SerializeField] private GameObject playerTagPanel;
        [SerializeField] private TMP_InputField playerTagInput;
        ProfanityFilter profanityFilter;
        [SerializeField] private TMP_Text warningText;
        //[SerializeField] private TMP_Text playerTagDisplayText;

        public string playerTag { get; private set; }

        [Header("Dev Mode")]
        public bool isDevModeOn = true;
        public bool ignoreAllBundles = false;
        public bool ignoreMapsOnly = false;
        public string[] maps;
        public GameObject[] cars;
        public GameObject defaultCar;
        public GameObject car1;
        public GameObject car2;
        public GameObject carRenders;
        public GameObject nFTRenders;
        public GameObject wheelRenders;
        public GameObject decals;
        public GameObject mapRenders;

        public CheckDateTimeType checkDateTimeType;

        private void Awake()
        {
            if (instance == null) instance = this;
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            Invoke(nameof(SplashScreenTimer), 2);
            profanityFilter = GetComponent<ProfanityFilter>();
        }

        public void ReloadGame()
        {
            Invoke(nameof(SplashScreenTimer), 2);
        }

        void SplashScreenTimer()
        {
            //var checkProfileRequest = new GetAccountInfoRequest
            //{
            //    Email = WalletData.instance.id
            //};
            //PlayFabClientAPI.GetAccountInfo(checkProfileRequest, CheckAccountInfo, OnError);

            InevitableUI.instance.HideSplashScreen();
            if (automaticLogin && !usingWalletConnection)
            {
                var request = new LoginWithEmailAddressRequest
                {
                    Email = "name@name.com",
                    Password = "123456",
                    InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                    {
                        GetPlayerProfile = true
                    }
                };

                InevitableUI.instance.ShowLoadingScreen();
                InevitableUI.instance.LogOnLoadingScreen("Logging in!");
                PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
            }

            else if (automaticLogin && usingWalletConnection)
            {
                var request = new LoginWithCustomIDRequest
                {
                    CustomId = WalletData.instance.id,
                    CreateAccount = true,
                    InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                    {
                        GetPlayerProfile = true
                    }
                };

                InevitableUI.instance.ShowLoadingScreen();
                InevitableUI.instance.LogOnLoadingScreen("Logging in!");
                PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnError);

                //StartCoroutine(GetAssetBundle());
            }
        }

        public void DisableLoginPanel()
        {
            LoginPanel.SetActive(false);
        }

        #region Get Current Time

        public void CheckDateTime()
        {
            PlayFabClientAPI.GetTime(new GetTimeRequest(), OnGetTimeSuccess, LogFailure);
        }

        private void LogFailure(PlayFabError error)
        {
            Master.ColoredLog("yellow", "Fetching time error: " + error.GenerateErrorReport());
        }

        private void OnGetTimeSuccess(GetTimeResult result)
        {
            Master.ColoredLog("yellow", "The time is: " + result.Time.Year);
            Master.ColoredLog("yellow", "The time is: " + result.Time.DayOfYear);

            bool canStartDRM = true;
            //GameManager.instance.currentDRMData = null;
            for (int i = 0; i < DatabaseHandler.instance.playerData.dailyRaceData.Count; i++)
            {
                if (DatabaseHandler.instance.playerData.dailyRaceData[i].year == result.Time.Year)
                {
                    if (DatabaseHandler.instance.playerData.dailyRaceData[i].day == result.Time.DayOfYear)
                    {
                        //GameManager.instance.currentDRMData = new DRData(DatabaseHandler.instance.playerData.dailyRaceData[i].year,
                        //    DatabaseHandler.instance.playerData.dailyRaceData[i].day, DatabaseHandler.instance.playerData.dailyRaceData[i].timesPlayed);
                        if (DatabaseHandler.instance.playerData.dailyRaceData[i].timesPlayed >= DatabaseHandler.instance.masterData.DRModeLimit)
                        {
                            canStartDRM = false;
                        }
                    }
                }
            }

            //if (GameManager.instance.currentDRMData == null)
            //{
            //    GameManager.instance.currentDRMData = new DRData(result.Time.Year, result.Time.DayOfYear, 0);
            //}

            if (checkDateTimeType == CheckDateTimeType.BeforeStartingDRM)
                GameManager.instance.menuScript.DailyModeStart(canStartDRM);
            else if (checkDateTimeType == CheckDateTimeType.IncreaseDRMPlays)
                DatabaseHandler.instance.IncreaseDRMPlays(result.Time);
        }

        #endregion

        #region Login

        void Login()
        {
            var request = new LoginWithCustomIDRequest
            {
                CustomId = SystemInfo.deviceUniqueIdentifier,
                CreateAccount = true
            };

            PlayFabClientAPI.LoginWithCustomID(request, OnSuccess, OnError);

            StartCoroutine(GetAssetBundle());
        }

        void OnSuccess(LoginResult result)
        {
            Debug.Log("Logged In Successfully!");
        }

        void OnError(PlayFabError error)
        {
            Debug.Log(error.GenerateErrorReport());
            Debug.Log(error.ErrorMessage);
            InevitableUI.instance.HideLoadingScreenWithPopup(error.GenerateErrorReport(), 2);

            Debug.Log("New Player ID" + WalletData.instance.id);
            if (error.ErrorMessage.Contains("Invalid input parameters"))
            {
                Debug.Log("Error Message Contains the string");
                var request = new RegisterPlayFabUserRequest
                {
                    Email = WalletData.instance.id,
                    Password = WalletData.instance.id,
                };

                PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
            }
        }

        public void OnClick_RegisterButton()
        {
            if (passwordInput.text.Length < 6)
            {
                messageText.text = "Password Too Short!";
                return;
            }

            var request = new RegisterPlayFabUserRequest
            {
                Email = emailInput.text,
                Password = passwordInput.text,
                Username = usernameInput.text,
                RequireBothUsernameAndEmail = true
            };

            PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
        }

        void OnRegisterSuccess(RegisterPlayFabUserResult result)
        {
            messageText.text = "Account Created Successfully";
        }

        public void OnClick_LoginButton()
        {
            var request = new LoginWithEmailAddressRequest
            {
                Email = emailInput.text,
                Password = passwordInput.text,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                {
                    GetPlayerProfile = true
                }
            };

            PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
        }

        void OnLoginSuccess(LoginResult result)
        {
            messageText.text = "Logged In!";
            InevitableUI.instance.AddInProgressOnLoadingScreen(5);
            LoginPanel.SetActive(false);
            if (result.InfoResultPayload != null)
            {
                if (result.InfoResultPayload.PlayerProfile != null)
                    Debug.Log("USERNAME: " + result.InfoResultPayload.PlayerProfile.DisplayName);

            }

            if (result.InfoResultPayload.PlayerProfile != null && result.InfoResultPayload.PlayerProfile.DisplayName != null)
            {
                Debug.Log("Player Profile is not null");

                playerTagPanel.SetActive(false);
                NetworkingScript.instance.SetPlayerName(result.InfoResultPayload.PlayerProfile.DisplayName);
                playerTag = result.InfoResultPayload.PlayerProfile.DisplayName;
            }
            else
            {
                Debug.Log("Player Profile is null");

                playerTagPanel.SetActive(true);
                return;
            }
            if(LogSaver.instance)
            LogSaver.instance.canStartLogging = true;
            InevitableUI.instance.LogOnLoadingScreen("Log in Success");
            GetTitleData();
            GetPlayerSavedData();
            
        }

        public void OnClick_ResetPasswordButton()
        {
            var request = new SendAccountRecoveryEmailRequest
            {
                Email = emailInput.text,
                TitleId = "7FB68"
            };

            PlayFabClientAPI.SendAccountRecoveryEmail(request, OnPasswordReset, OnError);
        }

        void OnPasswordReset(SendAccountRecoveryEmailResult result)
        {
            messageText.text = "Reset mail sent! Check your email.";
        }

        IEnumerator GetAssetBundle()
        {
            yield return new WaitForSeconds(2f);

            if (BundleWebLoader.instance.urlList == null)
            {
                StartCoroutine(GetAssetBundle());
                yield return null;
            }
            InevitableUI.instance.AddInProgressOnLoadingScreen(5);
            BundleWebLoader.instance.CallGetAssetBundle();
        }

        public void OnClick_SubmitPlayerTag()
        {
            if(playerTagInput.text.Length < 3 || playerTagInput.text.Length > 25)
            {
                warningText.text = "PLayer Name Length Should between 3 & 25";
                return;
            }
            if (profanityFilter.FilterText(playerTagInput))
            {
                warningText.text = "Please Dont Not use Explicit Names!!";
                playerTagInput.text = "";
                return;
            }
            var request = new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = playerTagInput.text
            };

            playerTag = playerTagInput.text;

            PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdate, OnError);

        }

        void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result)
        {
            playerTagPanel.SetActive(false);

            SplashScreenTimer();
        }

        #endregion

        #region TitleData

        void GetTitleData()
        {
            PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), OnTitleDataRecieved, OnError);
        }

        void OnTitleDataRecieved(GetTitleDataResult result)
        {
            if (result == null)
            {
                Debug.Log("No File Found");
                return;
            }

            //Debug.Log("RESULT: " + result.Data["commoncar"]);

            foreach(var item in result.Data.Keys)
            {
                if (item == "MasterData")
                {
                    BundleWebLoader.instance.CallGetMasterData(result.Data[item]);

                    Master.ColoredLog("red", item);

                }
                else
                {
                    Master.ColoredLog("green", item);
                    if (isDevModeOn && ignoreMapsOnly)
                    {
                        if (!item.Contains("DCM") && !item.Contains("map1") && !item.Contains("map2"))
                            BundleWebLoader.instance.urlList.Add(result.Data[item]);
                    }
                    else if(!isDevModeOn || (isDevModeOn && !ignoreAllBundles))
                    {
                        if (Application.platform == RuntimePlatform.WebGLPlayer || Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
                        {
                            if (string.Compare(item, "WebGL", true) == 0)
                            {
                                string[] temp = result.Data[item].Split('%');

                                for (int i = 0; i < temp.Length; i++)
                                {
                                    BundleWebLoader.instance.urlList.Add(temp[i]);
                                }
                            }
                        }

                        //BundleWebLoader.instance.urlList.Add(result.Data[item]);
                    }
                }
            }
            StartCoroutine(GetAssetBundle());
            InevitableUI.instance.AddInProgressOnLoadingScreen(5);
        }

        private void OnDataSend(UpdateUserDataResult result)
        {
            Debug.Log("Data Sent Successfully");
        }

        public void GetPlayerSavedData()
        {
            PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnCharacterDataRecieved, OnError);
            //PlayFabClientAPI.GetUserPublisherData()
        }

        private void OnCharacterDataRecieved(GetUserDataResult result)
        {
            Debug.Log("Data Recieved Successfully");
            InevitableUI.instance.LogOnLoadingScreen("Player Data Loaded Successfully");
            if (result.Data.ContainsKey("PlayerData"))
                DatabaseHandler.instance.playerData = JsonConvert.DeserializeObject<PlayerData>(result.Data["PlayerData"].Value);
            else DatabaseHandler.instance.playerData = new PlayerData();
            if (result.Data.ContainsKey("PlayerStats"))
                DatabaseHandler.instance.playerStats = JsonConvert.DeserializeObject<PlayerStats>(result.Data["PlayerStats"].Value);
            else DatabaseHandler.instance.playerStats = new PlayerStats();
            //DatabaseHandler.instance.playerData.carsOwned = new List<CarDetails>();
            if (DatabaseHandler.instance.savePlayerDataLocally)
                PlayerData.instance = DatabaseHandler.instance.playerData;
            InevitableUI.instance.AddInProgressOnLoadingScreen(5);
        }


        #endregion

    }
}
