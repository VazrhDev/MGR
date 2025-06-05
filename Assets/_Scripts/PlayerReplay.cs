using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace MGR
{
    public class PlayerReplay : MonoBehaviour
    {
        [SerializeField]ReplayDataObject replayDataObject;
        bool goOut = false;


        private void Awake()
        {
            Invoke(nameof(EnableScript), 0.05f);
        }

        void EnableScript()
        {
            if (!enabled)
                this.enabled = true;
            Invoke(nameof(EnableScript), 1);
        }

        private void Start()
        {
            replayDataObject.name = gameObject.GetPhotonView().Owner.NickName;
            //Debug.Log(gameObject.GetPhotonView().Owner.NickName);

        }

        private void FixedUpdate()
        {
            if (RaceManager.Instance.RaceStarted)
            {
                replayDataObject.replayDatas.Add(new ReplayData { position = transform.position, rotation = transform.rotation });
            }
            if(RaceManager.Instance.RaceEnded && !RaceManager.Instance.RaceStarted && !goOut)
            {
                ReplayManager.instance.AddReplayDataObject(replayDataObject);
                goOut = true;
            }
        }
    }
}