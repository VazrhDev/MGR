using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace MGR
{
    public class MapDataScript : MonoBehaviour
    {
        MapDataDetailed myDetailedMapData;

        [SerializeField] TextMeshProUGUI countryName;
        [SerializeField] Image mapThumbnail;
        [SerializeField] TextMeshProUGUI mapName;
        [SerializeField] TextMeshProUGUI trackType;
        [SerializeField] TextMeshProUGUI trackRoute;
        [SerializeField] TextMeshProUGUI totalLength;
        [SerializeField] TextMeshProUGUI weatherConditions;

        public void SetMapdata(MapDataDetailed mapData)
        {
            myDetailedMapData = mapData;
            countryName.text = mapData.mapData.countryName;
            mapThumbnail.sprite = mapData.render;
            mapName.text = "Name: " + mapData.mapData.mapName;
            trackType.text = "Track Type: " + mapData.mapData.trackType;
            totalLength.text = "Total Length: " + mapData.mapData.totalLength;
            trackRoute.text = "Track Route: " + mapData.mapData.trackRoute;
            weatherConditions.text = "Weather Conditions: " + mapData.mapData.weatherCondition;
        }

        public void SelectMapAndStart()
        {
            GameManager.instance.menuScript.SetMapAndStartGame(myDetailedMapData);
        }
    }
}