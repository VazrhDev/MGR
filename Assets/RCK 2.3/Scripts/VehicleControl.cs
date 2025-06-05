using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MGR;
using System;
using TMPro;
using Photon.Pun;
using UnityEngine.Pool;

public enum ControlMode { simple = 1, touch = 2 }


public class VehicleControl : MonoBehaviour
{


    public ControlMode controlMode = ControlMode.simple;

    public bool activeControl = false;


    // Wheels Setting /////////////////////////////////

    public CarWheels carWheels;

    [System.Serializable]
    public class CarWheels
    {
        public ConnectWheel wheels;
        public WheelSetting setting;
    }


    [System.Serializable]
    public class ConnectWheel
    {
        public bool frontWheelDrive = true;
        public Transform frontRight;
        public Transform frontLeft;

        public bool backWheelDrive = true;
        public Transform backRight;
        public Transform backLeft;
    }

    [System.Serializable]
    public class WheelSetting
    {
        public float Radius = 0.4f;
        public float Weight = 1000.0f;
        public float Distance = 0.2f;
    }


    // Lights Setting ////////////////////////////////

    public CarLights carLights;

    [System.Serializable]
    public class CarLights
    {
        public Light[] brakeLights;
        public Light[] reverseLights;
    }

    // Car sounds /////////////////////////////////

    public CarSounds carSounds;

    [System.Serializable]
    public class CarSounds
    {
        public AudioSource IdleEngine, LowEngine, HighEngine;

        public AudioSource nitro;
        public AudioSource switchGear;
    }

    // Car Particle /////////////////////////////////

    public CarParticles carParticles;

    [System.Serializable]
    public class CarParticles
    {
        public GameObject brakeParticlePerfab;
        public GameObject skidMarksPrefab;
        public ParticleSystem shiftParticle1, shiftParticle2;
        private GameObject[] wheelParticle = new GameObject[4];
        public GameObject CollisionSparks;
    }

    // Car Engine Setting /////////////////////////////////

    public CarSetting carSetting;

    [System.Serializable]
    public class CarSetting
    {

        public bool showNormalGizmos = false;
        public Transform carSteer;
        public HitGround[] hitGround;

        public List<Transform> cameraSwitchView;

        public float springs = 25000.0f;
        public float dampers = 1500.0f;

        public float carPower = 120f;               // Power at which car moves/Acceleration and deceleration value of car
        public float shiftPower = 150f;
        public float brakePower = 8000f;
        public float acceleration = 100f;
        public float deceleration = 50f;

        public Vector3 shiftCentre = new Vector3(0.0f, -0.8f, 0.0f);

        public float maxSteerAngle = 25.0f;

        public float shiftDownRPM = 1500.0f;
        public float shiftUpRPM = 2500.0f;
        public float idleRPM = 500.0f;

        public float stiffness = 2.0f;

        public bool automaticGear = true;

        public float[] gears = { -10f, 9f, 6f, 4.5f, 3f, 2.5f };


        public float LimitBackwardSpeed = 60.0f;
        public float LimitForwardSpeed = 220.0f;


        public float sportiveness;
        public float fuelEfficiency;
        public float stability;
    }




    [System.Serializable]
    public class HitGround
    {

        public string tag = "street";
        public bool grounded = false;
        public AudioClip brakeSound;
        public AudioClip groundSound;
        public Color brakeColor;
    }

    [System.Serializable]
    public class CarUIWeb
    {
        public Image tachometerNeedle;
        public Image barShiftGUI;

        public TMP_Text speedText;
        public TMP_Text GearText;

    }
    [System.Serializable]
    public class CarUIMobile
    {
        public Image tachometerNeedle;
        public Image barShiftGUI;

        public TMP_Text speedText;
        public TMP_Text GearText;

    }
    public CarUIWeb carUIWeb;
    public CarUIMobile carUIMobile;


    [Header("Model")]
    [SerializeField] private GameObject carModelHolder;
    public GameObject CarModelHolder { get { return carModelHolder; } }

    [Header("Photon Networking")]
    [SerializeField] private PhotonView photonView;

    [Header("Car UI")]
    //[SerializeField] CarUIManager carUIManager;
    [SerializeField] GameObject carUIWebGL;
    [SerializeField] GameObject carUIAndroid;
    [SerializeField] GameObject mainUI;


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public bool isboosting { get; private set; } = false;
    private ObjectPool<GameObject> _CollisionPool;

    private float steer = 0;
    private float accel = 0.0f;
    [HideInInspector]
    public bool brake;

    private bool shifmotor;

    [HideInInspector]
    public float curTorque = 100f;
    [HideInInspector]
    public float powerShift = 100;
    [HideInInspector]
    public bool shift;

    private float torque = 100f;

    [HideInInspector]
    public float speed = 0.0f;

    private float lastSpeed = -10.0f;


    private bool shifting = false;


    float[] efficiencyTable = { 0.6f, 0.65f, 0.7f, 0.75f, 0.8f, 0.85f, 0.9f, 1.0f, 1.0f, 0.95f, 0.80f, 0.70f, 0.60f, 0.5f, 0.45f, 0.40f, 0.36f, 0.33f, 0.30f, 0.20f, 0.10f, 0.05f };


    float efficiencyTableStep = 250.0f;



