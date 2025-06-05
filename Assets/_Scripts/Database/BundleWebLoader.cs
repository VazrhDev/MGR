using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using Newtonsoft.Json;
using MasterDataNS;
using UnityEngine.SceneManagement;

namespace MGR
{
    public class BundleWebLoader : MonoBehaviour
    {
        public static BundleWebLoader instance;
        [SerializeField] bool ClearCacheOnstart = false;

        public List<string> urlList;
        //public string url;

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

        // Start is called before the first frame update
        void Start()
        {
            //StartCoroutine(GetAssetBundle());
            if(ClearCacheOnstart)
            {
                Caching.ClearCache();
            }
        }

        [ContextMenu("GetAssetBundle")]
        public void CallGetAssetBundle()
        {
            StartCoroutine(DownloadAndCache());
        }

        IEnumerator DownloadAndCache()
        {
            // Wait for the Caching system to be ready
            while (!Caching.ready)
            {
                yield return null;
            }
            InevitableUI.instance.LogOnLoadingScreen("Loading Cars and Maps.");
            int i = 0;
            float startProgressValue = InevitableUI.instance.loadingProgressBar.fillAmount * 100;
            if (PlayfabManager.instance.isDevModeOn && PlayfabManager.instance.ignoreAllBundles)
            {
                InevitableUI.instance.ProgressOnLoadingScreen(100);
                LoadDefaultCarAndMap();
                DatabaseHandler.instance.SavePlayerDataInDB();
                GameManager.instance.SpawnCArsTemp();
                //GameManager.instance.NFTCheck();
            }
            else
            {
                //if you want to always load from server, can clear cache first
                //Caching.CleanCache();
                string bundleURL = "";
                for (int j = 0; j < urlList.Count; j++)
                //foreach (var currentUrl in urlList)
                {
                    bundleURL = urlList[j];
                    // get current bundle hash from server, random value added to avoid caching
                    UnityWebRequest www = UnityWebRequest.Get(bundleURL + ".manifest?r=" + (UnityEngine.Random.value * 9999999));
                    Debug.Log("Loading manifest:" + bundleURL + ".manifest");

                    // wait for load to finish
                    yield return www.Send();

                    // if received error, exit
                    if (www.isNetworkError == true)
                    {
                        Debug.LogError("www error: " + www.error);
                        www.Dispose();
                        www = null;
                        yield break;
                    }

                    // create empty hash string
                    Hash128 hashString = (default(Hash128));// new Hash128(0, 0, 0, 0);

                    Debug.Log("Download Handler: " + www.downloadHandler.isDone.ToString());

                    // check if received data contains 'ManifestFileVersion'
                    if (www.downloadHandler.text.Contains("ManifestFileVersion"))
                    {
                        // extract hash string from the received data, TODO should add some error checking here
                        var hashRow = www.downloadHandler.text.ToString().Split("\n".ToCharArray())[5];
                        hashString = Hash128.Parse(hashRow.Split(':')[1].Trim());

                        if (hashString.isValid == true)
                        {
                            // we can check if there is cached version or not
                            if (Caching.IsVersionCached(bundleURL, hashString) == true)
                            {
                                Debug.Log("Bundle with this hash is already cached!" + bundleURL);
                            }
                            else
                            {
                                Debug.Log("No cached version founded for this hash.." + bundleURL);
                            }
                        }
                        else
                        {
                            // invalid loaded hash, just try loading latest bundle
                            Debug.LogError("Invalid hash:" + hashString);
                            yield break;
                        }

                    }
                    else
                    {
                        Debug.LogError("Manifest doesn't contain string 'ManifestFileVersion': " + bundleURL + ".manifest");
                        yield break;
                    }

                    // now download the actual bundle, with hashString parameter it uses cached version if available
                    www = UnityWebRequestAssetBundle.GetAssetBundle(bundleURL + "?r=" + (UnityEngine.Random.value * 9999999), hashString, 0);

                    // wait for load to finish
                    yield return www.SendWebRequest();

                    if (www.error != null)
                    {
                        Debug.LogError("www error: " + www.error);
                        www.Dispose();
                        www = null;
                        yield break;
                    }

                    // get bundle from downloadhandler
                    AssetBundle bundle = ((DownloadHandlerAssetBundle)www.downloadHandler).assetBundle;

                    GameObject currentBundle = null;
                    if(bundle.GetAllAssetNames().Length <= 0)
                    {
                        GameManager.instance.MapSceneName = bundle.name;
                        Debug.LogError(bundle.GetAllScenePaths());
                        Debug.LogError("map name added: " + bundle.name);
                        if (j == urlList.Count - 1)
                        {
                            //AllBundlesLoaded();
                            GameManager.instance.NFTCheck();
                        }
                        continue;
                    }
                    currentBundle = (GameObject)bundle.LoadAsset(bundle.GetAllAssetNames()[0]);

                    float progressTemp = ((float)(j + 1) / urlList.Count) * 100;
                    float progressTemp1 = (progressTemp / 100) * 80;
                    InevitableUI.instance.ProgressOnLoadingScreen(startProgressValue + progressTemp1);

                    // if we got something out
                    if (currentBundle != null)
                    {
#if UNITY_EDITOR
                        // Temperory Car Material Fix
                        FixMaterials((GameObject)currentBundle);
#endif

                        //Debug.Log("<color=yellow>" + currentBundle.name + "</color>");
                        Master.ColoredLog("yellow", currentBundle.name);
                        InevitableUI.instance.LogOnLoadingScreen(currentBundle.name + " loaded");

                        // Tracks Fetching
                        //if (currentBundle.name.Contains("Map") && !currentBundle.name.Contains("Renders"))
                        //{
                        //    Master.ColoredLog("blue", "MAP BUNDLE");
                        //    if (!DatabaseHandler.instance.mapsList.Contains(currentBundle))
                        //    {
                        //        DatabaseHandler.instance.mapsList.Add(currentBundle);
                        //    }

                        //    if (j == urlList.Count - 1)
                        //    {
                        //        //AllBundlesLoaded();
                        //        GameManager.instance.NFTCheck();
                        //    }
                        //    continue;
                        //}
                        if (string.Compare(currentBundle.name, "carrenders", true) == 0)
                        {
                            DatabaseHandler.instance.carRenders = currentBundle;
                            if (j == urlList.Count - 1)
                            {
                                //AllBundlesLoaded();
                                GameManager.instance.NFTCheck();
                            }
                            //if (DatabaseHandler.instance.mapsList == null || DatabaseHandler.instance.mapsList.Count<=0)
                            //{
                            //    for (int k = 0; k < PlayfabManager.instance.maps.Length; k++)
                            //    {
                            //        DatabaseHandler.instance.mapsList.Add(PlayfabManager.instance.maps[k]);
                            //    }
                            //}
                            continue;
                        }
                        else if (string.Compare(currentBundle.name, "nftrenders", true) == 0)
                        {
                            DatabaseHandler.instance.nFTRenders = currentBundle;
                            if (j == urlList.Count - 1)
                            {
                                //AllBundlesLoaded();
                                GameManager.instance.NFTCheck();
                            }
                            continue;
                        }
                        else if (string.Compare(currentBundle.name, "wheelrenders", true) == 0)
                        {
                            DatabaseHandler.instance.wheelrenders = currentBundle;
                            if (j == urlList.Count - 1)
                            {
                                //AllBundlesLoaded();
                                GameManager.instance.NFTCheck();
                            }
                            continue;
                        }
                        else if (string.Compare(currentBundle.name, "decals", true) == 0)
                        {
                            DatabaseHandler.instance.decals = currentBundle;
                            if (j == urlList.Count - 1)
                            {
                                //AllBundlesLoaded();
                                GameManager.instance.NFTCheck();
                            }
                            continue;
                        }
                        else if (string.Compare(currentBundle.name, "maprenders", true) == 0)
                        {
                            DatabaseHandler.instance.mapRenders = currentBundle;
                            if (j == urlList.Count - 1)
                            {
                                //AllBundlesLoaded();
                                GameManager.instance.NFTCheck();
                            }
                            continue;
                        }

                        // Cars Fetching
                        DatabaseHandler.instance.carsList.Add(currentBundle);
                        Master.ColoredLog("green", "Added New Car");
                    }

                    www.Dispose();
                    www = null;

                    // try to cleanup memory
                    Resources.UnloadUnusedAssets();
                    bundle.Unload(false);
                    bundle = null;
                    if (j == urlList.Count - 1)
                    {
                        //AllBundlesLoaded();
                        GameManager.instance.NFTCheck();
                    }
                }

            }
        }

