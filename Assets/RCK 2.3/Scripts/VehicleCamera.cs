using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using MasterDataNS;
using MGR;
using TMPro;
using Photon.Pun;
using UnityEngine.Rendering;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine.Rendering.Universal;
using static Unity.Burst.Intrinsics.X86.Avx;

public class VehicleCamera : MonoBehaviour
{
    public Transform target;

    public float smooth = 0.3f;
    public float distance = 5.0f;
    public float boostDistance = 10.0f;
    public float boostSmooth = 0.5f;
    public float height = 1.0f;
    public float Angle = 20;
    float initialDistace;
    public float TimetoTrans = 5f;

    public List<Transform> cameraSwitchView;
    public LayerMask lineOfSightMask = 0;

    public CarUIClass CarUI;


   
    private float yVelocity = 0.0f;
    private float xVelocity = 0.0f;
    [HideInInspector]
    public int Switch;

    private int gearst = 0;
    private float thisAngle = -150;
    private float restTime = 0.0f;


    private Rigidbody myRigidbody;

    ChromaticAberration chromaticAberration;
    DepthOfField depthOfField;


    private VehicleControl carScript;

    private Vector3 _originalPos;
    private float _timeAtCurrentFrame;
    private float _timeAtLastFrame;
    private float _fakeDelta;

    [System.Serializable]
    public class CarUIClass
    {

        public Image tachometerNeedle;
        public Image barShiftGUI;

        public TMP_Text speedText;
        public TMP_Text GearText;

    }


    


    ////////////////////////////////////////////// TouchMode (Control) ////////////////////////////////////////////////////////////////////


    private int PLValue = 0;


    public void PoliceLightSwitch()
    {

        if (!target.gameObject.GetComponent<PoliceLights>()) return;

        PLValue++;

        if (PLValue > 1) PLValue = 0;

        if (PLValue == 1)
            target.gameObject.GetComponent<PoliceLights>().activeLight = true;

        if (PLValue == 0)
            target.gameObject.GetComponent<PoliceLights>().activeLight = false;


    }


    public void CameraSwitch()
    {
        Switch++;
        if (Switch > cameraSwitchView.Count) { Switch = 0; }
    }


    public void CarAccelForward(float amount)
    {
        carScript.accelFwd = amount;
    }

    public void CarAccelBack(float amount)
    {
        carScript.accelBack = amount;
    }

    public void CarSteer(float amount)
    {
        carScript.steerAmount = amount;
    }

    public void CarHandBrake(bool HBrakeing)
    {
        carScript.brake = HBrakeing;
    }

    public void CarShift(bool Shifting)
    {
        carScript.shift = Shifting;
    }



    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    

    public void RestCar()
    {

        if (restTime == 0)
        {
            myRigidbody.AddForce(Vector3.up * 500000);
            myRigidbody.MoveRotation(Quaternion.Euler(0, transform.eulerAngles.y, 0));
            restTime = 2.0f;
        }

    }




    public void ShowCarUI()
    {



        gearst = carScript.currentGear;
        CarUI.speedText.text = ((int)carScript.speed).ToString();




        if (carScript.carSetting.automaticGear)
        {

            if (gearst > 0 && carScript.speed > 1)
            {
                CarUI.GearText.color = Color.green;
                CarUI.GearText.text = gearst.ToString();
            }
            else if (carScript.speed > 1)
            {
                CarUI.GearText.color = Color.red;
                CarUI.GearText.text = "R";
            }
            else
            {
                CarUI.GearText.color = Color.white;
                CarUI.GearText.text = "N";
            }

        }
        else
        {

            if (carScript.NeutralGear)
            {
                CarUI.GearText.color = Color.white;
                CarUI.GearText.text = "N";
            }
            else
            {
                if (carScript.currentGear != 0)
                {
                    CarUI.GearText.color = Color.green;
                    CarUI.GearText.text = gearst.ToString();
                }
                else
                {

                    CarUI.GearText.color = Color.red;
                    CarUI.GearText.text = "R";
                }
            }

        }





        thisAngle = (carScript.motorRPM / 20) - 175;
        thisAngle = Mathf.Clamp(thisAngle, -180, 90);

        CarUI.tachometerNeedle.rectTransform.rotation = Quaternion.Euler(0, 0, -thisAngle);
        CarUI.barShiftGUI.rectTransform.localScale = new Vector3(carScript.powerShift / 100.0f, 1, 1);

    }



    void Start()
    {

        //carScript = (VehicleControl)target.GetComponent<VehicleControl>();

        //myRigidbody = target.GetComponent<Rigidbody>();

        //cameraSwitchView = carScript.carSetting.cameraSwitchView;
        
        Invoke(nameof(GetPlayer), 2f);
        initialDistace = distance;
    }

