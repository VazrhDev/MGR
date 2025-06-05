using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;

namespace MGR
{

    public class LobbyScript : MonoBehaviourPunCallbacks
    {

        [SerializeField] Button startGameBtn;
        [SerializeField] TextMeshProUGUI playerNamesTextField;
        [SerializeField] TextMeshProUGUI gameModeTextField;
        [SerializeField] bool buttonpressed = false;

        public static LobbyScript instance;


        private void Awake()
        {
            instance = this;
            if (GameManager.instance)
            {
                if (!PhotonNetwork.IsMasterClient) startGameBtn.gameObject.SetActive(false);
                UpdatePlayerNames();
                //gameModeTextField.text = GameManager.instance.gameMode.ToString();
                //GameManager.instance.mySpawnPosition = PlayerEnteredSpawnPositionUpdate();
                //GameManager.instance.map = (Maps)System.Enum.Parse(typeof(Maps), PhotonNetwork.CurrentRoom.CustomProperties["Maps"].ToString());
                InevitableUI.instance.HideLoadingScreen();
                if(GameManager.instance.selectedMapData.mapData.mapName == "Dubai Autodrome" || true)
                {
                    StartCoroutine(LoadScene());
                }
            }
            else Invoke(nameof(Awake), 0.01f);

        }

        IEnumerator LoadScene()
        {
            yield return null;

            //Begin to load the Scene you specify
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(GameManager.instance.selectedMapData.mapData.mapCode);
            Debug.Log("Pro :" + GameManager.instance.selectedMapData.mapData.mapCode);
            //Don't let the Scene activate until you allow it to
            asyncOperation.allowSceneActivation = false;
            Debug.Log("Pro :" + asyncOperation.progress);
            //When the load is still in progress, output the Text and progress bar
            while (!asyncOperation.isDone)
            {
                //Output the current progress

                // Check if the load has finished
                if (asyncOperation.progress >= 0.9f)
                {
                    if (!startGameBtn.interactable)
                        EnableConfirmBtn();
                    if (buttonpressed)
                    {
                        asyncOperation.allowSceneActivation = true;
                        loadMapScene();
                        buttonpressed = false;
                    }
                }
                yield return null;
            }

        }

        void EnableConfirmBtn()
        {
            Debug.Log("Enabled");
            startGameBtn.interactable = true;
        }

        //private void OnEnable()
        //{
        //    PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        //}

        //private void OnDisable()
        //{
        //    PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        //}

        public void UpdatePlayerNames()
        {
            playerNamesTextField.text = "";
            foreach (var item in PhotonNetwork.CurrentRoom.Players)
            {
                playerNamesTextField.text += item.Value.NickName + "\n";
            }
            startGameBtn.interactable = PhotonNetwork.CurrentRoom.PlayerCount > 1 ? true : false;
        }

        public void StartGame()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;
                //PhotonNetwork.LoadLevel("LobbyTest");
                LoadGame();
                object[] content = new object[] { };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent((byte)NetworkingScript.instance.leavingLobbyEventCode, content, raiseEventOptions, SendOptions.SendReliable);
            }

        }

        bool loadGameCalled = false;

        public void LoadGame()
        {
            if (!loadGameCalled)
            {
                loadGameCalled = true;
                Debug.LogError("Button not pressed");
                buttonpressed = true;
                InevitableUI.instance.ShowLoadingScreen();
            }
            else Debug.LogError("Button already pressed");
        }

        void loadMapScene()
        {
            SceneManager.LoadScene("LobbyTest", LoadSceneMode.Additive);
        }

        void OnEvent(EventData eventData)
        {
            if (eventData.Code > 199)
                return;

            //if (eventData.Code == leavingLobbyEventCode)
            //{
            //    Debug.LogError("Event is here");
            //    CallLoadingScreenEvent();
            //}
        }

        private void CallLoadingScreenEvent()
        {
            InevitableUI.instance.ShowLoadingScreen();
        }

        public void LeaveLobby()
        {
            NetworkingScript.instance.CallLeaveLobby();
        }

        #region Spawn Point Implementation

        //public void PlayerLeftSpawnPositionUpdate(int playerLeftPos)
        //{
        //    Debug.Log("Current Spawn Positions Available: " + PhotonNetwork.CurrentRoom.CustomProperties["SpawnPositions"]);
        //    ExitGames.Client.Photon.Hashtable temp = PhotonNetwork.CurrentRoom.CustomProperties;
        //    string abc = temp["SpawnPositions"].ToString();
        //    string[] temp1 = abc.Split(',');
        //    List<int> bulla = new List<int>();
        //    foreach (var item in temp1)
        //    {
        //        bulla.Add(int.Parse(item));
        //    }
        //    bulla.Add(playerLeftPos);
        //    bulla.Sort();
        //    string finalString = "";
        //    for (int i = 0; i < bulla.Count; i++)
        //    {
        //        finalString += bulla[i].ToString();
        //        if (i != bulla.Count - 1)
        //        {
        //            finalString += ",";
        //        }
        //    }
        //    temp["SpawnPositions"] = finalString;
        //    PhotonNetwork.CurrentRoom.SetCustomProperties(temp);
        //    Debug.Log("Updated Spawn Positions Available: " + PhotonNetwork.CurrentRoom.CustomProperties["SpawnPositions"]);
        //}

        //public int PlayerEnteredSpawnPositionUpdate()
        //{
        //    Debug.Log("Current Spawn Positions Available: " + PhotonNetwork.CurrentRoom.CustomProperties["SpawnPositions"]);
        //    ExitGames.Client.Photon.Hashtable temp = PhotonNetwork.CurrentRoom.CustomProperties;
        //    string abc = temp["SpawnPositions"].ToString();
        //    string[] temp1 = abc.Split(',');
        //    List<int> bulla = new List<int>();
        //    foreach (var item in temp1)
        //    {
        //        bulla.Add(int.Parse(item));
        //    }

        //    if (bulla.Count > 1)
        //    {
        //        int temp2 = bulla[0];
        //        if (bulla.Contains(temp2))
        //        {
        //            bulla.Remove(temp2);
        //        }
        //        string finalString = "";
        //        for (int i = 0; i < bulla.Count; i++)
        //        {
        //            finalString += bulla[i].ToString();
        //            if (i != bulla.Count - 1)
        //            {
        //                finalString += ",";
        //            }
        //        }
        //        temp["SpawnPositions"] = finalString;
        //        PhotonNetwork.CurrentRoom.SetCustomProperties(temp);
        //        Debug.Log("Updated Spawn Positions Available: " + PhotonNetwork.CurrentRoom.CustomProperties["SpawnPositions"]);
        //        Debug.Log("Found Spawn Position: " + temp2);
        //        return temp2;
        //    }
        //    else
        //    {
        //        Debug.Log("Not found spawn Position, returning -1");
        //        return -1;
        //    }

        //}

        #endregion


        #region Pun Callbacks

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            UpdatePlayerNames();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            UpdatePlayerNames();
        }

        #endregion
    }
}


