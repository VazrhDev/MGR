using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MGR
{
    public class TrackCheckpoint : MonoBehaviour
    {
        public int checkPointNum = 0;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            // Update Checkpoint number 
            if (other.GetComponentInParent<PlayerRaceData>().NextCheckpoint == checkPointNum)
            {
                other.GetComponentInParent<PlayerRaceData>().UpdateCheckpoint();
            }
        }
    }

}