using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;

namespace MGR
{
    public class NetworkingScript : MonoBehaviourPunCallbacks
    {

        public static NetworkingScript instance;

        public bool automaticallyStartGame = false;

        public bool isQuickie = false;

        public int leavingLobbyEventCode = 12;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        private void OnEnable()
        {
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDisable()
        {
            PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        void OnEvent(EventData eventData)
        {
            if (eventData.Code > 199)
                return;

            Debug.LogError("Event: " + eventData.Code);
            if (eventData.Code == leavingLobbyEventCode)
            {
                Debug.LogError("Event is here");
                LobbyScript.instance.LoadGame();
            }
        }

        //[ContextMenu("LEave Lobbby")]
        //void LeaveLobby()
        //{
        //    PhotonNetwork.LeaveLobby();
        //}

        void Start()
        {
            //PhotonNetwork.LocalPlayer.NickName = "Player" + Random.Range(0, 10000);
            PhotonNetwork.AutomaticallySyncScene = true;
            Debug.Log("Connecting using settings");
            PhotonNetwork.ConnectUsingSettings();
        }

        public void SetPlayerName(string playerName)
        {
            Debug.Log("Setting Player Name: " + playerName);
            PhotonNetwork.LocalPlayer.NickName = playerName;
        }

        public void CreateOrJoinRoom()
        {
            Debug.Log(PhotonNetwork.InLobby);
            Debug.Log(PhotonNetwork.IsConnectedAndReady);
            Debug.Log(PhotonNetwork.IsConnected);
            if (PhotonNetwork.InLobby)
            {
                Debug.Log(PhotonNetwork.CurrentLobby.Name);
                automaticallyStartGame = false;
                ExitGames.Client.Photon.Hashtable expectedCustomProperties;
                if (isQuickie)
                {
                    expectedCustomProperties =
                        new ExitGames.Client.Photon.Hashtable() { };
                }
                else if (GameManager.instance.onRandomMap)
                    expectedCustomProperties =
                        new ExitGames.Client.Photon.Hashtable() { { "GameMode", GameManager.instance.gameMode.ToString() } };
                else if (GameManager.instance.gameMode == GameMode.Practice)
                {
                    expectedCustomProperties =
                    new ExitGames.Client.Photon.Hashtable() { { "GameMode", GameManager.instance.gameMode.ToString() }, { "Maps", GameManager.instance.selectedMapData.ToString() },
                    { "Difficulty", GameManager.instance.difficultyMode.ToString() }};
                }
                else
                {
                    expectedCustomProperties =
                    new ExitGames.Client.Photon.Hashtable() { { "GameMode", GameManager.instance.gameMode.ToString() }, { "Maps", GameManager.instance.selectedMapData.ToString() } };
                }
                InevitableUI.instance.LogOnLoadingScreen("Joining Lobby");
                PhotonNetwork.JoinRandomOrCreateRoom(expectedCustomProperties, 20, MatchmakingMode.FillRoom, TypedLobby.Default, null, null, GetRoomOptions());
            }
            else
            {
                automaticallyStartGame = true;

                if(!PhotonNetwork.IsConnectedAndReady)
                {
                    PhotonNetwork.ConnectUsingSettings();
                }

                if (PhotonNetwork.InRoom)
                {
                    Debug.Log("IN ROOM");
                }
                else
                {
                    Debug.Log("NOT IN ROOM");

                }

                Debug.Log("Else");
                //PhotonNetwork.JoinLobby();
            }
        }

        public void CallLeaveLobby()
        {
            Debug.Log("BEFORE LEAVING: " + PhotonNetwork.InLobby);
            Debug.Log("BEFORE LEAVING ROOM :" + PhotonNetwork.InRoom);

            if (PhotonNetwork.InLobby)
            {
                Debug.Log("Leaving Lobby");
                PhotonNetwork.LeaveLobby();
            }
            else
            {
                if (PhotonNetwork.InRoom)
                {
                    Debug.Log("Leaving room");
                    PhotonNetwork.LeaveRoom();
                }
            }

            Debug.Log("Leave Lobby");

            Debug.Log("BEFORE LEAVING: " + PhotonNetwork.InLobby);
            Debug.Log("BEFORE LEAVING:" + PhotonNetwork.InRoom);

            //PhotonNetwork.LoadLevel("MenuScene");

            //PhotonNetwork.Disconnect();
            //SceneManager.LoadScene("MenuScene");

            //PlayfabManager.instance.DisableLoginPanel();

            //StartCoroutine(LeaveRoom());
        }


        #region Pun Callbacks

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            Debug.Log("Connected to Master -- Trying to Join Lobby");
            PhotonNetwork.JoinLobby();
        }

        public override void OnErrorInfo(ErrorInfo errorInfo)
        {
            base.OnErrorInfo(errorInfo);
            Debug.Log("Error info");
        }

        public override void OnConnected()
        {
            base.OnConnected();
            Debug.Log("On Connected");
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            Debug.Log("Left Room");

            if (SceneManager.GetActiveScene().name != "MenuScene")
            {
                SceneManager.LoadScene("MenuScene");

                //if (PhotonNetwork.CurrentRoom.PlayerCount <= 1)
                //{
                //    PhotonNetwork.DestroyAll();
                //}

                //PlayfabManager.instance.ReloadGame();
            }
        }

        public override void OnLeftLobby()
        {
            base.OnLeftLobby();
            Debug.Log("Left Lobby");

            if (SceneManager.GetActiveScene().name == "LobbyTest")
            {
                SceneManager.LoadScene("MenuScene");
            }
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("Joined Lobby Successfully");
            if (automaticallyStartGame) CreateOrJoinRoom();
        }

        public override void OnCreatedRoom()
        {
            PhotonNetwork.LoadLevel("Lobby");
        }
        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            if(SceneManager.GetActiveScene().name == "MenuScene")
            {
                Debug.Log("ON JOINED ROOM IN MENU");
                if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
                {
                    PhotonNetwork.LoadLevel("Lobby");
                }
                    
            }
        }

