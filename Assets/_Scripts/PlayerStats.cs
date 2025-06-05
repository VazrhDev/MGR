using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MGR
{
    [Serializable]
    public class PlayerStats
    {
        public List<RaceData> raceData;
    }

    [Serializable]
    public class RaceData
    {
        public string gameMode;
        public string carUsed;
        public int totalRacers;
        public string mapName;
        public int position;
        public string dateTime;

        public RaceData(string _gameMode, string _carUsed, int _totalRacers, string _mapName, int _position, string _dateTime)
        {
            gameMode = _gameMode;
            carUsed = _carUsed;
            totalRacers = _totalRacers;
            mapName = _mapName;
            position = _position;
            dateTime = _dateTime;
        }
    }
}