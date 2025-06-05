using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using MGR;

namespace MasterDataNS
{
    public class MasterDataUploader : MonoBehaviour
    {

        public static MasterDataUploader instance;

        [SerializeField] bool saveMasterJsonLocallyOnStart;
        [SerializeField] bool loadMasterJsonLocallyOnStart;
        public bool loadFromDatabase;
        public MasterData masterData;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            if (saveMasterJsonLocallyOnStart)
            {
                Save();
            }
            if (loadMasterJsonLocallyOnStart)
            {
                Load();
            }
        }

        [ContextMenu("Save Master Data")]
        public void Save()
        {
            SaveFile abc = new SaveFile("Jatin", ".json", false);
            abc.Write(JsonUtility.ToJson(masterData));
            abc.Save();
            Master.ColoredLog("grey", Application.persistentDataPath);
        }

        void Load()
        {
            Debug.Log(Application.persistentDataPath);
            SaveFile abc = new SaveFile("Jatin", ".json", false);
            if(abc.Load(out string data))
            {
                masterData = JsonConvert.DeserializeObject<MasterData>(data);
            }
        }
    }

    [Serializable]
    public class MasterData
    {
        public string Name = "MasterData";
        public Car[] cars;
        public EngineData[] engines;
        public NitroData[] nitros;
        public SteeringData[] steerings;
        public WheelData[] wheels;
        public BrakeData[] brakes;
        public FrameData[] frames;
        public SuspensionData[] suspensions;
        public CompoundData[] compounds;
        public TransmissionData[] transmissions;
        public MapData[] mapData;

        public float minAcceleration;
        public float maxAcceleration;
        public float minTopSpeed;
        public float maxTopSpeed;
        public float minHandling;
        public float maxHandling;
        public float maxWeight;
        public float maxSportiveness;
        public float maxFuelEfficiency;
        public float maxStability;
        public float minDeceleration;
        public float maxDeceleration;

        public int DRModeLimit = 10;
        public float[] DRPositionTokens;

        public int[] points;
        public float[] performanceIndex;

    }

    [Serializable]
    public class Car : CarComponent
    {
        public string nitroCode;
        public string engineCode;
        public string steeringCode;
        public string wheelsCode;
        public string brakeCode;
        public string frameCode;
        public string suspensionCode;
        public string compoundCode;
        public string transmissionCode;
        public string colorCode;
        public CarType carType;

        public Car(string _nitroCode, string _engineCode, string _steeringCode, string _wheelsCode, string _brakeCode, string _frameCode,
            string _suspensionCode, string _compoundCode, string _transmissionCode, string _colorCode, CarType _carType)
        {
            nitroCode = _nitroCode;
            engineCode = _engineCode;
            steeringCode = _steeringCode;
            wheelsCode = _wheelsCode;
            brakeCode = _brakeCode;
            frameCode = _frameCode;
            suspensionCode = _suspensionCode;
            compoundCode = _compoundCode;
            transmissionCode = _transmissionCode;
            colorCode = _colorCode;
            carType = _carType;
        }
    }

    [Serializable]
    public class CarComponent
    {
        public string componentCode;
        public string componentName;
        public int maxUsage;
        public float performanceIndex;
    }

    [Serializable]
    public class EngineData : CarComponent
    {
        public float acceleration;
        public float topSpeed;
        public float fuelEfficiency;

        public EngineData(string _componentCode, int _maxUsage, float _acceleration, float _topSpeed, float _fuelEfficiency, float _performanceIndex)
        {
            componentCode = _componentCode;
            maxUsage = _maxUsage;
            acceleration = _acceleration;
            topSpeed = _topSpeed;
            fuelEfficiency = _fuelEfficiency;
            performanceIndex = _performanceIndex;
        }
    }

    [Serializable]
    public class NitroData : CarComponent
    {
        public float accelerationOnUsing;
        public float duration;

        public NitroData(string _componentCode, int _maxUsage, float _accelerationOnUsing, float _duration, float _performanceIndex)
        {
            componentCode = _componentCode;
            maxUsage = _maxUsage;
            accelerationOnUsing = _accelerationOnUsing;
            duration = _duration;
            performanceIndex = _performanceIndex;
        }
    }

    [Serializable]
    public class SteeringData : CarComponent
    {
        public float handling;
        public float stability;

        public SteeringData(string _componentCode, int _maxUsage, float _handling, float _stability, float _performanceIndex)
        {
            componentCode = _componentCode;
            maxUsage = _maxUsage;
            handling = _handling;
            stability = _stability;
            performanceIndex = _performanceIndex;
        }

    }

    [Serializable]
    public class WheelData : CarComponent
    {
        public float acceleration;
        public float deceleration;
        public float weight;
        public float sportiveness;

        public WheelData(string _componentCode, int _maxUsage, float _acceleration, float _deceleration, float _weight, float _sportiveness, float _performanceIndex)
        {
            componentCode = _componentCode;
            maxUsage = _maxUsage;
            acceleration = _acceleration;
            deceleration = _deceleration;
            weight = _weight;
            sportiveness = _sportiveness;
            performanceIndex = _performanceIndex;
        }
    }

    [Serializable]
    public class BrakeData : CarComponent
    {
        public float fuelEfficiency;
        public float deceleration;

        public BrakeData(string _componentCode, int _maxUsage, float _fuelEfficiency, float _deceleration, float _performanceIndex)
        {
            componentCode = _componentCode;
            maxUsage = _maxUsage;
            fuelEfficiency = _fuelEfficiency;
            deceleration = _deceleration;
            performanceIndex = _performanceIndex;
        }
    }

    [Serializable]
    public class FrameData : CarComponent
    {
        public float acceleration;
        public float weight;
        public float fuelEfficiency;

        public FrameData(string _componentCode, int _maxUsage, float _acceleration, float _weight, float _fuelEfficiency, float _performanceIndex)
        {
            componentCode = _componentCode;
            maxUsage = _maxUsage;
            acceleration = _acceleration;
            weight = _weight;
            fuelEfficiency = _fuelEfficiency;
            performanceIndex = _performanceIndex;
        }
    }

    [Serializable]
    public class SuspensionData : CarComponent
    {
        public float acceleration;
        public float topSpeed;
        public float sportiveness;
        public float stability;

        public SuspensionData(string _componentCode, int _maxUsage, float _acceleration, float _topSpeed, float _sportiveness, float _stability, float _performanceIndex)
        {
            componentCode = _componentCode;
            maxUsage = _maxUsage;
            acceleration = _acceleration;
            topSpeed = _topSpeed;
            sportiveness = _sportiveness;
            stability = _stability;
            performanceIndex = _performanceIndex;
        }

    }

    [Serializable]
    public class CompoundData : CarComponent
    {
        public float acceleration;
        public float topSpeed;
        public float sportiveness;
        public float stability;
        public float deceleration;

        public CompoundData(string _componentCode, int _maxUsage, float _acceleration, float _topSpeed, float _sportiveness, float _stability,
            float _deceleration, float _performanceIndex)
        {
            componentCode = _componentCode;
            maxUsage = _maxUsage;
            acceleration = _acceleration;
            topSpeed = _topSpeed;
            sportiveness = _sportiveness;
            stability = _stability;
            deceleration = _deceleration;
            performanceIndex = _performanceIndex;
        }

    }

    [Serializable]
    public class TransmissionData : CarComponent
    {
        public float acceleration;
        public float topSpeed;
        public float fuelEfficiency;

        public TransmissionData(string _componentCode, int _maxUsage, float _acceleration, float _topSpeed, float _fuelEfficiency, float _performanceIndex)
        {
            componentCode = _componentCode;
            maxUsage = _maxUsage;
            acceleration = _acceleration;
            topSpeed = _topSpeed;
            fuelEfficiency = _fuelEfficiency;
            performanceIndex = _performanceIndex;
        }

    }

    [Serializable]
    public class MapData
    {
        public string mapCode;
        public string countryName;
        public string mapName;
        public string trackType;
        public string trackRoute;
        public string totalLength;
        public string weatherCondition;

        public MapData(string _mapCode, string _countryName, string _mapName, string _trackType, string _trackRoute, string _totalLength,
            string _weatherCondition)
        {
            mapCode = _mapCode;
            countryName = _countryName;
            mapName = _mapName;
            trackType = _trackType;
            trackRoute = _trackRoute;
            totalLength = _totalLength;
            weatherCondition = _weatherCondition;
        }
    }
}