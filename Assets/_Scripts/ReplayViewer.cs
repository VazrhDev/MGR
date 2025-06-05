using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGR
{
    public class ReplayViewer : MonoBehaviour
    {
        float currentReplayIndex;
        float indexChangeRate = 1f;
        float replaySpeed = 1f;
        public ReplayDataObject replayDataObject;

        private void Start()
        {
            ReplayViewManager.instance.onForwardClick += replayForward;
            ReplayViewManager.instance.onReverseClick += replayRevese;
            ReplayViewManager.instance.onChangeSpeed += speedOfReplay;
            ReplayViewManager.instance.onPausePlayClick += PausePLay;
            indexChangeRate = 1f;
        }

        private void FixedUpdate()
        {
            float nextIndex = currentReplayIndex + indexChangeRate;

            if (nextIndex < replayDataObject.replayDatas.Count && nextIndex >= 0)
            {
                setTransform(nextIndex);
            }
        }

        void setTransform(float index)
        {
            currentReplayIndex = index;

            ReplayData replayData = replayDataObject.replayDatas[(int)index];

            transform.position = replayData.position;
            transform.rotation = replayData.rotation;
        }

        public void speedOfReplay(float speed)
        {
            if (speed == 0)
            {
                replaySpeed = 0.5f;
            }
            else
            {
                replaySpeed = speed;
            }
            if (indexChangeRate > 0)
            {
                replayForward();
            }
            else if(indexChangeRate < 0)
            {   
                replayRevese();
            }
        }

        public void PausePLay()
        {
            if (indexChangeRate == 0)
            {
                indexChangeRate = 1 * replaySpeed;
            }
            else
            {
                indexChangeRate = 0;
            }
        }

        public void replayForward()
        {
            indexChangeRate = 1 * replaySpeed;
        }
        public void replayRevese()
        {
            indexChangeRate = -1 * replaySpeed;
        }

        private void OnDestroy()
        {
            ReplayViewManager.instance.onForwardClick -= replayForward;
            ReplayViewManager.instance.onReverseClick -= replayRevese;
            ReplayViewManager.instance.onChangeSpeed -= speedOfReplay;
            ReplayViewManager.instance.onPausePlayClick -= PausePLay;
        }
    }

}