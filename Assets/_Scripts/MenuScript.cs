using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.Audio;

namespace MGR
{   

    public class MenuScript : MonoBehaviourPunCallbacks
    {
        [SerializeField] int currentSelectedCar = 0;

        [Header("Panels")]
        [SerializeField] GameObject[] selectCarPanels;
        [SerializeField] GameObject[] garagePanels;
        [SerializeField] GameObject ModeSelectPanel;
        [SerializeField] GameObject DifficultySelectPanel;
        [SerializeField] GameObject MapSelectPanel;
        [SerializeField] float panelTransitionTime;
        public float PanelTransitionTime { get { return panelTransitionTime; } }   

        [SerializeField] GameObject SelectCar3D;
        [SerializeField] GameObject[] mapsDisplay;

        public Transform menuCarPosition;
        public List<MenuCarScript> menuCars = new List<MenuCarScript>();

        public GameObject currentSelectedCarObj;

        Vector3 res = new Vector3(1920, 0, 0);

        [Header("Map Panel Objects")]
        [SerializeField] TextMeshProUGUI modeTextInMapPanel;
        [SerializeField] RectTransform contentView;
        [SerializeField] GameObject mapDataPrefab;

        [Header("Confirmation Panel Objs")]
        [SerializeField] GameObject confirmationPanel;
        [SerializeField] Image cpMapThumbnail;
        [SerializeField] TextMeshProUGUI cpCountryNameText;
        [SerializeField] TextMeshProUGUI cpMapNameText;
        [SerializeField] TextMeshProUGUI cpTrackTypeText;
        [SerializeField] TextMeshProUGUI cpTrackRouteText;
        [SerializeField] TextMeshProUGUI cpTotalLengthText;
        [SerializeField] TextMeshProUGUI cpWeatherConditionText;
        [SerializeField] TextMeshProUGUI cpCarName;
        [SerializeField] TextMeshProUGUI cpCarNftNumber;
        [SerializeField] Image cpCarThumbnail;

        [Header("Camera Components")]
        [SerializeField] Transform cameraTarget1;
        [SerializeField] Transform cameraTarget2;
        [SerializeField] Transform cameraTarget3;
        [SerializeField] Transform cameraTarget4;
        [SerializeField] Transform cameraTarget5;
        [SerializeField] Transform cameraTarget6;
        [SerializeField] float CameraSpeed = 10.0f;
        [SerializeField] Vector3 dist;
        [SerializeField] Transform lookTarget;
        private int currentTarget;
        private Transform cameraTarget;
        Camera CameraTrans;
        bool changeCamera = true;
        [SerializeField] float fadeTime;
        [Header("Options Menu")]
        [SerializeField] GameObject OptionsPanel;
        AudioMixer masterAudioMixer;
        [SerializeField] Slider masterVolumeSlider;
        [SerializeField] Slider musicVolumeSlider;
        [SerializeField] Slider sfxVolumelSlider;


        public GameObject[] objs;
        int index = 0;

        [SerializeField] private TMP_Text playerTagDisplayText;

        public void HideGame()
        {
            foreach (var item in objs)
            {
                item.SetActive(true);
            }
            objs[index].SetActive(false);
            index++;
            if (index >= objs.Length) index = 0;
        }


        private void Start()
        {
            if (GameManager.instance)
            {
                GameManager.instance.menuScript = this;
            }
            else
            {
                Invoke(nameof(Start), 0.01f);
            }
            currentTarget = 1;
            SetCameraTarget(currentTarget);
            CameraTrans = Camera.main;
            closeButtons(ModeSelectPanel);
            closeButtons(DifficultySelectPanel);
            closeButtons(OptionsPanel);
            masterAudioMixer = AudioManager.instance.MasterAudioMixer;
            masterVolumeSlider.value = PlayerPrefs.GetFloat(AudioManager.MASTER_KEY, 1f);
            musicVolumeSlider.value = PlayerPrefs.GetFloat(AudioManager.MUSIC_KEY, 1f);
            sfxVolumelSlider.value = PlayerPrefs.GetFloat(AudioManager.SFX_KEY, 1f);
            if (PlayfabManager.instance.playerTag == null)
                Invoke(nameof(setPlayerTag), 4f);
            else
                setPlayerTag();

        }