        void LoadDefaultCarAndMap()
        {
            //Map
            InevitableUI.instance.LogOnLoadingScreen("Map loaded");
            if (DatabaseHandler.instance.mapsList == null || DatabaseHandler.instance.mapsList.Count <= 0)
            {
                for (int k = 0; k < PlayfabManager.instance.maps.Length; k++)
                {
                    DatabaseHandler.instance.mapsList.Add(PlayfabManager.instance.maps[k]);
                }
            }


            DatabaseHandler.instance.carRenders = PlayfabManager.instance.carRenders;
            DatabaseHandler.instance.nFTRenders = PlayfabManager.instance.nFTRenders;
            DatabaseHandler.instance.wheelrenders = PlayfabManager.instance.wheelRenders;
            DatabaseHandler.instance.decals = PlayfabManager.instance.decals;
            DatabaseHandler.instance.mapRenders = PlayfabManager.instance.mapRenders;

            //DatabaseHandler.instance.carsList.Add(PlayfabManager.instance.defaultCar);

            //InevitableUI.instance.LogOnLoadingScreen(PlayfabManager.instance.defaultCar.name + " loaded");
            //Car carTemp = new Car("nitro01", "engine01", "steering01", "wheel01", "brake01", "frame01", "suspension01", "compound01",
            //    "transmission01", "Black", CarType.Common);
            //CarDetails carDetailsTemp = new CarDetails("dummy", 0, carTemp);

            //CarDetailedData temp = new CarDetailedData();
            //temp.carDetails = carDetailsTemp;
            //temp.carObject = PlayfabManager.instance.defaultCar;
            //temp.carDefaultCode = PlayfabManager.instance.defaultCar.name;
            //DatabaseHandler.instance.playerDetailedData.carsOwned.Add(temp);

            for(int i = 0; i < PlayfabManager.instance.cars.Length; i++)
            {
                AddCar(PlayfabManager.instance.cars[i], "26,255,0,255");
            }
            //AddCar(PlayfabManager.instance.defaultCar,"0,186,255,255");
            //AddCar(PlayfabManager.instance.car1, "26,255,0,255");
            //AddCar(PlayfabManager.instance.car2, "255,0,0,255");
        }

