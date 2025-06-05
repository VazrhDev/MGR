using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGR
{
    public class MiniMapCamera : MonoBehaviour
    {
        private Transform playerTransform = null;
        [SerializeField] private float xOffset = 0;
        [SerializeField] private float zOffset = 0;
        [SerializeField] private float yOffset = 0;

        [SerializeField] private float xRot = -20.0f;
        [SerializeField] private float yRot= 0;
        [SerializeField] private float zRot = 0;


        // Start is called before the first frame update
        void Start()
        {
            //for (int i = 0; i < RaceManager.Instance.CheckpointsCount; i++)
            //{
            //    lineRenderer.positionCount += 1;
            //    lineRenderer.SetPosition(i, RaceManager.Instance.GetCheckpointPosition(i));
            //}
        }

        // Update is called once per frame
        void Update()
        {
            if (playerTransform == null)
                return;

            transform.position = new Vector3(playerTransform.position.x + xOffset, transform.position.y, playerTransform.position.z + zOffset);

            transform.rotation = Quaternion.Euler(90 + xRot, playerTransform.eulerAngles.y + yRot, 0 + zRot);
        }

        public void SetPlayerCar(Transform _PlayerTransform)
        {
            playerTransform = _PlayerTransform;
        }
    }
}