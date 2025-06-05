using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MGR
{
    public class CarUIManager : MonoBehaviour
    {

        [SerializeField] VehicleControl carController;
        [SerializeField] GameObject Needdle;
        [SerializeField] TextMeshProUGUI kph;
        [SerializeField] TextMeshProUGUI gearNum;
        [SerializeField] Image nitrusSlider;
        [SerializeField] float turningRate = 0.5f;

        [SerializeField] Image CheckPointIcon;
        [SerializeField] Image CheckPointArrow;
        [SerializeField] float indicatorOffset = 1f;
        [SerializeField] float indicatorLimit = 0.7f;

        private float startPos, endPos;
        private float desierdPos;

        PlayerRaceData playerRacedata;

        private void Awake()
        {
            startPos = 22.2f;
            endPos = -205.74f;
            playerRacedata = gameObject.transform.parent.GetComponent<PlayerRaceData>();
        }

        private void Update()
        {
            UpdateIndicator();
        }

        private void FixedUpdate()
        {
            kph.text = carController.speed.ToString("0");
            UpdateNeedel();
            nitrusUI();
        }
       

        private void UpdateNeedel()
        {
            
            desierdPos = startPos - endPos;
            float temp = (carController.motorRPM / 10000);
            Needdle.transform.eulerAngles = new Vector3(0, 0, (int )(startPos - temp * desierdPos));
            //Needdle.transform.rotation = Quaternion.RotateTowards(Needdle.transform.rotation, Quaternion.Euler(new Vector3(0, 0, (int)(startPos - temp * desierdPos))), turningRate * Time.deltaTime);
        }

        public void changeGear()
        {
            //gearNum.text = (!carController.Reverse) ? (carController.GearNum + 1).ToString() : "R";
        }

        private void nitrusUI()
        {
            //nitrusSlider.fillAmount = carController.BoostValue / 45;
        }

        void UpdateIndicator()
        {
            //Finding the checkpoint in ViewPort
            Vector3 CheckPoint = RaceManager.Instance.GetCheckpointPosition(playerRacedata.CurrentCheckpoint) + Vector3.up * indicatorOffset;
            Vector3 viewportPoint = Camera.main.WorldToViewportPoint(CheckPoint);
            bool behindCam = viewportPoint.z < 0;
            viewportPoint.z = 0f;
            //position calculations
            Vector3 viewportCentre = new Vector3(0.5f, 0.5f, 0);
            Vector3 fromCentre = viewportPoint - viewportCentre;
            float halfLimit = indicatorLimit / 2f;
            bool showArrow = false;

            if (behindCam)
            {
                // limit the distace from center 
                fromCentre = -fromCentre.normalized * halfLimit;
                showArrow = true;
            }
            else
            {
                if (fromCentre.magnitude > halfLimit)
                {
                    fromCentre = fromCentre.normalized * halfLimit;
                    showArrow = true;
                }
            }
            //Updating the position
            CheckPointArrow.gameObject.SetActive(showArrow);
            CheckPointArrow.rectTransform.rotation = Quaternion.FromToRotation(Vector3.up, fromCentre);
            CheckPointIcon.rectTransform.position = Camera.main.ViewportToScreenPoint(fromCentre + viewportCentre);
        }
    }

}