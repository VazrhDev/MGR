using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MasterDataNS;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;
//using Palmmedia.ReportGenerator.Core.Parser.Analysis;


namespace MGR
{
    [System.Serializable]
    public class MapDataDetailed
    {
        public MapData mapData;
        public Sprite render;
        public GameObject mapObject;
    }

    [System.Serializable]
    public class CarDisplayData
    {
        public string CarId;
        public string CarName;
        public string OpenSeaNaming;
        public string Type;
        public float Price;
        public float PerformanceIndex;
        public string Engine;
        public string Transmission;
        public string Tyre;
        public string Bodyframe;
        public string Suspension;
        public string BreakingSystem;
        public string Steering;
        public string Wheels;
        public float Acceleration;
        public float TopSpeed;
        public float Handling;
        public float Weight;
        public float Sportiveness;
        public float FuelEfficiency;
        public float Stability;
        public float Deceleration;
        public string BoosterComponent;
        public string AerodynamicKits;
        public string Color;
        public string ColorType;
        public string ColorShade;
        public string BackgroundColor;
        public string BaseColor;
        public string Rim;
        public string Manufacturer;
        public string Rarity;

        public CarDisplayData(string carId, string carName, string openSeaNaming, string type, float price, float performanceIndex, string engine, string transmission, string tyre, string bodyframe, string suspension, string breakingSystem, string steering, string wheels, float acceleration, float topSpeed, float handling, float weight, float sportiveness, float fuelEfficiency, float stability, float deceleration, string boosterComponent, string aerodynamicKits, string color, string colorType, string colorShade, string backgroundColor, string baseColor, string rim, string manufacturer, string rarity)
        {
            this.CarId = carId;
            this.CarName = carName;
            this.OpenSeaNaming = openSeaNaming;
            this.Type = type;
            this.Price = price;
            this.PerformanceIndex = performanceIndex;
            this.Engine = engine;
            this.Transmission = transmission;
            this.Tyre = tyre;
            this.Bodyframe = bodyframe;
            this.Suspension = suspension;
            this.BreakingSystem = breakingSystem;
            this.Steering = steering;
            this.Wheels = wheels;
            this.Acceleration = acceleration;
            this.TopSpeed = topSpeed;
            this.Handling = handling;
            this.Weight = weight;
            this.Sportiveness = sportiveness;
            this.FuelEfficiency = fuelEfficiency;
            this.Stability = stability;
            this.Deceleration = deceleration;
            this.BoosterComponent = boosterComponent;
            this.AerodynamicKits = aerodynamicKits;
            this.Color = color;
            this.ColorType = colorType;
            this.ColorShade = colorShade;
            this.BackgroundColor = backgroundColor;
            this.BaseColor = baseColor;
            this.Rim = rim;
            this.Manufacturer = manufacturer;
            this.Rarity = rarity;
        }

        
    }

    [System.Serializable]
    public class DisplayData
    {
        public string Name = "DisplayData";
        public List<CarDisplayData> carDisplayData;
    }


    public class DatabaseHandler : MonoBehaviour
    {
        [Header("Display Data")]
        public int fileIndex;
        public TextAsset[] textAsset;
        public string[] colorCodes;
        public List<string> finalColorCodes;
        public List<string> finalColorCodes1;
        public List<string> finalColorCodes2;
        public List<string> finalColorCodes3;
        public List<string> finalColorCodes4;
        public List<string> finalColorCodes5;
        [TextArea(1, 100)]
        public string colorCodesString;

        public static DatabaseHandler instance;
        public PlayerData playerData;
        public PlayerStats playerStats;
        [SerializeField] Car carToAdd;
        [SerializeField] bool addCar = false;
        public MasterData masterData;
        public DisplayData displayData;
        public DisplayData newDisplayData;
        [TextArea(1, 100)]
        public string temp;
        public PlayerDetailedData playerDetailedData;
        public List<MapDataDetailed> mapDataDetailed = new List<MapDataDetailed>();

        public bool savePlayerDataLocally;
        public bool loadPlayerDataLocally;
        public bool hasLoadedMaps;

        public List<GameObject> carsList = new List<GameObject>();
        public List<string> mapsList = new List<string>();
        public GameObject carRenders;
        public GameObject nFTRenders;
        public GameObject wheelrenders;
        public GameObject mapRenders;
        public GameObject decals;
        public TextAsset tempMaster;

