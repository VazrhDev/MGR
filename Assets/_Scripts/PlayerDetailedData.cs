using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MGR;
using System;
using MasterDataNS;

[Serializable]
public class PlayerDetailedData
{
    public List<CarDetailedData> carsOwned = new List<CarDetailedData>();
    public List<EngineDetailedData> enginesOwned = new List<EngineDetailedData>();
    public List<NitroDetailedData> nitrosOwned = new List<NitroDetailedData>();
    public List<SteeringDetailedData> steeringsOwned = new List<SteeringDetailedData>();
    public List<WheelDetailedData> wheelsOwned = new List<WheelDetailedData>();
    public List<BrakeDetailedData> brakesOwned = new List<BrakeDetailedData>();
    public List<FrameDetailedData> framesOwned = new List<FrameDetailedData>();
    public List<SuspensionDetailedData> suspensionsOwned = new List<SuspensionDetailedData>();
    public List<CompoundDetailedData> compoundsOwned = new List<CompoundDetailedData>();
    public List<TransmissionDetailedData> transmissionsOwned = new List<TransmissionDetailedData>();
    public List<DecalDetailedData> decalsOwned = new List<DecalDetailedData>();
}

[Serializable]
public class CarDetailedData
{
    public string carDefaultCode;
    public CarDetails carDetails;
    public int MaxUsage;
    public Sprite carRender;
    public GameObject carObject;
}

[Serializable]
public class WheelDetailedData : DetailedDataBase
{
    public WheelData wheelData;
    public Sprite render;
    public GameObject objectt;
}

[Serializable]
public class DecalDetailedData : DetailedDataBase
{
    public Sprite render;
    public GameObject objectt;
}

[Serializable]
public class EngineDetailedData : DetailedDataBase
{
    public EngineData engineData;
}

[Serializable]
public class NitroDetailedData : DetailedDataBase
{
    public NitroData nitroData;
}

[Serializable]
public class SteeringDetailedData : DetailedDataBase
{
    public SteeringData steeringData;
}

[Serializable]
public class BrakeDetailedData : DetailedDataBase
{
    public BrakeData brakeData;
}

[Serializable]
public class FrameDetailedData : DetailedDataBase
{
    public FrameData frameData;
}

[Serializable]
public class SuspensionDetailedData : DetailedDataBase
{
    public SuspensionData suspensionData;
}

[Serializable]
public class CompoundDetailedData : DetailedDataBase
{
    public CompoundData compoundData;
}

[Serializable]
public class TransmissionDetailedData : DetailedDataBase
{
    public TransmissionData transmissionData;
}

[Serializable]
public class DetailedDataBase
{
    public PartDetails partDetails;
}