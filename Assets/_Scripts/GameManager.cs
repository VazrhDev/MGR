using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MasterDataNS;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

namespace MGR
{
    [SerializeField]
    public enum GameMode
    {
        Practice,
        DailyRace
    }

    public enum Maps
    {
        map1,
        map2
    }

    public enum DifficultyMode
    {
        Easy,
        Medium,
        Hard
    }

    public class GameManager : MonoBehaviour
    {

        public MenuScript menuScript;

        public static GameManager instance;
        public GameMode gameMode;
        public Maps map;
        public DifficultyMode difficultyMode;
        public bool onRandomMap = false;

        public int mySpawnPosition = 0;
        public string currentSelectedCar;

        public CarData currentCarData;
        public MapDataDetailed selectedMapData;
        public DateTime currentDateTime;
        public string MapSceneName;

        private bool hasFirstTimeLoaded = true;



        private void Awake()
        {
            if (instance == null) instance = this;
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public void SpawnCArsTemp()
        {
            Invoke(nameof(SpawnCars), 2);
        }

        [SerializeField] CarDetailedData selectedCar = null;
        public void SpawnCars()
        {
            Debug.Log("Spawn cars");
            selectedCar = null;
            if (!PlayfabManager.instance.isDevModeOn)
            {
                if (DatabaseHandler.instance.playerData != null)
                {

                    foreach (var item in DatabaseHandler.instance.playerDetailedData.carsOwned)
                    {
                        if (item.carDetails == null) { item.carDetails = new CarDetails("dummy", 0, null); continue; }
                        if (item.carDetails.code == DatabaseHandler.instance.playerData.selectedCarCode)
                        {
                            Debug.LogError("Found");
                            selectedCar = item;
                            break;
                        }
                    }
                }
            }
            else
            {
                selectedCar = DatabaseHandler.instance.playerDetailedData.carsOwned.Last();
                //selectedCar = DatabaseHandler.instance.playerDetailedData.carsOwned[UnityEngine.Random.Range(0, DatabaseHandler.instance.playerDetailedData.carsOwned.Count)];
            }
            if (selectedCar == null)
            {
                selectedCar = DatabaseHandler.instance.playerDetailedData.carsOwned[UnityEngine.Random.Range(0, DatabaseHandler.instance.playerDetailedData.carsOwned.Count)];
                while (selectedCar.carDetails == null || selectedCar.carDetails.car == null)
                    selectedCar = DatabaseHandler.instance.playerDetailedData.carsOwned[UnityEngine.Random.Range(0, DatabaseHandler.instance.playerDetailedData.carsOwned.Count)];
            }
            //SetCarColor(selectedCar.carObject, selectedCar.carDetails.car.colorCode);
            GameObject spawnedObject = Instantiate(selectedCar.carObject, menuScript.menuCarPosition);
            //Debug.LogError(spawnedObject.transform.localEulerAngles);
            menuScript.currentSelectedCarObj = spawnedObject;
            spawnedObject.AddComponent<MenuCarScript>().myCarData = DatabaseHandler.instance.GetCarDataFromObjectAndCarDetails(selectedCar.carObject, selectedCar.carDetails);
            menuScript.menuCars = new List<MenuCarScript>();
            menuScript.menuCars.Add(spawnedObject.GetComponent<MenuCarScript>());
            currentCarData = DatabaseHandler.instance.GetCarDataFromObjectAndCarDetails(selectedCar.carObject, selectedCar.carDetails);
            //DatabaseHandler.instance.playerData.selectedCarCode

            DatabaseHandler.instance.TooBject2();
            DatabaseHandler.instance.SetDetailedData();
            InevitableUI.instance.HideLoadingScreen();
        }

        public void NFTCheck()
        {
            //Checking if first time login then Add common car in player data
            if (DatabaseHandler.instance.playerData.firstLogin)
            {
                Debug.LogError("First login");
                for (int i = 0; i < DatabaseHandler.instance.masterData.cars.Length; i++)
                {
                    if (DatabaseHandler.instance.masterData.cars[i].carType == CarType.Common)
                    {
                        Car carTemp = DatabaseHandler.instance.masterData.cars[i];
                        DatabaseHandler.instance.playerData.carsOwned.Add(new CarDetails(DatabaseHandler.instance.masterData.cars[i].componentCode,
                            0, carTemp));

                        DatabaseHandler.instance.playerDetailedData.carsOwned.Add(
                            DatabaseHandler.instance.GetCarDetailedDataFromMasterFromCode(DatabaseHandler.instance.masterData.cars[i].componentCode));
                        break;
                    }
                }
                DatabaseHandler.instance.playerData.firstLogin = false;
            }
            //for (int i = 0; i < DatabaseHandler.instance.playerData.carsOwned.Count; i++)
            //{
            //    DatabaseHandler.instance.playerDetailedData.carsOwned.Add(
            //                    DatabaseHandler.instance.GetCarDetailedDataFromMasterFromCode(DatabaseHandler.instance.playerData.carsOwned[i].code));
            //}
            //Checking for new Bought NFTs and adding in player Data
            for (int i = 0; i < WalletData.instance.ownedNfts.Length; i++)
            {
                bool hasFound = false;
                for (int j = 0; j < DatabaseHandler.instance.playerData.carsOwned.Count; j++)
                {
                    if (WalletData.instance.ownedNfts[i] == DatabaseHandler.instance.playerData.carsOwned[j].code)
                    {
                        DatabaseHandler.instance.playerDetailedData.carsOwned.Add(
                                            DatabaseHandler.instance.GetCarDetailedDataFromPlayerDataFromCarDetails(DatabaseHandler.instance.playerData.carsOwned[i]));
                        hasFound = true;
                        break;
                    }
                }
                if (!hasFound)
                {
                    for (int k = 0; k < DatabaseHandler.instance.masterData.cars.Length; k++)
                    {
                        if (DatabaseHandler.instance.masterData.cars[k].componentCode == WalletData.instance.ownedNfts[i])
                        {
                            Car carTemp = DatabaseHandler.instance.masterData.cars[k];
                            Debug.LogError("Adding new nft in player data: " + carTemp.componentCode);
                            DatabaseHandler.instance.playerData.carsOwned.Add(new CarDetails(DatabaseHandler.instance.masterData.cars[k].componentCode,
                                0, carTemp));

                            DatabaseHandler.instance.playerDetailedData.carsOwned.Add(
                                DatabaseHandler.instance.GetCarDetailedDataFromMasterFromCode(DatabaseHandler.instance.masterData.cars[k].componentCode));
                            break;
                        }
                        
                    }
                }
            }

            //Checking for Sold NFTs from player Database
            List<CarDetails> carsToRemove = new List<CarDetails>();
            for (int i = 0; i < DatabaseHandler.instance.playerData.carsOwned.Count; i++)
            {
                bool hasFound = false;
                for (int j = 0; j < WalletData.instance.ownedNfts.Length; j++)
                {
                    if (WalletData.instance.ownedNfts[j] == DatabaseHandler.instance.playerData.carsOwned[i].code)
                    {
                        hasFound = true;
                        break;
                    }
                }
                if (!hasFound)
                {
                    if (DatabaseHandler.instance.playerData.carsOwned[i].car.carType != CarType.Common)
                    {
                        Debug.LogError("Sold Nft: " + DatabaseHandler.instance.playerData.carsOwned[i].code);
                        carsToRemove.Add(DatabaseHandler.instance.playerData.carsOwned[i]);
                    }
                }
            }

            //Removing Sold NFTs from player Database
            for (int i = 0; i < carsToRemove.Count; i++)
            {
                DatabaseHandler.instance.playerData.carsOwned.Remove(carsToRemove[i]);
            }

            carsToRemove.Clear();

            DatabaseHandler.instance.SavePlayerDataInDB();
            SpawnCars();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {

            if (hasFirstTimeLoaded && scene.name == "MenuScene")
            {
                hasFirstTimeLoaded = false;
                return;
            }

            if (scene.name == "MenuScene")
            {
                InevitableUI.instance.ShowLoadingScreen();

                Invoke(nameof(SpawnCars), 0.5f);
                //SpawnCars();
            }
        }

        public void SetCarColor(GameObject spawnObject, string colorCode)
        {
            for (int index = 0; index < spawnObject.transform.childCount; index++)
            {
                spawnObject.transform.GetChild(index).transform.TryGetComponent<Renderer>(out Renderer renderer);

                if (spawnObject.transform.GetChild(index).name.Contains("_body") && renderer)
                {
                    Master.ColoredLog("blue", "TRY CHANGING COLOR");
                    foreach (Material mat in renderer.sharedMaterials)
                    {
                        mat.SetColor("_BaseColor", ColorFromColorCode(colorCode));
                    }
                }
                else
                {
                    GameObject childObject = spawnObject.transform.GetChild(index).gameObject;

                    for (int index1 = 0; index1 < childObject.transform.childCount; index1++)
                    {
                        childObject.transform.GetChild(index1).transform.TryGetComponent<Renderer>(out Renderer childRenderer);

                        if (childObject.transform.GetChild(index1).name.Contains("_body") && childRenderer)
                        {
                            foreach (Material mat in childRenderer.sharedMaterials)
                            {
                                mat.SetColor("_BaseColor", ColorFromColorCode(colorCode));
                            }
                        }
                    }
                }
            }
        }

        public Color ColorFromColorCode(string colorCodeInBytes)
        {
            Color target;
            
            string[] splittedBytes = colorCodeInBytes.Split(',');

            if (splittedBytes.Length == 4)
            {
                target = new Color32(ByteVerificationForString(splittedBytes[0]), ByteVerificationForString(splittedBytes[1]),
                    ByteVerificationForString(splittedBytes[2]), ByteVerificationForString(splittedBytes[3]));
            }
            else target = Color.black;
            
            Debug.LogError(target);
            return target;
        }

        byte ByteVerificationForString(string stringgedByte)
        {
            int numberedString = int.Parse(stringgedByte);
            if (numberedString <= 255 && numberedString >= 0)
            {
                return (byte)numberedString;
            }
            else
            {
                numberedString = UnityEngine.Random.Range(0, 256);
                return (byte)numberedString;
            }
        }
    }
}