        void AddCar(GameObject car,string color)
        {
            DatabaseHandler.instance.carsList.Add(car);

            InevitableUI.instance.LogOnLoadingScreen(car.name + " loaded");
            Car carTemp = new Car("nitro02", "engine02", "steering02", "wheel02", "brake02", "frame02", "suspension02", "compound02",
                "transmission02", color, CarType.Common);
            CarDetails carDetailsTemp = new CarDetails(car.name, 0, carTemp);

            CarDetailedData temp = new CarDetailedData();
            temp.carDetails = carDetailsTemp;
            temp.carObject = car;
            temp.carDefaultCode = car.name;
            for (int i = 0; i < PlayfabManager.instance.carRenders.transform.childCount; i++)
            {
                if(car.name.ToLower()== PlayfabManager.instance.carRenders.transform.GetChild(i).name.ToLower())
                {
                    temp.carRender = PlayfabManager.instance.carRenders.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite;
                }
            }
            DatabaseHandler.instance.playerDetailedData.carsOwned.Add(temp);
        }

        void FixMaterials(GameObject spawnObject)
        {
            // Temporary Material Fix
            for (int index = 0; index < spawnObject.transform.childCount; index++)
            {
                if (spawnObject.transform.GetChild(index).name == "Weather")
                {
                    spawnObject.transform.GetChild(index).gameObject.SetActive(false);
                }

                spawnObject.transform.GetChild(index).transform.TryGetComponent<Renderer>(out Renderer renderer);

                if (renderer)
                {
                    Master.ColoredLog("blue", "TRY CHANGING MATERIAL");
                    //renderer.material.shader = Shader.Find("Universal Render Pipeline/Lit");
                    foreach (Material mat in renderer.sharedMaterials)
                    {
                        //childRenderer.material.shader = Shader.Find("Universal Render Pipeline/Lit");
                        mat.shader = Shader.Find("Universal Render Pipeline/Lit");
                    }
                }
                else
                {
                    GameObject childObject = spawnObject.transform.GetChild(index).gameObject;
                    Master.ColoredLog("Red", "Child Oject: " + childObject.name);

                    for (int index1 = 0; index1 < childObject.transform.childCount; index1++)
                    {
                        childObject.transform.GetChild(index1).transform.TryGetComponent<Renderer>(out Renderer childRenderer);

                        if (childRenderer)
                        {
                            foreach (Material mat in childRenderer.sharedMaterials)
                            {
                                //childRenderer.material.shader = Shader.Find("Universal Render Pipeline/Lit");
                                mat.shader = Shader.Find("Universal Render Pipeline/Lit");
                            }
                        }
                    }
                }
            }
        }

