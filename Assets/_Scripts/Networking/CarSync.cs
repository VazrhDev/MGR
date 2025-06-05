using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

namespace MGR
{
    public class CarSync : MonoBehaviourPun, IPunObservable
    {
        [SerializeField] private Rigidbody carRigidbody;
        [SerializeField] private Transform[] wheels;

        // Values that are needed to sync over the network
        private Vector3 latestPos;
        private Quaternion latestRot;
        private Vector3 latestVelocity;
        private Vector3 latestAngularVelocity;
        private Quaternion[] wheelRotations = new Quaternion[0];

        // Lag Compensation
        private float currentTime = 0;
        private double currentPacketTime = 0;
        private double lastPacketTime = 0;
        private Vector3 positionAtLastPacket = Vector3.zero;
        private Quaternion rotationAtLastPacket = Quaternion.identity;
        private Vector3 velocityAtLastPacket = Vector3.zero;
        private Vector3 angularVelocityAtLastPacket = Vector3.zero;

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
                stream.SendNext(carRigidbody.velocity);
                stream.SendNext(carRigidbody.angularVelocity);

                wheelRotations = new Quaternion[wheels.Length];
                for (int i = 0; i < wheels.Length; i++)
                {
                    wheelRotations[i] = wheels[i].localRotation;
                }
                stream.SendNext(wheelRotations);
            }
            else
            {
                // Network player, receive data
                latestPos = (Vector3)stream.ReceiveNext();
                latestRot = (Quaternion)stream.ReceiveNext();
                latestVelocity = (Vector3)stream.ReceiveNext();
                latestAngularVelocity = (Vector3)stream.ReceiveNext();
                wheelRotations = (Quaternion[])stream.ReceiveNext();

                // Lag compensation
                currentTime = 0.0f;
                lastPacketTime = currentPacketTime;
                currentPacketTime = info.SentServerTime;
                positionAtLastPacket = transform.position;
                rotationAtLastPacket = transform.rotation;
                velocityAtLastPacket = carRigidbody.velocity;
                angularVelocityAtLastPacket = carRigidbody.angularVelocity;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            if (!photonView.IsMine)
            {
                MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();

                for (int i = 0; i < scripts.Length; i++)
                {
                    if (scripts[i] is CarSync) continue;
                    else if (scripts[i] is PhotonView) continue;

                    scripts[i].enabled = false;
                }
            }

            // Find Gameobject that contails wheel colliders
            GameObject wheelCollidersObject = GameObject.Find("WheelColliders");

            if (wheelCollidersObject != null)
            {
                wheels = new Transform[4];

                for (int i = 0; i < wheelCollidersObject.transform.childCount; i++)
                {
                    wheels[i] = wheelCollidersObject.transform.GetChild(i);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!photonView.IsMine)
            {
                // Lag compensation
                double timeToReachGoal = currentPacketTime - lastPacketTime;
                currentTime += Time.deltaTime;

                // Update car position and velocity
                transform.position = Vector3.Lerp(positionAtLastPacket, latestPos, (float)(currentTime / timeToReachGoal));
                transform.rotation = Quaternion.Lerp(rotationAtLastPacket, latestRot, (float)(currentTime / timeToReachGoal));
                carRigidbody.velocity = Vector3.Lerp(velocityAtLastPacket, latestVelocity, (float)(currentTime / timeToReachGoal));
                carRigidbody.angularVelocity = Vector3.Lerp(angularVelocityAtLastPacket, latestAngularVelocity, (float)(currentTime / timeToReachGoal));

                //Apply wheel rotation
                if (wheelRotations.Length == wheels.Length)
                {
                    for (int i = 0; i < wheelRotations.Length; i++)
                    {
                        wheels[i].localRotation = Quaternion.Lerp(wheels[i].localRotation, wheelRotations[i], Time.deltaTime * 6.5f);
                    }
                }
            }
        }
    }
}