        [ContextMenu("Shocked")]
        void JatinZindabad()
        {
            if (fileIndex == 0) finalColorCodes = new List<string>();
            else if (fileIndex == 1) finalColorCodes1 = new List<string>();
            else if (fileIndex == 2) finalColorCodes2 = new List<string>();
            else if (fileIndex == 3) finalColorCodes3 = new List<string>();
            else if (fileIndex == 4) finalColorCodes4 = new List<string>();
            else if (fileIndex == 5) finalColorCodes5 = new List<string>();
            colorCodesString = textAsset[fileIndex].text.Replace("\n\n", "%");
            colorCodesString = textAsset[fileIndex].text.Replace("\n", "%");
            colorCodes = colorCodesString.Split('%');
            foreach (var item in colorCodes)
            {
                if (!string.IsNullOrEmpty(item) && !string.IsNullOrWhiteSpace(item))
                {
                    if (fileIndex == 0) finalColorCodes.Add(item);
                    else if (fileIndex == 1) finalColorCodes1.Add(item);
                    else if (fileIndex == 2) finalColorCodes2.Add(item);
                    else if (fileIndex == 3) finalColorCodes3.Add(item);
                    else if (fileIndex == 4) finalColorCodes4.Add(item);
                    else if (fileIndex == 5) finalColorCodes5.Add(item);
                }
            }
            int temp = 0;
            if (fileIndex == 0) temp = finalColorCodes.Count;
            else if (fileIndex == 1) temp = finalColorCodes1.Count;
            else if (fileIndex == 2) temp = finalColorCodes2.Count;
            else if (fileIndex == 3) temp = finalColorCodes3.Count;
            else if (fileIndex == 4) temp = finalColorCodes4.Count;
            else if (fileIndex == 5) temp = finalColorCodes5.Count;
            for (int i = 0; i < temp; i++)
            {
                if (fileIndex == 0) finalColorCodes[i] = finalColorCodes[i].Replace("\n", "");
                else if (fileIndex == 1) finalColorCodes1[i] = finalColorCodes1[i].Replace("\n", "");
                else if (fileIndex == 2) finalColorCodes2[i] = finalColorCodes2[i].Replace("\n", "");
                else if (fileIndex == 3) finalColorCodes3[i] = finalColorCodes3[i].Replace("\n", "");
                else if (fileIndex == 4) finalColorCodes4[i] = finalColorCodes4[i].Replace("\n", "");
                else if (fileIndex == 5) finalColorCodes5[i] = finalColorCodes5[i].Replace("\n", "");
            }
        }

        [ContextMenu("COLORRR")]
        void AddColor()
        {
            int startPoint;
            for (int i = 0; i < 5; i++)
            {
                startPoint = i * 540;
                for (int j = 0; j < finalColorCodes.Count; j++)
                {
                    newDisplayData.carDisplayData[startPoint + j].Color = finalColorCodes[j];
                    newDisplayData.carDisplayData[startPoint + j].ColorType = finalColorCodes1[j];
                    newDisplayData.carDisplayData[startPoint + j].ColorShade = finalColorCodes2[j];
                    newDisplayData.carDisplayData[startPoint + j].Rim = finalColorCodes3[j];
                    newDisplayData.carDisplayData[startPoint + j].BaseColor = finalColorCodes4[j];
                }
            }
            //for (int i = 0; i < 5; i++)
            //{
            //    startPoint = i * 480 + 2700;
            //    for (int j = 0; j < finalColorCodes1.Count; j++)
            //    {
            //        newDisplayData.carDisplayData[startPoint + j].Color = finalColorCodes1[j];
            //        newDisplayData.carDisplayData[startPoint + j].ColorType = finalColorCodes3[j];
            //        newDisplayData.carDisplayData[startPoint + j].ColorShade = finalColorCodes5[j];
            //    }
            //}
        }

        private void Awake()
        {
            //playerData = new PlayerData();
            if (instance == null) instance = this;
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);   
        }

        [ContextMenu("To Object")]
        public void ToObject()
        {
            displayData = JsonConvert.DeserializeObject<DisplayData>(temp);
        }
        [ContextMenu("To Object2")]
        public void TooBject2()
        {
            //masterData = new MasterData();
            masterData = JsonConvert.DeserializeObject<MasterData>(tempMaster.text);
        }

        [ContextMenu("New Display Data To String")]
        public void ToStringgg()
        {
            temp = JsonConvert.SerializeObject(newDisplayData);
        }

        [ContextMenu("Display Data To String")]
        public void ToStringg()
        {
            temp = JsonConvert.SerializeObject(displayData);
        }

        [ContextMenu("Update Display Data")]
        public void UpdateDisplayData()
        {
            foreach (var dData in displayData.carDisplayData)
            {
                //dData.CarName = dData.CarId;
                dData.BackgroundColor = "White";
                dData.BaseColor = "Black";
            }
        }

        [ContextMenu("Manufacturer")]
        void ChangeMAnufacturer()
        {
            //for (int i = 540+540+540+540+540+480+480+480+480; i < 540+540+540+540+540+480+480+480+480+480; i++)
            //{
            //    newDisplayData.carDisplayData[i].Manufacturer = "Mcrun";
            //}
        }