    private float Pitch;
    private float PitchDelay;

    private float shiftTime = 0.0f;

    private float shiftDelay = 0.0f;


    [HideInInspector]
    public int currentGear = 0;
    [HideInInspector]
    public bool NeutralGear = true;

    [HideInInspector]
    public float motorRPM = 0.0f;

    [HideInInspector]
    public bool Backward = false;

    ////////////////////////////////////////////// TouchMode (Control) ////////////////////////////////////////////////////////////////////


    [HideInInspector]
    public float accelFwd = 0.0f;
    [HideInInspector]
    public float accelBack = 0.0f;
    [HideInInspector]
    public float steerAmount = 0.0f;


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



    private float wantedRPM = 0.0f;
    private float w_rotate;
    [HideInInspector] public float slip, slip2 = 0.0f;


    private GameObject[] Particle = new GameObject[4];
    private GameObject[] SkidParticle = new GameObject[4];

    private Vector3 steerCurAngle;

    public Rigidbody myRigidbody;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    private WheelComponent[] wheels;



    private class WheelComponent
    {

        public Transform wheel;
        public WheelCollider collider;
        public Vector3 startPos;
        public float rotation = 0.0f;
        public float rotation2 = 0.0f;
        public float maxSteer;
        public bool drive;
        public float pos_y = 0.0f;
    }


    private WheelComponent SetWheelComponent(Transform wheel, float maxSteer, bool drive, float pos_y, WheelCollider wheelCol)
    {
        Debug.Log("Setting Wheel Component");

        WheelComponent result = new WheelComponent();
        //GameObject wheelCol = new GameObject(wheel.name + "WheelCollider");

        //wheelCol.transform.parent = transform;
        //wheelCol.transform.position = wheel.position;
        //wheelCol.transform.eulerAngles = transform.eulerAngles;
        //pos_y = wheelCol.transform.localPosition.y;

        //WheelCollider col = (WheelCollider)wheelCol.AddComponent(typeof(WheelCollider));

        result.wheel = wheel;
        //result.collider = wheelCol.GetComponent<WheelCollider>();
        result.collider = wheelCol;
        result.drive = drive;
        result.pos_y = pos_y;
        result.maxSteer = maxSteer;
        result.startPos = wheelCol.transform.localPosition;

        return result;

    }



    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

  

    void Awake()
    {

        //if (carSetting.automaticGear) NeutralGear = false;

        //myRigidbody = transform.GetComponent<Rigidbody>();

        //wheels = new WheelComponent[4];

        //wheels[0] = SetWheelComponent(carWheels.wheels.frontRight, carSetting.maxSteerAngle, carWheels.wheels.frontWheelDrive, carWheels.wheels.frontRight.position.y);
        //wheels[1] = SetWheelComponent(carWheels.wheels.frontLeft, carSetting.maxSteerAngle, carWheels.wheels.frontWheelDrive, carWheels.wheels.frontLeft.position.y);

        //wheels[2] = SetWheelComponent(carWheels.wheels.backRight, 0, carWheels.wheels.backWheelDrive, carWheels.wheels.backRight.position.y);
        //wheels[3] = SetWheelComponent(carWheels.wheels.backLeft, 0, carWheels.wheels.backWheelDrive, carWheels.wheels.backLeft.position.y);

        //if (carSetting.carSteer)
        //steerCurAngle = carSetting.carSteer.localEulerAngles;

        //foreach (WheelComponent w in wheels)
        //{


        //    WheelCollider col = w.collider;
        //    col.suspensionDistance = carWheels.setting.Distance;
        //    JointSpring js = col.suspensionSpring;

        //    js.spring = carSetting.springs;
        //    js.damper = carSetting.dampers;
        //    col.suspensionSpring = js;


        //    col.radius = carWheels.setting.Radius;

        //    col.mass = carWheels.setting.Weight;


        //    WheelFrictionCurve fc = col.forwardFriction;

        //    fc.asymptoteValue = 5000.0f;
        //    fc.extremumSlip = 2.0f;
        //    fc.asymptoteSlip = 20.0f;
        //    fc.stiffness = carSetting.stiffness;
        //    col.forwardFriction = fc;
        //    fc = col.sidewaysFriction;
        //    fc.asymptoteValue = 7500.0f;
        //    fc.asymptoteSlip = 2.0f;
        //    fc.stiffness = carSetting.stiffness;
        //    col.sidewaysFriction = fc;


        //}

        if (!photonView.IsMine)
        {
            Debug.LogError("MEra nhi hai");
            carUIAndroid.SetActive(false);
            carUIWebGL.SetActive(false);
            mainUI.SetActive(false);
        }
        else
        {
            Debug.LogError("MEra hi hai");
            mainUI.SetActive(true);
            if (Application.platform == RuntimePlatform.Android)
            {
                controlMode = ControlMode.touch;

                carUIAndroid.SetActive(true);
                carUIWebGL.SetActive(false);
            }
            else
            {
                controlMode = ControlMode.simple;

                carUIAndroid.SetActive(false);
                carUIWebGL.SetActive(true);
            }
        }
        
            // Activate Controller
            ActivateController();

#if UNITY_WEBGL



#endif

#if UNITY_ANDROID

        

#endif

    }