        private void FixedUpdate()
        {
            if(changeCamera)
            {
                Vector3 dpos = cameraTarget.position + dist;
                Vector3 spos = Vector3.Lerp(CameraTrans.transform.position, dpos, CameraSpeed * Time.deltaTime);
                CameraTrans.transform.position = spos;
                CameraTrans.transform.LookAt(lookTarget.position);
                if(Vector3.Distance(CameraTrans.transform.position, dpos) < 0.1f)
                {
                    changeCamera = false;
                }
            }
        }

        public void DailyModeStart(bool canStartDRM)
        {
            Master.ColoredLog("yellow", "CAn start DRM: " + canStartDRM);
            if (canStartDRM)
            {
                ActuallyStartGame();
            }
            else
            {
                InevitableUI.instance.ShowPopUp("Daily Mode Limit Reached for Day", 2);
            }
        }

        void SetMapDataInMapPanel()
        {
            for (int i = 0; i < contentView.transform.childCount; i++)
            {
                Destroy(contentView.transform.GetChild(i).gameObject);
            }
            modeTextInMapPanel.text = GameManager.instance.gameMode.ToString();
            contentView.sizeDelta = new Vector2(contentView.sizeDelta.x, (DatabaseHandler.instance.mapDataDetailed.Count * 270) - 5);
            for (int i = 0; i < DatabaseHandler.instance.mapDataDetailed.Count; i++)
            {
                GameObject temp = Instantiate(mapDataPrefab, contentView.transform);
                temp.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, i * -270);
                temp.GetComponent<MapDataScript>().SetMapdata(DatabaseHandler.instance.mapDataDetailed[i]);
            }
        }

        public void SetCameraTarget(int target)
        {
            switch (target)
            {
                case 1:
                    cameraTarget = cameraTarget1.transform;
                    break;
                case 2:
                    cameraTarget = cameraTarget2.transform;
                    break;
                case 3:
                    cameraTarget = cameraTarget3.transform;
                    break;
                case 4:
                    cameraTarget = cameraTarget4.transform;
                    break;
                case 5:
                    cameraTarget = cameraTarget5.transform;
                    break;
                case 6:
                    cameraTarget = cameraTarget6.transform;
                    break ;
            }
            changeCamera = true;
        }

        #region Switch Panels

        IEnumerator Opencar()
        {
            
            closeButtons(ModeSelectPanel);
            closeButtons(OptionsPanel);
            selectCarPanels[0].SetActive(true);
            yield return new WaitForSeconds(0.1f);
            SetCameraTarget(1);
            //selectCarPanels[0].GetComponent<RectTransform>().DOAnchorPos(Vector3.zero, panelTransitionTime);
            //ModeSelectPanel.GetComponent<RectTransform>().DOAnchorPos(res, panelTransitionTime);
            yield return new WaitForSeconds(panelTransitionTime);
            ModeSelectPanel.SetActive(false);
            OptionsPanel.SetActive(false);
            closeButtons(ModeSelectPanel);
            OpenButtons(selectCarPanels[0]);

        }
        IEnumerator OpenMode()
        {
            ModeSelectPanel.SetActive(true);
            
            closeButtons(selectCarPanels[0]);
            closeButtons(DifficultySelectPanel);

            yield return new WaitForSeconds(0.1f);
            SetCameraTarget(2);
            //selectCarPanels[0].GetComponent<RectTransform>().DOAnchorPos(-res, panelTransitionTime);
            //ModeSelectPanel.GetComponent<RectTransform>().DOAnchorPos(Vector3.zero, panelTransitionTime);
            MapSelectPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector3(0, -1080f, 0), panelTransitionTime);

