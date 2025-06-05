using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MasterDataNS;

namespace MGR
{

    [Serializable]
    public enum CarType
    {
        Common,
        Muscle,
        Super
    }

    [Serializable]
    public class PlayerData
    {
        private static string playerPrefString = "PlayerData";
        private static PlayerData _playerData;

        public static PlayerData instance
        {
            get
            {
                if (_playerData == null)
                {
                    if (PlayerPrefs.HasKey(playerPrefString))
                    {
                        _playerData = JsonUtility.FromJson<PlayerData>(PlayerPrefs.GetString(playerPrefString));
                    }
                    else
                    {
                        _playerData = new PlayerData();
                    }
                    if (DatabaseHandler.instance)
                    {
                        Debug.LogError("Set player ddata");
                        DatabaseHandler.instance.playerData = _playerData;
                    }
                }
                return _playerData;
            }
            set
            {
                _playerData = value;
                PlayerPrefs.SetString(playerPrefString, JsonUtility.ToJson(_playerData));
                if (DatabaseHandler.instance)
                {
                    Debug.LogError("Set player data");
                    DatabaseHandler.instance.playerData = _playerData;
                }
            }
        }

        public string playerId;
        public string playerName;
        public string selectedCarCode;
        public float money = 0;
        public float mgrTokens = 0;
        public int pointsEarned = 0;
        public bool firstLogin = true;
        public List<CarDetails> carsOwned = new List<CarDetails>();
        public List<PartDetails> enginesOwned = new List<PartDetails>();
        public List<PartDetails> nitrosOwned = new List<PartDetails>();
        public List<PartDetails> steeringsOwned = new List<PartDetails>();
        public List<PartDetails> wheelsOwned = new List<PartDetails>();
        public List<PartDetails> brakesOwned = new List<PartDetails>();
        public List<PartDetails> framesOwned = new List<PartDetails>();
        public List<PartDetails> suspensionsOwned = new List<PartDetails>();
        public List<PartDetails> compundsOwned = new List<PartDetails>();
        public List<PartDetails> transmissionsOwned = new List<PartDetails>();
        public List<PartDetails> decalsOwned = new List<PartDetails>();
        public List<DRData> dailyRaceData = new List<DRData>();
    }

    [Serializable]
    public class DRData
    {
        public int year;
        public int day;
        public int timesPlayed;

        public DRData(int _year, int _day,int _timesPlayed)
        {
            year = _year;
            day = _day;
            timesPlayed = _timesPlayed;
        }
    }

    [Serializable]
    public class CarDetails
    {
        public string code;
        public int timesUsed;
        public Car car;

        public CarDetails(string _code, int _timesUsed, Car _car)
        {
            code = _code;
            timesUsed = _timesUsed;
            car = _car;
        }

    }

    [Serializable]
    public class PartDetails
    {
        public string code;
        public int timesUsed;
        public float maxValue = 0;
        public float performanceIndex;

        public PartDetails(string _code, int _timesUsed, float _maxValue, float _performanceIndex)
        {
            code = _code;
            timesUsed = _timesUsed;
            maxValue = _maxValue;
            performanceIndex = _performanceIndex;
        }

    }

    [Serializable]
    public struct CarData
    {
        public string carCode;
        public GameObject carModel;
        public string color;
        public CarType carType;
        public Sprite carRender;
        public EngineDetailedData engineDetails;
        public NitroDetailedData nitroDetails;
        public SteeringDetailedData steeringDetails;
        public WheelDetailedData wheelsDetails;
        public BrakeDetailedData brakesDetails;
        public FrameDetailedData framesDetails;
        public SuspensionDetailedData suspensionDetails;
        public CompoundDetailedData compoundsDetails;
        public TransmissionDetailedData transmissionDetails;

        public CarData(GameObject _carModel, string _color, string _carCode = "null", Sprite _carRender = null, EngineDetailedData _engineDetails = null, NitroDetailedData _nitroDetails = null, SteeringDetailedData _steeringDetails = null
            , WheelDetailedData _wheelsDetails = null, BrakeDetailedData _brakesDetails = null, FrameDetailedData _frameDetails = null
            , SuspensionDetailedData _suspensionDetails = null, CompoundDetailedData _compoundDetails = null, TransmissionDetailedData _transmissionDetails = null,
            CarType _carType = CarType.Common)
        {
            carModel = _carModel;
            color = _color;
            carCode = _carCode;
            carRender = _carRender;
            engineDetails = _engineDetails;
            nitroDetails = _nitroDetails;
            steeringDetails = _steeringDetails;

            wheelsDetails = _wheelsDetails;
            brakesDetails = _brakesDetails;
            framesDetails = _frameDetails;

            suspensionDetails = _suspensionDetails;
            compoundsDetails = _compoundDetails;
            transmissionDetails = _transmissionDetails;

            carType = _carType;
        }
    }
}