    private void Start()
    {
        if (carSetting.automaticGear) NeutralGear = false;

        myRigidbody = transform.GetComponent<Rigidbody>();

        if (carSetting.carSteer)
            steerCurAngle = carSetting.carSteer.localEulerAngles;

        // Wheel Data

        // Gameobject that contains wheel meshes
        GameObject wheelMeshesObject = GameObject.Find("WheelMeshes");

        if (wheelMeshesObject != null)
        {
            Debug.Log("Wheel Mesh object is not null");

            var wheelMeshes = new Transform[4];

            for (int i = 0; i < wheelMeshesObject.transform.childCount; i++)
            {
                wheelMeshes[i] = wheelMeshesObject.transform.GetChild(i);
            }

            carWheels.wheels.frontLeft = wheelMeshes[0];
            carWheels.wheels.frontRight = wheelMeshes[1];
            carWheels.wheels.backLeft = wheelMeshes[2];
            carWheels.wheels.backRight = wheelMeshes[3];
        }

        // Find Gameobject that contains wheel colliders
        GameObject wheelCollidersObject = GameObject.Find("WheelColliders");

        if (wheelCollidersObject != null)
        {
            Debug.Log("Wheel Colliders object is not null");

            var wheelColliders = new WheelCollider[4];

            wheels = new WheelComponent[4];

            for (int i = 0; i < wheelCollidersObject.transform.childCount; i++)
            {
                wheelColliders[i] = wheelCollidersObject.transform.GetChild(i).GetComponent<WheelCollider>();
            }

            wheels[0] = SetWheelComponent(carWheels.wheels.frontRight, carSetting.maxSteerAngle, carWheels.wheels.frontWheelDrive, carWheels.wheels.frontRight.position.y, wheelColliders[0]);
            wheels[1] = SetWheelComponent(carWheels.wheels.frontLeft, carSetting.maxSteerAngle, carWheels.wheels.frontWheelDrive, carWheels.wheels.frontLeft.position.y, wheelColliders[1]);

            wheels[2] = SetWheelComponent(carWheels.wheels.backRight, 0, carWheels.wheels.backWheelDrive, carWheels.wheels.backRight.position.y, wheelColliders[2]);
            wheels[3] = SetWheelComponent(carWheels.wheels.backLeft, 0, carWheels.wheels.backWheelDrive, carWheels.wheels.backLeft.position.y, wheelColliders[3]);
        }


        // Updating Wheels Data
        foreach (WheelComponent w in wheels)
        {


            WheelCollider col = w.collider;
            col.suspensionDistance = carWheels.setting.Distance;
            JointSpring js = col.suspensionSpring;

            js.spring = carSetting.springs;
            js.damper = carSetting.dampers;
            col.suspensionSpring = js;


            //col.radius = carWheels.setting.Radius;

            col.mass = carWheels.setting.Weight;


            WheelFrictionCurve fc = col.forwardFriction;

            fc.asymptoteValue = 5000.0f;
            fc.extremumSlip = 2.0f;
            fc.asymptoteSlip = 20.0f;
            fc.stiffness = carSetting.stiffness;
            col.forwardFriction = fc;
            fc = col.sidewaysFriction;
            fc.asymptoteValue = 7500.0f;
            fc.asymptoteSlip = 2.0f;
            fc.stiffness = carSetting.stiffness;
            col.sidewaysFriction = fc;
        }


        //SetCarData();

        // Deactivate UI if it's not owner / Stopping UI from replicating
        if (!photonView)
            return;
        if (!photonView.IsMine)
        {
            //carUIManager.gameObject.SetActive(false);
        }
        //// Object Pool
        _CollisionPool = new ObjectPool<GameObject>(() =>
        {
            return Instantiate(carParticles.CollisionSparks);
        }, GameObject =>
        {
            GameObject.gameObject.SetActive(true);
        }, GameObject =>
        {
            GameObject.gameObject.SetActive(false);
        }, GameObject =>
        {
            Destroy(GameObject.gameObject);
        }, false, 10, 20);
    }

