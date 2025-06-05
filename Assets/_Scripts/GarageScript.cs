using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using UnityStandardAssets.Effects;
using System;

namespace MGR
{

    public class GarageScript : MonoBehaviour
    {
        public static GarageScript instance;

        [SerializeField] GameObject garagePanel;
        [SerializeField] GameObject selectCarPanel;

        [SerializeField] bool isPanelUp = false;
        [SerializeField] float TransitionSpeed = 1;
        [SerializeField] GameObject carRenderPrefab;
        [SerializeField] Transform scrollGrandParent;
        [SerializeField] RectTransform scrollContent;
        [SerializeField] GameObject firstPanel;
        [SerializeField] GameObject secondPanel;
        [SerializeField] Transform carVersionsParent;
        [SerializeField] TextMeshProUGUI selectedCarName;
        [SerializeField] TextMeshProUGUI selectedDefaultCarName;

        [SerializeField] float grandParentBottomLocalPos;
        [SerializeField] float grandParentTopLocalPos;
        
        [SerializeField] Sprite dummyCarRender;

        [Header("Car Stats Panel")]
        [SerializeField] Slider engineBar;
        [SerializeField] Slider nitroBar;
        [SerializeField] Slider stabilityBar;
        [SerializeField] Slider brakeBar;

        [Header("Player Stats Panel")]
        [SerializeField] GameObject playerStatsPanel;
        [SerializeField] GameObject statPlacer;
        [SerializeField] GameObject statsCell;
        List<GameObject> playerStats = new List<GameObject>();
        bool isStatViewON = false;

        bool isCarViewOn = false;
        Vector3 mPrevPos = Vector3.zero;
        Vector3 mposDelta = Vector3.zero;
        [SerializeField]GameObject CarHolder;
        [SerializeField] float timeToRotate;

        private void Awake()
        {
            instance = this;
            GameManager.instance.menuScript.closeButtons(garagePanel);
            GameManager.instance.menuScript.closeButtons(playerStatsPanel);
        }


        private void Update()
        {
            if(Input.GetMouseButton(0) && isCarViewOn)
            {
                mposDelta = Input.mousePosition - mPrevPos;
                CarHolder.transform.Rotate(CarHolder.transform.up, Vector3.Dot(mposDelta, -Camera.main.transform.right) * Time.deltaTime * timeToRotate, Space.World);
            }
            mPrevPos = Input.mousePosition;
        }


        IEnumerator OpenGarageAnime()
        {
            GameManager.instance.menuScript.closeButtons(selectCarPanel);
            SetStats();
            selectedCarName.text = GameManager.instance.currentCarData.carCode;
            SpawnCarRenders();
            GameManager.instance.menuScript.SetCameraTarget(4);
            yield return new WaitForSeconds(0.1f);
            selectCarPanel.SetActive(true);
            garagePanel.SetActive(true);
            firstPanel.SetActive(true);
            yield return new WaitForSeconds(GameManager.instance.menuScript.PanelTransitionTime);
            GameManager.instance.menuScript.OpenButtons(garagePanel);
        }
        IEnumerator BackfromGarageAnime()
        {
            GameManager.instance.menuScript.SetCameraTarget(1);
            GameManager.instance.menuScript.closeButtons(garagePanel);

            yield return new WaitForSeconds(0.1f);
            garagePanel.SetActive(false);
            selectCarPanel.SetActive(true);

            yield return new WaitForSeconds(GameManager.instance.menuScript.PanelTransitionTime);
            GameManager.instance.menuScript.OpenButtons(selectCarPanel);

        }

        public void OpenGaragePanel()
        {
            StartCoroutine(OpenGarageAnime());
        }

        public void OpenPlayerStats()
        {
            StartCoroutine(OpenPlayerStatus());
        }