        RoomOptions GetRoomOptions()
        {
            RoomOptions roomOps;
            if (GameManager.instance.gameMode == GameMode.Practice)
            {
                roomOps = new RoomOptions()
                {
                    IsVisible = true,
                    IsOpen = true,
                    MaxPlayers = 20,
                    CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "GameMode", GameManager.instance.gameMode.ToString() },
                {"SpawnPositions","0,1,2,3,4,5,6,7,8,9" },{"Maps", GameManager.instance.selectedMapData.ToString() },
                    {"Difficulty", GameManager.instance.difficultyMode.ToString() }}
                };
                roomOps.CustomRoomPropertiesForLobby = new string[]
                {
                    "GameMode",
                    "SpawnPositions",
                    "Maps",
                    "Difficulty"
                };
            }
            else
            {
                roomOps = new RoomOptions()
                {
                    IsVisible = true,
                    IsOpen = true,
                    MaxPlayers = 20,
                    CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "GameMode", GameManager.instance.gameMode.ToString() },
                {"SpawnPositions","0,1,2,3,4,5,6,7,8,9" },{"Maps", GameManager.instance.selectedMapData.ToString() } }
                };
                roomOps.CustomRoomPropertiesForLobby = new string[]
                {
                    "GameMode",
                    "SpawnPositions",
                    "Maps"
                };
            }
            return roomOps;
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            return;
            string roomName = "Room" + Random.Range(0, 10000) + Random.Range(0, 10000) + Random.Range(0, 10000) + Random.Range(0, 10000);
            Debug.LogError("Join random failed");
            RoomOptions roomOps;

            roomOps = GetRoomOptions();
            Debug.Log("RoomName: " + roomName);
            Debug.Log("GameMode: " + GameManager.instance.gameMode.ToString());
            Debug.Log("Maps: " + GameManager.instance.selectedMapData.ToString());

            InevitableUI.instance.LogOnLoadingScreen("Creating New Lobby");

            // Adding Race timer custom property
            roomOps.CustomRoomProperties.Add("TotalLapTimerMin", 0);
            roomOps.CustomRoomProperties.Add("TotalLapTimerSec", 0);
            roomOps.CustomRoomProperties.Add("TotalLapTimerMilli", 0.0f);

            roomOps.BroadcastPropsChangeToAll = true;
            roomOps.CleanupCacheOnLeave = false;

            PhotonNetwork.CreateRoom(roomName, roomOps);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            InevitableUI.instance.HideLoadingScreenWithPopup(message, 2);
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            base.OnMasterClientSwitched(newMasterClient);
            Debug.Log("Master client switched");
        }

        #endregion
    }
}