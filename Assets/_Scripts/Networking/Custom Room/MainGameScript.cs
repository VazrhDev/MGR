using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun.UtilityScripts;
using System.Linq;

namespace MGR
{
    public enum GameEvents
    {
        SpawnPlayer
    }

    public class MainGameScript : MonoBehaviourPunCallbacks
    {
        public static MainGameScript instance;

        public RaceData raceData;

        public GameObject playerPrefab;

        public List<Transform> SpawnPoints;

        private void Awake()
        {
            instance = this;
        }

        public void SetRaceData()
        {
            if (GameManager.instance)
            {
                raceData = new RaceData(GameManager.instance.gameMode.ToString(), GameManager.instance.currentCarData.carCode, PhotonNetwork.CurrentRoom.PlayerCount,
                    GameManager.instance.selectedMapData.mapData.mapName, PhotonNetwork.CurrentRoom.PlayerCount, GameManager.instance.currentDateTime.ToString());
            }
            else Invoke(nameof(SetRaceData), 0.1f);
        }

        public void SetRaceDataAndUpload(int positionOfPlayer)
        {
            raceData.position = positionOfPlayer;
            if (DatabaseHandler.instance.playerStats == null) DatabaseHandler.instance.playerStats = new PlayerStats();
            if (DatabaseHandler.instance.playerStats.raceData == null)
            {
                DatabaseHandler.instance.playerStats.raceData = new List<RaceData>();
            }
            DatabaseHandler.instance.playerStats.raceData.Add(raceData);
            DatabaseHandler.instance.SavePlayerStatsInDB();

            // Updating Points earned by player on completing the race
            if (GameManager.instance.gameMode == GameMode.DailyRace)
            {
                DatabaseHandler.instance.playerData.pointsEarned += DatabaseHandler.instance.masterData.points[positionOfPlayer - 1];
                //DatabaseHandler.instance.SavePlayerDataInDB();
            }

            // Upating car's performance index
            float oldPerformanceIndex = 0;
            float newPerformanceIndex = 0;
            foreach (var item in DatabaseHandler.instance.playerDetailedData.carsOwned)
            {
                if (item.carDetails.code == GameManager.instance.currentCarData.carCode)
                {
                    oldPerformanceIndex = item.carDetails.car.performanceIndex;
                    if(DatabaseHandler.instance.masterData.performanceIndex.Length > 0)
                        item.carDetails.car.performanceIndex += DatabaseHandler.instance.masterData.performanceIndex[positionOfPlayer - 1];
                    newPerformanceIndex = item.carDetails.car.performanceIndex;

                    break;
                }
            }

            float changeInIndex = ((newPerformanceIndex - oldPerformanceIndex) / oldPerformanceIndex) * 100;

            // Update car's components performance index
            CarData carData = GameManager.instance.currentCarData;
            carData.engineDetails.partDetails.performanceIndex += (carData.engineDetails.partDetails.performanceIndex + changeInIndex) / 100;
            carData.brakesDetails.partDetails.performanceIndex += (carData.brakesDetails.partDetails.performanceIndex + changeInIndex) / 100;
            carData.compoundsDetails.partDetails.performanceIndex += (carData.compoundsDetails.partDetails.performanceIndex + changeInIndex) / 100;
            carData.framesDetails.partDetails.performanceIndex += (carData.framesDetails.partDetails.performanceIndex + changeInIndex) / 100;
            carData.nitroDetails.partDetails.performanceIndex += (carData.nitroDetails.partDetails.performanceIndex + changeInIndex) / 100;
            carData.steeringDetails.partDetails.performanceIndex += (carData.steeringDetails.partDetails.performanceIndex + changeInIndex) / 100;
            carData.suspensionDetails.partDetails.performanceIndex += (carData.suspensionDetails.partDetails.performanceIndex + changeInIndex) / 100;
            carData.transmissionDetails.partDetails.performanceIndex += (carData.transmissionDetails.partDetails.performanceIndex + changeInIndex) / 100;
            carData.suspensionDetails.partDetails.performanceIndex += (carData.suspensionDetails.partDetails.performanceIndex + changeInIndex) / 100;


            // Saving Data and Stats in Database
            DatabaseHandler.instance.SavePlayerStatsInDB();
            DatabaseHandler.instance.SavePlayerDataInDB();
        }

        private void OnEnable()
        {
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        }

        private void OnDisable()
        {
            PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        }

        void OnEvent(EventData photonEvent)
        {

        }

        void Start()
        {
            PlayfabManager.instance.checkDateTimeType = CheckDateTimeType.IncreaseDRMPlays;
            PlayfabManager.instance.CheckDateTime();

            GameObject SpawnPointsParent = GameObject.Find("SpawnPoints");
            for(int i = 0; i < SpawnPointsParent.transform.childCount; i++)
            {
                SpawnPoints.Add(SpawnPointsParent.transform.GetChild(i));
            }
            int spawnNumber = Mathf.Clamp(PhotonNetwork.LocalPlayer.GetPlayerNumber(), 0, SpawnPoints.Count);
            Debug.Log(spawnNumber);
            Debug.Log(SpawnPoints.Count);
            Debug.Log(SpawnPoints[spawnNumber].position);
            GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, SpawnPoints[spawnNumber].position + new Vector3(0, 10f, 0), SpawnPointsParent.transform.rotation);
            //GameObject player = PhotonNetwork.InstantiateRoomObject(playerPrefab.name, SpawnPoints[spawnNumber].position, SpawnPointsParent.transform.rotation);
            Debug.Log(player.transform.position);
            player.GetComponent<CarPropertiesSync>().CallSpawnCarModel();
            
            //GameObject carModel = Instantiate(GameManager.instance.currentCarData.carModel);
            //carModel.transform.SetParent(player.GetComponent<CarMovementController>().CarModelHolder.transform);
            //carModel.transform.localScale = new Vector3(0.31f, 0.31f, 0.31f);
            //carModel.transform.localPosition = new Vector3(0, 0, 0);    
        }

    }
}