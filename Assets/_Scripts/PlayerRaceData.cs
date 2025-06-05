using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

namespace MGR
{
    public class PlayerRaceData : MonoBehaviour
    {

        [Header("Position and Lap Texts")]
        [SerializeField] private TMP_Text posText;
        [Header("Time Stats Texts")]
        [SerializeField] private TMP_Text totalTimeText;
        [SerializeField] private TMP_Text currentLapTimeText;
        [Header("Race Distances Texts and Warning")]
        [SerializeField] private GameObject wrongWayMessage;
        [SerializeField] private TMP_Text checkpointDistanceText;
        [SerializeField] private TMP_Text racePercentageText;
        [Header("Lap indications")]
        [SerializeField] private TMP_Text lapText;
        [SerializeField] private TMP_Text lapPopUp;

        [Header("MiniMap Position Icons")]
        [SerializeField] private GameObject playerIcon;
        [SerializeField] private GameObject otherPlayersIcon;

        public Transform posOverHeadText;

        private int currentLap = 1;
        private int totalLaps;
        public int CurrentLap { get { return currentLap; } }
        private int currentCheckpoint = 0;
        public int CurrentCheckpoint { get { return currentCheckpoint; } }

        [Header("Misc")]
        public int position = 0;

        private int nextCheckpoint = 1;
        public int NextCheckpoint { get { return nextCheckpoint; } }

        private bool isRaceFinished = false;

        public string playerName;
        private string totalTime;

        float totalRaceDistance = 0;
        float distanceCovered = 0;

        List<float> checkpointsDistanceList = new List<float>();

        [SerializeField] private GameObject raceStartTimeline;

        // Pause Menu
        bool isPaused = false;

        struct LapTime
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

        LapTime currentLapTime = new LapTime(0, 0, 0);
        LapTime totalLapTime = new LapTime(0, 0, 0);

        private bool shouldStartLapTimer = false;

        int nextCheckPointDis = 0;
        int prevCheckPointDis = 0;
        [SerializeField] int offsetPrevCheckPointDis = 0;

        private void Awake()
        {
            Debug.Log("Enable Player race data");

            //Invoke(nameof(EnableScript), 0.1f);
        }

        // Start is called before the first frame update
        void Start()
        {
            if (!GetComponent<PhotonView>().IsMine)
                return;

            StartCoroutine(PlayRaceStartTimeline(true, 4f));

            InvokeRepeating(nameof(CheckDirection), 10.0f, 2.0f);
            StartCoroutine(Blinking());
            prevCheckPointDis = (int)Vector3.Distance(transform.position, RaceManager.Instance.GetCheckpointPosition(nextCheckpoint));

            CalculateTotalRaceDistance();

            totalLaps = RaceManager.Instance.TotalLaps;
            lapText.text = currentLap + " / " + totalLaps;
        }

        private void Update()
        {
            if (!isRaceFinished)
            {
                totalTimeText.text = string.Format("{0:00}:{1:00}:{2:00}", RaceManager.Instance.totalLapTime.minutes, RaceManager.Instance.totalLapTime.seconds, RaceManager.Instance.totalLapTime.milliSecond);

                currentLapTimeText.text = string.Format("{0:00}:{1:00}:{2:00}", RaceManager.Instance.totalLapTime.minutes, RaceManager.Instance.totalLapTime.seconds, RaceManager.Instance.totalLapTime.milliSecond);


                PhotonView.Get(this).RPC(nameof(UpdateTimer), RpcTarget.All);
            }

            //Update Checkpoint Distance 
            Vector3 currentCheckpointPosition = RaceManager.Instance.GetCheckpointPosition(currentCheckpoint);
            float checkpointDistance = Vector3.Distance(transform.position, currentCheckpointPosition);
            checkpointDistanceText.text = $"{(int)checkpointDistance}m";


            // Calculating Race Percentage
            float distanceLeft = 0;
            //Adding Remaining Checkpoints distance in current lap
            for (int i = currentCheckpoint + 1; i < checkpointsDistanceList.Count; i++)
            {
                distanceLeft += checkpointsDistanceList[i];
            }

            // Adding Checkpoints Distance for remaining laps
            for (int i = currentLap; i < RaceManager.Instance.TotalLaps; i++)
            {
                distanceLeft = distanceLeft + (totalRaceDistance / RaceManager.Instance.TotalLaps);
            }

            // Adding current checkpoint distance for player's real time position
            distanceLeft += checkpointDistance;
            distanceCovered = totalRaceDistance - distanceLeft;        // Calculating remaining distance 


            int racePercentage = (int)((distanceCovered / totalRaceDistance) * 100);

            racePercentage = Mathf.Clamp(racePercentage, 0, 100);

            racePercentageText.text = $"{racePercentage}%";


            ////Pause Menu 
            //if (Input.GetKeyDown(KeyCode.Escape))
            //{
            //    if (!isPaused)
            //        RaceManager.Instance.OnPause(this.gameObject);
            //    else
            //        RaceManager.Instance.OnResume(this.gameObject);

            //    isPaused = !isPaused;
            //}
        }

