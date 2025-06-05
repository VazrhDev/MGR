using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MGR
{

    internal enum CarDriveType
    {
        FourWheelDrive,
        FrontWheelDrive,
        rearWheelDrive
    }

    public class CarMovementController : MonoBehaviour
    {
        [Header("Vehicle Data")]
        [SerializeField] private CarDriveType carDriveType = CarDriveType.FourWheelDrive;
        [SerializeField] private Vector3 centerOfMassOffset = new Vector3(0, 0, 0);
        [SerializeField] private float downforce = 200;
        private Rigidbody carRigidbody;


        [Header("Engine")]
        [SerializeField] public AnimationCurve enginePower;        // Calculates Torque based on engineRPM - Horsepower/Torque
        [SerializeField] private float acceleration = 750;
        [SerializeField] private float deacceleration = 500;
        [SerializeField] private float topSpeed = 250;
        [SerializeField] public float engineLife = 100;
        [SerializeField] private float maxRPM = 5600;
        public float MaxRPM { get { return maxRPM; } }
        [SerializeField] private float minRPM = 3000;
        [SerializeField] private float smoothTime = 0.01f;
        [SerializeField] private float[] gears;
        [SerializeField] private float[] gearChangeSpeed;
        [SerializeField] private int gearNum = 0;
        public int GearNum { get { return gearNum; } }
        [SerializeField] private float currentSpeed;
        public float CurrentSpeed { get { return currentSpeed; } }

        private float torque = 2500f;
        private float engineRPM;
        private bool reverse = false;
        public bool Reverse { get { return reverse; } }
        public float EngineRPM { get { return engineRPM; } }
        private bool IsMaxedRPM = false;


        [Header("Wheels")]
        [SerializeField] private WheelCollider[] wheelColliders;
        [SerializeField] private Transform[] wheelMeshes;
        [SerializeField] private float brakeForce = 500;            // Force applied when using normal brake
        [SerializeField] private float handBrakeFrictionMultiplier = 2f;
        [SerializeField] private float frictionMultiplier = 2f;
        [SerializeField] private float handBrakeSmoothFactor = 1.5f;    // Smooth factor for car to back to normal after handbrake    
        [SerializeField] public float steering = 6;                // Vehicle's default steering angle / higher value for slow steering
        private float steeringRadius = 6;
        private WheelFrictionCurve forwardFriction, sidewaysFriction;
        private float brakePower = 500f;
        private float wheelsRPM;
        private float driftFactor;

        [Header("Data")]
        private float sportiveness;
        private float fuelEfficiency;
        private float stability;


        [Header("Nitro")]
        [SerializeField] private bool canUseNitro = true;
        [SerializeField] private float maxNitroValue = 10.0f;       // Total Nitro Value
        [SerializeField] public float NitroForce = 5000.0f;        // Force that is applied when using nitro
        private float boostValue = 10.0f;
        public float BoostValue { get { return boostValue; } }

        [Header("Photon Networking")]
        [SerializeField] private PhotonView photonView;

        [Header("Car UI")]
        [SerializeField] CarUIManager carUIManager;

        [Header("Camera")]
        [SerializeField] private GameObject FPCameraLookObject;
        public GameObject FPCameraLook { get { return FPCameraLookObject; } }

        [Header("Model")]
        [SerializeField] private GameObject carModelHolder;
        public GameObject CarModelHolder { get { return carModelHolder; } }

        // Post Processing Data
        //public Volume postProcessVolume;
        //private ChromaticAberration ca;
        //private MotionBlur mb;
        //private DepthOfField dof;


        private void Awake()
        {
            StartCoroutine(AdjustSteeringRadius());
        }

        void Start()
        {
            carRigidbody = GetComponent<Rigidbody>();

            carRigidbody.centerOfMass = centerOfMassOffset;

            if (canUseNitro)
            {
                boostValue = maxNitroValue;
            }

            // Find Gameobject that contains wheel colliders
            GameObject wheelCollidersObject = GameObject.Find("WheelColliders");

            if (wheelCollidersObject != null)
            {
                wheelColliders = new WheelCollider[4];

                for (int i = 0; i < wheelCollidersObject.transform.childCount; i++)
                {
                    wheelColliders[i] = wheelCollidersObject.transform.GetChild(i).GetComponent<WheelCollider>();

                    WheelFrictionCurve sidewaysFrictionCurve = wheelColliders[i].sidewaysFriction;
                    sidewaysFrictionCurve.stiffness = 3.0f;
                    wheelColliders[i].sidewaysFriction = sidewaysFrictionCurve;

                    WheelFrictionCurve forwardFrictionCurve = wheelColliders[i].forwardFriction;
                    forwardFrictionCurve.stiffness = 1.5f;
                    wheelColliders[i].forwardFriction = forwardFrictionCurve;
                }
            }

            // Gameobject that contains wheel meshes
            GameObject wheelMeshesObject = GameObject.Find("WheelMeshes");

            if (wheelMeshesObject != null)
            {
                wheelMeshes = new Transform[4];

                for (int i = 0; i < wheelMeshesObject.transform.childCount; i++)
                {
                    wheelMeshes[i] = wheelMeshesObject.transform.GetChild(i);
                }
            }

            SetCarData();

            // Deactivate UI if it's not owner / Stopping UI from replicating
            if (!photonView)
                return;
            if (!photonView.IsMine)
            {
                carUIManager.gameObject.SetActive(false);
            }

            InvokeRepeating(nameof(CheckIfFlipped), 2f, 2f);
        }

        void SetCarData()
        {
            return;

            CarData carData = GameManager.instance.currentCarData;

            // Set Top Speed
            float totalTopSpeed = carData.engineDetails.engineData.topSpeed + carData.compoundsDetails.compoundData.topSpeed + carData.transmissionDetails.transmissionData.topSpeed + carData.suspensionDetails.suspensionData.topSpeed;

            float speedMargin = (totalTopSpeed / 10) * (DatabaseHandler.instance.masterData.maxTopSpeed - DatabaseHandler.instance.masterData.minTopSpeed);

            topSpeed = DatabaseHandler.instance.masterData.minTopSpeed + speedMargin;

            // Set Acceleration
            float totalAcceleration = carData.engineDetails.engineData.acceleration + carData.transmissionDetails.transmissionData.acceleration + carData.compoundsDetails.compoundData.acceleration + carData.framesDetails.frameData.acceleration + carData.suspensionDetails.suspensionData.acceleration;

            float accMargin = (totalAcceleration / 10) * (DatabaseHandler.instance.masterData.maxAcceleration - DatabaseHandler.instance.masterData.minAcceleration);

            acceleration = accMargin + DatabaseHandler.instance.masterData.minAcceleration;

            // Set Handling
            float totalHandling = carData.steeringDetails.steeringData.handling;

            steering = DatabaseHandler.instance.masterData.maxHandling - totalHandling;

            // Set Weight Data
            float totalWeight = carData.framesDetails.frameData.weight + carData.wheelsDetails.wheelData.weight;

            carRigidbody.mass = (totalWeight / 10) * DatabaseHandler.instance.masterData.maxWeight;

            // Set Sportiveness Data
            float totalSportiveness = carData.suspensionDetails.suspensionData.sportiveness + carData.compoundsDetails.compoundData.sportiveness + carData.wheelsDetails.wheelData.sportiveness;

            sportiveness = (totalSportiveness / 10) * DatabaseHandler.instance.masterData.maxSportiveness;

            // Set Fuel Efficiency Data
            float totalFE = carData.transmissionDetails.transmissionData.fuelEfficiency + carData.framesDetails.frameData.fuelEfficiency + carData.brakesDetails.brakeData.fuelEfficiency;

            fuelEfficiency = (totalFE / 10) * DatabaseHandler.instance.masterData.maxFuelEfficiency;

            // Set Stability Data
            float totalStability = carData.compoundsDetails.compoundData.stability + carData.suspensionDetails.suspensionData.stability + carData.steeringDetails.steeringData.stability;

            stability = (totalStability / 10) * DatabaseHandler.instance.masterData.maxStability;

            // Set Brake Data
            float totalBrakeForce = carData.brakesDetails.brakeData.deceleration + carData.compoundsDetails.compoundData.deceleration + carData.wheelsDetails.wheelData.deceleration;

            brakeForce = (totalBrakeForce / 10) * DatabaseHandler.instance.masterData.maxDeceleration;


            // Set Nitro Data
            NitroForce = carData.nitroDetails.nitroData.accelerationOnUsing;
            //maxNitroValue = carData.nitroDetails.nitroData.duration;

            //Set Wheel Data
            //for (int i = 0; i < wheelColliders.Length; i++)
            //{
            //    wheelColliders[i].mass = carData.wheelsDetails.wheelData.weight;
            //}


            // Setting post processing data
            //try
            //{
            //    postProcessVolume.profile.TryGet(out ca);
            //    postProcessVolume.profile.TryGet(out mb);
            //    postProcessVolume.profile.TryGet(out dof);
            //}
            //catch
            //{
            //    Debug.LogError("Error Trying to get post process data from profile");
            //}
        }

        void CheckIfFlipped()
        {
            if (Vector3.Dot(transform.up, Vector3.down) > 0)
            {
                // Car's roof is pointing slightly downwards if vector > 0, 1 if it's upsidedown 

                transform.rotation = Quaternion.Euler(0f, transform.rotation.y, 0f);
            }
        }

        public void Move(float _vertical, float _horizontal, bool _handbrake, bool _usingNitro)
        {
            if (!photonView.IsMine)
                return;

            if (!carRigidbody)
                return;

            AddDownforce();
            SteerVehicle(_horizontal);
            CalculateEnginePower(_vertical);
            //GearShifter();
            AdjustTraction(_horizontal, _handbrake);
            RotateWheels();

            BrakeVehicle(_vertical);


            switch (carDriveType)
            {
                case CarDriveType.FourWheelDrive:
                    for (int i = 0; i < wheelColliders.Length; i++)
                    {
                        wheelColliders[i].motorTorque = (torque / 4f);
                        wheelColliders[i].brakeTorque = brakePower;
                    }
                    break;

                case CarDriveType.rearWheelDrive:
                    wheelColliders[2].motorTorque = torque / 2;
                    wheelColliders[3].motorTorque = torque / 2;
                    for (int i = 0; i < wheelColliders.Length; i++)
                    {
                        wheelColliders[i].brakeTorque = brakePower;
                    }
                    break;

                case CarDriveType.FrontWheelDrive:
                    wheelColliders[0].motorTorque = torque / 2;
                    wheelColliders[1].motorTorque = torque / 2;
                    for (int i = 0; i < wheelColliders.Length; i++)
                    {
                        wheelColliders[i].brakeTorque = brakePower;
                    }
                    break;
            }

            currentSpeed = carRigidbody.velocity.magnitude * 3.6f;
            currentSpeed = Mathf.Clamp(currentSpeed, 0, topSpeed);

            wheelColliders[2].brakeTorque = wheelColliders[3].brakeTorque = brakePower;


            if (canUseNitro)
            {
                ActivateNitro(_usingNitro);
            }
        }

        private void BrakeVehicle(float _vertical)
        {
            if (_vertical < 0 && !reverse)
            {
                brakePower = (currentSpeed >= 10) ? brakeForce : 0;
            }
            else if (_vertical == 0 && (currentSpeed <= 10 || currentSpeed >= -10))
            {
                brakePower = 10;
            }
            else
            {
                brakePower = 0;
            }
        }

        private void SteerVehicle(float _horizontal)
        {
            // Ackermann steering

            //rear tracks size is set to 1.5f       wheel base has been set to 2.55f
            if (_horizontal > 0)
            {
                wheelColliders[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (steeringRadius + (1.5f / 2))) * _horizontal;
                wheelColliders[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (steeringRadius - (1.5f / 2))) * _horizontal;
            }
            else if (_horizontal < 0)
            {
                wheelColliders[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (steeringRadius - (1.5f / 2))) * _horizontal;
                wheelColliders[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (steeringRadius + (1.5f / 2))) * _horizontal;
            }
            else
            {
                wheelColliders[0].steerAngle = 0;
                wheelColliders[1].steerAngle = 0;
            }
        }


        private void RotateWheels()
        {
            // Return if there's no wheel mesh
            if (wheelMeshes.Length <= 0)
                return;


            // Add Rotation and position to wheel mesh
            Vector3 wheelPosition = Vector3.zero;
            Quaternion wheelRotation = Quaternion.identity;

            for (int i = 0; i < wheelColliders.Length; i++)
            {
                wheelColliders[i].GetWorldPose(out wheelPosition, out wheelRotation);

                //wheelMeshes[i].transform.position = wheelPosition;
                wheelMeshes[i].transform.rotation = wheelRotation;
            }
        }

        private void AddDownforce()
        {
            carRigidbody.AddForce(-transform.up * downforce * carRigidbody.velocity.magnitude);
        }

        private void CalculateEnginePower(float _vertical)
        {
            WheelRPM();

            if (_vertical != 0)
            {
                carRigidbody.drag = 0.005f;
            }
            if (_vertical == 0)
            {
                carRigidbody.drag = 0.1f;
            }

            // Stop Calculating torque if car's speed exceeds the topspeed && trying to accelerate
            if (currentSpeed >= topSpeed && _vertical > 0)
            {
                return;
            }

            //torque = 3.6f * enginePower.Evaluate(engineRPM) * (_vertical);
            if (_vertical >= 0)
            {
                torque = 3.6f * acceleration * (_vertical);
            }
            else
            {
                torque = 3.6f * deacceleration * (_vertical);
            }

            float velocity = 0.0f;
            if (engineRPM >= maxRPM || IsMaxedRPM)
            {
                engineRPM = Mathf.SmoothDamp(engineRPM, maxRPM - 500, ref velocity, 0.05f);

                IsMaxedRPM = (engineRPM >= maxRPM - 450) ? true : false;
            }
            else
            {
                engineRPM = Mathf.SmoothDamp(engineRPM, 1000 + (Mathf.Abs(wheelsRPM) * 3.6f * (gears[gearNum])), ref velocity, smoothTime);
            }
            if (engineRPM >= maxRPM + 1000) engineRPM = maxRPM + 1000; // clamp at max
            GearShifter();
        }

        private void WheelRPM()
        {
            float sum = 0;
            int r = 0;

            for (int i = 0; i < 4; i++)
            {
                sum += wheelColliders[i].rpm;

                r++;
            }

            if (carRigidbody.velocity.magnitude == 0)
            {
                sum = 0;
            }
            wheelsRPM = (r != 0) ? sum / r : 0;
            if (wheelsRPM < 0 && !reverse)
            {
                reverse = true;
                carUIManager.changeGear();
            }
            else if (wheelsRPM > 0 && reverse)
            {
                reverse = false;
                carUIManager.changeGear();
            }
        }

        private bool CheckGears()
        {
            if (currentSpeed >= gearChangeSpeed[gearNum]) return true;
            else return false;
        }

        private void GearShifter()
        {
            if (!IsGrounded())
                return;

            if (engineRPM > maxRPM && gearNum < gears.Length - 1 && !reverse && CheckGears())
            {
                gearNum++;
                carUIManager.changeGear();
            }

            if (engineRPM < minRPM && gearNum > 0)
            {
                gearNum--;
                carUIManager.changeGear();
            }
        }

        private bool IsGrounded()
        {
            if (wheelColliders[0].isGrounded && wheelColliders[1].isGrounded && wheelColliders[2].isGrounded && wheelColliders[3].isGrounded)
                return true;
            else
                return false;
        }

        private void AdjustTraction(float _horizontal, bool _handbrake)
        {
            //time it takes to go from normal drive to drift 
            float driftSmoothFactor = handBrakeSmoothFactor * Time.deltaTime;

            //executed when handbrake is being held
            if (_handbrake)
            {
                sidewaysFriction = wheelColliders[0].sidewaysFriction;
                forwardFriction = wheelColliders[0].forwardFriction;

                float velocity = 0;
                sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = forwardFriction.extremumValue = forwardFriction.asymptoteValue =
                    Mathf.SmoothDamp(forwardFriction.asymptoteValue, driftFactor * handBrakeFrictionMultiplier * frictionMultiplier, ref velocity, driftSmoothFactor);


                for (int i = 0; i < 4; i++)
                {
                    wheelColliders[i].sidewaysFriction = sidewaysFriction;
                    wheelColliders[i].forwardFriction = forwardFriction;
                }

                sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = forwardFriction.extremumValue = forwardFriction.asymptoteValue = 1.1f;
                //extra grip for the front wheels
                for (int i = 0; i < 2; i++)
                {
                    wheelColliders[i].sidewaysFriction = sidewaysFriction;
                    wheelColliders[i].forwardFriction = forwardFriction;
                }
                carRigidbody.AddForce(transform.forward * (currentSpeed / 400) * 10000);
            }

            //executed when handbrake is not being held
            else
            {
                forwardFriction = wheelColliders[0].forwardFriction;
                sidewaysFriction = wheelColliders[0].sidewaysFriction;

                forwardFriction.extremumValue = forwardFriction.asymptoteValue = sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue =
                    ((currentSpeed * handBrakeFrictionMultiplier) / 300) + 1 * frictionMultiplier;


                for (int i = 0; i < 4; i++)
                {
                    wheelColliders[i].forwardFriction = forwardFriction;
                    wheelColliders[i].sidewaysFriction = sidewaysFriction;

                }
            }

            //checks the amount of slip to control the drift
            for (int i = 2; i < 4; i++)
            {

                WheelHit wheelHit;

                wheelColliders[i].GetGroundHit(out wheelHit);

                if (wheelHit.sidewaysSlip < 0) driftFactor = (1 - _horizontal) * Mathf.Abs(wheelHit.sidewaysSlip);

                if (wheelHit.sidewaysSlip > 0) driftFactor = (1 + _horizontal) * Mathf.Abs(wheelHit.sidewaysSlip);
            }

        }

        private IEnumerator AdjustSteeringRadius()
        {
            while (true)
            {
                yield return new WaitForSeconds(.7f);
                steeringRadius = steering + currentSpeed / 20;
            }
        }

        public void ActivateNitro(bool _usingNitro)
        {
            // Return if not using nitro and it's completely recharged
            if (!_usingNitro && boostValue >= maxNitroValue)
                return;

            // Recharge nitro value if it's not full and not using nitro
            if (!_usingNitro && boostValue <= maxNitroValue)
            {
                boostValue += Time.deltaTime / 2;

                if (boostValue >= maxNitroValue)
                {
                    boostValue = maxNitroValue;
                }

                //if (ca.active)
                //    ca.active = false;

                //if (mb.active)
                //    mb.active = false;
            }
            // Decrease nitro value when using the nitro
            else
            {
                boostValue -= (boostValue <= 0) ? 0 : Time.deltaTime;

                //if (!ca.active)
                //    ca.active = true;

                //if (!mb.active)
                //    mb.active = true;
            }

            // Add force to car when using nitro
            if (_usingNitro)
            {
                if (boostValue > 0 && currentSpeed < topSpeed)
                {
                    carRigidbody.AddForce(transform.forward * NitroForce);
                }
            }
        }
    }
}
