using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace MGR
{
    [CreateAssetMenu(fileName = "PartsData", menuName = "Assets/PartsData")]
    public class PartsData : ScriptableObject
    {
        public CarInfo[] carsInfo;
        public EngineInfo[] engineInfo;
        public NitroInfo[] nitroInfo;
        public SteeringInfo[] steeringInfo;

        public GameObject dummyCarModel;
        public GameObject dummyEngineModel;
        public GameObject dummyNitroModel;
        public GameObject dummySteeringModel;

        public Sprite dummyCarImg;
        public Sprite dummyEngineImg;
        public Sprite dummyNitroImg;
        public Sprite dummySteeringImg;
    }

    [Serializable]
    public class EngineInfo : PartBase
    {
        public AnimationCurve curve;
    }

    [Serializable]
    public class CarInfo : PartBase
    {

    }

    [Serializable]
    public class NitroInfo : PartBase
    {
        public int maxValue = 2000;
    }

    [Serializable]
    public class SteeringInfo : PartBase
    {
        public int maxValue = 10;
    }

    [Serializable]
    public class PartBase
    {
        public string code;
        public GameObject model;
        public Sprite img;
    }
}