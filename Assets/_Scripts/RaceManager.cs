using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;
using Unity.VisualScripting;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Audio;
using System;
using UnityEngine.UI;

namespace MGR
{
    public class RaceManager : MonoBehaviourPunCallbacks
    {
        public GameOverUI gameOverUI;
        // Singleton
        private static RaceManager instance;
        public static RaceManager Instance { get { return instance; } }

        public List<Transform> playerOverheadPositionUI = new List<Transform>();
        [SerializeField] CameraChange camChange;
        [SerializeField] Camera carCam;

        [Header("RaceData")]
        [SerializeField] private List<GameObject> checkpoints = new List<GameObject>();
        public int CheckpointsCount { get { return checkpoints.Count; } }

        [SerializeField] private List<GameObject> playersList = new List<GameObject>();
        List<PlayerRaceData> sortedPlayerData = new List<PlayerRaceData>();

        [SerializeField] GameObject myPlayer;

        [SerializeField] private int totalLaps = 1;
        public int TotalLaps { get { return totalLaps; } }

        [Header("Race Info UI")]
        [SerializeField] private TMP_Text countdownText;
        [SerializeField] private TMP_Text raceModeText;
        [SerializeField] private TMP_Text[] nameText;
        [SerializeField] private GameObject[] namePanel;
        [SerializeField] private TMP_Text raceTimeText;
        [Tooltip("Enter total time in Seconds")]
        [SerializeField] private int raceTimeLimit;

        bool raceStarted = false;
        bool raceEnded = false;
        public bool RaceStarted { get { return raceStarted; } }
        public bool RaceEnded { get { return raceEnded; } }

        [Header("PauseMenu")]
        [SerializeField] GameObject pauseMenu;
        [SerializeField] GameObject settingsPanel;

        [SerializeField] Slider masterVolumeSlider;
        [SerializeField] Slider musicVolumeSlider;
        [SerializeField] Slider sfxVolumelSlider;
        AudioMixer masterAudioMixer;
        //bool isPaused = false;
        [SerializeField]Volume globalVolume;
        DepthOfField dofComponent;
        public DepthOfField depthOfField { get { return dofComponent; } }
        ChromaticAberration chromaticComponent;
        public ChromaticAberration chromaticAberration { get { return chromaticComponent; } }

        // Race Timer
        public struct LapTime
        {
            public float milliSecond;
            public int seconds;
            public int minutes;

            public LapTime(int _min, int _sec, float _frac)
            {
                milliSecond = _frac;
                seconds = _sec;
                minutes = _min;
            }

            public void ResetTime()
            {
                milliSecond = 0;
                seconds = 0;
                minutes = 0;
            }
        }

        public LapTime totalLapTime = new LapTime(0, 0, 0);

        private bool shouldStartLapTimer = false;


        private const int raceStartEventCode = 11;

        private void Awake()
        {
            instance = this;
            camChange = FindObjectOfType<CameraChange>();

            carCam = Camera.main;
            masterAudioMixer = AudioManager.instance.MasterAudioMixer;
        }

