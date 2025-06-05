using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGR
{
    public class ReplayManager : MonoBehaviour
    {
        public static ReplayManager instance;
        [SerializeField] List<ReplayDataObject> replayDataObjects = new List<ReplayDataObject>();

        private void Awake()
        {
            if (instance == null) instance = this;
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }

        public void AddReplayDataObject(ReplayDataObject replayDataObject)
        {
            replayDataObjects.Add(replayDataObject);
        }

        public int GetReplayDataObjectsSize()
        {
            return replayDataObjects.Count;
        }

        public ReplayDataObject GetReplayDataObject(int index)
        {
            return replayDataObjects[index];
        }
    }
}