        [ContextMenu("Add New Display Data")]
        public void AddNewDisplayData()
        {
            int assetID = 1+540+540+540+540+540+480+480+480+480;
            int openSeaNaming = 0+540+540+540+540+540+480+480+480+480;

            string carName = "McRun";

            //foreach(var dData in displayData.carDisplayData)
            //for (int carIndex = 1; carIndex <= 10; carIndex++)
            //{
            //    switch(carIndex)
            //    {
            //        case 1:
            //            carName = "Ruder";
            //            break;
            //        case 2:
            //            carName = "Brute";
            //            break;
            //        case 3:
            //            carName = "Rocker";
            //            break;
            //        case 4:
            //            carName = "Screamer";
            //            break;
            //        case 5:
            //            carName = "Striker";
            //            break;
            //        case 6:
            //            carName = "Lambro";
            //            break;
            //        case 7:
            //            carName = "Astrony";
            //            break;
            //        case 8:
            //            carName = "Faeron";
            //            break;
            //        case 9:
            //            carName = "Poker";
            //            break;
            //        case 10:
            //            carName = "McRun";
            //            break;
            //    }

            //    if (carIndex < 6)
            //    {
                    for (int i = 55; i < displayData.carDisplayData.Count; i++)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                    //CarDisplayData data = displayData.carDisplayData[i];
                    //data = displayData.carDisplayData[i];
                    CarDisplayData data = new CarDisplayData(displayData.carDisplayData[i].CarId, displayData.carDisplayData[i].CarName,
                        displayData.carDisplayData[i].OpenSeaNaming, displayData.carDisplayData[i].Type, displayData.carDisplayData[i].Price,
                        displayData.carDisplayData[i].PerformanceIndex, displayData.carDisplayData[i].Engine, displayData.carDisplayData[i].Transmission,
                        displayData.carDisplayData[i].Tyre, displayData.carDisplayData[i].Bodyframe, displayData.carDisplayData[i].Suspension,
                        displayData.carDisplayData[i].BreakingSystem, displayData.carDisplayData[i].Steering, displayData.carDisplayData[i].Wheels,
                        displayData.carDisplayData[i].Acceleration, displayData.carDisplayData[i].TopSpeed, displayData.carDisplayData[i].Handling,
                        displayData.carDisplayData[i].Weight, displayData.carDisplayData[i].Sportiveness, displayData.carDisplayData[i].FuelEfficiency,
                        displayData.carDisplayData[i].Stability, displayData.carDisplayData[i].Deceleration, displayData.carDisplayData[i].BoosterComponent,
                        displayData.carDisplayData[i].AerodynamicKits, displayData.carDisplayData[i].Color, displayData.carDisplayData[i].ColorType,
                        displayData.carDisplayData[i].ColorShade, displayData.carDisplayData[i].BackgroundColor, displayData.carDisplayData[i].BaseColor,
                        displayData.carDisplayData[i].Rim, displayData.carDisplayData[i].Manufacturer, displayData.carDisplayData[i].Rarity);
                            data.CarId = carName + assetID.ToString("0000");
                            data.CarName = carName + assetID.ToString("0000");
                            data.OpenSeaNaming = openSeaNaming.ToString();

                            newDisplayData.carDisplayData.Add(data);

                            //Debug.Log(assetID);
                            assetID += 1;
                            openSeaNaming += 1;
                        }
                    }
            //}
            //else
            //{
            //        for (int i = 55; i < displayData.carDisplayData.Count; i++)
            //        {
            //            for (int j = 0; j < 10; j++)
            //            {
            //                CarDisplayData data = displayData.carDisplayData[i];

            //                data.CarId = carName + assetID.ToString("0000");
            //                data.CarName = carName + assetID.ToString("0000");
            //                data.OpenSeaNaming = openSeaNaming.ToString();

            //                newDisplayData.carDisplayData.Add(data);

            //                assetID += 1;
            //                openSeaNaming += 1;
            //            }
            //}
            //}
        }
        //}


        internal void UpdateMuscleCar()
        {

            int muscleCarsNumbers = 540;
            int numOfCarsToChange = 14;

            for (int i = 1; i < 6; i++)
            {
                for (int j = 0; j < muscleCarsNumbers; j++)
                {
                    int num = (i * 540) - (numOfCarsToChange + j);

                }
            }
        }


        private void Update()
        {
            if (addCar)
            {
                List<Car> temp = new List<Car>();
                temp.AddRange(masterData.cars);
                Car tempp = new Car(carToAdd.nitroCode, carToAdd.engineCode, carToAdd.steeringCode, carToAdd.wheelsCode, carToAdd.brakeCode,
                    carToAdd.frameCode, carToAdd.suspensionCode, carToAdd.compoundCode, carToAdd.transmissionCode, carToAdd.colorCode, carToAdd.carType);
                tempp.componentCode = carToAdd.componentCode;
                tempp.componentName = carToAdd.componentName;
                tempp.maxUsage = carToAdd.maxUsage;
                tempp.performanceIndex = carToAdd.performanceIndex;
                temp.Add(tempp);
                //carToAdd = null;
                masterData.cars = temp.ToArray();
                addCar = false;
            }
        }

        private void Start()
        {
            if (loadPlayerDataLocally)
                playerData = PlayerData.instance;
            
        }

        #region Development Methods

        [ContextMenu("Save Car Display Data Json Locally")]
        public void SaveCarDataJson()
        {
            Debug.Log(Application.persistentDataPath);
            SaveFile abc = new SaveFile("DataJson", ".json", false);
            abc.Write(JsonUtility.ToJson(displayData));
            abc.Save();
        }


        [ContextMenu("Save Master Json Locally")]
        public void SaveMasterJson()
        {
            Debug.Log(Application.persistentDataPath);
            Debug.LogError("Bitch");
            SaveFile abc = new SaveFile("MasterJson", ".json", false);
            abc.Write(JsonUtility.ToJson(masterData));
            abc.Save();
        }

        [ContextMenu("Save Player Data in DB")]
        public void SavePlayerDataInDB()
        {
            if (savePlayerDataLocally)
                PlayerData.instance = playerData;
            var request = new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
            {
                { "PlayerData", JsonConvert.SerializeObject(playerData) }
            }
            };

            PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
        }

        [ContextMenu("Save Player Stats in DB")]
        public void SavePlayerStatsInDB()
        {
            var request = new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
            {
                { "PlayerStats", JsonConvert.SerializeObject(playerStats) }
            }
            };

            PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
        }

        private void OnDataSend(UpdateUserDataResult result)
        {
            Master.ColoredLog("green", "Data Sent Successfully"); 
        }

        void OnError(PlayFabError error)
        {
            Master.ColoredLog("red", error.GenerateErrorReport());
        }

        #endregion

        public void IncreaseDRMPlays(System.DateTime dateTime)
        {
            GameManager.instance.currentDateTime = dateTime;
            if (MainGameScript.instance)
            {
                MainGameScript.instance.SetRaceData();
            }
            if (GameManager.instance.gameMode == GameMode.DailyRace && GameManager.instance.currentCarData.carType != CarType.Common)
            {
                bool hasFound = false;
                for (int i = 0; i < playerData.dailyRaceData.Count; i++)
                {
                    if (playerData.dailyRaceData[i].year == dateTime.Year)
                    {
                        if (playerData.dailyRaceData[i].day == dateTime.DayOfYear)
                        {
                            playerData.dailyRaceData[i].timesPlayed++;
                            hasFound = true;
                        }
                    }
                }
                if (!hasFound)
                {
                    DRData temp = new DRData(dateTime.Year, dateTime.DayOfYear, 1);
                    playerData.dailyRaceData.Add(temp);
                }
                SavePlayerDataInDB();
            }
        }

        public Sprite GetCarRenderFromCarCode(string _carCode)
        {
                for (int i = 0; i < nFTRenders.transform.childCount; i++)
                {
                    if (string.Compare(nFTRenders.transform.GetChild(i).name, _carCode, true) == 0)
                    {
                        return nFTRenders.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite;
                    }
                }
            return null;
        }

        public void SetDetailedData()
        {
            foreach (var item in playerDetailedData.carsOwned)
            {
                for (int i = 0; i < nFTRenders.transform.childCount; i++)
                {
                    if (string.Compare(nFTRenders.transform.GetChild(i).name, item.carDetails.code, true) == 0)
                    {
                        item.carRender = nFTRenders.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite;
                        break;
                    }
                }
            }
            foreach (var item in playerData.enginesOwned)
            {
                EngineDetailedData temp = new EngineDetailedData();
                temp.partDetails = new PartDetails(item.code, item.timesUsed, item.maxValue, item.performanceIndex);
                temp.engineData = GetEngineDataFromCode(temp.partDetails.code);
                playerDetailedData.enginesOwned.Add(temp);
            }
            foreach (var item in playerData.nitrosOwned)
            {
                NitroDetailedData temp = new NitroDetailedData();
                temp.partDetails = new PartDetails(item.code, item.timesUsed, item.maxValue, item.performanceIndex);
                temp.nitroData = GetNitroDataFromCode(temp.partDetails.code);
                playerDetailedData.nitrosOwned.Add(temp);
            }
            foreach (var item in playerData.steeringsOwned)
            {
                SteeringDetailedData temp = new SteeringDetailedData();
                temp.partDetails = new PartDetails(item.code, item.timesUsed, item.maxValue, item.performanceIndex);
                temp.steeringData = GetSteeringDataFromCode(temp.partDetails.code);
                playerDetailedData.steeringsOwned.Add(temp);
            }
            foreach (var item in playerData.wheelsOwned)
            {
                WheelDetailedData temp = new WheelDetailedData();
                temp.partDetails = new PartDetails(item.code, item.timesUsed, item.maxValue, item.performanceIndex);
                temp.wheelData = GetWheelDataFromCode(temp.partDetails.code);
                for (int i = 0; i < wheelrenders.transform.childCount; i++)
                {
                    if(string.Compare(wheelrenders.transform.GetChild(i).name, temp.partDetails.code, true) == 0)
                    {
                        temp.render = wheelrenders.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite;
                    }
                }
                playerDetailedData.wheelsOwned.Add(temp);
            }
            foreach (var item in playerData.brakesOwned)
            {
                BrakeDetailedData temp = new BrakeDetailedData();
                temp.partDetails = new PartDetails(item.code, item.timesUsed, item.maxValue, item.performanceIndex);
                temp.brakeData = GetBrakeDataFromCode(temp.partDetails.code);
                playerDetailedData.brakesOwned.Add(temp);
            }
            foreach (var item in playerData.framesOwned)
            {
                FrameDetailedData temp = new FrameDetailedData();
                temp.partDetails = new PartDetails(item.code, item.timesUsed, item.maxValue, item.performanceIndex);
                temp.frameData = GetFrameDataFromCode(temp.partDetails.code);
                playerDetailedData.framesOwned.Add(temp);
            }
            foreach (var item in playerData.suspensionsOwned)
            {
                SuspensionDetailedData temp = new SuspensionDetailedData();
                temp.partDetails = new PartDetails(item.code, item.timesUsed, item.maxValue, item.performanceIndex);
                temp.suspensionData = GetSuspensionDataFromCode(temp.partDetails.code);
                playerDetailedData.suspensionsOwned.Add(temp);
            }
            foreach (var item in playerData.compundsOwned)
            {
                CompoundDetailedData temp = new CompoundDetailedData();
                temp.partDetails = new PartDetails(item.code, item.timesUsed, item.maxValue, item.performanceIndex);
                temp.compoundData = GetCompoundDataFromCode(temp.partDetails.code);
                playerDetailedData.compoundsOwned.Add(temp);
            }
            foreach (var item in playerData.transmissionsOwned)
            {
                TransmissionDetailedData temp = new TransmissionDetailedData();
                temp.partDetails = new PartDetails(item.code, item.timesUsed, item.maxValue, item.performanceIndex);
                temp.transmissionData = GetTransmissionDataFromCode(temp.partDetails.code);
                playerDetailedData.transmissionsOwned.Add(temp);
            }
            foreach (var item in playerData.decalsOwned)
            {
                DecalDetailedData temp = new DecalDetailedData();
                temp.partDetails = new PartDetails(item.code, item.timesUsed, item.maxValue, item.performanceIndex);
                for (int i = 0; i < decals.transform.childCount; i++)
                {
                    if (string.Compare(decals.transform.GetChild(i).name, temp.partDetails.code, true) == 0)
                    {
                        temp.render = decals.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite;
                    }
                }
                playerDetailedData.decalsOwned.Add(temp);
            }
            mapsList = new List<string>();
            for (int k = 0; k < PlayfabManager.instance.maps.Length; k++)
            {
                DatabaseHandler.instance.mapsList.Add(PlayfabManager.instance.maps[k]);
            }

            List<MapData> currentMapsData = new List<MapData>();
            for (int i = 0; i < masterData.mapData.Length; i++)
            {
                currentMapsData.Add(masterData.mapData[i]);
            }

            MapData newMapData = new MapData("PikesPeakFull", "North America", "Pikes Peak", "Mountain Range", "On Road", "69Km", "Snow");

#if UNITY_WEBGL
            currentMapsData.Add(newMapData);
#endif

#if UNITY_ANDROID || UNITY_IOS
            currentMapsData[currentMapsData.Count - 1] = newMapData;
#endif

            masterData.mapData = currentMapsData.ToArray();

            if (masterData.mapData != null && !hasLoadedMaps)
            {
                foreach (var item in masterData.mapData)
                {
                    foreach (var item1 in mapsList)
                    {
                        
                        if (string.Compare(item.mapCode, item1, true) == 0)
                        {
                            hasLoadedMaps = true;
                            MapDataDetailed temp = new MapDataDetailed();
                            temp.mapData = new MapData(item.mapCode, item.countryName, item.mapName, item.trackType, item.trackRoute, item.totalLength,
                                item.weatherCondition);
                            temp.mapObject = null;
                            //Debug.LogError("New data added " + temp.mapData.countryName);
                            mapDataDetailed.Add(temp);
                            break;
                        }
                    }
                }
                if (mapRenders == null) return;
                foreach (var item in mapDataDetailed)
                {
                    for (int i = 0; i < mapRenders.transform.childCount; i++)
                    {
                        if (string.Compare(mapRenders.transform.GetChild(i).name, item.mapData.mapCode, true) == 0)
                        {
                            item.render = mapRenders.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite;
                        }
                    }
                }
            }
            if(mapDataDetailed != null && mapDataDetailed.Count > 0)
            {
                GameManager.instance.selectedMapData = mapDataDetailed[0];
            }
        }