        public void BackButton()
        {
            if(isPanelUp && !isCarViewOn && !isStatViewON)
            {
                BackFromSelectingCar();
            }
            else if(!isPanelUp && !isCarViewOn && !isStatViewON)
            {
                StartCoroutine(BackfromGarageAnime());
                //BackFromGarage();
            }
            else if(!isPanelUp && isCarViewOn && !isStatViewON)
            {
                BackFromCarViewer();
            }
            else if(isStatViewON && !isPanelUp && !isCarViewOn)
            {
                StartCoroutine(ClosePlayerStats());
            }
        }

        void BackFromGarage()
        {
            GameManager.instance.menuScript.SetCameraTarget(1);
            selectCarPanel.SetActive(true);
            garagePanel.SetActive(false);
        }

        List<string> temp = new List<string>();
        List<CarDetailedData> temp1 = new List<CarDetailedData>();

        void SpawnCarRenders()
        {
            for (int i = 0; i < scrollContent.childCount; i++)
            {
                Destroy(scrollContent.GetChild(i).gameObject);
            }
            temp.Clear();
            temp1 = DatabaseHandler.instance.playerDetailedData.carsOwned;
            for (int i = 0; i < temp1.Count; i++)
            {
                if (!temp.Contains(temp1[i].carDefaultCode.ToLower()))
                {
                    GameObject temp2 = Instantiate(carRenderPrefab, scrollContent.transform);
                    Debug.LogError(temp1[i].carDefaultCode);
                    for (int j = 0; j < DatabaseHandler.instance.carRenders.transform.childCount; j++)
                    {
                        if (temp1[i].carDefaultCode.ToLower() == DatabaseHandler.instance.carRenders.transform.GetChild(j).name.ToLower())
                        {
                            //Debug.LogError(temp1[i].carDefaultCode);
                            temp2.GetComponent<Image>().sprite = DatabaseHandler.instance.carRenders.transform.GetChild(j).GetComponent<SpriteRenderer>().sprite;
                            temp2.GetComponent<CarRenderClicked>().carDefaultCode = temp1[i].carDefaultCode;
                            temp2.GetComponent<CarRenderClicked>().carRenderType = CarRenderType.JustRender;
                            temp2.GetComponent<CarRenderClicked>().NFTNumberText.gameObject.SetActive(true);
                            temp2.GetComponent<CarRenderClicked>().NFTNumberText.text = temp1[i].carDefaultCode;
                            temp2.GetComponent<RectTransform>().anchoredPosition = new Vector2((temp.Count * 500) + (temp.Count * 100),
                                temp2.GetComponent<RectTransform>().anchoredPosition.y);
                            temp.Add(temp1[i].carDefaultCode.ToLower());
                            break;
                        }
                        else if (j == DatabaseHandler.instance.carRenders.transform.childCount - 1)
                        {
                            Debug.LogError(temp1[i].carDefaultCode);
                            temp2.GetComponent<Image>().sprite = dummyCarRender;
                            temp2.GetComponent<CarRenderClicked>().carDefaultCode = temp1[i].carDefaultCode;
                            temp2.GetComponent<CarRenderClicked>().carRenderType = CarRenderType.JustRender;
                            temp2.GetComponent<CarRenderClicked>().NFTNumberText.gameObject.SetActive(false);
                            temp2.GetComponent<RectTransform>().anchoredPosition = new Vector2((temp.Count * 500) + (temp.Count * 100),
                                temp2.GetComponent<RectTransform>().anchoredPosition.y);
                            temp.Add(temp1[i].carDefaultCode.ToLower());
                            break;
                        }
                    }
                }
            }
            if (temp.Count > 0)
                scrollContent.sizeDelta = new Vector2((500 * temp.Count) + (100 * (temp.Count - 1)), scrollContent.sizeDelta.y);
        }

        public void ClickedOneCar(string carDefCode)
        {
            if (!isPanelUp)
            {
                ShowCarVarients(carDefCode);
                MoveGrandparentUp();
            }
            else
            {
                ShowCarVarients(carDefCode);
            }
        }