    void SetCarData()
    {

        // Car Data

        CarData carData = GameManager.instance.currentCarData;

        // Set Top Speed
        float totalTopSpeed = carData.engineDetails.engineData.topSpeed + carData.compoundsDetails.compoundData.topSpeed + carData.transmissionDetails.transmissionData.topSpeed + carData.suspensionDetails.suspensionData.topSpeed;

        float speedMargin = (totalTopSpeed / 10) * (DatabaseHandler.instance.masterData.maxTopSpeed - DatabaseHandler.instance.masterData.minTopSpeed);

        carSetting.LimitForwardSpeed = DatabaseHandler.instance.masterData.minTopSpeed + speedMargin;

        // Set Acceleration
        float totalAcceleration = carData.engineDetails.engineData.acceleration + carData.transmissionDetails.transmissionData.acceleration + carData.compoundsDetails.compoundData.acceleration + carData.framesDetails.frameData.acceleration + carData.suspensionDetails.suspensionData.acceleration;

        float accMargin = (totalAcceleration / 10) * (DatabaseHandler.instance.masterData.maxAcceleration - DatabaseHandler.instance.masterData.minAcceleration);

        //acceleration = accMargin + DatabaseHandler.instance.masterData.minAcceleration;
        carSetting.acceleration = accMargin + DatabaseHandler.instance.masterData.minAcceleration;

        // Set Handling
        float totalHandling = carData.steeringDetails.steeringData.handling;

        float handlingMargin = (totalHandling / 10) * (DatabaseHandler.instance.masterData.maxHandling - DatabaseHandler.instance.masterData.minHandling);

        carSetting.maxSteerAngle = DatabaseHandler.instance.masterData.minHandling + totalHandling;     // angle need to be between 30-60

        // Set Weight Data
        float totalWeight = carData.framesDetails.frameData.weight + carData.wheelsDetails.wheelData.weight;

        myRigidbody.mass = (totalWeight / 10) * DatabaseHandler.instance.masterData.maxWeight;

        // Set Sportiveness Data
        float totalSportiveness = carData.suspensionDetails.suspensionData.sportiveness + carData.compoundsDetails.compoundData.sportiveness + carData.wheelsDetails.wheelData.sportiveness;

        carSetting.stability = (totalSportiveness / 10) * DatabaseHandler.instance.masterData.maxSportiveness;

        // Set Fuel Efficiency Data
        float totalFE = carData.transmissionDetails.transmissionData.fuelEfficiency + carData.framesDetails.frameData.fuelEfficiency + carData.brakesDetails.brakeData.fuelEfficiency;

        carSetting.fuelEfficiency = (totalFE / 10) * DatabaseHandler.instance.masterData.maxFuelEfficiency;

        // Set Stability Data
        float totalStability = carData.compoundsDetails.compoundData.stability + carData.suspensionDetails.suspensionData.stability + carData.steeringDetails.steeringData.stability;

        carSetting.stability = (totalStability / 10) * DatabaseHandler.instance.masterData.maxStability;


        // Set Brake Data
        float totalBrakeForce = carData.brakesDetails.brakeData.deceleration + carData.compoundsDetails.compoundData.deceleration + carData.wheelsDetails.wheelData.deceleration;

        float decMargin = (totalBrakeForce / 10) * (DatabaseHandler.instance.masterData.maxDeceleration - DatabaseHandler.instance.masterData.minDeceleration);

        carSetting.deceleration = decMargin + DatabaseHandler.instance.masterData.minDeceleration;


        // Set Nitro Data
        //NitroForce = carData.nitroDetails.nitroData.accelerationOnUsing;


    }


    public void ShiftUp()
    {
        float now = Time.timeSinceLevelLoad;

        if (now < shiftDelay) return;

        if (currentGear < carSetting.gears.Length - 1)
        {

            // if (!carSounds.switchGear.isPlaying)
            carSounds.switchGear.GetComponent<AudioSource>().Play();


            if (!carSetting.automaticGear)
            {
                if (currentGear == 0)
                {
                    if (NeutralGear) { currentGear++; NeutralGear = false; }
                    else
                    { NeutralGear = true; }
                }
                else
                {
                    currentGear++;
                }
            }
            else
            {
                currentGear++;
            }


            shiftDelay = now + 1.0f;
            shiftTime = 1.5f;
        }
    }




    public void ShiftDown()
    {
        float now = Time.timeSinceLevelLoad;

        if (now < shiftDelay) return;

        if (currentGear > 0 || NeutralGear)
        {

            //w if (!carSounds.switchGear.isPlaying)
            carSounds.switchGear.GetComponent<AudioSource>().Play();

            if (!carSetting.automaticGear)
            {

                if (currentGear == 1)
                {
                    if (!NeutralGear) { currentGear--; NeutralGear = true; }
                }
                else if (currentGear == 0) { NeutralGear = false; } else { currentGear--; }
            }
            else
            {
                currentGear--;
            }


            shiftDelay = now + 0.1f;
            shiftTime = 2.0f;
        }
    }



    void OnCollisionEnter(Collision collision)
    {

        if (collision.transform.root.GetComponent<VehicleControl>())
        {

            collision.transform.root.GetComponent<VehicleControl>().slip2 = Mathf.Clamp(collision.relativeVelocity.magnitude, 0.0f, 10.0f);

            //myRigidbody.angularVelocity = new Vector3(-myRigidbody.angularVelocity.x * 0.5f, myRigidbody.angularVelocity.y * 0.5f, -myRigidbody.angularVelocity.z * 0.5f);
            //myRigidbody.velocity = new Vector3(myRigidbody.velocity.x, myRigidbody.velocity.y * 0.5f, myRigidbody.velocity.z);

        }
        else
        {
            myRigidbody.angularVelocity = new Vector3(-myRigidbody.angularVelocity.x * 0.5f, myRigidbody.angularVelocity.y * 0.5f, -myRigidbody.angularVelocity.z * 0.5f);
            myRigidbody.velocity = new Vector3(myRigidbody.velocity.x, myRigidbody.velocity.y * 0.5f, myRigidbody.velocity.z);
            //Invoke(nameof(WallCollision), 1f);

            // If vehicle collides from front or back
            if (collision.transform.forward == transform.forward || collision.transform.forward == -transform.forward)
            {
            }
        }


        ContactPoint[] contacts = new ContactPoint[3];
        int numContacts = collision.GetContacts(contacts);
        for (int i = 0; i < numContacts; i++)
        {
            //if (contacts[i].point.z - carModelHolder.transform.GetChild(0).position.z > 0)
            //{
            //    Debug.LogError("left!!");
            //    spawnCollisionVfxAt(contacts[i].point, 0.7f);
            //}
            //else
            //{
            //    Debug.LogError("right!");
            //    spawnCollisionVfxAt(contacts[i].point, 0.7f);
            //}
            //Debug.LogError(contacts[i].point - carModelHolder.transform.GetChild(0).position);
            spawnCollisionVfxAt(contacts[i].point, 0.7f);
        }


    }


