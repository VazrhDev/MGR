using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGR
{
    [Serializable]
    public class ReplayData
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    [Serializable]
    public class ReplayDataObject
    {
        public string name;
        public List<ReplayData> replayDatas = new List<ReplayData>();
    }

}