        public void SelectACarAndGoBack(string carCode)
        {
            foreach (var item in GameManager.instance.menuScript.menuCars)
            {
                item.gameObject.SetActive(false);
            }
            bool alreadySpawned = false;
            for (int i = 0; i < GameManager.instance.menuScript.menuCars.Count; i++)
            {
                if (GameManager.instance.menuScript.menuCars[i].myCarData.carCode == carCode)
                {
                    GameManager.instance.menuScript.menuCars[i].gameObject.SetActive(true);
                    GameManager.instance.menuScript.currentSelectedCarObj = GameManager.instance.menuScript.menuCars[i].gameObject;
                    GameManager.instance.currentCarData = GameManager.instance.menuScript.menuCars[i].myCarData;
                    selectedCarName.text = carCode;
                    alreadySpawned = true;
                    break;
                }
            }
            if (!alreadySpawned)
            {
                for (int i = 0; i < temp1.Count; i++)
                {
                    if (temp1[i].carDetails.code == carCode)
                    {
                        GameObject spawnedObject = Instantiate((GameObject)temp1[i].carObject, GameManager.instance.menuScript.menuCarPosition);
                        //GameManager.instance.SetCarColor(spawnedObject, temp1[i].carDetails.car.colorCode);
                        spawnedObject.AddComponent<MenuCarScript>().myCarData = DatabaseHandler.instance.GetCarDataFromObjectAndCarDetails((GameObject)temp1[i].carObject, temp1[i].carDetails);
                        GameManager.instance.menuScript.menuCars.Add(spawnedObject.GetComponent<MenuCarScript>());
                        GameManager.instance.currentCarData = DatabaseHandler.instance.GetCarDataFromObjectAndCarDetails((GameObject)temp1[i].carObject, temp1[i].carDetails);
                        GameManager.instance.menuScript.currentSelectedCarObj = spawnedObject;
                        selectedCarName.text = carCode;
                        break;
                    }
                }
            }
            DatabaseHandler.instance.playerData.selectedCarCode = carCode;
            DatabaseHandler.instance.SavePlayerDataInDB();
            SetStats();
            BackFromSelectingCar();
        }

        void ShowCarVarients(string defCode)
        {
            for (int i = 0; i < carVersionsParent.childCount; i++)
            {
                Destroy(carVersionsParent.GetChild(i).gameObject);
            }

            selectedDefaultCarName.text = defCode;
            for (int i = 0; i < temp1.Count; i++)
            {
                if (temp1[i].carDefaultCode == defCode)
                {
                    GameObject temp2 = Instantiate(carRenderPrefab, carVersionsParent);
                    temp2.GetComponent<Image>().sprite = temp1[i].carRender;
                    temp2.GetComponent<CarRenderClicked>().carDefaultCode = temp1[i].carDefaultCode;
                    temp2.GetComponent<CarRenderClicked>().carRenderType = CarRenderType.RenderWithVariant;
                    temp2.GetComponent<CarRenderClicked>().NFTNumberText.gameObject.SetActive(true);
                    temp2.GetComponent<CarRenderClicked>().NFTNumberText.text = temp1[i].carDetails.code;
                }
            }
        }
        void SetStats()
        {
            float totalTopSpeed = GameManager.instance.currentCarData.engineDetails.engineData.topSpeed + GameManager.instance.currentCarData.compoundsDetails.compoundData.topSpeed + GameManager.instance.currentCarData.transmissionDetails.transmissionData.topSpeed + GameManager.instance.currentCarData.suspensionDetails.suspensionData.topSpeed;

            engineBar.value = (float)(totalTopSpeed / 10);

            nitroBar.value = (float)(5 / 10);

            float totalStability = GameManager.instance.currentCarData.compoundsDetails.compoundData.stability + GameManager.instance.currentCarData.suspensionDetails.suspensionData.stability + GameManager.instance.currentCarData.steeringDetails.steeringData.stability;
            stabilityBar.value = (float)(totalStability / 10);

            float totalBrakeForce = GameManager.instance.currentCarData.brakesDetails.brakeData.deceleration + GameManager.instance.currentCarData.compoundsDetails.compoundData.deceleration + GameManager.instance.currentCarData.wheelsDetails.wheelData.deceleration;
            brakeBar.value = (float)(totalBrakeForce / 10);
        }