        [PunRPC]
        void UpdateTimer()
        {
            totalTime = string.Format("{0:00}:{1:00}:{2:00}", RaceManager.Instance.totalLapTime.minutes, RaceManager.Instance.totalLapTime.seconds, RaceManager.Instance.totalLapTime.milliSecond);
        }

        private void EnableScript()
        {
            this.enabled = true;
        }

        void CalculateTotalRaceDistance()
        {
            for (int i = 0; i < RaceManager.Instance.CheckpointsCount; i++)
            {
                float checkpointDistance = 0;

                if (i == 0)
                {
                    checkpointDistance = Vector3.Distance(transform.position, RaceManager.Instance.GetCheckpointPosition(i));
                    checkpointsDistanceList.Add(checkpointDistance);
                }
                else
                {
                    checkpointDistance = Vector3.Distance(RaceManager.Instance.GetCheckpointPosition(i - 1), RaceManager.Instance.GetCheckpointPosition(i));

                    checkpointsDistanceList.Add(checkpointDistance);
                }

                totalRaceDistance += checkpointDistance;
            }

            totalRaceDistance *= RaceManager.Instance.TotalLaps;
        }

        public void StartLapTimer(bool _status)
        {
            Debug.Log("START LAP TIMER");

            shouldStartLapTimer = _status;

            TryGetComponent<PhotonView>(out PhotonView photonView);
            if (photonView)
            {
                if (photonView.IsMine)
                {
                    ActivateRaceIcon(true);
                }
                else
                {
                    Debug.Log("TRYING TO ACTIVATE OTHER PLAYER ICON");
                    ActivateRaceIcon(false);
                }
            }
        }


        public void UpdatePosition(int _pos, int _totalPlayers)
        {
            position = _pos;
            posText.text = _pos + " / " + _totalPlayers;
            posOverHeadText.GetComponent<TextMesh>().text = _pos.ToString();
        }

        public void UpdateCheckpoint()
        {
            // Reset Checkpoint & increase lap when checkpoints are completed
            if (RaceManager.Instance.CheckpointsCount == nextCheckpoint)
            {
                currentLap += 1;
                currentCheckpoint = 0;
                nextCheckpoint = 1;

                currentLapTime.ResetTime();
                lapText.text = currentLap + " / " + totalLaps;
                StartCoroutine(LapNotificationPopup());

            }
            // update current checkpoint and next checkpoint
            else
            {
                currentCheckpoint = (currentCheckpoint + 1);
                nextCheckpoint = (nextCheckpoint + 1);
                prevCheckPointDis = (int)Vector3.Distance(transform.position, RaceManager.Instance.GetCheckpointPosition(nextCheckpoint));

            }

            if (RaceManager.Instance.TotalLaps + 1 == currentLap)
            {
                // Race Ends here
                RaceCompleted();
            }
        }


        void CheckDirection()
        {
            nextCheckPointDis = (int)Vector3.Distance(transform.position, RaceManager.Instance.GetCheckpointPosition(nextCheckpoint));

            if (nextCheckPointDis <= prevCheckPointDis)
            {
                wrongWayMessage.SetActive(false);
                prevCheckPointDis = nextCheckPointDis;
            }
            else
            {
                if (prevCheckPointDis + offsetPrevCheckPointDis <= nextCheckPointDis)
                {
                    wrongWayMessage.SetActive(true);
                }
                else
                {
                    wrongWayMessage.SetActive(false);
                }
            }
        }