        // Start is called before the first frame update
        void Start()
        {
            InevitableUI.instance.ShowLoadingScreen();

            //Instantiate(DatabaseHandler.instance.mapsList[(int)GameManager.instance.map]);
            //Instantiate(GameManager.instance.selectedMapData.mapObject);
            GameObject CheckPointParent = GameObject.Find("CheckPoints");
            GameObject difficultyMode = GameObject.Find("Map" + GameManager.instance.difficultyMode.ToString());

            if (difficultyMode != null)
            {
                Debug.Log("Difficulty Mode working");
                for (int i = 0; i < difficultyMode.transform.childCount; i++)
                {
                    difficultyMode.transform.GetChild(i).gameObject.SetActive(true);
                }
            }
            else Debug.LogError("Difficulty Mode not working");
            for (int i = 0; i < CheckPointParent.transform.childCount; i++)
            {
                var child = CheckPointParent.transform.GetChild(i).AddComponent<TrackCheckpoint>();
                child.GetComponent<TrackCheckpoint>().checkPointNum = i + 1;
                checkpoints.Add(child.gameObject);
            }

            Invoke(nameof(GetPlayer), 3);

            raceModeText.text = GameManager.instance.gameMode.ToString();

            for (int i = 0; i < namePanel.Length; i++)
            {
                namePanel[i].SetActive(false);
            }

            globalVolume = GameObject.Find("Global Volume").GetComponent<Volume>();
            DepthOfField tmp;
            ChromaticAberration chromatictmp;
            if(globalVolume.profile.TryGet<DepthOfField>(out tmp))
            {
                dofComponent = tmp;
            }
            if(globalVolume.profile.TryGet<ChromaticAberration>(out chromatictmp))
            {
                chromaticComponent = chromatictmp;
            }
            getVolumeData();
        }

