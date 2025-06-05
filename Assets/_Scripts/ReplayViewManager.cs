using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MGR
{
    public class ReplayViewManager : MonoBehaviour
    {
        public static ReplayViewManager instance;

        [SerializeField]GameObject Car;
        [SerializeField]CameraChange CameraChange;

        private void Awake()
        {
            instance = this;
            onChangeSpeedValue(1f);
            Invoke(nameof(SpawnReplayObjects), 2f);
        }
        //private void Start()
        //{
        //    SpawnReplayObjects();
        //}

        public event Action onForwardClick;
        public void onForwardClickEnter()
        {
            if (onForwardClick != null)
            {
                onForwardClick();
            }
        }
        public event Action onReverseClick;
        public void onReverseClickEnter()
        {
            if (onReverseClick != null)
            {
                onReverseClick();
            }
        }
        public event Action<float> onChangeSpeed;
        public void onChangeSpeedValue(float value)
        {
            if (onChangeSpeed != null)
            {
                onChangeSpeed(value);
            }
        }

        public event Action onPausePlayClick;
        public void onPausePlayClickEnter()
        {
            if(onPausePlayClick != null)
            {
                onPausePlayClick();
            }
        }

        [SerializeField] Slider speedSlider;

        [SerializeField] TMP_Text value;

        private void Update()
        {
            if(speedSlider.value == 0)
            {
                value.text = "0.5";
            }
            else
            {
                value.text = speedSlider.value.ToString("0.0");
            }
        }

        void SpawnReplayObjects()
        {
            Debug.Log(ReplayManager.instance.GetReplayDataObjectsSize());
            for(int i = 0; i < ReplayManager.instance.GetReplayDataObjectsSize(); i++)
            {
                var pawn = Instantiate(Car,Vector3.zero,Quaternion.identity);
                //Debug.Log(pawn);
                Car = pawn;
                pawn.GetComponent<ReplayViewer>().replayDataObject = ReplayManager.instance.GetReplayDataObject(i);
            }
        }
    }
}