        IEnumerator Blinking()
        {
            TMP_Text temp = wrongWayMessage.GetComponent<TMP_Text>();
            while (true)
            {
                switch (temp.color.a.ToString())
                {
                    case "0":
                        temp.color = new Color(temp.color.r, temp.color.g, temp.color.b, 1);
                        yield return new WaitForSeconds(0.5f);
                        break;
                    case "1":
                        temp.color = new Color(temp.color.r, temp.color.g, temp.color.b, 0);
                        yield return new WaitForSeconds(0.5f);
                        break;
                }
            }
        }

        IEnumerator LapNotificationPopup()
        {
            if (currentLap == totalLaps)
            {
                lapPopUp.text = "Last Lap";

                lapPopUp.gameObject.transform.parent.gameObject.SetActive(true);
                yield return new WaitForSeconds(1.0f);
                lapPopUp.gameObject.transform.parent.gameObject.SetActive(false);
            }
            else if (currentLap < totalLaps)
            {
                lapPopUp.text = currentLap + " Lap";

                lapPopUp.gameObject.transform.parent.gameObject.SetActive(true);
                yield return new WaitForSeconds(1.0f);
                lapPopUp.gameObject.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }


        }

        public void RaceCompleted()
        {
            if (!isRaceFinished)
            {
                isRaceFinished = true;

                RaceManager.Instance.RaceCompleted(this.gameObject, position);
                StartLapTimer(false);
                totalTime = string.Format("{0:00}:{1:00}:{2:00}", RaceManager.Instance.totalLapTime.minutes, RaceManager.Instance.totalLapTime.seconds, RaceManager.Instance.totalLapTime.milliSecond);
                

                if (GetComponent<PhotonView>().IsMine)
                {
                    GetReward(position);
                    //PhotonView.Get(this).RPC(nameof(UpdateScoreboard), RpcTarget.All);
                    Debug.LogError("BUSHSHSHSHSS" + totalTime);
                    RaceManager.Instance.gameOverUI.AddScore(new Score(position, playerName, totalTime));
                    PhotonView.Get(this).RPC(nameof(UpdateScoreboardDav), RpcTarget.Others, totalTime);

                    RaceManager.Instance.gameOverUI.ActivateLeaderboard(GetComponent<PhotonView>().Owner.NickName,position);

                }
            }
        }

        void GetReward(int _pos)
        {
            for (int i = 0; i < DatabaseHandler.instance.masterData.DRPositionTokens.Length; i++)
            {
                if (i + 1 == _pos)
                {
                    DatabaseHandler.instance.playerData.mgrTokens += DatabaseHandler.instance.masterData.DRPositionTokens[i];
                    DatabaseHandler.instance.SavePlayerDataInDB();
                    break;
                }
            }
        }

        [PunRPC]
        void UpdateScoreboardDav(string _time)
        {
            Debug.LogError("DAV:::" + _time);
            RaceManager.Instance.gameOverUI.AddScore(new Score(position, playerName, _time));
        }

        [PunRPC]
        void UpdateScoreboard()
        {
            Debug.Log("IS MINE: " + PhotonView.Get(this).IsMine + " Update Time: " + totalTime);
            RaceManager.Instance.gameOverUI.AddScore(new Score(position, playerName, totalTime));   
        }

        IEnumerator PlayRaceStartTimeline(bool _play, float _waitTime)
        {
            yield return new WaitForSeconds(_waitTime);

            if (raceStartTimeline != null)
            {
                if (_play)
                {
                    raceStartTimeline.SetActive(true);

                    StartCoroutine(PlayRaceStartTimeline(false, 5f));
                }
                else
                {
                    raceStartTimeline.SetActive(false);
                }

            }
        }

        public void ActivateRaceIcon(bool _isMine)
        {
            if (_isMine)
            {
                playerIcon.SetActive(true);
                otherPlayersIcon.SetActive(false);

                posOverHeadText.gameObject.SetActive(false);
            }
            else
            {
                playerIcon.SetActive(false);
                otherPlayersIcon.SetActive(true);

                posOverHeadText.gameObject.SetActive(true);
            }
        }

    }
}