        private void Update()
        {
            //if (camChange.activeCamera)
            if (carCam)
            {
                foreach (var item in playerOverheadPositionUI)
                {
                    if (item != null)
                    {
                        //item.LookAt(camChange.activeCamera);
                        item.LookAt(carCam.transform);
                        item.eulerAngles += Vector3.up * 180;
                    }
                }
            }

            // Timer
            if (PhotonNetwork.IsMasterClient)
            {
                if (shouldStartLapTimer)
                {
                    totalLapTime.milliSecond += Time.deltaTime * 10f;

                    if (totalLapTime.milliSecond >= 10)
                    {
                        totalLapTime.milliSecond = 0;
                        totalLapTime.seconds += 1;
                    }
                    if (totalLapTime.seconds >= 60)
                    {
                        totalLapTime.seconds = 0;
                        totalLapTime.minutes += 1;
                    }

                    if (PhotonNetwork.IsConnectedAndReady)
                    {
                        var tableData = PhotonNetwork.CurrentRoom.CustomProperties;
                        tableData["TotalLapTimerMin"] = totalLapTime.minutes;
                        tableData["TotalLapTimerSec"] = totalLapTime.seconds;
                        tableData["TotalLapTimerMilli"] = totalLapTime.milliSecond;
                        PhotonNetwork.CurrentRoom.SetCustomProperties(tableData);
                    }
                }
            }
            ////Pause Menu 
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!isPaused)
                    OnPause();
                else
                    OnResume();
                isPaused = !isPaused;
            }
        }

        bool isPaused;

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            // Not sure about the exact API right now, it's not in their docs
            // it's either ContainsKey, HasKey or something similar ..maybe even TryGetValue works

            if (!PhotonNetwork.IsMasterClient)
            {
                totalLapTime.minutes = (int)PhotonNetwork.CurrentRoom.CustomProperties["TotalLapTimerMin"];
                totalLapTime.seconds = (int)PhotonNetwork.CurrentRoom.CustomProperties["TotalLapTimerSec"];
                totalLapTime.milliSecond = (float)PhotonNetwork.CurrentRoom.CustomProperties["TotalLapTimerMilli"];
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
            PlayerPrefs.SetFloat(AudioManager.MASTER_KEY, masterVolumeSlider.value);
            PlayerPrefs.SetFloat(AudioManager.MUSIC_KEY, musicVolumeSlider.value);
            PlayerPrefs.SetFloat(AudioManager.SFX_KEY, sfxVolumelSlider.value);
        }

        void GetPlayer()
        {
            var photonViews = UnityEngine.Object.FindObjectsOfType<PhotonView>();
            foreach (var view in photonViews)
            {
                var player = view.Owner;
                //Objects in the scene don't have an owner, its means view.owner will be null
                if (player != null)
                {
                    var playerPrefabObject = view.gameObject;
                    if (view.IsMine)
                    {
                        myPlayer = view.gameObject;
                    }

                    if (!playersList.Contains(playerPrefabObject))
                    {
                        playerOverheadPositionUI.Add(playerPrefabObject.GetComponent<PlayerRaceData>().posOverHeadText);
                        playersList.Add(playerPrefabObject);
                    }
                }
            }

            for (int i = 0; i < playersList.Count; i++)
            {
                sortedPlayerData.Add(playersList[i].GetComponent<PlayerRaceData>());
            }

            for (int i = 0; i < nameText.Length; i++)
            {
                if (i < sortedPlayerData.Count)
                {
                    sortedPlayerData[i].playerName = sortedPlayerData[i].gameObject.GetPhotonView().Owner.NickName;

                    namePanel[i].gameObject.SetActive(true);
                }
                else
                {
                    namePanel[i].gameObject.SetActive(false);
                }
            }

            InvokeRepeating(nameof(UpdateRaceStats), 0.5f, 0.5f);


            if (PhotonNetwork.IsMasterClient)
            {
                //SendStartRaceEvent();
                Invoke(nameof(SendStartRaceEvent), 2f);
            }

            //foreach (Player player in PhotonNetwork.PlayerList)
            //{
            //    if (player.IsMasterClient)
            //    {
            //        SendStartRaceEvent();
            //        //Invoke(nameof(SendStartRaceEvent), 2f);
            //    }
            //}
        }

        void OnEvent(EventData eventData)
        {
            if (eventData.Code > 199)
                return;
            Debug.Log("Event Recieved");
            if (eventData.Code == raceStartEventCode)
            {
                Debug.Log("CALLING EVENT");

                StartRaceText();
            }

        }

        private void SendStartRaceEvent()
        {
            object[] content = new object[] { };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(raceStartEventCode, content, raiseEventOptions, SendOptions.SendReliable);
        }


        private void StartRaceText()
        {
            InevitableUI.instance.HideLoadingScreen();

            StartCoroutine(StartCountdown(3));
            raceStarted = true;
        }

        private IEnumerator StartCountdown(int _count)
        {
            yield return new WaitForSeconds(1);

            countdownText.text = _count.ToString();

            if (_count > 0)
            {
                StartCoroutine(StartCountdown(_count - 1));
            }
            else
            {
                for (int i = 0; i < playersList.Count; i++)
                {
                    if (playersList[i].GetPhotonView().IsMine)
                    {
                        playersList[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

                        Debug.Log("CALLING PLAYER");
                        // Start Race Timer

                        raceTimeText.text = string.Format("{0:00}:{1:00}", raceTimeLimit / 60, raceTimeLimit % 60);
                        StartCoroutine(StartRaceTime(true));
                    }

                    sortedPlayerData[i].StartLapTimer(true);

                }

                countdownText.gameObject.SetActive(false);
            }

            shouldStartLapTimer = true;

        }

        private IEnumerator StartRaceTime(bool _isCounting)
        {
            yield return new WaitForSeconds(1);

            raceTimeLimit -= 1;

            raceTimeText.text = string.Format("{0:00}:{1:00}", raceTimeLimit / 60, raceTimeLimit % 60);

            if (raceTimeLimit <= 0)
            {
                // Time Over / End Race
                StopCoroutine(StartRaceTime(false));

                foreach (PlayerRaceData player in sortedPlayerData)
                {
                    player.RaceCompleted();
                }

            }
            else
            {
                StartCoroutine(StartRaceTime(true));
            }
        }

        private void UpdateRaceStats()
        {
            if (playersList.Count <= 0)
                return;

            sortedPlayerData.Sort((a, b) => compare(a, b));
            for (int i = 0; i < sortedPlayerData.Count; i++)
            {
                sortedPlayerData[i].UpdatePosition(i + 1, playersList.Count);

                // Update Player name in ui
                nameText[i].text = sortedPlayerData[i].gameObject.GetPhotonView().Owner.NickName;
            }
        }


        int compare(PlayerRaceData a, PlayerRaceData b)
        {
            if (a == null)
            {
                sortedPlayerData.Remove(a);
                return -1;
            }
            if (b == null)
            {
                sortedPlayerData.Remove(b);
                return -1;
            }

            int checkpointa = (a.CurrentCheckpoint + 100 * a.CurrentLap - 1) * CheckpointsCount;
            int checkpointb = (b.CurrentCheckpoint + 100 * b.CurrentLap - 1) * CheckpointsCount;
            if (checkpointa == checkpointb)
            {
                Vector3 nextCheckPointPos = GetCheckpointPosition(a.NextCheckpoint);
                int compare = Vector3.Distance(a.transform.position, nextCheckPointPos).CompareTo(Vector3.Distance(b.transform.position, nextCheckPointPos));
                return compare;
            }
            else
            {
                int compare = -1 * checkpointa.CompareTo(checkpointb);
                return compare;
            }
        }


        public Vector3 GetCheckpointPosition(int _index)
        {
            if (_index < checkpoints.Count)
                return checkpoints[_index].transform.position;
            else
                return checkpoints[0].transform.position;
        }

        public void RaceCompleted(GameObject _player, int _pos)
        {
            if (_player.GetPhotonView().IsMine)
            {
                countdownText.gameObject.SetActive(true);
                countdownText.text = $"Your Position {_pos}";
                raceStarted = false;
                raceEnded = true;
                MainGameScript.instance.SetRaceDataAndUpload(_pos);

                _player.TryGetComponent<VehicleControl>(out VehicleControl vehicleControl);
                if (vehicleControl != null)
                {
                    vehicleControl.StopController();
                }
            }
        }

        public void OnResume()
        {
                //isPaused = false;
                pauseMenu.SetActive(false);
                dofComponent.focusDistance.value = 6.95f;

                myPlayer.TryGetComponent<VehicleControl>(out VehicleControl vehicleControl);
                if (vehicleControl != null)
                {
                    vehicleControl.ActivateController();
                }
        }

        public void OnPause()
        {
                //isPaused = true;
                pauseMenu.SetActive(true);
                settingsPanel.SetActive(false);
                dofComponent.focusDistance.value = 0.1f;

                myPlayer.TryGetComponent<VehicleControl>(out VehicleControl vehicleControl);
                if (vehicleControl != null)
                {
                    vehicleControl.StopController();
                }
        }

        public void Onsettings()
        {
            pauseMenu.SetActive(false);
            settingsPanel.SetActive(true);
        }

        public void BackSettings()
        {
            pauseMenu.SetActive(true);
            settingsPanel.SetActive(false);
        }

        #region Options Menu
        void getVolumeData()
        {
            masterVolumeSlider.value = PlayerPrefs.GetFloat(AudioManager.MASTER_KEY, 1f);
            musicVolumeSlider.value = PlayerPrefs.GetFloat(AudioManager.MUSIC_KEY, 1f);
            sfxVolumelSlider.value = PlayerPrefs.GetFloat(AudioManager.SFX_KEY, 1f);
        }
        public void setMasterVolume(float value)
        {
            masterAudioMixer.SetFloat(AudioManager.Mixer_Master, MathF.Log10(value) * 20);
        }
        public void setMusicVolume(float value)
        {
            masterAudioMixer.SetFloat(AudioManager.Mixer_Music, MathF.Log10(value) * 20);
        }
        public void setSFXVolume(float value)
        {
            masterAudioMixer.SetFloat(AudioManager.Mixer_SFX, MathF.Log10(value) * 20);
        }
        public void ispostProcess(bool enable)
        {
            globalVolume.gameObject.SetActive(enable);
        }
        #endregion

        public void OnclickExit()
        {
            pauseMenu.SetActive(false);
            NetworkingScript.instance.CallLeaveLobby();
        }

        
    }
}