using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGR
{
    public class LobbyMapViewer : MonoBehaviour
    {

        [SerializeField] GameObject CarHolder;


        private void Awake()
        {
            Instantiate(GameManager.instance.currentCarData.carModel, CarHolder.transform);
        }
    }
}