#region Get Part Detail from code

        public PartDetails GetEngineDetailsFromCode(string _code)
        {
            foreach (var item in playerData.enginesOwned)
            {
                if (item.code == _code)
                {
                    return new PartDetails(item.code, item.timesUsed, item.maxValue, item.performanceIndex);
                }
            }
            return new PartDetails("Dummy", 0, 0, 0);
        }

        public PartDetails GetNitroDetailsFromCode(string _code)
        {
            foreach (var item in playerData.nitrosOwned)
            {
                if (item.code == _code)
                {
                    return new PartDetails(item.code, item.timesUsed, item.maxValue, item.performanceIndex);
                }
            }
            return new PartDetails("Dummy", 0, 0, 0);
        }

        public PartDetails GetSteeringDetailsFromCode(string _code)
        {
            foreach (var item in playerData.steeringsOwned)
            {
                if (item.code == _code)
                {
                    return new PartDetails(item.code, item.timesUsed, item.maxValue, item.performanceIndex);
                }
            }
            return new PartDetails("Dummy", 0, 0, 0);
        }

        public PartDetails GetWheelDetailsFromCode(string _code)
        {
            foreach (var item in playerData.wheelsOwned)
            {
                if (item.code == _code)
                {
                    return new PartDetails(item.code, item.timesUsed, item.maxValue, item.performanceIndex);
                }
            }
            return new PartDetails("Dummy", 0, 0, 0);
        }

        public PartDetails GetBrakeDetailsFromCode(string _code)
        {
            foreach (var item in playerData.brakesOwned)
            {
                if (item.code == _code)
                {
                    return new PartDetails(item.code, item.timesUsed, item.maxValue, item.performanceIndex);
                }
            }
            return new PartDetails("Dummy", 0, 0, 0);
        }

        public PartDetails GetFrameDetailsFromCode(string _code)
        {
            foreach (var item in playerData.framesOwned)
            {
                if (item.code == _code)
                {
                    return new PartDetails(item.code, item.timesUsed, item.maxValue, item.performanceIndex);
                }
            }
            return new PartDetails("Dummy", 0, 0, 0);
        }

        public PartDetails GetSuspensionDetailsFromCode(string _code)
        {
            foreach (var item in playerData.suspensionsOwned)
            {
                if (item.code == _code)
                {
                    return new PartDetails(item.code, item.timesUsed, item.maxValue, item.performanceIndex);
                }
            }
            return new PartDetails("Dummy", 0, 0, 0);
        }

        public PartDetails GetCompoundDetailsFromCode(string _code)
        {
            foreach (var item in playerData.compundsOwned)
            {
                if (item.code == _code)
                {
                    return new PartDetails(item.code, item.timesUsed, item.maxValue, item.performanceIndex);
                }
            }
            return new PartDetails("Dummy", 0, 0, 0);
        }

        public PartDetails GetTransmissionDetailsFromCode(string _code)
        {
            foreach (var item in playerData.transmissionsOwned)
            {
                if (item.code == _code)
                {
                    return new PartDetails(item.code, item.timesUsed, item.maxValue, item.performanceIndex);
                }
            }
            return new PartDetails("Dummy", 0, 0, 0);
        }