        IEnumerator OpenPlayerStatus()
        {
            isStatViewON = true;
            isPanelUp = false;
            isCarViewOn = false;
            SetPlayerStats();
            GameManager.instance.menuScript.closeButtons(firstPanel);
            GameManager.instance.menuScript.closeButtons(scrollGrandParent.gameObject);
            scrollGrandParent.GetComponent<Image>().enabled = false;
            yield return new WaitForSeconds(0.1f);
            firstPanel.SetActive(false);
            playerStatsPanel.SetActive(true);
            yield return new WaitForSeconds(TransitionSpeed);
            GameManager.instance.menuScript.OpenButtons(playerStatsPanel);
        }
        IEnumerator ClosePlayerStats()
        {
            GameManager.instance.menuScript.closeButtons(playerStatsPanel);
            isStatViewON = false;
            yield return new WaitForSeconds(0.1f);
            firstPanel.SetActive(true);
            playerStatsPanel.SetActive(false);
            yield return new WaitForSeconds(TransitionSpeed);
            GameManager.instance.menuScript.OpenButtons(firstPanel);
            GameManager.instance.menuScript.OpenButtons(scrollGrandParent.gameObject);
            scrollGrandParent.GetComponent<Image>().enabled = true;
        }
        void SetPlayerStats()
        {
            if(playerStats.Count > 0)
            {
                for(int i = 0;i < playerStats.Count; i++)
                {
                    Destroy(playerStats[i].gameObject);
                }
                playerStats.Clear();
            }
            for(int i = 0; i < DatabaseHandler.instance.playerStats.raceData.Count; i++)
            {
                GameObject spawn = Instantiate(statsCell, statPlacer.transform);
                var temp = DatabaseHandler.instance.playerStats.raceData[i];
                spawn.GetComponent<StatCell>().SetStateData(temp.position.ToString(), temp.gameMode, temp.carUsed, temp.totalRacers.ToString(), temp.mapName, temp.dateTime);
                playerStats.Add(spawn);
            }
        }
        #region GrandParent Panel Animations
        void MoveGrandparentUp()
        {
            StartCoroutine(MoveUp());
        }

        void BackFromSelectingCar()
        {
            StartCoroutine(MoveDown());
        }

        IEnumerator MoveUp()
        {
            
            scrollGrandParent.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, grandParentTopLocalPos), TransitionSpeed);
            firstPanel.SetActive(false);
            secondPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(970f, 531f), TransitionSpeed);
            isPanelUp = true;
            yield return new WaitForSeconds(TransitionSpeed);
            //StopCoroutine(MoveUp());
        }
        IEnumerator MoveDown()
        {
            scrollGrandParent.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, grandParentBottomLocalPos), TransitionSpeed);
            secondPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(970f, -189f), TransitionSpeed);
            yield return new WaitForSeconds(TransitionSpeed);
            firstPanel.SetActive(true);
            isPanelUp = false;
            //StopCoroutine(MoveDown());
        }
        #endregion

        #region Car viewar
        public void OpenCarviewer()
        {
            scrollGrandParent.gameObject.SetActive(false);
            firstPanel.SetActive(false);
            secondPanel.SetActive(false);
            isCarViewOn = true;
        }

        void BackFromCarViewer()
        {
            scrollGrandParent.gameObject.SetActive(true);
            firstPanel.SetActive(true);
            secondPanel.SetActive(true);
            isCarViewOn = false;
            CarHolder.transform.DORotate(new Vector3(0f, 230f, 0f), 0.15f);
            //CarHolder.transform.rotation = Quaternion.Euler(0f, 230f, 0f);
        }
        #endregion
 

    }
}