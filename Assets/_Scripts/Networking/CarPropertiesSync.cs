using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using MasterDataNS;


namespace MGR
{
    public class CarPropertiesSync : MonoBehaviourPun
    {

        [SerializeField] VehicleControl movementController;
        [SerializeField] GameObject[] cars;
        public CarDetails selectedCarDetails;

        [SerializeField]
        CarData carData;

        private void Start()
        {
            if (photonView.IsMine)
            {
                InitializeStuff();
            }
        }

        public void SetCar()
        {
            Master.ColoredLog("yellow", "Three Disabled");
            //float nitroMax = 0, steeringMax = 0;
            //for (int i = 0; i < GameManager.instance.partsData.engineInfo.Length; i++)
            //{
            //    if (selectedCarDetails.engineCode == GameManager.instance.partsData.engineInfo[i].code)
            //    {
            //        movementController.enginePower = GameManager.instance.partsData.engineInfo[i].curve;
            //    }
            //}
            //for (int i = 0; i < GameManager.instance.partsData.nitroInfo.Length; i++)
            //{
            //    if (selectedCarDetails.nitroCode == GameManager.instance.partsData.nitroInfo[i].code)
            //    {
            //        nitroMax = GameManager.instance.partsData.nitroInfo[i].maxValue;
            //    }
            //}
            //for (int i = 0; i < GameManager.instance.partsData.steeringInfo.Length; i++)
            //{
            //    if (selectedCarDetails.steeringCode == GameManager.instance.partsData.steeringInfo[i].code)
            //    {
            //        steeringMax = GameManager.instance.partsData.steeringInfo[i].maxValue;
            //    }
            //}
            //movementController.NitroForce = nitroMax;
            //movementController.steering = steeringMax;
        }

        void InitializeStuff()
        {
            //cars[GameManager.instance.currentSelectedCar].SetActive(true);
            for (int i = 0; i < DatabaseHandler.instance.playerData.carsOwned.Count; i++)
            {
                if (DatabaseHandler.instance.playerData.carsOwned[i].code == GameManager.instance.currentSelectedCar)
                {
                    Car temp = new Car(DatabaseHandler.instance.playerData.carsOwned[i].car.nitroCode, DatabaseHandler.instance.playerData.carsOwned[i].car.engineCode,
                        DatabaseHandler.instance.playerData.carsOwned[i].car.steeringCode, DatabaseHandler.instance.playerData.carsOwned[i].car.wheelsCode,
                        DatabaseHandler.instance.playerData.carsOwned[i].car.brakeCode, DatabaseHandler.instance.playerData.carsOwned[i].car.frameCode,
                        DatabaseHandler.instance.playerData.carsOwned[i].car.suspensionCode, DatabaseHandler.instance.playerData.carsOwned[i].car.compoundCode,
                        DatabaseHandler.instance.playerData.carsOwned[i].car.transmissionCode, DatabaseHandler.instance.playerData.carsOwned[i].car.colorCode,
                        DatabaseHandler.instance.playerData.carsOwned[i].car.carType);
                    selectedCarDetails = new CarDetails(DatabaseHandler.instance.playerData.carsOwned[i].code, DatabaseHandler.instance.playerData.carsOwned[i].timesUsed,
                        temp);
                    break;
                }
            }
            Master.ColoredLog("yellow", "Three Disabled");
            //string playerCardData = GameManager.instance.currentSelectedCar.ToString() + ",";
            //playerCardData += selectedCarDetails.nitroCode + "," +
            //    selectedCarDetails.engineCode + "," +
            //    selectedCarDetails.steeringCode;
            //photonView.RPC("InitializeRpc", RpcTarget.All, playerCardData);
            SetCar();
        }

        [PunRPC]
        public void InitializeRpc(string carData)
        {
            if (!photonView.IsMine)
            {
                Master.ColoredLog("yellow", "Two Disabled");
                //Debug.LogError("Initialize RPC");
                //string[] temp = carData.Split(',');
                ////cars[int.Parse(temp[0])].SetActive(true);
                //selectedCarDetails = new CarDetails();
                //selectedCarDetails.code = temp[1];
                //selectedCarDetails.nitroCode = temp[2];
                //selectedCarDetails.engineCode = temp[3];
                //SetCar();
            }
        }


        public void CallSpawnCarModel()
        {
            //object[] carDataObject = new object[] { JsonUtility.ToJson(GameManager.instance.currentCarData) };
            object[] carDataObject = new object[] { GameManager.instance.currentCarData.carModel.name };

            gameObject.GetPhotonView().RPC(nameof(SpawnCarModel), RpcTarget.All, carDataObject);

        }

        [PunRPC]
        void SpawnCarModel(object _carObject)
        {
            //carData = JsonUtility.FromJson<CarData>((string)_carObject);
            string carName = (string)_carObject;

            Debug.Log(carData.color);

            Debug.Log("RPC CAR NAME: " + carData.carModel);

            GameObject selectedCar = null;

            foreach(GameObject car in DatabaseHandler.instance.carsList)
            {
                if(car.name == carName)
                {
                    selectedCar = car;
                }
            }

            if (selectedCar == null)
                return;

            GameObject carModel = Instantiate(selectedCar);
            carModel.transform.SetParent(gameObject.GetComponent<VehicleControl>().CarModelHolder.transform);
            //if (carModel.name.Contains("common"))
            //{ 
            //    carModel.transform.localScale = new Vector3(0.31f, 0.31f, 0.31f);
            //}
            //else
            //{
            //    carModel.transform.localScale = new Vector3(2f, 2f, 2f);
            //}
            carModel.transform.localPosition = new Vector3(0, 0, 0);
            carModel.transform.localRotation = Quaternion.identity;

        }
    }
}