#endregion

#region Get Detailed Data from code

        public EngineDetailedData GetDetailedEngineFromCode(string code)
        {
            PartDetails item = GetEngineDetailsFromCode(code);
            EngineDetailedData temp = new EngineDetailedData();
            temp.partDetails = new PartDetails(item.code, item.timesUsed, item.maxValue, item.performanceIndex);
            temp.engineData = GetEngineDataFromCode(code);
            return temp;
        }

        public NitroDetailedData GetDetailedNitroFromCode(string code)
        {
            PartDetails item = GetNitroDetailsFromCode(code);
            NitroDetailedData temp = new NitroDetailedData();
            temp.partDetails = new PartDetails(item.code, item.timesUsed, item.maxValue, item.performanceIndex);
            temp.nitroData = GetNitroDataFromCode(code);
            return temp;
        }

        public SteeringDetailedData GetDetailedSteeringFromCode(string code)
        {
            PartDetails item = GetSteeringDetailsFromCode(code);
            SteeringDetailedData temp = new SteeringDetailedData();
            temp.partDetails = new PartDetails(item.code, item.timesUsed, item.maxValue, item.performanceIndex);
            temp.steeringData = GetSteeringDataFromCode(code);
            return temp;
        }

        public WheelDetailedData GetDetailedWheelFromCode(string code)
        {
            PartDetails item = GetWheelDetailsFromCode(code);
            WheelDetailedData temp = new WheelDetailedData();
            temp.partDetails = new PartDetails(item.code, item.timesUsed, item.maxValue, item.performanceIndex);
            temp.wheelData = GetWheelDataFromCode(code);
            return temp;
        }

        public BrakeDetailedData GetDetailedBrakeFromCode(string code)
        {
            PartDetails item = GetBrakeDetailsFromCode(code);
            BrakeDetailedData temp = new BrakeDetailedData();
            temp.partDetails = new PartDetails(item.code, item.timesUsed, item.maxValue, item.performanceIndex);
            temp.brakeData = GetBrakeDataFromCode(code);
            return temp;
        }

        public FrameDetailedData GetDetailedFrameFromCode(string code)
        {
            PartDetails item = GetFrameDetailsFromCode(code);
            FrameDetailedData temp = new FrameDetailedData();
            temp.partDetails = new PartDetails(item.code, item.timesUsed, item.maxValue, item.performanceIndex);
            temp.frameData = GetFrameDataFromCode(code);
            return temp;
        }

        public SuspensionDetailedData GetDetailedSuspensionFromCode(string code)
        {
            PartDetails item = GetSuspensionDetailsFromCode(code);
            SuspensionDetailedData temp = new SuspensionDetailedData();
            temp.partDetails = new PartDetails(item.code, item.timesUsed, item.maxValue, item.performanceIndex);
            temp.suspensionData = GetSuspensionDataFromCode(code);
            return temp;
        }

        public CompoundDetailedData GetDetailedCompoundFromCode(string code)
        {
            PartDetails item = GetCompoundDetailsFromCode(code);
            CompoundDetailedData temp = new CompoundDetailedData();
            temp.partDetails = new PartDetails(item.code, item.timesUsed, item.maxValue, item.performanceIndex);
            temp.compoundData = GetCompoundDataFromCode(code);
            return temp;
        }

        public TransmissionDetailedData GetDetailedTransmissionFromCode(string code)
        {
            PartDetails item = GetTransmissionDetailsFromCode(code);
            TransmissionDetailedData temp = new TransmissionDetailedData();
            temp.partDetails = new PartDetails(item.code, item.timesUsed, item.maxValue, item.performanceIndex);
            temp.transmissionData = GetTransmissionDataFromCode(code);
            return temp;
        }