    void OnCollisionStay(Collision collision)
    {

        if (collision.transform.root.GetComponent<VehicleControl>())
            collision.transform.root.GetComponent<VehicleControl>().slip2 = 5.0f;

    }

    // Stop Vehicle after colliding with walls
    void WallCollision()
    {
        currentGear = 0;
        motorRPM = 0;
        wantedRPM = 0;

        myRigidbody.isKinematic = true;

        //NeutralGear = true;

        myRigidbody.angularVelocity = Vector3.zero;
        myRigidbody.velocity = Vector3.zero;


        Invoke(nameof(FreeFromCollision), 0.5f);

        foreach (WheelComponent w in wheels)
        {
            w.collider.motorTorque = 0;
        }
    }

    void FreeFromCollision()
    {
        myRigidbody.isKinematic = false;
    }

    // Spawn CollisionVFX
    void spawnCollisionVfxAt(Vector3 position, float timeToKill)
    {
        var vfx = _CollisionPool.Get();
        vfx.transform.parent = transform;
        vfx.transform.position = position;
        vfx.transform.rotation = Quaternion.LookRotation(position);
        StartCoroutine(KillVfx(vfx, timeToKill));
    }
    IEnumerator KillVfx(GameObject objectVfx, float timeToKill)
    {
        yield return new WaitForSeconds(timeToKill);
        _CollisionPool.Release(objectVfx);
    }




    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void Update()
    {


        if (!carSetting.automaticGear && activeControl)
        {
            if (Input.GetKeyDown("page up"))
            {
                ShiftUp();


            }
            if (Input.GetKeyDown("page down"))
            {
                ShiftDown();

            }
        }

    }




