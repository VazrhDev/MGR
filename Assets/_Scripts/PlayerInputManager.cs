using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGR
{
    public class PlayerInputManager : MonoBehaviour
    {
        private float vertical;
        public float Vertical { get { return vertical; } }
        private float horizontal;
        public float Horizontal { get { return horizontal; } }
        private bool handbrake;
        private bool isUsingNitro;

        CarMovementController carMovementController;

        //public bool IsRaceFinished = false;

        private void Awake()
        {
            carMovementController = GetComponent<CarMovementController>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            
        }

        private void FixedUpdate()
        {
            //if (IsRaceFinished)
            //    return;

            vertical = Input.GetAxis("Vertical");
            horizontal = Input.GetAxis("Horizontal");
            handbrake = (Input.GetAxis("Jump") != 0) ? true : false;

            isUsingNitro = (Input.GetKey(KeyCode.LeftShift)) ? true : false;

            carMovementController.Move(vertical, horizontal, handbrake, isUsingNitro);
        }
    }
}