#endregion

#region Get MasterData From Code

        public EngineData GetEngineDataFromCode(string code)
        {
            if (masterData.engines == null) return new EngineData("Dummy", 0, 0, 0, 0, 0);
            for (int i = 0; i < masterData.engines.Length; i++)
            {
                if (code == masterData.engines[i].componentCode)
                {
                    Debug.Log("Engine: " + code);
                    return new EngineData(masterData.engines[i].componentCode, masterData.engines[i].maxUsage, masterData.engines[i].acceleration,
                        masterData.engines[i].topSpeed, masterData.engines[i].fuelEfficiency, masterData.engines[i].performanceIndex);
                }
            }
                    Debug.Log("Engine null: " + code);
            return new EngineData("Dummy", 0, 0, 0, 0, 0);
        }

        public NitroData GetNitroDataFromCode(string code)
        {
            if (masterData.nitros == null) return new NitroData("Dummy", 0, 0, 0, 0);
            for (int i = 0; i < masterData.nitros.Length; i++)
            {
                if (code == masterData.nitros[i].componentCode)
                {
                    return new NitroData(masterData.nitros[i].componentCode, masterData.nitros[i].maxUsage, masterData.nitros[i].accelerationOnUsing,
                        masterData.nitros[i].duration, masterData.nitros[i].performanceIndex);
                }
            }
            return new NitroData("Dummy", 0, 0, 0, 0);
        }

        public SteeringData GetSteeringDataFromCode(string code)
        {
            if (masterData.steerings == null) return new SteeringData("Dummy", 0, 0, 0, 0);
            for (int i = 0; i < masterData.steerings.Length; i++)
            {
                if (code == masterData.steerings[i].componentCode)
                {
                    return new SteeringData(masterData.steerings[i].componentCode, masterData.steerings[i].maxUsage, masterData.steerings[i].handling,
                        masterData.steerings[i].stability, masterData.steerings[i].performanceIndex);
                }
            }
            return new SteeringData("Dummy", 0, 0, 0, 0);
        }

        public WheelData GetWheelDataFromCode(string code)
        {
            if (masterData.wheels == null) return new WheelData("Dummy", 0, 0, 0, 0, 0, 0);
            for (int i = 0; i < masterData.wheels.Length; i++)
            {
                if (code == masterData.wheels[i].componentCode)
                {
                    return new WheelData(masterData.wheels[i].componentCode, masterData.wheels[i].maxUsage, masterData.wheels[i].acceleration,
                        masterData.wheels[i].deceleration, masterData.wheels[i].weight, masterData.wheels[i].sportiveness, masterData.wheels[i].performanceIndex);
                }
            }
            return new WheelData("Dummy", 0, 0, 0, 0, 0, 0);
        }

        public BrakeData GetBrakeDataFromCode(string code)
        {
            if (masterData.brakes == null) return new BrakeData("Dummy", 0, 0, 0, 0);
            for (int i = 0; i < masterData.brakes.Length; i++)
            {
                if (code == masterData.brakes[i].componentCode)
                {
                    return new BrakeData(masterData.brakes[i].componentCode, masterData.brakes[i].maxUsage, masterData.brakes[i].fuelEfficiency,
                        masterData.brakes[i].deceleration, masterData.brakes[i].performanceIndex);
                }
            }
            return new BrakeData("Dummy", 0, 0, 0, 0);
        }

        public FrameData GetFrameDataFromCode(string code)
        {
            if (masterData.frames == null) return new FrameData("Dummy", 0, 0, 0, 0, 0);
            for (int i = 0; i < masterData.frames.Length; i++)
            {
                if (code == masterData.frames[i].componentCode)
                {
                    return new FrameData(masterData.frames[i].componentCode, masterData.frames[i].maxUsage, masterData.frames[i].acceleration,
                        masterData.frames[i].weight, masterData.frames[i].fuelEfficiency, masterData.frames[i].performanceIndex);
                }
            }
            return new FrameData("Dummy", 0, 0, 0, 0, 0);
        }

        public SuspensionData GetSuspensionDataFromCode(string code)
        {
            if (masterData.suspensions == null) return new SuspensionData("Dummy", 0, 0, 0, 0, 0, 0);
            for (int i = 0; i < masterData.suspensions.Length; i++)
            {
                if (code == masterData.suspensions[i].componentCode)
                {
                    return new SuspensionData(masterData.suspensions[i].componentCode, masterData.suspensions[i].maxUsage,
                        masterData.suspensions[i].acceleration, masterData.suspensions[i].topSpeed, masterData.suspensions[i].sportiveness,
                        masterData.suspensions[i].stability, masterData.suspensions[i].performanceIndex);
                }
            }
            return new SuspensionData("Dummy", 0, 0, 0, 0, 0, 0);
        }

        public CompoundData GetCompoundDataFromCode(string code)
        {
            if (masterData.compounds == null) return new CompoundData("Dummy", 0, 0, 0, 0, 0, 0, 0);
            for (int i = 0; i < masterData.compounds.Length; i++)
            {
                if (code == masterData.compounds[i].componentCode)
                {
                    return new CompoundData(masterData.compounds[i].componentCode, masterData.compounds[i].maxUsage,
                        masterData.compounds[i].acceleration, masterData.compounds[i].topSpeed, masterData.compounds[i].sportiveness,
                        masterData.compounds[i].stability, masterData.compounds[i].deceleration, masterData.compounds[i].performanceIndex);
                }
            }
            return new CompoundData("Dummy", 0, 0, 0, 0, 0, 0, 0);
        }

        public TransmissionData GetTransmissionDataFromCode(string code)
        {
            if (masterData.transmissions == null) return new TransmissionData("Dummy", 0, 0, 0, 0, 0);
            for (int i = 0; i < masterData.transmissions.Length; i++)
            {
                if (code == masterData.transmissions[i].componentCode)
                {
                    return new TransmissionData(masterData.transmissions[i].componentCode, masterData.transmissions[i].maxUsage,
                        masterData.transmissions[i].acceleration, masterData.transmissions[i].topSpeed, masterData.transmissions[i].fuelEfficiency, masterData.transmissions[i].performanceIndex);
                }
            }

            return new TransmissionData("Dummy", 0, 0, 0, 0, 0);
        }