    void FixedUpdate()
    {


        // speed of car
        speed = myRigidbody.velocity.magnitude * 2.7f;



        if (speed < lastSpeed - 10 && slip < 10)
        {
            slip = lastSpeed / 15;
        }

        lastSpeed = speed;




        if (slip2 != 0.0f)
            slip2 = Mathf.MoveTowards(slip2, 0.0f, 0.1f);



        myRigidbody.centerOfMass = carSetting.shiftCentre;




        if (activeControl)
        {

            if (controlMode == ControlMode.simple)
            {


                accel = 0;
                brake = false;
                shift = false;

                if (carWheels.wheels.frontWheelDrive || carWheels.wheels.backWheelDrive)
                {
                    steer = Mathf.MoveTowards(steer, Input.GetAxis("Horizontal"), 0.2f);
                    accel = Input.GetAxis("Vertical");
                    brake = Input.GetButton("Jump");
                    shift = Input.GetKey(KeyCode.LeftShift) | Input.GetKey(KeyCode.RightShift);
                    //shift = Input.GetKeyDown(KeyCode.LeftShift) | Input.GetKeyDown(KeyCode.RightShift);


                    if (accel > 0)
                    {
                        carSetting.carPower = carSetting.acceleration;
                    }
                    else if (accel < 0)
                    {
                        carSetting.carPower = carSetting.deceleration;
                    }
                    else
                    {
                        carSetting.carPower = 0;
                    }
                }

            }
            else if (controlMode == ControlMode.touch)
            {

                if (accelFwd != 0)
                {
                    accel = accelFwd;
                    carSetting.carPower = carSetting.acceleration;
                }

                else
                {
                    accel = accelBack;
                    carSetting.carPower = carSetting.deceleration;
                }
                steer = Mathf.MoveTowards(steer, steerAmount, 0.07f);

            }

        }
        else
        {
            accel = 0.0f;
            steer = 0.0f;
            brake = false;
            shift = false;
        }



        if (!carWheels.wheels.frontWheelDrive && !carWheels.wheels.backWheelDrive)
            accel = 0.0f;



        if (carSetting.carSteer)
            carSetting.carSteer.localEulerAngles = new Vector3(steerCurAngle.x, steerCurAngle.y, steerCurAngle.z + (steer * -120.0f));



        if (carSetting.automaticGear && (currentGear == 1) && (accel < 0.0f))
        {
            if (speed < 5.0f)
                ShiftDown();


        }
        else if (carSetting.automaticGear && (currentGear == 0) && (accel > 0.0f))
        {
            if (speed < 5.0f)
                ShiftUp();

        }
        else if (carSetting.automaticGear && (motorRPM > carSetting.shiftUpRPM) && (accel > 0.0f) && speed > 10.0f && !brake)
        {

            ShiftUp();

        }
        else if (carSetting.automaticGear && (motorRPM < carSetting.shiftDownRPM) && (currentGear > 1))
        {
            ShiftDown();
        }



        if (speed < 1.0f) Backward = true;



        if (currentGear == 0 && Backward == true)
        {
            //  carSetting.shiftCentre.z = -accel / -5;
            if (speed < carSetting.gears[0] * -10)
                accel = -accel;
        }
        else
        {
            Backward = false;
            //   if (currentGear > 0)
            //   carSetting.shiftCentre.z = -(accel / currentGear) / -5;
        }




        //  carSetting.shiftCentre.x = -Mathf.Clamp(steer * (speed / 100), -0.03f, 0.03f);



        // Brake Lights

        foreach (Light brakeLight in carLights.brakeLights)
        {
            if (brake || accel < 0 || speed < 1.0f)
            {
                brakeLight.intensity = Mathf.MoveTowards(brakeLight.intensity, 8, 0.5f);
            }
            else
            {
                brakeLight.intensity = Mathf.MoveTowards(brakeLight.intensity, 0, 0.5f);

            }

            brakeLight.enabled = brakeLight.intensity == 0 ? false : true;
        }


        // Reverse Lights

        foreach (Light WLight in carLights.reverseLights)
        {
            if (speed > 2.0f && currentGear == 0)
            {
                WLight.intensity = Mathf.MoveTowards(WLight.intensity, 8, 0.5f);
            }
            else
            {
                WLight.intensity = Mathf.MoveTowards(WLight.intensity, 0, 0.5f);
            }
            WLight.enabled = WLight.intensity == 0 ? false : true;
        }






        wantedRPM = (5500.0f * accel) * 0.1f + wantedRPM * 0.9f;

        float rpm = 0.0f;
        int motorizedWheels = 0;
        bool floorContact = false;
        int currentWheel = 0;





        foreach (WheelComponent w in wheels)
        {
            WheelHit hit;
            WheelCollider col = w.collider;

            if (w.drive)
            {
                if (!NeutralGear && brake && currentGear < 2)
                {
                    rpm += accel * carSetting.idleRPM;

                    /*
                    if (rpm > 1)
                    {
                        carSetting.shiftCentre.z = Mathf.PingPong(Time.time * (accel * 10), 2.0f) - 1.0f;
                    }
                    else
                    {
                        carSetting.shiftCentre.z = 0.0f;
                    }
                    */

                }
                else
                {
                    if (!NeutralGear)
                    {
                        rpm += col.rpm;
                    }
                    else
                    {
                        rpm += (carSetting.idleRPM * accel);
                    }
                }


                motorizedWheels++;
            }




            if (brake || accel < 0.0f)
            {

                if ((accel < 0.0f) || (brake && (w == wheels[2] || w == wheels[3])))
                {

                    if (brake && (accel > 0.0f))
                    {
                        slip = Mathf.Lerp(slip, 5.0f, accel * 0.01f);
                    }
                    else if (speed > 1.0f)
                    {
                        slip = Mathf.Lerp(slip, 1.0f, 0.002f);
                    }
                    else
                    {
                        slip = Mathf.Lerp(slip, 1.0f, 0.02f);
                    }


                    wantedRPM = 0.0f;
                    col.brakeTorque = carSetting.brakePower;
                    w.rotation = w_rotate;

                }
            }
            else
            {


                col.brakeTorque = accel == 0 || NeutralGear ? col.brakeTorque = 1000 : col.brakeTorque = 0;


                slip = speed > 0.0f ?
    (speed > 100 ? slip = Mathf.Lerp(slip, 1.0f + Mathf.Abs(steer), 0.02f) : slip = Mathf.Lerp(slip, 1.5f, 0.02f))
    : slip = Mathf.Lerp(slip, 0.01f, 0.02f);


                w_rotate = w.rotation;

            }



            WheelFrictionCurve fc = col.forwardFriction;



            fc.asymptoteValue = 5000.0f;
            fc.extremumSlip = 2.0f;
            fc.asymptoteSlip = 20.0f;
            fc.stiffness = carSetting.stiffness / (slip + slip2);
            col.forwardFriction = fc;
            fc = col.sidewaysFriction;
            fc.stiffness = carSetting.stiffness / (slip + slip2);


            fc.extremumSlip = 0.2f + Mathf.Abs(steer);

            col.sidewaysFriction = fc;




            if (shift && (currentGear > 1 && speed > 50.0f) && shifmotor && Mathf.Abs(steer) < 0.2f)
            {
                isboosting = true;
                if (powerShift == 0) { shifmotor = false; }

                powerShift = Mathf.MoveTowards(powerShift, 0.0f, Time.deltaTime * 10.0f);

                carSounds.nitro.volume = Mathf.Lerp(carSounds.nitro.volume, 1.0f, Time.deltaTime * 10.0f);

                if (!carSounds.nitro.isPlaying)
                {
                    carSounds.nitro.GetComponent<AudioSource>().Play();

                }


                curTorque = powerShift > 0 ? carSetting.shiftPower : carSetting.carPower;
                carParticles.shiftParticle1.emissionRate = Mathf.Lerp(carParticles.shiftParticle1.emissionRate, powerShift > 0 ? 50 : 0, Time.deltaTime * 10.0f);
                carParticles.shiftParticle2.emissionRate = Mathf.Lerp(carParticles.shiftParticle2.emissionRate, powerShift > 0 ? 50 : 0, Time.deltaTime * 10.0f);
            }
            else
            {
                isboosting = false;
                if (powerShift > 50)
                {
                    shifmotor = true;
                }

                carSounds.nitro.volume = Mathf.MoveTowards(carSounds.nitro.volume, 0.0f, Time.deltaTime * 2.0f);

                if (carSounds.nitro.volume == 0)
                    carSounds.nitro.Stop();

                powerShift = Mathf.MoveTowards(powerShift, 100.0f, Time.deltaTime * 5.0f);
                curTorque = carSetting.carPower;
                carParticles.shiftParticle1.emissionRate = Mathf.Lerp(carParticles.shiftParticle1.emissionRate, 0, Time.deltaTime * 10.0f);
                carParticles.shiftParticle2.emissionRate = Mathf.Lerp(carParticles.shiftParticle2.emissionRate, 0, Time.deltaTime * 10.0f);
            }


            w.rotation = Mathf.Repeat(w.rotation + Time.deltaTime * col.rpm * 360.0f / 60.0f, 360.0f);
            w.rotation2 = Mathf.Lerp(w.rotation2, col.steerAngle, 0.1f);
            w.wheel.localRotation = Quaternion.Euler(w.rotation, w.rotation2, 0.0f);



            Vector3 lp = w.wheel.localPosition;


            if (col.GetGroundHit(out hit))
            {


                if (carParticles.brakeParticlePerfab)
                {
                    if (Particle[currentWheel] == null)
                    {
                        Particle[currentWheel] = Instantiate(carParticles.brakeParticlePerfab, w.wheel.position, Quaternion.identity) as GameObject;
                        SkidParticle[currentWheel] = Instantiate(carParticles.skidMarksPrefab, w.wheel.GetChild(0).transform.position, Quaternion.Euler(90f,0f,0f)) as GameObject;
                        Particle[currentWheel].name = "WheelParticle";
                        SkidParticle[currentWheel].name = "SkidParticle";
                        Particle[currentWheel].transform.parent = transform;
                        SkidParticle[currentWheel].transform.parent = transform;
                        Particle[currentWheel].AddComponent<AudioSource>();
                        Particle[currentWheel].GetComponent<AudioSource>().maxDistance = 50;
                        Particle[currentWheel].GetComponent<AudioSource>().spatialBlend = 1;
                        Particle[currentWheel].GetComponent<AudioSource>().dopplerLevel = 5;
                        Particle[currentWheel].GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Custom;
                    }


                    var pc = Particle[currentWheel].GetComponent<ParticleSystem>();
                    var sk = SkidParticle[currentWheel].GetComponent<TrailRenderer>();
                    bool WGrounded = false;


                    for (int i = 0; i < carSetting.hitGround.Length; i++)
                    {

                        if (hit.collider.CompareTag(carSetting.hitGround[i].tag))
                        {
                            WGrounded = carSetting.hitGround[i].grounded;

                            if ((brake || Mathf.Abs(hit.sidewaysSlip) > 0.5f) && speed > 1)
                            {
                                Particle[currentWheel].GetComponent<AudioSource>().clip = carSetting.hitGround[i].brakeSound;
                            }
                            else if (Particle[currentWheel].GetComponent<AudioSource>().clip != carSetting.hitGround[i].groundSound && !Particle[currentWheel].GetComponent<AudioSource>().isPlaying)
                            {

                                Particle[currentWheel].GetComponent<AudioSource>().clip = carSetting.hitGround[i].groundSound;
                            }

                            Particle[currentWheel].GetComponent<ParticleSystem>().startColor = carSetting.hitGround[i].brakeColor;

                        }


                    }




                    if (WGrounded && speed > 5 && !brake)
                    {

                        pc.enableEmission = true;
                        sk.emitting = true;
                        Particle[currentWheel].GetComponent<AudioSource>().volume = 0.5f;

                        if (!Particle[currentWheel].GetComponent<AudioSource>().isPlaying)
                            Particle[currentWheel].GetComponent<AudioSource>().Play();

                    }
                    else if ((brake || Mathf.Abs(hit.sidewaysSlip) > 0.6f) && speed > 1)
                    {

                        if ((accel < 0.0f) || ((brake || Mathf.Abs(hit.sidewaysSlip) > 0.6f) && (w == wheels[2] || w == wheels[3])))
                        {

                            if (!Particle[currentWheel].GetComponent<AudioSource>().isPlaying)
                                Particle[currentWheel].GetComponent<AudioSource>().Play();
                            pc.enableEmission = true;
                            sk.emitting = true;
                            Particle[currentWheel].GetComponent<AudioSource>().volume = 10;

                        }

                    }
                    else
                    {

                        pc.enableEmission = false;
                        sk.emitting = false;
                        Particle[currentWheel].GetComponent<AudioSource>().volume = Mathf.Lerp(Particle[currentWheel].GetComponent<AudioSource>().volume, 0, Time.deltaTime * 10.0f);
                    }

                }


                lp.y -= Vector3.Dot(w.wheel.position - hit.point, transform.TransformDirection(0, 1, 0) / transform.lossyScale.x) - (col.radius);
                lp.y = Mathf.Clamp(lp.y, -10.0f, w.pos_y);
                floorContact = floorContact || (w.drive);


            }
            else
            {

                if (Particle[currentWheel] != null)
                {
                    var pc = Particle[currentWheel].GetComponent<ParticleSystem>();
                    var sk = SkidParticle[currentWheel].GetComponent<TrailRenderer>();
                    pc.enableEmission = false;
                    sk.emitting = false;
                }



                lp.y = w.startPos.y - carWheels.setting.Distance;

                myRigidbody.AddForce(Vector3.down * 5000);

            }

            currentWheel++;
            //w.wheel.localPosition = lp;


        }

        if (motorizedWheels > 1)
        {
            rpm = rpm / motorizedWheels;
        }


        motorRPM = 0.95f * motorRPM + 0.05f * Mathf.Abs(rpm * carSetting.gears[currentGear]);
        if (motorRPM > 5500.0f) motorRPM = 5200.0f;


        int index = (int)(motorRPM / efficiencyTableStep);
        if (index >= efficiencyTable.Length) index = efficiencyTable.Length - 1;
        if (index < 0) index = 0;



        float newTorque = curTorque * carSetting.gears[currentGear] * efficiencyTable[index];

        foreach (WheelComponent w in wheels)
        {
            WheelCollider col = w.collider;

            if (w.drive)
            {

                if (Mathf.Abs(col.rpm) > Mathf.Abs(wantedRPM))
                {

                    col.motorTorque = 0;
                }
                else
                {
                    // 
                    float curTorqueCol = col.motorTorque;

                    if (!brake && accel != 0 && NeutralGear == false)
                    {
                        if ((speed < carSetting.LimitForwardSpeed && currentGear > 0) ||
                            (speed < carSetting.LimitBackwardSpeed && currentGear == 0))
                        {

                            col.motorTorque = curTorqueCol * 0.9f + newTorque * 1.0f;
                        }
                        else
                        {
                            col.motorTorque = 0;
                            col.brakeTorque = 2000;
                        }


                    }
                    else
                    {
                        col.motorTorque = 0;

                    }
                }

            }





            if (brake || slip2 > 2.0f)
            {
                col.steerAngle = Mathf.Lerp(col.steerAngle, steer * w.maxSteer, 0.02f);
            }
            else
            {

                float SteerAngle = Mathf.Clamp(speed / carSetting.maxSteerAngle, 1.0f, carSetting.maxSteerAngle);
                col.steerAngle = steer * (w.maxSteer / SteerAngle);


            }

        }




        // calculate pitch (keep it within reasonable bounds)
        Pitch = Mathf.Clamp(1.2f + ((motorRPM - carSetting.idleRPM) / (carSetting.shiftUpRPM - carSetting.idleRPM)), 1.0f, 10.0f);

        shiftTime = Mathf.MoveTowards(shiftTime, 0.0f, 0.1f);

        if (Pitch == 1)
        {
            carSounds.IdleEngine.volume = Mathf.Lerp(carSounds.IdleEngine.volume, 1.0f, 0.1f);
            carSounds.LowEngine.volume = Mathf.Lerp(carSounds.LowEngine.volume, 0.5f, 0.1f);
            carSounds.HighEngine.volume = Mathf.Lerp(carSounds.HighEngine.volume, 0.0f, 0.1f);

        }
        else
        {

            carSounds.IdleEngine.volume = Mathf.Lerp(carSounds.IdleEngine.volume, 1.8f - Pitch, 0.1f);


            if ((Pitch > PitchDelay || accel > 0) && shiftTime == 0.0f)
            {
                carSounds.LowEngine.volume = Mathf.Lerp(carSounds.LowEngine.volume, 0.0f, 0.2f);
                carSounds.HighEngine.volume = Mathf.Lerp(carSounds.HighEngine.volume, 1.0f, 0.1f);
            }
            else
            {
                carSounds.LowEngine.volume = Mathf.Lerp(carSounds.LowEngine.volume, 0.5f, 0.1f);
                carSounds.HighEngine.volume = Mathf.Lerp(carSounds.HighEngine.volume, 0.0f, 0.2f);
            }




            carSounds.HighEngine.pitch = Pitch;
            carSounds.LowEngine.pitch = Pitch;

            PitchDelay = Pitch;
        }

    }


    public void StopController()
    {
        activeControl = false;
    }

    public void ActivateController()
    {
        activeControl = true;
    }



    /////////////// Show Normal Gizmos ////////////////////////////

    void OnDrawGizmos()
    {

        if (!carSetting.showNormalGizmos || Application.isPlaying) return;

        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);

        Gizmos.matrix = rotationMatrix;
        Gizmos.color = new Color(1, 0, 0, 0.5f);

        Gizmos.DrawCube(Vector3.up / 1.5f, new Vector3(2.5f, 2.0f, 6));
        Gizmos.DrawSphere(carSetting.shiftCentre / transform.lossyScale.x, 0.2f);

    }


    ////////////////////// Touch Controls //////////////////////////////
    #region Android
    public void CarAccelForward(float amount)
    {
        accelFwd = amount;
    }

    public void CarAccelBack(float amount)
    {
        accelBack = amount;
    }

    public void CarSteer(float amount)
    {
        steerAmount = amount;
    }

    public void CarHandBrake(bool HBrakeing)
    {
        brake = HBrakeing;
    }

    public void CarShift(bool Shifting)
    {
        shift = Shifting;
    }

    #endregion

}