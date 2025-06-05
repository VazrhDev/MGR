using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace MGR
{
    [Serializable]
    public class ScoreData
    {

        public List<Score> scores;
        public ScoreData()
        {
            scores = new List<Score>();
        }

    }

    [Serializable]
    public class Score
    {
        public string playerName;
        public int Place;
        public string score;

        public Score(int place, string name, string score)
        {
            playerName = name;
            Place = place;
            this.score = score;
        }
    }
}