#endregion

        public CarData GetCarDataFromObjectAndCarDetails(GameObject carObject, CarDetails _carDetails)
        {
            return new CarData(carObject, _carDetails.car.colorCode, _carDetails.code, GetCarRenderFromCarCode(_carDetails.code),
                GetDetailedEngineFromCode(_carDetails.car.engineCode), GetDetailedNitroFromCode(_carDetails.car.nitroCode),
                GetDetailedSteeringFromCode(_carDetails.car.steeringCode), GetDetailedWheelFromCode(_carDetails.car.wheelsCode),
                GetDetailedBrakeFromCode(_carDetails.car.brakeCode), GetDetailedFrameFromCode(_carDetails.car.frameCode),
                GetDetailedSuspensionFromCode(_carDetails.car.suspensionCode), GetDetailedCompoundFromCode(_carDetails.car.compoundCode),
                GetDetailedTransmissionFromCode(_carDetails.car.transmissionCode));
        }

        public CarDetailedData GetCarDetailedDataFromMasterFromCode(string _code)
        {
            CarDetails temp = new CarDetails(_code, 0, null);
            for (int i = 0; i < masterData.cars.Length; i++)
            {
                if (masterData.cars[i].componentCode == _code)
                {
                    temp.car = masterData.cars[i];
                    break;
                }
            }
            CarDetailedData temp1 = new CarDetailedData();
            for (int i = 0; i < carsList.Count; i++)
            {
                if (_code.ToLower().Contains(carsList[i].name.ToLower()))
                {
                    temp1.carDetails = temp;
                    temp1.carObject = carsList[i];
                    temp1.carDefaultCode = carsList[i].name;
                    break;
                }
                if (i == carsList.Count - 1)
                {
                    Debug.LogError("Pkda gya: " + _code);
                    temp1.carDetails = temp;
                    temp1.carObject = null;
                    temp1.carDefaultCode = "commoncar";
                    break;
                }
            }
            if (temp == null)
            {
                Debug.LogError("Pkda gya1: " + _code);
            }
            if (temp.car==null)
            {
                Debug.LogError("Pkda gya2: " + _code);
            }
            return temp1;
        }

        public CarDetailedData GetCarDetailedDataFromPlayerDataFromCarDetails(CarDetails temp2)
        {
            Car temp3 = new Car(temp2.car.nitroCode, temp2.car.engineCode, temp2.car.steeringCode, temp2.car.wheelsCode, temp2.car.brakeCode,
                temp2.car.frameCode, temp2.car.suspensionCode, temp2.car.compoundCode, temp2.car.transmissionCode, temp2.car.colorCode, temp2.car.carType);
            CarDetails temp = new CarDetails(temp2.code, temp2.timesUsed, temp3);
            CarDetailedData temp1 = new CarDetailedData();
            for (int i = 0; i < carsList.Count; i++)
            {
                if (temp.code.ToLower().Contains(carsList[i].name.ToLower()))
                {
                    temp1.carDetails = temp;
                    temp1.carObject = carsList[i];
                    temp1.carDefaultCode = carsList[i].name;
                    break;
                }
            }
            return temp1;
        }

    }
}