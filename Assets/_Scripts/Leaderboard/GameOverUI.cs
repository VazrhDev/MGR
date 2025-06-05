using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

namespace MGR
{
    public class GameOverUI : MonoBehaviour
    {
        public static GameOverUI instance;
        //for sending the data 
        [SerializeField] ScoreData scoreData = new ScoreData();
        [SerializeField] TMP_Text nameTxt;
        [SerializeField] TMP_Text rankTxt;
        [SerializeField] TMP_Text raceTimeTxt;
        public int playerRank;
        public string playerName;


        [SerializeField] GameObject content;

        List<RowUi> rows = new List<RowUi>();
        public RowUi rowUi;

        private void Awake()
        {
            rows = new List<RowUi>();
            scoreData = new ScoreData();
            instance = this;

        }

        private void Update()
        {

        }

        //Issues (throws error)
        public void RemoveFromListByName(Score score)
        {
            foreach (RowUi row in rows)
            {
                if (row.playerName.text == score.playerName)
                {
                    Destroy(row.gameObject);
                    rows.Remove(row);
                    scoreData.scores.Remove(score);
                }
            }
        }


        public void resetLeaderBoard()
        {
            foreach (RowUi row in rows)
            {
                Destroy(row.gameObject);
            }
            rows.Clear();
            scoreData.scores.Clear();
        }

        public void LoadReplay()
        {
            SceneManager.LoadScene("ReplayTest");
        }

        public void AddScore(Score score)
        {
            scoreData.scores.Add(score);
            var row = Instantiate(rowUi, content.transform).GetComponent<RowUi>();
            rows.Add(row);
            //row.Place.text = score.Place.ToString();
            row.Place.text = score.Place.ToString();
            row.playerName.text = score.playerName;
            row.lapTime.text = score.score;
        }

        public void ActivateLeaderboard(string namee, int positionn )
        {
            Debug.Log("Activated leaderboard called");
            gameObject.transform.parent.gameObject.SetActive(true);
            setPlayerStats(namee,positionn);
        }

        void setPlayerStats(string nameee, int positionnn)
        {
            nameTxt.text = "Name: " + nameee;
            rankTxt.text = "Rank: " + positionnn;
            raceTimeTxt.text = "Time: " + string.Format("{0:00}:{1:00}:{2:00}", RaceManager.Instance.totalLapTime.minutes, RaceManager.Instance.totalLapTime.seconds, RaceManager.Instance.totalLapTime.milliSecond); ;
        }

        public void OnClick_ContinueBtn()
        {
            NetworkingScript.instance.CallLeaveLobby();
        }
    }
}
