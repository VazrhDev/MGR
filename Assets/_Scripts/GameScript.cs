using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGR
{
    public class GameScript : MonoBehaviour
    {
        public static GameScript instance;

        [SerializeField] Continue continueScript;
        [SerializeField] GameObject[] cars;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }
    }
}