            yield return new WaitForSeconds(panelTransitionTime);

            OpenButtons(ModeSelectPanel);
            selectCarPanels[0].SetActive(false);
            MapSelectPanel.SetActive(false);
            DifficultySelectPanel.SetActive(false);

        }

        IEnumerator OpenDifficulty()
        {
            closeButtons(ModeSelectPanel);
            DifficultySelectPanel.SetActive(true);
            SetMapDataInMapPanel();
            yield return new WaitForSeconds(0.1f);
            SetCameraTarget(5);
            yield return new WaitForSeconds(panelTransitionTime);
            ModeSelectPanel.SetActive(false);
            OpenButtons(DifficultySelectPanel);
        }

        IEnumerator OpenMap()
        {
            MapSelectPanel.SetActive(true);
            closeButtons(ModeSelectPanel);
            SetMapDataInMapPanel();
            yield return new WaitForSeconds(0.1f);
            SetCameraTarget(3);
            MapSelectPanel.GetComponent<RectTransform>().DOAnchorPos(Vector3.zero, panelTransitionTime);

            yield return new WaitForSeconds(panelTransitionTime);
            ModeSelectPanel.SetActive(false);
            DifficultySelectPanel.SetActive(false);
            confirmationPanel.SetActive(false);
        }
        IEnumerator OpenSettings()
        {
            OptionsPanel.SetActive(true);
            closeButtons(selectCarPanels[0]);
            yield return new WaitForSeconds(0.1f);
            yield return new WaitForSeconds(panelTransitionTime);
            selectCarPanels[0].SetActive(false);
            OpenButtons(OptionsPanel);
        }

        public void OpenModeSelector()
        {
            StartCoroutine(OpenMode());
            RandomizeMap(false);

        }
        public void OpenCarSelector()
        {
            StartCoroutine(Opencar());
        }

        public void OpenDifficultySelector()
        {
            StartCoroutine(OpenDifficulty());
        }

        public void OpenMapSelector()
        {
            StartCoroutine(OpenMap());
        }

        public void OnOpenSettings()
        {
            StartCoroutine(OpenSettings());
        }

        public void closeButtons(GameObject Panel)
        {
            for (int i = 0; i < Panel.transform.childCount; i++)
            {
                Panel.transform.GetChild(i).transform.localScale = Vector3.one;
            }
            for (int i = 0; i < Panel.transform.childCount; i++)
            {
                if(Panel.transform.GetChild(i).GetComponent<Image>() == null)
                {
                    closeButtons(Panel.transform.GetChild(i).gameObject);
                }
                else
                {
                    Panel.transform.GetChild(i).transform.DOScale(0f, fadeTime);
                }
            }
        }
        public void OpenButtons(GameObject Panel)
        {
            for (int i = 0; i < Panel.transform.childCount; i++)
            {
                if(Panel.transform.GetChild(i).GetComponent<Image>() == null)
                {
                    Panel.transform.GetChild(i).transform.localScale = Vector3.one;
                }
                else
                {
                    Panel.transform.GetChild(i).transform.localScale = Vector3.zero;
                }
            }
            for (int i = 0; i < Panel.transform.childCount; i++)
            {
                if (Panel.transform.GetChild(i).GetComponent<Image>() == null)
                {
                    OpenButtons(Panel.transform.GetChild(i).gameObject);
                }
                else
                {
                    Panel.transform.GetChild(i).transform.DOScale(1f, fadeTime);
                }
            }
        }

        #endregion

        #region Extra Ghacha Macha

        public void setPlayerTag()
        {
            playerTagDisplayText.text = PlayfabManager.instance.playerTag;
        }
        public void setMap(int index)
        {
            GameManager.instance.map = (Maps)index;
            GameManager.instance.onRandomMap = false;
        }
        public void tempMapSelectorOpen()
        {
            SetCameraTarget(3);
            confirmationPanel.SetActive(false);
            MapSelectPanel.SetActive(true);
        }
        public void SetMapAndStartGame(MapDataDetailed selectedDetailedMapData)
        {
            Debug.Log("Select map and start");
            GameManager.instance.selectedMapData = selectedDetailedMapData;
            GameManager.instance.onRandomMap = false;
            SetConfirmationPanel();
            SetCameraTarget(6);
            confirmationPanel.SetActive(true);
            MapSelectPanel.SetActive(false);
        }

        void SetConfirmationPanel()
        {
            cpMapThumbnail.sprite = GameManager.instance.selectedMapData.render;
            cpCountryNameText.text = GameManager.instance.selectedMapData.mapData.countryName;
            cpMapNameText.text = GameManager.instance.selectedMapData.mapData.mapName;
            cpTrackTypeText.text = GameManager.instance.selectedMapData.mapData.trackType;
            cpTrackRouteText.text = GameManager.instance.selectedMapData.mapData.trackRoute;
            cpTotalLengthText.text = GameManager.instance.selectedMapData.mapData.totalLength;
            cpWeatherConditionText.text = GameManager.instance.selectedMapData.mapData.weatherCondition;
            cpCarName.text = GameManager.instance.currentCarData.carCode;
            cpCarNftNumber.text = GameManager.instance.currentCarData.carCode;
            cpCarThumbnail.sprite = GameManager.instance.currentCarData.carRender;
        }

        public void ConfirmAndStartGame()
        {
            PlayGame(false);
        }        
        public void RandomizeMap(bool set)
        {
            GameManager.instance.onRandomMap = set;
            if(GameManager.instance.onRandomMap)
            {
                int temp = UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(Maps)).Length);
                GameManager.instance.map = (Maps)temp;
                for (int x = 0; x < mapsDisplay.Length; x++)
                {
                    if (temp == x)
                    {
                        mapsDisplay[x].SetActive(true);
                    }
                    else
                    {
                        mapsDisplay[x].SetActive(false);
                    }
                }
            }
        }


        public void SetGameMode(int index)
        {
            GameManager.instance.gameMode = (GameMode)index;
        }

        public void SetDifficultyMode(int index)
        {
            GameManager.instance.difficultyMode = (DifficultyMode)index;
        }

        public void PlayGame(bool isQuickPlay)
        {
            Debug.Log("Play game " + GameManager.instance.gameMode);
            NetworkingScript.instance.isQuickie = isQuickPlay;
            if (GameManager.instance.gameMode == GameMode.DailyRace && GameManager.instance.currentCarData.carType != CarType.Common)
            {
                PlayfabManager.instance.checkDateTimeType = CheckDateTimeType.BeforeStartingDRM;
                PlayfabManager.instance.CheckDateTime();
            }
            else
            {
                ActuallyStartGame();
            }
        }

        void ActuallyStartGame()
        {
            Debug.Log("SHow loading screen");
            InevitableUI.instance.ShowLoadingScreen();
            NetworkingScript.instance.CreateOrJoinRoom();
        }

        #endregion

        
        private void OnDisable()
        {
            PlayerPrefs.SetFloat(AudioManager.MASTER_KEY, masterVolumeSlider.value);
            PlayerPrefs.SetFloat(AudioManager.MUSIC_KEY, musicVolumeSlider.value);
            PlayerPrefs.SetFloat(AudioManager.SFX_KEY, sfxVolumelSlider.value);
        }

        #region Options Menu

        public void setMasterVolume(float value)
        {
            masterAudioMixer.SetFloat(AudioManager.Mixer_Master, MathF.Log10(value) * 20);
        }
        public void setMusicVolume(float value)
        {
            masterAudioMixer.SetFloat(AudioManager.Mixer_Music, MathF.Log10(value) * 20);
        }
        public void setSFXVolume(float value)
        {
            masterAudioMixer.SetFloat(AudioManager.Mixer_SFX, MathF.Log10(value) * 20);
        }
        #endregion

    }
}