        //void AllBundlesLoaded()
        //{
        //    DatabaseHandler.instance.SetDetailedData();
        //    GameManager.instance.HideLoadingScreen();
        //}

        public void CallGetMasterData(string _masterDataUrl)
        {
            StartCoroutine(GetMasterData(_masterDataUrl));

            DatabaseHandler.instance.TooBject2();
        }

        IEnumerator GetMasterData(string _masterDataUrl)
        {
            //UnityWebRequest uwr = UnityWebRequest.Get(_masterDataUrl);

            //yield return uwr.SendWebRequest();

            //if(uwr.result != UnityWebRequest.Result.Success)
            //{
            //    Debug.Log("Master Data Request Failed");
            //    Debug.Log(uwr.error);
            //}
            //else
            //{
            //    //string path = Path.Combine(Application.persistentDataPath, "unity3d.html");
            //    //uwr.downloadHandler. = new DownloadHandlerFile(path);

            //    Master.ColoredLog("yellow", "uwr: " + uwr.downloadHandler.data);
            //}

            using (UnityWebRequest uwr = UnityWebRequest.Get(_masterDataUrl))
            {
                uwr.useHttpContinue = false;
                Debug.Log("URL: " + _masterDataUrl);
                uwr.SetRequestHeader("Access-Control-Allow-Origin", "*");

                uwr.downloadHandler = new DownloadHandlerBuffer();

                yield return uwr.SendWebRequest();

                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    
                    Debug.Log("Master Data Request Failed");
                    Debug.Log(uwr.error);
                    Debug.Log(uwr.isHttpError);
                    Debug.Log(uwr.isNetworkError);
                    DatabaseHandler.instance.TooBject2();
                    List<MapData> mapData = new List<MapData>();
                    foreach (var item in DatabaseHandler.instance.masterData.mapData)
                    {
                        mapData.Add(item);
                    }
                    //MapData temp = new MapData("Lasvegas_track", mapData[1].countryName, mapData[1].mapName, mapData[1].trackType,
                    //    mapData[1].trackRoute, mapData[1].totalLength, mapData[1].weatherCondition);
                    //mapData.Add(temp);
                    //DatabaseHandler.instance.masterData.mapData = mapData.ToArray();
                    if (MasterDataNS.MasterDataUploader.instance.loadFromDatabase)
                    {
                        MasterDataNS.MasterDataUploader.instance.masterData = DatabaseHandler.instance.masterData;
                    }
                }
                else
                {
                    string data = uwr.downloadHandler.text;

                    //data = data.Replace(data[0], ' ');
                    //data = data.Replace(data[data.Length - 1], ' ');

                    Master.ColoredLog("Yellow", data);
                    //GameManager.instance.masterData = JsonUtility.FromJson<MasterDataNS.MasterData>(data);

                    DatabaseHandler.instance.masterData = JsonConvert.DeserializeObject<MasterDataNS.MasterData>(data);
                    List<MapData> mapData = new List<MapData>();
                    foreach (var item in DatabaseHandler.instance.masterData.mapData)
                    {
                        mapData.Add(item);
                    }
                    MapData temp = new MapData("Lasvegas_track", mapData[1].countryName, mapData[1].mapName, mapData[1].trackType,
                        mapData[1].trackRoute, mapData[1].totalLength, mapData[1].weatherCondition);
                    mapData.Add(temp);
                    DatabaseHandler.instance.masterData.mapData = mapData.ToArray();
                    if (MasterDataNS.MasterDataUploader.instance.loadFromDatabase)
                    {
                        MasterDataNS.MasterDataUploader.instance.masterData = DatabaseHandler.instance.masterData;
                    }
                }
            }
        }
    }
}