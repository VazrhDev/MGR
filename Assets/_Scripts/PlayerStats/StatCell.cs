using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatCell : MonoBehaviour
{
    [SerializeField] TMP_Text position;
    [SerializeField] TMP_Text gameMode;
    [SerializeField] TMP_Text carUsed;
    [SerializeField] TMP_Text totalPlayers;
    [SerializeField] TMP_Text mapName;
    [SerializeField] TMP_Text dateTime;

    public void SetStateData(string Position, string GameMode, string CarUsed, string TotalPlayers, string MapName, string DateTime)
    {
        position.text = Position;
        gameMode.text = GameMode;
        carUsed.text = CarUsed;
        totalPlayers.text = TotalPlayers;
        mapName.text = MapName;
        dateTime.text = DateTime;
    }
}
