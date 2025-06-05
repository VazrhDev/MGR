using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace MGR
{
    public class CameraChange : MonoBehaviour
    {
        //change the active camera during the race with V key, in input manager you can change the key in Viewmode input
        public Camera NormalCam, FarCam, FPCam;//set the different cameras from inspector or add more in here
        [Header("Edit > Project Settings > Input - Set your axes name here:")]
        public string ButtonName;
        private int CamMode;

        public Transform activeCamera;

        [SerializeField] private Transform car;
        [SerializeField] private MiniMapCamera miniMapCamera;

        [SerializeField] private Volume postProcessVolume;

        private void Start()
        {
            FPCam.transform.SetParent(car);
            FarCam.transform.SetParent(car);
            activeCamera = NormalCam.transform;
            Invoke(nameof(GetPlayer), 2f);
        }
        void GetPlayer()
        {
            GameObject playerCar = GameObject.FindGameObjectWithTag("PlayerCar");

            if(!playerCar)
            {
                Invoke(nameof(GetPlayer), 1f);
                return;
            }

            playerCar.TryGetComponent<Transform>(out car);

            if (!car)
            {
                Invoke(nameof(GetPlayer), 1f);
                return;
            }

            CameraController cameracontroller = null;

            NormalCam.TryGetComponent<CameraController>(out cameracontroller);
            if (cameracontroller != null)
            {
                cameracontroller.car = car;
                cameracontroller.SetController();

                cameracontroller = null;
            }

            FarCam.TryGetComponent<CameraController>(out cameracontroller);
            if (cameracontroller != null)
            {
                cameracontroller.car = car;
                cameracontroller.SetController();

                cameracontroller = null;
            }

            FPCam.TryGetComponent<CameraController>(out cameracontroller);
            if (cameracontroller != null)
            {
                cameracontroller.car = car;
                cameracontroller.SetController();

                cameracontroller = null;
            }

            miniMapCamera.SetPlayerCar(car);

            //car.GetComponent<CarMovementController>().postProcessVolume = postProcessVolume;

        }
        void Update()
        {
            if (Input.GetButtonDown(ButtonName))//if you press the camera change keyboard button
            {
                if (CamMode == 2)//once you reach the last camera
                {
                    CamMode = 0;//you go back to the first one
                }
                else
                {
                    CamMode += 1;//if you are not in the last camera, the next one is showed
                }
                StartCoroutine(ChangeCamera());//the camera changes because of the ChangeCamera coroutine
            }
        }
        public void SetCar(GameObject Car)
        {
            car = car.transform;
        }

        IEnumerator ChangeCamera()
        {
            yield return new WaitForSeconds(0.01f);//it waits for the next frame
                                                   //and sets the corresponding camera for each number and deactivates the other ones
            if (CamMode == 0)
            {
                NormalCam.transform.SetParent(this.transform);
                FPCam.transform.SetParent(car);

                NormalCam.gameObject.SetActive(true);
                FPCam.gameObject.SetActive(false);
                activeCamera = NormalCam.transform;
            }
            if (CamMode == 1)
            {
                FarCam.transform.SetParent(this.transform);
                NormalCam.transform.SetParent(car);

                FarCam.gameObject.SetActive(true);
                NormalCam.gameObject.SetActive(false);
                activeCamera = FarCam.transform;
            }
            if (CamMode == 2)
            {
                FPCam.transform.SetParent(this.transform);
                FarCam.transform.SetParent(car);

                FPCam.gameObject.SetActive(true);
                FarCam.gameObject.SetActive(false);
                activeCamera = FPCam.transform;
            }
        }
    }
}