    void GetPlayer()
    {
        GameObject playerCar = GameObject.FindGameObjectWithTag("PlayerCar");

        if (!playerCar)
        {
            Invoke(nameof(GetPlayer), 1f);
            return;
        }

        playerCar.TryGetComponent<Transform>(out target);

        if (!target)
        {
            Invoke(nameof(GetPlayer), 1f);
            return;
        }

        carScript = (VehicleControl)target.GetComponent<VehicleControl>();

        myRigidbody = target.GetComponent<Rigidbody>();

        chromaticAberration = RaceManager.Instance.chromaticAberration;
        depthOfField = RaceManager.Instance.depthOfField;

        cameraSwitchView = carScript.carSetting.cameraSwitchView;

#if UNITY_WEBGL

        CarUI.tachometerNeedle = carScript.carUIWeb.tachometerNeedle;
        CarUI.barShiftGUI = carScript.carUIWeb.barShiftGUI;
        CarUI.speedText = carScript.carUIWeb.speedText;
        CarUI.GearText = carScript.carUIWeb.GearText;
#endif
#if UNITY_ANDROID

        CarUI.tachometerNeedle = carScript.carUIMobile.tachometerNeedle;
        CarUI.barShiftGUI = carScript.carUIMobile.barShiftGUI;
        CarUI.speedText = carScript.carUIMobile.speedText;
        CarUI.GearText = carScript.carUIMobile.GearText;

#endif


    }


    void Update()
    {

        if (!target) return;


        carScript = (VehicleControl)target.GetComponent<VehicleControl>();



        //if (Input.GetKeyDown(KeyCode.G))
        //{
        //    RestCar();
        //}


        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    Application.LoadLevel(Application.loadedLevel);
        //}


        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    PoliceLightSwitch();
        //}


        if (restTime!=0.0f)
        restTime=Mathf.MoveTowards(restTime ,0.0f,Time.deltaTime);




        ShowCarUI();

        if(!carScript.isboosting)
        {
            GetComponent<Camera>().fieldOfView = Mathf.Clamp(carScript.speed / 5.0f + 60.0f, 60, 90.0f);
            chromaticAberration.active = false;
            depthOfField.focalLength.value = Mathf.Lerp(225f, 206f, TimetoTrans);
            distance = Mathf.Lerp(distance, initialDistace, boostSmooth);
        }
        else
        {
            
            distance = Mathf.Lerp(distance, boostDistance, boostSmooth);
            chromaticAberration.active = true;
            depthOfField.focalLength.value = 225f;
        }



        if (Input.GetKeyDown(KeyCode.C))
        {
            Switch++;
            if (Switch > cameraSwitchView.Count) { Switch = 0; }
        }



        if (Switch == 0)
        {
            // Damp angle from current y-angle towards target y-angle

            float xAngle = Mathf.SmoothDampAngle(transform.eulerAngles.x,
           target.eulerAngles.x + Angle, ref xVelocity, smooth);

            float yAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y,
            target.eulerAngles.y, ref yVelocity, smooth);

            // Look at the target
            transform.eulerAngles = new Vector3(xAngle, yAngle,0.0f);

            var direction = transform.rotation * -Vector3.forward;
            var targetDistance = AdjustLineOfSight(target.position + new Vector3(0, height, 0), direction);


            transform.position = target.position + new Vector3(0, height, 0) + direction * targetDistance;


        }
        else
        {

            transform.position = cameraSwitchView[Switch - 1].position;
            transform.rotation = Quaternion.Lerp(transform.rotation, cameraSwitchView[Switch - 1].rotation, Time.deltaTime * 5.0f);

        }

    }



    float AdjustLineOfSight(Vector3 target, Vector3 direction)
    {


        
        RaycastHit[] hit = Physics.RaycastAll(target, direction, distance, lineOfSightMask.value);

        //if (Physics.Raycast(target, direction, out hit, distance, lineOfSightMask.value))
        //{
        //    hit.transform.root.GetComponent<PhotonView>();
        //    return hit.distance;
        //}
        //else
        //    return distance;
        if(hit.Length > 0)
        {
            foreach (var item in hit)
            {
                if(item.transform.root.GetComponent<PhotonView>())
                {
                    if(item.transform.root.GetComponent<PhotonView>().IsMine)
                    {
                        return item.distance;
                    }
                }
            }
        }
        return distance;
    }

    //Camera Shake
    public void Shake(float duration, float amount)
    {
        _originalPos = transform.localPosition;
        StopAllCoroutines();
        StartCoroutine(cShake(duration, amount));
    }
    IEnumerator cShake(float duration, float amount)
    {
        float endTime = Time.time + duration;

        while (duration > 0)
        {
            transform.localPosition = _originalPos + UnityEngine.Random.insideUnitSphere * amount;

            duration -= _fakeDelta;

            yield return null;
        }
        transform.localPosition = _originalPos;
    }

}
